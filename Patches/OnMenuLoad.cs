using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MOAR.Patches
{
    public class OnMenuLoad : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Awake));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            foreach (var item in Plugin.GetPresetsList())
            {
                Plugin.DisplayMessage(item);
            };
            return true;
        }
    }
}
