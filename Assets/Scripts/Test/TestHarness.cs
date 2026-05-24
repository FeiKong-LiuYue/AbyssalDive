using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 测试框架 - 用于自动运行游戏模拟
    /// </summary>
    public class TestHarness {
        private int _ victories = 0;
        private int _defeats = 0;
        private int _totalTurns = 0;
        private int _totalDamageDealt = 0;
        private int _totalDamageTaken = 0;

        public struct TestConfig {
            public int numEnemiesPerWave;     // 每波敌人数量
            public int totalWaves;            // 总波数
            public int playerHealth;
            public int playerAttack;
            public int playerDefense;
            public int playerSpeed;
            public bool logBattles;           // 是否打印战斗日志
        }

        public static TestConfig DefaultConfig() {
            return new TestConfig {
                numEnemiesPerWave = 1,
                totalWaves = 5,
                playerHealth = 100,
                playerAttack = 12,
                playerDefense = 2,
                playerSpeed = 8,
                logBattles = false
            };
        }

        /// <summary>
        /// 运行单次游戏
        /// </summary>
        public bool RunSingleGame(TestConfig config, Action<int> onWaveStart = null) {
            // 创建玩家
            var player = CreatePlayer(config);

            // 记录初始属性
            float initialHealth = player.maxHealth;
            float initialAttack = player.attack;

            // 运行每波战斗
            for (int wave = 1; wave <= config.totalWaves; wave++) {
                if (!player.IsAlive) break;

                if (config.logBattles) {
                    Debug.Log($"===== 第{wave}波开始 =====");
                }

                onWaveStart?.Invoke(wave);

                // 生成该波敌人
                var enemies = GenerateEnemies(wave, config.numEnemiesPerWave);

                // 创建战斗系统并开始战斗
                var battleSystem = new BattleSystem();
                battleSystem.StartBattle(player, enemies);

                // 模拟战斗直到结束
                SimulateBattle(battleSystem, player, config.logBattles);

                if (battleSystem.lastBattleResult == BattleResult.Defeat) {
                    if (config.logBattles) {
                        Debug.Log($"第{wave}波失败，游戏结束");
                    }
                    _defeats++;
                    return false;
                }

                // 战斗胜利后给予奖励
                if (wave < config.totalWaves) {
                    ApplyWaveReward(player, wave, config);
                }

                if (config.logBattles) {
                    Debug.Log($"第{wave}波胜利，玩家状态: HP={player.currentHealth:F0}/{player.maxHealth:F0}");
                }
            }

            _victories++;
            _totalTurns += (int)config.totalWaves * 10; // 估算
            return true;
        }

        /// <summary>
        /// 运行多次游戏测试
        /// </summary>
        public void RunBatchTest(int numGames, TestConfig config) {
            _victories = 0;
            _defeats = 0;
            _totalTurns = 0;
            _totalDamageDealt = 0;
            _totalDamageTaken = 0;

            for (int i = 0; i < numGames; i++) {
                if (i % 100 == 0) {
                    Debug.Log($"进度: {i}/{numGames}");
                }
                RunSingleGame(config);
            }

            PrintResults(numGames, config);
        }

        private PlayerUnit CreatePlayer(TestConfig config) {
            var player = new GameObject("TestPlayer").AddComponent<PlayerUnit>();
            player.maxHealth = config.playerHealth;
            player.currentHealth = config.playerHealth;
            player.attack = config.playerAttack;
            player.defense = config.playerDefense;
            player.speed = config.playerSpeed;

            // 设置初始行动栏
            player.actionBar = GeneratePlayerActionBar();
            player.actionBarIndex = 0;
            player.actionValue = 0;

            return player;
        }

        private List<ActionData> GeneratePlayerActionBar() {
            // 生成一个基础行动栏（模拟随机抽卡）
            var actions = new List<ActionData> {
                CreateAction(ActionType.连打, 1),
                CreateAction(ActionType.重击, 1),
                CreateAction(ActionType.治疗, 1),
                CreateAction(ActionType.追击, 1),
            };
            return actions;
        }

        private ActionData CreateAction(ActionType type, int level) {
            return new ActionData {
                type = type,
                name = type.ToString(),
                category = GetCategoryForAction(type),
                level = level,
                actionCost = 30
            };
        }

        private ActionCategory GetCategoryForAction(ActionType type) {
            // 连击类
            if (type >= ActionType.连打 && type <= ActionType.乘胜追击) {
                return ActionCategory.Combo;
            }
            // 击伤类
            if (type == ActionType.戮血 || type == ActionType.截击) {
                return ActionCategory.Strike;
            }
            // 基础类（大部分是基础）
            return ActionCategory.Basic;
        }

        private EnemyUnit[] GenerateEnemies(int wave, int count) {
            var enemies = new List<EnemyUnit>();

            for (int i = 0; i < count; i++) {
                var enemy = new GameObject($"Enemy_W{wave}_{i}").AddComponent<EnemyUnit>();
                enemy.name = $"敌人_w{wave}_{i}";

                // 敌人属性随波数成长
                float scaleFactor = 1f + (wave - 1) * 0.25f;
                enemy.maxHealth = Mathf.RoundToInt(30f * scaleFactor);
                enemy.currentHealth = enemy.maxHealth;
                enemy.attack = Mathf.RoundToInt(8f * scaleFactor);
                enemy.defense = Mathf.RoundToInt(1f * scaleFactor);
                enemy.speed = 6f + wave * 0.5f;

                // 敌人行动池
                enemy.actionPool = GenerateEnemyActionPool(wave);
                enemy.actionBar = enemy.actionPool;
                enemy.actionValue = 0;

                enemies.Add(enemy);
            }

            return enemies.ToArray();
        }

        private List<ActionData> GenerateEnemyActionPool(int wave) {
            var pool = new List<ActionData>();
            float scaleFactor = 1f + (wave - 1) * 0.15f;

            // 随机添加2-4个行动
            int numActions = Random.Range(2, 5);
            var actionTypes = new[] {
                ActionType.连打, ActionType.追击, ActionType.重击,
                ActionType.戮血, ActionType.举盾, ActionType.溃伤攻击
            };

            for (int i = 0; i < numActions; i++) {
                var actionType = actionTypes[Random.Range(0, actionTypes.Length)];
                int level = Random.Range(1, 4);
                pool.Add(new ActionData {
                    type = actionType,
                    name = actionType.ToString(),
                    category = ActionCategory.Basic,
                    level = level,
                    actionCost = Mathf.RoundToInt(25 * scaleFactor)
                });
            }

            return pool;
        }

        private void SimulateBattle(BattleSystem battleSystem, PlayerUnit player, bool log) {
            float simulatedTime = 0f;
            float timeStep = 0.1f;
            int maxIterations = 5000; // 防止死循环

            while (battleSystem.isBattleActive && simulatedTime < 100f && maxIterations > 0) {
                Time.timeScale = 100f; // 加速
                battleSystem.Update();
                simulatedTime += timeStep;
                maxIterations--;
            }

            if (log) {
                Debug.Log($"战斗结束: {battleSystem.lastBattleResult}, 耗时: {simulatedTime:F1}");
            }
        }

        private void ApplyWaveReward(PlayerUnit player, int wave, TestConfig config) {
            // 每波胜利后给予奖励（模拟roguelike奖励选择）
            // 随机选择一种奖励
            int rewardType = Random.Range(0, 4);

            switch (rewardType) {
                case 0: // 生命恢复
                    player.Heal(player.maxHealth * 0.3f);
                    break;
                case 1: // 攻击力提升
                    player.attack += 1 + wave / 2;
                    break;
                case 2: // 防御力提升
                    player.defense += 0.5f;
                    break;
                case 3: // 速度提升
                    player.speed += 0.5f;
                    break;
            }

            // 有概率获得新行动
            if (Random.value < 0.3f) {
                var newActions = new[] {
                    ActionType.连斩, ActionType.连击延续, ActionType.爆发
                };
                player.actionBar.Add(CreateAction(newActions[Random.Range(0, newActions.Length)], 1));
            }
        }

        private void PrintResults(int numGames, TestConfig config) {
            float winRate = (float)_victories / numGames * 100f;

            Debug.Log("========== 测试结果 ==========");
            Debug.Log($"总游戏数: {numGames}");
            Debug.Log($"胜利: {_victories} ({winRate:F1}%)");
            Debug.Log($"失败: {_defeats} ({100f - winRate:F1}%)");
            Debug.Log($"配置: HP={config.playerHealth}, ATK={config.playerAttack}, DEF={config.playerDefense}, SPD={config.playerSpeed}");
            Debug.Log($"波数: {config.totalWaves}, 每波敌人: {config.numEnemiesPerWave}");
            Debug.Log("==============================");

            // 根据胜率给出建议
            if (winRate < 15f) {
                Debug.Log("⚠️ 难度过高，胜率低于15%，建议: 降低敌人属性 或 增加玩家属性");
            } else if (winRate < 25f) {
                Debug.Log("✓ 难度合适，胜率在15-25%区间");
            } else if (winRate < 40f) {
                Debug.Log("⚠️ 难度偏低，胜率高于25%，建议: 提高敌人属性 或 减少奖励");
            } else {
                Debug.Log("⚠️ 难度过低，胜率高于40%，需要大幅调整");
            }
        }

        public int victories => _victories;
        public int defeats => _defeats;
    }
}