using System;
using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 独立测试运行器 - 不依赖Unity，可在命令行运行
    /// </summary>
    class Program {
        static int totalWins = 0;
        static int totalLosses = 0;
        static int totalGames = 100;

        static void Main(string[] args) {
            Console.WriteLine("=== 潜渊 (Abyssal Dive) 测试 ===");
            Console.WriteLine();

            // 默认配置
            var config = new GameConfig {
                playerHealth = 100,
                playerAttack = 12,
                playerDefense = 2,
                playerSpeed = 8,
                waves = 5,
                enemyBaseHealth = 30,
                enemyBaseAttack = 8,
                enemyScaling = 0.3f  // 每波+30%
            };

            Console.WriteLine($"配置: HP={config.playerHealth}, ATK={config.playerAttack}, DEF={config.playerDefense}, SPD={config.playerSpeed}");
            Console.WriteLine($"波数: {config.waves}, 敌人成长: {config.enemyScaling*100}%/波");
            Console.WriteLine();

            for (int i = 0; i < totalGames; i++) {
                if (i % 20 == 0) Console.WriteLine($"进度: {i}/{totalGames}");

                bool won = RunSingleGame(config, i);
                if (won) totalWins++;
                else totalLosses++;
            }

            PrintResults();
        }

        static bool RunSingleGame(GameConfig config, int gameIndex) {
            var player = CreatePlayer(config);
            int wave = 1;

            while (wave <= config.waves) {
                if (!player.IsAlive) return false;

                var enemy = CreateEnemy(wave, config);
                var battle = new BattleSimulator();
                battle.StartBattle(player, new[] { enemy });

                while (battle.IsActive) {
                    battle.Update();
                    if (battle.Tick > 5000) break; // 防止死循环
                }

                if (!player.IsAlive) return false;

                // 波次间奖励
                player.currentHealth = Math.Min(player.currentHealth + player.maxHealth * 0.2f, player.maxHealth);
                player.attack += 0.5f;

                wave++;
            }

            return true;
        }

        static PlayerSim CreatePlayer(GameConfig config) {
            return new PlayerSim {
                maxHealth = config.playerHealth,
                currentHealth = config.playerHealth,
                attack = config.playerAttack,
                defense = config.playerDefense,
                speed = config.playerSpeed,
                actionBar = new List<ActionSim> {
                    new ActionSim { type = "连打", cost = 30, category = ActionCategory.Combo },
                    new ActionSim { type = "重击", cost = 30, category = ActionCategory.Basic },
                    new ActionSim { type = "治疗", cost = 30, category = ActionCategory.Basic },
                    new ActionSim { type = "追击", cost = 30, category = ActionCategory.Combo },
                }
            };
        }

        static EnemySim CreateEnemy(int wave, GameConfig config) {
            float scale = 1f + (wave - 1) * config.enemyScaling;
            return new EnemySim {
                maxHealth = (int)(config.enemyBaseHealth * scale),
                currentHealth = (int)(config.enemyBaseHealth * scale),
                attack = (int)(config.enemyBaseAttack * scale),
                defense = 1,
                speed = 6f + wave * 0.5f,
                actionPool = new List<ActionSim> {
                    new ActionSim { type = "连打", cost = 25, category = ActionCategory.Combo },
                    new ActionSim { type = "追击", cost = 25, category = ActionCategory.Combo },
                    new ActionSim { type = "重击", cost = 25, category = ActionCategory.Basic },
                }
            };
        }

        static void PrintResults() {
            float winRate = (float)totalWins / totalGames * 100f;
            Console.WriteLine();
            Console.WriteLine("========== 结果 ==========");
            Console.WriteLine($"总游戏数: {totalGames}");
            Console.WriteLine($"胜利: {totalWins} ({winRate:F1}%)");
            Console.WriteLine($"失败: {totalLosses} ({100f - winRate:F1}%)");
            Console.WriteLine();

            if (winRate < 15f) {
                Console.WriteLine("评估: 难度过高 (胜率<15%)");
            } else if (winRate < 25f) {
                Console.WriteLine("评估: 难度合适 (胜率15-25%)");
            } else if (winRate < 40f) {
                Console.WriteLine("评估: 难度偏低 (胜率25-40%)");
            } else {
                Console.WriteLine("评估: 难度过低 (胜率>40%)");
            }
        }
    }

    class GameConfig {
        public int playerHealth;
        public int playerAttack;
        public int playerDefense;
        public float playerSpeed;
        public int waves;
        public int enemyBaseHealth;
        public int enemyBaseAttack;
        public float enemyScaling;
    }

    // ========== 简化战斗模拟 ==========

    class PlayerSim {
        public int maxHealth;
        public int currentHealth;
        public int attack;
        public int defense;
        public float speed;
        public List<ActionSim> actionBar;
        public int actionIndex;
        public float actionValue;
        public bool IsAlive => currentHealth > 0;
    }

    class EnemySim {
        public int maxHealth;
        public int currentHealth;
        public int attack;
        public int defense;
        public float speed;
        public List<ActionSim> actionPool;
        public int actionIndex;
        public float actionValue;
        public bool IsAlive => currentHealth > 0;
    }

    class ActionSim {
        public string type;
        public int cost;
        public ActionCategory category;
        public int level = 1;
    }

    enum ActionCategory { Combo, Strike, Charge, Shield, Wound, Basic }

    class BattleSimulator {
        PlayerSim _player;
        List<EnemySim> _enemies = new();
        bool _isActive = true;
        public bool IsActive => _isActive;
        public int Tick { get; private set; }

        public void StartBattle(PlayerSim player, EnemySim[] enemies) {
            _player = player;
            _enemies = new List<EnemySim>(enemies);
            _player.actionValue = 0;
            _player.actionIndex = 0;
            foreach (var e in _enemies) {
                e.actionValue = 0;
                e.actionIndex = 0;
            }
            _isActive = true;
            Tick = 0;
        }

        public void Update() {
            Tick++;
            if (!_isActive) return;

            // 检查战斗结束
            if (!_player.IsAlive || _enemies.TrueForAll(e => !e.IsAlive)) {
                _isActive = false;
                return;
            }

            // ATB恢复
            _player.actionValue += _player.speed * 2f;
            foreach (var e in _enemies) {
                if (e.IsAlive) e.actionValue += e.speed * 2f;
            }

            // 单位行动
            TryAct(_player, _enemies[0]);
            foreach (var e in _enemies) {
                if (e.IsAlive && _isActive) TryAct(e, _player);
            }
        }

        void TryAct(PlayerSim actor, PlayerSim target) {
            if (actor.actionValue < 30) return;

            var action = actor.actionBar[actor.actionIndex % actor.actionBar.Count];
            ExecuteAction(actor, target, action);

            actor.actionValue -= action.cost;
            actor.actionIndex++;
        }

        void ExecuteAction(PlayerSim attacker, PlayerSim target, ActionSim action) {
            int damage = 0;

            switch (action.type) {
                case "连打":
                    damage = (int)(attacker.attack * 0.5f);
                    target.currentHealth -= Math.Max(0, damage - target.defense);
                    break;

                case "重击":
                    // 重击不造成伤害，只加状态
                    break;

                case "追击":
                    damage = 8 + action.level * 4;
                    int actualDmg = Math.Max(0, damage - target.defense);
                    if (actualDmg > 0) {
                        target.currentHealth -= actualDmg;
                        // 追击有追加伤害
                        target.currentHealth -= (int)(attacker.attack * 0.5f);
                    }
                    break;

                case "治疗":
                    attacker.currentHealth = Math.Min(attacker.maxHealth, attacker.currentHealth + 15);
                    break;

                default:
                    damage = attacker.attack;
                    target.currentHealth -= Math.Max(0, damage - target.defense);
                    break;
            }
        }
    }
}