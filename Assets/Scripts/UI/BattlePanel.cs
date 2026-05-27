using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbyssalDive;

namespace AbyssalDive.UI {
    /// <summary>
    /// 战斗界面面板
    /// </summary>
    public class BattlePanel : UIPanel {
        [Header("Top Bar")]
        [SerializeField] private Text battleNameText;
        [SerializeField] private Text turnText;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button retreatButton;

        [Header("ATB Progress")]
        [SerializeField] private Transform atbContainer;
        [SerializeField] private GameObject atbBarPrefab;

        [Header("Battle Field")]
        [SerializeField] private Transform enemyField;
        [SerializeField] private Transform playerField;
        [SerializeField] private Text vsText;

        [Header("Bottom Status")]
        [SerializeField] private Text comboText;
        [SerializeField] private Text chargeText;
        [SerializeField] private Text healthText;
        [SerializeField] private Text shieldText;
        [SerializeField] private Transform stateIconsContainer;

        [Header("Action Bar")]
        [SerializeField] private Transform actionBarContainer;
        [SerializeField] private Text[] actionNameTexts;
        [SerializeField] private Text[] actionLevelTexts;

        private List<GameObject> _atbBars = new();
        private List<GameObject> _enemyCards = new();
        private List<GameObject> _playerCards = new();

        private void Start() {
            menuButton.onClick.AddListener(OnMenu);
            retreatButton.onClick.AddListener(OnRetreat);
        }

        protected override void OnShow() {
            RefreshBattle();
        }

        public void RefreshBattle() {
            UpdateTopBar();
            CreateATBBars();
            CreateEnemyField();
            CreatePlayerField();
            UpdateBottomStatus();
            UpdateActionBar();
        }

        private void UpdateTopBar() {
            if (turnText != null) {
                turnText.text = $"回合 {BattleSystem.Instance.currentTurn}";
            }
            if (battleNameText != null) {
                battleNameText.text = $"第 {GameManager.Instance.CurrentFloor} 层";
            }
        }

        private void CreateATBBars() {
            // 清理旧数据
            foreach (var bar in _atbBars) {
                Destroy(bar);
            }
            _atbBars.Clear();

            var units = BattleSystem.Instance.GetAllUnits();
            foreach (var unit in units) {
                var bar = Instantiate(atbBarPrefab, atbContainer);
                _atbBars.Add(bar);
                // 后续绑定 ATB 值变化
            }
        }

        private void CreateEnemyField() {
            // 清理旧数据
            foreach (var card in _enemyCards) {
                Destroy(card);
            }
            _enemyCards.Clear();

            var enemies = BattleSystem.Instance.GetEnemies();
            foreach (var enemy in enemies) {
                var card = CreateUnitCard(enemy, enemyField);
                _enemyCards.Add(card);
            }
        }

        private void CreatePlayerField() {
            // 清理旧数据
            foreach (var card in _playerCards) {
                Destroy(card);
            }
            _playerCards.Clear();

            var player = BattleSystem.Instance.GetPlayer();
            if (player != null) {
                var card = CreateUnitCard(player, playerField);
                _playerCards.Add(card);
            }
        }

        private GameObject CreateUnitCard(Unit unit, Transform parent) {
            var card = new GameObject("UnitCard");
            card.transform.SetParent(parent);

            // 后续替换为实际预制体
            var image = card.AddComponent<Image>();
            image.color = unit is EnemyUnit ? new Color(0.5f, 0.2f, 0.5f) : new Color(0.2f, 0.5f, 0.5f);

            var rect = card.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 120);

            return card;
        }

        private void UpdateBottomStatus() {
            var player = BattleSystem.Instance.GetPlayer();
            if (player == null) return;

            // 更新连击
            if (comboText != null) {
                int combo = player.stateComponent.GetStateStacks("连击");
                comboText.text = combo > 0 ? $"连击 {combo}" : "";
            }

            // 更新蓄力
            if (chargeText != null) {
                int charge = player.stateComponent.GetStateStacks("蓄力");
                chargeText.text = charge > 0 ? $"蓄力 {charge}" : "";
            }

            // 更新血量
            if (healthText != null) {
                healthText.text = $"{player.currentHealth}/{player.maxHealth}";
            }

            // 更新护盾
            if (shieldText != null) {
                shieldText.text = $"护盾 {player.shieldComponent.currentShield}";
            }
        }

        private void UpdateActionBar() {
            var player = GameManager.Instance.player;
            if (player == null) return;

            for (int i = 0; i < actionNameTexts.Length; i++) {
                if (i < player.actionBar.Count) {
                    actionNameTexts[i].text = player.actionBar[i].name;
                    if (actionLevelTexts[i] != null) {
                        actionLevelTexts[i].text = $"Lv.{player.actionBar[i].level}";
                    }
                } else {
                    actionNameTexts[i].text = "-";
                    if (actionLevelTexts[i] != null) {
                        actionLevelTexts[i].text = "";
                    }
                }
            }
        }

        public void UpdateATBDisplay() {
            // 更新所有 ATB 条
            var units = BattleSystem.Instance.GetAllUnits();
            for (int i = 0; i < _atbBars.Count && i < units.Length; i++) {
                var bar = _atbBars[i];
                var unit = units[i];
                // 计算填充比例
                float fillPercent = unit.actionValue / unit.actionThreshold;
                // 后续更新进度条
            }
        }

        public void ShowDamageNumber(Vector3 position, float damage, bool isPlayer) {
            // 后续实现伤害数字显示
        }

        private void OnMenu() {
            // 打开暂停菜单
        }

        private void OnRetreat() {
            // 撤退确认
        }

        public override void UpdatePanel() {
            UpdateATBDisplay();
            UpdateBottomStatus();
        }
    }
}