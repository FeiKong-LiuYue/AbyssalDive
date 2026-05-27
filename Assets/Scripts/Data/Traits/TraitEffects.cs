using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 特性效果注册表 - 实现所有特性效果
    /// </summary>
    public static class TraitEffects {
        // ==================== 输出收割系 ====================

        /// <summary>
        /// 断命利刃 - 敌人血量<30%时，增加伤害
        /// </summary>
        public static void 断命利刃(Unit self, Unit target, int level) {
            if (target.currentHealth / target.maxHealth < 0.3f) {
                float bonusDamage = 6f + (level - 1) * 3f;
                // 效果在伤害计算中体现，这里记录加成
                self.stateComponent.AddState(StateConfig.力量, Mathf.RoundToInt(bonusDamage));
            }
        }

        /// <summary>
        /// 久战愈勇 - 每回合获得蓄力
        /// </summary>
        public static void 久战愈勇(Unit self, Unit target, int level) {
            int chargeLayers = level;
            self.AddState(StateConfig.蓄力, chargeLayers);
        }

        /// <summary>
        /// 尾击 - 回合结束时发起攻击
        /// </summary>
        public static void 尾击(Unit self, Unit target, int level) {
            // 在 TurnEnd 事件中触发
            float damageMultiplier = 0.3f + level * 0.15f;
            float dmg = DamageCalculator.CalculatePhysicalDamage(self, target, self.GetTotalAttack() * damageMultiplier);
            target.TakeDamage(dmg, self);
        }

        /// <summary>
        /// 连击炽火 - 攻击时额外伤害（基于已攻击次数）
        /// </summary>
        public static void 连击炽火(Unit self, Unit target, int level) {
            int comboCount = self.stateComponent.GetStateStacks("连击");
            float bonusDamage = Mathf.Min(comboCount, 3) * level;
            // 效果在伤害计算中体现
            self.AddState(StateConfig.力量, Mathf.RoundToInt(bonusDamage));
        }

        /// <summary>
        /// 追击回响 - 追击成功后加速
        /// </summary>
        public static void 追击回响(Unit self, Unit target, int level) {
            float bonusAction = 0.1f + (level - 1) * 0.05f;
            self.actionValue += self.actionThreshold * bonusAction;
        }

        /// <summary>
        /// 反击回响 - 反击成功后加速
        /// </summary>
        public static void 反击回响(Unit self, Unit target, int level) {
            float bonusAction = 0.1f + (level - 1) * 0.05f;
            self.actionValue += self.actionThreshold * bonusAction;
        }

        /// <summary>
        /// 连击护盾 - 存在连击时受击获得护盾
        /// </summary>
        public static void 连击护盾(Unit self, Unit target, int level) {
            int comboCount = self.stateComponent.GetStateStacks("连击");
            if (comboCount > 0) {
                float multiplier = level == 1 ? 1f : (level == 2 ? 1.5f : 2f);
                self.shieldComponent.Add(comboCount * multiplier);
            }
        }

        /// <summary>
        /// 溃伤传染 - 溃伤伤害时传染
        /// </summary>
        public static void 溃伤传染(Unit self, Unit target, int level) {
            float chance = 0.3f + level * 0.15f;
            if (UnityEngine.Random.value < chance) {
                // 传染给随机另一目标（需要战场信息）
                var enemies = BattleSystem.Instance?.GetEnemiesOf(self);
                if (enemies != null && enemies.Length > 0) {
                    var randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Length)];
                    randomEnemy.AddState(StateConfig.溃伤, 1);
                }
            }
        }

        /// <summary>
        /// 溃伤扩散 - 施加溃伤时传染
        /// </summary>
        public static void 溃伤扩散(Unit self, Unit target, int level) {
            float chance = 0.3f + level * 0.15f;
            if (UnityEngine.Random.value < chance) {
                var enemies = BattleSystem.Instance?.GetEnemiesOf(self);
                if (enemies != null && enemies.Length > 0) {
                    var randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Length)];
                    randomEnemy.AddState(StateConfig.溃伤, 1);
                }
            }
        }

        /// <summary>
        /// 溃伤加深 - 施加溃伤时额外层数
        /// </summary>
        public static void 溃伤加深(Unit self, Unit target, int level) {
            int extraStacks = level;
            target.AddState(StateConfig.溃伤, extraStacks);
        }

        /// <summary>
        /// 溃伤侵蚀 - 造成溃伤伤害时额外施加
        /// </summary>
        public static void 溃伤侵蚀(Unit self, Unit target, int level) {
            int extraStacks = level;
            target.AddState(StateConfig.溃伤, extraStacks);
        }

        /// <summary>
        /// 溃伤治愈 - 造成溃伤伤害时回血
        /// </summary>
        public static void 溃伤治愈(Unit self, Unit target, float damage, int level) {
            float healPercent = 0.2f + (level - 1) * 0.1f;
            self.Heal(damage * healPercent);
        }

        // ==================== 生存续航系 ====================

        /// <summary>
        /// 深渊吐纳 - 回合开始回血
        /// </summary>
        public static void 深渊吐纳(Unit self, Unit target, int level) {
            float healAmount = 2f + level;
            if (self.currentHealth / self.maxHealth < 0.5f) {
                healAmount *= 2f;
            }
            self.Heal(healAmount);
        }

        /// <summary>
        /// 浴血攫生 - 造成伤害回血
        /// </summary>
        public static void 浴血攫生(Unit self, Unit target, float damage, int level) {
            float healAmount = 2f + level;
            self.Heal(healAmount);
        }

        /// <summary>
        /// 墟骨自愈 - 击杀后获得治愈状态
        /// </summary>
        public static void 墟骨自愈(Unit self, Unit target, int level) {
            int治愈Layers = 2 + level;
            self.AddState(StateConfig.治愈, 治愈Layers);
        }

        /// <summary>
        /// 眩晕复苏 - 眩晕时回血
        /// </summary>
        public static void 眩晕复苏(Unit self, Unit target, int level) {
            if (self.stateComponent.HasState("眩晕")) {
                float healAmount = 3f + level * 2f;
                self.Heal(healAmount);
            }
        }

        // ==================== 护盾闭环系 ====================

        /// <summary>
        /// 裂锋护壳 - 造成伤害获得护盾
        /// </summary>
        public static void 裂锋护壳(Unit self, Unit target, float damage, int level) {
            float shieldAmount = 3f + level;
            self.shieldComponent.Add(shieldAmount);
        }

        /// <summary>
        /// 护壳裂击 - 护盾转伤害
        /// </summary>
        public static void 护壳裂击(Unit self, Unit target, int level) {
            float conversionRate = 0.3f + level * 0.1f;
            float shieldDamage = self.shieldComponent.currentShield * conversionRate;
            self.shieldComponent.Clear();
            target.TakeDamage(shieldDamage, self, true); // 无视护甲
        }

        /// <summary>
        /// 裂盾裁决 - 对护盾目标额外伤害
        /// </summary>
        public static void 裂盾裁决(Unit self, Unit target, int level) {
            float bonusDamage = 5f + level * 2f;
            if (target.shieldComponent.currentShield > 0) {
                // 直接在目标上记录加成伤害（实际在DamageCalculator中处理）
                target.AddState(StateConfig.力量, -Mathf.RoundToInt(bonusDamage)); // 降低目标护甲/增加自身伤害
            }
        }

        /// <summary>
        /// 破盾回壳 - 破盾后获得护盾
        /// </summary>
        public static void 破盾回壳(Unit self, Unit target, int level) {
            float shieldAmount = 4f + level * 2f;
            self.shieldComponent.Add(shieldAmount);
        }

        /// <summary>
        /// 坚壁免摧 - 减弱破盾负面
        /// </summary>
        public static void 坚壁免摧(Unit self, Unit target, int level) {
            // 减少破盾时获得的debuff
            if (level >= 3) {
                // 免疫破盾debuff
                self.AddState(StateConfig.守御, 2);
            }
        }

        // ==================== Debuff联动系 ====================

        /// <summary>
        /// 易伤猎杀 - 对易伤目标额外伤害
        /// </summary>
        public static void 易伤猎杀(Unit self, Unit target, int level) {
            float bonusDamage = 7f + level * 3.5f;
            if (target.stateComponent.HasState("易伤")) {
                target.AddState(StateConfig.力量, -Mathf.RoundToInt(bonusDamage));
            }
        }

        /// <summary>
        /// 溃伤撕裂 - 对溃伤目标额外伤害
        /// </summary>
        public static void 溃伤撕裂(Unit self, Unit target, int level) {
            float bonusDamage = 5f + level * 3f;
            int woundStacks = target.stateComponent.GetStateStacks("溃伤");
            if (woundStacks > 0) {
                target.AddState(StateConfig.力量, -Mathf.RoundToInt(bonusDamage + woundStacks));
            }
        }

        // ==================== 反制补刀系 ====================

        /// <summary>
        /// 荆棘 - 受击反弹伤害
        /// </summary>
        public static void 荆棘(Unit self, Unit target, int level) {
            float thornDamage = 2f + level;
            target.TakeDamage(thornDamage, self);
        }

        /// <summary>
        /// 回合收刀 - 回合结束时有概率造成伤害
        /// </summary>
        public static void 回合收刀(Unit self, Unit target, int level) {
            float chance = 0.5f + level * 0.1f;
            if (UnityEngine.Random.value < chance) {
                float damage = DamageCalculator.CalculatePhysicalDamage(self, target, self.GetTotalAttack() * 0.5f);
                target.TakeDamage(damage, self);
            }
        }

        // ==================== 蓄力额外获得系 ====================

        /// <summary>
        /// 渊能汲取 - 获得蓄力时额外层数
        /// </summary>
        public static void 渊能汲取(Unit self, Unit target, int level) {
            // 在蓄力获得时调用，增加层数
            // 具体实现需要在蓄力获取逻辑中调用
        }

        // ==================== 蓄力护盾联动系 ====================

        /// <summary>
        /// 聚盾 - 获得蓄力时获得护盾
        /// </summary>
        public static void 聚盾(Unit self, Unit target, int level) {
            float shieldAmount = 3f + level * 2f;
            self.shieldComponent.Add(shieldAmount);
        }

        /// <summary>
        /// 蓄甲转化 - 蓄力高时获得护盾
        /// </summary>
        public static void 蓄甲转化(Unit self, Unit target, int level) {
            int chargeStacks = self.stateComponent.GetStateStacks("蓄力");
            if (chargeStacks >= 3) {
                float shieldPercent = 0.15f + level * 0.1f;
                float shieldAmount = self.defense * shieldPercent;
                self.shieldComponent.Add(shieldAmount);
            }
        }

        // ==================== 蓄力破盾加成系 ====================

        /// <summary>
        /// 破盾之力 - 蓄力存在时对护盾目标额外伤害
        /// </summary>
        public static void 破盾之力(Unit self, Unit target, int level) {
            float bonusDamage = 5f + level * 3f;
            if (self.stateComponent.HasState("蓄力") && target.shieldComponent.currentShield > 0) {
                target.AddState(StateConfig.力量, -Mathf.RoundToInt(bonusDamage));
            }
        }

        /// <summary>
        /// 穿盾之势 - 击破护盾时额外伤害
        /// </summary>
        public static void 穿盾之势(Unit self, Unit target, float shieldBefore, int level) {
            float bonusDamage = 3f + level * 2f;
            if (shieldBefore > 0 && target.shieldComponent.currentShield == 0) {
                // 护盾被击破
                target.TakeDamage(bonusDamage, self);
            }
        }

        /// <summary>
        /// 蓄劲余韵 - 击伤时保留部分蓄力
        /// </summary>
        public static void 蓄劲余韵(Unit self, Unit target, int level) {
            float retainPercent = 0.25f + level * 0.25f;
            int currentCharge = self.stateComponent.GetStateStacks("蓄力");
            if (currentCharge > 0) {
                int retainedCharge = Mathf.RoundToInt(currentCharge * retainPercent);
                // 先移除，触发效果后再加回
                self.stateComponent.RemoveState("蓄力");
                if (retainedCharge > 0) {
                    self.AddState(StateConfig.蓄力, retainedCharge);
                }
            }
        }

        /// <summary>
        /// 血债 - 被击伤时获得蓄力
        /// </summary>
        public static void 血债(Unit self, Unit target, float damage, int level) {
            float chargePercent = 0.2f + (level - 1) * 0.1f;
            int chargeGained = Mathf.RoundToInt(damage * chargePercent);
            if (chargeGained > 0) {
                self.AddState(StateConfig.蓄力, chargeGained);
            }
        }

        /// <summary>
        /// 渊底绞杀 - 敌人低血时蓄力效果增强
        /// </summary>
        public static void 渊底绞杀(Unit self, Unit target, int level) {
            if (target.currentHealth / target.maxHealth < 0.5f) {
                float bonusPercent = 0.15f + (level - 1) * 0.1f;
                // 蓄力效果增加（需要在蓄力计算中应用）
                self.AddState(StateConfig.战意, Mathf.RoundToInt(bonusPercent * 10));
            }
        }

        /// <summary>
        /// 残息爆发 - 自身低血时蓄力效果增强
        /// </summary>
        public static void 残息爆发(Unit self, Unit target, int level) {
            if (self.currentHealth / self.maxHealth < 0.5f) {
                float bonusPercent = 0.15f + (level - 1) * 0.1f;
                self.AddState(StateConfig.战意, Mathf.RoundToInt(bonusPercent * 10));
            }
        }

        // ==================== 机制反制系 ====================

        /// <summary>
        /// 缄默看破 - 对眩晕目标额外伤害
        /// </summary>
        public static void 缄默看破(Unit self, Unit target, int level) {
            if (target.stateComponent.HasState("眩晕")) {
                float bonusPercent = 0.1f + level * 0.05f;
                self.AddState(StateConfig.狂怒, Mathf.RoundToInt(bonusPercent * 10));
            }
        }

        /// <summary>
        /// 咒印消解 - 负面时间缩短
        /// </summary>
        public static void 咒印消解(Unit self, Unit target, int level) {
            // 减少自身负面状态持续时间（通过增加衰减速度实现）
            if (level >= 3) {
                // 驱散并获得增伤减伤
                self.stateComponent.RemoveState("易伤");
                self.stateComponent.RemoveState("虚弱");
                self.AddState(StateConfig.守御, 2);
                self.AddState(StateConfig.战意, 3);
            }
        }

        /// <summary>
        /// 逆刃绝缘 - 反弹伤害降低
        /// </summary>
        public static void 逆刃绝缘(Unit self, Unit target, int level) {
            // 减少荆棘等反伤效果的影响
            if (level >= 3) {
                // 多段攻击不被反噬（需要行动执行器配合）
            }
        }

        // ==================== 战场协同系 ====================

        /// <summary>
        /// 孤勇临渊 - 敌少时增伤
        /// </summary>
        public static void 孤勇临渊(Unit self, Unit target, int level) {
            var enemies = BattleSystem.Instance?.GetEnemiesOf(self);
            int enemyCount = enemies != null ? enemies.Length : 1;
            if (enemyCount <= 2) {
                float bonusPercent = 0.1f + level * 0.05f;
                self.AddState(StateConfig.狂怒, Mathf.RoundToInt(bonusPercent * 10));
            }
        }

        /// <summary>
        /// 镇灵封禁 - 限制敌人复活
        /// </summary>
        public static void 镇灵封禁(Unit self, Unit target, int level) {
            // 敌人复活血量降低或无法复活
            if (level >= 3 && target is EnemyUnit enemy) {
                enemy.canResurrect = false;
            }
        }

        // ==================== 伤害规则系 ====================

        /// <summary>
        /// 锐破护甲 - 无视目标护盾
        /// </summary>
        public static void 锐破护甲(Unit self, Unit target, int level) {
            int ignoreShield = 2 + level;
            // 在伤害计算中应用（需要修改DamageCalculator）
            self.AddState(StateConfig.力量, ignoreShield);
        }
    }
}