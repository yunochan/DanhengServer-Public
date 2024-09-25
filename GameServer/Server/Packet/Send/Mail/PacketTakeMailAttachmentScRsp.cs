using EggLink.DanhengServer.Database.Mail;
using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.GameServer.Game.Inventory;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail;
public class PacketTakeMailAttachmentScRsp : BasePacket
{
    public PacketTakeMailAttachmentScRsp(List<MailInfo> mailList) : base(CmdIds.TakeMailAttachmentScRsp)
    {
        var proto = new TakeMailAttachmentScRsp()
        {
            Attachment = new ItemList()
        };
            
		foreach (var mail in mailList)
		{
			proto.SuccMailIdList.Add((uint)mail.MailID);
		
			if (mail.Attachment != null && mail.Attachment.Items != null)
			{
				foreach (var item in mail.Attachment.Items)
				{
					if (item != null)
					{
						proto.Attachment.ItemList_.Add(item.ToProto());
					}
				}
			}
		}

		SetData(proto);
	}
}