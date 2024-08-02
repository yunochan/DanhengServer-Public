using EggLink.DanhengServer.Game.Player;
using EggLink.DanhengServer.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.Server.Packet.Send.Others
{
    public class PacketClientDownloadDataScNotify : BasePacket
    {
        public PacketClientDownloadDataScNotify(byte[] data, PlayerInstance player) : base(CmdIds.ClientDownloadDataScNotify)
        {

            var clientDownloadData = new ClientDownloadData
            {
                Data = Google.Protobuf.ByteString.CopyFrom(data),
                Version = 81,
                Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var proto = new ClientDownloadDataScNotify
            {
                DownloadData = clientDownloadData
            };

            SetData(proto);
        }
    }
}