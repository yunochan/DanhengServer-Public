using EggLink.DanhengServer.Data;
using EggLink.DanhengServer.Enums.Avatar;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.Player;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.PlayerSync;
using EggLink.DanhengServer.Internationalization;
using EggLink.DanhengServer.Proto;

namespace EggLink.DanhengServer.Command.Command.Cmd;

[CommandInfo("avatar", "Game.Command.Avatar.Desc", "Game.Command.Avatar.Usage")]
public class CommandAvatar : ICommand
{
    [CommandMethod("all")]
    public async ValueTask SetAll(CommandArg arg){
        if (arg.Target == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }
        arg.CharacterArgs.TryGetValue("p", out var rankStr);
        arg.CharacterArgs.TryGetValue("lv", out var levelStr);
        arg.CharacterArgs.TryGetValue("s", out var talentLevelStr);
        rankStr ??= "1";
        levelStr ??= "1";
        talentLevelStr ??= "1";
 
        //此处执行逻辑，修改玩家已拥有的角色命座、角色等级、天赋等级
        if (!int.TryParse(rankStr, out var rank) || !int.TryParse(levelStr, out var level)||!int.TryParse(talentLevelStr, out var talentLevel))
        {
            arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }
        var player = arg.Target.Player!;
        player.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
        {
            avatar.Rank = Math.Max(Math.Min(rank, 6), 0);
            avatar.Level = Math.Max(Math.Min(level, 80), 0);
            avatar.Promotion = GameData.GetMinPromotionForLevel(avatar.Level);
            avatar.Excel?.SkillTree.ForEach(talent =>
            {
                avatar.SkillTree![talent.PointID] = Math.Min(talentLevel, talent.MaxLevel);
            });
        });
        // 向玩家发送通知
        await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Rank"), rank.ToString()));
        await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Avatar"), level.ToString()));
        await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Talent"), talentLevel.ToString()));
 
