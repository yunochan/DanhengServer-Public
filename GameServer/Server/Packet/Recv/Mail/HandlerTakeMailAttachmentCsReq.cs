using EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail;
using EggLink.DanhengServer.Database.Mail;
using EggLink.DanhengServer.Kcp;
using EggLink.DanhengServer.Proto;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Recv.Mail;

[Opcode(CmdIds.TakeMailAttachmentCsReq)]
public class HandlerTakeMailAttachmentCsReq : Handler
{
    public override async Task OnHandle(Connection connection, byte[] header, byte[] data)
    {
        var req = TakeMailAttachmentCsReq.Parser.ParseFrom(data);
        var player = connection.Player!;
        List<MailInfo> attachments = await player.MailManager!.TakeMailAttachments(req.MailIdList);
        
        attachments ??= new List<MailInfo>();
        await connection.SendPacket(new PacketTakeMailAttachmentScRsp(attachments));
    }

}
