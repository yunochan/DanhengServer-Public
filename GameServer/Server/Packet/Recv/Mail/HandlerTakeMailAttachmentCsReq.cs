using EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail;
using EggLink.DanhengServer.Database.Mail;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Server;
using EggLink.DanhengServer.Server.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Recv.Mail
{
    [Opcode(CmdIds.TakeMailAttachmentCsReq)]
    public class HandlerTakeMailAttachmentCsReq : Handler
    {
        public override void OnHandle(Connection connection, byte[] header, byte[] data)
        {
            var req = TakeMailAttachmentCsReq.Parser.ParseFrom(data);
            var player = connection.Player!;
            List<MailInfo> attachments = player.MailManager!.TakeMailAttachments(req.MailIdList);

            connection.SendPacket(new PacketTakeMailAttachmentScRsp(attachments));
        }
        
    }
}
