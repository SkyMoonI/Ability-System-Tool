# Ability System Tool – MVP v1 Scope (Feature List + Out of Scope)

## MVP v1 – Kesin olacaklar

### 1) Data Model (ScriptableObjects)

- **AbilitySO**
  - `List<EffectSO> effects`
  - (Opsiyonel) ability display name/description/icon (demo için)

- **EffectSO**
  - `duration` (0 = instant desteklenebilir)
  - `HasTick`, `TickInterval`
  - `StackingPolicy`: Refresh / Replace / Stack
  - (Stack için) `maxStacks` (opsiyonel: -1 = unlimited) _(istersen v1.1’e itebiliriz)_

### 2) Runtime Core

- **AbilityRunner / AbilitySystemComponent**
  - `Cast(AbilitySO ability, IAbilityTarget target)` benzeri tek giriş API (MVP’de target basit olabilir)

- **ActiveEffect runtime state**
  - `remainingDuration`
  - `timeUntilNextTick`
  - `instanceId` (debug için)

- **Lifecycle**
  - OnApply → OnTick (optional) → OnExpire
  - Update loop ile time progression

### 3) Effect Logic (Minimum)

MVP’nin “işe yarar” olması için effect’in sadece log atması yetmez. En az 1-2 gerçek davranış şart:

- **Stat Modifier (baseline)**
  - Örn: MoveSpeedMultiplier, DamageTakenMultiplier gibi basit bir stat seti
  - Apply’de stat değişir, Expire’da geri alınır

- **Damage over Time (tick)**
  - Tick’te hedefe damage uygular (basit health component)

> Not: Bu ikisi ile Poison/Slow gibi klasik efektler yapılır.

### 4) Stacking Policies (Gerçek davranış)

- **Refresh:** mevcut instance’ın `remainingDuration` reset (tick phase korunur: senin kararın)
- **Replace:** instance reinitialize (tick timer reset + state reset)
- **Stack:** yeni instance ekle (her biri ayrı expire/tick)

### 5) Demo Scene + Sample Content

- 1 sahne:
  - Player (AbilityRunner)
  - Dummy target (Health + Stats)
  - Space ile 1 ability cast

- Sample assets:
  - 1 AbilitySO (ör: “Poison Bolt”)
  - 2-3 EffectSO (Poison DoT, Slow, Instant Damage)

### 6) Debug / DX (Developer Experience)

- Log formatı tutarlı:
  - `[CAST]`, `[APPLY]`, `[REFRESH]`, `[REPLACE]`, `[STACK]`, `[TICK]`, `[EXPIRE]` + `id=`

- Null guard’lar, basic validation (TickInterval min vs)

---

## MVP v1 – Out of Scope (sonraya)

Bunlar “hemen gerekli değil”, v1’i geciktirir:

- Networking / DOTS / multiplayer
- Targeting sistemleri (cone, AoE, raycast vs) — MVP’de manuel target yeter
- Upgrade/Meta progression, random upgrades
- Editor tooling heavy:
  - custom inspectors, graphs, timeline, preview sim (v1 sonrası)

- Advanced stacking:
  - per-source stacking, stack decay, partial refresh rules, priority rules

- Optimizations beyond baseline:
  - pooling, burst, jobs, bitsets

- Save/Load, serialization runtime state
- Resistance/Immunity system
- Event bus / gameplay tags gibi kompleks sistemler

## Demo: Poison Bolt (DoT + Slow)

A minimal playable demo scene is included to showcase the MVP runtime loop (Cast → Apply → Tick → Expire) with real effect logic.

### Open the demo scene

- Scene: `Assets/AbilitySystemTool/Demo/Scenes/Demo_PoisonBolt.unity`

### How to run

1. Open the scene above.
2. Press **Play**.
3. Press **Space** to cast **Poison Bolt** on the dummy target.

### What happens

- **Poison DoT** ticks every `1s` for `5s` (damage per tick: `10`)
- **Slow** applies for `3s` (move speed multiplier: `0.7`)
- Logs show the full lifecycle: `[CAST]`, `[APPLY]`, `[TICK]`, `[EXPIRE]` (+ effect action logs like `[SLOW APPLY]` / `[SLOW EXPIRE]`)

### Sample assets included

**Ability**

- `Assets/AbilitySystemTool/Demo/SampleAssets/AbilityAssets/Ability_PoisonBolt.asset`

**Effects**

- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectAssets/Effect_Poison_DoT.asset`
- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectAssets/Effect_Slow.asset`

**Effect Actions**

- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectActionAssets/Action_DamageOverTime_10.asset`
- `Assets/AbilitySystemTool/Demo/SampleAssets/EffectActionAssets/Action_ModifyMoveSpeed_0p7.asset`

### Scene setup (for reference)

**Player**

- `AbilityRunner` (casts on Space)

**DummyTarget**

- `AbilityTarget`
- `AbilitySystemComponent`
- `HealthComponent`
- `StatsComponent`

> Tip: To quickly list asset paths in the project, use `Tools/Export/Copy Asset Paths (Selection or Assets)`.
