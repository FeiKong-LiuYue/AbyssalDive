using UnityEngine;
using UnityEngine.UI;
using AbyssalDive;
using AbyssalDive.UI;

namespace AbyssalDive {
    /// <summary>
    /// UI 自动 setup 组件 - 运行时自动生成所有 UI 元素
    /// 放置在场景中空的 GameObject 上即可
    /// </summary>
    public class UISetup : MonoBehaviour {
        [Header("Screen Settings")]
        public Vector2 referenceResolution = new Vector2(1920, 1080);
        public Color backgroundColor = new Color(0.07f, 0.06f, 0.08f);

        [Header("Color Palette")]
        public Color playerColor = new Color(0.77f, 0.13f, 0.28f);    // #C42048
        public Color energyColor = new Color(0.13f, 0.56f, 0.77f);   // #2090C4
        public Color dangerColor = new Color(0.44f, 0.13f, 0.77f);    // #7020C4
        public Color goldColor = new Color(0.72f, 0.60f, 0.42f);      // #B89A6A
        public Color healthColor = new Color(0.88f, 0.25f, 0.25f);   // #E04040
        public Color shieldColor = new Color(0.25f, 0.63f, 0.88f);    // #40A0E0

        // 面板引用
        private Canvas _canvas;
        private EventSystem _eventSystem;
        private MainMenuPanel _mainMenuPanel;
        private PreparationPanel _preparationPanel;
        private BattlePanel _battlePanel;
        private RewardPanel _rewardPanel;
        private GameOverPanel _gameOverPanel;

        private void Start() {
            SetupCanvas();
            SetupEventSystem();
            SetupMainMenuPanel();
            SetupPreparationPanel();
            SetupBattlePanel();
            SetupRewardPanel();
            SetupGameOverPanel();

            // 默认显示主菜单
            UIManager.Instance.ShowPanel(PanelType.MainMenu);
        }

        private void SetupCanvas() {
            // 创建 Canvas
            var canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(transform);
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.scaleFactor = 1f;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // 设置背景
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = backgroundColor;
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
        }

