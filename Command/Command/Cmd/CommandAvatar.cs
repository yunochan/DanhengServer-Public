using EggLink.DanhengServer.Data;
using EggLink.DanhengServer.Database;
using EggLink.DanhengServer.Internationalization;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Server.Packet.Send.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using EggLink.DanhengServer.Util;
namespace EggLink.DanhengServer.Command.Cmd
{
    [CommandInfo("avatar", "Game.Command.Avatar.Desc", "Game.Command.Avatar.Usage")]
    public class CommandAvatar : ICommand
    {
        public static Logger logger = new("Command");
        [CommandMethod("all")]
        public void SetAll(CommandArg arg){
            if (arg.Target == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }
            arg.CharacterArgs.TryGetValue("p", out var rankStr);
            arg.CharacterArgs.TryGetValue("lv", out var levelStr);
            arg.CharacterArgs.TryGetValue("s", out var talentLevelStr);
            rankStr ??= "1";
            levelStr ??= "1";
            talentLevelStr ??= "1";
            // Debug
            logger.Info($"Received parameters: rank={rankStr}, level={levelStr}, talentLevel={talentLevelStr}");

            //此处执行逻辑，修改玩家已拥有的角色命座、角色等级、天赋等级
            if (!int.TryParse(rankStr, out var rank) || !int.TryParse(levelStr, out var level)||!int.TryParse(talentLevelStr, out var talentLevel))
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
                return;
            }
            // Debug
            logger.Info($"Parsed parameters: rank={rank}, level={level}, talent={talentLevel}");
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
            arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Rank"), rank.ToString()));
            arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Avatar"), level.ToString()));
            arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Talent"), talentLevel.ToString()));

            // sync
            player.SendPacket(new PacketPlayerSyncScNotify(player.AvatarManager.AvatarData.Avatars));
        }
        
        [CommandMethod("talent")]
        public void SetTalent(CommandArg arg)
        {
            if (arg.Target == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }
            if (arg.BasicArgs.Count < 2)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
                return;
            }
            var Player = arg.Target.Player!;
            // change basic type
            var avatarId = arg.GetInt(0);
            var level = arg.GetInt(1);
            if (level < 0 || level > 10)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.InvalidLevel", I18nManager.Translate("Word.Talent")));
                return;
            }
            var player = arg.Target.Player!;
            if (avatarId == -1)
            {
                player.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
                {
                    if (avatar.HeroId > 0)
                    {
                        avatar.SkillTreeExtra.TryGetValue(avatar.HeroId, out var hero);
                        hero ??= [];
                        var excel = GameData.AvatarConfigData[avatar.HeroId];
                        excel.SkillTree.ForEach(talent =>
                        {
                            hero[talent.PointID] = Math.Min(level, talent.MaxLevel);
                        });
                    } else
                    {
                        avatar.Excel?.SkillTree.ForEach(talent =>
                        {
                            avatar.SkillTree![talent.PointID] = Math.Min(level, talent.MaxLevel);
                        });
                    }
                });
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Talent"), level.ToString()));

                // sync
                player.SendPacket(new PacketPlayerSyncScNotify(player.AvatarManager.AvatarData.Avatars));

                return;
            }
            var avatar = player.AvatarManager!.GetAvatar(avatarId);
            if (avatar == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
                return;
            }
            avatar.Excel?.SkillTree.ForEach(talent =>
            {
                avatar.SkillTree![talent.PointID] = Math.Min(level, talent.MaxLevel);
            });

            // sync
            player.SendPacket(new PacketPlayerSyncScNotify(avatar));

            arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet", avatar.Excel?.Name?.Replace("{NICKNAME}", player.Data.Name) ?? avatarId.ToString(), I18nManager.Translate("Word.Talent"), level.ToString()));
        }

        [CommandMethod("get")]
        public void GetAvatar(CommandArg arg)
        {
            if (arg.Target == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            if (arg.BasicArgs.Count < 1)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            }

            var id = arg.GetInt(0);
            var excel = arg.Target.Player!.AvatarManager!.AddAvatar(id);

            if (excel == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarFailedGet", id.ToString()));
                return;
            }
            arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarGet", excel.Name ?? id.ToString()));
        }

        [CommandMethod("rank")]
        public void SetRank(CommandArg arg)
        {
            if (arg.Target == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            if (arg.BasicArgs.Count < 2)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
            }

            var id = arg.GetInt(0);
            var rank = arg.GetInt(1);
            if (rank < 0 || rank > 6)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.InvalidLevel", I18nManager.Translate("Word.Rank")));
                return;
            }
            if (id == -1)
            {
                arg.Target.Player!.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
                {
                    avatar.Rank = Math.Min(rank, 6);
                });
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Rank"), rank.ToString()));

                // sync
                arg.Target.SendPacket(new PacketPlayerSyncScNotify(arg.Target.Player!.AvatarManager.AvatarData.Avatars));
            }
            else
            {
                var avatar = arg.Target.Player!.AvatarManager!.GetAvatar(id);
                if (avatar == null)
                {
                    arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
                    return;
                }
                avatar.Rank = Math.Min(rank, 6);

                // sync
                arg.Target.SendPacket(new PacketPlayerSyncScNotify(avatar));

                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet", avatar.Excel?.Name?.Replace("{NICKNAME}", arg.Target.Player!.Data.Name) ?? id.ToString(), I18nManager.Translate("Word.Rank"), rank.ToString()));
            }
        }

        [CommandMethod("level")]
        public void SetLevel(CommandArg arg)
        {
            if (arg.Target == null)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.PlayerNotFound"));
                return;
            }

            if (arg.BasicArgs.Count < 2)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Notice.InvalidArguments"));
                return;
            }

            var id = arg.GetInt(0);
            var level = arg.GetInt(1);
            if (level < 1 || level > 80)
            {
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.InvalidLevel", I18nManager.Translate("Word.Avatar")));
                return;
            }

            if (id == -1)
            {
                arg.Target.Player!.AvatarManager!.AvatarData.Avatars.ForEach(avatar =>
                {
                    avatar.Level = Math.Min(level, 80);
                    avatar.Promotion = GameData.GetMinPromotionForLevel(avatar.Level);
                });
                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AllAvatarsLevelSet", I18nManager.Translate("Word.Avatar"), level.ToString()));

                // sync
                arg.Target.SendPacket(new PacketPlayerSyncScNotify(arg.Target.Player!.AvatarManager.AvatarData.Avatars));
            }
            else
            {
                var avatar = arg.Target.Player!.AvatarManager!.GetAvatar(id);
                if (avatar == null)
                {
                    arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarNotFound"));
                    return;
                }
                avatar.Level = Math.Min(level, 80);
                avatar.Promotion = GameData.GetMinPromotionForLevel(avatar.Level);

                // sync
                arg.Target.SendPacket(new PacketPlayerSyncScNotify(avatar));

                arg.SendMsg(I18nManager.Translate("Game.Command.Avatar.AvatarLevelSet", avatar.Excel?.Name?.Replace("{NICKNAME}", arg.Target.Player!.Data.Name) ?? id.ToString(), I18nManager.Translate("Word.Avatar"), level.ToString()));
            }
        }
    }
}
