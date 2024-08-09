using EggLink.DanhengServer.Enums.Scene;
using SqlSugar;

namespace EggLink.DanhengServer.Database.Scene;

[SugarTable("Scene")]
public class SceneData : BaseDatabaseDataHelper
{
    [SugarColumn(IsJson = true, ColumnDataType = "TEXT")]
    public Dictionary<int, Dictionary<int, List<ScenePropData>>> ScenePropData { get; set; } =
        []; // Dictionary<FloorId, Dictionary<GroupId, ScenePropData>>

    [SugarColumn(IsJson = true, ColumnDataType = "VARCHAR(4000)")]
    public Dictionary<int, List<int>> UnlockSectionIdList { get; set; } = []; // Dictionary<FloorId, List<SectionId>>

    [SugarColumn(IsJson = true, ColumnDataType = "VARCHAR(4000)")]

    public Dictionary<int, Dictionary<int, string>> CustomSaveData { get; set; } =
        []; // Dictionary<EntryId, Dictionary<GroupId, SaveData>>

    [SugarColumn(IsJson = true, ColumnDataType = "VARCHAR(12000)")]

    public Dictionary<int, Dictionary<string, int>> FloorSavedData { get; set; } =
        []; // Dictionary<FloorId, Dictionary<SaveDataKey, SaveDataValue>>
}

public class ScenePropData
{
    public int PropId;
    public PropStateEnum State;
}