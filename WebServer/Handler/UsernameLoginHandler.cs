using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EggLink.DanhengServer.Database.Account;
using EggLink.DanhengServer.Database.UserManagement;
using EggLink.DanhengServer.Util;
using static EggLink.DanhengServer.WebServer.Objects.LoginResJson;
using EggLink.DanhengServer.WebServer.Objects;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using SqlSugar;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class UsernameLoginHandler
    {
        public static Logger logger = new("Dispatch");
        // 将正则表达式初始化提取到类的静态字段中
        private static readonly Regex UsernameRegex = new Regex("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z0-9_]{8,16}$", RegexOptions.Compiled);
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserActivityHandler _userActivityHandler;
        
        // 构造函数注入 IHttpContextAccessor
         public UsernameLoginHandler(IHttpContextAccessor httpContextAccessor, UserActivityHandler userActivityHandler)
        {
            _httpContextAccessor = httpContextAccessor;
            _userActivityHandler = userActivityHandler;
        }
        
        public JsonResult Handle(string account, string password, bool isCrypto)
        {
            // 获取客户端 IP 地址
            string clientIp = GetClientIpAddress();

            // 初始化返回对象
            LoginResJson res = new();

            // 检查 IP 是否在黑名单中
            if (_userActivityHandler.IsUserBlacklisted(clientIp))
            {
                logger.Warn("客户端 {0} 因被系统拉黑而无法登录", clientIp!);
                return new JsonResult(new LoginResJson { message = "ip在黑名单里", retcode = -200 });
            }
            // 尝试获取账户数据
            AccountData? accountData = AccountData.GetAccountByUserName(account);

            // 如果账户不存在且允许自动创建用户
            if (accountData == null && ConfigManager.Config.ServerOption.AutoCreateUser)
            {
                // 校验账户格式
                if (!UsernameRegex.IsMatch(account))
                {
                    return new JsonResult(new LoginResJson { message = "账号只能由英文字母、数字和下划线组成，长度为8-16个字符，并需要包含英文字母和数字", retcode = -201 });
                }

                // 创建新账户
                AccountHelper.CreateAccount(account, 0);
                accountData = AccountData.GetAccountByUserName(account);

                // 再次检查账户数据
                if (accountData == null)
                {
                    logger.Warn("账号 {0} 自动注册失败！", account);
                    return new JsonResult(new LoginResJson { message = "自动注册失败，请联系管理员", retcode = -202 });
                }
                    logger.Info("账号 {0} 自动注册成功", account!);
            }
            else if (accountData == null) // 账户不存在且不允许自动创建用户
            {
                return new JsonResult(new LoginResJson { message = "自动注册已关闭，请手动注册", retcode = -203 });
            }

            // 检查是否异地登录
            if (accountData.IP != null && !_userActivityHandler.IsSameIpPrefix(clientIp, accountData.IP))
            {
                logger.Warn("账号 {0} UID: {1} 异地登录，IP: {2} -> {3}", accountData.Username!, accountData.Uid!, accountData.IP!, clientIp!);
                int count = accountData.Count ?? 0;
                accountData.SetCount(count + 1);
            }
                       
            accountData.SetIP(clientIp);//更新IP记录

            //检查账号是否被封禁
            if(accountData.IsBan){
                logger.Warn("账号 {0} UID: {1} 登录失败，原因: 账号被封禁", accountData.Username!, accountData.Uid!);
                return new JsonResult(new LoginResJson { message = "您的账号已经封停，有任何问题请联系管理员", retcode = -204 });

            }

            // 记录用户登录活动
            _userActivityHandler.RecordUserActivity(clientIp, "login");
            // 检查并拉黑用户
            _userActivityHandler.CheckAndBlacklistUser(clientIp, "login");
            // 账户存在，返回成功信息
            res.message = "OK";
            res.data = new VerifyData(accountData.Uid.ToString(), accountData.Username!, accountData.GenerateDispatchToken());
            logger.Info("账号 {0} UID: {1} 登录成功，IP来自 {3}", accountData.Username!, accountData.Uid!,accountData.IP!);
            return new JsonResult(res);
        }

        // 获取客户端 IP 地址
        private string GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        }

    }
}   
