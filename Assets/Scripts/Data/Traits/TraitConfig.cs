using System;

namespace AbyssalDive {
    /// <summary>
    /// 特性配置数据（静态预设）
    /// </summary>
    public static class TraitConfig {
        // ==================== 输出收割系 ====================

        public static TraitData 断命利刃 = new TraitData {
            id = "断命利刃",
            name = "断命利刃",
            triggerEvents = new[] { GameEventType.Hit },
            levelDescriptions = new[] {
                "敌人血量<30%时，攻击+6伤害",
                "+9伤害",
                "+13伤害；触发时敌人无法反击⚡"
            }
        };

        public static TraitData 久战愈勇 = new TraitData {
            id = "久战愈勇",
            name = "久战愈勇",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "每回合获得1层蓄力",
                "+2层",
                "+3层⚡"
            }
        };

        public static TraitData 尾击 = new TraitData {
            id = "尾击",
            name = "尾击",
            triggerEvents = new[] { GameEventType.TurnEnd },
            levelDescriptions = new[] {
                "回合结束时，发起1次30%攻击力的攻击",
                "50%",
                "80%⚡"
            }
        };

        public static TraitData 连击炽火 = new TraitData {
            id = "连击炽火",
            name = "连击炽火",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "发起攻击时，该攻击额外伤害+1（基于本回合已攻击次数）",
                "+2",
                "+3"
            }
        };

        public static TraitData 追击回响 = new TraitData {
            id = "追击回响",
            name = "追击回响",
            triggerEvents = new[] { GameEventType.FollowUpTriggered },
            levelDescriptions = new[] {
                "追击触发成功后，下个行动+10%行动积累值",
                "+15%",
                "+20%⚡"
            }
        };

        public static TraitData 反击回响 = new TraitData {
            id = "反击回响",
            name = "反击回响",
            triggerEvents = new[] { GameEventType.CounterTriggered },
            levelDescriptions = new[] {
                "反击成功后，下个行动+10%行动积累值",
                "+15%",
                "+20%⚡"
            }
        };

        public static TraitData 连击护盾 = new TraitData {
            id = "连击护盾",
            name = "连击护盾",
            triggerEvents = new[] { GameEventType.Damaged },
            levelDescriptions = new[] {
                "存在连击层数时，受到攻击获得护盾，数值=当前连击层数",
                "×1.5",
                "×2⚡"
            }
        };

        public static TraitData 溃伤传染 = new TraitData {
            id = "溃伤传染",
            name = "溃伤传染",
            triggerEvents = new[] { GameEventType.Hit }, // 溃伤伤害
            levelDescriptions = new[] {
                "造成溃伤伤害时，30%几率传染1层溃伤给随机另一目标",
                "45%",
                "60%⚡"
            }
        };

        public static TraitData 溃伤扩散 = new TraitData {
            id = "溃伤扩散",
            name = "溃伤扩散",
            triggerEvents = new[] { GameEventType.StateAdded }, // 施加溃伤时
            levelDescriptions = new[] {
                "施加溃伤时，30%几率额外传染1层溃伤给随机另一目标",
                "45%",
                "60%⚡"
            }
        };

        public static TraitData 溃伤加深 = new TraitData {
            id = "溃伤加深",
            name = "溃伤加深",
            triggerEvents = new[] { GameEventType.StateAdded },
            levelDescriptions = new[] {
                "施加溃伤时，额外施加1层溃伤给目标",
                "+2层",
                "+3层⚡"
            }
        };

        public static TraitData 溃伤侵蚀 = new TraitData {
            id = "溃伤侵蚀",
            name = "溃伤侵蚀",
            triggerEvents = new[] { GameEventType.Hit },
            levelDescriptions = new[] {
                "造成溃伤伤害时，额外施加1层溃伤给目标",
                "+2层",
                "+3层⚡"
            }
        };

        public static TraitData 溃伤治愈 = new TraitData {
            id = "溃伤治愈",
            name = "溃伤治愈",
            triggerEvents = new[] { GameEventType.Hit }, // 溃伤造成的伤害
            levelDescriptions = new[] {
                "造成溃伤伤害时，获得等同于伤害值20%的生命回复",
                "30%",
                "40%⚡"
            }
        };

        // ==================== 生存续航系 ====================

        public static TraitData 深渊吐纳 = new TraitData {
            id = "深渊吐纳",
            name = "深渊吐纳",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "回合开始+2血",
                "+3",
                "+4；低血回血翻倍"
            }
        };

        public static TraitData 浴血攫生 = new TraitData {
            id = "浴血攫生",
            name = "浴血攫生",
            triggerEvents = new[] { GameEventType.Hit },
            levelDescriptions = new[] {
                "造成伤害+2血",
                "+3",
                "+4；击杀额外回血"
            }
        };

        public static TraitData 墟骨自愈 = new TraitData {
            id = "墟骨自愈",
            name = "墟骨自愈",
            triggerEvents = new[] { GameEventType.Kill },
            levelDescriptions = new[] {
                "击杀目标后，获得3层「治愈」状态",
                "获得4层",
                "获得5层⚡"
            }
        };

        public static TraitData 眩晕复苏 = new TraitData {
            id = "眩晕复苏",
            name = "眩晕复苏",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "眩晕时回血",
                "提升",
                "眩晕结束回血"
            }
        };

        // ==================== 护盾闭环系 ====================

        public static TraitData 裂锋护壳 = new TraitData {
            id = "裂锋护壳",
            name = "裂锋护壳",
            triggerEvents = new[] { GameEventType.Hit },
            levelDescriptions = new[] {
                "造成伤害+3护盾",
                "+4",
                "+5；单次多命中只叠一次"
            }
        };

        public static TraitData 护壳裂击 = new TraitData {
            id = "护壳裂击",
            name = "护壳裂击",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "护盾30%转伤害",
                "40%",
                "50%；无视护甲⚡"
            }
        };

        public static TraitData 裂盾裁决 = new TraitData {
            id = "裂盾裁决",
            name = "裂盾裁决",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "对护盾目标+5伤害",
                "+7",
                "+10；概率碎盾⚡"
            }
        };

        public static TraitData 破盾回壳 = new TraitData {
            id = "破盾回壳",
            name = "破盾回壳",
            triggerEvents = new[] { GameEventType.ShieldBroken },
            levelDescriptions = new[] {
                "破盾+4护盾",
                "+6",
                "+8；碎全盾获减伤⚡"
            }
        };

        public static TraitData 坚壁免摧 = new TraitData {
            id = "坚壁免摧",
            name = "坚壁免摧",
            triggerEvents = new[] { GameEventType.ShieldBroken },
            levelDescriptions = new[] {
                "破盾负面减弱",
                "提升",
                "免疫破盾debuff"
            }
        };

        // ==================== Debuff联动系 ====================

        public static TraitData 易伤猎杀 = new TraitData {
            id = "易伤猎杀",
            name = "易伤猎杀",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "对易伤+7伤害",
                "+10",
                "+14；概率刷新易伤⚡"
            }
        };

        public static TraitData 溃伤撕裂 = new TraitData {
            id = "溃伤撕裂",
            name = "溃伤撕裂",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "对溃伤+5伤害",
                "+8",
                "+11；溃伤层数越高加成越高⚡"
            }
        };

        // ==================== 反制补刀系 ====================

        public static TraitData 荆棘 = new TraitData {
            id = "荆棘",
            name = "荆棘",
            triggerEvents = new[] { GameEventType.Damaged },
            levelDescriptions = new[] {
                "受击反弹2伤害",
                "3",
                "4；多段只触发一次"
            }
        };

        public static TraitData 回合收刀 = new TraitData {
            id = "回合收刀",
            name = "回合收刀",
            triggerEvents = new[] { GameEventType.TurnEnd },
            levelDescriptions = new[] {
                "回合结束50%伤害",
                "60%",
                "70%；触发所有攻击特性⚡"
            }
        };

        // ==================== 蓄力额外获得系 ====================

        public static TraitData 渊能汲取 = new TraitData {
            id = "渊能汲取",
            name = "渊能汲取",
            triggerEvents = new[] { GameEventType.StateAdded }, // 蓄力获得时
            levelDescriptions = new[] {
                "获得蓄力时+1层",
                "+2层",
                "+3层⚡"
            }
        };

        // ==================== 蓄力护盾联动系 ====================

        public static TraitData 聚盾 = new TraitData {
            id = "聚盾",
            name = "聚盾",
            triggerEvents = new[] { GameEventType.StateAdded }, // 蓄力获得时
            levelDescriptions = new[] {
                "获得蓄力时+3护盾",
                "+5护盾",
                "+8护盾⚡"
            }
        };

        public static TraitData 蓄甲转化 = new TraitData {
            id = "蓄甲转化",
            name = "蓄甲转化",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "蓄力≥3层时，获得护盾+15%",
                "+25%",
                "+40%⚡"
            }
        };

        // ==================== 蓄力破盾加成系 ====================

        public static TraitData 破盾之力 = new TraitData {
            id = "破盾之力",
            name = "破盾之力",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "蓄力存在时，对护盾目标+5伤害",
                "+8伤害",
                "+12伤害⚡"
            }
        };

        public static TraitData 穿盾之势 = new TraitData {
            id = "穿盾之势",
            name = "穿盾之势",
            triggerEvents = new[] { GameEventType.ShieldBroken },
            levelDescriptions = new[] {
                "若击破护盾，额外造成3点伤害",
                "额外造成5点伤害",
                "额外造成8点伤害⚡"
            }
        };

        public static TraitData 蓄劲余韵 = new TraitData {
            id = "蓄劲余韵",
            name = "蓄劲余韵",
            triggerEvents = new[] { GameEventType.Hit },
            levelDescriptions = new[] {
                "击伤时，保留本次蓄力消耗的25%",
                "保留50%",
                "保留100%⚡"
            }
        };

        public static TraitData 血债 = new TraitData {
            id = "血债",
            name = "血债",
            triggerEvents = new[] { GameEventType.Damaged },
            levelDescriptions = new[] {
                "被击伤时，获得相当于受到伤害20%的蓄力",
                "获得30%",
                "获得40%⚡"
            }
        };

        public static TraitData 渊底绞杀 = new TraitData {
            id = "渊底绞杀",
            name = "渊底绞杀",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "敌人血量<50%时，蓄力效果+15%",
                "+25%",
                "+40%⚡"
            }
        };

        public static TraitData 残息爆发 = new TraitData {
            id = "残息爆发",
            name = "残息爆发",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "自身血量<50%时，蓄力效果+15%",
                "+25%",
                "+40%⚡"
            }
        };

        // ==================== 机制反制系 ====================

        public static TraitData 缄默看破 = new TraitData {
            id = "缄默看破",
            name = "缄默看破",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "对眩晕+伤害",
                "提升",
                "击杀后永久提升⚡"
            }
        };

        public static TraitData 咒印消解 = new TraitData {
            id = "咒印消解",
            name = "咒印消解",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "负面时间缩短",
                "提升",
                "驱散获增伤减伤⚡"
            }
        };

        public static TraitData 逆刃绝缘 = new TraitData {
            id = "逆刃绝缘",
            name = "逆刃绝缘",
            triggerEvents = new[] { GameEventType.Damaged },
            levelDescriptions = new[] {
                "反弹伤害降低",
                "提升",
                "多段不被反噬"
            }
        };

        // ==================== 战场协同系 ====================

        public static TraitData 孤勇临渊 = new TraitData {
            id = "孤勇临渊",
            name = "孤勇临渊",
            triggerEvents = new[] { GameEventType.TurnStart },
            levelDescriptions = new[] {
                "敌少+增伤",
                "提升",
                "最终敌人破盾暴击强化⚡"
            }
        };

        public static TraitData 镇灵封禁 = new TraitData {
            id = "镇灵封禁",
            name = "镇灵封禁",
            triggerEvents = new[] { GameEventType.Kill },
            levelDescriptions = new[] {
                "敌复活血量降低",
                "降低召唤攻击",
                "击杀单位无法复活⚡"
            }
        };

        // ==================== 伤害规则系 ====================

        public static TraitData 锐破护甲 = new TraitData {
            id = "锐破护甲",
            name = "锐破护甲",
            triggerEvents = new[] { GameEventType.Attack },
            levelDescriptions = new[] {
                "无视目标2点护盾",
                "无视3点",
                "无视4点护盾⚡"
            }
        };
    }
}