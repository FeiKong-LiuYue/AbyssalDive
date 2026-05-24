using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 测试运行器 - 在Unity编辑器中运行测试
    /// 将此脚本附加到场景中的任意GameObject上并运行游戏即可
    /// </summary>
    public class TestRunner : MonoBehaviour {
        [Header("测试配置")]
        public int numGames = 100;
        public bool logBattles = false;

        [Header("玩家初始属性")]
        public int playerHealth = 100;
        public int playerAttack = 12;
        public int playerDefense = 2;
        public int playerSpeed = 8;

        [Header("关卡配置")]
        public int totalWaves = 5;
        public int enemiesPerWave = 1;

        private TestHarness _harness;
        private bool _testStarted = false;
        private bool _testComplete = false;

        void Start() {
            _harness = new TestHarness();
            Debug.Log("TestRunner已启动。按T键开始测试...");
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.T) && !_testStarted) {
                StartTest();
            }
        }

        void OnGUI() {
            if (!_testComplete) return;

            GUILayout.BeginArea(new Rect(10, 10, 400, 300));
            GUILayout.Label("测试结果:");
            GUILayout.Label($"胜利: {_harness.victories}");
            GUILayout.Label($"失败: {_harness.defeats}");
            GUILayout.Label($"胜率: {(float)_harness.victories / numGames * 100:F1}%");
            GUILayout.EndArea();
        }

        private void StartTest() {
            _testStarted = true;
            Debug.Log($"开始测试: {numGames}场游戏...");

            var config = new TestHarness.TestConfig {
                numEnemiesPerWave = enemiesPerWave,
                totalWaves = totalWaves,
                playerHealth = playerHealth,
                playerAttack = playerAttack,
                playerDefense = playerDefense,
                playerSpeed = playerSpeed,
                logBattles = logBattles
            };

            StartCoroutine(RunTestCoroutine(config));
        }

        private System.Collections.IEnumerator RunTestCoroutine(TestHarness.TestConfig config) {
            _harness = new TestHarness();

            for (int i = 0; i < numGames; i++) {
                if (i % 20 == 0) {
                    Debug.Log($"进度: {i}/{numGames}");
                    yield return null;
                }

                _harness.RunSingleGame(config);

                if (i == 0 && config.logBattles) {
                    Debug.Log("第一场战斗详情已输出");
                }
            }

            FinishTest(config);
        }

        private void FinishTest(TestHarness.TestConfig config) {
            _testComplete = true;
            float winRate = (float)_harness.victories / numGames * 100f;

            Debug.Log("========== 测试完成 ==========");
            Debug.Log($"总游戏数: {numGames}");
            Debug.Log($"胜利: {_harness.victories} ({winRate:F1}%)");
            Debug.Log($"失败: {_harness.defeats} ({100f - winRate:F1}%)");
            Debug.Log($"玩家属性: HP={config.playerHealth}, ATK={config.playerAttack}, DEF={config.playerDefense}, SPD={config.playerSpeed}");
            Debug.Log($"关卡: {config.totalWaves}波, 每波{config.numEnemiesPerWave}敌人");

            if (winRate < 15f) {
                Debug.LogWarning("难度过高，胜率低于15%");
            } else if (winRate < 25f) {
                Debug.Log("✓ 难度合适，胜率在15-25%区间");
            } else if (winRate < 40f) {
                Debug.LogWarning("难度偏低，胜率高于25%");
            } else {
                Debug.LogError("❌ 难度过低，胜率高于40%");
            }
        }
    }
}