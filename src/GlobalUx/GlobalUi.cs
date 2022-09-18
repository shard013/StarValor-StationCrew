using HarmonyLib;
using UnityEngine;

namespace StationCrew.GlobalUx
{
    public static class GlobalUi
    {
        public static Inventory Inventory
        {
            get
            {
                if (Inventory.instance == null)
                {
                    return null;
                }
                return Inventory.instance;
            }
        }

        public static PlayerUIControl PlayerUiControl
        {
            get
            {
                if (Inventory == null)
                {
                    return null;
                }

                var puc = Traverse.Create(Inventory).Field("puc").GetValue<PlayerUIControl>();
                if (puc == null)
                {
                    return null;
                }
                return puc;
            }
        }

        public static void UpdateUiComponents()
        {
            UpdatePlayerUiControl();
            UpdateCredits();
        }

        public static void UpdatePlayerUiControl()
        {
            PlayerUiControl?.UpdateUI();
        }

        public static void UpdateStationMarketUi()
        {
            var pc = Traverse.Create(Inventory).Field("pc").GetValue<PlayerControl>();
            if (pc == null || pc.dockingUI == null)
            {
                return;
            }

            var marketPanel = Traverse.Create(pc.dockingUI).Field("marketPanel").GetValue<GameObject>();
            if (marketPanel.activeSelf)
            {
                pc.dockingUI.OpenPanel(2); //marketPanel
            }
            else
            {
                pc.dockingUI.OpenPanel(3); //shipInfo
            }
        }

        public static void UpdateCredits()
        {
            Inventory?.UpdateCredits();
        }
    }
}
