using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 状态解析器 - 处理状态衰减和DOT伤害
    /// </summary>
    public class StateResolver {

        // 回合衰减型状态列表
        private static readonly string[] TurnDecayStates = {
            "易伤", "虚弱", "溃伤", "守御", "招架", "坚壁",
            "荆棘", "碎盾", "眩晕", "治愈", "威慑", "连击"
        };

        // 永久叠层型状态列表（战斗结束清空）
        private static readonly string[] PermanentStates = {
            "蓄势", "战意", "力量", "狂怒"
        };

        /// <summary>
        /// 处理回合开始的状态效果
        /// </summary>
        public static void ProcessTurnStart(Unit unit) {
            // 回合开始时，蓄势提供蓄力，战意+1层
            int battleStacks = unit.stateComponent.GetStateStacks("蓄势");
            if (battleStacks > 0) {
                unit.AddState(GetStateData("蓄力"), battleStacks);
            }

            int warStacks = unit.stateComponent.GetStateStacks("战意");
            if (warStacks >= 0) {
                unit.AddState(GetStateData("战意"), 1);
            }

            // 治愈每回合恢复
            int healStacks = unit.stateComponent.GetStateStacks("治愈");
            if (healStacks > 0) {
                unit.Heal(healStacks);
            }

            // DOT伤害（溃伤）在BattleSystem的回合开始处理
        }

        /// <summary>
        /// 处理回合结束的状态效果
        /// </summary>
        public static void ProcessTurnEnd(Unit unit) {
            // 回合衰减型状态 -1层
            foreach (string stateId in TurnDecayStates) {
                int stacks = unit.stateComponent.GetStateStacks(stateId);
                if (stacks > 0) {
                    // 移除当前层数
                    StateData stateData = GetStateData(stateId);
                    unit.stateComponent.RemoveState(stateId);

                    // 如果还有剩余，添加减少后的层数
                    if (stacks > 1) {
                        unit.stateComponent.AddState(stateData, stacks - 1);
                    }
                }
            }

            // 招架状态：回合结束提供护盾（生效后衰减）
            int guardStacks = unit.stateComponent.GetStateStacks("招架");
            if (guardStacks > 0) {
                float shieldAmount = guardStacks * unit.defense;
                unit.shieldComponent.Add(shieldAmount);
            }

            // 护盾减半（如果无坚壁状态）
            if (!unit.stateComponent.HasState("坚壁")) {
                unit.shieldComponent.Halve();
            }

            // 铁壁检查：受击消耗型，无需在此处理
        }

        /// <summary>
        /// 处理受击时的状态效果
        /// </summary>
        public static bool ProcessOnHit(Unit unit, float damage, Unit attacker) {
            bool damageBlocked = false;

            // 铁壁：抵消物理伤害（每层抵消一次）
            int ironWallStacks = unit.stateComponent.GetStateStacks("铁壁");
            if (ironWallStacks > 0 && damage > 0) {
                // 消耗1层铁壁，本次伤害无效
                unit.stateComponent.RemoveState("铁壁");
                damageBlocked = true;
                // 注意：铁壁是受击消耗型，每被攻击一次-1层
            }

            // 荆棘：受到攻击时向攻击方施加荆棘层数的物理伤害
            int thornStacks = unit.stateComponent.GetStateStacks("荆棘");
            if (thornStacks > 0 && attacker != null) {
                attacker.TakeDamage(thornStacks);
            }

            // 刺甲：受到攻击时向攻击方施加刺甲层数的溃伤
            int spikeStacks = unit.stateComponent.GetStateStacks("刺甲");
            if (spikeStacks > 0 && attacker != null) {
                attacker.AddState(GetStateData("溃伤"), spikeStacks);
            }

            // 威慑：存在时降低攻击方1点临时力量
            int intimidationStacks = unit.stateComponent.GetStateStacks("威慑");
            if (intimidationStacks > 0 && attacker != null) {
                attacker.AddState(GetStateData("力量"), -1); // 负数表示减少
            }

            // 碎甲：造成物理伤害时，扣除目标等量于层数的护盾
            int armorBreakStacks = unit.stateComponent.GetStateStacks("碎甲");
            if (armorBreakStacks > 0 && attacker != null && attacker.shieldComponent.currentShield > 0) {
                attacker.shieldComponent.Reduce(armorBreakStacks);
            }

            return damageBlocked;
        }

        /// <summary>
        /// 处理状态触发（从TriggerSystem调用）
        /// </summary>
        public static void ProcessStateTrigger(Unit unit, GameEventType eventType, object eventData) {
            // 根据事件类型处理状态效果
            switch (eventType) {
                case GameEventType.Hit:
                    // 击伤时处理疫病状态
                    int plagueStacks = unit.stateComponent.GetStateStacks("疫病");
                    if (plagueStacks > 0 && eventData is AttackEventData hitData) {
                        hitData.target.AddState(GetStateData("溃伤"), 1);
                        // 触发成功后减少1层
                        unit.stateComponent.RemoveState("疫病");
                        unit.AddState(GetStateData("疫病"), plagueStacks - 1);
                    }
                    break;

                case GameEventType.Attack:
                    // 攻击时处理奇袭状态
                    int ambushStacks = unit.stateComponent.GetStateStacks("奇袭");
                    if (ambushStacks > 0) {
                        // 每层+10%追加攻击概率，触发成功后减少1层
                        // 具体概率判定在ActionExecutor中进行
                    }
                    break;

                case GameEventType.Damaged:
                    // 受击时处理警戒状态
                    int alertStacks = unit.stateComponent.GetStateStacks("警戒");
                    if (alertStacks > 0 && eventData is AttackEventData dmgData) {
                        // 每层+10%反击概率，触发成功后减少1层
                        // 具体概率判定在受伤处理中进行
                    }
                    break;
            }
        }

        /// <summary>
        /// 战斗结束时清空状态
        /// </summary>
        public static void ClearBattleStates(Unit unit) {
            // 清空永久叠层型状态
            foreach (string stateId in PermanentStates) {
                unit.stateComponent.RemoveState(stateId);
            }

            // 清空所有状态
            unit.stateComponent.Clear();
        }

        /// <summary>
        /// 获取状态数据（从配置获取）
        /// </summary>
        private static StateData GetStateData(string stateId) {
            return StateConfig.Get(stateId);
        }
    }
}