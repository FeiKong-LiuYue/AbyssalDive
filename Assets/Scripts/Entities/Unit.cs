using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 战斗单位基类
    /// </summary>
    public abstract class Unit {
        public string id;
        public string name;

        // 属性
        public float attack;
        public float maxHealth;
        public float currentHealth;
        public float defense;
        public float speed;

        // ATB行动值
        public float actionValue;
        public float actionThreshold = 100f; // 行动门槛

        // 行动栏
        public List<ActionData> actionBar = new();
        public int actionBarIndex = 0;

        // 组件
        public HealthComponent healthComponent;
        public ShieldComponent shieldComponent;
        public StateComponent stateComponent;

        public Unit() {
            healthComponent = new HealthComponent(this);
            shieldComponent = new ShieldComponent(this);
            stateComponent = new StateComponent(this);
        }

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => currentHealth > 0;

        /// <summary>
        /// 是否可以行动
        /// </summary>
        public bool CanAct => actionValue >= actionThreshold && IsAlive;

        /// <summary>
        /// 获取当前应执行的行动
        /// </summary>
        public ActionData GetCurrentAction() {
            if (actionBar.Count == 0) return null;
            return actionBar[actionBarIndex % actionBar.Count];
        }

        /// <summary>
        /// 执行行动后推进行动栏
        /// </summary>
        public void AdvanceActionBar() {
            actionBarIndex = (actionBarIndex + 1) % actionBar.Count;
        }

        /// <summary>
        /// 造成伤害
        /// </summary>
        public virtual void TakeDamage(float damage, Unit attacker = null, bool ignoreShield = false) {
            healthComponent.TakeDamage(damage, attacker, ignoreShield);
        }

        /// <summary>
        /// 治疗
        /// </summary>
        public virtual void Heal(float amount) {
            healthComponent.Heal(amount);
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        public void AddState(StateData state, int layers = 1) {
            stateComponent.AddState(state, layers);
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        public void RemoveState(string stateId) {
            stateComponent.RemoveState(stateId);
        }

        /// <summary>
        /// 每回合开始时调用
        /// </summary>
        public virtual void OnTurnStart() {
            // 状态衰减
            stateComponent.ProcessTurnStart();

            // ATB恢复
            actionValue += speed * 10f; // 速度影响行动值积累
        }

        /// <summary>
        /// 每回合结束时调用
        /// </summary>
        public virtual void OnTurnEnd() {
            // 护盾减半
            shieldComponent.Halve();

            // 状态衰减
            stateComponent.ProcessTurnEnd();
        }

        /// <summary>
        /// 获取总攻击力（派生类可覆盖）
        /// </summary>
        public virtual float GetTotalAttack() => attack;
    }
}