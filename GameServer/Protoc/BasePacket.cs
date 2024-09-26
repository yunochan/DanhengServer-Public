using EggLink.DanhengServer.Util;
using Google.Protobuf;
using System;
using System.IO;

namespace EggLink.DanhengServer.Server.Packet
{
    public class BasePacket
    {
        private const uint HEADER_CONST = 2641676052u;
        private const uint TAIL_CONST = 3617673928u;

        public ushort CmdId { get; set; }
        public byte[] Data { get; set; }

        public BasePacket(ushort cmdId)
        {
            CmdId = cmdId;
            Data = Array.Empty<byte>(); // 初始化 Data
        }

        public void SetData(byte[] data)
        {
            Data = data;
        }

        public void SetData(IMessage message)
        {
            Data = message.ToByteArray(); // 使用 IMessage 的 ToByteArray 方法
        }

        public byte[] BuildPacket()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            // 写入数据到内存流
            Extensions.WriteUInt32BE(binaryWriter, HEADER_CONST);
            Extensions.WriteUInt16BE(binaryWriter, CmdId);
            Extensions.WriteUInt16BE(binaryWriter, (ushort)0); // 长度占位
            Extensions.WriteUInt32BE(binaryWriter, (uint)Data.Length);
            if (Data.Length != 0)
            {
                binaryWriter.Write(Data);
            }
            Extensions.WriteUInt32BE(binaryWriter, TAIL_CONST);

            return memoryStream.ToArray(); // 返回构建的字节数组
        }
    }
}
