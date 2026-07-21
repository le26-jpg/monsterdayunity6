using System.Linq;
using Monsterday.Core;
using Monsterday.Economy;
using Monsterday.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Monsterday.Gameplay
{
    public sealed class GameUIManager : MonoBehaviour
    {
        public Text coinsLabel;
        public Text diamondsLabel;
        public Text questTitleLabel;
        public Text questProgressLabel;
        public Text itemCountLabel;
        public Text healthLabel;
        public Text xpLabel;

        private WalletService wallet;
        private InventoryService inventory;
        private QuestService questService;
        private PlayerSaveData profile;
        private MonsterCombatant playerCombatant;
        private QuestDefinition activeQuest;
        private QuestRecord questRecord;

        private void Start()
        {
            ServiceRegistry.TryGet(out wallet);
            ServiceRegistry.TryGet(out inventory);
            ServiceRegistry.TryGet(out questService);

            if (wallet != null) wallet.Changed += RefreshHud;
            if (inventory != null) inventory.InventoryChanged += RefreshHud;

            activeQuest = questService?.AllDefinitions.FirstOrDefault();
            if (activeQuest != null)
            {
                questRecord = questService.GetQuest(activeQuest.Id);
                questService.QuestUpdated += OnQuestUpdated;
            }

            RefreshHud();
        }

        private void OnDestroy()
        {
            if (wallet != null) wallet.Changed -= RefreshHud;
            if (inventory != null) inventory.InventoryChanged -= RefreshHud;
            if (questService != null) questService.QuestUpdated -= OnQuestUpdated;
        }

        private void OnQuestUpdated(QuestDefinition definition, QuestRecord record)
        {
            if (definition == null || record == null) return;
            if (activeQuest == null || activeQuest.Id != definition.Id) activeQuest = definition;
            questRecord = record;
            RefreshHud();
        }

        public void RefreshHud()
        {
            if (wallet != null)
            {
                if (coinsLabel != null) coinsLabel.text = $"Coins: {wallet.Get(CurrencyType.Coins)}";
                if (diamondsLabel != null) diamondsLabel.text = $"Diamanten: {wallet.Get(CurrencyType.Diamonds)}";
            }

            if (playerCombatant != null && healthLabel != null)
            {
                healthLabel.text = $"HP: {playerCombatant.CurrentHealth}/{playerCombatant.MaxHealth}";
            }

            if (profile != null && xpLabel != null)
            {
                xpLabel.text = $"LV {profile.playerLevel} · XP {profile.playerExperience}";
            }

            if (activeQuest != null && questRecord != null)
            {
                if (questTitleLabel != null) questTitleLabel.text = activeQuest.Title;
                if (questProgressLabel != null) questProgressLabel.text = questRecord.completed ? "Abgeschlossen" : $"{questRecord.progress}/{activeQuest.ObjectiveCount}";
            }

            if (inventory != null && itemCountLabel != null)
            {
                var itemCount = inventory.GetItemCount("health-potion");
                itemCountLabel.text = $"Tränke: {itemCount}";
            }
        }
    }
}
