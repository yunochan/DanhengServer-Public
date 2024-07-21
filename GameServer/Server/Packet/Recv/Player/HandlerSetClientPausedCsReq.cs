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
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Lue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIGJldGFfdGV4dChvYmopCiAgICBsb2NhbCBnYW1lT2JqZWN0ID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJVSVJvb3QvQWJvdmVEaWFsb2cvQmV0YUhpbnREaWFsb2coQ2xvbmUpIikKCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQoKICAgICAgICBpZiB0ZXh0Q29tcG9uZW50IHRoZW4KICAgICAgICAgICAgbG9jYWwgdWlkID0gdGV4dENvbXBvbmVudC50ZXh0CiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyIpIHRoZW4KICAgICAgICAgICAgICAgIHVpZCA9IHVpZDpnc3ViKCJVSUQ6IiwgIjxjb2xvcj0jZmYwMDAwPuWFjei0ueacjeWKoSDnpoHmraLlgJLljZY8L2NvbG9yPlxuPGNvbG9yPSNjMTE5YjE+R2xhemVQUyBVSUQ6IDwvY29sb3I+IikKICAgICAgICAgICAgICAgIHRleHRDb21wb25lbnQudGV4dCA9ICI8Y29sb3I9IzAwRTZDRj4iLi51aWQuLiI8L2NvbG9yPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgpiZXRhX3RleHQoKQ==")
                    }); 
                    break;
                case "CHT":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Lue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIGJldGFfdGV4dChvYmopCiAgICBsb2NhbCBnYW1lT2JqZWN0ID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJVSVJvb3QvQWJvdmVEaWFsb2cvQmV0YUhpbnREaWFsb2coQ2xvbmUpIikKCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQoKICAgICAgICBpZiB0ZXh0Q29tcG9uZW50IHRoZW4KICAgICAgICAgICAgbG9jYWwgdWlkID0gdGV4dENvbXBvbmVudC50ZXh0CiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyIpIHRoZW4KICAgICAgICAgICAgICAgIHVpZCA9IHVpZDpnc3ViKCJVSUQ6IiwgIjxjb2xvcj0jZmYwMDAwPuWFjei0ueacjeWKoSDnpoHmraLlgJLljZY8L2NvbG9yPlxuPGNvbG9yPSNjMTE5YjE+R2xhemVQUyBVSUQ6IDwvY29sb3I+IikKICAgICAgICAgICAgICAgIHRleHRDb21wb25lbnQudGV4dCA9ICI8Y29sb3I9IzAwRTZDRj4iLi51aWQuLiI8L2NvbG9yPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgpiZXRhX3RleHQoKQ==")
                    });
                    break;
                default:
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Lue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIGJldGFfdGV4dChvYmopCiAgICBsb2NhbCBnYW1lT2JqZWN0ID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCJVSVJvb3QvQWJvdmVEaWFsb2cvQmV0YUhpbnREaWFsb2coQ2xvbmUpIikKCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQoKICAgICAgICBpZiB0ZXh0Q29tcG9uZW50IHRoZW4KICAgICAgICAgICAgbG9jYWwgdWlkID0gdGV4dENvbXBvbmVudC50ZXh0CiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyIpIHRoZW4KICAgICAgICAgICAgICAgIHVpZCA9IHVpZDpnc3ViKCJVSUQ6IiwgIjxjb2xvcj0jZmYwMDAwPuWFjei0ueacjeWKoSDnpoHmraLlgJLljZY8L2NvbG9yPlxuPGNvbG9yPSNjMTE5YjE+R2xhemVQUyBVSUQ6IDwvY29sb3I+IikKICAgICAgICAgICAgICAgIHRleHRDb21wb25lbnQudGV4dCA9ICI8Y29sb3I9IzAwRTZDRj4iLi51aWQuLiI8L2NvbG9yPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgpiZXRhX3RleHQoKQ==")
                    });
                    break;
            }
        }
    }
}
