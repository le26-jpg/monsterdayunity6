using Monsterday.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Monsterday.Combat
{
    public sealed class MonsterCombatant : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int HitHash = Animator.StringToHash("Hit");
        private static readonly int KnockoutHash = Animator.StringToHash("Knockout");

        [SerializeField] private MonsterDefinition definition;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private UnityEvent<float> healthChanged;
        [SerializeField] private UnityEvent knockedOut;
        private MonsterCombatant target;
        private int currentHealth;

        public UnityEvent KnockedOut => knockedOut;
        private float nextAttackTime;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => definition?.BaseStats.health ?? 0;
        public bool IsAlive => currentHealth > 0;
        public MonsterDefinition Definition => definition;

        private void Awake()
        {
            if (definition != null) currentHealth = definition.BaseStats.health;
        }

        private void Update()
        {
            if (!IsAlive || definition == null) return;

            if (target == null && !gameObject.CompareTag("Player"))
            {
                var player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    var playerCombatant = player.GetComponent<MonsterCombatant>();
                    if (playerCombatant != null && playerCombatant.IsAlive)
                        target = playerCombatant;
                }
            }

            if (target == null || !target.IsAlive) return;
            var offset = target.transform.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude > 2.25f)
            {
                var velocity = offset.normalized * definition.BaseStats.moveSpeed;
                transform.position += velocity * Time.deltaTime;
                if (visualRoot != null && velocity.sqrMagnitude > 0.01f)
                    visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 12f);
                if (animator != null) animator.SetFloat(SpeedHash, 1f, 0.1f, Time.deltaTime);
            }
            else
            {
                if (animator != null) animator.SetFloat(SpeedHash, 0f, 0.1f, Time.deltaTime);
                TryAttack();
            }
        }

        public void SetTarget(MonsterCombatant newTarget) => target = newTarget;

        public void ReceiveDamage(int rawDamage)
        {
            if (!IsAlive || definition == null) return;
            var damage = Mathf.Max(1, rawDamage - Mathf.RoundToInt(definition.BaseStats.defense * 0.5f));
            currentHealth = Mathf.Max(0, currentHealth - damage);
            healthChanged?.Invoke((float)currentHealth / definition.BaseStats.health);
            if (animator != null) animator.SetTrigger(currentHealth > 0 ? HitHash : KnockoutHash);
            if (currentHealth == 0) knockedOut?.Invoke();
        }

        private void TryAttack()
        {
            if (Time.time < nextAttackTime || target == null) return;
            nextAttackTime = Time.time + 1f / Mathf.Max(0.1f, definition.BaseStats.attackSpeed);
            if (animator != null) animator.SetTrigger(AttackHash);
            target.ReceiveDamage(definition.BaseStats.attack);
        }

#if UNITY_EDITOR
        public void EditorConfigure(MonsterDefinition monster, Animator monsterAnimator, Transform visuals)
        {
            definition = monster;
            animator = monsterAnimator;
            visualRoot = visuals;
        }
#endif
    }
}
