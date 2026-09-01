---
<h2 align="center">.:[ Community | Support ]:.</h2>
<p align="center">
  <a href="https://discord.com/invite/U7AuQhu">
    <img src="https://img.shields.io/badge/Discord-Join-5865F2?style=for-the-badge&logo=discord&logoColor=white" />
  </a>
  <a href="https://ko-fi.com/goldkingz">
    <img src="https://img.shields.io/badge/Ko--fi-Support-FF5E5B?style=for-the-badge&logo=kofi&logoColor=white" />
  </a>
</p>

---

# [CS2] Auto-Delete-GoldKingZ (1.0.4)

Auto Delete Any Files And Folders From Any Given Paths On Every Map Change

<img width="740" height="465" alt="Auto-Delete-GoldKingZ" src="https://github.com/user-attachments/assets/0e047331-dbd7-4379-827e-019b4aed52ff" />


---

## 📦 Dependencies

[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-REQUIRED_TO_DOWNLOAD-red?logo=sourceengine&labelColor=2d2d2d)](https://www.sourcemm.net)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-REQUIRED_TO_DOWNLOAD-red?logo=github&labelColor=83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

[![JSON](https://img.shields.io/badge/JSON-INCLUDED_IN_ZIP-brightgreen?logo=json&labelColor=000000)](https://www.newtonsoft.com/json)

---

## 📥 Installation

### Plugin Installation
1. Download the latest `Auto-Delete-GoldKingZ.x.x.x.zip` release
2. Extract contents to your `csgo` directory
3. Configure settings in `Auto-Delete-GoldKingZ/config/config.json`
4. Restart your server

---

## ⚙️ Configuration

> [!IMPORTANT]
> **Main Configuration**  
> `../ESP-Players-GoldKingZ/config/config.json`  

## 🛠️ `config/config.json`
<details open>
<summary><b>Main Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `Reload_Plugin_CommandsInGame` | Commands to reload the plugin (console/chat by `!` or `css_`) | `Console_Commands:` `Chat_Commands:`<br>Both empty = Disable | - |
| `Reload_Plugin_Flags` | Restrict reload command to SteamIDs, Flags, Groups | `SteamIDs:` `Flags:` `Groups:`<br>All empty = Allow everyone | `Reload_Plugin_CommandsInGame` |
| `Reload_Plugin_Hide` | Hide chat after executing reload command | `0`-No<br>`1`-Only after successful toggle<br>`2`-Hide all the time | `Reload_Plugin_Flags` |

</details>
<details>
<summary><b>Auto Delete Config</b> (Click to expand 🔽)</summary>

`Auto_Delete_Settings` holds a list of paths, each one with its own delete rules.

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `Path` | Folder to work in, starting from the `game` folder | `csgo` -> game/csgo<br>`csgo/demos` -> game/csgo/demos<br>`../Steam/logs` -> Steam/logs<br>(`../` or `{back}/` goes back one folder) | - |
| `Files` | Which files to delete inside the path | `*`-Every file<br>`*.log`-Anything ending with .log<br>`backup_round*.txt`-Anything starting with backup_round and ending with .txt<br>`test.txt`-Only that exact file<br>Removed or empty = Delete nothing | - |
| `Folders` | Which folders to delete inside the path (folder and everything inside it) | `*`-Every folder<br>`temp*`-Any folder starting with temp<br>`cache`-Only that exact folder<br>Removed or empty = Delete nothing | - |
| `OlderThanXDays` | Only delete entries older than X days | `0`-No age check<br>e.g. `5`-Only older than 5 days | `Files` or `Folders` |

</details>
<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `EnableDebug` | Enable debug in server console (helps debug issues) | `true`/`false` | - |

</details>

---

## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>

### [1.0.4]
- Upgrade Net.10
- Change Plugin Name From Auto-Delete-Files-GoldKingZ To Auto-Delete-GoldKingZ
- CleanUp + Optimization
- Remove Debug From Release For Optimization
- Remove AutoDelete_Settings.json Added Into Main config.json `Auto_Delete_Settings`
- Rework On Debug Message When Delete/Skip/Failed
- Added `Folders` To Delete Folders On That Path
- Added Reload_Plugin Command
- Added {back} or ../ To go back From /game/

### [1.0.3]
- Upgrade Net.7 To Net.8
- Rework Auto Delete Files
- Added SendErrorLogsToServerConsole

### [1.0.2]
- Added "CounterstrikeSharpMoreThanXdaysOld"
- Added "BackupRoundPath"
- Added "DemoPath"

### [1.0.1]
- Fix Some Bugs
- Added "DemoMoreThanXdaysOld"

### [1.0.0]
- Initial plugin release

</details>

---
