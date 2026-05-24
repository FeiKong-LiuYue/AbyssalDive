using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 玩家单位
    /// </summary>
    public class PlayerUnit : Unit {
        // 特性列表
        public List<TraitData> traits = new();

        // 永久属性增幅（Roguelike成长）
        public float bonusAttack = 0;
        public float bonusHealth = 0;
        public float bonusSpeed = 0;

        public PlayerUnit() : base() { }

        public override float GetTotalAttack() => attack + bonusAttack;

        public float GetTotalMaxHealth() => maxHealth + bonusHealth;

        public float GetTotalSpeed() => speed + bonusSpeed;

        public override void OnTurnStart() {
            base.OnTurnStart();

            var battleCharges = stateComponent.GetStateStacks("蓄势");
            if (battleCharges > 0) {
                stateComponent.AddState(StateConfig.蓄力, battleCharges);
            }
        }
    }
}