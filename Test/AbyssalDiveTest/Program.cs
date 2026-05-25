using System;
using System.Collections.Generic;

namespace AbyssalDive {
    class Program {
        static int totalWins = 0;
        static int totalLosses = 0;
        static int totalGames = 500;

        static void Main(string[] args) {
            Console.WriteLine("=== 潜渊 (Abyssal Dive) 测试 ===");
            Console.WriteLine();

            var config = new GameConfig {
                playerHealth = 100,
                playerAttack = 12,
                playerDefense = 2,
                playerSpeed = 8,
                waves = 5,
                enemyBaseHealth = 30,
                enemyBaseAttack = 8,
                enemyScaling = 0.33f
            };

            Console.WriteLine($"配置: HP={config.playerHealth}, ATK={config.playerAttack}, DEF={config.playerDefense}, SPD={config.playerSpeed}");
            Console.WriteLine($"波数: {config.waves}, 敌人成长: {config.enemyScaling*100}%/波");
            Console.WriteLine();

            for (int i = 0; i < totalGames; i++) {
                if (i % 20 == 0) Console.WriteLine($"进度: {i}/{totalGames}");
                bool won = RunSingleGame(config);
                if (won) totalWins++;
                else totalLosses++;
            }

            PrintResults();
        }

        static Random _globalRandom = new Random();

        static bool RunSingleGame(GameConfig config) {
            var player = new UnitSim {
                maxHealth = config.playerHealth,
                currentHealth = config.playerHealth,
                attack = config.playerAttack,
                defense = config.playerDefense,
                speed = config.playerSpeed
            };

            for (int wave = 1; wave <= config.waves; wave++) {
                if (!player.IsAlive) return false;

                float scale = 1f + (wave - 1) * config.enemyScaling;
                var enemy = new UnitSim {
                    maxHealth = (int)(config.enemyBaseHealth * scale),
                    currentHealth = (int)(config.enemyBaseHealth * scale),
                    attack = (int)(config.enemyBaseAttack * scale),
                    defense = 1,
                    speed = 6f + wave * 0.5f
                };

                var battle = new BattleSim(player, enemy);
                while (battle.IsActive) {
                    battle.Update();
                    if (battle.Tick > 5000) break;
                }

                if (!player.IsAlive) return false;

                player.currentHealth = Math.Min(player.maxHealth, player.currentHealth + (int)(player.maxHealth * (0.12f + _globalRandom.NextDouble() * 0.06f)));
                player.attack += 1;
            }

            return true;
        }

        static void PrintResults() {
            float winRate = (float)totalWins / totalGames * 100f;
            Console.WriteLine();
            Console.WriteLine("========== 结果 ==========");
            Console.WriteLine($"总游戏数: {totalGames}");
            Console.WriteLine($"胜利: {totalWins} ({winRate:F1}%)");
            Console.WriteLine($"失败: {totalLosses} ({100f - winRate:F1}%)");
            Console.WriteLine();

            if (winRate < 15f) Console.WriteLine("评估: 难度过高 (胜率<15%)");
            else if (winRate < 25f) Console.WriteLine("评估: 难度合适 (胜率15-25%)");
            else if (winRate < 40f) Console.WriteLine("评估: 难度偏低 (胜率25-40%)");
            else Console.WriteLine("评估: 难度过低 (胜率>40%)");
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

    class UnitSim {
        public int maxHealth;
        public int currentHealth;
        public int attack;
        public int defense;
        public float speed;
        public float actionValue;
        public int actionIndex;
        public bool IsAlive => currentHealth > 0;
        public int ActionCost => 30;
    }

    class BattleSim {
        UnitSim _player;
        UnitSim _enemy;
        bool _isActive = true;
        Random _random = new Random();
        public bool IsActive => _isActive;
        public int Tick { get; private set; }

        public BattleSim(UnitSim player, UnitSim enemy) {
            _player = player;
            _enemy = enemy;
            _player.actionValue = 0;
            _enemy.actionValue = 0;
            _player.actionIndex = 0;
            _enemy.actionIndex = 0;
            Tick = 0;
        }

        public void Update() {
            Tick++;
            if (!_isActive) return;

            if (!_player.IsAlive || !_enemy.IsAlive) {
                _isActive = false;
                return;
            }

            // ATB恢复 - 添加随机波动模拟真实ATB
            _player.actionValue += _player.speed * 2f * (float)(0.9f + _random.NextDouble() * 0.2f);
            _enemy.actionValue += _enemy.speed * 2f * (float)(0.9f + _random.NextDouble() * 0.2f);

            // 玩家行动
            if (_player.actionValue >= _player.ActionCost) {
                PlayerAct();
                _player.actionValue -= _player.ActionCost;
            }

            // 敌人行动
            if (_enemy.IsAlive && _enemy.actionValue >= _enemy.ActionCost) {
                EnemyAct();
                _enemy.actionValue -= _enemy.ActionCost;
            }
        }

        void PlayerAct() {
            int act = _player.actionIndex % 4;
            _player.actionIndex++;

            switch (act) {
                case 0: // 连打 - 2次50%攻击
                    DealDamage(_player, _enemy, (int)(_player.attack * 0.5f));
                    DealDamage(_player, _enemy, (int)(_player.attack * 0.5f));
                    break;
                case 1: // 重击 - 造成100%攻击伤害
                    DealDamage(_player, _enemy, _player.attack);
                    break;
                case 2: // 治疗 - 15治疗
                    _player.currentHealth = Math.Min(_player.maxHealth, _player.currentHealth + 15);
                    break;
                case 3: // 追击 - 10 + 60%
                    DealDamage(_player, _enemy, 10);
                    DealDamage(_player, _enemy, (int)(_player.attack * 0.6f));
                    break;
            }
        }

        void EnemyAct() {
            int act = _enemy.actionIndex % 3;
            _enemy.actionIndex++;

            switch (act) {
                case 0: // 连打 - 2次50%攻击
                    DealDamage(_enemy, _player, (int)(_enemy.attack * 0.5f));
                    DealDamage(_enemy, _player, (int)(_enemy.attack * 0.5f));
                    break;
                case 1: // 追击 - 8 + 50%
                    DealDamage(_enemy, _player, 8);
                    DealDamage(_enemy, _player, (int)(_enemy.attack * 0.5f));
                    break;
                case 2: // 重击 - 100%攻击伤害
                    DealDamage(_enemy, _player, _enemy.attack);
                    break;
            }
        }

        void DealDamage(UnitSim attacker, UnitSim target, int damage) {
            int variance = (int)((_random.NextDouble() - 0.5) * damage * 0.3); // +/-15% damage variance
            int actual = Math.Max(0, damage + variance - target.defense);
            target.currentHealth -= actual;
        }
    }
}