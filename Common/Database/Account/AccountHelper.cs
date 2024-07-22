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
                newUid = GetNextUid();
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

         private static int GetNextUid()
        {
            int nextUid;
            DatabaseHelper.SqlSugarScope?.Ado.BeginTran(); // 开启事务

            try
            {
                // 获取当前的 Counter 记录
                var counter = DatabaseHelper.SqlSugarScope?.Queryable<Counter>().Single(it => it.Id == "Player");
                if (counter == null)
                {
                    throw new Exception("Counter record not found");
                }

                nextUid = counter.NextUid;

                // 更新 Counter 表中的 NextUid
                counter.NextUid++;
                DatabaseHelper.SqlSugarScope?.Updateable(counter).ExecuteCommand();

                DatabaseHelper.SqlSugarScope?.Ado.CommitTran(); // 提交事务
            }
            catch (Exception)
            {
                DatabaseHelper.SqlSugarScope?.Ado.RollbackTran(); // 回滚事务
                throw;
            }

            return nextUid;
        }
    }
}
