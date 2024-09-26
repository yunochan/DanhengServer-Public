using EggLink.DanhengServer.Util;
using Google.Protobuf;
using System;
using System.IO;

namespace EggLink.DanhengServer.Server.Packet
{
    public class BasePacket
    {
        private const uint HEADER_CONST = 0x9d74c714;
        private const uint TAIL_CONST = 0xd7a152c8;

        public ushort CmdId { get; private set; }
        public byte[] Data { get; private set; } = Array.Empty<byte>();

        public BasePacket(ushort cmdId) // 修正构造函数定义
        {
            CmdId = cmdId;
        }

        public void SetData(byte[] data)
        {
            Data = data;
        }

        public void SetData(IMessage message)
        {
            Data = message.ToByteArray();
        }

        public byte[] BuildPacket()
        {
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);

            bw.WriteUInt32BE(HEADER_CONST);
            bw.WriteUInt16BE(CmdId);
            bw.WriteUInt16BE(0); // 这里可能是长度占位
            bw.WriteUInt32BE((uint)Data.Length);
            if (Data.Length > 0)
            {
                bw.Write(Data);
            }
            bw.WriteUInt32BE(TAIL_CONST);

            return ms.ToArray();
        }
    }
}
