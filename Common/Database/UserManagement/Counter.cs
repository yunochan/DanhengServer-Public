using EggLink.DanhengServer.Util;
using System;
using System.IO;
using SqlSugar;

namespace EggLink.DanhengServer.Database.UserManagement
{
    [SugarTable("Counter")]
    public class Counter
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = "Player";
        [SugarColumn(IsNullable = false)]
        public int NextUid { get; set; } = 100000001;

        // 获取 SqlSugarClient 实例
        private static SqlSugarClient GetDbClient()
        {
            var config = ConfigManager.Config.Database;
            string connectionString;
            DbType dbType;

            // 根据配置的数据库类型设置 DbType 和连接字符串
            switch (config.DatabaseType.ToLower())
            {
                case "sqlite":
                    dbType = DbType.Sqlite;
                    var fileInfo = new FileInfo(Path.Combine(config.Path.DatabasePath, config.DatabaseName));
                    if (!fileInfo.Exists && fileInfo.Directory != null)
                    {
                        fileInfo.Directory.Create();
                    }
                    connectionString = $"Data Source={fileInfo.FullName};";
                    break;

                case "mysql":
                    dbType = DbType.MySql;
                    connectionString = $"server={config.MySqlHost};Port={config.MySqlPort};Database={config.MySqlDatabase};Uid={config.MySqlUser};Pwd={config.MySqlPassword};";
                    break;

                default:
                    throw new NotSupportedException($"Database type {config.DatabaseType} is not supported.");
            }

            return new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = dbType,
                IsAutoCloseConnection = true
            });
        }

        // 获取并增加下一个 UID 的方法
        public static int GetNextUid()
        {
            var db = GetDbClient();
            var counter = db.Queryable<Counter>().Single();

            if (counter != null)
            {
                int currentUid = counter.NextUid;
                counter.NextUid += 1;

                db.Updateable(counter)
                  .SetColumns(c => c.NextUid == counter.NextUid)
                  .Where(c => c.Id == "Player")
                  .ExecuteCommand();

                return currentUid;
            }

            throw new Exception("Counter record not found");
        }
    }
}
