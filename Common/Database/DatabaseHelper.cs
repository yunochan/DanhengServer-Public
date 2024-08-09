﻿using EggLink.DanhengServer.Database.Inventory;
using EggLink.DanhengServer.Database.Quests;
using EggLink.DanhengServer.Database.UserManagement;
using EggLink.DanhengServer.Internationalization;
using EggLink.DanhengServer.Util;
using SqlSugar;

namespace EggLink.DanhengServer.Database;

public class DatabaseHelper
{
    public static Logger logger = new("Database");
    public static SqlSugarScope? sqlSugarScope;
    public static DatabaseHelper? Instance;
    public static readonly Dictionary<int, List<BaseDatabaseDataHelper>> UidInstanceMap = [];
    public static readonly List<int> ToSaveUidList = [];
    public static long LastSaveTick = DateTime.UtcNow.Ticks;
    public static Thread? SaveThread;

    public DatabaseHelper()
    {
        Instance = this;
    }

    public void Initialize()
    {
        logger.Info(I18nManager.Translate("Server.ServerInfo.LoadingItem", I18nManager.Translate("Word.Database")));
        var config = ConfigManager.Config;
        DbType type;
        string connectionString;
        switch (config.Database.DatabaseType)
        {
            case "sqlite":
                type = DbType.Sqlite;
                var f = new FileInfo(config.Path.DatabasePath + "/" + config.Database.DatabaseName);
                if (!f.Exists && f.Directory != null) f.Directory.Create();
                connectionString = $"Data Source={f.FullName};";
                break;
            case "mysql":
                type = DbType.MySql;
                connectionString =
                    $"server={config.Database.MySqlHost};Port={config.Database.MySqlPort};Database={config.Database.MySqlDatabase};Uid={config.Database.MySqlUser};Pwd={config.Database.MySqlPassword};";
                break;
            default:
                return;
        }

        sqlSugarScope = new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = type,
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                SerializeService = new CustomSerializeService()
            }
        });

        switch (config.Database.DatabaseType)
        {
            case "sqlite":
                InitializeSqlite(); // for all database types
                break;
            case "mysql":
                InitializeMysql();
                break;
            default:
                logger.Error("Unsupported database type");
                break;
        }

        var baseType = typeof(BaseDatabaseDataHelper);
        var assembly = typeof(BaseDatabaseDataHelper).Assembly;
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        foreach (var t in types)
            typeof(DatabaseHelper).GetMethod("InitializeTable")?.MakeGenericMethod(t)
                .Invoke(null, null); // cache the data

        // Initialize special tables
        InitializeSpecialTable<BlackList>();
        InitializeSpecialTable<UserActivity>();


        LastSaveTick = DateTime.UtcNow.Ticks;

        SaveThread = new Thread(() =>
        {
            while (true) CalcSaveDatabase();
        });
        SaveThread.Start();
    }

    public static void InitializeTable<T>() where T : class, new()
    {
        var list = sqlSugarScope?.Queryable<T>()
            .Select(x => x)
            .ToList();

        foreach (var instance in list!)
        {
            var inst = (instance as BaseDatabaseDataHelper)!;
            if (!UidInstanceMap.TryGetValue(inst.Uid, out var value))
            {
                value = [];
                UidInstanceMap[inst.Uid] = value;
            }

            value.Add(inst); // add to the map
        }
    }

    public void UpgradeDatabase()
    {
        logger.Info("Upgrading database...");

        foreach (var instance in GetAllInstance<MissionData>()!) instance.MoveFromOld();

        foreach (var instance in GetAllInstance<InventoryData>()!) UpdateInstance(instance);
    }

    public void MoveFromSqlite()
    {
        logger.Info("Moving from sqlite...");

        var config = ConfigManager.Config;
        var f = new FileInfo(config.Path.DatabasePath + "/" + config.Database.DatabaseName);
        var sqliteScope = new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = $"Data Source={f.FullName};",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                SerializeService = new CustomSerializeService()
            }
        });

        var baseType = typeof(BaseDatabaseDataHelper);
        var assembly = typeof(BaseDatabaseDataHelper).Assembly;
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        foreach (var type in types)
            typeof(DatabaseHelper).GetMethod("MoveSqliteTable")?.MakeGenericMethod(type).Invoke(null, [sqliteScope]);

        // exit the program
        Environment.Exit(0);
    }

    public static void MoveSqliteTable<T>(SqlSugarScope scope) where T : class, new()
    {
        try
        {
            var list = scope.Queryable<T>().ToList();
            foreach (var instance in list!) sqlSugarScope?.Insertable(instance).ExecuteCommand();
        }
        catch (Exception e)
        {
            Logger.GetByClassName().Error("An error occurred while moving the table", e);
        }
    }

    public static void InitializeSqlite()
    {
        var baseType = typeof(BaseDatabaseDataHelper);
        var assembly = typeof(BaseDatabaseDataHelper).Assembly;
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        foreach (var type in types)
            typeof(DatabaseHelper).GetMethod("InitializeSqliteTable")?.MakeGenericMethod(type).Invoke(null, null);
    }

    public static void InitializeMysql()
    {
        sqlSugarScope?.DbMaintenance.CreateDatabase();
        InitializeSqlite();
    }

    public static void InitializeSqliteTable<T>() where T : class, new()
    {
        try
        {
            sqlSugarScope?.CodeFirst.InitTables<T>();
        }
        catch
        {
        }
    }

    public T? GetInstance<T>(int uid) where T : class, new()
    {
        try
        {
            if (!UidInstanceMap.TryGetValue(uid, out var value))
            {
                value = [];
                UidInstanceMap[uid] = value;
            }

            foreach (var instance in value)
                if (instance is T)
                    return instance as T; // found

            return null; // not found
        }
        catch (Exception e)
        {
            logger.Error("Unsupported type", e);
            return null;
        }
    }

    public T GetInstanceOrCreateNew<T>(int uid) where T : class, new()
    {
        var instance = GetInstance<T>(uid);
        if (instance == null)
        {
            instance = new T();
            (instance as BaseDatabaseDataHelper)!.Uid = uid;
            SaveInstance(instance);
        }

        return instance;
    }

    public static List<T>? GetAllInstance<T>() where T : class, new()
    {
        try
        {
            return sqlSugarScope?.Queryable<T>()
                .Select(x => x)
                .ToList();
        }
        catch (Exception e)
        {
            logger.Error("Unsupported type", e);
            return null;
        }
    }

    public static void SaveInstance<T>(T instance) where T : class, new()
    {
        sqlSugarScope?.Insertable(instance).ExecuteCommand();
        UidInstanceMap[(instance as BaseDatabaseDataHelper)!.Uid]
            .Add((instance as BaseDatabaseDataHelper)!); // add to the map
    }

    public void UpdateInstance<T>(T instance) where T : class, new()
    {
        //lock (GetLock((instance as BaseDatabaseDataHelper)!.Uid))
        //{
        //    sqlSugarScope?.Updateable(instance).ExecuteCommand();
        //}
    }

    public void CalcSaveDatabase() // per 30 s
    {
        if (LastSaveTick + TimeSpan.TicksPerSecond * 30 > DateTime.UtcNow.Ticks) return;
        SaveDatabase();
    }

    public void SaveDatabase() // per 30 s
    {
        try
        {
            var prev = DateTime.Now;
            var list = ToSaveUidList.ToList(); // copy the list to avoid the exception
            foreach (var uid in list)
            {
                var value = UidInstanceMap[uid];
                var baseType = typeof(BaseDatabaseDataHelper);
                var assembly = typeof(BaseDatabaseDataHelper).Assembly;
                var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
                foreach (var type in types)
                {
                    var instance = value.Find(x => x.GetType() == type);
                    if (instance != null)
                        typeof(DatabaseHelper).GetMethod("SaveDatabaseType")?.MakeGenericMethod(type)
                            .Invoke(null, [instance]);
                }
            }

            logger.Info(I18nManager.Translate("Server.ServerInfo.SaveDatabase",
                (DateTime.Now - prev).TotalSeconds.ToString()[..4]));

            ToSaveUidList.Clear();
        }
        catch (Exception e)
        {
            logger.Error("An error occurred while saving the database", e);
        }

        LastSaveTick = DateTime.UtcNow.Ticks;
    }

    public static void SaveDatabaseType<T>(T instance) where T : class, new()
    {
        try
        {
            sqlSugarScope?.Updateable(instance).ExecuteCommand();
        }
        catch (Exception e)
        {
            logger.Error("An error occurred while saving the database", e);
        }
    }

    public void DeleteInstance<T>(T instance) where T : class, new()
    {
        sqlSugarScope?.Deleteable(instance).ExecuteCommand();
        UidInstanceMap[(instance as BaseDatabaseDataHelper)!.Uid]
            .Remove((instance as BaseDatabaseDataHelper)!); // remove from the map
        ToSaveUidList.Remove((instance as BaseDatabaseDataHelper)!.Uid); // remove from the save list
    }
}