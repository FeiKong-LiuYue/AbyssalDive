using UnityEngine;
using UnityEngine.UI;
using AbyssalDive;

namespace AbyssalDive.UI {
    /// <summary>
    /// 奖励选择面板
    /// </summary>
    public class RewardPanel : UIPanel {
        [Header("Header")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;

        [Header("Reward Cards")]
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private GameObject rewardCardPrefab;
        [SerializeField] private Button[] rewardButtons;

        [Header("Bottom")]
        [SerializeField] private Button continueButton;

        private int _selectedRewardIndex = -1;

        protected override void OnShow() {
            _selectedRewardIndex = -1;
            UpdateRewardCards();
        }

        private void UpdateRewardCards() {
            if (titleText != null) {
                titleText.text = "深渊馈赠";
            }
            if (subtitleText != null) {
                subtitleText.text = $"第 {GameManager.Instance.CurrentFloor} 层通关";
            }

            // 清理旧卡牌
            foreach (Transform child in rewardContainer) {
                Destroy(child.gameObject);
            }

            // 生成奖励选项（示例：3个选项）
            string[] sampleRewards = { "攻击力 +5", "生命上限 +20", "新行动：追击" };
            Color[] rewardColors = {
                new Color(0.8f, 0.2f, 0.2f),
                new Color(0.2f, 0.8f, 0.2f),
                new Color(0.2f, 0.4f, 0.8f)
            };

            for (int i = 0; i < 3; i++) {
                var card = Instantiate(rewardCardPrefab, rewardContainer);
                var cardText = card.GetComponentInChildren<Text>();
                if (cardText != null) {
                    cardText.text = sampleRewards[i];
                }

                var image = card.GetComponent<Image>();
                if (image != null) {
                    image.color = rewardColors[i];
                }

                var button = card.GetComponent<Button>();
                int index = i;
                button.onClick.AddListener(() => OnRewardSelected(index));
            }
        }

        private void OnRewardSelected(int index) {
            _selectedRewardIndex = index;
            UIEventBus.Instance.Publish(UIEventType.RewardSelected, this, index);
        }

        private void OnContinue() {
            if (_selectedRewardIndex >= 0) {
                UIEventBus.Instance.Publish(UIEventType.RewardSelected, this, _selectedRewardIndex);
            }
        }
    }

    /// <summary>
    /// 游戏结算面板
    /// </summary>
    public class GameOverPanel : UIPanel {
        [Header("Header")]
        [SerializeField] private Text resultText;       // 胜利/失败
        [SerializeField] private Text floorReachedText; // 到达层数

        [Header("Stats")]
        [SerializeField] private Text totalDamageText;
        [SerializeField] private Text totalKillsText;
        [SerializeField] private Text totalTimeText;

        [Header("Buttons")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button retryButton;

        protected override void OnShow() {
            UpdateStats();
        }

        private void UpdateStats() {
            if (resultText != null) {
                // 根据游戏结果显示
                resultText.text = BattleSystem.Instance.lastBattleResult == BattleResult.Victory
                    ? "胜利" : "失败";
            }

            if (floorReachedText != null) {
                floorReachedText.text = $"到达第 {GameManager.Instance.CurrentFloor} 层";
            }

            // 后续从存档系统获取实际统计
            if (totalDamageText != null) {
                totalDamageText.text = "总伤害: 0";
            }
            if (totalKillsText != null) {
                totalKillsText.text = "击杀: 0";
            }
            if (totalTimeText != null) {
                totalTimeText.text = "用时: 00:00";
            }
        }

        private void Start() {
            mainMenuButton.onClick.AddListener(OnMainMenu);
            retryButton.onClick.AddListener(OnRetry);
        }

        private void OnMainMenu() {
            UIEventBus.Instance.Publish(UIEventType.ReturnToMainMenu, this);
        }

        private void OnRetry() {
            UIEventBus.Instance.Publish(UIEventType.StartGame, this);
        }
    }
}