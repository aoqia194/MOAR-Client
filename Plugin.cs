using BepInEx;
using BepInEx.Logging;
using MOAR.Helpers;
using MOAR.Patches;

namespace MOAR
{
    [
        BepInPlugin("MOAR.settings", "MOAR", "1.0.0"),
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
        }
    };
}
