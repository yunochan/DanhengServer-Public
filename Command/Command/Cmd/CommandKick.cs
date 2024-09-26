﻿using EggLink.DanhengServer.GameServer.Server.Packet.Send.Player;
using EggLink.DanhengServer.Internationalization;

namespace EggLink.DanhengServer.Command.Command.Cmd;

[CommandInfo(
    name: "kick", 
    description: "Game.Command.Kick.Desc", 
    usage: "Game.Command.Kick.Usage", 
    permission: "server.kick"
)]
public class CommandKick : ICommand
{
    [CommandDefault]
    public async ValueTask Kick(CommandArg arg)
    {
        if (arg.Target == null)
        {
            await arg.SendMsg(I18NManager.Translate("Game.Command.Notice.PlayerNotFound"));
            return;
        }

        await arg.Target.SendPacket(new PacketPlayerKickOutScNotify());
        await arg.SendMsg(I18NManager.Translate("Game.Command.Kick.PlayerKicked", arg.Target.Player!.Data.Name!));
        arg.Target.Stop();
    }
}