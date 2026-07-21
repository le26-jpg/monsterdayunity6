using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monsterday.Data
{
    public enum CurrencyType { Coins, Diamonds, Tickets }

    [Serializable]
    public struct RarityWeight
    {
        public MonsterRarity rarity;
        [Min(0f)] public float weight;
    }

    [Serializable]
    public struct PityMilestone
    {
        [Min(1)] public int everyPulls;
        public MonsterRarity minimumRarity;
    }

    [CreateAssetMenu(menuName = "Monsterday/Data/Gacha Banner", fileName = "Banner_")]
    public sealed class GachaBannerDefinition : ScriptableObject
    {
        [SerializeField] private string id = "standard";
        [SerializeField] private string displayName = "Standardportal";
        [SerializeField] private CurrencyType currency = CurrencyType.Diamonds;
        [SerializeField, Min(0)] private int singlePullCost = 50;
        [SerializeField, Min(0)] private int tenPullCost = 450;
        [SerializeField, Min(0)] private int tenPullBonusShards = 5;
        [SerializeField, Range(0f, 1f)] private float featuredChance = 0.5f;
        [SerializeField] private List<RarityWeight> rarityWeights = new();
        [SerializeField] private List<PityMilestone> pityMilestones = new();
        [SerializeField] private List<string> featuredMonsterIds = new();

        public string Id => id;
        public string DisplayName => displayName;
        public CurrencyType Currency => currency;
        public int SinglePullCost => singlePullCost;
        public int TenPullCost => tenPullCost;
        public int TenPullBonusShards => tenPullBonusShards;
        public float FeaturedChance => featuredChance;
        public IReadOnlyList<RarityWeight> RarityWeights => rarityWeights;
        public IReadOnlyList<PityMilestone> PityMilestones => pityMilestones;
        public IReadOnlyList<string> FeaturedMonsterIds => featuredMonsterIds;

#if UNITY_EDITOR
        public void EditorConfigure(string newId, string newName, List<RarityWeight> weights, List<PityMilestone> milestones, List<string> featured)
        {
            id = newId;
            displayName = newName;
            rarityWeights = weights;
            pityMilestones = milestones;
            featuredMonsterIds = featured;
        }
#endif
    }
}
