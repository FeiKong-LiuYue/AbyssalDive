namespace AbyssalDive {
    /// <summary>
    /// 战斗系统 - 战斗入口
    /// </summary>
    public class BattleSystem : IEventListener {
        private static BattleSystem _instance;
        public static BattleSystem Instance => _instance ??= new BattleSystem();

        private ATBSystem _atbSystem;
        private bool _isBattleActive = false;
        private Unit _immediateActionUnit = null;
        private PlayerUnit _currentPlayer;
        private EnemyUnit[] _currentEnemies;
        private int _currentTurn;

        public int currentTurn => _currentTurn;
        public bool isBattleActive => _isBattleActive;
        public BattleResult? lastBattleResult { get; private set; }

        private BattleSystem() {
            _atbSystem = new ATBSystem();
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        public void StartBattle(PlayerUnit player, EnemyUnit[] enemies) {
            _currentPlayer = player;
            _currentEnemies = enemies;
            _currentTurn = 0;

            _atbSystem.Clear();
            _atbSystem.RegisterUnit(player);

            foreach (var enemy in enemies) {
                _atbSystem.RegisterUnit(enemy);
            }

            _isBattleActive = true;
            lastBattleResult = null;
            _atbSystem.OnTurnStart();
        }

        /// <summary>
        /// 更新战斗
        /// </summary>
        public void Update() {
            if (!_isBattleActive) return;

            if (_atbSystem.IsBattleEnded()) {
                EndBattle();
                return;
            }

            var currentUnit = _atbSystem.GetCurrentActingUnit();
            if (currentUnit != null && currentUnit.CanAct) {
                var action = currentUnit.GetCurrentAction();
                if (action != null) {
                    ExecuteAction(currentUnit, action);

                    if (_immediateActionUnit != null) {
                        var nextAction = _immediateActionUnit.GetCurrentAction();
                        if (nextAction != null) {
                            ExecuteAction(_immediateActionUnit, nextAction);
                        }
                        _immediateActionUnit = null;
                    }

                    currentUnit.AdvanceActionBar();
                    currentUnit.actionValue -= action.actionCost;
                }
            }

            UpdateATB();
        }

        /// <summary>
        /// 执行一回合（用于测试）
        /// </summary>
        public void ExecuteTurn() {
            if (!_isBattleActive) return;

            _currentTurn++;
            _atbSystem.OnTurnStart();

            // 执行所有单位行动直到回合结束
            while (_isBattleActive && !_atbSystem.IsBattleEnded()) {
                Update();
            }
        }

        /// <summary>
        /// 通知击杀后立即行动
        /// </summary>
        public void NotifyKillAndImmediateAction(Unit unit) {
            _immediateActionUnit = unit;
        }

        private void ExecuteAction(Unit unit, ActionData action) {
            if (unit is PlayerUnit) {
                var enemies = GetEnemies();
                if (enemies.Length > 0) {
                    ActionExecutor.ExecuteAction(unit, enemies[0], action);
                }
            } else if (unit is EnemyUnit enemy) {
                var player = GetPlayer();
                if (player != null) {
                    ActionExecutor.ExecuteAction(enemy, player, action);
                }
            }
        }

        private void UpdateATB() {
            float deltaTime = Time.deltaTime;
            var units = GetAllUnits();
            foreach (var unit in units) {
                if (unit.IsAlive) {
                    unit.actionValue += unit.speed * deltaTime * 10f;
                }
            }
        }

        private void EndBattle() {
            _isBattleActive = false;
            lastBattleResult = _atbSystem.GetBattleResult();
            _atbSystem.OnTurnEnd();

            EventBus.Instance.Publish(GameEventType.BattleEnd, new BattleEndEventData {
                result = lastBattleResult.Value
            });
        }

        public void OnEvent(GameEventType eventType, object eventData) { }

        public PlayerUnit GetPlayer() {
            return _currentPlayer;
        }

        public EnemyUnit[] GetEnemies() {
            return _currentEnemies ?? new EnemyUnit[0];
        }

        public Unit[] GetAllUnits() {
            return _atbSystem.GetAllUnits();
        }

        /// <summary>
        /// 获取某单位的敌人列表
        /// </summary>
        public Unit[] GetEnemiesOf(Unit unit) {
            if (unit is PlayerUnit) {
                return GetEnemies();
            } else {
                return new Unit[] { _currentPlayer };
            }
        }

        /// <summary>
        /// 获取战斗结果
        /// </summary>
        public BattleResult GetBattleResult() {
            return _atbSystem.GetBattleResult();
        }
    }

    public class BattleEndEventData : GameEventData {
        public BattleResult result;
    }
}