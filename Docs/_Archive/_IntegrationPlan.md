# Refactor 目录整合方案

> 创建时间：2026-05-12
> 更新：2026-05-12（新增目录结构）

---

## 一、目录结构

```
Refactor/
├── Player/              # 玩家角色相关内容
│   └── （待迁移文件）
├── Enemy/               # 敌人角色相关内容
│   └── （待迁移文件）
├── Mechanics/            # 游戏机制方面
│   ├── GameDesignOverview.md   # 核心设计总览
│   ├── ActionSheet.md          # 玩家行动
│   ├── CharacterStates.md      # 角色状态
│   ├── CharacterStateSheet.md # 角色特性
│   ├── ChargeSystem.md         # 蓄力体系
│   ├── EnemyRoster.md         # 敌人条目
│   └── （其他机制文件）
├── UI/                  # UI布局等美术内容
│   └── ArtDesign.md           # 美术设计
├── resources/           # 资源（已有）
└── _Archive/           # 归档旧稿
    ├── DesignSummary.md
    ├── EnemyDesign.md
    ├── EnemyDesignDetailed.md
    └── EnemyMechanisms.md
```

---

## 二、各目录职责

| 目录 | 职责 | 包含文件 |
|------|------|----------|
| **Player** | 玩家角色相关 | 玩家角色设定、属性、成长等 |
| **Enemy** | 敌人角色相关 | 敌人设计、敌人条目、敌人机制等 |
| **Mechanics** | 游戏机制 | 战斗系统、行动、状态、奖励、蓄力等 |
| **UI** | 界面美术 | 界面布局、配色、图标、美术规范 |

---

## 三、待迁移文件清单

| 原文件 | 迁移目标 | 状态 |
|--------|----------|------|
| ActionSheet.md | Mechanics/ | ✅ 已存在 |
| CharacterStates.md | Mechanics/ | ✅ 已存在 |
| CharacterStateSheet.md | Mechanics/ | ✅ 已存在 |
| ChargeSystem.md | Mechanics/ | ✅ 已存在 |
| EnemyRoster.md | Enemy/ | 待迁移 |
| ArtDesign.md | UI/ | 待迁移 |
| GameDesignOverview.md | Mechanics/ | ✅ 已存在 |

---

## 四、执行步骤

1. [ ] 创建 `Player/` 目录（如需新文件）
2. [ ] 创建 `Enemy/` 目录（如需新文件）
3. [ ] 创建 `Mechanics/` 目录（如需新文件）
4. [ ] 创建 `UI/` 目录（如需新文件）
5. [ ] 迁移 `EnemyRoster.md` 到 `Enemy/`
6. [ ] 迁移 `ArtDesign.md` 到 `UI/`
7. [ ] 归档旧稿到 `_Archive/`
8. [ ] 更新各目录 README.md

---

## 五、待完善项

| 目录 | 待完善内容 |
|------|-----------|
| **Enemy** | 18个敌人完整条目配置 |
| **Mechanics** | EnemyRoster.md 需与 GameDesignOverview.md 对齐 |
| **UI** | ArtDesign.md 需完善UI规范 |

---

*最后更新：2026-05-12*
