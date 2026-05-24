namespace AbyssalDive {
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameState {
        Menu,           // 主菜单
        Preparation,    // 战备阶段
        Battle,         // 战斗阶段
        Reward,         // 奖励阶段
        GameOver        // 游戏结束
    }

    /// <summary>
    /// 游戏主管理器
    /// </summary>
    public class GameManager {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        private GameState _currentState = GameState.Menu;
        private int _currentFloor = 1;
        private int _currentStage = 1;

        // 玩家数据
        public PlayerUnit player;

        // 存档系统
        private SaveSystem _saveSystem;

        private GameManager() {
            _saveSystem = new SaveSystem();
        }

        /// <summary>
        /// 获取当前游戏状态
        /// </summary>
        public GameState CurrentState => _currentState;

        /// <summary>
        /// 获取当前层数
        /// </summary>
        public int CurrentFloor => _currentFloor;

        /// <summary>
        /// 获取当前阶段
        /// </summary>
        public int CurrentStage => _currentStage;

        /// <summary>
        /// 切换游戏状态
        /// </summary>
        public void ChangeState(GameState newState) {
            _currentState = newState;

            // 状态切换时执行相应逻辑
            switch (newState) {
                case GameState.Preparation:
                    OnEnterPreparation();
                    break;
                case GameState.Battle:
                    OnEnterBattle();
                    break;
                case GameState.Reward:
                    OnEnterReward();
                    break;
                case GameState.GameOver:
                    OnEnterGameOver();
                    break;
            }
        }

        /// <summary>
        /// 进入战备阶段
        /// </summary>
        private void OnEnterPreparation() {
            // TODO: 加载敌人数据
        }

        /// <summary>
        /// 进入战斗阶段
        /// </summary>
        private void OnEnterBattle() {
            // TODO: 初始化战斗系统
        }

        /// <summary>
        /// 进入奖励阶段
        /// </summary>
        private void OnEnterReward() {
            // TODO: 显示奖励选择
        }

        /// <summary>
        /// 进入游戏结束
        /// </summary>
        private void OnEnterGameOver() {
            // TODO: 保存结算数据
        }

        /// <summary>
        /// 开始新游戏
        /// </summary>
        public void StartNewGame() {
            _currentFloor = 1;
            _currentStage = 1;

            // 初始化玩家
            player = new PlayerUnit();
            player.attack = 10;
            player.maxHealth = 100;
            player.currentHealth = 100;
            player.defense = 5;
            player.speed = 1.0f;

            ChangeState(GameState.Preparation);
        }

        /// <summary>
        /// 继续游戏（从存档加载）
        /// </summary>
        public void ContinueGame() {
            // TODO: 从存档加载数据
            ChangeState(GameState.Preparation);
        }

        /// <summary>
        /// 进入下一层
        /// </summary>
        public void NextFloor() {
            _currentFloor++;

            // 每5层切换阶段
            if (_currentFloor % 5 == 1 && _currentFloor > 1) {
                _currentStage = (_currentFloor - 1) / 5 + 1;
            }

            // 检查是否通关（20层）
            if (_currentFloor > 20) {
                ChangeState(GameState.GameOver);
            } else {
                ChangeState(GameState.Preparation);
            }
        }

        /// <summary>
        /// 保存游戏
        /// </summary>
        public void SaveGame() {
            _saveSystem.Save(this);
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        public void LoadGame() {
            _saveSystem.Load(this);
        }
    }
}