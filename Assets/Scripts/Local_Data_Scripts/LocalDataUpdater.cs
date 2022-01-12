using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

public static class LocalDataUpdater
{
    public static void Update(
        UserDataManager userDataManager, AreaDataManager areaDataManager, GssDbHub gssDbHub)
    {
        userDataManager.UpdateAllUserNamesToGss(gssDbHub);
        areaDataManager.UpdateAllDatasToGss(gssDbHub);
    }
}
