namespace StationCrew.AutoStation.Services
{
    public static class RepairService
    {
        public static void RepairShip(StationService stationService)
        {
            if (!stationService.PropertiesValid())
            {
                return;
            }

            var repairPrice = stationService.Station.GetRepairPrice(true, stationService.SpaceShip);

            if (repairPrice <= 0f)
            {
                return;
            }

            if (repairPrice > stationService.CargoSystem.credits)
            {
                SideInfo.AddMsg("Auto Repair Warning: Not enough credits to fully repair");
                return;
            }

            SideInfo.AddMsg($"Auto Repaired ship for {repairPrice:N0}");
            stationService.DockingUi.RepairShip(true);
        }
    }
}
