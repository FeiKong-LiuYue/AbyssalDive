using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 触发系统 - 监听战斗事件并触发特性效果
    /// </summary>
    public class TriggerSystem : IEventListener {
        private static TriggerSystem _instance;
        public static TriggerSystem Instance => _instance ??= new TriggerSystem();

        private List<TraitData> _registeredTraits = new();

        private TriggerSystem() {
            // 订阅所有事件
            foreach (GameEventType type in System.Enum.GetValues(typeof(GameEventType))) {
                EventBus.Instance.Subscribe(type, this);
            }
        }

        /// <summary>
        /// 注册特性
        /// </summary>
        public void RegisterTrait(TraitData trait) {
            if (!_registeredTraits.Contains(trait)) {
                _registeredTraits.Add(trait);
            }
        }

        /// <summary>
        /// 注销特性
        /// </summary>
        public void UnregisterTrait(TraitData trait) {
            _registeredTraits.Remove(trait);
        }

        public void OnEvent(GameEventType eventType, object eventData) {
            // 遍历所有注册的特性，检查触发条件
            foreach (var trait in _registeredTraits) {
                if (ShouldTrigger(trait, eventType, eventData)) {
                    TriggerEffect(trait, eventData);
                }
            }
        }

        /// <summary>
        /// 检查是否应该触发
        /// </summary>
        private bool ShouldTrigger(TraitData trait, GameEventType eventType, object eventData) {
            if (trait.triggerEvents == null) return false;

            foreach (var triggerEvent in trait.triggerEvents) {
                if (triggerEvent == eventType) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 触发特性效果
        /// </summary>
        private void TriggerEffect(TraitData trait, object eventData) {
            Unit attacker = null;
            Unit target = null;

            // 从事件数据提取attacker和target
            if (eventData is AttackEventData attackData) {
                attacker = attackData.attacker;
                target = attackData.target;
            } else if (eventData is StateEventData stateData) {
                target = stateData.target;
            }

            if (trait.effect != null && attacker != null && target != null) {
                trait.effect(attacker, target);
            }
        }

        /// <summary>
        /// 清空所有特性
        /// </summary>
        public void Clear() {
            _registeredTraits.Clear();
        }
    }
}