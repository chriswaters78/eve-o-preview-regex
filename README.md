## Overview

The purpose of this application is to provide a simple way to keep an eye on several simultaneously running EVE Online clients and to easily switch between them. While running it shows a set of live thumbnails for each of the active EVE Online clients. These thumbnails allow fast switch to the corresponding EVE Online client either using mouse or configurable hotkeys.

It's essentially a task switcher, it does not relay any keyboard/mouse events and suchlike. The application works with EVE, EVE through Steam, or any combination thereof.

The program does NOT (and will NOT ever) do the following things:

* modify EVE Online interface
* display modified EVE Online interface
* broadcast any keyboard or mouse events
* anyhow interact with EVE Online except of bringing its main window to foreground or resizing/minimizing it

<div style="page-break-after: always;"></div>

**Under any conditions you should NOT use EVE-O Preview for any actions that break EULA or ToS of EVE Online.**

If you have find out that some of the features or their combination of EVE-O Preview might cause actions that can be considered as breaking EULA or ToS of EVE Online you should consider them as a bug and immediately notify the Developer ( Aura Asuna ) via in-game mail.

<div style="page-break-after: always;"></div>

## How To Install & Use

1. Download and extract the contents of the .zip archive to a location of your choice (ie: Desktop, CCP folder, etc)
..* **Note**: Please do not install the application into the *Program Files* or *Program files (x86)* folders. These folders in general do not allow applications to write anything there while EVE-O Preview now stores its configuration file next to its executable, thus requiring the write access to the folder it is installed into.
2. Start up both EVE-O Preview and your EVE Clients (the order does not matter)
3. Adjust settings as you see fit. Program options are described below

Video Guides:

