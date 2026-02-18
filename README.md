# Ability System Tool (Unity) — MVP v1

ScriptableObject-driven ability/effect runtime for singleplayer Unity projects (no DOTS, no networking).  
Focus: modular design (SOLID-ish), low GC (no LINQ/alloc in hot paths), clean API, and a small playable demo.

---

## MVP v1 Scope (Included)

### 1) Data Model (ScriptableObjects)

**AbilitySO**

- `List<EffectSO> EffectList`
- Optional (demo-friendly): display name / description / icon (not required for MVP)

**EffectSO**

- `EffectDuration` (>= 0)
- `HasTick`, `TickInterval` (when `HasTick` is true)
- `StackingPolicy`: `Refresh` / `Replace` / `Stack`
- `EffectActionSO` reference (behavior implementation)
- Basic validation via `OnValidate()` (safe defaults)

**EffectActionSO (abstract ScriptableObject)**

- `OnApply(target, instanceId, effect)`
- `OnTick(target, instanceId, effect)` (only when tick-enabled)
- `OnExpire(target, instanceId, effect)`

---

### 2) Runtime Core

**AbilityRunner (caster / input demo)**

- Triggers casts (in the demo: `Space`)

**AbilitySystemComponent (on the target)**

- Owns and updates active effects for that target
- Entry point: `ApplyAbility(AbilitySO ability)`

**EffectRuntimeSystem (pure C# runtime)**

- Tracks `ActiveEffect` instances (duration / tick timers / instanceId)
- Handles stacking policies and lifecycle calls

**Lifecycle**

- `OnApply` → `OnTick` (optional) → `OnExpire`
- Time progression through an update loop

---

### 3) Effect Logic (Minimum “Useful” Set)

MVP includes real effect behavior (not logs only):

- **Damage Over Time (DoT)**
  - Applies damage on tick via `HealthComponent`
- **Stat Modifier (Move Speed)**
  - Applies a multiplier via `StatsComponent`
  - Reverts on expire

These two are enough to build classic effects like **Poison** and **Slow**.

---

### 4) Stacking Policies

- **Refresh**: resets `remainingDuration` of the existing instance
- **Replace**: expires the old instance, then applies a new instance (new `instanceId`)
- **Stack**: adds a new instance; each instance ticks/expires independently

---

### 5) Demo Scene + Sample Content

A small demo scene is included:

- Player casts one ability on a dummy target (Space key)
- Includes sample ScriptableObject assets (Ability / Effects / Effect Actions)

---

### 6) Debug / Developer Experience

- Consistent logs: `[CAST] [APPLY] [REFRESH] [REPLACE] [STACK] [TICK] [EXPIRE]` with `id=`
- Null guards
- Basic ScriptableObject validation (safe, non-spammy)

---

## Out of Scope (Not in MVP v1)

These are intentionally postponed to avoid delaying MVP:

- Networking / DOTS / multiplayer
- Targeting systems (cone/AoE/raycast, etc.) — demo uses a manual target reference
- Meta progression / upgrades / random upgrades
- Heavy editor tooling (custom inspectors, graphs, timelines, simulators)
- Advanced stacking (per-source stacking, stack decay, priority rules, partial refresh rules)
- Deep performance optimizations (pooling, Burst/Jobs, bitsets)
- Save/Load of runtime state
- Resistance/Immunity systems
- Gameplay tags / event bus frameworks

---

## Quickstart / Demo: Poison Bolt (DoT + Slow)

### Open the demo scene

- `Assets/AbilitySystemTool/Demo/Scenes/Demo_PoisonBolt.unity`

### How to run

1. Open the scene above.
2. Press **Play**.
3. Press **Space** to cast **Poison Bolt** on the dummy target.

### What happens

- **Poison DoT** ticks every `1s` for `5s` (damage per tick: `10`)
- **Slow** applies for `3s` (move speed multiplier: `0.7`)
- Logs show the full lifecycle: `[CAST]`, `[APPLY]`, `[TICK]`, `[EXPIRE]`
  - Plus action logs like `[SLOW APPLY]` / `[SLOW EXPIRE]`

### Sample assets included

**Ability**

- `Assets/AbilitySystemTool/Demo/SampleAssets/AbilityAssets/Ability_PoisonBolt.asset`

**Effects**

- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectAssets/Effect_Poison_DoT.asset`
- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectAssets/Effect_Slow.asset`

**Effect Actions**

- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectActionAssets/Action_DamageOverTime_10.asset`
- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectActionAssets/Action_ModifyMoveSpeed_0p7.asset`

### Scene setup (reference)

**Player**

- `AbilityRunner` (casts on Space)

**DummyTarget**

- `AbilityTarget`
- `AbilitySystemComponent`
- `HealthComponent`
- `StatsComponent`

> Tip: To quickly list asset paths in the project, use `Tools/Export/Copy Asset Paths (Selection or Assets)`.
