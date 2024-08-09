﻿using EggLink.DanhengServer.Database.Inventory;

namespace EggLink.DanhengServer.Configuration;

public class ConfigContainer
{
    public HttpServerConfig HttpServer { get; set; } = new();
    public KeyStoreConfig KeyStore { get; set; } = new();
    public GameServerConfig GameServer { get; set; } = new();
    public PathConfig Path { get; set; } = new();
    public DatabaseConfig Database { get; set; } = new();
    public ServerOption ServerOption { get; set; } = new();
    public DownloadUrlConfig DownloadUrl { get; set; } = new();
    public MuipServerConfig MuipServer { get; set; } = new();
}

public class HttpServerConfig
{
    public string PublicAddress { get; set; } = "127.0.0.1";
    public int PublicPort { get; set; } = 443;
    public bool UseSSL { get; set; } = true;

    public string GetDisplayAddress()
    {
        return (UseSSL ? "https" : "http") + "://" + PublicAddress + ":" + PublicPort;
    }
}

public class KeyStoreConfig
{
    public string KeyStorePath { get; set; } = "certificate.p12";
    public string KeyStorePassword { get; set; } = "123456";
}

public class GameServerConfig
{
    public string PublicAddress { get; set; } = "127.0.0.1";
    public uint PublicPort { get; set; } = 23301;
    public string GameServerId { get; set; } = "dan_heng";
    public string GameServerName { get; set; } = "DanhengServer";
    public string GameServerDescription { get; set; } = "A re-implementation of StarRail server";
    public int UpdateInterval { get; set; } = 40;
    public string GetDisplayAddress()
    {
        return PublicAddress + ":" + PublicPort;
    }
}

public class PathConfig
{
    public string ResourcePath { get; set; } = "Resources";
    public string ConfigPath { get; set; } = "Config";
    public string DatabasePath { get; set; } = "Config/Database";
    public string LogPath { get; set; } = "Logs";
    public string PluginPath { get; set; } = "Plugins";
}

public class DatabaseConfig
{
    public string DatabaseType { get; set; } = "sqlite";
    public string DatabaseName { get; set; } = "danheng.db";
    public string MySqlHost { get; set; } = "127.0.0.1";
    public int MySqlPort { get; set; } = 3306;
    public string MySqlUser { get; set; } = "root";
    public string MySqlPassword { get; set; } = "123456";
    public string MySqlDatabase { get; set; } = "danheng";
}

public class ServerOption
{
    public bool EnableDialog { get; set; } = true;
    public bool EnableWindy { get; set; } = true;
    public int StartTrailblazerLevel { get; set; } = 1;
    public bool AutoUpgradeWorldLevel { get; set; } = true;
    public bool EnableMission { get; set; } = true; // experimental
    public bool AutoLightSection { get; set; } = true;
    public string Language { get; set; } = "EN";
    public List<string> DefaultPermissions { get; set; } = ["*"];
    public int ThresholdCount { get; set; } = 5; // Limit logins
    public int MaxPlayers { get; set; } = -1; // Limit Oline Players
    public ServerAnnounce ServerAnnounce { get; set; } = new();
    public WelcomeMessage WelcomeMessage { get; set; } = new();
    public WelcomeMail WelcomeMail { get; set; } = new();
    public ServerProfile ServerProfile { get; set; } = new();
    public bool AutoCreateUser { get; set; } = true;
    public bool SavePersonalDebugFile { get; set; } = false;
}

public class ServerAnnounce
{
    public bool EnableAnnounce { get; set; } = true;
    public string AnnounceContent { get; set; } = "Welcome to danhengserver!";
    public int Duration { get; set; } = 5;

}

public class WelcomeMessage {
    public int[] Emotes { get; set; } = {121005, 121006, 121007, 121008};// Sparkle Emotes
    public string Message { get; set; } = "Welcome to a DanHengServer";
}

public class WelcomeMail {
    public string Title { get; set; } = "Welcome to a DanHengServer";
    public string SenderName { get; set; } = "Server";
    public string Content { get; set; } = "Welcome to DanHengServer! Please take these items as a starter gift. For a list of commands, type /help in the server chat window. Check out our <a type=OpenURL1 href=https://discord.com/invite/xRtZsmHBVj>Discord</a> and <a type=OpenURL1 href=https://github.com/EggLinks/DanhengServer-Public>Github</a> for more information about the server.";
    public int ExpiredDay { get; set; } = 365;
    public List<ItemData> Attachment { get; set; } = new List<ItemData>();
}

public class ServerProfile
{
    public string Name { get; set; } = "Server";
    public int Uid { get; set; } = 80;
    public string Signature { get; set; } = "Type /help for a list of commands";
    public int Level { get; set; } = 1;
    public int HeadIcon { get; set; } = 201306;
    public int ChatBubbleId { get; set; } = 220001;
    public int DisplayAvatarId { get; set; } = 1306;
    public int DisplayAvatarLevel { get; set; } = 1;
}

public class DownloadUrlConfig
{
    public string? AssetBundleUrl { get; set; } = null;
    public string? ExResourceUrl { get; set; } = null;
    public string? LuaUrl { get; set; } = null;
    public string? IfixUrl { get; set; } = null;
}

public class MuipServerConfig
{
    public string AdminKey { get; set; } = "None";
}