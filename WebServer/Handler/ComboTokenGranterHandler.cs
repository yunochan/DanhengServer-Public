using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.WebServer.Objects;
using EggLink.DanhengServer.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class ComboTokenGranterHandler
    {
        public static Logger logger = new("ComboTokenGranterHandler");
        public JsonResult Handle(int app_id, int channel_id, string data, string device, string sign)
        {
            var tokenData = JsonConvert.DeserializeObject<LoginTokenData>(data);
            ComboTokenResJson res = new ComboTokenResJson();
            if (tokenData == null)
            {
                res.retcode = -202;
                res.message = "Invalid login data";
                return new JsonResult(res);
            }
             //Debug
            logger.Info($"uid={tokenData.uid}");
            AccountData? accountData = AccountData.GetAccountByUid(int.Parse(tokenData.uid!));//异常，发生null
            if (accountData == null)
            {
                res.retcode = -201;
                res.message = "Game account cache information error";
                //Debug
                logger.Info("发生accountData=null ");
                return new JsonResult(res);
            } else
            {
                res.message = "OK";
                //Debug
                logger.Info($"向数据库写入ComboToken={accountData.GenerateComboToken()}");
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
