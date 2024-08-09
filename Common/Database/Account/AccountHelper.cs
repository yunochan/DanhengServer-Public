﻿using EggLink.DanhengServer.Util;
using EggLink.DanhengServer.Database.UserManagement;
using EggLink.DanhengServer.Database;

namespace EggLink.DanhengServer.Database.Account;

public static class AccountHelper
{
    public static AccountData CreateAccount(string username, int uid)
    {
        if (AccountData.GetAccountByUserName(username) != null) throw new Exception("Account already exists");
        if (AccountData.GetAccountByUid(uid) != null)
        {
        }

        var newUid = uid;
        if (uid == 0)
        {
            newUid = 100000001; // start from 10001
            while (AccountData.GetAccountByUid(newUid) != null) newUid++;
        }

        //var per = ConfigManager.Config.ServerOption.DefaultPermissions;
        //var perStr = string.Join(",", per);
        var account = new AccountData
        {
            Uid = newUid,
            Username = username,
            //Permissions = perStr
        };
        DatabaseHelper.SaveInstance(account);
        return account;
    }
}