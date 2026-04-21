using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Глобальный менеджер музыки
/// Работает в связке с MadnessManager через события
/// Автоматически переключает треки в зависимости от уровня рассудка
/// </summary>
public class MusicManager : MonoBehaviour
{
    #region Singleton
    public static MusicManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    
    #region Inspector Fields
    [Header("Настройки аудио")]
    [Tooltip("Аудиоисточник для музыки")]
    public AudioSource musicSource;
    
    [Tooltip("Второй аудиоисточник для плавного перехода (crossfade)")]
    public AudioSource musicSource2;
    
    [Header("Плейлист по умолчанию")]
    [Tooltip("Плейлист, который воспроизводится при старте")]
    public MusicPlaylistData defaultPlaylist;
    
    [Header("Настройки перехода")]
    [Tooltip("Глобальная громкость музыки (0-1)")]
    [Range(0f, 1f)]
    public float masterVolume = 0.7f;
    
    [Tooltip("Время плавного перехода между треками (секунды)")]
    [Range(0.1f, 3f)]
    public float crossfadeDuration = 1f;
    #endregion
    
    #region Private Fields
    private MusicPlaylistData currentPlaylist;
    private MusicTrackData currentTrack;
    private MusicTrackData nextTrack;
    private bool isCrossfading = false;
    private float lastSwitchTime = 0f;
    private bool useTwoSources = false;
    #endregion
    
    #region Properties
    /// <summary>
    /// Текущий уровень рассудка (получается из MadnessManager)
    /// </summary>
    private float CurrentSanity => MadnessManager.Instance?.CurrentSanity ?? 100f;
    
    /// <summary>
    /// Текущее состояние безумия
    /// </summary>
    private bool IsInMadness => MadnessManager.Instance?.IsInMadness ?? false;
    #endregion
    
    #region Lifecycle
    private void Start()
    {
        // Инициализация аудиоисточников
        InitializeAudioSources();
        
        // Подписка на события MadnessManager
        SubscribeToEvents();
        
        // Запуск плейлиста по умолчанию
        if (defaultPlaylist != null)
        {
            SetPlaylist(defaultPlaylist);
        }
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion
    
    #region Initialization
    private void InitializeAudioSources()
    {
        // Создаём первый источник если не задан
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
        
        // Создаём второй источник для crossfade если не задан
        if (musicSource2 == null)
        {
            musicSource2 = gameObject.AddComponent<AudioSource>();
            musicSource2.playOnAwake = false;
            musicSource2.loop = true;
            useTwoSources = true;
        }
        else
        {
            useTwoSources = true;
        }
        
        // Применяем громкость
        musicSource.volume = masterVolume;
        musicSource2.volume = 0f;
    }
    
    private void SubscribeToEvents()
    {
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnSanityChanged += OnSanityChanged;
            MadnessManager.Instance.OnMadnessEntered += OnMadnessEntered;
            MadnessManager.Instance.OnMadnessExited += OnMadnessExited;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnSanityChanged -= OnSanityChanged;
            MadnessManager.Instance.OnMadnessEntered -= OnMadnessEntered;
            MadnessManager.Instance.OnMadnessExited -= OnMadnessExited;
        }
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Установить новый плейлист для текущей сцены
    /// </summary>
    public void SetPlaylist(MusicPlaylistData playlist)
    {
        if (playlist == null)
        {
            Debug.LogWarning("[MusicManager] Плейлист не задан!");
            return;
        }
        
        currentPlaylist = playlist;
        Debug.Log($"[MusicManager] Установлен плейлист: {playlist.playlistName}");
        
        // Немедленно подобрать трек под текущий рассудок
        UpdateTrackForCurrentSanity();
    }
    
    /// <summary>
    /// Добавить трек в текущий плейлист
    /// </summary>
    public void AddTrackToPlaylist(MusicTrackData track)
    {
        if (currentPlaylist == null)
        {
            Debug.LogWarning("[MusicManager] Нет активного плейлиста!");
            return;
        }
        
        if (!currentPlaylist.tracks.Contains(track))
        {
            currentPlaylist.tracks.Add(track);
        }
    }
    
    /// <summary>
    /// Воспроизвести конкретный трек (игнорируя рассудок)
    /// </summary>
    public void PlayTrack(MusicTrackData track, bool force = false)
    {
        if (track == null || track.audioClip == null)
        {
            Debug.LogWarning("[MusicManager] Трек или аудиофайл не заданы!");
            return;
        }
        
        StartCoroutine(PlayTrackCoroutine(track, force));
    }
    
    /// <summary>
    /// Остановить музыку
    /// </summary>
    public void StopMusic()
    {
        StopAllCoroutines();
        musicSource.Stop();
        musicSource2.Stop();
        currentTrack = null;
    }
    
    /// <summary>
    /// Установить громкость музыки
    /// </summary>
    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        musicSource.volume = masterVolume;
        musicSource2.volume = masterVolume;
    }
    
