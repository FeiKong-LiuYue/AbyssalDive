using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 简化测试运行器 - 直接在控制台输出结果
    /// 挂载到场景中，按T键开始测试
    /// </summary>
    public class SimpleTestRunner : MonoBehaviour {
        [Header("测试配置")]
        public int numGames = 50;
        public bool verbose = false;

        void Start() {
            Debug.Log("=== 简化测试运行器 ===");
            Debug.Log("按 T 开始测试");
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                RunTest();
            }
        }

        void RunTest() {
            int wins = 0;
            int losses = 0;
            List<int> turnsPerGame = new();

            for (int i = 0; i < numGames; i++) {
                bool won = RunSingleGame(i, verbose && i < 3);
                if (won) wins++;
                else losses++;
            }

            float winRate = (float)wins / numGames * 100f;
            Debug.Log($"\n======== 测试结果 ========");
            Debug.Log($"总场次: {numGames}");
            Debug.Log($"胜利: {wins} ({winRate:F1}%)");
            Debug.Log($"失败: {losses} ({100f - winRate:F1}%)");

            if (winRate < 15f) {
                Debug.LogWarning("胜率 <15%: 难度过高");
            } else if (winRate < 25f) {
                Debug.Log("胜率 15-25%: 难度合适");
            } else if (winRate < 40f) {
                Debug.LogWarning("胜率 25-40%: 难度偏低");
            } else {
                Debug.LogError("胜率 >40%: 难度过低!");
            }
        }

        bool RunSingleGame(int gameIndex, bool log) {
            // 创建玩家
            var player = CreatePlayer();

            if (log) Debug.Log($"--- 游戏 {gameIndex} ---");

            // 5波战斗
            for (int wave = 1; wave <= 5; wave++) {
                if (!player.IsAlive) break;

                if (log) Debug.Log($"第{wave}波开始: HP={player.currentHealth:F0}");

                // 创建敌人
                var enemy = CreateEnemy(wave);
                var battleSystem = new BattleSystem();
                battleSystem.StartBattle(player, new EnemyUnit[] { enemy });

                // 模拟战斗
                SimulateBattle(battleSystem, player, log);

                if (!player.IsAlive) {
                    if (log) Debug.Log($"第{wave}波失败");
                    return false;
                }

                if (log) Debug.Log($"第{wave}波胜利: HP={player.currentHealth:F0}");

                // 奖励
                ApplyReward(player, wave);
            }

            return true;
        }

        PlayerUnit CreatePlayer() {
            var go = new GameObject("TestPlayer");
            var player = go.AddComponent<PlayerUnit>();
            player.name = "玩家";
            player.maxHealth = 100;
            player.currentHealth = 100;
            player.attack = 12;
            player.defense = 2;
            player.speed = 8;

            // 初始行动 - 必须设置正确的category
            player.actionBar = new List<ActionData> {
                CreateAction(ActionType.连打, 1, ActionCategory.Combo),
                CreateAction(ActionType.重击, 1, ActionCategory.Basic),
                CreateAction(ActionType.治疗, 1, ActionCategory.Basic),
                CreateAction(ActionType.追击, 1, ActionCategory.Combo),
            };
            player.actionValue = 0;
            return player;
        }

        EnemyUnit CreateEnemy(int wave) {
            var go = new GameObject($"Enemy_w{wave}");
            var enemy = go.AddComponent<EnemyUnit>();
            enemy.name = $"敌人_w{wave}";

            float scale = 1f + (wave - 1) * 0.3f;
            enemy.maxHealth = Mathf.RoundToInt(30 * scale);
            enemy.currentHealth = enemy.maxHealth;
            enemy.attack = Mathf.RoundToInt(8 * scale);
            enemy.defense = Mathf.RoundToInt(1 * scale);
            enemy.speed = 6f + wave * 0.5f;

            enemy.actionPool = new List<ActionData> {
                CreateAction(ActionType.连打, 1, ActionCategory.Combo),
                CreateAction(ActionType.追击, 1, ActionCategory.Combo),
                CreateAction(ActionType.重击, 1, ActionCategory.Basic),
            };
            enemy.actionBar = enemy.actionPool;
            enemy.actionValue = 0;
            return enemy;
        }

        ActionData CreateAction(ActionType type, int level, ActionCategory category = ActionCategory.Basic) {
            return new ActionData {
                type = type,
                name = type.ToString(),
                category = category,
                level = level,
                actionCost = 30
            };
        }

        void SimulateBattle(BattleSystem bs, PlayerUnit player, bool log) {
            int tick = 0;
            int maxTicks = 2000;

            while (bs.isBattleActive && tick < maxTicks) {
                Time.timeScale = 100f;
                bs.Update();
                tick++;
            }

            if (log) Debug.Log($"战斗结束: {bs.lastBattleResult}, ticks={tick}");
        }

        void ApplyReward(PlayerUnit player, int wave) {
            // 恢复少量生命
            player.Heal(player.maxHealth * 0.2f);
            // 略微提升攻击
            player.attack += 0.5f;
        }
    }
}