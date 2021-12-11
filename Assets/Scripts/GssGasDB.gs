let gssKey = 'YOUR KEY CHANGE THIS';
let sheetName = 'sheet';
// Constants used and shared within GAS and Unity.
const CONSTS = {
  Payload: "actualPayload",
  UserId: "userId", //An id that will probably always exist in any data design.
  UserName: "userName",
  Message: "message",
  Time: "updateTime",
  Method : "method",
  SaveMessageMethod: "SaveMessage",
  SaveLonLatMethod: "SaveLonLat",
  RemoveDataMethod: "RemoveData",
  GetDatasMethod: "GetUserDatas",
  GetUserNamesMethod: "GetUserNames",
  UpdateTimeColumn : 0,
  UserIdColumn : 1,
  UserNameColumn : 2,
  MessageColumn : 3,
  AreaId : "areaId",
  VertexId : "vertexId",
  LonLat : "lonLat",
};

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
    [CONSTS.Payload] : findUserNames(sheetData)
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
    userIdsSet.add(sheetData[i][CONSTS.UserNameColumn]);
  }

  let userIdsArr = [];
  for(let userId of userIdsSet)
  {
    let userIdElement = new Object();
    userIdElement[CONSTS.UserName] = userId;
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
  returning_datas[CONSTS.Payload] = datas;

  const sendingBackPayload = ContentService.createTextOutput(JSON.stringify(returning_datas));
  Logger.log(sendingBackPayload.getContent());
  return sendingBackPayload;
}

function findUserRowsByUserId(sheetData, userId) {
  let rows = [];
  for(let i = 1; i < sheetData.length; i++){
    if(sheetData[i][CONSTS.UserIdColumn] == userId){
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
  
  if(request[CONSTS.Method] == CONSTS.GetDatasMethod){
    return getUserDatas(request[CONSTS.UserName]);
  }
  else if(request[CONSTS.Method] == CONSTS.GetUserNamesMethod){
    return getUserNames();
  }
}

function generateDebugObjectForGET(){
  //[variable], [] makes the variable to expand.
  const fakePayload = {
    [CONSTS.Method] : [CONSTS.GetDatasMethod],
    [CONSTS.UserName] : "tester",
  };
  return fakePayload;
}


// GAS's event function that will be called when https POST is requested.
function doPost(e){
  const request = (e == null) ? generateDebugObjectForPOST() : JSON.parse(e.postData.getDataAsString());
  //Logger.log(JSON.stringify(JSON.parse('{"userName":"aaa" , "message": { "vertexId": 0 } }')["message"]));

  if(request == null ){
    return ContentService.createTextOutput("Error: payload was empty.");
  }

  if(request[CONSTS.Method] == CONSTS.SaveMessageMethod){
    return saveMessage(request);
  }
  else if(request[CONSTS.Method] == CONSTS.RemoveDataMethod){
    return removeData(request);
  }
  else if(request[CONSTS.Method] == CONSTS.SaveLonLatMethod){
    return saveLonLat(request);
  }
}

function saveMessage(request){
  const gssSheet = getSheet(gssKey, sheetName);
  const sheetData = gssSheet.getDataRange().getValues();

  const currentTime = Utilities.formatDate(new Date(), "GMT+9", "yyyy/MM/dd HH:mm:ss");

  const userId = findUserId(sheetData, request[CONSTS.UserName]);
  const message = JSON.stringify(request[CONSTS.Message]);
  const areaId = request[CONSTS.Message][CONSTS.AreaId];
  const vertexId = request[CONSTS.Message][CONSTS.VertexId];
  //const lonLat = request[CONSTS.Message][CONSTS.LonLat];
  
  if(userId != null){
    const userRows = findUserRowsByUserId(sheetData, userId);

    for(let userRow of userRows){
      if(JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.AreaId] == areaId){
        if(JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.VertexId] == vertexId){
          gssSheet.getRange(1 + userRow, CONSTS.UpdateTimeColumn+1).setValue(currentTime);
          gssSheet.getRange(1 + userRow, CONSTS.MessageColumn   +1).setValue(message);
          return ContentService.createTextOutput(`areaId and vertexId already existed for userId. Updated lonLat`);
        }
      }
    }

  }

  let addingData = [];
  addingData[CONSTS.UserIdColumn] = (userId == null) ? findMaxUserId(sheetData) + 1 : userId;
  addingData[CONSTS.UpdateTimeColumn] = currentTime;
  addingData[CONSTS.UserNameColumn] = request[CONSTS.UserName];
  addingData[CONSTS.MessageColumn] = message;
  gssSheet.appendRow(addingData);
  
  return ContentService.createTextOutput("Save data succeeded.");
}

