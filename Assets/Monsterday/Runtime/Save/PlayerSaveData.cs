using System;
using System.Collections.Generic;

namespace Monsterday.Save
{
    [Serializable]
    public sealed class OwnedMonsterRecord
    {
        public string monsterId;
        public int copies;
        public int level = 1;
        public int evolution;
    }

    [Serializable]
    public sealed class PityCounterRecord
    {
        public string bannerId;
        public int pulls;
    }

    [Serializable]
    public sealed class QuestRecord
    {
        public string questId;
        public int progress;
        public bool completed;
    }

    [Serializable]
    public sealed class ItemRecord
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public sealed class PlayerSaveData
    {
        public int schemaVersion = 1;
        public string playerName = "Beschwörer";
        public int playerLevel = 1;
        public int playerExperience;
        public int energy = 100;
        public int coins = 300;
        public int diamonds = 75;
        public int tickets = 5;
        public int evolutionShards;
        public string lastSaveUtc;
        public string lastDailyClaimUtc;
        public List<OwnedMonsterRecord> monsters = new();
        public List<PityCounterRecord> pityCounters = new();
        public List<QuestRecord> quests = new();
        public List<ItemRecord> items = new();

        public static PlayerSaveData CreateNew() => new();

        public void AddExperience(int amount)
        {
            if (amount <= 0) return;
            playerExperience += amount;
            var nextLevelXp = GetExperienceForLevel(playerLevel);
            while (playerExperience >= nextLevelXp)
            {
                playerLevel++;
                nextLevelXp = GetExperienceForLevel(playerLevel);
            }
        }

        private static int GetExperienceForLevel(int level)
        {
            return 100 * level;
        }

        public PityCounterRecord GetPity(string bannerId)
        {
            var record = pityCounters.Find(entry => entry.bannerId == bannerId);
            if (record != null) return record;
            record = new PityCounterRecord { bannerId = bannerId };
            pityCounters.Add(record);
            return record;
        }

        public QuestRecord GetQuest(string questId)
        {
            var record = quests.Find(entry => entry.questId == questId);
            if (record != null) return record;
            record = new QuestRecord { questId = questId };
            quests.Add(record);
            return record;
        }

        public ItemRecord GetItem(string itemId)
        {
            var record = items.Find(entry => entry.itemId == itemId);
            if (record != null) return record;
            record = new ItemRecord { itemId = itemId };
            items.Add(record);
            return record;
        }
    }
}
