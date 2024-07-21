using Microsoft.AspNetCore.Mvc;
using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.Util;
using static EggLink.DanhengServer.WebServer.Objects.NewLoginResJson;
using EggLink.DanhengServer.WebServer.Objects;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class NewUsernameLoginHandler
    {
        public JsonResult Handle(string account, string password)
        {
             //Debug
            logger.Info($"NewUsernameLoginHandler类传入的account={account},password={password}");
            NewLoginResJson res = new();
            AccountData? accountData = AccountData.GetAccountByUserName(account);

            if (accountData == null)
            {
                if (ConfigManager.Config.ServerOption.AutoCreateUser)
                {
                    AccountHelper.CreateAccount(account, 0);
                    accountData = AccountData.GetAccountByUserName(account);
                }
                else
                {
                    return new JsonResult(new NewLoginResJson { message = "Account not found", retcode = -201 });
                }
            }
            if (accountData != null)
            {
                res.message = "OK";
            //Debug
            logger.Info($"NewUsernameLoginHandler类应写入DispatchToken={accountData.GenerateDispatchToken()}");
                res.data = new VerifyData(accountData.Uid.ToString(), accountData.Username!, accountData.GenerateDispatchToken());
            }

            return new JsonResult(res);
        }
    }
}
