using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.Server.Packet.Send.Others
{
    public class PacketServerAnnounceNotify : BasePacket
    {
        private static readonly Logger Logger = new("PacketServerAnnounceNotify");
        public PacketServerAnnounceNotify() : base(CmdIds.ServerAnnounceNotify)
        {
            var proto = new ServerAnnounceNotify();

            var beginTime = Extensions.GetUnixSec();
            var endTime = beginTime + 3600;
            var configId = 1;
            var announceContent = ConfigManager.Config.ServerOption.ServerAnnounce.AnnounceContent;
            proto.AnnounceDataList.Add(new AnnounceData()
            {
                BeginTime = beginTime,
                EndTime = endTime,
                ConfigId = (uint)configId,
                CHJPFPLHJBJ = announceContent,
            });
                Logger.Debug("Announcement is enabled");
                Logger.Debug($"BeginTime={beginTime}");
                Logger.Debug($"EndTime={endTime}");
                Logger.Debug($"ConfigId={configId}");
                Logger.Debug($"CHJPFPLHJBJ={announceContent}");

            if (ConfigManager.Config.ServerOption.ServerAnnounce.EnableAnnounce)
            {
                SetData(proto);
            }
            else 
            {
                Logger.Debug("Announcement is diabled");
            }
        }
    }
}
