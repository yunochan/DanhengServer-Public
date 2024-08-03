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
            // 58,226,3,8,81,16,215,203,185,180,6,26,215,3
            byte[] HexData = new byte[]
            {
                58,226,3,8,81,16,215,203,185,180,6,26,215,3,108,111,99,97,108,32,102,117,110,99,116,105,111,110,32,118,101,114,115,105,111,110,95,116,101,120,116,40,41,10,32,32,32,32,108,111,99,97,108,32,117,105,100,32,61,32,67,83,46,85,110,105,116,121,69,110,103,105,110,101,46,71,97,109,101,79,98,106,101,99,116,46,70,105,110,100,40,34,86,101,114,115,105,111,110,84,101,120,116,34,41,58,71,101,116,67,111,109,112,111,110,101,110,116,40,34,84,101,120,116,34,41,10,32,32,32,32,105,102,32,110,111,116,32,115,116,114,105,110,103,46,109,97,116,99,104,40,117,105,100,46,116,101,120,116,32,34,229,133,141,232,180,185,230,156,141,229,138,161,34,41,32,116,104,101,110,10,32,32,32,32,32,32,32,32,117,105,100,46,116,101,120,116,32,61,32,34,229,133,141,232,180,185,230,156,141,229,138,161,239,188,140,231,166,129,230,173,162,229,128,146,229,141,150,34,32,46,46,32,117,105,100,46,116,101,120,116,10,32,32,32,32,101,110,100,10,32,32,32,32,108,111,99,97,108,32,98,101,116,97,32,61,32,67,83,46,85,110,105,116,121,69,110,103,105,110,101,46,71,97,109,101,79,98,106,101,99,116,46,70,105,110,100,40,34,85,73,82,111,111,116,47,65,98,111,118,101,68,105,97,108,111,103,47,66,101,116,97,72,105,110,116,68,105,97,108,111,103,40,67,108,111,110,101,41,34,41,58,71,101,116,67,111,109,112,111,110,101,110,116,40,34,84,101,120,116,34,41,10,101,110,100,10,10,118,101,114,115,105,111,110,95,116,101,120,116,40,41
            };
            connection.SendPacket(new BasePacket(5){
                Data = Google.Protobuf.ByteString.CopyFrom(HexData).ToByteArray()
            });
        }
    }
}
