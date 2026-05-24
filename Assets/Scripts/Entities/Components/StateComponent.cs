using System.Collections.Generic;
using System.Linq;

namespace AbyssalDive {
    /// <summary>
    /// 状态组件 - 管理单位身上的状态
    /// </summary>
    public class StateComponent {
        private Unit _owner;
        private Dictionary<string, int> _states = new();

        public StateComponent(Unit owner) {
            _owner = owner;
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        public void AddState(StateData stateData, int layers = 1) {
            // 检查最大层数限制
            int currentStacks = _states.ContainsKey(stateData.id) ? _states[stateData.id] : 0;
            int newStacks = Mathf.Min(currentStacks + layers, stateData.maxLayers);

            _states[stateData.id] = newStacks;

            EventBus.Instance.Publish(GameEventType.StateAdded, new StateEventData {
                target = _owner,
                state = stateData,
                stackChange = newStacks - currentStacks
            });
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        public void RemoveState(string stateId) {
            if (_states.ContainsKey(stateId)) {
                int layers = _states[stateId];
                _states.Remove(stateId);

                StateData stateData = StateConfig.Get(stateId);
                EventBus.Instance.Publish(GameEventType.StateRemoved, new StateEventData {
                    target = _owner,
                    state = stateData,
                    stackChange = -layers
                });
            }
        }

        /// <summary>
        /// 获取状态层数
        /// </summary>
        public int GetStateStacks(string stateId) {
            return _states.ContainsKey(stateId) ? _states[stateId] : 0;
        }

        /// <summary>
        /// 检查是否有某状态
        /// </summary>
        public bool HasState(string stateId) {
            return _states.ContainsKey(stateId) && _states[stateId] > 0;
        }

        /// <summary>
        /// 处理回合开始
        /// </summary>
        public void ProcessTurnStart() {
            // 回合开始时，蓄势+蓄力，战意+1层
            StateResolver.ProcessTurnStart(_owner);
        }

        /// <summary>
        /// 处理回合结束
        /// </summary>
        public void ProcessTurnEnd() {
            // 处理状态衰减、护盾减半等
            StateResolver.ProcessTurnEnd(_owner);
        }

        /// <summary>
        /// 清空所有状态
        /// </summary>
        public void Clear() {
            _states.Clear();
        }

        /// <summary>
        /// 获取所有状态（用于调试）
        /// </summary>
        public Dictionary<string, int> GetAllStates() {
            return new Dictionary<string, int>(_states);
        }
    }
}