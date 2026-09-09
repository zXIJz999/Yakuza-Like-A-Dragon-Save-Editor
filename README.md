# Yakuza LAD Save Editor

A save editor for Yakuza: Like a Dragon (PS4) decrypted saves. Built with DevExpress WinForms and .NET 9.

## Features

- **General** - Money, management funds, play time, difficulty, stomach, clothing, drunk level, battle style
- **Save Title & Subtitle** - Edit the PS4 save title and subtitle (not sure if this even works didnt test)
- **Save Icon** - Change the PS4 save icon (`icon0.png`), accepts any image format, auto-resized to 228x128 and converted to PNG
- **Characters** - Level, EXP, HP, heat, bonus stats for all party members
- **Jobs** - Per-character job levels and EXP
- **Inventory** - Carry items and item box with 744 named items
- **Points** - All 412 point entries
- **Management** - Company rank, target rank, turn count, period, stock price, scale
- **Scene** - Scene ID, config, stage, day/night
- **Karaoke** - Song scores, ranks, play counts
- **Dragon Kart** - Cart upgrade levels
- **Batting, Golf, Pinball, Mahjong, Can Collection, Cinema** - All editable values

Save title, subtitle, and icon editing only appear when the `sce_sys` folder is next to the save file.

## Important

This was a quick project, not much thought or effort went into it. I have done very little testing, only tested around 3 saves. There will probably be bugs.

You must use a **decrypted PS4 save**. The editor does not handle decryption, re-encryption, or resigning. You need to do that yourself. I suggest using [HTOS](https://discord.gg/htos) bot for that.

The editor preserves the exact file size of your decrypted save so re-encryption doesn't break.

## Usage

1. Decrypt your PS4 save
2. Open `data.sav` in the editor
3. Edit values across the tabs
4. Save or Save As
5. Re-encrypt and resign yourself

## Bugs

If there are bugs with the editor feel free to fix it yourself or DM me on Discord: **zxijz_**

Do NOT DM me "yo", "hey", etc. and wait for me to reply. I simply won't reply. Get straight to the point, state the bug and what led up to it. The more info the better.

## Credits

Thanks to [NextGenUpdate](https://nextgenupdate.com/forums/ps4-game-save-modding/1031045-yakuza-4-7-items.html) for the item IDs and names for the inventory.

By zXIJz

Enjoy.