        private void SetupEventSystem() {
            var esObj = new GameObject("EventSystem");
            esObj.transform.SetParent(transform);
            _eventSystem = esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        #region 主菜单面板

        private void SetupMainMenuPanel() {
            var panelObj = CreatePanel("MainMenuPanel", out var panelRect);
            _mainMenuPanel = panelObj.AddComponent<MainMenuPanel>();

            // 标题
            CreateText("Title", panelObj.transform, "潜渊",
                new Vector2(960, 700), new Vector2(400, 80),
                72, goldColor, true);

            CreateText("Subtitle", panelObj.transform, "ABYSSAL DIVE",
                new Vector2(960, 620), new Vector2(400, 40),
                24, goldColor, true);

            // 按钮
            float buttonWidth = 300;
            float buttonHeight = 60;
            float startY = 450;
            float spacing = 80;

            var startBtn = CreateButton("StartGameButton", panelObj.transform,
                "开始下潜", new Vector2(960, startY), new Vector2(buttonWidth, buttonHeight), playerColor);

            var continueBtn = CreateButton("ContinueButton", panelObj.transform,
                "继续游戏", new Vector2(960, startY - spacing), new Vector2(buttonWidth, buttonHeight), energyColor);

            var settingsBtn = CreateButton("SettingsButton", panelObj.transform,
                "设置", new Vector2(960, startY - spacing * 2), new Vector2(buttonWidth, buttonHeight), goldColor);

            var quitBtn = CreateButton("QuitButton", panelObj.transform,
                "退出", new Vector2(960, startY - spacing * 3), new Vector2(buttonWidth, buttonHeight), dangerColor);

            // 绑定按钮（通过查找组件）
            StartCoroutine(BindMainMenuButtons());
        }

        private System.Collections.IEnumerator BindMainMenuButtons() {
            yield return null; // 等待下一帧

            var startBtn = GameObject.Find("StartGameButton")?.GetComponent<Button>();
            var continueBtn = GameObject.Find("ContinueButton")?.GetComponent<Button>();
            var settingsBtn = GameObject.Find("SettingsButton")?.GetComponent<Button>();
            var quitBtn = GameObject.Find("QuitButton")?.GetComponent<Button>();

            if (_mainMenuPanel != null) {
                // 反射设置私有字段
                SetField(_mainMenuPanel, "startGameButton", startBtn);
                SetField(_mainMenuPanel, "continueButton", continueBtn);
                SetField(_mainMenuPanel, "settingsButton", settingsBtn);
                SetField(_mainMenuPanel, "quitButton", quitBtn);
            }
        }

        #endregion

        #region 战备界面

        private void SetupPreparationPanel() {
            var panelObj = CreatePanel("PreparationPanel", out var panelRect);
            _preparationPanel = panelObj.AddComponent<PreparationPanel>();
            panelObj.SetActive(false);

            // 左侧 - 敌人预览 (40%)
            CreateText("EnemyTitle", panelObj.transform, "敌人预览",
                new Vector2(350, 900), new Vector2(200, 40), 28, dangerColor, true);

            var enemyContainer = CreateContainer("EnemyList", panelObj.transform,
                new Vector2(350, 700), new Vector2(300, 400));

            // 右侧 - 玩家信息 (60%)
            CreateText("PlayerStatsTitle", panelObj.transform, "玩家状态",
                new Vector2(1100, 900), new Vector2(200, 40), 28, playerColor, true);

            // 玩家状态面板
            var playerPanel = CreateContainer("PlayerPanel", panelObj.transform,
                new Vector2(1100, 800), new Vector2(500, 150));
            CreateText("HealthText", playerPanel.transform, "❤️ 100/100",
                new Vector2(-150, 20), new Vector2(200, 30), 20, healthColor, false);
            CreateText("ShieldText", playerPanel.transform, "🛡 0",
                new Vector2(50, 20), new Vector2(200, 30), 20, shieldColor, false);

            // 行动栏
            CreateText("ActionBarTitle", panelObj.transform, "行动栏",
                new Vector2(1100, 600), new Vector2(200, 40), 28, goldColor, true);

            var actionBarContainer = CreateContainer("ActionBarContainer", panelObj.transform,
                new Vector2(1100, 520), new Vector2(600, 80));

            // 5个行动槽
            for (int i = 0; i < 5; i++) {
                var slot = CreateButton($"ActionSlot_{i}", actionBarContainer.transform,
                    "-", new Vector2(-240 + i * 120, 0), new Vector2(100, 70), new Color(0.2f, 0.18f, 0.22f));
            }

            // 行动池
            CreateText("ActionPoolTitle", panelObj.transform, "行动池",
                new Vector2(1100, 380), new Vector2(200, 40), 28, goldColor, true);

            var actionPoolContainer = CreateContainer("ActionPoolContainer", panelObj.transform,
                new Vector2(1100, 200), new Vector2(700, 200));

            // 示例行动
            string[] sampleActions = { "连打", "爆发", "蓄力", "举盾", "戮血", "追击" };
            for (int i = 0; i < sampleActions.Length; i++) {
                int row = i / 3;
                int col = i % 3;
                CreateButton($"ActionPool_{i}", actionPoolContainer.transform,
                    sampleActions[i], new Vector2(-200 + col * 150, 50 - row * 60), new Vector2(120, 50), energyColor);
            }

            // 开始下潜按钮
            CreateButton("StartBattleButton", panelObj.transform,
                "开始下潜", new Vector2(960, 60), new Vector2(300, 50), playerColor);

            // 层数信息
            CreateText("FloorText", panelObj.transform, "第 1 层",
                new Vector2(960, 1000), new Vector2(200, 40), 24, Color.white, true);
        }

        #endregion

        #region 战斗界面

        private void SetupBattlePanel() {
            var panelObj = CreatePanel("BattlePanel", out var panelRect);
            _battlePanel = panelObj.AddComponent<BattlePanel>();
            panelObj.SetActive(false);

            // 顶部栏
            CreateText("BattleName", panelObj.transform, "第 1 层",
                new Vector2(200, 1000), new Vector2(200, 40), 24, Color.white, true);
            CreateText("TurnText", panelObj.transform, "回合 1",
                new Vector2(960, 1000), new Vector2(200, 40), 28, goldColor, true);

            CreateButton("MenuButton", panelObj.transform, "菜单",
                new Vector2(1700, 1000), new Vector2(120, 40), goldColor);
            CreateButton("RetreatButton", panelObj.transform, "撤退",
                new Vector2(1550, 1000), new Vector2(120, 40), dangerColor);

            // ATB 进度条区域
            var atbContainer = CreateContainer("ATBContainer", panelObj.transform,
                new Vector2(960, 920), new Vector2(1800, 50));

            // ATB 条示例
            for (int i = 0; i < 4; i++) {
                var atbBar = CreateContainer($"ATBBar_{i}", atbContainer.transform,
                    new Vector2(-650 + i * 200, 0), new Vector2(150, 40));
                var barImage = atbBar.AddComponent<Image>();
                barImage.color = new Color(0.2f, 0.2f, 0.25f);
            }

            // 战场区域
            var enemyField = CreateContainer("EnemyField", panelObj.transform,
                new Vector2(400, 600), new Vector2(400, 400));

            CreateText("VSText", panelObj.transform, "VS",
                new Vector2(960, 600), new Vector2(100, 50), 48, goldColor, true);

            var playerField = CreateContainer("PlayerField", panelObj.transform,
                new Vector2(1520, 600), new Vector2(400, 400));

            // 底部状态
            CreateText("ComboText", panelObj.transform, "",
                new Vector2(300, 350), new Vector2(150, 40), 24, playerColor, true);
            CreateText("ChargeText", panelObj.transform, "",
                new Vector2(500, 350), new Vector2(150, 40), 24, goldColor, true);

            // 行动栏
            var actionBarContainer = CreateContainer("BattleActionBar", panelObj.transform,
                new Vector2(960, 200), new Vector2(900, 80));

            for (int i = 0; i < 5; i++) {
                var btn = CreateButton($"BattleAction_{i}", actionBarContainer.transform,
                    "-", new Vector2(-400 + i * 200, 0), new Vector2(150, 70), new Color(0.2f, 0.18f, 0.22f));
            }
        }

        #endregion

        #region 奖励面板

        private void SetupRewardPanel() {
            var panelObj = CreatePanel("RewardPanel", out var panelRect);
            _rewardPanel = panelObj.AddComponent<RewardPanel>();
            panelObj.SetActive(false);

            CreateText("RewardTitle", panelObj.transform, "深渊馈赠",
                new Vector2(960, 850), new Vector2(300, 60), 48, goldColor, true);
            CreateText("RewardSubtitle", panelObj.transform, "第 1 层通关",
                new Vector2(960, 780), new Vector2(300, 40), 24, Color.white, true);

            var rewardContainer = CreateContainer("RewardContainer", panelObj.transform,
                new Vector2(960, 550), new Vector2(900, 200));

            // 3个奖励选项
            string[] rewards = { "攻击力 +5", "生命上限 +20", "新行动" };
            Color[] rewardColors = { playerColor, shieldColor, energyColor };

            for (int i = 0; i < 3; i++) {
                var card = CreateButton($"Reward_{i}", rewardContainer.transform,
                    rewards[i], new Vector2(-300 + i * 300, 0), new Vector2(200, 150), rewardColors[i]);
            }

            CreateButton("ContinueButton", panelObj.transform,
                "继续下潜", new Vector2(960, 150), new Vector2(250, 50), playerColor);
        }

        #endregion

        #region 结算面板

        private void SetupGameOverPanel() {
            var panelObj = CreatePanel("GameOverPanel", out var panelRect);
            _gameOverPanel = panelObj.AddComponent<GameOverPanel>();
            panelObj.SetActive(false);

            CreateText("ResultText", panelObj.transform, "胜利",
                new Vector2(960, 800), new Vector2(300, 80), 64, goldColor, true);
            CreateText("FloorReachedText", panelObj.transform, "到达第 1 层",
                new Vector2(960, 700), new Vector2(300, 50), 32, Color.white, true);

            CreateText("StatsText", panelObj.transform,
                "总伤害: 0\n击杀: 0\n用时: 00:00",
                new Vector2(960, 550), new Vector2(300, 150), 24, Color.white, true);

            CreateButton("MainMenuButton", panelObj.transform,
                "返回主菜单", new Vector2(800, 250), new Vector2(200, 50), energyColor);
            CreateButton("RetryButton", panelObj.transform,
                "重试", new Vector2(1120, 250), new Vector2(200, 50), playerColor);
        }

        #endregion

        #region 辅助方法

        private GameObject CreatePanel(string name, out RectTransform rect) {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(_canvas.transform);
            rect = panelObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            panelObj.AddComponent<CanvasGroup>();
            return panelObj;
        }

        private GameObject CreateContainer(string name, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta) {
            var container = new GameObject(name);
            container.transform.SetParent(parent);
            var rect = container.AddComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            return container;
        }

        private Text CreateText(string name, Transform parent, string content,
            Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, Color color, bool centered) {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            var rect = textObj.AddComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            rect.anchorMin = centered ? new Vector2(0.5f, 0.5f) : new Vector2(0, 0.5f);
            rect.anchorMax = centered ? new Vector2(0.5f, 0.5f) : new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            var text = textObj.AddComponent<Text>();
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = centered ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return text;
        }

        private Button CreateButton(string name, Transform parent, string label,
            Vector2 anchoredPosition, Vector2 sizeDelta, Color color) {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            var image = btnObj.AddComponent<Image>();
            image.color = color;

            var button = btnObj.AddComponent<Button>();
            button.targetGraphic = image;

            // 添加文字
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.pivot = new Vector2(0.5f, 0.5f);

            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            return button;
        }

        private void SetField(object obj, string fieldName, object value) {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) {
                field.SetValue(obj, value);
            }
        }

        #endregion
    }
}