namespace AbyssalDive {
    /// <summary>
    /// 状态消耗类型
    /// </summary>
    public enum StateDecayType {
        TurnDecay,      // 回合衰减型
        OnHit,          // 受击消耗型
        Permanent,      // 永久叠层型
        Active          // 主动消耗型
    }

    /// <summary>
    /// 状态分类
    /// </summary>
    public enum StateCategory {
        Buff,       // 正向增益
        Resource,   // 资源状态
        DOT,        // DOT负向
        Defensive,  // 防御
        Debuff      // 负向
    }

    /// <summary>
    /// 状态数据定义
    /// </summary>
    [System.Serializable]
    public class StateData {
        public string id;                 // 唯一标识
        public string name;               // 名称
        public StateCategory category;    // 分类
        public StateDecayType decayType;  // 消耗类型
        public int maxLayers;             // 最大层数

        /// <summary>
        /// 效果类型描述
        /// </summary>
        public string effectType;

        /// <summary>
        /// 效果描述
        /// </summary>
        public string description;
    }

    /// <summary>
    /// 状态配置数据（静态预设）
    /// </summary>
    public static class StateConfig {
        // 正向增益
        public static readonly StateData 连击 = new StateData {
            id = "连击", name = "连击", category = StateCategory.Buff,
            decayType = StateDecayType.TurnDecay, maxLayers = 99,
            effectType = "叠层攻击", description = "每次发起攻击+1层；回合结束时若本回合未发起过攻击则清空"
        };

        public static readonly StateData 蓄势 = new StateData {
            id = "蓄势", name = "蓄势", category = StateCategory.Buff,
            decayType = StateDecayType.Permanent, maxLayers = 10,
            effectType = "回合获取蓄力", description = "每回合开始，获得等同于自身层数的蓄力"
        };

        public static readonly StateData 战意 = new StateData {
            id = "战意", name = "战意", category = StateCategory.Buff,
            decayType = StateDecayType.Permanent, maxLayers = 99,
            effectType = "叠层伤害加成", description = "回合开始+1层，层数越高伤害加成越高"
        };

        public static readonly StateData 力量 = new StateData {
            id = "力量", name = "力量", category = StateCategory.Buff,
            decayType = StateDecayType.Permanent, maxLayers = 20,
            effectType = "攻击调整", description = "每层增加1点当前攻击力，可为负数"
        };

        public static readonly StateData 狂怒 = new StateData {
            id = "狂怒", name = "狂怒", category = StateCategory.Buff,
            decayType = StateDecayType.Permanent, maxLayers = 20,
            effectType = "物理伤害加成", description = "每层增加5%物理伤害"
        };

        public static readonly StateData 疫病 = new StateData {
            id = "疫病", name = "疫病", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 5,
            effectType = "击伤施加溃伤", description = "击伤时，额外施加1层溃伤给目标，触发成功后减少1层"
        };

        public static readonly StateData 奇袭 = new StateData {
            id = "奇袭", name = "奇袭", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 5,
            effectType = "攻击追加概率", description = "每次发起攻击时，每层+10%追加攻击概率（100%攻击力），触发成功后减少1层"
        };

        public static readonly StateData 警戒 = new StateData {
            id = "警戒", name = "警戒", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 5,
            effectType = "受击反击概率", description = "受到攻击时，每层+10%反击概率（100%攻击力），触发成功后减少1层"
        };

        // 资源状态
        public static readonly StateData 蓄力 = new StateData {
            id = "蓄力", name = "蓄力", category = StateCategory.Resource,
            decayType = StateDecayType.Active, maxLayers = 99,
            effectType = "伤害附加", description = "每层增加附加物理伤害1点；触发后清空"
        };

        // DOT负向
        public static readonly StateData 易伤 = new StateData {
            id = "易伤", name = "易伤", category = StateCategory.DOT,
            decayType = StateDecayType.TurnDecay, maxLayers = 10,
            effectType = "受伤加成", description = "层数越高，受到伤害越高"
        };

        public static readonly StateData 虚弱 = new StateData {
            id = "虚弱", name = "虚弱", category = StateCategory.DOT,
            decayType = StateDecayType.TurnDecay, maxLayers = 10,
            effectType = "伤害削减", description = "层数越高，自身造成伤害越低"
        };

        public static readonly StateData 溃伤 = new StateData {
            id = "溃伤", name = "溃伤", category = StateCategory.DOT,
            decayType = StateDecayType.TurnDecay, maxLayers = 20,
            effectType = "持续伤害", description = "层数越高，回合开始时受到溃伤伤害越高（无视护盾）"
        };

        // 防御
        public static readonly StateData 铁壁 = new StateData {
            id = "铁壁", name = "铁壁", category = StateCategory.Defensive,
            decayType = StateDecayType.OnHit, maxLayers = 10,
            effectType = "受击抵消", description = "受击消耗1层，每层抵消一次物理伤害"
        };

