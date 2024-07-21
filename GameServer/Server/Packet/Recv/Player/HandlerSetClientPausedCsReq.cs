using System;
using System.IO;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;
using EggLink.DanhengServer.Server.Packet.Send.Player;
using EggLink.DanhengServer.Server.Packet.Send.Others;
namespace EggLink.DanhengServer.Server.Packet.Recv.Player
{
    [Opcode(CmdIds.SetClientPausedCsReq)]
    public class HandlerSetClientPausedCsReq : Handler
    {
        public override void OnHandle(Connection connection, byte[] header, byte[] data)
        {
            var req = SetClientPausedCsReq.Parser.ParseFrom(data);
            var paused = req.Paused;
            connection.SendPacket(new PacketSetClientPausedScRsp(paused));

            // DO NOT REMOVE THIS CODE
            // This code is responsible for sending the client data to the player
            switch (ConfigManager.Config.ServerOption.Language)
            {
                case "CHS":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Lue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIHZlcnNpb25fdGV4dCgpCiAgICBsb2NhbCB1aWRPYmplY3QgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlZlcnNpb25UZXh0Iik6R2V0Q29tcG9uZW50KCJUZXh0IikKICAgIGlmIG5vdCBzdHJpbmcubWF0Y2godWlkT2JqZWN0LnRleHQsICJHbGF6ZVBTIikgdGhlbgogICAgICAgIGxvY2FsIG5ld1RleHQgPSAiPGNvbG9yPSNmZjAwMDA+5YWN6LS55pyN5YqhIOemgeatouWAkuWNljwvY29sb3I+XG48Y29sb3I9I2MxMTliMT5HbGF6ZVBTIFVJRDogPC9jb2xvcj4gIiAuLiB1aWRPYmplY3QudGV4dAogICAgICAgIG5ld1RleHQgPSAiPGNvbG9yPSMwMEU2Q0Y+IiAuLiBuZXdUZXh0IC4uICI8L2NvbG9yPiIKICAgICAgICB1aWRPYmplY3QudGV4dCA9IG5ld1RleHQKICAgICAgICB1aWRPYmplY3QuZm9udFNpemUgPSA3Ni4wCiAgICBlbmQKICAgIGxvY2FsIGJldGFPYmplY3QgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlVJUm9vdC9BYm92ZURpYWxvZy9CZXRhSGludERpYWxvZyhDbG9uZSkiKTpHZXRDb21wb25lbnQoIlRleHQiKQplbmQKCnZlcnNpb25fdGV4dCgp")
                    }); 
                    break;
                case "CHT":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("bG9jYWwgZnVuY3Rpb24gdmVyc2lvbl90ZXh0KCkKICAgIGxvY2FsIHVpZE9iamVjdCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVmVyc2lvblRleHQiKTpHZXRDb21wb25lbnQoIlRleHQiKQogICAgaWYgbm90IHN0cmluZy5tYXRjaCh1aWRPYmplY3QudGV4dCwgIkdsYXplUFMiKSB0aGVuCiAgICAgICAgbG9jYWwgbmV3VGV4dCA9ICI8Y29sb3I9I2ZmMDAwMD7lhY3otLnmnI3liqEg56aB5q2i5YCS5Y2WPC9jb2xvcj5cbjxjb2xvcj0jYzExOWIxPkdsYXplUFMgVUlEOiA8L2NvbG9yPiAiIC4uIHVpZE9iamVjdC50ZXh0CiAgICAgICAgbmV3VGV4dCA9ICI8Y29sb3I9IzAwRTZDRj4iIC4uIG5ld1RleHQgLi4gIjwvY29sb3I+IgogICAgICAgIHVpZE9iamVjdC50ZXh0ID0gbmV3VGV4dAogICAgICAgIHVpZE9iamVjdC5mb250U2l6ZSA9IDc2LjAKICAgIGVuZAogICAgbG9jYWwgYmV0YU9iamVjdCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVUlSb290L0Fib3ZlRGlhbG9nL0JldGFIaW50RGlhbG9nKENsb25lKSIpOkdldENvbXBvbmVudCgiVGV4dCIpCmVuZAoKdmVyc2lvbl90ZXh0KCk=")
                    });
                    break;
                default:
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("OvEDCFEQ18u5tAYa5gNsb2NhbCBmdW5jdGlvbiB2ZXJzaW9uX3RleHQoKQogICAgbG9jYWwgdWlkID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJWZXJzaW9uVGV4dCIpOkdldENvbXBvbmVudCgiVGV4dCIpCiAgICBpZiBub3Qgc3RyaW5nLm1hdGNoKHVpZC50ZXh0LCAiRGFuaGVuZyBTZXJ2ZXIiKSB0aGVuCiAgICAgICAgdWlkLnRleHQgPSAiRGFuaGVuZyBTZXJ2ZXIgaXMgYSBzZW1pIG9wZW4tc291cmNlIHNlcnZlciBzb2Z0d2FyZS5cbkVkdWNhdGlvbmFsIHB1cnBvc2Ugb25seSwgcGxlYXNlIHN1cHBvcnQgdGhlIGdlbnVpbmUgZ2FtZS4gIiAuLiB1aWQudGV4dAogICAgICAgIHVpZC5mb250U2l6ZSA9IDc2LjAKICAgIGVuZAogICAgbG9jYWwgYmV0YSA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVUlSb290L0Fib3ZlRGlhbG9nL0JldGFIaW50RGlhbG9nKENsb25lKSIpOkdldENvbXBvbmVudCgiVGV4dCIpCmVuZAoKdmVyc2lvbl90ZXh0KCk=")
                    });
                    break;
            }
        }
    }
}
