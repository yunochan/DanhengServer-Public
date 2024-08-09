using EggLink.DanhengServer.Proto;
using SqlSugar;

namespace EggLink.DanhengServer.Database.Tutorial;

[SugarTable("TutorialGuide")]
public class TutorialGuideData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true, ColumnDataType = "TEXT")]
    public Dictionary<int, TutorialStatus> Tutorials { get; set; } = [];
}