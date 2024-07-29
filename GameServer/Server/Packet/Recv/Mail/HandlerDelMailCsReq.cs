using EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail;
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
    [Opcode(CmdIds.DelMailCsReq)]
    public class HandlerDelMailCsReq : Handler
    {
        public override void OnHandle(Connection connection, byte[] header, byte[] data)
        {
            var req = DelMailCsReq.Parser.ParseFrom(data);
            var player = connection.Player!;
            List<int> deleted = player.MailManager!.DeleteMail(req.IdList);
            connection.SendPacket(new PacketDelMailScRsp(deleted));
        }
    }
}
