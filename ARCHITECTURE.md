using System;
using System.Collections.Generic;
using UnityEngine;
#if MONSTERDAY_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace Monsterday.Data
{
    public enum MonsterRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic, Divine, Exclusive, Limited }
    public enum MonsterElement { Fire, Water, Nature, Earth, Air, Ice, Lightning, Light, Shadow, Poison, Metal, Magic, Chaos, Holy, Darkness }
    public enum CombatRole { Tank, Warrior, Berserker, Mage, Warlock, Ranger, Healer, Summoner, Assassin, Support, Defender, Necromancer, Druid, Paladin, Beastmaster }

    [Serializable]
    public struct MonsterStats
    {
        [Min(1)] public int health;
        [Min(0)] public int attack;
        [Min(0)] public int defense;
        [Min(0.1f)] public float moveSpeed;
        [Min(0.1f)] public float attackSpeed;
        [Range(0f, 1f)] public float criticalChance;
    }

    [Serializable]
    public struct AbilityDefinition
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        [Min(0f)] public float cooldownSeconds;
        [Min(0)] public int manaCost;
        public Sprite icon;
    }

    [CreateAssetMenu(menuName = "Monsterday/Data/Monster", fileName = "Monster_")]
    public sealed class MonsterDefinition : ScriptableObject
    {
        [SerializeField] private string id = "monster-id";
        [SerializeField] private string displayName = "Neues Monster";
        [SerializeField, TextArea(3, 8)] private string lore;
        [SerializeField] private string faction = "Unbekannt";
        [SerializeField] private MonsterElement element;
        [SerializeField] private MonsterRarity rarity;
        [SerializeField] private CombatRole role;
        [SerializeField] private MonsterStats baseStats = new() { health = 100, attack = 12, defense = 8, moveSpeed = 3.5f, attackSpeed = 1f, criticalChance = 0.05f };
        [SerializeField] private List<AbilityDefinition> abilities = new();
        [SerializeField] private string nextEvolutionId;
        [SerializeField] private Sprite portrait;
#if MONSTERDAY_ADDRESSABLES
        [SerializeField] private AssetReferenceGameObject prefab;
        public AssetReferenceGameObject Prefab => prefab;
#else
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;
#endif

        public string Id => id;
        public string DisplayName => displayName;
        public string Lore => lore;
        public string Faction => faction;
        public MonsterElement Element => element;
        public MonsterRarity Rarity => rarity;
        public CombatRole Role => role;
        public MonsterStats BaseStats => baseStats;
        public IReadOnlyList<AbilityDefinition> Abilities => abilities;
        public string NextEvolutionId => nextEvolutionId;
        public Sprite Portrait => portrait;

#if UNITY_EDITOR
        public void EditorConfigure(string newId, string newName, string newFaction, MonsterElement newElement, MonsterRarity newRarity, CombatRole newRole, MonsterStats stats)
        {
            id = newId;
            displayName = newName;
            faction = newFaction;
            element = newElement;
            rarity = newRarity;
            role = newRole;
            baseStats = stats;
        }
#endif
    }

    [CreateAssetMenu(menuName = "Monsterday/Data/Monster Catalog", fileName = "MonsterCatalog")]
    public sealed class MonsterCatalog : ScriptableObject
    {
        [SerializeField] private List<MonsterDefinition> monsters = new();
        public IReadOnlyList<MonsterDefinition> Monsters => monsters;

        public MonsterDefinition Find(string id)
        {
            return monsters.Find(monster => monster != null && monster.Id == id);
        }

#if UNITY_EDITOR
        public void EditorSetMonsters(List<MonsterDefinition> definitions) => monsters = definitions;
#endif
    }
}
