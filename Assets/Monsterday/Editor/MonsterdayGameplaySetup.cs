using System.Collections.Generic;
using Monsterday.Core;
using Monsterday.Data;
using Monsterday.Gameplay;
using Monsterday.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Monsterday.Editor
{
    public static class MonsterdayGameplaySetup
    {
        private const string Root = "Assets/Monsterday";
        private const string ScenesPath = Root + "/Scenes";
        private const string SettingsPath = Root + "/Settings";
        private const string PrefabsPath = Root + "/Prefabs";
        private const string DataPath = Root + "/Data";

        [MenuItem("Monsterday/Setup/Create Exploration Starter", priority = 3)]
        public static void CreateExplorationStarter()
        {
            EnsureFolders();
            ConfigureProject();
            var catalog = CreateMonsterData();
            var banner = CreateBanner();
            CreateExplorationScene(catalog, banner);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Monsterday", "Die Exploration-Starter-Szene wurde erstellt. Öffne Assets/Monsterday/Scenes/Exploration.unity und drücke Play.", "OK");
        }

        private static void EnsureFolders()
        {
            CreateFolder("Assets", "Monsterday");
            CreateFolder(Root, "Data");
            CreateFolder(Root, "Scenes");
            CreateFolder(Root, "Settings");
            CreateFolder(Root, "Prefabs");
            CreateFolder(Root, "Art");
            CreateFolder(Root, "Audio");
            CreateFolder(Root, "Animations");
            CreateFolder(Root, "Materials");
            CreateFolder(Root, "Models");
            CreateFolder(Root, "Effects");
            CreateFolder(Root, "Localization");
        }

        private static void ConfigureProject()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(SettingsPath + "/Monsterday_Mobile_URP.asset");
            if (pipeline == null)
            {
                pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                pipeline.name = "Monsterday Mobile URP";
                pipeline.LoadBuiltinRendererData(RendererType.UniversalRenderer);
                AssetDatabase.CreateAsset(pipeline, SettingsPath + "/Monsterday_Mobile_URP.asset");
            }
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
        }

        private static MonsterCatalog CreateMonsterData()
        {
            var path = DataPath + "/MonsterCatalog.asset";
            var catalog = AssetDatabase.LoadAssetAtPath<MonsterCatalog>(path);
            if (catalog != null) return catalog;

            var monsters = new List<MonsterDefinition>();
            var specs = new[]
            {
                new MonsterSpec("ember-claw", "Flammenklaue", "Feuerwesen", MonsterElement.Fire, MonsterRarity.Common, CombatRole.Warrior, 120, 18, 8),
                new MonsterSpec("crystal-golem", "Kristallgolem", "Golems", MonsterElement.Magic, MonsterRarity.Rare, CombatRole.Tank, 220, 16, 26),
                new MonsterSpec("night-healer", "Nyra Nachtlicht", "Dunkelelfen", MonsterElement.Shadow, MonsterRarity.Epic, CombatRole.Healer, 175, 22, 17)
            };

            foreach (var spec in specs)
            {
                var monsterPath = DataPath + $"/Monster_{spec.Id}.asset";
                var monster = AssetDatabase.LoadAssetAtPath<MonsterDefinition>(monsterPath);
                if (monster == null)
                {
                    monster = ScriptableObject.CreateInstance<MonsterDefinition>();
                    AssetDatabase.CreateAsset(monster, monsterPath);
                }
                monster.EditorConfigure(spec.Id, spec.Name, spec.Faction, spec.Element, spec.Rarity, spec.Role,
                    new MonsterStats { health = spec.Health, attack = spec.Attack, defense = spec.Defense, moveSpeed = 3.5f, attackSpeed = 1f, criticalChance = 0.1f });
                EditorUtility.SetDirty(monster);
                monsters.Add(monster);
            }

            catalog = ScriptableObject.CreateInstance<MonsterCatalog>();
            catalog.EditorSetMonsters(monsters);
            AssetDatabase.CreateAsset(catalog, path);
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static GachaBannerDefinition CreateBanner()
        {
            var path = DataPath + "/Banner_Standard.asset";
            var banner = AssetDatabase.LoadAssetAtPath<GachaBannerDefinition>(path);
            if (banner != null) return banner;
            banner = ScriptableObject.CreateInstance<GachaBannerDefinition>();
            banner.EditorConfigure("standard", "Portal von Aetherra",
                new List<RarityWeight>
                {
                    new() { rarity = MonsterRarity.Common, weight = 52f },
                    new() { rarity = MonsterRarity.Rare, weight = 30f },
                    new() { rarity = MonsterRarity.Epic, weight = 13f },
                    new() { rarity = MonsterRarity.Legendary, weight = 4.5f },
                    new() { rarity = MonsterRarity.Mythic, weight = 0.5f }
                },
                new List<PityMilestone>
                {
                    new() { everyPulls = 10, minimumRarity = MonsterRarity.Rare },
                    new() { everyPulls = 40, minimumRarity = MonsterRarity.Legendary },
                    new() { everyPulls = 100, minimumRarity = MonsterRarity.Mythic }
                },
                new List<string> { "crystal-golem", "night-healer" });
            AssetDatabase.CreateAsset(banner, path);
            EditorUtility.SetDirty(banner);
            return banner;
        }

        public static void BuildExplorationScene(MonsterCatalog catalog, GachaBannerDefinition banner)
        {
            EnsureFolders();
            ConfigureProject();
            CreateExplorationScene(catalog, banner);
        }

        private static void CreateExplorationScene(MonsterCatalog catalog, GachaBannerDefinition banner)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera", typeof(Camera));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 8f, -10f);
            cameraObject.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            cameraObject.AddComponent<SimpleCameraFollow>();

            var terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
            terrain.name = "Ground";
            terrain.transform.localScale = new Vector3(4f, 1f, 4f);
            var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = new Color(0.28f, 0.45f, 0.18f) };
            terrain.GetComponent<Renderer>().sharedMaterial = groundMat;

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            var playerRigidbody = player.AddComponent<Rigidbody>();
            playerRigidbody.isKinematic = true;
            playerRigidbody.useGravity = false;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            var playerCombat = player.AddComponent<MonsterCombatant>();
            playerCombat.EditorConfigure(catalog.Monsters[0], null, player.transform);
            player.AddComponent<PlayerController>();
            var playerMat = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = new Color(0.2f, 0.6f, 0.85f) };
            player.GetComponent<Renderer>().sharedMaterial = playerMat;
            var playerCamFollow = cameraObject.GetComponent<SimpleCameraFollow>();
            playerCamFollow.SetTarget(player.transform);

            var enemy = CreateEnemy(catalog.Monsters[1], new Vector3(4f, 1f, 4f));
            var enemy2 = CreateEnemy(catalog.Monsters[2], new Vector3(-4f, 1f, 4f));

            var uiCanvas = CreateUICanvas();
            CreateEventSystem();
            var questDefinition = CreateQuestDefinition();
