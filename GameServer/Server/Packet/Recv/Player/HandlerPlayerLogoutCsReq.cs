using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EggLink.DanhengServer.Util;
namespace EggLink.DanhengServer.Server.Packet.Recv.Player
{
    [Opcode(CmdIds.PlayerLogoutCsReq)]
    public class HandlerPlayerLogoutCsReq : Handler
    {
        private static readonly Logger Logger = new("GameServer");
        public override void OnHandle(Connection connection, byte[] header, byte[] data)
        {
            connection.SendPacket(CmdIds.PlayerLogoutScRsp);
            connection.Stop();
            Logger.Debug("Stop方法在HandlerPlayerLogoutCsReq被执行，Connection was closed");

        }
    }
}
