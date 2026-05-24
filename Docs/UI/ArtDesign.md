# 潜渊（Abyssfall）- UI设计规范

> 状态：已定稿（手机竖屏）
> 位置：UI/ArtDesign.md

---

## 一、风格定位

### 关键词
深渊遗迹 / 古老石板 / 破损符文 / 暗金荧光 / 金属锈蚀

### UI气质
- 不明亮、不卡通、不扁平
- 偏厚重、做旧、蚀刻、裂纹
- 所有面板带边缘破损、石材质感、暗纹底
- 强调：焦黑底色 + 暗红/幽蓝/暗紫荧光

---

## 二、配色规范

### 主色调（深渊黑）

| 用途 | 色值 |
|------|------|
| 背景底 | #121014 |
| 面板底色 | #1D1A21 |
| 边框/描边 | #2E2A34 |

### 强调色（遗迹荧光）

| 用途 | 色值 |
|------|------|
| 玩家主色 | #C42048（暗红荧光） |
| 能量/行动点 | #2090C4（幽蓝荧光） |
| 危险/深渊/敌人 | #7020C4（暗紫荧光） |
| 金色符文 | #B89A6A（哑光金） |

### 状态色

| 状态 | 色值 |
|------|------|
| 生命 | #E04040 |
| 护盾 | #40A0E0 |
| 蓄力/增益 | #E0B040 |
| 减益/侵蚀 | #A040E0 |

---

## 三、战斗主界面布局（竖屏）

```
┌─────────────────────┐
│  第X层   阶段X 回合X  │  ← 顶部信息栏
├─────────────────────┤
│                     │
│    [敌人立绘区域]    │  ← 上方：敌人展示
│                     │
├─────────────────────┤
│  敌人血条：████████░ │
├─────────────────────┤
│                     │
│    [玩家立绘区域]    │  ← 中间：玩家展示
│                     │
├─────────────────────┤
│  ❤️血量：██████░░░░ │
│  🛡护盾：████░░░░░░ │
│  ⚡行动值：█████░░░ │
├─────────────────────┤
│                     │
│   状态图标区         │  ← 状态栏
│                     │
├─────────────────────┤
│ ┌───┐ ┌───┐ ┌───┐  │
│ │连打│ │爆发│ │连斩│  │  ← 行动按钮区
│ └───┘ └───┘ └───┘  │     （可滑动/分页）
│ ┌───┐ ┌───┐ ┌───┐  │
│ │疾风│ │蓄力│ │藏锋│  │
│ └───┘ └───┘ └───┘  │
└─────────────────────┘
```

---

## 四、战备界面布局（竖屏）

```
┌─────────────────────┐
│  第X层 | 阶段 | 敌人 │  ← 顶部信息
├─────────────────────┤
│                     │
│    [敌人预览立绘]    │  ← 敌人展示
│                     │
├─────────────────────┤
│  敌人血量参考        │
├─────────────────────┤
│                     │
│    [玩家立绘区域]    │  ← 玩家立绘
│                     │
├─────────────────────┤
│  属性：攻/血/护甲/速度│
│  行动等级总览        │
├─────────────────────┤
│                     │
│  行动栏 [■][■][■]   │  ← 核心：已选行动
│  （初始3格，最高5格）  │
│                     │
├─────────────────────┤
│  可选行动池          │
│  ┌───┐ ┌───┐ ┌───┐  │
│  │行动│ │行动│ │行动│  │  ← 行动选择
│  └───┘ └───┘ └───┘  │
│  ┌───┐ ┌───┐ ┌───┐  │
│  │行动│ │行动│ │行动│  │
│  └───┘ └───┘ └───┘  │
├─────────────────────┤
│                     │
│   [开始下潜·进入战斗] │  ← 主按钮
│                     │
└─────────────────────┘
```

---

## 五、战斗奖励界面布局（竖屏）

```
┌─────────────────────┐
│     深渊馈赠         │  ← 标题
│     第X层通关        │
├─────────────────────┤
│                     │
│  ┌─────────────────┐│
│  │                 ││
│  │   奖励1展示     ││  ← 奖励卡片
│  │   行动升级      ││
│  │                 ││
│  └─────────────────┘│
│                     │
│  ┌─────────────────┐│
│  │   奖励2展示     ││
│  │   属性增幅      ││
│  └─────────────────┘│
│                     │
│  ┌─────────────────┐│
│  │   奖励3展示     ││
│  │   深渊遗物      ││
│  └─────────────────┘│
│                     │
├─────────────────────┤
│  ← 左右滑动切换奖励 → │
│                     │
│    [ 选择并继续 ]    │
│                     │
└─────────────────────┘
```

---

## 六、分区布局规范

### 6.1 战备界面分区
```
┌─────────────────────┐
│     上半区：敌人信息    │
│  立绘 + 特性标签 + 属性 │
├─────────────────────┤
│     下半区：玩家配置    │
│  立绘 + 行动栏 + 行动池 │
└─────────────────────┘
```

**动线：** 上方浏览敌人 → 下方配置行动

### 6.2 战斗界面分区
```
┌─────────────────────┐
│   左区域        右区域   │
│   敌人信息      玩家信息 │
├─────────────────────┤
│      ATB行动进度条      │
├─────────────────────┤
│      状态图标（小型化）   │
├─────────────────────┤
│      行动按钮（收拢）   │
└─────────────────────┘
```

