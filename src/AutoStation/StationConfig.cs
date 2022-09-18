using StationCrew.GlobalUx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StationCrew.AutoStation
{
    public class StationConfig
    {
        public Dictionary<string, int> AutoTradeQuantity { get; set; } = new();
        public List<string> AutoStash { get; set; } = new();

        public StationConfig()
        {
            try
            {
                AutoTradeQuantityFromString(Plugin.Settings.AutoTradeQuantity.Value);
                AutoStashFromString(Plugin.Settings.AutoStash.Value);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Failed to read player config: {e.Message}");
            }
        }

        public bool SetAutoItemQuantity(string itemRefName, int quantity)
        {
            if (!AutoTradeQuantity.ContainsKey(itemRefName))
            {
                AutoStash.Remove(itemRefName);
                AutoTradeQuantity.Add(itemRefName, quantity);
            }
            else if (AutoTradeQuantity[itemRefName] != quantity)
            {
                AutoStash.Remove(itemRefName);
                AutoTradeQuantity[itemRefName] = quantity;
            }
            else
            {
                return false;
            }

            SaveToFile();
            return true;
        }

        public bool SetAutoStash(string itemRefName)
        {
            if (!AutoStash.Contains(itemRefName))
            {
                AutoTradeQuantity.Remove(itemRefName);
                AutoStash.Add(itemRefName);
            }
            else
            {
                return false;
            }

            SaveToFile();
            return true;
        }

        public bool ClearAutoItemQuantity(string itemRefName)
        {
            if (!AutoTradeQuantity.Remove(itemRefName) && !AutoStash.Remove(itemRefName))
            {
                return false;
            }

            SaveToFile();
            return true;
        }

        public void ClearAllAutoItem()
        {
            AutoTradeQuantity.Clear();
            AutoStash.Clear();
            SaveToFile();
        }

        private void SaveToFile()
        {
            Plugin.Settings.AutoTradeQuantity.Value = AutoTradeQuantityToString();
            Plugin.Settings.AutoStash.Value = AutoStashToString();
        }

        private string AutoTradeQuantityToString()
        {
            var data = AutoTradeQuantity.Select(i => $"{i.Key}:{i.Value}").ToList();
            string combinedString = string.Join(",", data.ToArray());
            return combinedString;
        }

        private string AutoStashToString()
        {
            string combinedString = string.Join(",", AutoStash.ToArray());
            return combinedString;
        }

        private void AutoTradeQuantityFromString(string combinedString)
        {
            AutoTradeQuantity.Clear();

            if (string.IsNullOrWhiteSpace(combinedString))
            {
                return;
            }

            var data = combinedString.Split(',').ToList();
            foreach (var item in data)
            {
                if (string.IsNullOrWhiteSpace(item) || !item.Contains(':'))
                {
                    continue;
                }
                var key = item.Split(':')[0];
                var value = int.Parse(item.Split(':')[1]);
                AutoTradeQuantity.Add(key, value);
            }
        }

        private void AutoStashFromString(string combinedString)
        {
            AutoStash.Clear();

            if (string.IsNullOrWhiteSpace(combinedString))
            {
                return;
            }

            var data = combinedString.Split(',').ToList();
            foreach (var item in data)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                AutoStash.Add(item);
            }
        }

    }
}
