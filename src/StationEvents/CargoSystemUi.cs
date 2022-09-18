using HarmonyLib;
using StationCrew.AutoStation;
using StationCrew.GlobalUx;

namespace StationCrew.StationEvents
{
    [HarmonyPatch(typeof(CargoSystem), "Update")]
    public class CargoSystemUi
    {
        public static void Postfix(CargoSystem __instance)
        {
            if (!__instance.gameObject.activeInHierarchy)
            {
                return;
            }

            if (Inventory.instance == null)
            {
                return;
            }

            if (Traverse.Create(InputDialog.inst).Field("active").GetValue<bool>())
            {
                return;
            }

            var cargoItem = Inventory.instance.GetSelectedItemIndex();
            if (cargoItem == null)
            {
                return;
            }

            if (Plugin.Settings.HotkeySellAllItem.Value.IsDown())
            {
                UpdateItem(__instance, cargoItem, sellAll: true);
            }

            if (Plugin.Settings.HotkeyAddStashItem.Value.IsDown())
            {
                UpdateItem(__instance, cargoItem, stash: true);
            }

            if (Plugin.Settings.HotkeyClearItem.Value.IsDown())
            {
                UpdateItem(__instance, cargoItem, clear: true);
            }
        }

        public static void UpdateItem(CargoSystem cs, CargoItem cargoItem, bool sellAll = false, bool stash = false, bool clear = false)
        {
            var itemId = cargoItem.itemID;
            var itemType = cargoItem.itemType;

            if (itemType != 3) //only auto handle cargo items
            {
                GlobalSound.PlaySound(3);
                SideInfo.AddMsg("Auto Trade only possible on goods");
                Inventory.instance.DeselectItems();
                return;
            }

            if (itemId < 0)
            {
                return;
            }

            var itemRefName = ItemDB.GetItem(itemId).refName;
            if (string.IsNullOrEmpty(itemRefName))
            {
                return;
            }

            if (sellAll)
            {
                StationUtils.SetAutoItemQuantity(itemRefName, 0);
            }

            if (stash)
            {
                StationUtils.SetAutoStash(itemRefName);
            }

            if (clear)
            {
                StationUtils.ClearAutoItemQuantity(itemRefName);
            }

            var pc = Traverse.Create(cs).Field("pc").GetValue<PlayerControl>();
            var dockingUi = pc?.dockingUI;

            if (dockingUi?.playerDocked == true)
            {
                var ss = Traverse.Create(pc).Field("ss").GetValue<SpaceShip>();
                StationUtils.PlayerStationDock(dockingUi, ss);
                GlobalUi.UpdateStationMarketUi();
            }

            GlobalSound.PlayQueuedSound();
        }

    }
}
