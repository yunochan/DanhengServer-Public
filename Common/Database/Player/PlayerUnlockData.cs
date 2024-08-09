using SqlSugar;

namespace EggLink.DanhengServer.Database.Player;

[SugarTable("UnlockData")]
public class PlayerUnlockData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true, ColumnDataType = "VARCHAR(12000)")]
    public List<int> HeadIcons { get; set; } = [];

    [SugarColumn(IsJson = true)] public List<int> ChatBubbles { get; set; } = [];

    [SugarColumn(IsJson = true)] public List<int> PhoneThemes { get; set; } = [];
}