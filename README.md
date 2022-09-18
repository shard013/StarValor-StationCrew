# Star Valor Station Crew - auto buy/sell/repair

An BepInEx plugin for [Star Valor](https://store.steampowered.com/app/833360/Star_Valor/).

## Overview

- When in range of a space station automatically:
    - Buy or sell cargo items to a specified amount
    - Move cargo items to your stash
    - Repair your ship

------------------------------

Installing Plugins
---
These mods require the BepInEx mod framework.
Install the latest 5.x (x86) release, see here for instructions: [BepinEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html)

**Important**: Star Valor is a 32-bit application. Make sure you choose the 32-bit version of BepinEx.

After installing BepInEx, download the latest release of the plugin (the .dll file) from the link below and put it in the BepInEx/plugins folder inside of your Star Valor game directory.

* As with all mods, make sure to **back up your save game** before trying a new mod.
* Only download mods from reliable sources: as with everything on the internet, be careful.

------------------------------

## Station Crew Mod Usage

All hotkey defaults mentioned below can be changed in `com.shard.station_crew.cfg` after running the mod once.

- Trade and Stash settings are persisted when closing the in the config
  - The same settings apply to all ships and all saves so be careful when swapping
  - Setting are saved in the following settings but this is not recommended to be edited manually:
    - `AutoTradeQuantity`
    - `AutoStash`


- Select a cargo item in your inventory and use the hotkeys to configure that item
  - Auto sell all: **`LeftCtrl+X`**
  - Stash all: **`S`**
  - Clear settings for this item: **`F2`**


- Set a specified amount to buy or sell an item to
  - Must be in the station and open the buy/sell # amount dialog for the wanted item
  - Enter the wanted amount
  - Press Keyboard hotkey: **`A`**


- Clear all auto buy/sell/stash configs at any time
  - **`LeftCtrl+F2`**


- The station crew charge a fee of 1% on all automatic buy and sell orders
  - Can be configured with the `StaffTradeFee` setting


- Different stacks of items can be merged together
  - Can be configured with the `RemoveSellingStationInformation` setting
  - Only applies on items that are moved by the station staff


- Staff will perform all actions when either docking or when in docking range of the station
  - Can be configured with the following settings:
    - `EnableAutoRepairOnDock`
    - `EnableAutoRepairOnStationApproach`
    - `EnableAutoTradeOnDock`
    - `EnableAutoTradeOnStationApproach`

------------------------------

## Developer : How to Build & Customize

**Minimum Requirement :**

- Visual Studio that Supports .NET Standard 2.0 and .NET library project, w/ dependencies fulfilled.

- [BepInEx NuGet repo](https://nuget.bepinex.dev/) as one of your Visual Studio NuGet source (https://nuget.bepinex.dev/v3/index.json)

**Dependencies**

This library project depends on several NuGet packages and a directly referred assembly/.dll file. While the NuGet packages are auto-resolved and auto-restored by Visual Studio, the assembly is not. The assembly is from :

- `Assembly-CSharp.dll` from the game's `Star Valor_Data\Managed\` folder

The project is configured to looks for the required assembly inside `<.csproj root folder>..\..\Libraries\Assembly-CSharp.dll` folder. You can fulfill those dependencies simply by copying the file into it.

Consult the build/Visual Studio error message or the `.csproj` file for the full list of the required dependencies.
