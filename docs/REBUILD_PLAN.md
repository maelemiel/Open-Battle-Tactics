# Rebuild Plan (approved 2026-09-09)

- Decision: option B - clean TypeScript rewrite, AI-driven, keep only `game_data/` as source of truth. Old JS prototype (commit `dcb4d96^`, ~2100 real lines, simplified wrong combat logic) not restored.

## Stack
- Vite 8 + TypeScript strict + Phaser 3.90 + Vitest
- Pure engine (`src/engine/`, zero Phaser/DOM, deterministic via seeded PRNG)
- UI Phaser (`src/ui/`) consumes engine public API

## Architecture
```
src/engine/
  types.ts          # UnitDef, AbilityDef, CombatStats, DieFace (frozen contract)
  data/loader.ts    # loads game_data/*.json (DB = source of truth)
  combat/           # engine.ts (createBattle), events.ts, state.ts, initiative.ts, damage.ts, rng.ts
src/ui/scenes/BattleScene.ts + src/ui/UnitView.ts
tests/engine/       # data.test.ts + combat.test.ts (deterministic)
```

## Execution (agent pipeline)
1. data-mapper: locate real combat stats in DB -> freeze types.ts + loader + DATA_MAPPING.md
2. engine: state machine + events + tests
3. ui: BattleScene animated from events, vector-only visuals (no ripped assets)
4. integration: build + E2E browser check + docs + PR

## Key findings (from data-mapper)
- Combat stats live in `unit_level_progression` (hp + 5-face die per level), NOT in `unit`
- Face types: 20000 DAMAGE, 40000 INITIATIVE, 60000 SPECIAL, 70000 ARMOUR_PIERCING
- MVP units: VOLT 11001 (hp 20 L1), LONGSHOT 11002 (hp 19 L1)
- No separate attack/defense stat: everything transits through die faces

## MVP scope delivered
- 1v1 VOLT vs LONGSHOT, roll/resolve loop, initiative, face damage, victory/restart
- SPECIAL face recognized (stub), ARMOUR_PIERCING = damage (TODO)
- Engine 100% tested (23 tests), UI E2E verified in browser

## Next (future PRs)
- SPECIAL abilities routed to unit_special handlers
- Teams of N units, targeting
- Phases First-Strike/ULTRA/PRE-COMBAT/REACTIVE (enum defined, no-ops)
- Validate initiative rule (current face vs accumulated pool) against real gameplay
- Levels > 1, progression tables
