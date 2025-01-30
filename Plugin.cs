using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using MOAR.Helpers;
using MOAR.Patches;

namespace MOAR
{
    [
        BepInPlugin("MOAR.settings", "MOAR", "3.0.0"),
        BepInDependency("com.fika.core", BepInDependency.DependencyFlags.SoftDependency)
    ]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private void Start()
        {
            LogSource = Logger;

            Settings.Init(Config);
            Routers.Init(Config);

            // new SpawnPatch().Enable();
            // new SpawnPatch2().Enable();
            // new SpawnPatch3().Enable();
            new OnGameStartedPatch().Enable();
            new NotificationPatch().Enable();
            new NotificationPatch2().Enable();
        }

        private void Update()
        {
            if (Settings.DeleteBotSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.DeleteBotSpawn();
                    Methods.DisplayMessage(
                        "Deleted 1 bot spawn point from "
                            + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
                else
                {
                    Methods.DisplayMessage(
                        "DeleteBotSpawn failed: Not in Raid",
                        EFT.Communications.ENotificationIconType.Alert
                    );
                }
            }
            if (Settings.AddBotSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.AddBotSpawn();
                    Methods.DisplayMessage(
                        "Added 1 bot spawn point to "
                            + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
                else
                {
                    Methods.DisplayMessage(
                        "AddBotSpawn failed: Not in Raid",
                        EFT.Communications.ENotificationIconType.Alert
                    );
                }
            }

            if (Settings.AddPlayerSpawn.Value.BetterIsDown())
            {
                if (Singleton<GameWorld>.Instance.MainPlayer != null)
                {
                    Routers.AddPlayerSpawn();
                    Methods.DisplayMessage(
                        "Added 1 player spawn point to "
                            + Singleton<GameWorld>.Instance.MainPlayer.Location,
                        EFT.Communications.ENotificationIconType.Default
                    );
                }
                else
                {
                    Methods.DisplayMessage(
                        "AddPlayerSpawn failed: Not in Raid",
                        EFT.Communications.ENotificationIconType.Alert
                    );
                }
            }
        }
    };
}