**动线：** 左右对立，敌我清晰

### 6.3 通用分区原则
- 上层展示核心内容
- 下层放置功能按钮
- 视觉从上至下引导操作

---

## 七、组件设计

### 按钮样式
- 形状：圆角矩形 + 边缘小破损
- 尺寸：最小48×48dp（触控友好）
- 背景：暗黑石底
- 边框：细金线 + 裂纹
- 按下：荧光亮起 + 符文闪烁
- 满级技能（质变）：边框发光 + 闪电小图标 ⚡

### 血条/护盾条
- 血条：暗红底 + 黑色裂纹
- 护盾：幽蓝半透明 + 能量流动
- 掉血/破盾：碎片飞溅动画
- **渐变警示**：血量降低时色调同步加深，强化视觉警示

### ATB行动进度条
```
[敌人进度条████████░░░░]
[玩家进度条██░░░░░░░░░░]
```
- 可视化展示敌我行动顺位
- 直观体现行动先后顺序
- 敌我双进度条并排显示
- 荧光填充表示当前进度

### 状态图标
- 图标小型化收纳
- 点击/悬浮弹出简易效果说明
- 不占用主视野
- 示例：显示为小图标列表

### 行动池配色
| 类型 | 色调 |
|------|------|
| 连击类 | 暗红系 |
| 蓄力类 | 幽蓝系 |
| 护盾类 | 金色系 |
| 基础类 | 灰色系 |

不同类型行动组件视觉差异化，提升浏览效率

### 奖励品级
| 品级 | 色调 |
|------|------|
| 普通奖励 | 浅幽蓝底色 |
| 优质奖励 | 暗金底色 |

用色调深浅区分，不做复杂特效

### 文字风格
- 标题：古符文风格 + 描金
- 内容：纤细无衬线体 + 微光
- 数字：机械/遗迹数字
- 字号：最小14sp，确保可读性

### 弹窗风格
- 升级/胜利/失败弹窗
- 背景：石板碎裂 + 深渊黑雾
- 边框：古老金属包边
- 标题：符文亮起

---

## 八、AI绘图关键词

### 全局风格关键词
```
dark fantasy abyss ruin UI, ancient slate texture, cracked and broken edge, 
dark occult style, minimalist and functional layout, fit for mobile game, 
no extra decoration, eerie dim glow, ancient rune decoration, 
weathered metal border, burnt black base color, dark red/blue/purple 
fluorescent accents, matte gold rune details, high fidelity, portrait orientation
```

### 战斗主界面
```
Unity game battle UI, vertical portrait layout 9:16, abyss ruin theme, 
ancient slate cracked panel, top: floor number + round count, 
upper area: enemy portrait, middle: player portrait, bottom: skill buttons grid, 
status bar with health shield action point, dark red + dark blue glow, 
matte gold rune, mobile touch friendly
```

---

## 九、Unity基础适配

### 分辨率规范
- 目标分辨率：1080×1920（9:16竖屏）
- 适配方案：安全区适配 + 动态锚点
- UI安全区域：顶部状态栏、底部导航栏留出
- 资源尺寸：所有UI切图导出2倍分辨率

### UI层级设计
1. 底层：背景纹理、场景底图
2. 中下层：战斗角色/敌人展示区
3. 中层：血条、护盾条、行动值条、状态图标
4. 中上层：技能按钮、功能按钮
5. 上层：弹窗、提示文字、特效遮罩
6. 顶层：临时提示、层数/回合浮动文字

### 触控规范
- 触控区域：最小48×48dp
- 按钮间距：至少8dp，防止误触
- 滑动手势：用于奖励选择、行动切换

---

## 十、行动图标AI提示词

| 行动 | AI提示词 |
|------|----------|
| 连打 | sword continuous strike icon, dark red glow, multiple sword shadows, cracked stone base |
| 爆发 | burst power icon, dark purple glow, energy accumulation effect, ancient rune |
| 连斩 | quick slash icon, blue glow, continuous blade light, weathered metal base |
| 疾风 | gale speed icon, wind streak effect, light blue fluorescence, minimalist |
| 蓄力 | power storage icon, energy layer symbol, golden rune, dark stone base |
| 藏锋 | hidden strike icon, half hidden sword, no energy consumption mark, dark red glow |
| 蓄劲一击 | charged heavy blow icon, concentrated energy, strong glow |
| 重击 | heavy strike icon, powerful impact effect, cracked texture, dark purple glow |

---

## 十一、敌人立绘AI提示词

### 通用敌人设计关键词
```
abyss eroded ruin enemy, twisted and weird shape, stone/metal texture, 
dark glow, eerie and oppressive, game enemy character design, portrait composition
```

| 敌人 | AI提示词 |
|------|----------|
| 爆发者 | bulky stone body, energy accumulation on chest, dark purple glow |
| 守护者 | thick metal armor, shield counter mark, red cracked glow |
| 咒术师 | twisted thin body, rune curse effect, purple mist |
| 吞噬者 | irregular shape, black abyss tentacle, dark glow |
| 唤灵师 | ruin skeleton, summoned spirit shadow, blue fluorescence |
| 亡魂 | transparent ghost body, poison effect, split mark |
| 顿挫者 | metal spike body, shield crack symbol |
| 反噬者 | twisted rune body, damage feedback mark |
| 沉睡者 | curled stone body, awakening glow effect |

---

*最后更新：2026-05-12*
