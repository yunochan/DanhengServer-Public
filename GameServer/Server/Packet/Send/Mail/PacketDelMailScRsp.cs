using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Server.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.GameServer.Server.Packet.Recv.Mail
{
    public class PacketDelMailScRsp : BasePacket
    {
        public PacketDelMailScRsp(List<int> deleteList) : base(CmdIds.DelMailScRsp)
        {
            var proto = new DelMailScRsp();
            proto.IdList.AddRange(deleteList.Select(id => (uint)id));
            SetData(proto);
        }
    }
}
