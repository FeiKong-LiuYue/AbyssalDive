namespace AbyssalDive {
    /// <summary>
    /// 行动分类
    /// </summary>
    public enum ActionCategory {
        Combo,      // 连击类
        Strike,     // 击伤类
        Charge,     // 蓄力类
        Shield,     // 护盾类
        Wound,      // 溃伤类
        Basic       // 基础类
    }

    /// <summary>
    /// 行动数据定义
    /// </summary>
    [System.Serializable]
    public class ActionData {
        public ActionType type;           // 行动类型（枚举）
        public string name;                // 显示名称
        public ActionCategory category;    // 分类
        public int level;                  // 等级 1-3
        public int actionCost;             // 行动值消耗

        /// <summary>
        /// 效果类型描述
        /// </summary>
        public string effectType;

        /// <summary>
        /// 触发条件描述
        /// </summary>
        public string triggerCondition;

        /// <summary>
        /// 效果描述（各等级）
        /// </summary>
        public string[] levelDescriptions = new string[3];

        /// <summary>
        /// 是否为满级（质变）
        /// </summary>
        public bool IsMaxLevel => level >= 3;

        /// <summary>
        /// 获取当前等级描述
        /// </summary>
        public string GetCurrentDescription() {
            if (level >= 1 && level <= 3) {
                return levelDescriptions[level - 1];
            }
            return levelDescriptions[0];
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ActionData() {
            level = 1;
            levelDescriptions = new string[3];
        }
    }
}