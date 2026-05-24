# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

---

## Project Overview

**Abyssal Dive (潜渊)** - A Roguelike auto-battle game where players descend through深渊 (the Abyss), fighting enemies in automated combat and making strategic choices during preparation phases.

Core loop: 下潜 → 战备 → 自动战斗 → 奖励 → 继续下潜/失败重置

---

## Unity Project Structure

This is a **Unity 2022.3 LTS** project using **URP (Universal Render Pipeline)**.

### Key Directories

| Directory | Purpose |
|-----------|---------|
| `Assets/Scripts/` | Game code (to be created) |
| `Assets/Scenes/` | Unity scenes |
| `Assets/Art/` | Art assets (to be organized) |
| `Docs/` | Game design documentation |
| `ProjectSettings/` | Unity configuration |

### Current State

- Fresh Unity project
- Design documents organized in `Docs/`
- No game code implemented yet

---

## Game Design Documents (`Docs/`)

| Folder | Content |
|--------|---------|
| `Mechanics/` | Core game systems (CoreLoop, Combat, State, Trait, Action, Reward, 数值框架) |
| `Player/` | Player character design |
| `Enemy/` | Enemy designs (21 enemies in A/B/C/D/Boss pools) |
| `UI/` | Interface specifications and layouts |
| `_Archive/` | Archived design documents |

### Core Mechanisms (六大核心机制)

1. **击伤** - Core damage trigger
2. **连击** - Multi-hit combos
3. **蓄力** - Temporary damage buffs
4. **护盾** - Shield system
5. **溃伤** - DOT (damage over time)
6. **基础类** - Simple effects

### Key Systems

- **ATB (Action Time Battle)** - Speed-based action system
- **20 floors** across 4 stages with boss encounters at 5/10/15/20
- **State system** with layer-based effects
- **Trait system** for passive abilities

---

## Development Guidance

### When Implementing Game Systems

1. Reference `Docs/Mechanics/` for game logic
2. Reference `Docs/Enemy/` for enemy design templates
3. Reference `Docs/UI/` for interface layouts

### Design Document Language

- All design documents are in **Chinese**
- Code should use **English** identifiers
- Follow existing naming conventions when established

### Unity Guidelines

- Use ScriptableObjects for data (enemies, actions, states)
- Separate UI logic from game logic
- Use addressables for asset management (future)

---

## Notes

- This is a fresh project - no legacy code to maintain
- Design documents migrated from previous project (Abyssfall) and reorganized
- Game code implementation has not started
- Git repository: git@github.com:FeiKong-LiuYue/AbyssalDive.git