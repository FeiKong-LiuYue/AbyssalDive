using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 行动执行器 - 实现所有58个行动的效果
    /// </summary>
    public class ActionExecutor {

        /// <summary>
        /// 执行行动
        /// </summary>
        public static void ExecuteAction(Unit attacker, Unit target, ActionData action) {
            switch (action.category) {
                case ActionCategory.Combo:
                    ExecuteComboAction(attacker, target, action);
                    break;
                case ActionCategory.Strike:
                    ExecuteStrikeAction(attacker, target, action);
                    break;
                case ActionCategory.Charge:
                    ExecuteChargeAction(attacker, target, action);
                    break;
                case ActionCategory.Shield:
                    ExecuteShieldAction(attacker, target, action);
                    break;
                case ActionCategory.Wound:
                    ExecuteWoundAction(attacker, target, action);
                    break;
                case ActionCategory.Basic:
                    ExecuteBasicAction(attacker, target, action);
                    break;
            }
        }

        // ==================== 连击类（12个）====================

        private static void ExecuteComboAction(Unit attacker, Unit target, ActionData action) {
            switch (action.type) {
                case ActionType.连打:
                    // 1级：2次攻击各50% / 2级：3次攻击各50% / 3级：4次攻击各50%⚡
                    int hitCount = action.level + 1;
                    for (int i = 0; i < hitCount; i++) {
                        float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * 0.5f);
                        target.TakeDamage(dmg, attacker);
                        PublishHitEvent(attacker, target, dmg);
                        attacker.stateComponent.AddState(StateConfig.连击, 1);
                    }
                    break;

                case ActionType.连斩:
                    // 30%/40%/50%伤害，执行后+1次追加攻击（追加概率20%/35%/50%）
                    float baseDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * (0.3f + action.level * 0.1f));
                    target.TakeDamage(baseDmg, attacker);
                    PublishHitEvent(attacker, target, baseDmg);
                    attacker.stateComponent.AddState(StateConfig.连击, 1);

                    float followUpChance = 0.2f + action.level * 0.15f;
                    if (Random.value < followUpChance) {
                        float followUpDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * 0.5f);
                        target.TakeDamage(followUpDmg, attacker);
                        PublishHitEvent(attacker, target, followUpDmg);
                    }
                    break;

                case ActionType.追击:
                    // 8/12/15伤害，击伤追加1次50%/60%/70%攻击力攻击，3级击伤必定附加1层溃伤
                    float fixedDmg = 8f + action.level * 4f;
                    float healthBefore = target.currentHealth;
                    target.TakeDamage(fixedDmg, attacker);
                    bool isHit = target.currentHealth < healthBefore;

                    if (isHit) {
                        float followUpDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * (0.5f + action.level * 0.1f));
                        target.TakeDamage(followUpDmg, attacker);
                        PublishHitEvent(attacker, target, followUpDmg);

                        if (action.level >= 3) {
                            target.AddState(StateConfig.溃伤, 1);
                        }
                    }
                    break;

                case ActionType.连击延续:
                    // 发起2/2/3次攻击，每次击伤额外获得1/2/2层连击
                    int attackCount = action.level >= 3 ? 3 : 2;
                    int comboPerHit = action.level >= 2 ? 2 : 1;

                    for (int i = 0; i < attackCount; i++) {
                        float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * 0.5f);
                        float hpBefore = target.currentHealth;
                        target.TakeDamage(dmg, attacker);
                        PublishHitEvent(attacker, target, dmg);

                        if (target.currentHealth < hpBefore) {
                            attacker.stateComponent.AddState(StateConfig.连击, comboPerHit);
                        }
                    }
                    break;

                case ActionType.连击狂热:
                    // 发起1次攻击；连击≥3时，追加1次30%/40%/50%攻击；≥5时无视护盾；≥7时回复生命
                    float attackDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack(), action.level >= 2);
                    target.TakeDamage(attackDmg, attacker);
                    PublishHitEvent(attacker, target, attackDmg);

                    int comboStacks = attacker.stateComponent.GetStateStacks("连击");
                    if (comboStacks >= 3) {
                        float followUpDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * (0.3f + action.level * 0.1f), action.level >= 2);
                        target.TakeDamage(followUpDmg, attacker);
                        PublishHitEvent(attacker, target, followUpDmg);
                    }

                    if (comboStacks >= 7 && action.level >= 3) {
                        attacker.Heal(10f);
                    }
                    break;

                case ActionType.连击终结:
                    // 连击≥5层时，额外发起1次全体攻击（+20%/+40%）
                    int comboForFinal = attacker.stateComponent.GetStateStacks("连击");
                    if (comboForFinal >= 5) {
                        float allAttackDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * (0.2f + action.level * 0.2f), true);
                        // 对所有敌人造成伤害
                        var enemies = BattleSystem.Instance?.GetEnemiesOf(attacker);
                        if (enemies != null) {
                            foreach (var enemy in enemies) {
                                enemy.TakeDamage(allAttackDmg, attacker);
                                PublishHitEvent(attacker, enemy, allAttackDmg);
                            }
                        } else {
                            // Fallback: just damage the current target
                            target.TakeDamage(allAttackDmg, attacker);
                            PublishHitEvent(attacker, target, allAttackDmg);
                        }
                    }
                    break;

                case ActionType.连击追击:
                    // 获得3/4/5层「奇袭」状态
                    attacker.AddState(StateConfig.奇袭, action.level + 2);
                    break;

                case ActionType.连击警戒:
                    // 获得3/4/5层「警戒」状态
                    attacker.AddState(StateConfig.警戒, action.level + 2);
                    break;

                case ActionType.连击狂暴:
                    // 消耗所有连击层数，每层转化为+1/2/3狂怒
                    int comboToConsume = attacker.stateComponent.GetStateStacks("连击");
                    if (comboToConsume > 0) {
                        attacker.stateComponent.RemoveState("连击");
                        attacker.AddState(StateConfig.狂怒, comboToConsume * action.level);
                    }
                    break;

                case ActionType.狂怒猛击:
                    // 2/3/4次4/5/6伤害，每次击伤+1狂怒
                    int hits = action.level + 1;
                    int baseDamage = 3 + action.level;
                    for (int i = 0; i < hits; i++) {
                        float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, baseDamage);
                        float hpBefore = target.currentHealth;
                        target.TakeDamage(dmg, attacker);
                        PublishHitEvent(attacker, target, dmg);

                        if (target.currentHealth < hpBefore) {
                            attacker.AddState(StateConfig.狂怒, 1);
                        }
                    }
                    break;

                case ActionType.倾泻:
                    // 消耗当前所有连击层数，每消耗1层发起1次50%/60%/75%攻击力攻击
                    int comboToUse = attacker.stateComponent.GetStateStacks("连击");
                    if (comboToUse > 0) {
                        attacker.stateComponent.RemoveState("连击");
                        float damageMultiplier = 0.5f + action.level * 0.25f;
                        for (int i = 0; i < comboToUse; i++) {
                            float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * damageMultiplier);
                            target.TakeDamage(dmg, attacker);
                            PublishHitEvent(attacker, target, dmg);
                        }
                    }
                    break;

                case ActionType.乘胜追击:
                    // 10/12/15伤害，击伤则立即执行下一行动
                    float chaseDmg = 10f + action.level * 2f;
                    float hpBeforeChase = target.currentHealth;
                    target.TakeDamage(chaseDmg, attacker);
                    PublishHitEvent(attacker, target, chaseDmg);

                    if (target.currentHealth <= 0 && target.currentHealth < hpBeforeChase) {
                        // 击杀后立即执行下一行动
                        BattleSystem.Instance?.NotifyKillAndImmediateAction(attacker);
                    }
                    break;
            }
        }

        // ==================== 击伤类（2个）====================

        private static void ExecuteStrikeAction(Unit attacker, Unit target, ActionData action) {
            switch (action.type) {
                case ActionType.戮血:
                    float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, 10f + action.level * 5f);
                    float hpBefore = target.currentHealth;
                    target.TakeDamage(dmg, attacker);
                    PublishHitEvent(attacker, target, dmg);

                    if (target.currentHealth < hpBefore) {
                        attacker.Heal(5f + action.level * 5f);
                    }
                    break;

                case ActionType.截击:
                    float strikeDmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, 10f + action.level * 2f);
                    target.TakeDamage(strikeDmg, attacker);
                    PublishHitEvent(attacker, target, strikeDmg);
                    attacker.shieldComponent.Add(5f);

                    if (action.level >= 3 && target.currentHealth <= 0) {
                        target.AddState(StateConfig.眩晕, 1);
                    }
                    break;
            }
        }

        // ==================== 蓄力类（7个）====================

        private static void ExecuteChargeAction(Unit attacker, Unit target, ActionData action) {
            switch (action.type) {
                case ActionType.蓄力:
                    attacker.AddState(StateConfig.蓄力, action.level + 1);
                    break;

                case ActionType.凝势:
                    int chargeStacks = attacker.stateComponent.GetStateStacks("蓄力");
                    if (chargeStacks > 0) {
                        attacker.stateComponent.RemoveState("蓄力");
                    }
                    break;

                case ActionType.盾化:
                    float shieldToConvert = attacker.shieldComponent.currentShield;
                    if (shieldToConvert > 0) {
                        float conversionRate = 0.5f + action.level * 0.25f;
                        int chargeGained = Mathf.FloorToInt(shieldToConvert * conversionRate);
                        attacker.shieldComponent.Clear();
                        attacker.AddState(StateConfig.蓄力, chargeGained);
                    }
                    break;

                case ActionType.连击转化:
                    int comboStacks = attacker.stateComponent.GetStateStacks("连击");
                    if (comboStacks > 0) {
                        float conversionRate = 0.3f + action.level * 0.25f;
                        int chargeGained = Mathf.FloorToInt(comboStacks * conversionRate);
                        attacker.AddState(StateConfig.蓄力, chargeGained);
                    }
                    break;

                case ActionType.蓄势:
                    attacker.AddState(StateConfig.蓄势, action.level);
                    break;

                case ActionType.藏锋:
                    float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, attacker.GetTotalAttack() * (0.5f + action.level * 0.2f));
                    target.TakeDamage(dmg, attacker);
                    PublishHitEvent(attacker, target, dmg);
                    break;

                case ActionType.聚锋:
                    attacker.AddState(StateConfig.蓄力, action.level);
                    break;
            }
        }

        // ==================== 护盾类（7个）====================

        private static void ExecuteShieldAction(Unit attacker, Unit target, ActionData action) {
            switch (action.type) {
                case ActionType.举盾:
                    float shieldAmount = attacker.defense * action.level;
                    attacker.shieldComponent.Add(shieldAmount);
                    if (action.level >= 3) {
                        attacker.AddState(StateConfig.守御, 2);
                    }
                    break;

                case ActionType.架势:
                    attacker.AddState(StateConfig.招架, action.level * 2 - 1);
                    break;

                case ActionType.护盾蓄力:
                    attacker.shieldComponent.Add(10f + action.level * 5f);
                    attacker.AddState(StateConfig.蓄力, action.level);
                    break;

                case ActionType.护盾强化:
                    float currentShield = attacker.shieldComponent.currentShield;
                    if (currentShield > 0) {
                        float multiplier = 0.25f + action.level * 0.25f;
                        attacker.shieldComponent.Add(currentShield * multiplier);
                    }
                    break;

                case ActionType.盾击:
                    float shieldDmg = attacker.shieldComponent.currentShield * (0.5f + action.level * 0.25f);
                    attacker.shieldComponent.Clear();
                    target.TakeDamage(shieldDmg, attacker);
                    break;

                case ActionType.绝甲:
                    attacker.shieldComponent.Add(15f + action.level * 5f);
                    target.AddState(StateConfig.眩晕, 1);
                    if (action.level >= 3) {
                        attacker.Heal(5f);
                    }
                    break;

                case ActionType.护甲猛击:
                    float dmg2 = DamageCalculator.CalculatePhysicalDamage(attacker, target, 10f + action.level * 5f);
                    float hpBefore = target.currentHealth;
                    target.TakeDamage(dmg2, attacker);
                    PublishHitEvent(attacker, target, dmg2);

                    if (target.currentHealth < hpBefore) {
                        float shieldGained = hpBefore - target.currentHealth;
                        attacker.shieldComponent.Add(shieldGained);
                    }
                    break;
            }
        }

        // ==================== 溃伤类（7个）====================

        private static void ExecuteWoundAction(Unit attacker, Unit target, ActionData action) {
            switch (action.type) {
                case ActionType.溃伤攻击:
                    int woundStacks = target.stateComponent.GetStateStacks("溃伤");
                    float baseDmg = 10f + action.level * 2f;
                    if (woundStacks > 0) {
                        baseDmg += action.level >= 3 ? woundStacks * 1.5f : woundStacks;
                    }
                    target.TakeDamage(baseDmg, attacker, true);
                    break;

                case ActionType.溃伤切割:
                    float dmg = DamageCalculator.CalculatePhysicalDamage(attacker, target, 10f + action.level * 2f);
                    float hpBefore = target.currentHealth;
                    target.TakeDamage(dmg, attacker);
                    PublishHitEvent(attacker, target, dmg);

                    if (target.currentHealth < hpBefore) {
                        target.AddState(StateConfig.溃伤, action.level);
                    }
                    break;

                case ActionType.溃伤爆发:
                    int woundToExplode = target.stateComponent.GetStateStacks("溃伤");
                    if (woundToExplode > 0) {
                        target.stateComponent.RemoveState("溃伤");
                        float damagePerStack = 2f + action.level;
                        target.TakeDamage(woundToExplode * damagePerStack, attacker, true);
                    }
                    break;

                case ActionType.溃伤传染:
                    attacker.AddState(StateConfig.疫病, action.level + 2);
                    break;

                case ActionType.溃伤吸取:
                    int woundToAbsorb = Mathf.Min(3, target.stateComponent.GetStateStacks("溃伤"));
                    if (woundToAbsorb > 0) {
                        target.stateComponent.RemoveState("溃伤");
                        attacker.Heal(woundToAbsorb * (2f + action.level));
                    }
                    break;

                case ActionType.溃伤护盾:
                    int woundToConvert = attacker.stateComponent.GetStateStacks("溃伤");
                    if (woundToConvert > 0) {
                        attacker.stateComponent.RemoveState("溃伤");
                        attacker.shieldComponent.Add(woundToConvert * (2f + action.level));
                    }
                    break;

                case ActionType.溃伤加深:
                    // 标记目标，下一次溃伤伤害加深50%/75%/100%
                    target.AddState(StateConfig.重击增益, action.level);
                    break;
            }
        }

        // ==================== 基础类（11个）====================

        private static void ExecuteBasicAction(Unit attacker, Unit target, ActionData action) {
            switch (action.type) {
                case ActionType.重击:
                    attacker.AddState(StateConfig.重击增益, action.level);
                    break;

                case ActionType.爆发:
                    attacker.AddState(StateConfig.爆发增益, action.level);
                    break;

                case ActionType.疾风:
                    int windCount = action.level + 1;
                    attacker.AddState(StateConfig.疾风, windCount);
                    break;

                case ActionType.坚壁:
                    attacker.AddState(StateConfig.坚壁, action.level);
                    break;

                case ActionType.刺盾:
                    attacker.AddState(StateConfig.刺甲, action.level);
                    break;

                case ActionType.狂暴:
                    attacker.AddState(StateConfig.狂怒, action.level + 1);
                    if (action.level >= 3) {
                        attacker.speed += 1f;
                    }
                    break;

                case ActionType.治疗:
                    attacker.Heal(15f + action.level * 10f);
                    break;

                case ActionType.减益冲击:
                    // 易伤/虚弱/威慑组合
                    if (action.level == 1) {
                        target.AddState(StateConfig.易伤, 1);
                    } else if (action.level == 2) {
                        target.AddState(StateConfig.易伤, 2);
                        target.AddState(StateConfig.虚弱, 1);
                    } else {
                        target.AddState(StateConfig.易伤, 3);
                        target.AddState(StateConfig.虚弱, 2);
                        target.AddState(StateConfig.威慑, 1);
                    }
                    break;

                case ActionType.猛攻:
                    // 目标有易伤/虚弱/眩晕时追加伤害
                    float baseAttackDmg = 12f + action.level * 3f;
                    int vulnStacks = target.stateComponent.GetStateStacks("易伤");
                    int weakStacks = target.stateComponent.GetStateStacks("虚弱");
                    int stunStacks = target.stateComponent.GetStateStacks("眩晕");

                    target.TakeDamage(baseAttackDmg, attacker);
                    if (vulnStacks > 0 || weakStacks > 0 || stunStacks > 0) {
                        float bonusDmg = 8f + action.level * 4f;
                        target.TakeDamage(bonusDmg, attacker);
                    }

                    if (action.level >= 3) {
                        target.TakeDamage(12f, attacker);
                        target.TakeDamage(12f, attacker);
                    }
                    break;

                case ActionType.血猎:
                    // 猎杀+致命追击合并
                    float huntDmg = 12f + action.level * 3f;
                    target.TakeDamage(huntDmg, attacker);

                    if (target.currentHealth <= 0) {
                        attacker.maxHealth += 5f + action.level * 3f;
                        attacker.currentHealth += 5f + action.level * 3f;
                        if (attacker is PlayerUnit huntPlayer) {
                            huntPlayer.bonusAttack += 2f + action.level;
                        }
                        if (action.level >= 3) {
                            attacker.AddState(StateConfig.战意, 3);
                        }
                    }
                    break;

                case ActionType.致命:
                    // 血量联动伤害
                    float fatalDmg = 10f + action.level * 2f;
                    float healthPercentLost = 1f - (attacker.currentHealth / attacker.maxHealth);
                    int bonusStacks = Mathf.FloorToInt(healthPercentLost * 10);
                    int bonusDmg = Mathf.Min(bonusStacks * (action.level + 1), 20);

                    target.TakeDamage(fatalDmg + bonusDmg, attacker);
                    break;

                case ActionType.镜像:
                    ActionData targetAction = null;
                    if (target is EnemyUnit enemy) {
                        targetAction = enemy.ChooseAction();
                    } else if (target is PlayerUnit) {
                        targetAction = target.GetCurrentAction();
                    }

                    if (targetAction != null) {
                        ActionData mirrorAction = new ActionData {
                            type = targetAction.type,
                            name = "镜像",
                            category = targetAction.category,
                            level = targetAction.level,
                            actionCost = targetAction.actionCost
                        };
                        ExecuteAction(attacker, target, mirrorAction);
                    }
                    break;
            }
        }

        // ==================== 辅助方法 ====================

        private static void PublishHitEvent(Unit attacker, Unit target, float damage) {
            EventBus.Instance.Publish(GameEventType.Hit, new AttackEventData {
                attacker = attacker,
                target = target,
                damage = damage,
                isHit = true
            });
        }
    }
}