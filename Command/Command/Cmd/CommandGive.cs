using EggLink.DanhengServer.Data;
using EggLink.DanhengServer.Database.Inventory;
using EggLink.DanhengServer.Internationalization;
using EggLink.DanhengServer.Util;

namespace EggLink.DanhengServer.Command.Command.Cmd;

[CommandInfo("give", "Game.Command.Give.Desc", "Game.Command.Give.Usage")]
public class CommandGive : ICommand
{
    [CommandDefault]
        public async ValueTask execute(CommandArg arg)
        {
            // 检查参数是否包自定义圣遗物参数
            bool hasSpecialChar = arg.BasicArgs.Any(arg => arg.Contains('s') || arg.Contains(':'));

            if (hasSpecialChar)
            {
                await GiveRelic(arg);
            }
            else
            {
                await GiveItem(arg);
            }
        }

    public async ValueTask GiveItem(CommandArg arg)
    {
        var player = arg.Target?.Player;
        if (player == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        if (arg.BasicArgs.Count == 0)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }

        GameData.ItemConfigData.TryGetValue(arg.GetInt(0), out var itemData);
        if (itemData == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Give.ItemNotFound"));
            return;
        }

        arg.CharacterArgs.TryGetValue("x", out var str);
        arg.CharacterArgs.TryGetValue("lv", out var levelStr);
        arg.CharacterArgs.TryGetValue("r", out var rankStr);
        str ??= "1";
        levelStr ??= "1";
        rankStr ??= "1";
        if (!int.TryParse(str, out var amount) || !int.TryParse(levelStr, out var level) ||
            !int.TryParse(rankStr, out var rank))
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }

        await player.InventoryManager!.AddItem(arg.GetInt(0), amount, rank: Math.Min(rank, 5),
            level: Math.Max(Math.Min(level, 80), 1));

        await arg.SendMsg(I18nManager.Translate("Game.Command.Give.GiveItem", player.Uid.ToString(), amount.ToString(),
            itemData.Name ?? itemData.ID.ToString()));
    }

     public async ValueTask GiveRelic(CommandArg arg)
     {
         var player = arg.Target?.Player;
         if (player == null)
         {
             await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
             return;
         }

         if (arg.BasicArgs.Count < 3)
         {
             await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
             return;
         }

         arg.CharacterArgs.TryGetValue("x", out var str);
         arg.CharacterArgs.TryGetValue("lv", out var levelStr);
         arg.CharacterArgs.TryGetValue("s", out var mainAffixStr);
         str ??= "1";
         levelStr ??= "1";
         mainAffixStr ??= "1";
         if (!int.TryParse(str, out var amount) || !int.TryParse(levelStr, out var level) || !int.TryParse(mainAffixStr, out var mainAffixId))
         {
             await rg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
             return;
         }

         GameData.RelicConfigData.TryGetValue(int.Parse(arg.BasicArgs[0]), out var itemConfig);
         GameData.ItemConfigData.TryGetValue(int.Parse(arg.BasicArgs[0]), out var itemConfigExcel);
         if (itemConfig == null || itemConfigExcel == null)
         {
             await arg.SendMsg(I18nManager.Translate("Game.Command.Relic.RelicNotFound"));
             return;
         }

         GameData.RelicSubAffixData.TryGetValue(itemConfig.SubAffixGroup, out var subAffixConfig);
         GameData.RelicMainAffixData.TryGetValue(itemConfig.MainAffixGroup, out var mainAffixConfig);
         if (subAffixConfig == null || mainAffixConfig == null)
         {
             await arg.SendMsg(I18nManager.Translate("Game.Command.Relic.RelicNotFound"));
             return;
         }

         // 解析主属性
         if (GameData.RelicMainAffixData.Values.SelectMany(x => x.Keys).All(id => id != mainAffixId))
         {
             await arg.SendMsg(I18nManager.Translate("Game.Command.Relic.InvalidMainAffixId"));
             return;
         }
         
         // 解析副属性
         var remainLevel = 5;
         var subAffixes = new List<(int, int)>();
         for (var i = 3; i < arg.BasicArgs.Count; i++)
         {
             var subAffix = arg.BasicArgs[i].Split(':');
             if (subAffix.Length != 2 || !int.TryParse(subAffix[0], out var subId) || !int.TryParse(subAffix[1], out var subLevel))
             {
                 await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
                 return;
             }
             if (!subAffixConfig.ContainsKey(subId))
             {
                 await arg.SendMsg(I18nManager.Translate("Game.Command.Relic.InvalidSubAffixId"));
                 return;
             }
             subAffixes.Add((subId, subLevel));
             remainLevel -= subLevel - 1;
         }
         if (subAffixes.Count < 4)
         {
             // 随机副词条
             var subAffixGroup = itemConfig.SubAffixGroup;
             var subAffixGroupConfig = GameData.RelicSubAffixData[subAffixGroup];
             var subAffixGroupKeys = subAffixGroupConfig.Keys.ToList();
             while (subAffixes.Count < 4)
             {
                 var subId = subAffixGroupKeys.RandomElement();
                 if (subAffixes.Any(x => x.Item1 == subId))
                 {
                     continue;
                 }
                 if (remainLevel <= 0)
                 {
                     subAffixes.Add((subId, 1));
                 }
                 else
                 {
                     var subLevel = Random.Shared.Next(1, Math.Min(remainLevel + 1, 5)) + 1;
                     subAffixes.Add((subId, subLevel));
                     remainLevel -= subLevel - 1;
                 }
             }
         }

         var itemData = new ItemData()
         {
             ItemId = int.Parse(arg.BasicArgs[0]),
             Level = Math.Max(Math.Min(level, 9999), 1),
             UniqueId = ++player.InventoryManager!.Data.NextUniqueId,
             MainAffix = mainAffixId,
             Count = 1,
         };

         foreach (var (subId, subLevel) in subAffixes)
         {
             subAffixConfig.TryGetValue(subId, out var subAffix);
             var aff = new ItemSubAffix(subAffix!, 1);
             for (var i = 1; i < subLevel; i++)
             {
                 aff.IncreaseStep(subAffix!.StepNum);
             }
             itemData.SubAffixes.Add(aff);
         }

         for (var i = 0; i < amount; i++)
         {
             await player.InventoryManager!.AddItem(itemData, notify: false);
         }

         await arg.SendMsg(I18nManager.Translate("Game.Command.Relic.RelicGiven", player.Uid.ToString(), amount.ToString(), itemConfigExcel.Name ?? itemData.ItemId.ToString(), itemData.MainAffix.ToString()));
     }
}