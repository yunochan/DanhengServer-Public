using EggLink.DanhengServer.Data;
using EggLink.DanhengServer.Database.Inventory;
using EggLink.DanhengServer.Enums.Avatar;
using EggLink.DanhengServer.Enums.Item;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.PlayerSync;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.Player;
using EggLink.DanhengServer.GameServer.Game.Player;
using EggLink.DanhengServer.Internationalization;
using EggLink.DanhengServer.Proto;

namespace EggLink.DanhengServer.Command.Command.Cmd
{
    [CommandInfo(
        name: "clear",
        description: "Game.Command.ClearAll.Desc",
        usage: "Game.Command.ClearAll.Usage",
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

            await player.InventoryManager!.RemoveItem(inventoryData.EquipmentItems, itemsToRemove);
            await player.InventoryManager!.RemoveItem(inventoryData.RelicItems, itemsToRemove);
            await player.InventoryManager!.RemoveItem(inventoryData.MaterialItems);

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
            await arg.SendMsg("已删除玩家全部角色");

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

            await player.InventoryManager!.RemoveItem(inventoryData.EquipmentItems, itemsToRemove);

            if (itemsToRemove.Count > 0)
            {
                await player.SendPacket(new PacketPlayerSyncScNotify(itemsToRemove));
                await arg.SendMsg("已删除玩家未锁定和未穿戴的光锥");
            }
            else
            {
                await arg.SendMsg("没有未锁定和未穿戴的光锥可删除");
            }
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

            await player.InventoryManager!.RemoveItem(inventoryData.RelicItems, player, itemsToRemove);

            if (itemsToRemove.Count > 0)
            {
                await player.SendPacket(new PacketPlayerSyncScNotify(itemsToRemove));
                await arg.SendMsg("已删除玩家未锁定和未穿戴的遗器");
            }
            else
            {
                await arg.SendMsg("没有未锁定和未穿戴的光锥可删除");
            }
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
                await player.InventoryManager!.RemoveItem(item.ItemId, item.Count, item.UniqueId);
            }

            await arg.SendMsg("已删除玩家全部材料");
        }
    }
}
