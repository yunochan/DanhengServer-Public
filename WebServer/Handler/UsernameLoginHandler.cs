using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.Database.UserManagement;
using EggLink.DanhengServer.Util;
using EggLink.DanhengServer.WebServer.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EggLink.DanhengServer.WebServer.Objects.LoginResJson;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using SqlSugar;

namespace EggLink.DanhengServer.WebServer.Handler;

public class UsernameLoginHandler
{
    public static Logger logger = new("Dispatch");
    // 将正则表达式初始化提取到类的静态字段中
    private static readonly Regex UsernameRegex = new Regex("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z0-9_]{8,16}$", RegexOptions.Compiled);
    
    public JsonResult Handle(string account, string password, bool isCrypto)
    {
        // 获取客户端 IP 地址
        string clientIp = GetClientIpAddress();

        // 初始化返回对象
        LoginResJson res = new();

         // 获取 UserActivityHandler 的单例实例
        var userActivityHandler = UserActivityHandler.Instance;

        // 检查 IP 是否在黑名单中
        if (userActivityHandler.IsUserBlacklisted(clientIp))
        {
            logger.Warn($"客户端 {clientIp} 因被系统拉黑而无法登录");
            return new JsonResult(new LoginResJson { message = "登录过于频繁，被系统拉黑", retcode = -200 });
        }
        // 尝试获取账号数据
        var accountData = AccountData.GetAccountByUserName(account);

        // 如果账号数据不存在且允许自动创建用户
        if (accountData == null && ConfigManager.Config.ServerOption.AutoCreateUser)
        {
            // 校验账号格式
            if (!UsernameRegex.IsMatch(account))
            {
                return new JsonResult(new LoginResJson { message = "账号只能由英文字母、数字和下划线组成，长度为8-16个字符，并需要包含英文字母和数字", retcode = -201 });
            }

            // 创建新账号
            accountData = AccountHelper.CreateAccount(account, 0);

            // 再次检查账号数据
            if (accountData == null)
            {
                logger.Warn($"账号 {account} 自动注册失败！");
                return new JsonResult(new LoginResJson { message = "自动注册失败，请联系管理员", retcode = -202 });
            }
                logger.Info($"账号 {account} 自动注册成功");
        }
        else if (accountData == null) // 账号不存在且不允许自动创建用户
        {
            return new JsonResult(new LoginResJson { message = "自动注册已关闭，请手动注册", retcode = -203 });
        }

        // 检查是否异地登录
        if (accountData.IP != null && !userActivityHandler.IsSameIpPrefix(clientIp, accountData.IP))
        {
            logger.Warn($"账号: {accountData.Username} UID: {accountData.Uid} 异地登录，客户端IP: {accountData.IP} -> {clientIp}");
            int count = accountData.Count ?? 0;
            accountData.SetCount(count + 1);
        }
                   
        //检查账号是否被封禁
        if(accountData.IsBan){
            logger.Warn($"账号 {accountData.Username} UID: {accountData.Uid} 登录失败，原因: 账号被封禁");
            return new JsonResult(new LoginResJson { message = "您的账号已经封停，有任何问题请联系管理员", retcode = -204 });
        }

        // 记录用户登录活动
        userActivityHandler.RecordUserActivity(clientIp, "login");
        // 检查并拉黑用户
        userActivityHandler.CheckAndBlacklistUser(clientIp, "login");
        // 账户存在，返回成功信息
        res.message = "OK";
        res.data = new VerifyData(accountData.Uid.ToString(), accountData.Username!, accountData.GenerateDispatchToken());
        logger.Info($"客户端{accountData.IP}登录成功，账号: {accountData.Username} UID: {accountData.Uid}");
        accountData.SetIP(clientIp);
        return new JsonResult(res);
    }

    // 获取客户端 IP 地址
    private string GetClientIpAddress()
    {
        var context = HttpContextProvider.HttpContext;
        return context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
