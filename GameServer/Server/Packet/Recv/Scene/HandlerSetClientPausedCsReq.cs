using EggLink.DanhengServer.GameServer.Server.Packet.Send.Player;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.Scene;
using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Recv.Scene;

[Opcode(CmdIds.SetClientPausedCsReq)]
public class HandlerSetClientPausedCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = SetClientPausedCsReq.Parser.ParseFrom(data);
        var paused = req.Paused;
        await connection.SendPacket(new PacketSetClientPausedScRsp(paused));
        if (ConfigManager.Config.ServerOption.EnableWindy)
            BasePacket basePacket = new HandshakePacket(Convert.FromBase64String("bG9jYWwgZnVuY3Rpb24gYmV0YV90ZXh0KCkKICAgIGxvY2FsIGdhbWVPYmplY3QgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlVJUm9vdC9BYm92ZURpYWxvZy9CZXRhSGludERpYWxvZyhDbG9uZSkiKQogICAgaWYgZ2FtZU9iamVjdCB0aGVuCiAgICAgICAgbG9jYWwgdGV4dENvbXBvbmVudCA9IGdhbWVPYmplY3Q6R2V0Q29tcG9uZW50SW5DaGlsZHJlbih0eXBlb2YoQ1MuUlBHLkNsaWVudC5Mb2NhbGl6ZWRUZXh0KSkKICAgICAgICBpZiB0ZXh0Q29tcG9uZW50IHRoZW4KICAgICAgICAgICAgdWlkID0gdGV4dENvbXBvbmVudC50ZXh0CiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyBVSUQ6IikgdGhlbgogICAgICAgICAgICAgICAgdWlkID0gdWlkOmdzdWIoIlVJRDoiLCAiPGI+PGNvbG9yPSNGRjAwMDA+5YWN6LS55pyN5YqhIOemgeatouWAkuWNljwvY29sb3I+XHJcbjxjb2xvcj0jQzExOUIxPkdsYXplUFMgVUlEOiA8L2NvbG9yPjwvYj4iKQogICAgICAgICAgICAgICAgdGV4dENvbXBvbmVudC50ZXh0ID0gIjxiPjxjb2xvcj0jMDBFNkNGPiIuLnVpZC4uIjwvY29sb3I+PC9iPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgpiZXRhX3RleHQoKQ=="));
            await connection.SendPacket(basePacket.BuildPacket());
    }
}