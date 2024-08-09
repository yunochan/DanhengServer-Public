using SqlSugar;

namespace EggLink.DanhengServer.Database.UserManagement
{
    [SugarTable("BlackList")]
    public class BlackList
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)] public string IP { get; set; } = "";
        [SugarColumn(IsNullable = false)] public DateTime StartTime { get; set; }
        [SugarColumn(IsNullable = false)] public DateTime EndTime { get; set; }
        [SugarColumn(IsNullable = true)] public string? Msg { get; set; }
    }
}
