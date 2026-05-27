using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbyssalDive;

namespace AbyssalDive.UI {
    /// <summary>
    /// 战备界面面板
    /// </summary>
    public class PreparationPanel : UIPanel, IUIEventListener {
        [Header("Left - Enemy Preview")]
        [SerializeField] private Transform enemyListContainer;
        [SerializeField] private GameObject enemyCardPrefab;

        [Header("Right - Player Info")]
        [SerializeField] private Text playerHealthText;
        [SerializeField] private Text playerShieldText;
        [SerializeField] private Text floorText;

        [Header("Action Bar Config")]
        [SerializeField] private Transform actionBarContainer;
        [SerializeField] private GameObject actionSlotPrefab;
        [SerializeField] private int maxActionSlots = 5;
        [SerializeField] private Button resetButton;

        [Header("Action Pool")]
        [SerializeField] private Transform actionPoolContainer;
        [SerializeField] private GameObject actionPoolItemPrefab;

        [Header("Bottom")]
        [SerializeField] private Button startBattleButton;
        [SerializeField] private Text floorInfoText;

        private List<GameObject> _actionBarSlots = new();
        private List<GameObject> _actionPoolItems = new();

        private void Start() {
            // 绑定事件
            startBattleButton.onClick.AddListener(OnStartBattle);
            resetButton.onClick.AddListener(OnResetActionBar);

            // 订阅 UI 事件
            UIEventBus.Instance.Subscribe(UIEventType.BattleStarted, this);
        }

        private void OnDestroy() {
            UIEventBus.Instance.Unsubscribe(UIEventType.BattleStarted, this);
        }

        protected override void OnShow() {
            RefreshPanel();
        }

        public void RefreshPanel() {
            UpdateFloorInfo();
            UpdatePlayerStatus();
            CreateEnemyPreview();
            CreateActionBar();
            CreateActionPool();
        }

        private void UpdateFloorInfo() {
            if (floorText != null) {
                floorText.text = $"第 {GameManager.Instance.CurrentFloor} 层";
            }
            if (floorInfoText != null) {
                int stage = (GameManager.Instance.CurrentFloor - 1) / 5 + 1;
                floorInfoText.text = $"第 {stage} 阶段 - 第 {GameManager.Instance.CurrentFloor} 层";
            }
        }

        private void UpdatePlayerStatus() {
            var player = GameManager.Instance.player;
            if (player != null) {
                if (playerHealthText != null) {
                    playerHealthText.text = $"{player.currentHealth}/{player.maxHealth}";
                }
                if (playerShieldText != null) {
                    playerShieldText.text = $"{player.shieldComponent.currentShield}";
                }
            }
        }

        private void CreateEnemyPreview() {
            // 清理旧数据
            foreach (Transform child in enemyListContainer) {
                Destroy(child.gameObject);
            }

            // 获取当前层敌人（待实现：从 StageManager 获取）
            // 目前用示例数据
            string[] sampleEnemies = { "墟骨哨卫", "纹盾祭者", "碎界行者" };
            int enemyCount = Mathf.Min(3, GameManager.Instance.CurrentFloor % 5 + 1);

            for (int i = 0; i < enemyCount; i++) {
                var card = Instantiate(enemyCardPrefab, enemyListContainer);
                var cardText = card.GetComponentInChildren<Text>();
                if (cardText != null && i < sampleEnemies.Length) {
                    cardText.text = sampleEnemies[i];
                }
            }
        }

        private void CreateActionBar() {
            // 清理旧数据
            foreach (var slot in _actionBarSlots) {
                Destroy(slot);
            }
            _actionBarSlots.Clear();

            // 创建行动栏格子
            for (int i = 0; i < maxActionSlots; i++) {
                var slot = Instantiate(actionSlotPrefab, actionBarContainer);
                _actionBarSlots.Add(slot);

                var button = slot.GetComponent<Button>();
                int index = i;
                button.onClick.AddListener(() => OnActionBarSlotClicked(index));
            }
        }

        private void CreateActionPool() {
            // 清理旧数据
            foreach (var item in _actionPoolItems) {
                Destroy(item);
            }
            _actionPoolItems.Clear();

            // 获取玩家可用行动（待实现：从玩家数据获取）
            // 目前用示例数据
            var sampleActions = new[] {
                new ActionData { id = "连打", name = "连打", category = ActionCategory.Combo, level = 1 },
                new ActionData { id = "爆发", name = "爆发", category = ActionCategory.Basic, level = 1 },
                new ActionData { id = "蓄力", name = "蓄力", category = ActionCategory.Charge, level = 1 },
                new ActionData { id = "举盾", name = "举盾", category = ActionCategory.Shield, level = 1 },
                new ActionData { id = "戮血", name = "戮血", category = ActionCategory.Strike, level = 1 },
            };

            foreach (var action in sampleActions) {
                var item = Instantiate(actionPoolItemPrefab, actionPoolContainer);
                _actionPoolItems.Add(item);

                var button = item.GetComponent<Button>();
                var actionData = action;
                button.onClick.AddListener(() => OnActionPoolItemClicked(actionData));
            }
        }

        private void OnActionBarSlotClicked(int index) {
            // 如果格子有行动，返回行动池
            UIEventBus.Instance.Publish(UIEventType.ActionBarChanged, this, index);
        }

        private void OnActionPoolItemClicked(ActionData action) {
            // 将行动添加到行动栏
            UIEventBus.Instance.Publish(UIEventType.ActionBarChanged, this, action);
        }

        private void OnStartBattle() {
            UIEventBus.Instance.Publish(UIEventType.StartBattle, this);
        }

        private void OnResetActionBar() {
            // 清空行动栏
            UIEventBus.Instance.Publish(UIEventType.ActionBarChanged, this, -1);
        }

        public void OnUIEvent(UIEventType eventType, UIEventData data) {
            if (eventType == UIEventType.BattleStarted) {
                Hide();
            }
        }

        public override void UpdatePanel() {
            // 每帧更新玩家状态
            UpdatePlayerStatus();
        }
    }
}