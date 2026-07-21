using Monsterday.Core;
using Monsterday.Gameplay;
using Monsterday.Combat;
using UnityEngine;

namespace Monsterday.Gameplay
{
    [RequireComponent(typeof(MonsterCombatant))]
    public sealed class EnemyQuestTarget : MonoBehaviour
    {
        [SerializeField] private string questId = "quest-intro";

        private MonsterCombatant combatant;
        private QuestService questService;

        private void Awake()
        {
            combatant = GetComponent<MonsterCombatant>();
            ServiceRegistry.TryGet(out questService);
        }

        private void OnEnable()
        {
            if (combatant != null)
                combatant.KnockedOut.AddListener(OnKnockedOut);
        }

        private void OnDisable()
        {
            if (combatant != null)
                combatant.KnockedOut.RemoveListener(OnKnockedOut);
        }

        private void OnKnockedOut()
        {
            questService?.AdvanceQuest(questId);
        }
    }
}
