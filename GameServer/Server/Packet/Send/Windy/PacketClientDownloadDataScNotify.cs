using EggLink.DanhengServer.GameServer.Game.Player;
using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.GameServer.Protoc;
using Google.Protobuf;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Send.Windy;
public class PacketClientDownloadDataScNotify :BasePacket 
{
    public PacketClientDownloadDataScNotify( byte[] data , PlayerInstance player) : base(CmdId.ClientDownloadDataScNotify)
    {
        ClientDownloadData downloadData = new ClientDownloadData
		{
			Data = ByteString.CopyFrom(data),
			Version = 81u,
			Time = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
		};
		var proto = new ClientDownloadDataScNotify
		{
			DownloadData = downloadData
		};

		SetData(proto);
    }
}