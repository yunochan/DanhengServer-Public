using EggLink.DanhengServer.GameServer.Server.Packet.Send.Lineup;
using EggLink.DanhengServer.Internationalization;

namespace EggLink.DanhengServer.Command.Command.Cmd;

[CommandInfo(
    name: "lineup", 
    description: "Game.Command.Lineup.Desc", 
    usage: "Game.Command.Lineup.Usage", 
    permission: "player.lineup"
)]
public class CommandLineup : ICommand
{
    [CommandMethod("0 mp")]
    public async ValueTask SetLineupMp(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        var count = arg.GetInt(1);
        await arg.Target.Player!.LineupManager!.GainMp(count == 0 ? 2 : count);
        await arg.SendMsg(I18NManager.Translate("Game.Command.Lineup.PlayerGainedMp", count.ToString()));
    }

    [CommandMethod("0 heal")]
    public async ValueTask HealLineup(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        var player = arg.Target.Player!;
        foreach (var avatar in player.LineupManager!.GetCurLineup()!.AvatarData!.Avatars) avatar.CurrentHp = 10000;
        await player.SendPacket(new PacketSyncLineupNotify(player.LineupManager.GetCurLineup()!));
        await arg.SendMsg(I18NManager.Translate("Game.Command.Lineup.HealedAllAvatars"));
    }

    [CommandMethod("0 change")]
    public async ValueTask ChangeLineup(CommandArg arg)
    {
         var player = arg.Target.Player!;
        if (player == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }
       
        var lineupIndex = arg.GetInt(1);
        if(lineupIndex < 0 || lineupIndex > 9){
            await arg.SendMsg("Error:阵容编号范围 0~9");
            return;
        }
        if (await player.LineupManager!.SetCurLineup(lineupIndex))
        {
            await arg.SendMsg($"阵容修改成功，阵容编号 [0{lineupIndex + 1}]");
        }
        else
        {
            await arg.SendMsg($"阵容修改失败，阵容编号 [0{lineupIndex + 1}] 角色为空");
        }
        
    }
}