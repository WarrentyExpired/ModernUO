# Exile Hunter's Bestiary - changelog & audit response

## Final stable state
This build is the final working configuration for the current ModernUO project state:
- No BaseCreature patch required.
- Death handling is done through generated death events.
- Drop result is resolved from combat tracker + killer resolution.
- Treatise is dropped into the corpse when available; otherwise, it is retried for a short window.
- Legacy partial-file patching is not used.

## Critical fixes applied
1. Treatise drop now lands in the corpse when the corpse exists.
2. Death processing no longer fails because the mob is already deleted by the time the event fires.
3. Auto-retry is used when corpse is not yet available immediately after death.
4. Duplicate processing of the same death is blocked by Serial-based guard.
5. Kill credit is resolved from LastKiller or combat tracker, not from dead/invalid objects.
6. Dead code and noisy debug logging were removed.
7. Registry validation is preserved.
8. Profile persistence remains compatible with XML save/load.

## Architecture
- Death hook is registered via generated event: `BaseCreature.CreatureDeathEvent`.
- Combat tracker records the last player damage event and the creature location/map.
- Bestiary drop logic is separated into:
  - Registry
  - Profile
  - Store
  - Engine
  - Combat tracker
  - Death hook
  - UI gump
  - Treatise item

## Balance
- Per-creature mastery bonus: +10%
- Category milestone bonus: up to +20%
- Total hard cap: +25%
- Pity timer: guaranteed treatise every 40 kills per creature
- Damage mode: Mark is default and patch-free

## Important note
The system does NOT require modifying `BaseCreature.cs` in the final version.
The working approach is:
- use combat tracker from `AggressiveAction`
- use generated death event
- resolve actual killer
- drop treatise into corpse or retry for a short time

## v3.2 - final stable build
- Removed old patch-based death handling and fallback partial overrides.
- Removed excessive debug logging from runtime combat flow.
- Death event is now the only actual drop trigger.
- Combat tracker stores last player hit and location before the mob is deleted.
- Treatise drop request waits a little if corpse is not immediately available.
- Final implementation is leaner and safer than patch-based variations.

## v3.3 - docs cleanup
- Removed outdated instructions about BaseCreature edits.
- Removed legacy mentions of "Variant A/B/C/D" patch hooks.
- Clarified that `HunterCreatureDeathEvent.cs` is the correct active death hook.
- Corrected troubleshooting around corpse timing in ModernUO.

## Current supported behavior
- Treatise drops only for registered creature types.
- Only players can receive credit.
- Summoned and bonded creatures are ignored.
- Global chance override: `exileBestiary.drop.chance`
- Default chance values are still taken from the registry if the global key is unset or set to -1.
- `exileBestiary.debug` is available for optional diagnostics, but production logs should stay clean.