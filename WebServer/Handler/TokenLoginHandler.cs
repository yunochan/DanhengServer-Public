using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.WebServer.Objects;
using Microsoft.AspNetCore.Mvc;
using static EggLink.DanhengServer.WebServer.Objects.LoginResJson;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class TokenLoginHandler
    {
        public JsonResult Handle(string uid, string token)
        {
            AccountData? accountData = AccountData.GetAccountByUid(int.Parse(uid));
            var res = new LoginResJson();
            if (accountData == null || !accountData?.DispatchToken?.Equals(token) == true)
            {
                res.retcode = -201;
                res.message = "token验证失败";
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
