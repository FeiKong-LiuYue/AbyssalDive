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
}