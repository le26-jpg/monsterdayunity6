# Monsterday Unity 6

Dies ist der erste **spielbare, modular aufgebaute Unity-Meilenstein** für Monsterday. Er ersetzt die HTML-App nicht automatisch, sondern bildet die technische Grundlage für das geplante Fantasy-Gacha-RPG.

## Enthalten

- Unity 6.0 und Universal Render Pipeline 17
- Input System, Cinemachine 3 und Addressables
- mobile Hochformat-UI mit Safe-Area-Unterstützung
- Hauptmenü mit Abenteuer, Beschwörung, Monster, Team, Shop und Gilde
- acht eigenständige Beispielmonster als ScriptableObjects
- Monsterkatalog, Fraktionen, Elemente, Rollen, Werte und Seltenheiten
- lokaler JSON-Spielstand mit Sicherungsdatei und WebGL-Fallback
- Wallet für Coins, Diamanten und Tickets
- Sammlung mit Duplikaten
- Gacha-Service mit offenen Gewichten, Pity, 10er-Garantie, Rabatt und Bonus-Splittern
- Addressables-fähige Beschwörungssequenz
- Grundbausteine für animierte Kämpfer, Cinemachine-Kameras und Objekt-Pooling

## In Unity öffnen

1. Melde dich einmal in Unity Hub an und aktiviere die kostenlose Unity-Personal-Lizenz.
2. Wähle in Unity Hub **Add → Add project from disk** und diesen Ordner `MonsterdayUnity6`.
3. Öffne das Projekt mit der installierten Version **Unity 6000.5.4f1**. Für Android wird zusätzlich Android Build Support benötigt.
4. Warte, bis Unity die Pakete installiert und die Skripte kompiliert hat.
5. Wähle oben im Unity-Menü **Monsterday → Setup → Create Playable Starter**.
6. Öffne `Assets/Monsterday/Scenes/MainMenu.unity` und drücke **Play**.

Nach der ersten erfolgreichen Hub-Anmeldung kann das Projekt außerdem direkt über `Open-Monsterday.bat` gestartet werden.

Die Einrichtung erstellt die URP-Konfiguration, acht Monster-Assets, ein Standardbanner und eine mobile Starter-Szene. Sie kann erneut ausgeführt werden, ohne den lokalen Spielstand zu löschen.

## Wichtige Grenzen dieses Meilensteins

3D-Modelle, Rigging, Animation-Clips, Musik und fertige Fantasy-Regionen sind keine Textdateien und deshalb noch nicht enthalten. Die vorgesehenen Felder, Addressables-Verweise, Animator-Parameter und Ordner sind vorbereitet. Eigene oder korrekt lizenzierte Assets können dort schrittweise eingesetzt werden.

Die Beschwörung benutzt ausschließlich lokale Demo-Währungen. Ein Echtgeld-Shop, Server, PvP und Gilden sind bewusst noch nicht angeschlossen.

## Animator-Parameter für Monster

- `Speed` (Float)
- `Attack` (Trigger)
- `Hit` (Trigger)
- `Knockout` (Trigger)
- optional für spätere Schritte: `Ability`, `Ultimate`, `Victory`, `Spawn`

## Nächster sinnvoller Meilenstein

Eine kleine 3D-Kampfarena mit zwei vollständig animierten Platzhalter-Monstern, Trefferpartikeln, Schadenszahlen, Cinemachine-Impuls und einer aktiven Fähigkeit. Erst danach sollten Weltkarte, Story und weitere Regionen folgen.