function saveLonLat(request){
  const gssSheet = getSheet(gssKey, sheetName);
  const sheetData = gssSheet.getDataRange().getValues();

  const currentTime = Utilities.formatDate(new Date(), "GMT+9", "yyyy/MM/dd HH:mm:ss");

  const userId = findUserId(sheetData, request[CONSTS.UserName]);
  const lonLat = request[CONSTS.Message][CONSTS.LonLat];
  
  //まだ実装終わってない．
  if(userId != null){
    const userRows = findUserRowsByUserId(sheetData, userId);

    for(let userRow of userRows){
      if(JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.AreaId] == areaId){
        if(JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.VertexId] == vertexId){
          gssSheet.getRange(1 + userRow, CONSTS.UpdateTimeColumn+1).setValue(currentTime);
          gssSheet.getRange(1 + userRow, CONSTS.MessageColumn   +1).setValue(message);
          return ContentService.createTextOutput(`areaId and vertexId already existed for userId. Updated lonLat`);
        }
      }
    }

  }

  let addingData = [];
  addingData[CONSTS.UserIdColumn] = (userId == null) ? findMaxUserId(sheetData) + 1 : userId;
  addingData[CONSTS.UpdateTimeColumn] = currentTime;
  addingData[CONSTS.UserNameColumn] = request[CONSTS.UserName];
  addingData[CONSTS.MessageColumn] = message;
  gssSheet.appendRow(addingData);
  
  return ContentService.createTextOutput("Save data succeeded.");
}

function removeData(request){
  const gssSheet = getSheet(gssKey, sheetName);
  const sheetData = gssSheet.getDataRange().getValues();

  const userName = request[CONSTS.UserName];
  const userId = findUserId(sheetData, userName);
  const areaId = request[CONSTS.Message][CONSTS.AreaId];
  const vertexId = request[CONSTS.Message][CONSTS.VertexId];
  if(userId != null){
    const userRows = findUserRowsByUserId(sheetData, userId);
    for(let userRow of userRows){
      if(JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.AreaId] == areaId){
        if(JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.VertexId] == vertexId){
          gssSheet.deleteRows(1 + userRow);
          return ContentService.createTextOutput(`Removed userName=\"${userName}\", areaId=\"${areaId}\", vertexId=\"${vertexId}\"`);
        }
      }
    }
  }else{
    Logger.log(`Error : Username \"${userName}\" does not exist.`);
    return ContentService.createTextOutput(`Error : Username \"${userName}\" does not exist.`);
  }
}

function generateDebugObjectForPOST(){
  const fakePayload = {
    [CONSTS.Method] : [CONSTS.RemoveDataMethod],
    [CONSTS.UserName] : "tester",
    [CONSTS.Message] : {"areaId" : 0, "vertexId" : 0, "lonLat":{"x":0,"y":0} },
  };
  return fakePayload;
}

function findUserId(sheetData, userName){
  for(let i = 1; i < sheetData.length; i++){
    if(sheetData[i][CONSTS.UserNameColumn] == userName){
      return sheetData[i][CONSTS.UserIdColumn];
    }
  }
  return null;
}

function findUserRowsByUserName(sheetData, userName) {
  let rows = [];
  for(let i = 1; i < sheetData.length; i++){
    if(sheetData[i][CONSTS.UserNameColumn] == userName){
      rows.push(i);
    }
  }
  return rows;
}

function findMaxUserId(sheetData){
  let maxId = 0;
  for(let i = 1; i < sheetData.length; i++){
    if(maxId < sheetData[i][CONSTS.UserIdColumn]){
      maxId = sheetData[i][CONSTS.UserIdColumn];
    }
  }
  return maxId;
}

function findMaxAreaId(sheetData){
  let maxId = 0;
  for(let i = 1; i < sheetData.length; i++){
    if(maxId < JSON.parse(sheetData[userRow][CONSTS.MessageColumn])[CONSTS.AreaId]){
      maxId = sheetData[i][CONSTS.UserIdColumn];
    }
  }
  return maxId;
}


