using EggLink.DanhengServer.Util;
using EggLink.DanhengServer.Database.UserManagement;
using EggLink.DanhengServer.Database;
using SqlSugar;

namespace EggLink.DanhengServer.Database.Account
{
    public static class AccountHelper
    {
        public static Logger logger = new("AccountHelper");

        public static AccountData CreateAccount(string username, int uid)
        {
            if (AccountData.GetAccountByUserName(username) != null)
            {
                throw new Exception("Account already exists");
            }

            int newUid = uid;
            if (uid == 0)
            {
                newUid = 100000001;  // start from 100000001
                while (AccountData.GetAccountByUid(newUid) != null)
                {
                    newUid++;
                }
            }
            var per = ConfigManager.Config.ServerOption.DefaultPermissions;
            var perStr = string.Join(",", per);
            var accountData = new AccountData()
            {
                Uid = newUid, 
                Username = username,
                Permissions = perStr
            };
            DatabaseHelper.SaveInstance(accountData);
            //Debug
            logger.Info($"分配的uid={accountData.Uid}，usrname={accountData.Username}");
            return accountData;
        }
    }
}
