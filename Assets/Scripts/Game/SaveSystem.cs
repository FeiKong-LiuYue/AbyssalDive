using System;
using System.IO;
using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 存档系统
    /// </summary>
    public class SaveSystem {

        private readonly string _savePath;

        public SaveSystem() {
            _savePath = Application.persistentDataPath + "/save.json";
        }

        /// <summary>
        /// 保存游戏
        /// </summary>
        public void Save(GameManager gameManager) {
            var saveData = new SaveData {
                currentFloor = gameManager.CurrentFloor,
                currentStage = gameManager.CurrentStage,
                playerAttack = gameManager.player.attack,
                playerMaxHealth = gameManager.player.maxHealth,
                playerCurrentHealth = gameManager.player.currentHealth,
                playerDefense = gameManager.player.defense,
                playerSpeed = gameManager.player.speed,
                playerBonusAttack = gameManager.player.bonusAttack,
                playerBonusHealth = gameManager.player.bonusHealth,
                playerBonusSpeed = gameManager.player.bonusSpeed,
                // TODO: 保存行动栏、特性等
            };

            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(_savePath, json);
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        public void Load(GameManager gameManager) {
            if (!File.Exists(_savePath)) {
                Debug.LogWarning("Save file not found");
                return;
            }

            string json = File.ReadAllText(_savePath);
            var saveData = JsonUtility.FromJson<SaveData>(json);

            // TODO: 恢复到gameManager
        }

        /// <summary>
        /// 检查存档是否存在（静态方法）
        /// </summary>
        public static bool HasSaveData() {
            string path = Application.persistentDataPath + "/save.json";
            return File.Exists(path);
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeleteSave() {
            if (File.Exists(_savePath)) {
                File.Delete(_savePath);
            }
        }
    }

    [Serializable]
    public class SaveData {
        public int currentFloor;
        public int currentStage;
        public float playerAttack;
        public float playerMaxHealth;
        public float playerCurrentHealth;
        public float playerDefense;
        public float playerSpeed;
        public float playerBonusAttack;
        public float playerBonusHealth;
        public float playerBonusSpeed;
    }
}