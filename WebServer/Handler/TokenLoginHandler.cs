using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.WebServer.Objects;
using Microsoft.AspNetCore.Mvc;
using EggLink.DanhengServer.Util;
using static EggLink.DanhengServer.WebServer.Objects.LoginResJson;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class TokenLoginHandler
    {
        public JsonResult Handle(string uid, string token)
        {
            //Debug
            logger.Info($"TokenLoginHandler类传入的uid={uid} token={token}");
            AccountData? account = AccountData.GetAccountByUid(int.Parse(uid));
            var res = new LoginResJson();
            if (account == null || !account?.DispatchToken?.Equals(token) == true)
            {
                res.retcode = -201;
                res.message = "Game account cache information error";
                //Debug
                logger.Info($"TokenLoginHandler类 {account} ,Token验证失败");
            }
            else
            {
                res.message = "OK";
                res.data = new VerifyData(account!.Uid.ToString(), account.Username!, token);
            }
            return new JsonResult(res);
        }
    }
}
