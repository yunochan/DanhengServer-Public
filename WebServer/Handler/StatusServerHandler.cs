using EggLink.DanhengServer.GameServer.Server;
using EggLink.DanhengServer.Util;

namespace EggLink.DanhengServer.WebServer.Handler
{
    public class StatusServerHandler
    {
        public string ResponseJson { get; set; }
        public int PlayerCount { get; set; }
        public int MaxPlayers { get; set; }
        public string Version { get; set; } = "2.4.0";

        public StatusServerHandler()
        {
    
            PlayerCount = Listener.GetActiveUidCount();
            MaxPlayers = ConfigManager.Config.ServerOption.MaxPlayers;

            // 使用字符串插值格式化 JSON 字符串
            ResponseJson = string.Format(
                "{{\"retcode\":0,\"status\":{{\"playerCount\":{0},\"maxPlayers\":{1},\"version\":\"{2}\"}}}}",
                PlayerCount, MaxPlayers, Version
            );
        }
    }
}
