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

        public static PlayerSaveData CreateNew() => new();

        public PityCounterRecord GetPity(string bannerId)
        {
            var record = pityCounters.Find(entry => entry.bannerId == bannerId);
            if (record != null) return record;
            record = new PityCounterRecord { bannerId = bannerId };
            pityCounters.Add(record);
            return record;
        }
    }
}
