# Dungeon Text Adventure

A small text-based dungeon adventure game built in C# as part of a group assignment focused on **software testing**.

## About the game

The player navigates through **6 rooms** in a dark dungeon, picks up items, fights a monster and tries to escape alive. Wrong choices lead to instant death, but the path is logical once you explore.

**Win condition:** collect the `Key` from the KeyRoom, then use it to unlock and enter the WinRoom (north of Start).

**Lose conditions:**
- Going west from Start → DeathRoom (instant death)
- Trying to leave the MonsterRoom while the Dragon is still alive → instant death
- Fighting the Dragon without a Sword → instant death
- Running out of HP during combat

## Room map

![Dungeon map](dungeon_map_dark.svg)

## Commands

| Command | Description |
|---|---|
| `help` | Show list of available commands |
| `look` | Show current room info, items, exits and your inventory |
| `inventory` | Show only your inventory |
| `go n\|e\|s\|w` | Move to another room |
| `take <item>` | Pick up an item from the current room |
| `fight` | Fight the monster in the current room |
| `quit` | Stop the game |

## Project structure

| Class | Responsibility |
|---|---|
| `Item` | Represents a pickable object (weapon, key, consumable) |
| `Monster` | Enemy in a room, may require a weapon to defeat |
| `Room` | A dungeon room with items, a monster, exits and special properties (deadly, locked, blocks exit) |
| `Inventory` | Holds the player's items, exposes `HasWeapon()` and `HasKey()` |
| `Player` | Tracks HP, inventory and win/alive state |
| `Direction` | Enum for the four directions (N, E, S, W) with a string parser |
| `Rooms` | Manages all rooms and the player's current position. Handles movement logic, death checks and combat |
| `GameSetup` | Creates the world: rooms, items, monsters and connections. Called once at startup |
| `Game` | Orchestrates game actions: move, take, fight, look, help, check win/game over |
| `Program` | Console interface. Reads player input and calls the right Game methods |

## Testing focus

This project emphasises testing over gameplay complexity:

- **Unit tests** — isolated tests for `Inventory`, `Room`, `Item`, `Monster`, `Direction`
- **Integration tests** — verify cooperation between `Player`, `Rooms` and `Game`
- **BDD / Gherkin** — full scenario tests: win path, lose by entering DeathRoom, lose by leaving monster alive, fight without sword

## Tech

- Language: **C#**
- Test framework: **NUnit**
- BDD: **SpecFlow** (Gherkin)
