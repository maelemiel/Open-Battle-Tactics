/**
 * RNG streams for the combat engine.
 *
 * The original game uses THREE independent `MicrosoftRNG` streams per battle
 * (`BattleSetupUtils.cs:InitTeamRandom/InitBattleRandom` 96-106, wired in
 * `BattleController.cs:BeginBattle` 205-223):
 *   - battle stream: initiative coin toss ONLY (`BattleLogic.cs:CalculateInitiativeWinner` 281)
 *   - one stream PER TEAM: dice rolls, target selection, damage ranges, drops
 *     (`BattleLogic.cs:GetNextTeamRandom` 628-641)
 *
 * `MicrosoftRNG.cs` is the MSVC rand() LCG:
 *   seed = seed * 214013 + 2531011        (ulong, wraps mod 2^64)
 *   return (uint)(seed / 65536 % max)     (floor division; NO 0x7fff mask)
 * The counter (`GeneratedNumbers`) tracks how many numbers were drawn
 * (logged per draw with a reason string in the original for replay/anti-cheat).
 */

const MASK_64 = (1n << 64n) - 1n;

/** Byte-exact port of `MicrosoftRNG.cs` (MSVC rand() LCG, 64-bit state). */
export class MicrosoftRNG {
  private seed: bigint;
  private counter = 0;

  constructor(seed: number) {
    // C# ctor takes a uint; mirror the truncation for any JS number input.
    this.seed = BigInt(seed >>> 0);
  }

  /** Draws `from + Next(to - from)` exactly like `BattleLogic.GetNext*Random`. */
  nextRange(from: number, to: number): number {
    if (from === to) return from; // early return WITHOUT advancing the stream
    return from + this.next(to - from);
  }

  /**
   * Uniform integer in [0, max). Advances the stream even when max === 1
   * (the C# version increments its counter and advances the LCG first too).
   */
  next(max: number): number {
    if (max <= 0) throw new Error(`MicrosoftRNG.next: invalid max ${max}`);
    this.counter += 1;
    this.seed = (this.seed * 214013n + 2531011n) & MASK_64;
    return Number((this.seed >> 16n) % BigInt(max));
  }

  /** How many numbers this stream has generated (C# `GeneratedNumbers`). */
  get generatedNumbers(): number {
    return this.counter;
  }
}

/** The three independent streams of one battle. */
export interface BattleRngStreams {
  /** Coin toss stream (`battle.randomProvider`). */
  readonly battle: MicrosoftRNG;
  /** Player/host team stream (dice, targeting, drops). */
  readonly player: MicrosoftRNG;
  /** Enemy/guest team stream. */
  readonly enemy: MicrosoftRNG;
}

export interface BattleSeeds {
  readonly battle: number;
  readonly player: number;
  readonly enemy: number;
}

const UINT32_MOD = 0x1_0000_0000;

/**
 * Deterministic derivation of the three stream seeds from a single config
 * seed. The original receives three independent seeds from the server
 * (`MatchParser.cs:31,103`); nothing in the client derives one from another,
 * so this scheme is OUR documented convention:
 *
 *   battle = seed
 *   player = (2 * seed + 1) mod 2^32   (always odd)
 *   enemy  = (2 * seed + 2) mod 2^32   (always even)
 *
 * Player and enemy seeds therefore ALWAYS differ from each other. `battle`
 * may coincide with a team seed for at most one specific seed value, which
 * only makes two streams identical — harmless for correctness.
 */
export function deriveSeeds(seed: number): BattleSeeds {
  const s = seed >>> 0;
  return {
    battle: s,
    player: (2 * s + 1) % UINT32_MOD,
    enemy: (2 * s + 2) % UINT32_MOD,
  };
}

export function createRngStreams(seeds: BattleSeeds): BattleRngStreams {
  return {
    battle: new MicrosoftRNG(seeds.battle),
    player: new MicrosoftRNG(seeds.player),
    enemy: new MicrosoftRNG(seeds.enemy),
  };
}
