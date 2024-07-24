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
                        Data = Convert.FromBase64String("bG9jYWwgZnVuY3Rpb24gdmVyc2lvbl90ZXh0KCkKICAgIGxvY2FsIGdhbWVPYmplY3QgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlVJUm9vdC9BYm92ZURpYWxvZy9CZXRhSGludERpYWxvZyhDbG9uZSkiKQogICAgCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQogICAgICAgIAogICAgICAgIGlmIHRleHRDb21wb25lbnQgdGhlbgogICAgICAgICAgICBsb2NhbCB1aWQgPSB0ZXh0Q29tcG9uZW50LnRleHQKICAgICAgICAgICAgCiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyBVSUQ6IikgdGhlbgogICAgICAgICAgICAgICAgdWlkID0gdWlkOmdzdWIoIlVJRDoiLCAiPGI+PGNvbG9yPSNmZjllYzY+R2xhemVQUyBVSUQ6IDwvY29sb3I+PC9iPiIpCiAgICAgICAgICAgICAgICB0ZXh0Q29tcG9uZW50LnRleHQgPSAiPGI+PGNvbG9yPSMwMGRiZTU+IiAuLiB1aWQgLi4gIjwvY29sb3I+PC9iPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgp2ZXJzaW9uX3RleHQoKQo=")
                    }); 
                    break;
                case "CHT":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("bG9jYWwgZnVuY3Rpb24gdmVyc2lvbl90ZXh0KCkKICAgIGxvY2FsIGdhbWVPYmplY3QgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlVJUm9vdC9BYm92ZURpYWxvZy9CZXRhSGludERpYWxvZyhDbG9uZSkiKQogICAgCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQogICAgICAgIAogICAgICAgIGlmIHRleHRDb21wb25lbnQgdGhlbgogICAgICAgICAgICBsb2NhbCB1aWQgPSB0ZXh0Q29tcG9uZW50LnRleHQKICAgICAgICAgICAgCiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyBVSUQ6IikgdGhlbgogICAgICAgICAgICAgICAgdWlkID0gdWlkOmdzdWIoIlVJRDoiLCAiPGI+PGNvbG9yPSNmZjllYzY+R2xhemVQUyBVSUQ6IDwvY29sb3I+PC9iPiIpCiAgICAgICAgICAgICAgICB0ZXh0Q29tcG9uZW50LnRleHQgPSAiPGI+PGNvbG9yPSMwMGRiZTU+IiAuLiB1aWQgLi4gIjwvY29sb3I+PC9iPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgp2ZXJzaW9uX3RleHQoKQo=")
                    });
                    break;
                default:
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("bG9jYWwgZnVuY3Rpb24gdmVyc2lvbl90ZXh0KCkKICAgIGxvY2FsIGdhbWVPYmplY3QgPSBDUy5Vbml0eUVuZ2luZS5HYW1lT2JqZWN0LkZpbmQoIlVJUm9vdC9BYm92ZURpYWxvZy9CZXRhSGludERpYWxvZyhDbG9uZSkiKQogICAgCiAgICBpZiBnYW1lT2JqZWN0IHRoZW4KICAgICAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gZ2FtZU9iamVjdDpHZXRDb21wb25lbnRJbkNoaWxkcmVuKHR5cGVvZihDUy5SUEcuQ2xpZW50LkxvY2FsaXplZFRleHQpKQogICAgICAgIAogICAgICAgIGlmIHRleHRDb21wb25lbnQgdGhlbgogICAgICAgICAgICBsb2NhbCB1aWQgPSB0ZXh0Q29tcG9uZW50LnRleHQKICAgICAgICAgICAgCiAgICAgICAgICAgIGlmIG5vdCB1aWQ6ZmluZCgiR2xhemVQUyBVSUQ6IikgdGhlbgogICAgICAgICAgICAgICAgdWlkID0gdWlkOmdzdWIoIlVJRDoiLCAiPGI+PGNvbG9yPSNmZjllYzY+R2xhemVQUyBVSUQ6IDwvY29sb3I+PC9iPiIpCiAgICAgICAgICAgICAgICB0ZXh0Q29tcG9uZW50LnRleHQgPSAiPGI+PGNvbG9yPSMwMGRiZTU+IiAuLiB1aWQgLi4gIjwvY29sb3I+PC9iPiIKICAgICAgICAgICAgZW5kCiAgICAgICAgZW5kCiAgICBlbmQKZW5kCgp2ZXJzaW9uX3RleHQoKQo=")
                    });
                    break;
            }
        }
    }
}