#if UNITY_EDITOR
            questDefinition.EditorConfigure("quest-intro", "Erste Herausforderung", "Besiege einen Gegner und hole deine Belohnung.", 1, 100, 10);
#endif
            CreateFolder("Assets", "Resources");
            CreateFolder("Assets/Resources", "Monsterday");
            CreateFolder("Assets/Resources/Monsterday", "Quests");
            AssetDatabase.CreateAsset(questDefinition, "Assets/Resources/Monsterday/Quests/Quest_Intro.asset");

            var itemPickup = CreateItemPickup(new Vector3(2f, 1f, -2f));
            itemPickup.Configure("health-potion", 1, "Heiltrank");

            var bootstrapObject = new GameObject("Game Bootstrapper", typeof(GameBootstrapper));
            bootstrapObject.GetComponent<GameBootstrapper>().EditorConfigure(catalog, banner);

            EditorSceneManager.SaveScene(scene, ScenesPath + "/Exploration.unity");
            AddSceneToBuildSettings(ScenesPath + "/Exploration.unity");
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (scenes.Exists(scene => scene.path == scenePath)) return;
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static GameObject CreateEnemy(MonsterDefinition monster, Vector3 position)
        {
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = monster.DisplayName;
            enemy.transform.position = position;
            var combatant = enemy.AddComponent<MonsterCombatant>();
            combatant.EditorConfigure(monster, null, enemy.transform);
            enemy.AddComponent<EnemyQuestTarget>();
            return enemy;
        }

        private static GameObject CreateItemPickup(Vector3 position)
        {
            var pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickup.name = "Health Potion";
            pickup.transform.position = position;
            var itemPickup = pickup.AddComponent<ItemPickup>();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = new Color(0.9f, 0.75f, 0.2f) };
            pickup.GetComponent<Renderer>().sharedMaterial = material;
            return pickup;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Object.DontDestroyOnLoad(eventSystem);
        }

        private static GameObject CreateUICanvas()
        {
            var uiObject = new GameObject("UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = uiObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = uiObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            var panel = new GameObject("HUD", typeof(RectTransform));
            panel.transform.SetParent(uiObject.transform, false);
            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0.2f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            CreateText(panel.transform, "Coins", new Vector2(0.1f, 0.7f), 24, out var coinsLabel);
            CreateText(panel.transform, "Diamonds", new Vector2(0.1f, 0.45f), 24, out var diamondsLabel);
            CreateText(panel.transform, "QuestTitle", new Vector2(0.5f, 0.7f), 26, out var questTitleLabel);
            CreateText(panel.transform, "QuestProgress", new Vector2(0.5f, 0.45f), 24, out var questProgressLabel);
            CreateText(panel.transform, "ItemCount", new Vector2(0.9f, 0.7f), 24, out var itemCountLabel);

            var manager = uiObject.AddComponent<GameUIManager>();
            manager.coinsLabel = coinsLabel;
            manager.diamondsLabel = diamondsLabel;
            manager.questTitleLabel = questTitleLabel;
            manager.questProgressLabel = questProgressLabel;
            manager.itemCountLabel = itemCountLabel;

            return uiObject;
        }

        private static void CreateText(Transform parent, string name, Vector2 anchor, int size, out Text text)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var rt = textObject.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.sizeDelta = new Vector2(420f, 80f);

            text = textObject.GetComponent<Text>();
            text.text = name;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        private static QuestDefinition CreateQuestDefinition()
        {
            var quest = ScriptableObject.CreateInstance<QuestDefinition>();
            return quest;
        }

        private static void CreateFolder(string parent, string child)
        {
            var path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }

        private readonly struct MonsterSpec
        {
            public readonly string Id;
            public readonly string Name;
            public readonly string Faction;
            public readonly MonsterElement Element;
            public readonly MonsterRarity Rarity;
            public readonly CombatRole Role;
            public readonly int Health;
            public readonly int Attack;
            public readonly int Defense;

            public MonsterSpec(string id, string name, string faction, MonsterElement element, MonsterRarity rarity, CombatRole role, int health, int attack, int defense)
            {
                Id = id;
                Name = name;
                Faction = faction;
                Element = element;
                Rarity = rarity;
                Role = role;
                Health = health;
                Attack = attack;
                Defense = defense;
            }
        }
    }
}
