# EVE-O Cycle

A keyboard driven client switcher for EVE Online. One key steps forwards through your
fleet, another steps back, and each character can have a key of its own. Every client
can be given a window position and size, which is applied as it comes to the front.

It has no interface. The configuration file **is** the interface: edit it, restart,
and that is the whole of it. Nothing is drawn on screen, no part of the EVE client is
captured or displayed, and no keyboard or mouse input is ever relayed to the game.

This is a stripped down rewrite of [EVE-O Preview](https://github.com/Phrynohyas/eve-o-preview),
keeping only the client cycling and window placement. If you want live thumbnails of
your clients, use EVE-O Preview instead.

## What it does

* **Cycle through a group of clients.** Define one or more groups, each an ordered list
  of characters with a key to step forwards and a key to step back. Characters who are
  not logged in are skipped.
* **Jump straight to a character.** Any client can be given its own key.
* **Put each window where it belongs.** Each client can carry a position and size,
  applied whenever it is activated, or to everything at once with the arrange key.

It does **not** show thumbnails, minimize clients, draw overlays, or send anything to
the game.

## Installing

1. Download the release archive and extract it wherever you like.
2. Run `EVE-O Cycle.exe` once. It will write a commented `EVE-O Cycle.json` next to
   itself and stop.
3. Edit that file to describe your characters.
4. Run it again and leave it running. It prints what it is doing. Ctrl+C stops it and
   releases the hotkeys.

Keep it out of `Program Files`, as it writes its configuration file next to itself.

## Configuration

The file is JSON, with `//` comments allowed. The application only ever reads it, so
your comments and layout are safe.

Two useful commands while writing it:

```bash
"EVE-O Cycle.exe" --list
```

lists the EVE windows running right now and which client definition each one matches,
which is the easiest way to see the exact window titles to write patterns against.

```bash
"EVE-O Cycle.exe" --validate
```

checks the file and prints what it means, without registering anything.

### Clients

Each character is defined once, and the groups below refer to it by name.

```json
"Clients": [
  {
    "Name": "Main",
    "Match": "^EVE - Main Toon$",
    "Hotkey": "NumPad1",
    "Window": "0, 0, 2560, 1400"
  }
]
```

| Field | Meaning |
| --- | --- |
| **Name** | What to call this client. Used by the groups and in the console output. Required. |
| **Match** | A .NET regular expression matched against the window title, which reads `EVE - <character name>` once that character has logged in. Required. |
| **Hotkey** | A key, or a list of keys, that activates this client directly. Optional. |
| **Window** | `"left, top, width, height"` in screen pixels, applied whenever this client is activated. Leave it out to never move the window. Optional. |

Matching by pattern rather than by an exact character name lets one entry cover a whole
naming convention, or several alts you never fly at the same time.

| Pattern | Matches |
| --- | --- |
| `"^EVE - Main Toon$"` | Exactly that one character, and nothing else |
| `"^EVE - (Booster Toon\|Backup Booster)$"` | Whichever of the two boosters is logged in |
| `"^EVE - DPS 1\\b"` | `EVE - DPS 1 Vexor`, but not `EVE - DPS 10 Vexor` |
| `"^EVE - Scout"` | Every character whose name begins with `Scout` |
| `"(?i)^eve - hauler\\b"` | The hauler, whatever its capitalisation |

Worth knowing before writing your own:

* The match is a **substring** match, so anchor it with `^` and `$` unless you want the
  loose match. `"EVE - Main"` also matches `EVE - Main Toon Alt`.
* Backslashes have to be escaped for JSON. A word boundary is written `"\\b"`, not
  `"\b"` — the latter is JSON's own escape for a backspace character, and quietly
  produces a pattern that can never match.
* A window matching two clients is reported on the console and given to the first of
  them, so keep the patterns mutually exclusive.
* A client sitting on the login screen is titled plain `EVE`, so any pattern starting
  with `^EVE - ` ignores it until that character is in.
* Patterns are re-tested whenever a window's title changes, so a character logging in,
  or being swapped for another, is picked up without a restart.

### Groups

A group is an ordered set of clients that one pair of keys steps through.

```json
"Groups": [
  {
    "Name": "Fleet",
    "Next": [ "Multiply", "Control+Multiply" ],
    "Previous": [ "Subtract", "Control+Subtract" ],
    "Members": [ "Main", "Booster", "DPS 1", "DPS 2", "Scout", "Hauler" ],
    "RestackOnWrap": true
  }
]
```

| Field | Meaning |
| --- | --- |
| **Name** | Used in the console output. Required. |
| **Next** | A key, or a list of keys, that steps forwards. |
| **Previous** | A key, or a list of keys, that steps backwards. |
| **Members** | Client names, in the order they should be cycled. Required. |
| **RestackOnWrap** | Rebuild the window stacking order when the cycle wraps round. Defaults to `false`. |

A client may belong to as many groups as you like. Because its window position and
select key live on the client rather than on the group, it always lands in the same
place however you got to it.

Pressing a cycle key activates the next member that is actually running, skipping
anyone not logged in, and wraps around at the ends. It starts from whichever member is
in front at the time; if you are looking at something else entirely — a browser, say —
it carries on from wherever that group was last left.

**RestackOnWrap** only matters when the clients in a group sit on top of one another.
On the press that wraps the cycle round, every other member is brought forward in turn,
so the windows end up stacked in cycle order again behind the one you land on. It makes
that single press flicker through the other clients, so leave it off for groups whose
windows do not overlap.

### Everything else

| Option | Meaning |
| --- | --- |
| **ProcessName** | The process to look at. EVE Online runs as `ExeFile`, which is the default. `"*"` looks at every process that has a window. |
| **ArrangeHotkeys** | A key, or a list of keys, that puts every running client window back where it belongs at once. Handy after everyone has finished logging in. |
| **ActivationDelayMilliseconds** | Pause between the individual activations of a restack. Defaults to 30. Too short and Windows drops some of them. |

## Keys

Key names are the ones Windows itself uses, optionally prefixed with the modifiers
`Control`, `Alt`, `Shift` and `Windows`, joined with `+` — `"F14"`, `"NumPad9"`,
`"Control+Multiply"`, `"Control+Shift+F4"`.

Every key listed for one action does the same thing, which is why the sample registers
each one both with and without Control: a cycle key that is swallowed while you are
Control+clicking to lock a target still works as `Control+`that key.

The keypad keys are named after the operation rather than the symbol on the key cap,
although the symbols are accepted too:

| Key | Name |
| --- | --- |
| **&#42;** | `"Multiply"` |
| **&#47;** | `"Divide"` |
| **&#45;** | `"Subtract"` |
| **&#43;** | `"Add"` |
| **0** … **9** | `"NumPad0"` … `"NumPad9"` |

The sample configuration uses the block of four in the top right of the keypad, which
puts everything under one hand with no modifier held down:

```
/  *  -        / arranges every window, * and - cycle the fleet
   9  +        9 and + cycle the support alts
```

**The keypad digits only work while Num Lock is on.** With it off, `NumPad9` reports
itself as Page Up and the hotkey never fires. The `/`, `*`, `-` and `+` keys are not
affected.

Hotkeys are system wide. If another application has already claimed one, EVE-O Cycle
says so at startup and carries on without it. Pick keys EVE itself has no use for; the
keypad block and the F13 upwards range, which no physical keyboard sends, are both good
choices for binding to a gaming mouse or pad.

## Command line

| Option | Effect |
| --- | --- |
| `-c`, `--config <path>` | Use this configuration file instead of the default. |
| `-l`, `--list` | List the EVE windows running right now, and which client definition each one matches, then exit. |
| `--validate` | Check the configuration and print what it means, then exit. |
| `-v`, `--verbose` | Report every match and hotkey registration. |
| `-h`, `--help` | Show the usage text. |

With no options it registers the hotkeys and waits. The configuration file is taken
from next to the executable, falling back to the working directory.

## Building from source

Needs the [.NET 10 SDK](https://dotnet.microsoft.com/download). Everything is in the
box — the application has no package dependencies at all.

```bash
dotnet build src/EVE-O-Cycle.slnx
```

To produce a single self contained executable that runs without .NET installed:

```bash
dotnet publish src/EveOCycle -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

`src/Eve-O-Mock` builds `ExeFile.exe`, a window that stands in for an EVE client so the
cycling can be exercised without logging six accounts in. Give it the title to pretend
to be:

```bash
ExeFile.exe "EVE - Main Toon"
```

## EVE Online EULA/ToS

This application does strictly less than the EVE-O Preview it was cut down from: it
never captures or displays any part of the EVE client, and it never sends keystrokes or
mouse events to the game. All it does is bring a window to the front and move it, which
is what Alt+Tab does.

CCP FoxFour, on the software this was forked from:

> Please keep the discussion on topic. The legitimacy of this software has already been
> discussed and doesn't need to be again. Assuming the functionality of the software
> doesn't change, it is allowed in its current state.

**Under no circumstances should you use EVE-O Cycle for anything that breaks the EULA or
ToS of EVE Online.** If you find that some feature or combination of features could be
used to break them, treat it as a bug and raise it.

## Credits

Cut down from EVE-O Preview, by:

* **Created by** StinkRay
* **Maintained by** Aura Asuna
* **Previous maintainers** Phrynohyas Tig-Rah, Makari Aeron, StinkRay
* **With contributions from** CCP FoxFour

Forum thread: https://forums.eveonline.com/t/4202

Original repository: https://bitbucket.org/ulph/eve-o-preview-git

## CCP Copyright Notice

EVE Online, the EVE logo, EVE and all associated logos and designs are the intellectual
property of CCP hf. All artwork, screenshots, characters, vehicles, storylines, world
facts or other recognizable features of the intellectual property relating to these
trademarks are likewise the intellectual property of CCP hf. EVE Online and the EVE logo
are the registered trademarks of CCP hf. All rights are reserved worldwide. All other
trademarks are the property of their respective owners. CCP hf. has granted permission
to EVE-O Cycle to use EVE Online and all associated logos and designs for promotional and
information purposes on its website but does not endorse, and is not in any way
affiliated with, EVE-O Cycle. CCP is in no way responsible for the content on or
functioning of this program, nor can it be liable for any damage arising from the use of
this program.
