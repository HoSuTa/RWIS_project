let gssKey = 'YOUR KEY CHANGE THIS';
let sheetName = 'sheet';
// Constants used and shared within GAS and Unity.
const PAYLOAD_CONSTS = {
  Payload: "actualPayload",
  UserId: "userId", //An id that will probably always exist in any data design.
  UserName: "userName",
  Message: "message",
  Time: "updateTime",
  Method : "method",
  SaveDataMethod: "SaveUserData",
  GetDatasMethod: "GetUserDatas",
  GetUserNamesMethod: "GetUserNames"
};
const UpdateTimeColumn = 0;
const UserIdColumn     = 1;
const UserNameColumn = 2;
const MessageColumn    = 3;

function getSheet(key, name){
  const gssSheet = SpreadsheetApp.openById(key).getSheetByName(name);
  if(gssSheet == null) {
    return ContentService.createTextOutput("Error: Invalid sheet name.");
  }
  return gssSheet;
}

function getUserNames(){
  const gssSheet = getSheet(gssKey, sheetName);
  const sheetData = gssSheet.getDataRange().getValues();

  userIdsData = {
    [PAYLOAD_CONSTS.Payload] : findUserNames(sheetData)
  }
  sendingBackPayload = ContentService.createTextOutput(JSON.stringify(userIdsData));
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}



function findUserNames(sheetData)
{
  let userIdsSet = new Set();
  //from 1 to skip the header row.
  for(let i = 1; i < sheetData.length; i++){
    userIdsSet.add(sheetData[i][UserNameColumn]);
  }

  let userIdsArr = [];
  for(let userId of userIdsSet)
  {
    let userIdElement = new Object();
    userIdElement[PAYLOAD_CONSTS.UserName] = userId;
    userIdsArr.push(userIdElement);
  }
  return userIdsArr;
}



function getUserDatas(UserName){
  const gssSheet = getSheet(gssKey, sheetName);
  const sheetData = gssSheet.getDataRange().getValues();
  const sheetHeader = sheetData[0];
  
  const userId = findUserId(sheetData, UserName);
  const user_rows = findUserRowsByUserId(sheetData, userId);

  if(user_rows.length == 0){
    Logger.log(`Error: \"${UserName}\" does not exist in the sheet.`);

    return ContentService.createTextOutput(`Error: \"${UserName}\" does not exist in the sheet.`);
  }

  let returning_datas = {};
  let datas = [];
  for(let i = 0; i < user_rows.length; i++)
  {
    data = new Object();
    for(let j = 2; j < sheetHeader.length; j++){
      data[sheetHeader[j]]= sheetData[user_rows[i]][j] ;
    }
    datas[i] = data;
  }
  returning_datas[PAYLOAD_CONSTS.Payload] = datas;

  const sendingBackPayload = ContentService.createTextOutput(JSON.stringify(returning_datas));
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function findUserRowsByUserId(sheetData, userId) {
  let rows = [];
  for(let i = 1; i < sheetData.length; i++){
    if(sheetData[i][UserIdColumn] == userId){
      rows.push(i);
    }
  }
  return rows;
}



// GAS's event function that will be called when https GET is requested.
function doGet(e){
  //e == null is just for debugging in GAS.
  //doesn't really matter with the actual function of codes.
  const request = (e == null) ? generateDebugObjectForGET() : e.parameter;

  if(request == null ){
    Logger.log("Error: payload was empty.");
    return ContentService.createTextOutput("Error: payload was empty.");
  }
  
  if(request[PAYLOAD_CONSTS.Method] == PAYLOAD_CONSTS.GetDatasMethod){
    const UserName = request[PAYLOAD_CONSTS.UserName];
    return getUserDatas(UserName);
  }
  else if(request[PAYLOAD_CONSTS.Method] == PAYLOAD_CONSTS.GetUserNamesMethod){
    return getUserNames();
  }
}

function generateDebugObjectForGET(){
  //[variable], [] makes the variable to expand.
  const fakePayload = {
    [PAYLOAD_CONSTS.Method] : [PAYLOAD_CONSTS.GetDatasMethod],
    [PAYLOAD_CONSTS.UserName] : "tester",
  };
  return fakePayload;
}


// GAS's event function that will be called when https POST is requested.
function doPost(e){
  const request = (e == null) ? generateDebugObjectForPOST() : JSON.parse(e.postData.getDataAsString());

  if(request == null ){
    return ContentService.createTextOutput("Error: payload was empty.");
  }

  const gssSheet = getSheet(gssKey, sheetName);
  const sheetData = gssSheet.getDataRange().getValues();

  const currentTime = Utilities.formatDate(new Date(), "GMT+9", "yyyy/MM/dd HH:mm:ss");
  Logger.log(currentTime);

  const userId = findUserId(sheetData, request[PAYLOAD_CONSTS.UserName]);

  if(userId != null){
    const userRows = findUserRowsByUserId(sheetData, userId);

    for(let user_row of userRows){
      if(sheetData[user_row][MessageColumn] == request[PAYLOAD_CONSTS.Message]){
        gssSheet.getRange(++user_row, UpdateTimeColumn+1).setValue(currentTime);
        return ContentService.createTextOutput(`message=\"${request[PAYLOAD_CONSTS.Message]}\" existed in gss. Updated the value of updateTime.`);
      }
    }
  }

  let addingData = [];
  addingData[UserIdColumn] = (userId == null) ? findMaxUserId(sheetData) + 1 : userId;
  addingData[UpdateTimeColumn] = currentTime;
  addingData[UserNameColumn] = request[PAYLOAD_CONSTS.UserName];
  addingData[MessageColumn] = request[PAYLOAD_CONSTS.Message];
  gssSheet.appendRow(addingData);
  
  return ContentService.createTextOutput("Save data succeeded.");
}

function generateDebugObjectForPOST(){
  const fakePayload = {
    [PAYLOAD_CONSTS.Method] : [PAYLOAD_CONSTS.SaveDataMethod],
    [PAYLOAD_CONSTS.UserName] : "tester",
    [PAYLOAD_CONSTS.Message] : "tester Unity Post",
  };
  return fakePayload;
}

function findUserId(sheetData, UserName){
  for(let i = 1; i < sheetData.length; i++){
    if(sheetData[i][UserNameColumn] == UserName){
      return sheetData[i][UserIdColumn];
    }
  }
  return null;
}

function findMaxUserId(sheetData){
  let maxId = 0;
  for(let i = 1; i < sheetData.length; i++){
    if(maxId < sheetData[i][UserIdColumn]){
      maxId = sheetData[i][UserIdColumn];
    }
  }
  return maxId;
}

