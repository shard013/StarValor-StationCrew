using System;
using System.Linq;

namespace StationCrew.AutoStation
{
    public class StationService
    {
        public DockingUI DockingUi { get; }
        public SpaceShip SpaceShip { get; }

        public Station Station { get => DockingUi.station; }

        public CargoSystem CargoSystem { get => SpaceShip.cs; }

        public StationService(DockingUI dockingUi, SpaceShip spaceShip)
        {
            DockingUi = dockingUi ?? throw new ArgumentNullException(nameof(dockingUi));
            SpaceShip = spaceShip ?? throw new ArgumentNullException(nameof(spaceShip));
        }

        public void UpdateCargo()
        {
            if (!PropertiesValid())
            {
                return;
            }

            CargoSystem.UpdateAmmoBuffers();
        }

        public bool PropertiesValid()
        {
            if (DockingUi == null)
            {
                Plugin.Log.LogError("Error required property DockingUi was null");
                return false;
            }

            if (SpaceShip == null)
            {
                Plugin.Log.LogError("Error required property SpaceShip was null");
                return false;
            }

            if (Station == null)
            {
                Plugin.Log.LogError("Error required property Station was null");
                return false;
            }

            if (CargoSystem == null)
            {
                Plugin.Log.LogError("Error required property CargoSystem was null");
                return false;
            }

            return true;
        }

        public void MergeItems(int itemId, CargoSystem cs)
        {
            var totalItemQuantity = cs.cargo
                .Where(item => item.itemID == itemId && item.itemType == 3 && item.stockStationID == -1)
                .Sum(item => item.qnt);

            var mergeItem = cs.cargo
                .Where(item => item.itemID == itemId && item.itemType == 3 && item.stockStationID == -1)
                .FirstOrDefault();

            if (mergeItem != null)
            {
                mergeItem.qnt = totalItemQuantity;
                mergeItem.sellerID = -1;
                mergeItem.pricePaid = 0f;
                cs.cargo.RemoveAll(item => item.itemID == itemId && item.itemType == 3 && item.stockStationID == -1);
                cs.cargo.Add(mergeItem);
            }
        }

    }
}
