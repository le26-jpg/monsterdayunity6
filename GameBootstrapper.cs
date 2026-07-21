using System.Collections.Generic;
using Monsterday.Core;
using Monsterday.Data;
using Monsterday.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if MONSTERDAY_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Monsterday.Editor
{
    public static class MonsterdayProjectSetup
    {
        private const string Root = "Assets/Monsterday";
        private const string DataPath = Root + "/Data";
        private const string ScenePath = Root + "/Scenes/MainMenu.unity";
        private static readonly Color DeepPurple = Hex("171024");
        private static readonly Color PanelPurple = Hex("27183D");
        private static readonly Color AccentPurple = Hex("A43CFF");
        private static readonly Color Mint = Hex("3CFFA2");
        private static readonly Color Gold = Hex("FFC857");

        [MenuItem("Monsterday/Setup/Create Playable Starter", priority = 1)]
        public static void CreatePlayableStarter()
        {
            EnsureFolders();
            ConfigureProject();
            var catalog = CreateMonsterData();
            var banner = CreateBanner();
            CreateMainMenuScene(catalog, banner);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Monsterday", "Die spielbare Starter-Szene wurde erstellt. Öffne Assets/Monsterday/Scenes/MainMenu.unity und drücke Play.", "Los geht's");
        }

        [MenuItem("Monsterday/Setup/Validate Starter", priority = 2)]
        public static void ValidateStarter()
        {
            var missing = new List<string>();
            if (AssetDatabase.LoadAssetAtPath<MonsterCatalog>(DataPath + "/MonsterCatalog.asset") == null) missing.Add("MonsterCatalog");
            if (AssetDatabase.LoadAssetAtPath<GachaBannerDefinition>(DataPath + "/Banner_Standard.asset") == null) missing.Add("Standardbanner");
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null) missing.Add("MainMenu-Szene");
            EditorUtility.DisplayDialog("Monsterday-Prüfung", missing.Count == 0 ? "Starter vollständig: Daten, Banner und Szene sind vorhanden." : "Fehlt: " + string.Join(", ", missing), "OK");
        }

        private static void EnsureFolders()
        {
            CreateFolder("Assets", "Monsterday");
            CreateFolder(Root, "Data");
            CreateFolder(Root, "Scenes");
            CreateFolder(Root, "Settings");
            CreateFolder(Root, "Art");
            CreateFolder(Root, "Audio");
            CreateFolder(Root, "Animations");
            CreateFolder(Root, "Materials");
            CreateFolder(Root, "Models");
            CreateFolder(Root, "Prefabs");
            CreateFolder(Root, "Effects");
            CreateFolder(Root, "Localization");
        }

        private static void ConfigureProject()
        {
            PlayerSettings.productName = "Monsterday";
            PlayerSettings.companyName = "Monsterday Studio";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            QualitySettings.vSyncCount = 0;

            var pipelinePath = Root + "/Settings/Monsterday_Mobile_URP.asset";
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
            if (pipeline == null)
            {
                pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                pipeline.name = "Monsterday Mobile URP";
                pipeline.LoadBuiltinRendererData(RendererType.UniversalRenderer);
                AssetDatabase.CreateAsset(pipeline, pipelinePath);
            }
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
        }

        private static MonsterCatalog CreateMonsterData()
        {
            var specs = new[]
            {
                new MonsterSpec("ember-claw","Flammenklaue","Feuerwesen",MonsterElement.Fire,MonsterRarity.Common,CombatRole.Warrior,120,18,8),
                new MonsterSpec("mossling","Moosling","Waldgeister",MonsterElement.Nature,MonsterRarity.Common,CombatRole.Support,135,12,12),
                new MonsterSpec("storm-harpy","Sturmharpyie","Harpyien",MonsterElement.Air,MonsterRarity.Rare,CombatRole.Ranger,140,25,10),
                new MonsterSpec("crystal-golem","Kristallgolem","Golems",MonsterElement.Magic,MonsterRarity.Rare,CombatRole.Tank,220,16,26),
                new MonsterSpec("night-healer","Nyra Nachtlicht","Dunkelelfen",MonsterElement.Shadow,MonsterRarity.Epic,CombatRole.Healer,175,22,17),
                new MonsterSpec("phoenix-auron","Auron der Phönix","Phönixe",MonsterElement.Fire,MonsterRarity.Epic,CombatRole.Mage,185,34,12),
                new MonsterSpec("dragon-ignivar","Ignivar","Drachen",MonsterElement.Chaos,MonsterRarity.Legendary,CombatRole.Berserker,260,46,22),
                new MonsterSpec("seraph-elysia","Seraph Elysia","Engel",MonsterElement.Holy,MonsterRarity.Mythic,CombatRole.Paladin,320,52,34)
            };

            var definitions = new List<MonsterDefinition>();
            foreach (var spec in specs)
            {
                var path = $"{DataPath}/Monster_{spec.Id}.asset";
                var definition = AssetDatabase.LoadAssetAtPath<MonsterDefinition>(path);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<MonsterDefinition>();
                    AssetDatabase.CreateAsset(definition, path);
                }
                definition.EditorConfigure(spec.Id, spec.Name, spec.Faction, spec.Element, spec.Rarity, spec.Role,
                    new MonsterStats { health = spec.Health, attack = spec.Attack, defense = spec.Defense, moveSpeed = 3.6f, attackSpeed = 1f, criticalChance = 0.08f });
                EditorUtility.SetDirty(definition);
                definitions.Add(definition);
            }

            var catalogPath = DataPath + "/MonsterCatalog.asset";
            var catalog = AssetDatabase.LoadAssetAtPath<MonsterCatalog>(catalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<MonsterCatalog>();
                AssetDatabase.CreateAsset(catalog, catalogPath);
            }
            catalog.EditorSetMonsters(definitions);
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static GachaBannerDefinition CreateBanner()
        {
            var path = DataPath + "/Banner_Standard.asset";
            var banner = AssetDatabase.LoadAssetAtPath<GachaBannerDefinition>(path);
            if (banner == null)
            {
                banner = ScriptableObject.CreateInstance<GachaBannerDefinition>();
                AssetDatabase.CreateAsset(banner, path);
            }
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
                new List<string> { "dragon-ignivar", "seraph-elysia" });
            EditorUtility.SetDirty(banner);
            return banner;
        }

        private static void CreateMainMenuScene(MonsterCatalog catalog, GachaBannerDefinition banner)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MainMenu";

            var cameraObject = new GameObject("Main Camera", typeof(UnityEngine.Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetPositionAndRotation(new Vector3(0f, 2f, -10f), Quaternion.identity);
            cameraObject.GetComponent<UnityEngine.Camera>().backgroundColor = DeepPurple;
            cameraObject.GetComponent<UnityEngine.Camera>().clearFlags = CameraClearFlags.SolidColor;

            var lightObject = new GameObject("Moon Key Light", typeof(Light));
            var light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.74f, 0.78f, 1f);
            light.intensity = 1.1f;
            lightObject.transform.rotation = Quaternion.Euler(45f, -25f, 0f);

            var bootstrapObject = new GameObject("Game Bootstrapper", typeof(GameBootstrapper));
            bootstrapObject.GetComponent<GameBootstrapper>().EditorConfigure(catalog, banner);

            var canvasObject = new GameObject("Mobile UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            var safeArea = CreateUiObject("Safe Area", canvasObject.transform);
            safeArea.gameObject.AddComponent<SafeAreaFitter>();
            Stretch(safeArea);
            var background = safeArea.gameObject.AddComponent<Image>();
            background.color = DeepPurple;

            var presenter = safeArea.gameObject.AddComponent<MainMenuPresenter>();
            var player = CreateText(safeArea, "Player", "Beschwörer", 34, TextAnchor.MiddleLeft, Color.white);
            SetAnchors(player.rectTransform, new Vector2(.04f, .93f), new Vector2(.34f, .985f));
            var level = CreateText(safeArea, "Level", "LV 1", 24, TextAnchor.MiddleLeft, Mint);
            SetAnchors(level.rectTransform, new Vector2(.04f, .89f), new Vector2(.18f, .935f));
            var energy = CreateText(safeArea, "Energy", "⚡ 100", 24, TextAnchor.MiddleCenter, Gold);
            SetAnchors(energy.rectTransform, new Vector2(.20f, .9f), new Vector2(.36f, .96f));
            var coins = CreateText(safeArea, "Coins", "Coins 300", 22, TextAnchor.MiddleCenter, Gold);
            SetAnchors(coins.rectTransform, new Vector2(.38f, .91f), new Vector2(.59f, .97f));
            var diamonds = CreateText(safeArea, "Diamonds", "Diamanten 75", 22, TextAnchor.MiddleCenter, Mint);
            SetAnchors(diamonds.rectTransform, new Vector2(.6f, .91f), new Vector2(.81f, .97f));
            var tickets = CreateText(safeArea, "Tickets", "Tickets 5", 22, TextAnchor.MiddleCenter, Color.white);
            SetAnchors(tickets.rectTransform, new Vector2(.82f, .91f), new Vector2(.98f, .97f));

            var panelNames = new[] { "Abenteuer", "Beschwörung", "Monster", "Team", "Shop", "Gilde" };
            var panels = new CanvasGroup[panelNames.Length];
            for (var i = 0; i < panelNames.Length; i++) panels[i] = CreatePanel(safeArea, panelNames[i], i);
            BuildHomePanel(panels[0].transform);
            BuildSummonPanel(panels[1].transform, banner);
            BuildPlaceholder(panels[2].transform, "🐲 MONSTER", "Sammlung, Level, Entwicklung und Fähigkeiten");
            BuildPlaceholder(panels[3].transform, "⚔ TEAM", "Stelle fünf Monster für die Arena auf");
            BuildPlaceholder(panels[4].transform, "💎 SHOP", "Demo ohne Echtgeld – später an einen sicheren Store anbinden");
            BuildPlaceholder(panels[5].transform, "🛡 GILDE", "Optionales Online-System für eine spätere Phase");
            presenter.EditorConfigure(panels, player, level, energy, coins, diamonds, tickets);

            for (var i = 0; i < panelNames.Length; i++)
            {
                var button = CreateButton(safeArea, "Nav " + panelNames[i], panelNames[i], AccentPurple);
                SetAnchors(button.GetComponent<RectTransform>(), new Vector2(i / 6f, 0f), new Vector2((i + 1) / 6f, .085f), new Vector2(5f, 8f), new Vector2(-5f, -8f));
                UnityEventTools.AddIntPersistentListener(button.onClick, presenter.ShowPanel, i);
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem));
#if MONSTERDAY_INPUT_SYSTEM
            eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        }

        private static void BuildHomePanel(Transform parent)
        {
            var title = CreateText(parent, "Title", "MONSTERDAY", 72, TextAnchor.MiddleCenter, Mint);
            SetAnchors(title.rectTransform, new Vector2(.08f, .7f), new Vector2(.92f, .88f));
            var subtitle = CreateText(parent, "Subtitle", "DAS PORTAL VON AETHERRA", 27, TextAnchor.MiddleCenter, Color.white);
            SetAnchors(subtitle.rectTransform, new Vector2(.08f, .64f), new Vector2(.92f, .72f));
            var adventure = CreateButton(parent, "Adventure", "ABENTEUER STARTEN", AccentPurple);
            SetAnchors(adventure.GetComponent<RectTransform>(), new Vector2(.16f, .34f), new Vector2(.84f, .44f));
            adventure.interactable = false;
            var note = CreateText(parent, "Milestone", "UNITY-6 STARTER · MOBILE UI · OFFLINE SAVE · GACHA-KERN", 20, TextAnchor.MiddleCenter, Gold);
            SetAnchors(note.rectTransform, new Vector2(.08f, .22f), new Vector2(.92f, .31f));
        }

        private static void BuildSummonPanel(Transform parent, GachaBannerDefinition banner)
        {
            var title = CreateText(parent, "Portal Title", "PORTAL VON AETHERRA", 52, TextAnchor.MiddleCenter, Mint);
            SetAnchors(title.rectTransform, new Vector2(.08f, .72f), new Vector2(.92f, .87f));
            var portal = CreateText(parent, "Portal", "✦  ◇  ✦\n  ◯\n✦  ◇  ✦", 76, TextAnchor.MiddleCenter, AccentPurple);
            SetAnchors(portal.rectTransform, new Vector2(.15f, .42f), new Vector2(.85f, .72f));
            var result = CreateText(parent, "Result", "Berühre das Portal und beschwöre dein erstes Monster.", 25, TextAnchor.MiddleCenter, Color.white);
            SetAnchors(result.rectTransform, new Vector2(.1f, .28f), new Vector2(.9f, .42f));
            var single = CreateButton(parent, "Single Pull", "1× BESCHWÖREN · 50 DIAMANTEN", AccentPurple);
            SetAnchors(single.GetComponent<RectTransform>(), new Vector2(.09f, .15f), new Vector2(.49f, .25f));
            single.gameObject.AddComponent<DemoSummonButton>().EditorConfigure(banner, 1, result);
            var ten = CreateButton(parent, "Ten Pull", "10× · 450 + BONUS", Gold);
            SetAnchors(ten.GetComponent<RectTransform>(), new Vector2(.51f, .15f), new Vector2(.91f, .25f));
            ten.gameObject.AddComponent<DemoSummonButton>().EditorConfigure(banner, 10, result);
            var odds = CreateText(parent, "Odds", "Offene Chancen: 52 % gewöhnlich · 30 % selten · 13 % episch · 4,5 % legendär · 0,5 % mythisch", 18, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, .68f));
            SetAnchors(odds.rectTransform, new Vector2(.06f, .07f), new Vector2(.94f, .14f));
        }

        private static void BuildPlaceholder(Transform parent, string titleText, string bodyText)
        {
            var title = CreateText(parent, "Title", titleText, 58, TextAnchor.MiddleCenter, Mint);
            SetAnchors(title.rectTransform, new Vector2(.08f, .55f), new Vector2(.92f, .72f));
            var body = CreateText(parent, "Body", bodyText + "\n\nDieser Bereich wird in einem nächsten Meilenstein ausgebaut.", 28, TextAnchor.MiddleCenter, Color.white);
            SetAnchors(body.rectTransform, new Vector2(.1f, .3f), new Vector2(.9f, .56f));
        }

        private static CanvasGroup CreatePanel(RectTransform parent, string name, int index)
        {
            var rect = CreateUiObject(name + " Panel", parent);
            SetAnchors(rect, new Vector2(.025f, .1f), new Vector2(.975f, .89f));
            var image = rect.gameObject.AddComponent<Image>();
            image.color = index == 1 ? new Color(.11f, .035f, .18f, .96f) : PanelPurple;
            var group = rect.gameObject.AddComponent<CanvasGroup>();
            rect.gameObject.SetActive(index == 0);
            return group;
        }

        private static Button CreateButton(Transform parent, string name, string label, Color color)
        {
            var rect = CreateUiObject(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            rect.gameObject.AddComponent<MobileButtonFeedback>();
            var text = CreateText(rect, "Label", label, 22, TextAnchor.MiddleCenter, color == Gold ? DeepPurple : Color.white);
            Stretch(text.rectTransform, 8f);
            return button;
        }

        private static Text CreateText(Transform parent, string name, string value, int size, TextAnchor anchor, Color color)
        {
            var rect = CreateUiObject(name, parent);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = size;
            text.alignment = anchor;
            text.color = color;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(10, size / 2);
            text.resizeTextMaxSize = size;
            return text;
        }

        private static RectTransform CreateUiObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<RectTransform>();
        }

        private static void SetAnchors(RectTransform rect, Vector2 minimum, Vector2 maximum, Vector2? offsetMinimum = null, Vector2? offsetMaximum = null)
        {
            rect.anchorMin = minimum;
            rect.anchorMax = maximum;
            rect.offsetMin = offsetMinimum ?? Vector2.zero;
            rect.offsetMax = offsetMaximum ?? Vector2.zero;
        }

        private static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.one * margin;
            rect.offsetMax = Vector2.one * -margin;
        }

        private static void CreateFolder(string parent, string child)
        {
            var path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }

        private static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString("#" + value, out var color) ? color : Color.magenta;
        }

        private readonly struct MonsterSpec
        {
            public readonly string Id, Name, Faction;
            public readonly MonsterElement Element;
            public readonly MonsterRarity Rarity;
            public readonly CombatRole Role;
            public readonly int Health, Attack, Defense;
            public MonsterSpec(string id, string name, string faction, MonsterElement element, MonsterRarity rarity, CombatRole role, int health, int attack, int defense)
            {
                Id = id; Name = name; Faction = faction; Element = element; Rarity = rarity; Role = role; Health = health; Attack = attack; Defense = defense;
            }
        }
    }
}
