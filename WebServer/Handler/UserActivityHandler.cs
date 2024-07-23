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
        private static readonly Lazy<UserActivityHandler> _instance = new Lazy<UserActivityHandler>(() =>
        {
            var sqlSugarClient = CreateSqlSugarClient(); // Create or obtain the ISqlSugarClient instance
            return new UserActivityHandler(sqlSugarClient);
        });

        public static UserActivityHandler Instance => _instance.Value;

        private readonly ISqlSugarClient _sqlSugarClient;

        private UserActivityHandler(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }
            
        public void RecordUserActivity(string ip, string activityType)
        {
            var userActivity = new UserActivity
            {
                IP = ip,
                ActivityTime =DateTime.Now,
                ActivityType = activityType
            };

            _sqlSugarClient.Insertable(userActivity).ExecuteCommand();
        }

        public bool IsUserBlacklisted(string ip)
        {
            var blacklisted = _sqlSugarClient.Queryable<BlackList>()
                                             .Where(b => b.IP == ip && b.EndTime >DateTime.Now)
                                             .Any();
            return blacklisted;
        }

        public void CheckAndBlacklistUser(string ip, string activityType)
        {
            // 限制用户每分钟登录次数
            int thresholdCount = ConfigManager.Config.ServerOption.ThresholdCount;
            TimeSpan thresholdTimeSpan = TimeSpan.FromMinutes(1);

            DateTime currentTime =DateTime.Now;
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

		private static ISqlSugarClient CreateSqlSugarClient()
		{
			// 从配置中读取数据库配置
			var config = ConfigManager.Config;
			DbType dbType;
			string connectionString;
		
			// 根据配置的数据库类型设置 DbType 和连接字符串
			switch (config.Database.DatabaseType.ToLower())
			{
				case "sqlite":
					dbType = DbType.Sqlite;
					var fileInfo = new FileInfo(Path.Combine(config.Path.DatabasePath, config.Database.DatabaseName));
					if (!fileInfo.Exists && fileInfo.Directory != null)
					{
						fileInfo.Directory.Create();
					}
					connectionString = $"Data Source={fileInfo.FullName};";
					break;
				case "mysql":
					dbType = DbType.MySql;
					connectionString = $"server={config.Database.MySqlHost};Port={config.Database.MySqlPort};Database={config.Database.MySqlDatabase};Uid={config.Database.MySqlUser};Pwd={config.Database.MySqlPassword};";
					break;
				default:
					throw new NotSupportedException($"Database type {config.Database.DatabaseType} is not supported.");
			}
		
			// 创建并返回 SqlSugarClient 实例
			return new SqlSugarClient(new ConnectionConfig
			{
				ConnectionString = connectionString,
				DbType = dbType,
				IsAutoCloseConnection = true
			});
		}
    }
}
