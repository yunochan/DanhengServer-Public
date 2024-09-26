using EggLink.DanhengServer.Data;
using EggLink.DanhengServer.Data.Custom;
using EggLink.DanhengServer.Internationalization;

namespace EggLink.DanhengServer.Command.Command.Cmd;

[CommandInfo(
    name: "reload", 
    description: "Game.Command.Reload.Desc", 
    usage: "Game.Command.Reload.Usage", 
    permission: "server.reload"
)]
public class CommandReload : ICommand
{
    [CommandMethod("0 banner")]
    public async ValueTask ReloadBanner(CommandArg arg)
    {
        // Reload the banners
        GameData.BannersConfig =
            ResourceManager.LoadCustomFile<BannersConfig>("Banner", "Banners") ?? new BannersConfig();
        await arg.SendMsg(I18NManager.Translate("Game.Command.Reload.ConfigReloaded",
            I18NManager.Translate("Word.Banner")));
    }

    [CommandMethod("0 activity")]
    public async ValueTask ReloadActivity(CommandArg arg)
    {
        // Reload the activities
        GameData.ActivityConfig = ResourceManager.LoadCustomFile<ActivityConfig>("Activity", "ActivityConfig") ??
                                  new ActivityConfig();
        await arg.SendMsg(I18NManager.Translate("Game.Command.Reload.ConfigReloaded",
            I18NManager.Translate("Word.Activity")));
    }
}