using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 敌人单位
    /// </summary>
    public class EnemyUnit : Unit {
        // 敌人特性
        public List<TraitData> traits = new();

        // 敌人行动池
        public List<ActionData> actionPool = new();

        // AI行为模式（待扩展）
        public string aiPattern;

        public EnemyUnit() : base() { }

        /// <summary>
        /// AI选择行动
        /// </summary>
        public ActionData ChooseAction() {
            // 简化版：随机选择
            if (actionPool.Count == 0) return null;
            return actionPool[Random.Range(0, actionPool.Count)];
        }
    }
}