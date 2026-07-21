using System;
using Monsterday.Data;
using Monsterday.Save;

namespace Monsterday.Economy
{
    public sealed class WalletService
    {
        private readonly PlayerSaveData profile;
        public event Action Changed;

        public WalletService(PlayerSaveData profile) => this.profile = profile;

        public int Get(CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.Coins => profile.coins,
                CurrencyType.Diamonds => profile.diamonds,
                CurrencyType.Tickets => profile.tickets,
                _ => 0
            };
        }

        public bool TrySpend(CurrencyType currency, int amount)
        {
            if (amount < 0 || Get(currency) < amount) return false;
            Add(currency, -amount);
            return true;
        }

        public void Add(CurrencyType currency, int amount)
        {
            switch (currency)
            {
                case CurrencyType.Coins: profile.coins = Math.Max(0, profile.coins + amount); break;
                case CurrencyType.Diamonds: profile.diamonds = Math.Max(0, profile.diamonds + amount); break;
                case CurrencyType.Tickets: profile.tickets = Math.Max(0, profile.tickets + amount); break;
            }
            Changed?.Invoke();
        }
    }
}
