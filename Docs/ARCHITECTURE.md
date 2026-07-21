# Architektur

## Schichten

### Data

`MonsterDefinition`, `MonsterCatalog` und `GachaBannerDefinition` sind ScriptableObjects. Sie enthalten unveränderliche Design-Daten, aber keinen Spielerfortschritt.

### Save

`PlayerSaveData` enthält ausschließlich serialisierbare Laufzeitdaten. `JsonSaveService` speichert atomar-ähnlich über eine temporäre Datei, legt vorher eine Sicherung an und verwendet in WebGL PlayerPrefs als Fallback.

### Services

`WalletService`, `MonsterCollectionService` und `GachaService` sind normale C#-Klassen. Sie hängen nicht von einer Szene ab und lassen sich später mit automatisierten Unity-Tests prüfen.

### Presentation

`MainMenuPresenter`, `DemoSummonButton`, `SafeAreaFitter` und `MobileButtonFeedback` verbinden Canvas UI mit den Services. Die Geschäftslogik bleibt außerhalb der UI.

### Runtime Gameplay

`MonsterCombatant` definiert den ersten Bewegung-/Angriff-/Treffer-Zyklus. `CameraDirector` kapselt Cinemachine. `ComponentPool<T>` ist die Grundlage für Schadenszahlen, Trefferpartikel und Projektil-Pooling.

## Szenenfluss

1. `GameBootstrapper` wird vor anderen Komponenten ausgeführt.
2. Spielstand und Services werden geladen und registriert.
3. `MainMenuPresenter` liest das Profil und zeigt die aktive Seite.
4. Eine Beschwörung ruft nur `GachaService.Pull` auf.
5. Der Dienst prüft Kosten, würfelt Seltenheit, wendet Pity an, aktualisiert Sammlung und speichert.
6. Die UI übernimmt ausschließlich die Präsentation des Ergebnisses.

## Erweiterungsregeln

- keine Spiellogik direkt in Button-Callbacks schreiben;
- keine Monsterwerte in Szenen oder Prefabs duplizieren;
- neue Monster über ScriptableObjects anlegen;
- große Modelle, Audio und VFX über Addressables laden;
- Partikel, Projektile und Schadenszahlen poolen;
- speicherrelevante Änderungen durch einen Service ausführen;
- nach jedem Meilenstein eine startbare Szene behalten.
