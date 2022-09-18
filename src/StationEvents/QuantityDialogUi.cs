using HarmonyLib;
using StationCrew.AutoStation;
using StationCrew.GlobalUx;

namespace StationCrew.StationEvents
{
    [HarmonyPatch(typeof(InputDialog), "Update")]
    public class QuantityDialogUi
    {
        private static int Mode => Traverse.Create(InputDialog.inst).Field("mode").GetValue<int?>().Value;
        private static object NumberInput => Traverse.Create(InputDialog.inst).Field("numberInput").GetValue();
        private static string NumberInputText => Traverse.Create(NumberInput).Method("get_text").GetValue<string>();

        public static void Postfix(InputDialog __instance)
        {
            if (!Traverse.Create(__instance).Field("active").GetValue<bool>())
            {
                return;
            }

            if (Plugin.Settings.HotkeyAddTradeItem.Value.IsDown())
            {
                UpdateItem(__instance, trade: true);
            }

            if (Plugin.Settings.HotkeyAddStashItem.Value.IsDown())
            {
                UpdateItem(__instance, stash: true);
            }

            if (Plugin.Settings.HotkeyClearItem.Value.IsDown())
            {
                UpdateItem(__instance, clear: true);
            }
        }

        public static int GetInputQuantity()
        {
            if (NumberInput == null)
            {
                return -1;
            }
            if (NumberInputText == null)
            {
                return -1;
            }
            if (!int.TryParse(NumberInputText, out var quantity))
            {
                return -1;
            }
            return quantity;
        }

        public static void UpdateItem(InputDialog inputDialog, bool trade = false, bool stash = false, bool clear = false)
        {
            var mode = Mode;
            var market = Traverse.Create(Inventory.instance).Field("marketComponent").GetValue<Market>();
            var cs = Traverse.Create(Inventory.instance).Field("cs").GetValue<CargoSystem>();

            var itemId = -1;
            var itemType = 0; // 1 weapon - 2 equipment - 3 cargo - 4 ship - 5 passenger

            if (mode == 0) //buy
            {
                var itemIndex = Traverse.Create(market).Field("selectedItem").GetValue<int?>().Value;
                var item = market.market[itemIndex];
                itemId = item.itemID;
                itemType = item.itemType;
                Traverse.Create(market).Method("DeselectItems").GetValue();
            }

            if (mode == 1) //sell
            {
                var itemIndex = Traverse.Create(Inventory.instance).Field("selectedItem").GetValue<int?>().Value;
                var item = cs.cargo[itemIndex];
                itemId = item.itemID;
                itemType = item.itemType;
                Inventory.instance.DeselectItems();
            }

            if (itemType != 3) //only auto handle cargo items
            {
                GlobalSound.PlaySound(3);
                SideInfo.AddMsg("Auto Trade only possible on goods");
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

            if (trade)
            {
                var quantity = GetInputQuantity();
                if (quantity < 0)
                {
                    return;
                }
                StationUtils.SetAutoItemQuantity(itemRefName, quantity);
            }

            if (stash)
            {
                StationUtils.SetAutoStash(itemRefName);
            }

            if (clear)
            {
                StationUtils.ClearAutoItemQuantity(itemRefName);
            }

            var pc = Traverse.Create(inputDialog).Field("pc").GetValue<PlayerControl>();
            var dockingUi = pc.dockingUI;
            var ss = Traverse.Create(pc).Field("ss").GetValue<SpaceShip>();
            StationUtils.PlayerStationDock(dockingUi, ss);

            GlobalUi.UpdateStationMarketUi();

            GlobalSound.PlayQueuedSound();

            inputDialog.Close();
        }

    }
}
