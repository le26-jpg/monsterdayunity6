using UnityEngine;

namespace Monsterday.Gameplay
{
    [CreateAssetMenu(menuName = "Monsterday/Data/Quest", fileName = "Quest_")]
    public sealed class QuestDefinition : ScriptableObject
    {
        [SerializeField] private string id = "quest-intro";
        [SerializeField] private string title = "Erste Aufgabe";
        [SerializeField, TextArea(3, 6)] private string description = "Besiege einen Gegner und erhalte deine ersten Belohnungen.";
        [SerializeField, Min(1)] private int objectiveCount = 1;
        [SerializeField, Min(0)] private int rewardCoins = 100;
        [SerializeField, Min(0)] private int rewardDiamonds;
        [SerializeField, Min(0)] private int rewardXp = 50;

        public string Id => id;
        public string Title => title;
        public string Description => description;
        public int ObjectiveCount => objectiveCount;
        public int RewardCoins => rewardCoins;
        public int RewardDiamonds => rewardDiamonds;
        public int RewardXp => rewardXp;

#if UNITY_EDITOR
        public void EditorConfigure(string id, string title, string description, int objectiveCount, int rewardCoins, int rewardDiamonds, int rewardXp)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.objectiveCount = objectiveCount;
            this.rewardCoins = rewardCoins;
            this.rewardDiamonds = rewardDiamonds;
            this.rewardXp = rewardXp;
        }
#endif
    }
}
            this.id = id;
            this.title = title;
            this.description = description;
            this.objectiveCount = objectiveCount;
            this.rewardCoins = rewardCoins;
            this.rewardDiamonds = rewardDiamonds;
        }
#endif
    }
}
