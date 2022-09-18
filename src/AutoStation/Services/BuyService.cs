using StationCrew.GlobalUx;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StationCrew.AutoStation.Services
{
    public static class BuyService
    {
        public static void AutoBuy(StationService stationService, IDictionary<string, int> items)
        {
            if (!stationService.PropertiesValid())
            {
                return;
            }

            foreach (var entry in items)
            {
                BuyItemToQuantity(stationService, entry.Key, entry.Value);
            }

            stationService.CargoSystem.SortEverything();
        }

        public static void BuyItemToQuantity(StationService stationService, string itemRefName, int quantity)
        {
            var item = StationUtils.GetItemFromRef(itemRefName);
            var itemId = item.id;

            var cargoItems = stationService.CargoSystem.cargo
                .FindAll(cargo => cargo.itemID == itemId && cargo.itemType == 3 && cargo.stockStationID == -1);

            if (cargoItems == null)
            {
                return;
            }

            var cargoQuantitySum = cargoItems.Sum(c => c.qnt);

            if (cargoQuantitySum < quantity)
            {
                var buyQuantity = quantity - cargoQuantitySum;
                BuyMarketItem(stationService, itemId, buyQuantity);
            }
        }

        public static void BuyMarketItem(StationService stationService, int itemId, int quantity)
        {
            var market = stationService.Station.market;
            var ss = stationService.SpaceShip;
            var cs = stationService.CargoSystem;

            if (market == null)
            {
                return;
            }

            MarketItem marketItem = market
                .FirstOrDefault(i => i.itemID == itemId && i.itemType == 3);

            if (marketItem == null)
            {
                return;
            }

            var genericCargoItem = new GenericCargoItem(marketItem.itemType, marketItem.itemID, marketItem.rarity, market, null, null, marketItem.extraData);

            if (genericCargoItem == null)
            {
                return;
            }

            genericCargoItem.unitPrice = MarketSystem.GetTradeModifier(
                genericCargoItem.unitPrice,
                marketItem.itemType,
                marketItem.itemID,
                isSelling: false,
                stationService.Station.factionIndex,
                ss
            );

            if (!genericCargoItem.hasReputationReq && GameData.data.gameMode != 1)
            {
                return;
            }

            if (quantity < 0 || cs.credits < genericCargoItem.unitPrice)
            {
                return;
            }

            if (quantity == 0)
            {
                quantity = (int)(cs.FreeSpace(marketItem.itemType == 5) / genericCargoItem.space);
            }

            if (quantity > marketItem.stock)
            {
                quantity = marketItem.stock;
            }

            if (quantity > cs.credits / genericCargoItem.unitPrice)
            {
                quantity = (int)(cs.credits / genericCargoItem.unitPrice);
            }

            if (quantity < 1)
            {
                return;
            }

            cs.credits -= genericCargoItem.unitPrice * quantity * (1f + Mathf.Abs(Plugin.Settings.StaffTradeFee.Value));
            if (marketItem.itemType == 3)
            {
                VerifyReputationGain(stationService.Station.factionIndex, genericCargoItem.unitPrice * quantity);
            }

            int itemStationId = stationService.Station.id;
            if (marketItem.itemType == 1)
            {
                itemStationId = -3;
            }

            if (marketItem.itemType == 2)
            {
                itemStationId = -4;
            }

            if (cs.FreeSpace(passengers: false) >= genericCargoItem.space * quantity)
            {
                itemStationId = -1;
            }
            else
            {
                SideInfo.AddMsg($"Not enough space to Auto Buy {genericCargoItem.name} x {quantity}");
                return;
            }

            if (marketItem.itemType == 4 && (int)ShipDB.GetModel(marketItem.itemID).shipClass >= ss.shipClass)
            {
                itemStationId = stationService.Station.id;
            }

            if (marketItem.itemType == 5 && cs.FreeSpace(passengers: true) >= genericCargoItem.space * quantity)
            {
                itemStationId = -1;
            }

            int sellerID = stationService.Station.id;
            if (marketItem.itemType != 3)
            {
                sellerID = -1;
            }

            cs.StoreItem(marketItem.itemType, marketItem.itemID, marketItem.rarity, quantity, genericCargoItem.unitPrice, sellerID, itemStationId, -1, marketItem.extraData);

            if (MarketSystem.AlterItemStock(market, market.IndexOf(marketItem), -quantity) == 0)
            {
                MarketSystem.SortMarket(market);
            }

            if (Plugin.Settings.RemoveSellingStationInformation.Value)
            {
                stationService.MergeItems(itemId, cs);
            }

            SideInfo.AddMsg($"Auto Buy {genericCargoItem.name} x {quantity}");
            GlobalUi.UpdateUiComponents();
            GlobalSound.QueueSound(20);
        }

        public static void VerifyReputationGain(int factionIndex, float value)
        {
            int num = Mathf.RoundToInt(value / (8f + PChar.Char.level * 2f));
            if (num > 1000)
            {
                num = 1000;
            }

            if (num > 0)
            {
                if (factionIndex == 2)
                {
                    PChar.AddRep(2, num * 2, reflect: false, 1, allowSkills: true);
                    return;
                }

                PChar.AddRep(factionIndex, num, reflect: false, 1, allowSkills: true);
                PChar.AddRep(2, num, reflect: false, 1, allowSkills: true);
            }
        }

    }
}
