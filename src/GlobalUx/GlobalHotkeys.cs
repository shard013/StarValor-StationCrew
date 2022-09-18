using HarmonyLib;
using StationCrew.AutoStation;

namespace StationCrew.GlobalUx
{
    [HarmonyPatch(typeof(PlayerControl), "Update")]
    public static class GlobalHotkeys
    {
        public static void Postfix()
        {
            if (Plugin.Settings.HotkeyClearAll.Value.IsDown())
            {
                StationUtils.ClearAllAutoItem();
                SideInfo.AddMsg($"Cleared all Auto Items");
                GlobalSound.PlaySound(8);
            }
        }
    }
}
