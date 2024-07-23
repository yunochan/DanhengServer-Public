using EggLink.DanhengServer.Game.Player;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;
namespace EggLink.DanhengServer.Server.Packet.Send.Lineup
{
    public class PacketGetAllLineupDataScRsp : BasePacket
    {
        public static Logger logger = new("PacketGetAllLineupDataScRsp");
        public PacketGetAllLineupDataScRsp(PlayerInstance player) : base(CmdIds.GetAllLineupDataScRsp)
        {
            var proto = new GetAllLineupDataScRsp()
            {
                CurIndex = (uint)player.LineupManager!.LineupData.CurLineup,
            };
            foreach (var lineup in player.LineupManager.GetAllLineup())
            {
                //Debug
                if (lineup == null)
                {
                    logger.Error("Found a null lineup.");
                    continue;
                }
                proto.LineupList.Add(lineup.ToProto());
            }

            SetData(proto);
        }
    }
}
