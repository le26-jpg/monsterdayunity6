using Monsterday.Collection;
using Monsterday.Data;
using Monsterday.Economy;
using Monsterday.Gacha;
using Monsterday.Save;
using UnityEngine;

namespace Monsterday.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private static GameBootstrapper instance;
        [SerializeField] private MonsterCatalog monsterCatalog;
        [SerializeField] private GachaBannerDefinition defaultBanner;

        public PlayerSaveData Profile { get; private set; }
        public GachaBannerDefinition DefaultBanner => defaultBanner;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            ServiceRegistry.Clear();

            var save = new JsonSaveService();
            Profile = save.LoadOrCreate();
            var wallet = new WalletService(Profile);
            var collection = new MonsterCollectionService(Profile);
            var gacha = new GachaService(monsterCatalog, Profile, wallet, collection, save);

            ServiceRegistry.Register<ISaveService>(save);
            ServiceRegistry.Register(Profile);
            ServiceRegistry.Register(wallet);
            ServiceRegistry.Register(collection);
            ServiceRegistry.Register(gacha);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) SaveNow();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) SaveNow();
        }

        private void OnApplicationQuit() => SaveNow();

        public void SaveNow()
        {
            if (Profile != null && ServiceRegistry.TryGet<ISaveService>(out var save)) save.Save(Profile);
        }

#if UNITY_EDITOR
        public void EditorConfigure(MonsterCatalog catalog, GachaBannerDefinition banner)
        {
            monsterCatalog = catalog;
            defaultBanner = banner;
        }
#endif
    }
}
