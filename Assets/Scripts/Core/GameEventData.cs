using System;

namespace AbyssalDive {
    /// <summary>
    /// 战斗事件数据基类
    /// </summary>
    public abstract class GameEventData { }

    /// <summary>
    /// 攻击事件数据
    /// </summary>
    public class AttackEventData : GameEventData {
        public Unit attacker;
        public Unit target;
        public float damage;
        public bool isHit;
        public bool isFollowUp;      // 是否追击
        public bool isCounter;      // 是否反击
        public float shieldBefore;   // 攻击前目标护盾值
    }

    /// <summary>
    /// 状态事件数据
    /// </summary>
    public class StateEventData : GameEventData {
        public Unit target;
        public StateData state;
        public int stackChange;
    }

    /// <summary>
    /// 死亡事件数据
    /// </summary>
    public class DeathEventData : GameEventData {
        public Unit deadUnit;
        public Unit killer;
    }

    /// <summary>
    /// 护盾事件数据
    /// </summary>
    public class ShieldEventData : GameEventData {
        public Unit unit;
        public float shieldBefore;
        public float shieldAfter;
    }

    /// <summary>
    /// 回合事件数据
    /// </summary>
    public class TurnEventData : GameEventData {
        public int turnNumber;
    }

    /// <summary>
    /// 行动执行事件数据
    /// </summary>
    public class ActionEventData : GameEventData {
        public Unit unit;
        public ActionData action;
    }
}