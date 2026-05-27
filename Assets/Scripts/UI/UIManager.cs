using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// UI 事件类型
    /// </summary>
    public enum UIEventType {
        // 菜单事件
        StartGame,
        ContinueGame,
        OpenSettings,
        QuitGame,

        // 战备事件
        ActionBarChanged,
        StartBattle,

        // 战斗事件
        BattleStarted,
        BattleEnded,

        // 奖励事件
        RewardSelected,
        ContinueNextFloor,

        // 结算事件
        ReturnToMainMenu
    }

    /// <summary>
    /// UI 事件数据
    /// </summary>
    public class UIEventData {
        public UIEventType EventType { get; set; }
        public object Sender { get; set; }
        public object Data { get; set; }

        public UIEventData(UIEventType eventType, object sender = null, object data = null) {
            EventType = eventType;
            Sender = sender;
            Data = data;
        }
    }

    /// <summary>
    /// UI 事件监听器接口
    /// </summary>
    public interface IUIEventListener {
        void OnUIEvent(UIEventType eventType, UIEventData data);
    }

    /// <summary>
    /// UI 事件总线
    /// </summary>
    public class UIEventBus {
        private static UIEventBus _instance;
        public static UIEventBus Instance => _instance ??= new UIEventBus();

        private readonly Dictionary<UIEventType, List<IUIEventListener>> _listeners = new();

        private UIEventBus() { }

        public void Subscribe(UIEventType eventType, IUIEventListener listener) {
            if (!_listeners.ContainsKey(eventType))
                _listeners[eventType] = new List<IUIEventListener>();
            _listeners[eventType].Add(listener);
        }

        public void Unsubscribe(UIEventType eventType, IUIEventListener listener) {
            if (_listeners.ContainsKey(eventType))
                _listeners[eventType].Remove(listener);
        }

        public void Publish(UIEventType eventType, object sender = null, object data = null) {
            if (_listeners.TryGetValue(eventType, out var list)) {
                var eventData = new UIEventData(eventType, sender, data);
                foreach (var listener in list) {
                    listener.OnUIEvent(eventType, eventData);
                }
            }
        }
    }

    /// <summary>
    /// 面板类型
    /// </summary>
    public enum PanelType {
        None,
        MainMenu,
        Preparation,
        Battle,
        Reward,
        GameOver
    }

    /// <summary>
    /// UI 管理器 - 管理所有面板的显示和切换
    /// </summary>
    public class UIManager : MonoBehaviour, IUIEventListener {
        private static UIManager _instance;
        public static UIManager Instance => _instance;

        [Header("Panel References")]
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private PreparationPanel preparationPanel;
        [SerializeField] private BattlePanel battlePanel;
        [SerializeField] private RewardPanel rewardPanel;
        [SerializeField] private GameOverPanel gameOverPanel;

        [Header("Canvas Settings")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private float screenWidth = 1920f;
        [SerializeField] private float screenHeight = 1080f;

        private PanelType _currentPanel = PanelType.None;

        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            // 订阅 UI 事件
            UIEventBus.Instance.Subscribe(UIEventType.StartGame, this);
            UIEventBus.Instance.Subscribe(UIEventType.ContinueGame, this);
            UIEventBus.Instance.Subscribe(UIEventType.StartBattle, this);
            UIEventBus.Instance.Subscribe(UIEventType.RewardSelected, this);
            UIEventBus.Instance.Subscribe(UIEventType.ContinueNextFloor, this);
            UIEventBus.Instance.Subscribe(UIEventType.ReturnToMainMenu, this);

            // 默认显示主菜单
            ShowPanel(PanelType.MainMenu);
        }

        private void OnDestroy() {
            UIEventBus.Instance.Unsubscribe(UIEventType.StartGame, this);
            UIEventBus.Instance.Unsubscribe(UIEventType.ContinueGame, this);
            UIEventBus.Instance.Unsubscribe(UIEventType.StartBattle, this);
            UIEventBus.Instance.Unsubscribe(UIEventType.RewardSelected, this);
            UIEventBus.Instance.Unsubscribe(UIEventType.ContinueNextFloor, this);
            UIEventBus.Instance.Unsubscribe(UIEventType.ReturnToMainMenu, this);
        }

        /// <summary>
        /// 显示指定面板
        /// </summary>
        public void ShowPanel(PanelType panelType) {
            if (_currentPanel == panelType) return;

            // 隐藏当前面板
            HideCurrentPanel();

            // 显示新面板
            _currentPanel = panelType;
            GetPanel(panelType)?.Show();
        }

        private void HideCurrentPanel() {
            var current = GetPanel(_currentPanel);
            if (current != null) {
                current.Hide();
            }
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        public UIPanel GetPanel(PanelType panelType) {
            return panelType switch {
                PanelType.MainMenu => mainMenuPanel,
                PanelType.Preparation => preparationPanel,
                PanelType.Battle => battlePanel,
                PanelType.Reward => rewardPanel,
                PanelType.GameOver => gameOverPanel,
                _ => null
            };
        }

        /// <summary>
        /// 获取当前面板类型
        /// </summary>
        public PanelType CurrentPanel => _currentPanel;

        public void OnUIEvent(UIEventType eventType, UIEventData data) {
            switch (eventType) {
                case UIEventType.StartGame:
                    GameManager.Instance.StartNewGame();
                    ShowPanel(PanelType.Preparation);
                    break;

                case UIEventType.ContinueGame:
                    GameManager.Instance.ContinueGame();
                    ShowPanel(PanelType.Preparation);
                    break;

                case UIEventType.StartBattle:
                    ShowPanel(PanelType.Battle);
                    break;

                case UIEventType.RewardSelected:
                    ShowPanel(PanelType.Preparation);
                    break;

                case UIEventType.ContinueNextFloor:
                    GameManager.Instance.NextFloor();
                    ShowPanel(PanelType.Preparation);
                    break;

                case UIEventType.ReturnToMainMenu:
                    ShowPanel(PanelType.MainMenu);
                    break;
            }
        }

        /// <summary>
        /// 更新当前面板
        /// </summary>
        private void Update() {
            var current = GetPanel(_currentPanel);
            current?.UpdatePanel();
        }
    }
}