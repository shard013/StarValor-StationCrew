using HarmonyLib;
using StationCrew.AutoStation;

namespace StationCrew.StationEvents
{
    [HarmonyPatch(typeof(DockingUI), nameof(DockingUI.StartDockingStation))]
    public class StationDock
    {
        public static void Postfix(DockingUI __instance, SpaceShip ___ss)
        {
            StationUtils.PlayerStationDock(__instance, ___ss);
        }
    }

}
