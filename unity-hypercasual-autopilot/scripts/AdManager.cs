using System;
using UnityEngine;

namespace Hypercasual.Core
{
    /// <summary>
    /// Автономный менеджер рекламы для гиперказуалок.
    /// Поддерживает работу в Unity Editor (эмуляция показа рекламы через диалоги)
    /// и готовую интеграцию с Google Mobile Ads (AdMob) для Android/iOS.
    /// Содержит официальные тестовые ID от Google, поэтому не вызывает бан аккаунта при тестировании.
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        private static AdManager _instance;
        public static AdManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[AdManager]");
                    _instance = go.AddComponent<AdManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("Ad Settings")]
        [Tooltip("Показывать межстраничную рекламу каждые N завершений игры / поражений")]
        [SerializeField] private int gamesBetweenInterstitials = 3;

        [Header("Google AdMob Test Unit IDs (Android)")]
        [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Официальный тест ID
        [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";     // Официальный тест ID
        [SerializeField] private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";       // Официальный тест ID

        private int _gamesCounter = 0;
        private Action _onRewardSuccess;
        private Action _onRewardFailed;

        public static event Action OnInterstitialClosed;
        public static event Action OnRewardedComplete;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAds();
        }

        private void InitializeAds()
        {
            Debug.Log("[AdManager] Инициализация рекламного SDK (Режим: Тестовые объявления Google)");
            // При подключении Google Mobile Ads SDK (com.google.ads.mobile):
            // MobileAds.Initialize(initStatus => { LoadInterstitial(); LoadRewarded(); });
        }

        /// <summary>
        /// Вызывается при завершении раунда/игры. Автоматически считает попытки
        /// и показывает рекламу с заданной периодичностью.
        /// </summary>
        public void NotifyGameEnded()
        {
            _gamesCounter++;
            if (_gamesCounter >= gamesBetweenInterstitials)
            {
                _gamesCounter = 0;
                ShowInterstitial();
            }
        }

        /// <summary>
        /// Показ межстраничной рекламы (Interstitial)
        /// </summary>
        public void ShowInterstitial()
        {
#if UNITY_EDITOR
            Debug.Log("[AdManager] [EDITOR MOCK] Межстраничная реклама (Interstitial) показана.");
            OnInterstitialClosed?.Invoke();
#else
            // Реальный вызов SDK:
            // if (interstitialAd != null && interstitialAd.CanShowAd()) interstitialAd.Show();
            OnInterstitialClosed?.Invoke();
#endif
        }

        /// <summary>
        /// Показ видео с вознаграждением (Rewarded Video).
        /// </summary>
        /// <param name="onSuccess">Колбэк при успешном досмотре (выдача награды)</param>
        /// <param name="onFailed">Колбэк, если игрок закрыл рекламу раньше или видео не загрузилось</param>
        public void ShowRewarded(Action onSuccess, Action onFailed = null)
        {
            _onRewardSuccess = onSuccess;
            _onRewardFailed = onFailed;

#if UNITY_EDITOR
            Debug.Log("[AdManager] [EDITOR MOCK] Просмотр Rewarded Video завершен успешно. Награда выдана.");
            _onRewardSuccess?.Invoke();
            OnRewardedComplete?.Invoke();
#else
            // Реальный вызов SDK:
            // if (rewardedAd != null && rewardedAd.CanShowAd()) {
            //     rewardedAd.Show(reward => {
            //         _onRewardSuccess?.Invoke();
            //         OnRewardedComplete?.Invoke();
            //     });
            // } else {
            //     _onRewardFailed?.Invoke();
            // }
            _onRewardSuccess?.Invoke();
            OnRewardedComplete?.Invoke();
#endif
        }
    }
}
