# Super Battle Tactics RE Summary

## Game Info
- Name: Super Battle Tactics
- Dev: Mobage (DeNA)
- Platforms: iOS, Android
- Package: com.mobage.ww.a1933.Super_Battle_Tactics_Android
- Version: 1.1.5 (Code: 1)
- Data Version: 3886
- Genre: Turn-based tactical strategy
- Status: Discontinued/Dead

## Tech Stack
- Engine: Unity (libunity.so, libmono.so)
- Lang: C# (Unity/Mono)
- DB: SQLite 3.x
- Arch: ARM (armeabi, armeabi-v7a), x86
- Integrations: Kamcord, Facebook, Google Play Services, ADM, Vungle, AppLovin, AdColony, Adjust, HyprMX

## File Structure
- Open-Battle-Tactics/
  - super-battle-tactics.zip (APK 52 MB)
  - AndroidManifest.xml
  - classes.dex
  - resources.arsc
  - assets/
    - dataModel.db (67 tables)
    - keyValue.db
    - lcd.json
    - api_key.txt
    - bin/Data/ (Unity assets 99 MB)
  - extracted_assets/
    - AudioClip/ (3 .wav)
    - Shader/ (6 shaders)
    - TextAsset/ (23 .txt)
    - Texture2D/ (21 .png)
  - lib/ (armeabi, armeabi-v7a, x86)
  - res/ (drawable, layout, raw)

## DB (dataModel.db)
- 67 tables

### Units (370)
- Schema: id, key_name, type, rarity, weapon_anim, blueprint_linkage_id, reward_type_id, reward_amount, research_time, unlock_tier, can_buy_direct, found_in_gacha, unit_index
- Types (14): Assault, Command, Operative, Helicopter, Exclusive (Assault/Command/Operative), Event (Air/Assault/Command/Operative), MEGA BOSS!
- Rarities: 1-5 (Common to Legendary)

### Abilities (34)
- Schema: id, key_name, key_description, ability_type, handler_id, action_type, action_boost_type, action_boost_value_a, action_boost_value_b, action_point, target_group, is_active, is_announcer, execution_order, unlock_tier, unlock_order, research_time, limit_one_per_battle, limit_one_per_round, num_kill_unit, icon_linkage_id, selection_text
- Types (4): ULTRA (first-strike), PRE-COMBAT, REACTIVE, PASSIVE

### Events
- Schema: id, key_name, key_description, event_type, start_timestamp, end_timestamp

### Other Tables
- localization (14 langs)
- item
- config
- gacha_* (7 tables)
- leaderboards
- progression_*
- ai_*
- contracts
- help_*
- boost
- asset_bundles

## Assets
- Textures: 21 PNGs (UI, sprites, FX). Sizes 810B to 1.1MB (unnamed_3.png, unnamed_10.png, unnamed_11.png, unnamed_12.png)
- Audio: 3 WAVs (unnamed_12.wav, unnamed_13.wav, unnamed_36.wav). Status: Empty/0 bytes. Need re-extract.
- Shaders: 6 Unity shaders. Status: Empty. Need re-extract.
- Text: 23 files (configs, data).

## Languages
- Support 14: English (UK/variants), French, Chinese, Portuguese, German, Italian (variants), Spanish, Default.

## Game Mechanics
- Combat Phases: First-Strike, ULTRA, PRE-COMBAT, Attack, REACTIVE
- Initiative: Quick Strike
- Progress: Tiers, Research time, Unit levels, Divisions, Leaderboards
- Gacha: Pools, Plinko (config, prizes, odds), A/B testing
- Events: Time-limited, points_battle, special units, boosts, Raid Boss
- Economy: Gems, items, scrap value, blueprints, contracts, gifts
- AI: Armies, handlers, army parts

## Native Libs
- ARMv7: libmain.so, libunity.so, libmono.so, libsqlite3.so, libkamcord*.so, libNDKPlugin.so
- ARM/x86: Kamcord compat

## Resources
- Wiki: https://super-battle-tactics.fandom.com/wiki/Super_Battle_Tactics_Wiki
- MobyGames: https://www.mobygames.com/game/158792/super-battle-tactics/
- Behance:
  - https://www.behance.net/gallery/70749049/Super-Battle-Tactics-Game-Assets
  - https://www.behance.net/gallery/122971387/SUPER-BATTLE-TACTICS-(iOSAndroid)
- ArtStation:
  - https://www.artstation.com/artwork/L4BmdR
  - https://www.artstation.com/artwork/meWkZ
  - https://www.artstation.com/artwork/9KDXQ

## OS Project Needs
- Stack: Unity/Godot, C#/GDScript, SQLite, Node/Python API, Postgres, Redis
- Data Have: Units (370), Abilities (34), Rarity/Type stats, Locales, Config, Progress logic
- Data Need: Visuals, Audio, Exact combat logic, Animations, 3D/Sprites
- Tasks: Re-extract assets, dump SQLite to JSON, rebuild UI (use references), clone combat logic
- Legal: Educate/preserve only. Do NOT use original assets. Re-create art/audio. Change name. Use MIT/GPL-3.0.

## Links/Config
- Asset URL: http://files.mobage.com/files/battledice/asset_3836/
- Facebook App ID: 1393429944247618
- Permissions: Net, Storage, Phone State, Accounts, Play Billing, Push (GCM/ADM), Mic
