using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MOAR.ExamplePatches
{
    internal class MoarPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            var preset = Plugin.GetServerString();
            Plugin.PostMessage(preset);
            return true;
        }

        [PatchPostfix]
        static void Postfix()
        {
            // code in Postfix() method runs AFTER the original code is executed
        }
    }
}
