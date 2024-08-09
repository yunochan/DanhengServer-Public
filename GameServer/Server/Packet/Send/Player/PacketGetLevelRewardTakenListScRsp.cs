using EggLink.DanhengServer.GameServer.Game.Player;
using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.Proto;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Send.Player;

public class PacketGetLevelRewardTakenListScRsp : BasePacket
{
    public PacketGetLevelRewardTakenListScRsp(PlayerInstance player) : base(CmdIds.GetLevelRewardTakenListScRsp)
    {
        if (player.Data.TakenLevelReward == null)
        {
            throw new ArgumentNullException(nameof(player.Data.TakenLevelReward), "TakenLevelReward list cannot be null.");
        }

        var proto = new GetLevelRewardTakenListScRsp
        {
            LevelRewardTakenList = player.Data.TakenLevelReward.Select(x => (uint)x).ToList()
        };

        SetData(proto);
    }
}
