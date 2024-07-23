using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.WebServer.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class ComboTokenGranterHandler
    {
        public JsonResult Handle(int app_id, int channel_id, string data, string device, string sign)
        {
            var tokenData = JsonConvert.DeserializeObject<LoginTokenData>(data);
            ComboTokenResJson res = new ComboTokenResJson();
            if (tokenData == null)
            {
                res.retcode = -202;
                res.message = "无效的登录数据";
                return new JsonResult(res);
            }
            AccountData? accountData = AccountData.GetAccountByUid(int.Parse(tokenData.uid!));
            if (accountData == null)
            {
                res.retcode = -201;
                res.message = "账号缓存错误";
                return new JsonResult(res);
            } else
            {
                res.message = "OK";
                res.data = new ComboTokenResJson.LoginData(accountData.Uid.ToString(), accountData.GenerateComboToken());
            }
            return new JsonResult(res);
        }
    }
    public class LoginTokenData
    {
        public string? uid { get; set; }
        public string? token { get; set; }
        public bool guest { get; set; }
    }
}
