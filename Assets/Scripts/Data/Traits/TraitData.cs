using System;

namespace AbyssalDive {
    /// <summary>
    /// 特性数据定义
    /// </summary>
    [System.Serializable]
    public class TraitData {
        public string id;                 // 唯一标识
        public string name;               // 名称
        public int level;                 // 等级 1-3
        public int maxLevel = 3;

        /// <summary>
        /// 触发事件类型（可多个）
        /// </summary>
        public GameEventType[] triggerEvents;

        /// <summary>
        /// 效果描述（各等级）
        /// </summary>
        public string[] levelDescriptions = new string[3];

        /// <summary>
        /// 是否为满级（质变）
        /// </summary>
        public bool IsMaxLevel => level >= maxLevel;

        /// <summary>
        /// 获取当前等级描述
        /// </summary>
        public string GetCurrentDescription() {
            if (level >= 1 && level <= maxLevel) {
                return levelDescriptions[level - 1];
            }
            return levelDescriptions[0];
        }

        /// <summary>
        /// 特性效果接口
        /// </summary>
        public Action<Unit, Unit> effect;
    }
}