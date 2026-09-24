# Чеклист релиза гиперказуальной игры: Google Play и Steam

Полный алгоритм подготовки сборки для пассивного заработка с минимальным последующим обслуживанием.

---

## Часть 1: Google Play Store (Android)

### 1. Настройки проекта в Unity (`Project Settings -> Player`)
* **Company Name & Product Name**: уникальные названия (например, `MyStudio` и `CubeRunner3D`).
* **Package Name**: формат `com.mystudio.cuberunner3d` (нельзя изменить после первой загрузки в Play Console!).
* **Version & Bundle Version Code**:
  * `Version`: `1.0.0`
  * `Bundle Version Code`: `1` (при каждом новом обновлении увеличивать на 1).
* **Minimum API Level**: Android 9.0 (API level 28) или 10.0 (API level 29).
* **Target API Level**: Android 14 (API level 34) или выше (требование Google Play).
* **Scripting Backend**: обязательно **IL2CPP** (Mono не поддерживает 64-битные требования Google).
* **Target Architectures**: отметить галочками **ARMv7** и **ARM64**.

### 2. Создание Android Keystore (Ключ подписи)
> [!IMPORTANT]
> Потеря файла keystore или пароля означает невозможность обновить игру в Google Play навсегда! Сделайте резервную копию ключа в облако.

1. Откройте `Player Settings -> Publishing Settings -> Keystore Manager`.
2. Нажмите `Keystore... -> Create New -> Anywhere...`.
3. Сохраните файл `user.keystore` в надежное место вне репозитория.
4. Заполните пароль (минимум 8 символов), Alias (`upload_key`), пароль алиаса и срок действия (не менее 25-30 лет).
5. Нажмите `Add Key`.

### 3. Сборка Android App Bundle (.aab)
1. Откройте `File -> Build Settings...`.
2. Выберите платформу **Android** и нажмите `Switch Platform`.
3. Установите галочку **Build App Bundle (Google Play)**.
4. Убедитесь, что `Development Build` выключен.
5. Нажмите **Build** и сохраните файл `game_v1.0.0.aab`.

### 4. Документы и Play Console
* **Политика конфиденциальности (Privacy Policy)**: Обязательна из-за наличия рекламы AdMob. Сгенерируйте за 2 минуты через бесплатный генератор (например, `privacypolicies.com` или `app-privacy-policy-generator`) и разместите на GitHub Pages / Notion.
* **Безопасность данных (Data Safety)**: Укажите, что приложение собирает данные об устройстве и диагностику сбоев для рекламных целей (Google Mobile Ads).
* **Рекламный идентификатор (Advertising ID)**: В анкете Play Console укажите: *Да, приложение использует рекламный идентификатор для показа рекламы*.

---

## Часть 2: Steam (Windows PC)

### 1. Подготовка билда Unity под Steam
* **Платформа**: `Standalone -> Windows -> Architecture: x86_64`.
* **Full Screen Mode**: `Exclusive Fullscreen` или `Fullscreen Window`.
* **Разрешения**: Поддержка 16:9 (1920x1080, 2560x1440).
* **Управление**: Добавьте поддержку геймпада (Input System) и паузу по клавише `Escape`.

### 2. Интеграция Steamworks.NET (Опционально, но рекомендуется)
1. Скачайте пакет `Steamworks.NET.unitypackage` с официального GitHub репозитория.
2. В коде инициализации:
   ```csharp
   using Steamworks;
   using UnityEngine;

   public class SteamInitializer : MonoBehaviour
   {
       private void Awake()
       {
           if (!SteamManager.Initialized)
           {
               Debug.LogWarning("[Steam] Steamworks не запущен. Убедитесь, что Steam клиент открыт.");
               return;
           }

           string personaName = SteamFriends.GetPersonaName();
           Debug.Log($"[Steam] Привет, {personaName}!");
       }

       // Выдача ачивки одной строкой:
       public static void UnlockAchievement(string achId)
       {
           if (SteamManager.Initialized)
           {
               SteamUserStats.SetAchievement(achId);
               SteamUserStats.StoreStats();
           }
       }
   }
   ```
3. Создайте файл `steam_appid.txt` в папке проекта рядом со скриптами с ID вашей игры в Steamworks (для теста можно использовать `480` — Spacewar).

### 3. Депоты и загрузка в SteamPipe
* Используйте утилиту `steamcmd` или GUI-инструмент `SteamPipeGUI` для автоматической загрузки папки билда на сервера Valve.
