using EggLink.DanhengServer.Data;
using EggLink.DanhengServer.Database.Inventory;
using EggLink.DanhengServer.Enums.Avatar;
using EggLink.DanhengServer.Enums.Item;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.PlayerSync;
using EggLink.DanhengServer.GameServer.Game.Player;
using EggLink.DanhengServer.Internationalization;

namespace EggLink.DanhengServer.Command.Command.Cmd
{
    [CommandInfo(
        name: "clear",
        description: "Game.Command.Clear.Desc",
        usage: "Game.Command.Clear.Usage",
        permission: "player.clear"
    )]
    public class CommandClearall : ICommand
    {
        [CommandMethod("0 all")]
        public async ValueTask ClearAllItem(CommandArg arg)
        {
            var player = arg.Target?.Player;
            if (player == null)
            {
                await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            var inventoryData = player.InventoryManager!.GetInventoryData(player.Uid);
            var itemsToRemove = new List<ItemData>();

            await RemoveItems(inventoryData.EquipmentItems, player, itemsToRemove);
            await RemoveItems(inventoryData.RelicItems, player, itemsToRemove);
            await RemoveItems(inventoryData.MaterialItems, player);

            if (itemsToRemove.Count > 0)
            {
                await player.SendPacket(new PacketPlayerSyncScNotify(itemsToRemove));
            }

            await arg.SendMsg("已清空玩家的全部背包物品，包括武器、圣遗物和材料");
        }

        [CommandMethod("0 avatars")]
        public async ValueTask ClearAllAvatar(CommandArg arg)
        {
            var player = arg.Target?.Player;
            if (player == null)
            {
                await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            var avatarData = player.AvatarManager!.AvatarData;
            var avatarsToRemove = avatarData.Avatars.Where(avatar => avatar.AvatarId != 8001).ToList();

            foreach (var avatar in avatarsToRemove)
            {
                avatarData.Avatars.Remove(avatar);
            }

            await player.LineupManager!.SetDefaultLineup();
            await arg.SendMsg("已删除玩家全部角色(不包括主角)");

            await arg.Target!.Player!.SendPacket(new PacketPlayerKickOutScNotify());
            arg.Target!.Stop();
        }

        [CommandMethod("0 lightcones")]
        public async ValueTask ClearAllLightcone(CommandArg arg)
        {
            var player = arg.Target?.Player;
            if (player == null)
            {
                await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            var inventoryData = player.InventoryManager!.GetInventoryData(player.Uid);
            var itemsToRemove = new List<ItemData>();

            await RemoveItems(inventoryData.EquipmentItems, player, itemsToRemove);

            if (itemsToRemove.Count > 0)
            {
                await player.SendPacket(new PacketPlayerSyncScNotify(itemsToRemove));
            }

            await arg.SendMsg("已删除玩家未锁定和未穿戴的光锥");
        }

        [CommandMethod("0 relics")]
        public async ValueTask ClearAllRelic(CommandArg arg)
        {
            var player = arg.Target?.Player;
            if (player == null)
            {
                await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            var inventoryData = player.InventoryManager!.GetInventoryData(player.Uid);
            var itemsToRemove = new List<ItemData>();

            await RemoveItems(inventoryData.RelicItems, player, itemsToRemove);

            if (itemsToRemove.Count > 0)
            {
                await player.SendPacket(new PacketPlayerSyncScNotify(itemsToRemove));
            }

            await arg.SendMsg("已删除玩家未锁定和未穿戴的遗器");
        }

        [CommandMethod("0 materials")]
        public async ValueTask ClearAllMaterial(CommandArg arg)
        {
            var player = arg.Target?.Player;
            if (player == null)
            {
                await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            var inventoryData = player.InventoryManager!.GetInventoryData(player.Uid);
            var materialItems = inventoryData.MaterialItems
                .Where(x => x.ItemId > 0)
                .ToList();

            foreach (var item in materialItems)
            {
                await player.InventoryManager.RemoveItem(item.ItemId, item.Count, item.UniqueId);
            }

            await arg.SendMsg("已删除玩家全部材料");
        }

        private async Task RemoveItems(IEnumerable<ItemData> items, PlayerInstance player, List<ItemData> itemsToRemove = null)
        {
            foreach (var item in items.Where(x => x.ItemId > 0 && !x.Locked && x.EquipAvatar <= 0))
            {
                var removedItem = await player.InventoryManager.RemoveItem(item.ItemId, item.Count, item.UniqueId);
                if (removedItem != null && itemsToRemove != null)
                {
                    itemsToRemove.Add(removedItem);
                }
            }
        }
    }
}
