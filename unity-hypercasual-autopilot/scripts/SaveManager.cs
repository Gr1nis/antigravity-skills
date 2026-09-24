using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Hypercasual.Core
{
    [Serializable]
    public class GameData
    {
        public int Coins = 0;
        public int HighScore = 0;
        public int CurrentLevel = 1;
        public bool SoundEnabled = true;
        public bool VibrationEnabled = true;
        public string LastLoginDate = "";
        public string SelectedSkinId = "default";
    }

    /// <summary>
    /// Универсальный менеджер сохранений для гиперказуалок.
    /// Поддерживает сохранение в JSON файл с шифрованием и резервное сохранение в PlayerPrefs.
    /// Не требует ручной настройки на сцене: создает себя сам при первом обращении.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager _instance;
        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[SaveManager]");
                    _instance = go.AddComponent<SaveManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public static GameData Data => Instance._data;

        public static event Action OnDataSaved;
        public static event Action OnDataLoaded;

        [Header("Encryption Settings")]
        [SerializeField] private bool useEncryption = true;
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("HyperSecretKey99"); // 16 bytes for AES-128

        private GameData _data = new GameData();
        private string _saveFilePath;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _saveFilePath = Path.Combine(Application.persistentDataPath, "savedata.dat");
            LoadGame();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public void SaveGame()
        {
            try
            {
                _data.LastLoginDate = DateTime.UtcNow.ToString("O");
                string json = JsonUtility.ToJson(_data, true);

                if (useEncryption)
                {
                    byte[] encrypted = EncryptStringToBytes(json, EncryptionKey);
                    File.WriteAllBytes(_saveFilePath, encrypted);
                }
                else
                {
                    File.WriteAllText(_saveFilePath, json);
                }

                // Резервная копия в PlayerPrefs на случай сбоев мобильной файловой системы
                PlayerPrefs.SetInt("hc_coins", _data.Coins);
                PlayerPrefs.SetInt("hc_highscore", _data.HighScore);
                PlayerPrefs.SetInt("hc_level", _data.CurrentLevel);
                PlayerPrefs.Save();

                OnDataSaved?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Ошибка при сохранении данных: {ex.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                if (File.Exists(_saveFilePath))
                {
                    string json;
                    if (useEncryption)
                    {
                        byte[] encrypted = File.ReadAllBytes(_saveFilePath);
                        json = DecryptStringFromBytes(encrypted, EncryptionKey);
                    }
                    else
                    {
                        json = File.ReadAllText(_saveFilePath);
                    }

                    _data = JsonUtility.FromJson<GameData>(json);
                }
                else
                {
                    // Первый запуск: пытаемся восстановить из PlayerPrefs
                    _data = new GameData
                    {
                        Coins = PlayerPrefs.GetInt("hc_coins", 0),
                        HighScore = PlayerPrefs.GetInt("hc_highscore", 0),
                        CurrentLevel = PlayerPrefs.GetInt("hc_level", 1),
                        LastLoginDate = DateTime.UtcNow.ToString("O")
                    };
                    SaveGame();
                }

                OnDataLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveManager] Ошибка чтения сохранения, создание нового: {ex.Message}");
                _data = new GameData();
                SaveGame();
            }
        }

        public void AddCoins(int amount)
        {
            _data.Coins = Mathf.Max(0, _data.Coins + amount);
            SaveGame();
        }

        public bool TrySpendCoins(int amount)
        {
            if (_data.Coins >= amount)
            {
                _data.Coins -= amount;
                SaveGame();
                return true;
            }
            return false;
        }

        public void UpdateHighScore(int score)
        {
            if (score > _data.HighScore)
            {
                _data.HighScore = score;
                SaveGame();
            }
        }

        #region AES Encryption Helpers
        private static byte[] EncryptStringToBytes(string plainText, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = new byte[16]; // Fixed zero-IV for simple local tamper protection
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = new byte[16];
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
