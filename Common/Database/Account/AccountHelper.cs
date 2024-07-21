using EggLink.DanhengServer.Util;

namespace EggLink.DanhengServer.Database.Account
{
    public static class AccountHelper
    {
        public static void CreateAccount(string username, int uid)
        {
            if (AccountData.GetAccountByUserName(username) != null)
            {
                throw new Exception("Account already exists");
            }

            // 创建新账户，Uid会自动分配
            var per = ConfigManager.Config.ServerOption.DefaultPermissions;
            var perStr = string.Join(",", per);
            var account = new AccountData()
            {
                Username = username,
                Permissions = perStr
            };

            // 如果指定了 uid，检查是否有重复的 uid 并设置 Uid
            if (uid != 0)
            {
                if (AccountData.GetAccountByUid(uid) != null)
                {
                    throw new Exception("Account with specified UID already exists");
                }
                account.Uid = uid;
            }

            DatabaseHelper.SaveInstance(account);
        }
    }
}
