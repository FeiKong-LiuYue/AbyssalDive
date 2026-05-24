using System;
using System.Collections.Generic;

namespace AbyssalDive {
    /// <summary>
    /// 事件监听器接口
    /// </summary>
    public interface IEventListener {
        void OnEvent(GameEventType eventType, object eventData);
    }

    /// <summary>
    /// 事件总线 - 组件间通信中心
    /// </summary>
    public class EventBus {
        private static EventBus _instance;
        public static EventBus Instance => _instance ??= new EventBus();

        private readonly Dictionary<GameEventType, List<IEventListener>> _listeners = new();

        private EventBus() { }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(GameEventType type, IEventListener listener) {
            if (!_listeners.ContainsKey(type)) {
                _listeners[type] = new List<IEventListener>();
            }
            if (!_listeners[type].Contains(listener)) {
                _listeners[type].Add(listener);
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe(GameEventType type, IEventListener listener) {
            if (_listeners.ContainsKey(type)) {
                _listeners[type].Remove(listener);
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish<T>(GameEventType type, T eventData) where T : class {
            if (_listeners.ContainsKey(type)) {
                foreach (var listener in _listeners[type]) {
                    listener.OnEvent(type, eventData);
                }
            }
        }

        /// <summary>
        /// 清空所有订阅
        /// </summary>
        public void Clear() {
            _listeners.Clear();
        }
    }
}