using StationCrew.GlobalUx;
using System.Collections.Generic;
using System.Linq;

namespace StationCrew.AutoStation.Services
{
    public static class StashService
    {
        public static void AutoStash(StationService stationService, IEnumerable<string> items)
        {
            if (!stationService.PropertiesValid())
            {
                return;
            }

            foreach (var itemRef in items)
            {
                StashItem(stationService, itemRef);
            }

            stationService.CargoSystem.SortEverything();
        }

        public static void StashItem(StationService stationService, string itemRefName)
        {
            var item = StationUtils.GetItemFromRef(itemRefName);

            if (Plugin.Settings.RemoveSellingStationInformation.Value)
            {
                stationService.MergeItems(item.id, stationService.CargoSystem);
            }

            var cargoItems = stationService.CargoSystem.cargo
                .FindAll(cargo => cargo.itemID == item.id && cargo.itemType == 3 && cargo.stockStationID == -1);

            if (cargoItems.Count == 0)
            {
                return;
            }

            var stationId = item.canBeStashed ? -2: stationService.Station.id;

            foreach (var cargoItem in cargoItems)
            {
                stationService.CargoSystem.StoreItem(
                    cargoItem.itemType,
                    cargoItem.itemID,
                    cargoItem.rarity,
                    cargoItem.qnt,
                    cargoItem.pricePaid,
                    cargoItem.sellerID,
                    stationId,
                    cargoItem.shipLoadoutID
                );
                stationService.CargoSystem.cargo.Remove(cargoItem);
            }
            var cargoName = ItemDB.GetItemNameModified(item, 0);
            var quantity = cargoItems.Sum(c => c.qnt);
            SideInfo.AddMsg($"Auto Stash {cargoName} x {quantity}");
            GlobalUi.UpdateUiComponents();
            GlobalSound.QueueSound(7);
        }

    }
}
