using HarmonyLib;
using StationCrew.GlobalUx;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StationCrew.AutoStation.Services
{
    public static class SellService
    {
        public static void AutoSell(StationService stationService, IDictionary<string, int> items)
        {
            if (!stationService.PropertiesValid())
            {
                return;
            }

            foreach (var entry in items)
            {
                SellItemToQuantity(stationService, entry.Key, entry.Value);
            }

            stationService.CargoSystem.SortEverything();
        }

        public static void SellItemToQuantity(StationService stationService, string itemRefName, int quantity)
        {
            var item = StationUtils.GetItemFromRef(itemRefName);
            var itemId = item.id;

            if (Plugin.Settings.RemoveSellingStationInformation.Value)
            {
                stationService.MergeItems(itemId, stationService.CargoSystem);
            }

            var cargoItemCount = stationService.CargoSystem.cargo
                .FindAll(cargo => cargo.itemID == itemId && cargo.itemType == 3 && cargo.stockStationID == -1)
                .Count;

            if (cargoItemCount == 0)
            {
                return;
            }

            for (int i = 0; i < cargoItemCount; i++)
            {
                var cargoItems = stationService.CargoSystem.cargo
                    .FindAll(cargo => cargo.itemID == itemId && cargo.itemType == 3 && cargo.stockStationID == -1);
                var cargoQuantitySum = cargoItems.Sum(c => c.qnt);

                if (cargoQuantitySum > quantity)
                {
                    var sellQuantity = cargoQuantitySum - quantity;
                    SellCargoItem(stationService, itemId, sellQuantity);
                }
            }
        }

        public static void SellCargoItem(StationService stationService, int itemId, int quantity)
        {
            var ss = stationService.SpaceShip;
            var cs = stationService.CargoSystem;

            CargoItem cargoItem = cs.cargo
                .Where(c => c.itemID == itemId && c.itemType == 3 && c.stockStationID == -1)
                .OrderByDescending(c => c.qnt)
                .FirstOrDefault();

            var genericCargoItem = new GenericCargoItem(cargoItem.itemType, cargoItem.itemID, cargoItem.rarity, stationService.Station.market, null, null, cargoItem.extraData);
            if (cargoItem.itemType == 4)
            {
                genericCargoItem.unitPrice += cargoItem.GetShipLoadout().GetEquipmentValue(stationService.Station);
            }

            genericCargoItem.unitPrice = MarketSystem.GetTradeModifier(genericCargoItem.unitPrice, cargoItem.itemType, cargoItem.itemID, isSelling: true, stationService.Station.factionIndex, ss);
            if (cargoItem.itemType == 3 && cargoItem.sellerID == stationService.Station.id)
            {
                if (genericCargoItem.unitPrice > cargoItem.pricePaid)
                {
                    genericCargoItem.unitPrice = cargoItem.pricePaid;
                }
            }

            if (quantity > cargoItem.qnt)
            {
                quantity = cargoItem.qnt;
            }

            if (cargoItem.itemType == 4)
            {
                return; //do not try to sell ships
            }

            if (!(genericCargoItem.unitPrice > -1f))
            {
                return;
            }

            if (cs.RemoveItem(cs.cargo.IndexOf(cargoItem), quantity) < 0)
            {
                return; //was unable to remove items
            }

            cs.credits += genericCargoItem.unitPrice * quantity * (1f - Mathf.Abs(Plugin.Settings.StaffTradeFee.Value));
            bool flag = false;
            if (cargoItem.itemType == 3)
            {
                flag = cargoItem.itemID == 54;
                float num = cargoItem.pricePaid;
                bool flag2 = true;
                if (num == 0f)
                {
                    num = ItemDB.GetItem(cargoItem.itemID).basePrice * 0.9f;
                    flag2 = false;
                }

                float num2 = genericCargoItem.unitPrice * quantity - num * quantity;
                num2 *= 0.5f;
                if (num2 > 0f)
                {
                    int num3 = Mathf.RoundToInt(num2 * (1f - PChar.Char.level * 0.01f));
                    if (!flag2)
                    {
                        num3 /= 2;
                    }

                    PChar.EarnXP(num3, 4, -1);
                }
            }

            if (!flag &&
                stationService.Station.market != null &&
                cargoItem.rarity >= 1 &&
                MarketSystem.AlterItemStock(stationService.Station.market, cargoItem.itemType, cargoItem.itemID, cargoItem.rarity, quantity) < 0 
            )
            {
                MarketItem item = new(cargoItem.itemType, cargoItem.itemID, cargoItem.rarity, 1, cargoItem.extraData);
                stationService.Station.market.Add(item);
                MarketSystem.SortMarket(stationService.Station.market);
            }

            cs.UpdateAmmoBuffers();
            SideInfo.AddMsg($"Auto Sell {genericCargoItem.name} x {quantity}");
            GlobalUi.UpdateUiComponents();
            GlobalSound.QueueSound(20);
        }

    }
}
