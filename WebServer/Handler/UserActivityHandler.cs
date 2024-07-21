using EggLink.DanhengServer.Database.UserManagement;
using EggLink.DanhengServer.Util;
using SqlSugar;
using System;
using System.Net;
using System.Net.Sockets;
namespace EggLink.DanhengServer.WebServer.Handler
{
    public class UserActivityHandler
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public UserActivityHandler(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public void RecordUserActivity(string ip, string activityType)
        {
            var userActivity = new UserActivity
            {
                IP = ip,
                ActivityTime = DateTime.UtcNow,
                ActivityType = activityType
            };

            _sqlSugarClient.Insertable(userActivity).ExecuteCommand();
        }

        public bool IsUserBlacklisted(string ip)
        {
            var blacklisted = _sqlSugarClient.Queryable<BlackList>()
                                             .Where(b => b.IP == ip && b.EndTime > DateTime.UtcNow)
                                             .Any();
            return blacklisted;
        }

        public void CheckAndBlacklistUser(string ip, string activityType)
        {
            // 限制用户每分钟登录次数
            int thresholdCount = ConfigManager.Config.ServerOption.ThresholdCount;
            TimeSpan thresholdTimeSpan = TimeSpan.FromMinutes(1);

            DateTime currentTime = DateTime.UtcNow;
            DateTime startTime = currentTime - thresholdTimeSpan;

            // 获取用户活动次数
            int activityCount = _sqlSugarClient.Queryable<UserActivity>()
                                               .Where(a => a.IP == ip && a.ActivityTime >= startTime && a.ActivityType == activityType)
                                               .Count();

            if (activityCount >= thresholdCount)
            {
                var blackListEntry = new BlackList
                {
                    IP = ip,
                    StartTime = currentTime,
                    EndTime = currentTime + TimeSpan.FromMinutes(30), // 拉黑 30 分钟
                    Msg = $"{activityType} 过于频繁"
                };

                _sqlSugarClient.Insertable(blackListEntry).ExecuteCommand();
            }
        }

        public string GetClientIpPrefix(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return "Unknown";
            }

            try
            {
                var address = IPAddress.Parse(ip);
                if (address.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    var ipParts = ip.Split('.');
                    return $"{ipParts[0]}.{ipParts[1]}";
                }
                else if (address.AddressFamily == AddressFamily.InterNetworkV6) // IPv6
                {
                    var ipParts = ip.Split(':');
                    return $"{ipParts[0]}:{ipParts[1]}:{ipParts[2]}:{ipParts[3]}";
                }
            }
            catch (FormatException)
            {
                // 无法解析 IP 地址
                return "Unknown";
            }

            return "Unknown";
        }

        public bool IsSameIpPrefix(string ip1, string ip2)
        {
            return GetClientIpPrefix(ip1) == GetClientIpPrefix(ip2);
        }
    }
}
