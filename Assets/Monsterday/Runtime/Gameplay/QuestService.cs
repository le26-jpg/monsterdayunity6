using System;
using System.Collections.Generic;
using System.Linq;
using Monsterday.Data;
using Monsterday.Economy;
using Monsterday.Save;
using UnityEngine;

namespace Monsterday.Gameplay
{
    public sealed class QuestService
    {
        private readonly PlayerSaveData profile;
        private readonly WalletService wallet;
        private readonly ISaveService save;
        private readonly Dictionary<string, QuestDefinition> definitions = new();

        public event Action<QuestDefinition, QuestRecord> QuestUpdated;

        public QuestService(PlayerSaveData profile, WalletService wallet, ISaveService save)
        {
            this.profile = profile;
            this.wallet = wallet;
            this.save = save;
            LoadDefinitions();
        }

        public IReadOnlyCollection<QuestDefinition> AllDefinitions => definitions.Values;

        public QuestRecord GetQuest(string questId)
        {
            return profile.GetQuest(questId);
        }

        public void AdvanceQuest(string questId, int amount = 1)
        {
            if (!definitions.TryGetValue(questId, out var definition)) return;
            var record = GetQuest(questId);
            if (record.completed) return;
            record.progress = Math.Min(definition.ObjectiveCount, record.progress + amount);
            if (record.progress >= definition.ObjectiveCount)
            {
                record.completed = true;
                wallet.Add(CurrencyType.Coins, definition.RewardCoins);
                wallet.Add(CurrencyType.Diamonds, definition.RewardDiamonds);
                profile.AddExperience(definition.RewardXp);
            }
            QuestUpdated?.Invoke(definition, record);
            save.Save(profile);
        }

        private void LoadDefinitions()
        {
            var questDefinitions = Resources.LoadAll<QuestDefinition>("Monsterday/Quests");
            foreach (var quest in questDefinitions)
            {
                if (quest == null || string.IsNullOrWhiteSpace(quest.Id)) continue;
                definitions[quest.Id] = quest;
            }
        }
    }
}
