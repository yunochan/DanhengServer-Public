using SqlSugar;

namespace EggLink.DanhengServer.Database.UserManagement
{
    [SugarTable("Counter")]
    public class Counter
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; } = "Player";
        [SugarColumn(IsNullable = false)]
        public int NextUid { get; set; } = 100000001;
    }
}
