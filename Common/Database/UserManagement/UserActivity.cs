using SqlSugar;

namespace EggLink.DanhengServer.Database.UserManagement
{
    [SugarTable("UserActivity")]
    public class UserActivity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)] public string IP { get; set; } = "";
        [SugarColumn(IsNullable = false)] public DateTime ActivityTime { get; set; }
        [SugarColumn(IsNullable = false)] public string? ActivityType { get; set; }
    }

}
