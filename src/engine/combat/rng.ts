/**
 * Seeded PRNG: every ounce of randomness in the combat engine flows through
 * this interface, so identical seeds replay identical battles.
 */
export interface Rng {
  /** Uniform float in [0, 1). */
  next(): number;
}

/** mulberry32: small, fast, decently distributed 32-bit PRNG. */
export function mulberry32(seed: number): Rng {
  let a = seed >>> 0;
  return {
    next(): number {
      a = (a + 0x6d2b79f5) >>> 0;
      let t = Math.imul(a ^ (a >>> 15), a | 1);
      t = (t ^ (t + Math.imul(t ^ (t >>> 7), t | 61))) >>> 0;
      return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
    },
  };
}

/** Uniform integer in [0, maxExclusive). Returns 0 when maxExclusive <= 1. */
export function nextInt(rng: Rng, maxExclusive: number): number {
  if (maxExclusive <= 1) return 0;
  return Math.floor(rng.next() * maxExclusive);
}
