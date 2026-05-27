using UnityEngine;
using UnityEngine.UI;

namespace AbyssalDive.UI {
    /// <summary>
    /// 主菜单面板
    /// </summary>
    public class MainMenuPanel : UIPanel {
        [Header("Menu Buttons")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Title")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;

        protected override void OnShow() {
            // 更新继续游戏按钮状态
            bool hasSave = SaveSystem.HasSaveData();
            continueButton.interactable = hasSave;
        }

        protected override void OnHide() {
            // 清理
        }

        private void Start() {
            // 绑定按钮事件
            startGameButton.onClick.AddListener(OnStartGame);
            continueButton.onClick.AddListener(OnContinueGame);
            settingsButton.onClick.AddListener(OnSettings);
            quitButton.onClick.AddListener(OnQuit);

            // 设置标题
            if (titleText != null) {
                titleText.text = "潜渊";
            }
            if (subtitleText != null) {
                subtitleText.text = "ABYSSAL DIVE";
            }
        }

        private void OnStartGame() {
            UIEventBus.Instance.Publish(UIEventType.StartGame, this);
        }

        private void OnContinueGame() {
            UIEventBus.Instance.Publish(UIEventType.ContinueGame, this);
        }

        private void OnSettings() {
            UIEventBus.Instance.Publish(UIEventType.OpenSettings, this);
        }

        private void OnQuit() {
            UIEventBus.Instance.Publish(UIEventType.QuitGame, this);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}