using EggLink.DanhengServer.GameServer.Server.Packet.Send.Player;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.Scene;
using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;
using Google.Protobuf;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.SetClientPausedCsReq)]
public class HandlerSetClientPausedCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SetClientPausedCsReq.Parser.ParseFrom(data);
        var paused = req.Paused;
        await connection.SendPacket(new PacketSetClientPausedScRsp(paused));
        if (ConfigManager.Config.ServerOption.EnableWindy){
            string base64String = "bG9jYWwgZnVuY3Rpb24gdmVyc2lvbl90ZXh0KCkKICAgIGxvY2FsIHVpZCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVmVyc2lvblRleHQiKTpHZXRDb21wb25lbnQoIlRleHQiKQogICAgaWYgbm90IHN0cmluZy5tYXRjaCh1aWQudGV4dCwgIuacrOacjSIpIHRoZW4KICAgICAgICB1aWQudGV4dCA9ICLmnKzmnI3lhY3otLnvvIznpoHmraLlgJLljZYiIC4uIHVpZC50ZXh0CiAgICAgICAgdWlkLmZvbnRTaXplID0gNzYuMAogICAgZW5kCiAgICBsb2NhbCBiZXRhID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJVSVJvb3QvQWJvdmVEaWFsb2cvQmV0YUhpbnREaWFsb2coQ2xvbmUpIik6R2V0Q29tcG9uZW50KCJUZXh0IikKZW5kCgp2ZXJzaW9uX3RleHQoKQ==";
            byte[] byteArray = Convert.FromBase64String(base64String);
            await connection.SendPacket(new HandshakePacket(byteArray));

        }
    }
}