using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 伤害计算器
    /// </summary>
    public class DamageCalculator {

        /// <summary>
        /// 计算物理伤害
        /// </summary>
        public static float CalculatePhysicalDamage(
            Unit attacker,
            Unit target,
            float baseDamage,
            bool ignoreShield = false) {

            float damage = baseDamage;

            // ========== 攻击方加成 ==========

            // 计算连击加成
            int comboStacks = attacker.stateComponent.GetStateStacks("连击");
            if (comboStacks > 0) {
                damage *= (1f + comboStacks * 0.1f);
            }

            // 计算蓄力加成（每层+1）
            int chargeStacks = attacker.stateComponent.GetStateStacks("蓄力");
            if (chargeStacks > 0) {
                damage += chargeStacks;
            }

            // 计算狂怒加成（每层+5%）
            int furyStacks = attacker.stateComponent.GetStateStacks("狂怒");
            if (furyStacks > 0) {
                damage *= (1f + furyStacks * 0.05f);
            }

            // 计算爆发增益（下次物理伤害+30%/45%/60%）
            int burstStacks = attacker.stateComponent.GetStateStacks("爆发增益");
            if (burstStacks > 0) {
                float burstMultiplier = burstStacks switch {
                    1 => 1.3f,
                    2 => 1.45f,
                    3 => 1.6f,
                    _ => 1f
                };
                damage *= burstMultiplier;
                attacker.stateComponent.RemoveState("爆发增益");
            }

            // 计算重击增益（下次伤害+50%/75%/100%）
            int heavyStacks = attacker.stateComponent.GetStateStacks("重击增益");
            if (heavyStacks > 0) {
                float heavyMultiplier = heavyStacks switch {
                    1 => 1.5f,
                    2 => 1.75f,
                    3 => 2f,
                    _ => 1f
                };
                damage *= heavyMultiplier;
                attacker.stateComponent.RemoveState("重击增益");
            }

            // 计算战意加成（每层+?）
            int warStacks = attacker.stateComponent.GetStateStacks("战意");
            if (warStacks > 0) {
                damage *= (1f + warStacks * 0.05f);
            }

            // 计算力量调整
            int powerStacks = attacker.stateComponent.GetStateStacks("力量");
            damage += powerStacks;

            // ========== 目标方减益 ==========

            // 计算虚弱减益（每层-10%）
            int weakStacks = target.stateComponent.GetStateStacks("虚弱");
            if (weakStacks > 0) {
                damage *= (1f - weakStacks * 0.1f);
            }

            // 计算易伤增伤（每层+15%）
            int vulnStacks = target.stateComponent.GetStateStacks("易伤");
            if (vulnStacks > 0) {
                damage *= (1f + vulnStacks * 0.15f);
            }

            // ========== 护盾抵扣 ==========

            if (!ignoreShield && target.shieldComponent.currentShield > 0) {
                float shieldAbsorb = Mathf.Min(damage, target.shieldComponent.currentShield);
                target.shieldComponent.Reduce(shieldAbsorb);
                damage -= shieldAbsorb;
            }

            // ========== 护甲减免 ==========

            damage -= target.defense;
            if (damage < 0) damage = 0;

            return damage;
        }

        /// <summary>
        /// 计算溃伤伤害（无视护盾）
        /// </summary>
        public static float CalculateWoundDamage(Unit target, float baseDamagePerStack = 1f) {
            int woundStacks = target.stateComponent.GetStateStacks("溃伤");
            if (woundStacks <= 0) return 0;

            return woundStacks * baseDamagePerStack;
        }

        /// <summary>
        /// 计算DOT总伤害（回合开始时调用）
        /// </summary>
        public static float CalculateDOTDamage(Unit target) {
            float totalDamage = 0f;

            // 溃伤
            int woundStacks = target.stateComponent.GetStateStacks("溃伤");
            if (woundStacks > 0) {
                totalDamage += woundStacks;
            }

            return totalDamage;
        }
    }
}