    /// <summary>
    /// Принудительно обновить трек по текущему рассудку
    /// </summary>
    public void ForceUpdateTrack()
    {
        UpdateTrackForCurrentSanity();
    }
    #endregion
    
    #region Event Handlers
    /// <summary>
    /// Вызывается при изменении рассудка
    /// </summary>
    private void OnSanityChanged(float newSanity)
    {
        if (currentPlaylist == null || !currentPlaylist.autoSwitchBySanity)
            return;
        
        // Проверка кулдауна переключения
        if (Time.time - lastSwitchTime < currentPlaylist.switchCooldown)
            return;
        
        // Проверка, нужен ли новый трек
        UpdateTrackForCurrentSanity();
    }
    
    /// <summary>
    /// Вызывается при входе в состояние безумия
    /// </summary>
    private void OnMadnessEntered()
    {
        Debug.Log("[MusicManager] Вход в безумие — переключение музыки");
        UpdateTrackForCurrentSanity();
    }
    
    /// <summary>
    /// Вызывается при выходе из состояния безумия
    /// </summary>
    private void OnMadnessExited()
    {
        Debug.Log("[MusicManager] Выход из безумия — переключение музыки");
        UpdateTrackForCurrentSanity();
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Подобрать и воспроизвести трек для текущего уровня рассудка
    /// </summary>
    private void UpdateTrackForCurrentSanity()
    {
        if (currentPlaylist == null)
            return;
        
        MusicTrackData newTrack = currentPlaylist.GetTrackForSanity(CurrentSanity, IsInMadness);
        
        if (newTrack == null)
        {
            Debug.LogWarning("[MusicManager] Не найдено трека для текущего рассудка!");
            return;
        }
        
        // Если трек не изменился — ничего не делаем
        if (newTrack == currentTrack)
            return;
        
        // Переключаем трек
        PlayTrack(newTrack, false);
        lastSwitchTime = Time.time;
    }
    
    /// <summary>
    /// Корутина для плавного переключения треков (crossfade)
    /// </summary>
    private IEnumerator PlayTrackCoroutine(MusicTrackData track, bool force)
    {
        if (isCrossfading && !force)
            yield break;
        
        isCrossfading = true;
        nextTrack = track;
        
        // Если используем два источника для crossfade
        if (useTwoSources && currentTrack != null)
        {
            // Определяем какой источник сейчас активен
            AudioSource activeSource = musicSource.volume > musicSource2.volume ? musicSource : musicSource2;
            AudioSource inactiveSource = activeSource == musicSource ? musicSource2 : musicSource;
            
            // Запускаем новый трек на неактивном источнике
            inactiveSource.clip = track.audioClip;
            inactiveSource.volume = 0f;
            inactiveSource.Play();
            
            // Плавный переход
            float elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;
                
                activeSource.volume = masterVolume * (1 - t);
                inactiveSource.volume = masterVolume * t;
                
                yield return null;
            }
            
            // Останавливаем старый источник
            activeSource.Stop();
            activeSource.volume = 0f;
            
            // Обновляем текущий трек
            currentTrack = track;
        }
        else
        {
            // Простое переключение без crossfade
            musicSource.clip = track.audioClip;
            musicSource.volume = masterVolume * track.volume;
            musicSource.Play();
            
            currentTrack = track;
        }
        
        isCrossfading = false;
        
        Debug.Log($"[MusicManager] Воспроизводится трек: {track.trackName} (Sanity: {CurrentSanity:F0})");
    }
    #endregion
    
    #region Debug
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(270, 10, 250, 150));
        GUILayout.Box("Music Manager (DEBUG)");
        
        GUILayout.Label($"Текущий трек: {currentTrack?.trackName ?? "Нет"}");
        GUILayout.Label($"Рассудок: {CurrentSanity:F0}");
        GUILayout.Label($"Безумие: {(IsInMadness ? "Да" : "Нет")}");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Обновить трек"))
            ForceUpdateTrack();
        
        if (GUILayout.Button("Остановить"))
            StopMusic();
        
        GUILayout.EndArea();
    }
    #endregion
}