using System;
using System.IO;
using UnityEngine;

namespace Monsterday.Save
{
    public interface ISaveService
    {
        PlayerSaveData LoadOrCreate();
        void Save(PlayerSaveData data);
    }

    public sealed class JsonSaveService : ISaveService
    {
        private const string WebKey = "monsterday.player.v1";
        private readonly string savePath = Path.Combine(Application.persistentDataPath, "monsterday-save.json");
        private readonly string backupPath = Path.Combine(Application.persistentDataPath, "monsterday-save.backup.json");

        public PlayerSaveData LoadOrCreate()
        {
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                var json = PlayerPrefs.GetString(WebKey, string.Empty);
#else
                var json = File.Exists(savePath) ? File.ReadAllText(savePath) : string.Empty;
#endif
                if (string.IsNullOrWhiteSpace(json)) return PlayerSaveData.CreateNew();
                var data = JsonUtility.FromJson<PlayerSaveData>(json);
                return data ?? PlayerSaveData.CreateNew();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Monsterday-Spielstand konnte nicht geladen werden: {exception.Message}");
                return TryLoadBackup() ?? PlayerSaveData.CreateNew();
            }
        }

        public void Save(PlayerSaveData data)
        {
            if (data == null) return;
            data.lastSaveUtc = DateTime.UtcNow.ToString("O");
            var json = JsonUtility.ToJson(data, true);
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                PlayerPrefs.SetString(WebKey, json);
                PlayerPrefs.Save();
#else
                Directory.CreateDirectory(Application.persistentDataPath);
                var temporaryPath = savePath + ".tmp";
                File.WriteAllText(temporaryPath, json);
                if (File.Exists(savePath)) File.Copy(savePath, backupPath, true);
                if (File.Exists(savePath)) File.Delete(savePath);
                File.Move(temporaryPath, savePath);
#endif
            }
            catch (Exception exception)
            {
                Debug.LogError($"Monsterday-Spielstand konnte nicht gespeichert werden: {exception.Message}");
            }
        }

        private PlayerSaveData TryLoadBackup()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return null;
#else
            try
            {
                return File.Exists(backupPath)
                    ? JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(backupPath))
                    : null;
            }
            catch
            {
                return null;
            }
#endif
        }
    }
}
