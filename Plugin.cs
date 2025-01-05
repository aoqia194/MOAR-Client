using BepInEx;
using BepInEx.Logging;
using MOAR.Helpers;
using MOAR.Patches;

namespace MOAR
{
    [
        BepInPlugin("MOAR.settings", "MOAR", "2.6.7"),
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

            new NotificationPatch().Enable();
            new NotificationPatch2().Enable();
        }
    };
}
