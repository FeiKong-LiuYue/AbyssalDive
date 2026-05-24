namespace AbyssalDive {
    /// <summary>
    /// 战斗事件类型
    /// </summary>
    public enum GameEventType {
        // 回合事件
        TurnStart,
        TurnEnd,

        // 行动事件
        ActionExecuting,
        ActionExecuted,

        // 攻击事件
        Attack,         // 发起攻击
        Hit,            // 造成击伤（实际伤害）
        Damaged,        // 被攻击

        // 触发事件
        FollowUpTriggered,  // 追击触发
        CounterTriggered,   // 反击触发

        // 死亡事件
        Death,          // 死亡
        Kill,           // 击杀

        // 护盾事件
        ShieldBroken,   // 破盾（护盾归零）
        ShieldDamaged,  // 碎盾（护盾减少但未归零）

        // 状态事件
        StateAdded,
        StateRemoved,
        StateStackChanged,

        // 战斗事件
        BattleEnd       // 战斗结束
    }
}