        public static readonly StateData 护盾 = new StateData {
            id = "护盾", name = "护盾", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 999,
            effectType = "伤害抵扣", description = "受到攻击时抵扣伤害，每层1点"
        };

        public static readonly StateData 守御 = new StateData {
            id = "守御", name = "守御", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "伤害减免", description = "减少物理伤害，每层5%，每回合-1层，受击-1层"
        };

        public static readonly StateData 招架 = new StateData {
            id = "招架", name = "招架", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "回合护盾", description = "回合结束时提供护盾，层数×护甲属性；生效后衰减1层"
        };

        public static readonly StateData 坚壁 = new StateData {
            id = "坚壁", name = "坚壁", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 3,
            effectType = "护盾锁存", description = "护盾不自然衰减，每回合-1层"
        };

        public static readonly StateData 碎盾 = new StateData {
            id = "碎盾", name = "碎盾", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 3,
            effectType = "护盾触发", description = "护盾减少时触发1次效果（一次减少无论多少层只触发1次）"
        };

        public static readonly StateData 破盾 = new StateData {
            id = "破盾", name = "破盾", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 3,
            effectType = "护盾归零触发", description = "护盾归零时触发1次效果"
        };

        public static readonly StateData 荆棘 = new StateData {
            id = "荆棘", name = "荆棘", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "反伤", description = "受到攻击时，向攻击方施加荆棘层数的物理伤害"
        };

        public static readonly StateData 刺甲 = new StateData {
            id = "刺甲", name = "刺甲", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "反伤溃伤", description = "受到攻击时，向攻击方施加刺甲层数的溃伤"
        };

        public static readonly StateData 治愈 = new StateData {
            id = "治愈", name = "治愈", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "持续治疗", description = "每回合恢复层数血量，回合结束时衰减一半"
        };

        public static readonly StateData 威慑 = new StateData {
            id = "威慑", name = "威慑", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "攻击削弱", description = "存在时，若被攻击，降低攻击方1点临时力量"
        };

        public static readonly StateData 碎甲 = new StateData {
            id = "碎甲", name = "碎甲", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 5,
            effectType = "破盾", description = "造成物理伤害时，扣除目标等量于层数的护盾"
        };

        public static readonly StateData 结界 = new StateData {
            id = "结界", name = "结界", category = StateCategory.Defensive,
            decayType = StateDecayType.TurnDecay, maxLayers = 3,
            effectType = "友方伤害减免", description = "为全场友方提供伤害减免，来源死亡后消失"
        };

        // 负向
        public static readonly StateData 眩晕 = new StateData {
            id = "眩晕", name = "眩晕", category = StateCategory.Debuff,
            decayType = StateDecayType.TurnDecay, maxLayers = 3,
            effectType = "行动跳过", description = "跳过回合"
        };

        // 特殊状态（用于行动效果）
        public static readonly StateData 疾风 = new StateData {
            id = "疾风", name = "疾风", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 3,
            effectType = "行动重复", description = "下次行动执行多次"
        };

        public static readonly StateData 护盾穿透 = new StateData {
            id = "护盾穿透", name = "护盾穿透", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 1,
            effectType = "无视护盾", description = "下次攻击无视护盾"
        };

        public static readonly StateData 重击增益 = new StateData {
            id = "重击增益", name = "重击增益", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 3,
            effectType = "伤害倍率提升", description = "下一次伤害+50%/75%/100%"
        };

        public static readonly StateData 爆发增益 = new StateData {
            id = "爆发增益", name = "爆发增益", category = StateCategory.Buff,
            decayType = StateDecayType.Active, maxLayers = 3,
            effectType = "物理伤害加成", description = "下一次物理伤害+30%/45%/60%"
        };

        /// <summary>
        /// 根据状态ID获取状态数据
        /// </summary>
        public static StateData Get(string stateId) {
            return stateId switch {
                "连击" => 连击,
                "蓄势" => 蓄势,
                "战意" => 战意,
                "力量" => 力量,
                "狂怒" => 狂怒,
                "疫病" => 疫病,
                "奇袭" => 奇袭,
                "警戒" => 警戒,
                "蓄力" => 蓄力,
                "易伤" => 易伤,
                "虚弱" => 虚弱,
                "溃伤" => 溃伤,
                "铁壁" => 铁壁,
                "护盾" => 护盾,
                "守御" => 守御,
                "招架" => 招架,
                "坚壁" => 坚壁,
                "碎盾" => 碎盾,
                "破盾" => 破盾,
                "荆棘" => 荆棘,
                "刺甲" => 刺甲,
                "治愈" => 治愈,
                "威慑" => 威慑,
                "碎甲" => 碎甲,
                "结界" => 结界,
                "眩晕" => 眩晕,
                "疾风" => 疾风,
                "护盾穿透" => 护盾穿透,
                "重击增益" => 重击增益,
                "爆发增益" => 爆发增益,
                _ => new StateData { id = stateId, name = stateId, maxLayers = 10 }
            };
        }
    }
}