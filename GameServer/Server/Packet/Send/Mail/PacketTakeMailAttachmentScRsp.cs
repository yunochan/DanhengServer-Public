using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Server.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail
{
    public class PacketTakeMailAttachmentScRsp : BasePacket
    {
        public PacketTakeMailAttachmentScRsp(List<MailInfo> attachments) : base(CmdIds.TakeMailAttachmentScRsp)
        {
            var proto = new TakeMailAttachmentScRsp();
            proto.SuccMailIdList.AddRange(attachments.Select(mail => mail.MailID));

            SetData(proto);
        }
    }
}
