using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 护盾组件
    /// </summary>
    public class ShieldComponent {
        private Unit _owner;

        public float currentShield;

        public ShieldComponent(Unit owner) {
            _owner = owner;
        }

        /// <summary>
        /// 添加护盾
        /// </summary>
        public void Add(float amount) {
            currentShield += amount;
        }

        /// <summary>
        /// 减少护盾
        /// </summary>
        public void Reduce(float amount) {
            float oldShield = currentShield;
            currentShield = Mathf.Max(0, currentShield - amount);

            // 触发碎盾事件
            if (oldShield > currentShield && currentShield > 0) {
                EventBus.Instance.Publish(GameEventType.ShieldDamaged, new ShieldEventData {
                    unit = _owner,
                    shieldBefore = oldShield,
                    shieldAfter = currentShield
                });
            }

            // 触发破盾事件
            if (oldShield > 0 && currentShield == 0) {
                EventBus.Instance.Publish(GameEventType.ShieldBroken, new ShieldEventData {
                    unit = _owner,
                    shieldBefore = oldShield,
                    shieldAfter = 0
                });
            }
        }

        /// <summary>
        /// 护盾减半（回合结束）
        /// </summary>
        public void Halve() {
            if (currentShield > 0) {
                float oldShield = currentShield;
                currentShield = Mathf.Floor(currentShield / 2f);

                if (currentShield > 0) {
                    EventBus.Instance.Publish(GameEventType.ShieldDamaged, new ShieldEventData {
                        unit = _owner,
                        shieldBefore = oldShield,
                        shieldAfter = currentShield
                    });
                }
            }
        }

        /// <summary>
        /// 清空护盾
        /// </summary>
        public void Clear() {
            currentShield = 0;
        }
    }

    /// <summary>
    /// 护盾事件数据
    /// </summary>
    public class ShieldEventData : GameEventData {
        public Unit unit;
        public float shieldBefore;
        public float shieldAfter;
    }
}