        // sync
        await player.SendPacket(new PacketPlayerSyncScNotify(player.AvatarManager.AvatarData.Avatars));
    }

    [CommandMethod("talent")]
    public async ValueTask SetTalent(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        if (arg.BasicArgs.Count < 3)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }

        // change basic type
        var avatarId = arg.GetInt(1);
        var level = arg.GetInt(2);
        if (level < 0 || level > 10)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.InvalidLevel",
                I18nManager.Translate("Word.Talent")));
            return;
        }

        var player = arg.Target.Player!;
        if (avatarId == -1)
        {
            player.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
            {
                if (avatar.PathId > 0)
                {
                    avatar.SkillTreeExtra.TryGetValue(avatar.PathId, out var hero);
                    hero ??= [];
                    var excel = GameData.AvatarConfigData[avatar.PathId];
                    excel.SkillTree.ForEach(talent => { hero[talent.PointID] = Math.Min(level, talent.MaxLevel); });
                }
                else
                {
                    avatar.Excel?.SkillTree.ForEach(talent =>
                    {
                        avatar.SkillTree[talent.PointID] = Math.Min(level, talent.MaxLevel);
                    });
                }
            });
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet",
                I18nManager.Translate("Word.Talent"), level.ToString()));

            // sync
            await player.SendPacket(new PacketPlayerSyncScNotify(player.AvatarManager.AvatarData.Avatars));

            return;
        }

        var avatar = player.AvatarManager!.GetAvatar(avatarId);
        if (avatar == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
            return;
        }

        avatar.Excel?.SkillTree.ForEach(talent =>
        {
            avatar.SkillTree[talent.PointID] = Math.Min(level, talent.MaxLevel);
        });

        // sync
        await player.SendPacket(new PacketPlayerSyncScNotify(avatar));

        await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet",
            avatar.Excel?.Name?.Replace("{NICKNAME}", player.Data.Name) ?? avatarId.ToString(),
            I18nManager.Translate("Word.Talent"), level.ToString()));
    }

    [CommandMethod("get")]
    public async ValueTask GetAvatar(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        if (arg.BasicArgs.Count < 2) await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));

        var id = arg.GetInt(1);
        var excel = await arg.Target.Player!.AvatarManager!.AddAvatar(id);

        if (excel == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarFailedGet", id.ToString()));
            return;
        }

        await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarGet", excel.Name ?? id.ToString()));
    }

    [CommandMethod("rank")]
    public async ValueTask SetRank(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        if (arg.BasicArgs.Count < 3) await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));

        var id = arg.GetInt(1);
        var rank = arg.GetInt(2);
        if (rank < 0 || rank > 6)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.InvalidLevel",
                I18nManager.Translate("Word.Rank")));
            return;
        }

        if (id == -1)
        {
            arg.Target.Player!.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
            {
                foreach (var path in avatar.PathInfoes.Values) path.Rank = Math.Min(rank, 6);
            });
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet",
                I18nManager.Translate("Word.Rank"), rank.ToString()));

            // sync
            await arg.Target.SendPacket(
                new PacketPlayerSyncScNotify(arg.Target.Player!.AvatarManager.AvatarData.Avatars));
        }
        else
        {
            var avatar = arg.Target.Player!.AvatarManager!.GetAvatar(id);
            if (avatar == null)
            {
                await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
                return;
            }

            foreach (var path in avatar.PathInfoes.Values) path.Rank = Math.Min(rank, 6);

            // sync
            await arg.Target.SendPacket(new PacketPlayerSyncScNotify(avatar));

            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet",
                avatar.Excel?.Name?.Replace("{NICKNAME}", arg.Target.Player!.Data.Name) ?? id.ToString(),
                I18nManager.Translate("Word.Rank"), rank.ToString()));
        }
    }

    [CommandMethod("level")]
    public async ValueTask SetLevel(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        if (arg.BasicArgs.Count < 3)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }

        var id = arg.GetInt(1);
        var level = arg.GetInt(2);
        if (level < 1 || level > 80)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.InvalidLevel",
                I18nManager.Translate("Word.Avatar")));
            return;
        }

        if (id == -1)
        {
            arg.Target.Player!.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
            {
                avatar.Level = Math.Min(level, 80);
                avatar.Promotion = GameData.GetMinPromotionForLevel(avatar.Level);
            });
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet",
                I18nManager.Translate("Word.Avatar"), level.ToString()));

            // sync
            await arg.Target.SendPacket(
                new PacketPlayerSyncScNotify(arg.Target.Player!.AvatarManager.AvatarData.Avatars));
        }
        else
        {
            var avatar = arg.Target.Player!.AvatarManager!.GetAvatar(id);
            if (avatar == null)
            {
                await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
                return;
            }

            avatar.Level = Math.Min(level, 80);
            avatar.Promotion = GameData.GetMinPromotionForLevel(avatar.Level);

            // sync
            await arg.Target.SendPacket(new PacketPlayerSyncScNotify(avatar));

            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet",
                avatar.Excel?.Name?.Replace("{NICKNAME}", arg.Target.Player!.Data.Name) ?? id.ToString(),
                I18nManager.Translate("Word.Avatar"), level.ToString()));
        }
    }

    [CommandMethod("path")]
    public async ValueTask SetPath(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        if (arg.BasicArgs.Count < 3)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            return;
        }

        var avatarId = arg.GetInt(1);
        var pathId = arg.GetInt(2);

        var avatar = arg.Target.Player!.AvatarManager!.GetAvatar(avatarId);
        if (avatar == null)
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
            return;
        }

        if (!GameData.MultiplePathAvatarConfigData.ContainsKey(pathId))
        {
            await arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
            return;
        }

        await arg.Target.Player.ChangeAvatarPathType(avatarId, (MultiPathAvatarTypeEnum)pathId);
        await arg.Target.SendPacket(new PacketAvatarPathChangedNotify((uint)avatarId, (MultiPathAvatarType)pathId));
        await arg.Target.SendPacket(new PacketPlayerSyncScNotify(avatar));

        // arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet", avatar.Excel?.Name?.Replace("{NICKNAME}", arg.Target.Player!.Data.Name) ?? id.ToString(), I18nManager.Translate("Word.Avatar"), level.ToString()));
    }
}