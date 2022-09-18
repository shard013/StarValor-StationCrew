using HarmonyLib;
using StationCrew.AutoStation.Services;
using StationCrew.GlobalUx;
using System.Collections.Generic;
using System.Linq;

namespace StationCrew.AutoStation
{
    public static class StationUtils
    {
        private static readonly StationConfig Config = new();

        public static void PlayerStationDock(DockingUI dockingUi, SpaceShip ss)
        {
            var stationService = new StationService(dockingUi, ss);
            stationService.UpdateCargo();

            if (Plugin.Settings.EnableAutoRepairOnDock.Value)
            {
                RepairService.RepairShip(stationService);
            }

            if (Plugin.Settings.EnableAutoTradeOnDock.Value)
            {
                StashService.AutoStash(stationService, Config.AutoStash);
                SellService.AutoSell(stationService, Config.AutoTradeQuantity);
                BuyService.AutoBuy(stationService, Config.AutoTradeQuantity);
            }

            GlobalSound.PlayQueuedSound();
        }

        public static void PlayerStationProximity(DockingUI dockingUi, SpaceShip ss)
        {
            var stationService = new StationService(dockingUi, ss);
            stationService.UpdateCargo();

            if (Plugin.Settings.EnableAutoRepairOnStationApproach.Value)
            {
                RepairService.RepairShip(stationService);
            }

            if (Plugin.Settings.EnableAutoTradeOnStationApproach.Value)
            {
                StashService.AutoStash(stationService, Config.AutoStash);
                SellService.AutoSell(stationService, Config.AutoTradeQuantity);
                BuyService.AutoBuy(stationService, Config.AutoTradeQuantity);
            }

            GlobalSound.PlayQueuedSound();
        }

        public static Item GetItemFromRef(string itemRef)
        {
            var item = Traverse.Create<ItemDB>().Field("items").GetValue<List<Item>>().FirstOrDefault(i => i.refName == itemRef);
            return item;
        }

        public static void SetAutoItemQuantity(string itemRefName, int quantity)
        {
            if (!Config.SetAutoItemQuantity(itemRefName, quantity))
            {
                return;
            }

            GlobalSound.QueueSound(1);

            var item = GetItemFromRef(itemRefName);
            var itemName = ItemDB.GetItemNameModified(item, 0);

            if (quantity > 0)
            {
                SideInfo.AddMsg($"Auto Trade {itemName} x {quantity}");
            }
            else
            {
                SideInfo.AddMsg($"Auto Sell all {itemName}");
            }
        }

        public static void SetAutoStash(string itemRefName)
        {
            if (!Config.SetAutoStash(itemRefName))
            {
                return;
            }

            GlobalSound.QueueSound(1);

            var item = GetItemFromRef(itemRefName);
            var itemName = ItemDB.GetItemNameModified(item, 0);
            SideInfo.AddMsg($"Auto Stash {itemName}");
        }

        public static void ClearAutoItemQuantity(string itemRefName)
        {
            if (!Config.ClearAutoItemQuantity(itemRefName))
            {
                return;
            }

            GlobalSound.QueueSound(8);

            var item = GetItemFromRef(itemRefName);
            var itemName = ItemDB.GetItemNameModified(item, 0);
            SideInfo.AddMsg($"Cleared {itemName} Auto Trade");
        }

        public static void ClearAllAutoItem()
        {
            Config.ClearAllAutoItem();
        }

    }
}
