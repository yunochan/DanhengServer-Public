using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.Util;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Recv.Player;

[Opcode(CmdIds.PlayerLoginFinishCsReq)]
public class HandlerPlayerLoginFinishCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        await connection.SendPacket(CmdIds.PlayerLoginFinishScRsp);
        if (ConfigManager.Config.ServerOption.ServerAnnounce.EnableAnnounce)
            await connection.SendPacket(new PacketServerAnnounceNotify());
        //var list = connection.Player!.MissionManager!.GetRunningSubMissionIdList();
        //connection.SendPacket(new PacketMissionAcceptScNotify(list));

    }
}