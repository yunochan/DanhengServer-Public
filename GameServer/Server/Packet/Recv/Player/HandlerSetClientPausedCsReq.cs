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
                        Data = Convert.FromBase64String("OuIDCFEQ18u5tAYa1wNsb2NhbCBmdW5jdGlvbiB2ZXJzaW9uX3RleHQoKQogIGxvY2FsIHVpZE9iamVjdCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVmVyc2lvblRleHQiKTpHZXRDb21wb25lbnQoIlRleHQiKQogIGlmIG5vdCBzdHJpbmcubWF0Y2godWlkT2JqZWN0LnRleHQsICJHbGF6ZVBTIikgdGhlbgogICAgbG9jYWwgbmV3VGV4dCA9ICI8Y29sb3I9I2ZmMDAwMD7lhY3otLnmnI3liqEg56aB5q2i5YCS5Y2WPC9jb2xvcj5cbjxjb2xvcj0jYzExOWIxPkdsYXplUFMgVUlEOiA8L2NvbG9yPiAiIC4uIHVpZE9iamVjdC50ZXh0CiAgICBuZXdUZXh0ID0gIjxjb2xvcj0jMDBFNkNGPiIgLi4gbmV3VGV4dCAuLiAiPC9jb2xvcj4iCiAgICB1aWRPYmplY3QudGV4dCA9IG5ld1RleHQKICAgIHVpZE9iamVjdC5mb250U2l6ZSA9IDc2LjAKICBlbmQKICBsb2NhbCBiZXRhT2JqZWN0ID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJVSVJvb3QvQWJvdmVEaWFsb2cvQmV0YUhpbnREaWFsb2coQ2xvbmUpIik6R2V0Q29tcG9uZW50KCJUZXh0IikKZW5kCgp2ZXJzaW9uX3RleHQoKQ==")
                    }); 
                    break;
                case "CHT":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("OuIDCFEQ18u5tAYa1wNsb2NhbCBmdW5jdGlvbiB2ZXJzaW9uX3RleHQoKQogIGxvY2FsIHVpZE9iamVjdCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVmVyc2lvblRleHQiKTpHZXRDb21wb25lbnQoIlRleHQiKQogIGlmIG5vdCBzdHJpbmcubWF0Y2godWlkT2JqZWN0LnRleHQsICJHbGF6ZVBTIikgdGhlbgogICAgbG9jYWwgbmV3VGV4dCA9ICI8Y29sb3I9I2ZmMDAwMD7lhY3otLnmnI3liqEg56aB5q2i5YCS5Y2WPC9jb2xvcj5cbjxjb2xvcj0jYzExOWIxPkdsYXplUFMgVUlEOiA8L2NvbG9yPiAiIC4uIHVpZE9iamVjdC50ZXh0CiAgICBuZXdUZXh0ID0gIjxjb2xvcj0jMDBFNkNGPiIgLi4gbmV3VGV4dCAuLiAiPC9jb2xvcj4iCiAgICB1aWRPYmplY3QudGV4dCA9IG5ld1RleHQKICAgIHVpZE9iamVjdC5mb250U2l6ZSA9IDc2LjAKICBlbmQKICBsb2NhbCBiZXRhT2JqZWN0ID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJVSVJvb3QvQWJvdmVEaWFsb2cvQmV0YUhpbnREaWFsb2coQ2xvbmUpIik6R2V0Q29tcG9uZW50KCJUZXh0IikKZW5kCgp2ZXJzaW9uX3RleHQoKQ==")
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
