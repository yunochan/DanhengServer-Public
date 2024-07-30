using EggLink.DanhengServer.Database.Mail;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Server.Packet;
using EggLink.DanhengServer.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail
{
    public class PacketTakeMailAttachmentScRsp : BasePacket
    {
        public PacketTakeMailAttachmentScRsp(IEnumerable<MailInfo> mailList) : base(CmdIds.TakeMailAttachmentScRsp)
        {
            var proto = new TakeMailAttachmentScRsp();
            
            foreach (var mail in mailList)
            {
                proto.SuccMailIdList.Add((uint)mail.MailID);
                
                if (mail.Attachment?.Items != null && mail.Attachment.Items.Count > 0)
                {
                    proto.Attachment.Add(mail.Attachment.ToProto());
                }
            }
            SetData(proto);
        }
    }
}