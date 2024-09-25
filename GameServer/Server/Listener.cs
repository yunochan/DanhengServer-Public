using EggLink.DanhengServer.Kcp;

namespace EggLink.DanhengServer.GameServer.Server;

public class Listener : DanhengListener
{
    public static Connection? GetActiveConnection(int uid)
    {
        var con = Connections.Values.FirstOrDefault(c =>
            (c as Connection)?.Player?.Uid == uid && c.State == SessionStateEnum.ACTIVE) as Connection;
        return con;
    }
    public static int GetActiveUidCount()
   {
    // 使用 LINQ 查询获取所有连接中状态为 ACTIVE 的 UID 总数
    var activeUidCount = Connections.Values
        .Where(c => (c as Connection)?.State == SessionStateEnum.ACTIVE)
        .Select(c => (c as Connection)?.Player?.Uid)
        .Distinct()
        .Count(uid => uid != null);

    return activeUidCount;
   }
}