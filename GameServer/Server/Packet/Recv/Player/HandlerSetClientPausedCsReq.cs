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
                        Data = Convert.FromBase64String("OuIDCFEQ18u5tAYa1wNsb2NhbCBmdW5jdGlvbiB2ZXJzaW9uX3RleHQoKQogICAgbG9jYWwgZ2FtZU9iamVjdCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVUlSb290L0Fib3ZlRGlhbG9nL0JldGFIaW50RGlhbG9nKENsb25lKSIpCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQogICAgICAgIGlmIHRleHRDb21wb25lbnQgdGhlbgogICAgICAgICAgICBsb2NhbCB1aWQgPSB0ZXh0Q29tcG9uZW50LnRleHQKICAgICAgICAgICAgaWYgbm90IHVpZDpmaW5kKCJHbGF6ZVBTIFVJRDoiKSB0aGVuCiAgICAgICAgICAgICAgICB1aWQgPSB1aWQ6Z3N1YigiVUlEOiIsICI8Yj48Y29sb3I9I2ZmOWVjNj5HbGF6ZVBTIFVJRDogPC9jb2xvcj48L2I+IikKICAgICAgICAgICAgICAgIHRleHRDb21wb25lbnQudGV4dCA9ICI8Yj48Y29sb3I9IzAwZGJlNT4iIC4uIHVpZCAuLiAiPC9jb2xvcj48L2I+IgogICAgICAgICAgICBlbmQKICAgICAgICBlbmQKICAgIGVuZAplbmQKCnZlcnNpb25fdGV4dCgp=")
                    }); 
                    break;
                case "CHT":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Lue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIHZlcnNpb25fdGV4dCgpCiAgICBsb2NhbCB1aWQgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlZlcnNpb25UZXh0Iik6R2V0Q29tcG9uZW50KCJUZXh0IikKICAgIGlmIG5vdCBzdHJpbmcubWF0Y2godWlkLnRleHQsICLlhY3otLkiKSB0aGVuCiAgICAgICAgdWlkLnRleHQgPSAiPGNvbG9yPSNmZjAwMDA+5YWN6LS55pyN5YqhIOemgeatouWAkuWNljwvY29sb3I+IiAuLiB1aWQudGV4dAogICAgICAgIHVpZC5mb250U2l6ZSA9IDc2LjAKICAgIGVuZAogICAgbG9jYWwgYmV0YSA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVUlSb290L0Fib3ZlRGlhbG9nL0JldGFIaW50RGlhbG9nKENsb25lKSIpOkdldENvbXBvbmVudCgiVGV4dCIpCmVuZAoKdmVyc2lvbl90ZXh0KCk=")
                    });
                    break;
                default:
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("OvEDCFEQ18u5tAYa5gNsb2NhbCBmdW5jdGlvbiB2ZXJzaW9uX3RleHQoKQogIGxvY2FsIHVpZE9iamVjdCA9IENTLlVuaXR5RW5naW5lLkdhbWVPYmplY3QuRmluZCgiVmVyc2lvblRleHQiKTpHZXRDb21wb25lbnQoIlRleHQiKQogIGlmIG5vdCBzdHJpbmcubWF0Y2godWlkT2JqZWN0LnRleHQsICJHbGF6ZVBTIikgdGhlbgogICAgbG9jYWwgbmV3VGV4dCA9ICI8Y29sb3I9I2ZmMDAwMD7lhY3otLnmnI3liqEg56aB5q2i5YCS5Y2WPC9jb2xvcj5cbjxjb2xvcj0jYzExOWIxPkdsYXplUFMgVUlEOiA8L2NvbG9yPiAiIC4uIHVpZE9iamVjdC50ZXh0CiAgICBuZXdUZXh0ID0gIjxjb2xvcj0jMDBFNkNGPiIgLi4gbmV3VGV4dCAuLiAiPC9jb2xvcj4iCiAgICB1aWRPYmplY3QudGV4dCA9IG5ld1RleHQKICBlbmQKZW5kCgp2ZXJzaW9uX3RleHQoKQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")
                    });
                    break;
            }
        }
    }
}
