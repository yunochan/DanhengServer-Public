using Microsoft.Data.Sqlite;
using SqlSugar;

namespace EggLink.DanhengServer.Database
{
    public abstract class BaseDatabaseDataHelper
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Uid { get; set; }
    }
}
