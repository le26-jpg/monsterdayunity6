using System;
using Monsterday.Core;
using Monsterday.Economy;
using Monsterday.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Monsterday.UI
{
    public sealed class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup[] panels = Array.Empty<CanvasGroup>();
        [SerializeField] private Text playerLabel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text energyLabel;
        [SerializeField] private Text coinsLabel;
        [SerializeField] private Text diamondsLabel;
        [SerializeField] private Text ticketsLabel;
        [SerializeField] private int initialPanel;
        private WalletService wallet;

        private void Start()
        {
            if (ServiceRegistry.TryGet<WalletService>(out wallet)) wallet.Changed += RefreshHud;
            ShowPanel(initialPanel);
            RefreshHud();
        }

        private void OnDestroy()
        {
            if (wallet != null) wallet.Changed -= RefreshHud;
        }

        public void ShowPanel(int index)
        {
            for (var i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null) continue;
                var active = i == index;
                panels[i].alpha = active ? 1f : 0f;
                panels[i].interactable = active;
                panels[i].blocksRaycasts = active;
                panels[i].gameObject.SetActive(active);
            }
        }

        public void RefreshHud()
        {
            if (!ServiceRegistry.TryGet<PlayerSaveData>(out var profile)) return;
            if (playerLabel != null) playerLabel.text = profile.playerName;
            if (levelLabel != null) levelLabel.text = $"LV {profile.playerLevel}";
            if (energyLabel != null) energyLabel.text = $"⚡ {profile.energy}";
            if (coinsLabel != null) coinsLabel.text = $"Coins {profile.coins}";
            if (diamondsLabel != null) diamondsLabel.text = $"Diamanten {profile.diamonds}";
            if (ticketsLabel != null) ticketsLabel.text = $"Tickets {profile.tickets}";
        }

#if UNITY_EDITOR
        public void EditorConfigure(CanvasGroup[] newPanels, Text player, Text level, Text energy, Text coins, Text diamonds, Text tickets)
        {
            panels = newPanels;
            playerLabel = player;
            levelLabel = level;
            energyLabel = energy;
            coinsLabel = coins;
            diamondsLabel = diamonds;
            ticketsLabel = tickets;
        }
#endif
    }
}
