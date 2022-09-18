using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace StationCrew.Settings
{
    public class ConfigOptions
    {
        public const string GeneralSection = "Section 01 - General";
        public ConfigEntry<bool> EnableAutoRepairOnDock { get; private set; }
        public ConfigEntry<bool> EnableAutoRepairOnStationApproach { get; private set; }
        public ConfigEntry<bool> EnableAutoTradeOnDock { get; private set; }
        public ConfigEntry<bool> EnableAutoTradeOnStationApproach { get; private set; }
        public ConfigEntry<bool> RemoveSellingStationInformation { get; private set; }
        public ConfigEntry<string> AutoTradeQuantity { get; private set; }
        public ConfigEntry<string> AutoStash { get; private set; }
        public ConfigEntry<float> StaffTradeFee { get; private set; }


        public const string HotkeysSection = "Section 02 - Hotkeys";
        public ConfigEntry<KeyboardShortcut> HotkeyAddTradeItem { get; private set; }
        public ConfigEntry<KeyboardShortcut> HotkeySellAllItem { get; private set; }
        public ConfigEntry<KeyboardShortcut> HotkeyAddStashItem { get; private set; }
        public ConfigEntry<KeyboardShortcut> HotkeyClearItem { get; private set; }
        public ConfigEntry<KeyboardShortcut> HotkeyClearAll { get; private set; }


        public static void LoadConfig(ConfigFile config)
        {
            // Section 01 - General

            Plugin.Settings.EnableAutoRepairOnDock = config.Bind(
                GeneralSection,
                nameof(EnableAutoRepairOnDock),
                true,
                "Enable Auto Repair on dock"
            );

            Plugin.Settings.EnableAutoRepairOnStationApproach = config.Bind(
                GeneralSection,
                nameof(EnableAutoRepairOnStationApproach),
                true,
                "Enable Auto Repair on approaching station"
            );

            Plugin.Settings.EnableAutoTradeOnDock = config.Bind(
                GeneralSection,
                nameof(EnableAutoTradeOnDock),
                true,
                "Enable Auto Buy/Sell on dock"
            );

            Plugin.Settings.EnableAutoTradeOnStationApproach = config.Bind(
                GeneralSection,
                nameof(EnableAutoTradeOnStationApproach),
                true,
                "Enable Auto Buy/Sell on approaching station"
            );

            Plugin.Settings.RemoveSellingStationInformation = config.Bind(
                GeneralSection,
                nameof(RemoveSellingStationInformation),
                true,
                "Remove selling station information when auto trading to keep items as 1 stack"
            );

            Plugin.Settings.AutoTradeQuantity = config.Bind(
                GeneralSection,
                nameof(AutoTradeQuantity),
                "",
                "Persist in game AutoTradeQuantity settings - do not recommend manual editing"
            );

            Plugin.Settings.AutoStash = config.Bind(
                GeneralSection,
                nameof(AutoStash),
                "",
                "Persist in game AutoStash settings - do not recommend manual editing"
            );

            Plugin.Settings.StaffTradeFee = config.Bind(
                GeneralSection,
                nameof(StaffTradeFee),
                0.01f,
                "Percentage fee that station staff charge on buying and selling, 0.01f = 1%"
            );


            // Section 02 - Hotkeys

            Plugin.Settings.HotkeyAddTradeItem = config.Bind(
                HotkeysSection,
                nameof(HotkeyAddTradeItem),
                new KeyboardShortcut(KeyCode.A),
                "Add wanted trade item quantity"
            );

            Plugin.Settings.HotkeySellAllItem = config.Bind(
                HotkeysSection,
                nameof(HotkeySellAllItem),
                new KeyboardShortcut(KeyCode.X, KeyCode.LeftControl),
                "Add wanted trade item quantity"
            );

            Plugin.Settings.HotkeyAddStashItem = config.Bind(
                HotkeysSection,
                nameof(HotkeyAddStashItem),
                new KeyboardShortcut(KeyCode.S),
                "Add item to stash"
            );

            Plugin.Settings.HotkeyClearItem = config.Bind(
                HotkeysSection,
                nameof(HotkeyClearItem),
                new KeyboardShortcut(KeyCode.F2),
                "Clear item auto actions"
            );

            Plugin.Settings.HotkeyClearAll = config.Bind(
                HotkeysSection,
                nameof(HotkeyClearAll),
                new KeyboardShortcut(KeyCode.F2, KeyCode.LeftControl),
                "Clear all item configs"
            );

        }
    }

}