* [Eve online , How To : EVE-O Preview (multiboxing; legal)](https://youtu.be/2r0NMKbogXU)


## System Requirements

* Windows 7, Windows 8/8.1, Windows 10
* Microsoft .NET Framework 4.6.2+
* EVE clients Display Mode should be set to **Fixed Window** or **Window Mode**. **Fullscreen** mode is not supported.

<div style="page-break-after: always;"></div>

## EVE Online EULA/ToS

This application is legal under the EULA/ToS:

CCP FoxFour wrote:
> Please keep the discussion on topic. The legitimacy of this software has already been discussed
> and doesn't need to be again. Assuming the functionality of the software doesn't change, it is
> allowed in its current state.

CCP Grimmi wrote:
> Overlays which contain a full, unchanged, EVE Client instance in a view only mode, no matter
> how large or small they are scaled, like it is done by EVE-O Preview as of today, are fine
> with us. These overlays do not allow any direct interaction with the EVE Client and you have
> to bring the respective EVE Client to the front/put the window focus on it, in order to
> interact with it.

<div style="page-break-after: always;"></div>

## Application Options

### Application Options Available Via GUI

#### **General** Tab
| Option | Description |
| --- | --- |
| Minimize to System Tray | Determines whether the main window form be minimized to windows tray when it is closed |
| Track client locations | Determines whether the client's window position should be restored when it is activated or started |
| Hide preview of active EVE client | Determines whether the thumbnail corresponding to the active EVE client is not displayed |
| Minimize inactive EVE clients | Allows to auto-minimize inactive EVE clients to save CPU and GPU |
| Previews always on top | Determines whether EVE client thumbnails should stay on top of all other windows |
| Hide previews when EVE client is not active | Determines whether all thumbnails should be visible only when an EVE client is active |
| Unique layout for each EVE client | Determines whether thumbnails positions are different depending on the EVE client being active |

#### **Thumbnail** Tab
| Option | Description |
| --- | --- |
| Opacity | Determines the inactive EVE thumbnails opacity (from almost invisible 20% to 100% solid) |
| Thumbnail Width | Thumbnails width. Can be set to any value from **100** to **640** points |
| Thumbnail Height | Thumbnails Height. Can be set to any value from **80** to **400** points |

#### **Zoom** Tab
| Option | Description |
| --- | --- |
| Zoom on hover | Determines whether a thumbnail should be zoomed when the mouse pointer is over it  |
| Zoom factor | Thumbnail zoom factor. Can be set to any value from **2** to **10** |
| Zoom anchor | Sets the starting point of the thumbnail zoom |

#### **Overlay** Tab
| Option | Description |
| --- | --- |
| Show overlay | Determines whether a name of the corresponding EVE client should be displayed on the thumbnail |
| Show frames | Determines whether thumbnails should be displays with window caption and borders |
| Highlight active client | Determines whether the thumbnail of the active EVE client should be highlighted with a bright border |
| Color | Color used to highlight the active client's thumbnail in case the corresponding option is set |

#### **Active Clients** Tab
| Option | Description |
| --- | --- |
| Thumbnails list | List of currently active EVE client thumbnails. Checking an element in this list will hide the corresponding thumbnail. However these checks are not persisted and on the next EVE client or EVE-O Preview run the thumbnail will be visible again |

<div style="page-break-after: always;"></div>

### Mouse Gestures and Actions

Mouse gestures are applied to the thumbnail window currently being hovered over.

| Action | Gesture |
| --- | --- |
| Activate the EVE Online client and bring it to front  | Click the thumbnail |
| Minimize the EVE Online client | Hold Control key and click the thumbnail |
| Switch to the last used application that is not an EVE Online client | Hold Control + Shift keys and click any thumbnail |
| Move thumbnail to a new position | Press right mouse button and move the mouse |
| Adjust thumbnail height | Press both left and right mouse buttons and move the mouse up or down |
| Adjust thumbnail width | Press both left and right mouse buttons and move the mouse left or right |

<div style="page-break-after: always;"></div>

### Configuration File-Only Options

Some of the application options are not exposed in the GUI. They can be adjusted directly in the configuration file.

**Note:** Do any changes to the configuration file only while the EVE-O Preview itself is closed. Otherwise the changes you made might be lost.

| Option | Description |
| --- | --- |
| **ActiveClientHighlightThickness** | <div style="font-size: small">Thickness of the border used to highlight the active client's thumbnail.<br />Allowed values are **1**...**6**.<br />The default value is **3**<br />For example: **"ActiveClientHighlightThickness": 3**</div> |
| **CompatibilityMode** | <div style="font-size: small">Enables the alternative render mode (see below)<br />The default value is **false**<br />For example: **"CompatibilityMode": true**</div> |
| **EnableThumbnailSnap** | <div style="font-size: small">Allows to disable thumbnails snap feature by setting its value to **false**<br />The default value is **true**<br />For example: **"EnableThumbnailSnap": true**</div> |
| **FlatLayout** | <div style="font-size: small">Position of each client's thumbnail window, written as **"&lt;exact window title&gt;": "x, y"**. Unlike the cycle groups the titles here are matched exactly, not as regular expressions. The application maintains this entry itself, saving the new position whenever a thumbnail is dragged somewhere, so it is only worth editing by hand to line thumbnails up precisely or to copy a layout between machines. Used whenever per-client thumbnail layouts are disabled, which is the default<br />For example: **"FlatLayout": { "EVE - Ondatra Patrouette": "2570, 600" }**</div> |
| **HideThumbnailsDelay** | <div style="font-size: small">Delay before thumbnails are hidden if the **General** -> **Hide previews when EVE client is not active** option is enabled<br />The delay is measured in thumbnail refresh periods<br />The default value is **2** (corresponds to 1 second delay)<br />For example: **"HideThumbnailsDelay": 2**</div> |
| **OrderPosition** | <div style="font-size: small">Where the EVE client window of each cycle group place is moved to when that client is activated, written as **"&lt;place&gt;": [ left, top, width, height ]** in screen pixels. Shared by both cycle groups. See **Cycle Clients with Hotkey Setup** below<br />The default value is an empty set **{}**, in which case no window is ever moved<br />For example: **"OrderPosition": { "1": [ 0, 0, 2560, 1400 ] }**</div> |
| **PriorityClients** | <div style="font-size: small">Allows to set a list of clients that are not auto-minimized on inactivity even if the **Minimize inactive EVE clients** option is enabled. Listed clients still can be minimized using Windows hotkeys or via _Ctrl+Click_ on the corresponding thumbnail<br />The default value is empty list **[]**<br />For example: **"PriorityClients": [ "EVE - Phrynohyas Tig-Rah", "EVE - Ondatra Patrouette" ]**</div> |
| **ThumbnailMinimumSize** | <div style="font-size: small">Minimum thumbnail size that can be set either via GUI or by resizing a thumbnail window. Value is written in the form "width, height"<br />The default value is **"100, 80"**.<br />For example: **"ThumbnailMinimumSize": "100, 80"**</div> |
| **ThumbnailMaximumSize** | <div style="font-size: small">Maximum thumbnail size that can be set either via GUI or by resizing a thumbnail window. Value is written in the form "width, height"<br />The default value is **"640, 400"**.<br />For example: **"ThumbnailMaximumSize": "640, 400"**</div> |
| **ThumbnailRefreshPeriod** | <div style="font-size: small">Thumbnail refresh period in milliseconds. This option accepts values between **300** and **1000** only.<br />The default value is **500** milliseconds.<br />For example: **"ThumbnailRefreshPeriod": 500**</div> |

<div style="page-break-after: always;"></div>

### Hotkey Setup

It is possible to set a key combinations to immediately jump to certain EVE window. However currently EVE-O Preview doesn't provide any GUI to set the these hotkeys. It should be done via editing the configuration file directly. Don't forget to make a backup copy of the file before editing it.

**Note**: Don't forget to make a backup copy of the file before editing it.

Open the file using any text editor. find the entry **ClientHotkey**. Most probably it will look like

    "ClientHotkey": {},

This means that no hotkeys are defined. Edit it to be like

    "ClientHotkey": {
      "EVE - Phrynohyas Tig-Rah": "F1",
      "EVE - Ondatra Patrouette": "F2"
    }

This simple edit will assign **F1** as a hotkey for Phrynohyas Tig-Rah and **F2** as a hotkey for Ondatra Patrouette, so pressing F1 anywhere in Windows will immediately open EVE client for Phrynohyas Tig-Rah if he is logged on.

The following hotkey is described as `modifier+key` where `modifier` can be **Control**, **Alt**, **Shift**, or their combination. F.e. it is possible to setup the hotkey as

    "ClientHotkey": {
      "EVE - Phrynohyas Tig-Rah": "F1",
      "EVE - Ondatra Patrouette": "Control+Shift+F4"
    }

**Note:** Do not set hotkeys to use the key combinations already used by EVE. It won't work as "_I set hotkey for my DPS char to F1 and when I'll press F1 it will automatically open the DPS char's window and activate guns_". Key combination will be swallowed by EVE-O Preview and NOT retranslated to EVE window. So it will be only "_it will automatically open the DPS char's window_".

<div style="page-break-after: always;"></div>

### Cycle Clients with Hotkey Setup

In a similar pattern to the per client Hotkey Setup, It is possible to set a key combinations to cycle through select Eve Windows. EVE-O Preview doesn't provide any GUI to set the these hotkeys. It should be done via editing the configuration file directly. Don't forget to make a backup copy of the file before editing it.

There are two cycle groups. Each group has its own pair of hotkeys and its own list of clients, and the two are matched completely independently, so a client can belong to one group, to both, or to neither. This is useful if you want one hotkey to cycle through a group of DPS characters while another cycles through support roles such as gate scouts, or a group of logi.

A cycle group can also place each client's window on the screen as it is activated. See **Arranging The Client Windows** below.

If you have not run EVE-O Preview before, or since this feature was added then it is recommended to quickly open and close EVE-O Preview once to trigger the config to update with some sample values.

A fully commented example configuration is included in the source as **EVE-O Preview.sample.json**. Copy it next to the executable as **EVE-O Preview.json** to try it out, but keep the commented copy somewhere else: the application rewrites its configuration file every time it closes, and the comments are lost when it does.

**Note**: Don't forget to make a backup copy of the file before editing it.

#### Hotkeys

Open the file using any text editor. find the entries **CycleGroup1ForwardHotkeys** and **CycleGroup1BackwardHotkeys**. Most probably it will look like

    "CycleGroup1ForwardHotkeys": [
      "NumPad2",
      "Control+F14"
    ],
    "CycleGroup1BackwardHotkeys": [
      "NumPad1",
      "Control+F13"
    ]

with the second group defaulting to **F16** / **Control+F16** and **F15** / **Control+F15**.

A comfortable alternative is the block of four keys in the top right corner of the numeric keypad, which puts both cycles under one hand and needs no modifier held down

        *  -        group 1 - the whole fleet
        9  +        group 2 - support only

the left key of each pair stepping forwards and the right key stepping back

    "CycleGroup1ForwardHotkeys": [
      "Multiply",
      "Control+Multiply"
    ],
    "CycleGroup1BackwardHotkeys": [
      "Subtract",
      "Control+Subtract"
    ],
    "CycleGroup2ForwardHotkeys": [
      "NumPad9",
      "Control+NumPad9"
    ],
    "CycleGroup2BackwardHotkeys": [
      "Add",
      "Control+Add"
    ]

Every key listed for a direction does the same thing, which is why each one is registered both with and without the Control modifier. Key names are the ones used by Windows itself, optionally prefixed with the modifiers **Control**, **Alt** and **Shift** joined by `+`. The keypad keys are named after the operation rather than the symbol printed on them

| Key | Name |
| --- | --- |
| **&#42;** | <div style="font-size: small">**"Multiply"**</div> |
| **&#47;** | <div style="font-size: small">**"Divide"**</div> |
| **&#45;** | <div style="font-size: small">**"Subtract"**</div> |
| **&#43;** | <div style="font-size: small">**"Add"**</div> |
| **0** ... **9** | <div style="font-size: small">**"NumPad0"** ... **"NumPad9"**</div> |

An unknown key name stops the application from starting.

**Note**: The keypad digits are only reported as **NumPad0**...**NumPad9** while **Num Lock is on**. With Num Lock off the same key reports itself as Page Up, Home and suchlike, and the hotkey will not fire. The **&#42;**, **&#47;**, **&#45;** and **&#43;** keys are not affected.

**Note**: It is highly recommended to pick keys that EVE itself does not use, and to bind them with a gaming device if you can support it.

#### Choosing Which Clients Cycle

Next find the entry **CycleGroup1ClientsOrder**. Most probably it will look like

    "CycleGroup1ClientsOrder": {
      "EVE - 1.*": 1,
      "EVE - 2.*": 2,
      "EVE - 3.*": 3
    }

Each key is a regular expression that is matched against the EVE window title, which reads `EVE - <character name>` once the character has logged in. The number on the right is the place that client takes in the cycle. You should modify this entry with an entry for each of your clients, replacing the patterns with something that matches your own character names.

Matching by pattern rather than by an exact character name means one entry can cover a whole naming convention, or several alts that you never fly at the same time.

| Pattern | Matches |
| --- | --- |
| **"^EVE - Main Toon$"** | <div style="font-size: small">Exactly that one character, and nothing else. The same behaviour as the older versions that took plain character names</div> |
| **"^EVE - (Booster Toon\|Backup Booster)$"** | <div style="font-size: small">Whichever of the two boosters happens to be logged in</div> |
| **"^EVE - DPS 1\\\\b"** | <div style="font-size: small">`EVE - DPS 1 Vexor`, but NOT `EVE - DPS 10 Vexor` - `\\b` is a word boundary</div> |
| **"^EVE - Scout"** | <div style="font-size: small">Every character whose name begins with `Scout`</div> |
| **"(?i)^eve - hauler\\\\b"** | <div style="font-size: small">The hauler, ignoring capitalisation - `(?i)` makes the rest of the pattern case-insensitive</div> |

Things worth knowing before writing your own:

* The match is a **substring** match. `"EVE - Main"` also matches `EVE - Main Toon Alt`, so anchor the pattern with `^` and `$` unless you want the loose match.
* Backslashes have to be escaped for JSON. A word boundary is written **"\\\\b"**, not `"\b"` - the latter is JSON's own escape for a backspace character and quietly produces a pattern that can never match.
* If a window title matches two patterns with **different** numbers the application reports an error, so keep the patterns mutually exclusive.
* Several patterns **may** share one number. Only log one of them in at a time, or the cycle will stall on the duplicated place.
* If a character appears in the list but is not currently logged in, then it will simply be skipped.
* If a character does not appear in the list, then they will never become active when cycling clients.
* A client sitting on the login screen is titled plain `EVE`, so any pattern starting with `^EVE - ` will ignore it until the character has logged in.
* A client's place is worked out once, when it is first seen under that title, and then remembered. Renaming a character, or editing the configuration file, has no effect until EVE-O Preview is restarted.

The second group is configured in exactly the same way using **CycleGroup2ForwardHotkeys**, **CycleGroup2BackwardHotkeys** and **CycleGroup2ClientsOrder**. F.e. this makes one hotkey walk the whole fleet and the other walk just the support alts

    "CycleGroup1ClientsOrder": {
      "^EVE - Main Toon$": 1,
      "^EVE - (Booster Toon|Backup Booster)$": 2,
      "^EVE - DPS 1\\b": 3,
      "^EVE - DPS 2\\b": 4,
      "^EVE - Scout": 5,
      "(?i)^eve - hauler\\b": 6
    },
    "CycleGroup2ClientsOrder": {
      "^EVE - (Booster Toon|Backup Booster)$": 2,
      "^EVE - Scout": 5,
      "(?i)^eve - hauler\\b": 6
    }

Note how the support characters keep the same numbers in both groups. Window positions are shared between the groups, so giving a client a different number in each group would drop it in a different place depending on which hotkey was pressed.

<div style="page-break-after: always;"></div>

#### Arranging The Client Windows

The number given to a client is also a screen slot. The entry **OrderPosition** says where the EVE client window for each slot is put

    "OrderPosition": {
      "1": [ 0, 0, 2560, 1400 ],
      "2": [ 0, 0, 2560, 1400 ],
      "5": [ 2560, 0, 960, 540 ]
    }

The four numbers are **left**, **top**, **width** and **height** in screen pixels. On a multiple monitor desktop the coordinates run across all of the monitors, which is why the third line above lands on a second screen placed to the right of a 2560 pixel wide primary one.

On each press of a cycle hotkey the application will

1. look up the place of the currently active client in the group that owns the hotkey. If the active client is not in that group then nothing happens at all;
2. walk to the next higher numbered client that is logged in, or the next lower one when cycling backwards, wrapping around at the ends;
3. activate that client and, if its window is not already at the top left corner of its slot, resize and move it to fit the slot exactly. Only the corner is compared, so a window that is in the right place but the wrong size is left alone;
4. when, and only when, the cycle wraps around from the last client back to the first, rebuild the whole stack first. Every other client in the group is activated in reverse order and moved to its own slot, with a short pause between each, before the client you are cycling to is activated last. This puts the windows back in the right order behind the one you end up on. Expect the screen to flicker through the other clients on that one press.

**OrderPosition** is optional and defaults to empty, in which case clients are activated but never moved. A slot with no entry of its own is left alone in the same way.

**Note**: The EVE client has to run in Windowed or Fixed Window mode to be positioned. A client in exclusive Fullscreen mode ignores being moved.

#### Turning It Off

Alternatively you may not want to use any of these HotKeys. Please note that deleting the values in their entirety will simply result in them being automatically re-generated.
Should you wish to remove these HotKeys completely, Simply set the values to empty, such as the example below:

      "CycleGroup1ForwardHotkeys": [],
	  "CycleGroup1BackwardHotkeys": [],
	  "CycleGroup1ClientsOrder": {},
	  "CycleGroup2ForwardHotkeys": [],
	  "CycleGroup2BackwardHotkeys": [],
	  "CycleGroup2ClientsOrder": {},
	  "OrderPosition": {}

**Note**: Use an empty list `[]` for the hotkeys, never `null`. A null hotkey list is not a valid setting.

**Hints** 
* Minimise the use of modifiers or standard keys to minimise issues with the client playing up. The examples above use the numeric keypad and unusual Function keys (e.g. F14), either of which can be bound to a game pad or gaming mouse.
* The Eve client can be somewhat less than stable, often getting confused as client focus switches. It is near certain that you will experience issues such as keys sticking or even in some cases D-Scan running each time the client swaps. So far I have found no perfect solution and opt for the most stable solution instead, of sticking to keys that EVE never sees any use for - the keypad block above, or the F14+ keys.
* For the best experience try to use the Control modifier. In the example above the keypad **&#42;** is used to cycle to the next client, but if pressed mid locking a target (Control + Clicking) then the client will not cycle. By registering **Control+Multiply** as an additional hotkey, the client will cycle either way.
* Number your clients once and reuse those numbers in both cycle groups, so that each client always lands in the same place on screen.
* Leave gaps in the numbering. Using 10, 20, 30 rather than 1, 2, 3 leaves room to slot another character in later without renumbering everything.
* For a list of supported keys, see: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys
* For a refresher on regular expression syntax, see: https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference

### Per Client Border Color
Have you ever wanted your main client to show up in a different color so that it more easily catches your eye? Or maybe your Logi to stand out?

EVE-O Preview doesn't provide any GUI to set the these per client overrides as yet. Though, It can be done via editing the configuration file directly. 
**Note** Don't forget to make a backup copy of the file before editing it.

Open the file using any text editor. find the entry **PerClientActiveClientHighlightColor**. Most probably it will look like

    "PerClientActiveClientHighlightColor": {
      "EVE - Example Toon 1": "Red",
      "EVE - Example Toon 2": "Green"
    }

You should modify this entry with a list of each of your clients replacing "Example Toon 1", etc with the name of your character. The names on the right represent which highligh color to use for that clients border.

If a client does not appear in this list, then it will use the global highlight color by default.

**Hint** For a list of supported colors see: https://docs.microsoft.com/en-us/dotnet/api/system.drawing.color#properties

### Compatibility Mode

This setting allows to enable an alternate thumbnail render. This render doesn't use advanced DWM API to create live previews. Instead it is a screenshot-based render with the following pros and cons:
* `+`  Should work even in remote desktop environments
* `-`  Consumes significantly more memory. In the testing environment EVE-O Preview did consume around 180 MB to manage 3 thumbnails using this render. At the same time the primary render did consume around 50 MB when run in the same environment.
* `-`  Thumbnail images are refreshed at 1 FPS rate
* `-`  Possible short mouse cursor freezes

<div style="page-break-after: always;"></div>

## Credits

### Maintained by

* Aura Asuna


### Created by

* StinkRay



### Previous maintainers

* Phrynohyas Tig-Rah
 
* Makari Aeron

* StinkRay


### With contributions from

* CCP FoxFour


### Forum thread

https://forums.eveonline.com/t/4202


### Original repository

https://bitbucket.org/ulph/eve-o-preview-git

<div style="page-break-after: always;"></div>

## CCP Copyright Notice

EVE Online, the EVE logo, EVE and all associated logos and designs are the intellectual property of CCP hf. All artwork, screenshots, characters, vehicles, storylines, world facts or other recognizable features of the intellectual property relating to these trademarks are likewise the intellectual property of CCP hf. EVE Online and the EVE logo are the registered trademarks of CCP hf. All rights are reserved worldwide. All other trademarks are the property of their respective owners. CCP hf. has granted permission to pyfa to use EVE Online and all associated logos and designs for promotional and information purposes on its website but does not endorse, and is not in any way affiliated with, pyfa. CCP is in no way responsible for the content on or functioning of this program, nor can it be liable for any damage arising from the use of this program. 

