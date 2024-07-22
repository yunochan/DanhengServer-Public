using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.WebServer.Objects;
using Microsoft.AspNetCore.Mvc;
using EggLink.DanhengServer.Util;
using static EggLink.DanhengServer.WebServer.Objects.LoginResJson;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class TokenLoginHandler
    {
        public static Logger logger = new("TokenLoginHandler");
        public JsonResult Handle(string uid, string token)
        {
            //Debug
            logger.Info($"传入的参数uid={uid} token={token}");
            AccountData? accountData = AccountData.GetAccountByUid(int.Parse(uid));
            var res = new LoginResJson();
            if (accountData == null || !accountData?.DispatchToken?.Equals(token) == true)
            {
                res.retcode = -201;
                res.message = "Game account cache information error";
                //Debug
                logger.Info($"accountData={accountData} ,Token验证失败");
            }
            else
            {
                res.message = "OK";
                res.data = new VerifyData(accountData!.Uid.ToString(), accountData.Username!, token);
            }
            return new JsonResult(res);
        }
    }
}
