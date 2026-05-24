using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// ATB行动值系统
    /// </summary>
    public class ATBSystem {
        private List<Unit> _units = new();
        private int _currentUnitIndex = 0;
        private int _turnNumber = 1;

        public int TurnNumber => _turnNumber;

        /// <summary>
        /// 注册单位
        /// </summary>
        public void RegisterUnit(Unit unit) {
            if (!_units.Contains(unit)) {
                _units.Add(unit);
            }
        }

        /// <summary>
        /// 注销单位
        /// </summary>
        public void UnregisterUnit(Unit unit) {
            _units.Remove(unit);
        }

        /// <summary>
        /// 处理回合开始
        /// </summary>
        public void OnTurnStart() {
            _turnNumber++;

            // 每个单位回合开始
            foreach (var unit in _units) {
                if (unit.IsAlive) {
                    unit.OnTurnStart();
                }
            }

            // 发布回合开始事件
            EventBus.Instance.Publish(GameEventType.TurnStart, null);
        }

        /// <summary>
        /// 处理回合结束
        /// </summary>
        public void OnTurnEnd() {
            // 每个单位回合结束
            foreach (var unit in _units) {
                if (unit.IsAlive) {
                    unit.OnTurnEnd();
                }
            }

            // 发布回合结束事件
            EventBus.Instance.Publish(GameEventType.TurnEnd, null);
        }

        /// <summary>
        /// 获取当前行动的单位
        /// </summary>
        public Unit GetCurrentActingUnit() {
            // 按行动值排序
            SortUnitsByActionValue();

            // 返回行动值最高的存活单位
            foreach (var unit in _units) {
                if (unit.IsAlive && unit.CanAct) {
                    return unit;
                }
            }
            return null;
        }

        /// <summary>
        /// 按行动值排序
        /// </summary>
        private void SortUnitsByActionValue() {
            _units.Sort((a, b) => b.actionValue.CompareTo(a.actionValue));
        }

        /// <summary>
        /// 执行单位行动
        /// </summary>
        public void ExecuteUnitAction(Unit unit) {
            if (!unit.IsAlive || !unit.CanAct) return;

            var action = unit.GetCurrentAction();
            if (action == null) return;

            // 发布行动开始事件
            EventBus.Instance.Publish(GameEventType.ActionExecuting, new ActionExecutingEventData {
                unit = unit,
                action = action
            });

            // TODO: 执行行动逻辑

            // 扣除行动值
            unit.actionValue -= action.actionCost;

            // 推进行动栏
            unit.AdvanceActionBar();

            // 发布行动结束事件
            EventBus.Instance.Publish(GameEventType.ActionExecuted, new ActionExecutingEventData {
                unit = unit,
                action = action
            });
        }

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        public bool IsBattleEnded() {
            bool hasAlivePlayer = false;
            bool hasAliveEnemy = false;

            foreach (var unit in _units) {
                if (unit is PlayerUnit && unit.IsAlive) hasAlivePlayer = true;
                if (unit is EnemyUnit && unit.IsAlive) hasAliveEnemy = true;
            }

            return !hasAlivePlayer || !hasAliveEnemy;
        }

        /// <summary>
        /// 获取战斗结果
        /// </summary>
        public BattleResult GetBattleResult() {
            bool playerAlive = false;
            bool enemyAlive = false;

            foreach (var unit in _units) {
                if (unit is PlayerUnit && unit.IsAlive) playerAlive = true;
                if (unit is EnemyUnit && unit.IsAlive) enemyAlive = true;
            }

            if (playerAlive && !enemyAlive) return BattleResult.Victory;
            if (!playerAlive && enemyAlive) return BattleResult.Defeat;
            return BattleResult.InProgress;
        }

        /// <summary>
        /// 清空所有单位
        /// </summary>
        public void Clear() {
            _units.Clear();
            _currentUnitIndex = 0;
            _turnNumber = 0;
        }

        /// <summary>
        /// 获取所有单位
        /// </summary>
        public Unit[] GetAllUnits() {
            return _units.ToArray();
        }
    }

    public enum BattleResult {
        InProgress,
        Victory,
        Defeat
    }

    /// <summary>
    /// 行动执行事件数据
    /// </summary>
    public class ActionExecutingEventData : GameEventData {
        public Unit unit;
        public ActionData action;
    }
}