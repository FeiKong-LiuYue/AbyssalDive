using UnityEngine;

namespace AbyssalDive {
    /// <summary>
    /// 生命值组件
    /// </summary>
    public class HealthComponent {
        private Unit _owner;

        public HealthComponent(Unit owner) {
            _owner = owner;
        }

        /// <summary>
        /// 造成伤害
        /// </summary>
        public void TakeDamage(float damage, Unit attacker = null, bool ignoreShield = false) {
            if (_owner.shieldComponent.currentShield > 0 && !ignoreShield) {
                // 先扣护盾
                float shieldDamage = Mathf.Min(damage, _owner.shieldComponent.currentShield);
                _owner.shieldComponent.Reduce(shieldDamage);
                damage -= shieldDamage;

                if (damage > 0) {
                    _owner.currentHealth -= damage;
                }
            } else {
                _owner.currentHealth -= damage;
            }

            // 触发被攻击事件
            EventBus.Instance.Publish(GameEventType.Damaged, new AttackEventData {
                attacker = attacker,
                target = _owner,
                damage = damage,
                isHit = true
            });

            // 检查死亡
            if (_owner.currentHealth <= 0) {
                _owner.currentHealth = 0;
                EventBus.Instance.Publish(GameEventType.Death, new DeathEventData {
                    deadUnit = _owner,
                    killer = attacker
                });
            }
        }

        /// <summary>
        /// 治疗
        /// </summary>
        public void Heal(float amount) {
            _owner.currentHealth = Mathf.Min(_owner.currentHealth + amount, _owner.maxHealth);
        }
    }
}