using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Глобальный менеджер звуковых эффектов.
/// Управляет воспроизведением SFX через пул аудиоисточников.
/// Интегрирован с MadnessManager для звуков безумия.
/// </summary>
public class SFXManager : MonoBehaviour
{
    #region Singleton
    public static SFXManager Instance { get; private set; }
    
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
    [Header("Настройки пула")]
    [Tooltip("Префаб пула аудиоисточников")]
    public SFXPool sfxPool;
    
    [Tooltip("Размер пула по умолчанию (если префаб не задан)")]
    public int defaultPoolSize = 30;
    
    [Header("Громкость по категориям")]
    [Range(0f, 1f)]
    public float uiVolume = 0.8f;
    
    [Range(0f, 1f)]
    public float interactionVolume = 1f;
    
    [Range(0f, 1f)]
    public float madnessVolume = 0.7f;
    
    [Range(0f, 1f)]
    public float environmentVolume = 0.9f;
    
    [Range(0f, 1f)]
    public float criticalVolume = 1f;
    
    [Header("Настройки безумия")]
    [Tooltip("Добавлять искажения к звукам в режиме безумия")]
    public bool applyMadnessEffects = true;
    
    [Tooltip("Дополнительный питч в режиме безумия")]
    [Range(0.5f, 1.5f)]
    public float madnessPitchModifier = 0.8f;
    #endregion
    
    #region Private Fields
    private Dictionary<SFXCategory, float> categoryVolumes;
    private bool isInMadness = false;
    #endregion
    
    #region Lifecycle
    private void Start()
    {
        InitializeCategoryVolumes();
        InitializePool();
        SubscribeToEvents();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion
    
    #region Initialization
    private void InitializeCategoryVolumes()
    {
        categoryVolumes = new Dictionary<SFXCategory, float>
        {
            { SFXCategory.UI, uiVolume },
            { SFXCategory.Interaction, interactionVolume },
            { SFXCategory.Madness, madnessVolume },
            { SFXCategory.Environment, environmentVolume },
            { SFXCategory.Critical, criticalVolume }
        };
    }
    
    private void InitializePool()
    {
        if (sfxPool == null)
        {
            // Создаём пул программно если префаб не задан
            GameObject poolObj = new GameObject("SFXPool");
            poolObj.transform.SetParent(transform);
            sfxPool = poolObj.AddComponent<SFXPool>();
            sfxPool.poolSize = defaultPoolSize;
        }
    }
    
    private void SubscribeToEvents()
    {
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnMadnessEntered += OnMadnessEntered;
            MadnessManager.Instance.OnMadnessExited += OnMadnessExited;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnMadnessEntered -= OnMadnessEntered;
            MadnessManager.Instance.OnMadnessExited -= OnMadnessExited;
        }
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Воспроизвести звук через ScriptableObject
    /// </summary>
    public void PlaySFX(SFXClipData clipData, Vector3? position = null)
    {
        if (clipData == null || clipData.audioClip == null)
        {
            Debug.LogWarning("[SFXManager] Попытка воспроизвести пустой звук!");
            return;
        }
        
        AudioSource source = sfxPool.GetSource();
        
        // Настройка источника
        source.clip = clipData.audioClip;
        source.volume = GetCategoryVolume(clipData.category) * clipData.volume;
        source.pitch = GetPitch(clipData);
        source.spatialBlend = clipData.is3D ? 1f : 0f;
        source.minDistance = clipData.minDistance;
        source.maxDistance = clipData.maxDistance;
        source.priority = clipData.priority;
        
        // Позиция
        if (position.HasValue)
        {
            source.transform.position = position.Value;
        }
        
        // Воспроизведение
        source.Play();
        
        // Возврат в пул после завершения
        StartCoroutine(ReturnToPoolAfterPlay(source, clipData.audioClip.length));
        
        Debug.Log($"[SFXManager] Воспроизведён: {clipData.clipName}");
    }
    
    /// <summary>
    /// Воспроизвести звук по категории (быстрый вызов для UI)
    /// </summary>
    public void PlaySFX(SFXCategory category, AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        
        AudioSource source = sfxPool.GetSource();
        source.clip = clip;
        source.volume = GetCategoryVolume(category) * volume;
        source.pitch = 1f + Random.Range(-0.05f, 0.05f); // Небольшая вариация
        source.spatialBlend = 0f; // UI звуки всегда 2D
        source.Play();
        
        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }
    
    /// <summary>
    /// Остановить все звуки категории
    /// </summary>
    public void StopCategory(SFXCategory category)
    {
        // Можно расширить для остановки конкретных категорий
        Debug.Log($"[SFXManager] Остановка категории: {category}");
    }
    
    /// <summary>
    /// Установить громкость категории
    /// </summary>
    public void SetCategoryVolume(SFXCategory category, float volume)
    {
        volume = Mathf.Clamp01(volume);
        
        if (categoryVolumes.ContainsKey(category))
        {
            categoryVolumes[category] = volume;
            
            // Обновляем соответствующее поле
            switch (category)
            {
                case SFXCategory.UI: uiVolume = volume; break;
                case SFXCategory.Interaction: interactionVolume = volume; break;
                case SFXCategory.Madness: madnessVolume = volume; break;
                case SFXCategory.Environment: environmentVolume = volume; break;
                case SFXCategory.Critical: criticalVolume = volume; break;
            }
        }
    }
    
    /// <summary>
    /// Установить глобальную громкость всех SFX
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        
        uiVolume = volume;
        interactionVolume = volume;
        madnessVolume = volume;
        environmentVolume = volume;
        criticalVolume = volume;
        
        InitializeCategoryVolumes();
    }
    #endregion
    
    #region Event Handlers
    private void OnMadnessEntered()
    {
        isInMadness = true;
        Debug.Log("[SFXManager] Вход в безумие — активация эффектов");
    }
    
    private void OnMadnessExited()
    {
        isInMadness = false;
        Debug.Log("[SFXManager] Выход из безумия — отключение эффектов");
    }
    #endregion
    
    #region Private Methods
    private float GetCategoryVolume(SFXCategory category)
    {
        if (categoryVolumes.TryGetValue(category, out float volume))
        {
            return volume;
        }
        return 1f;
    }
    
    private float GetPitch(SFXClipData clipData)
    {
        float basePitch = 1f;
        
        // Вариация для естественности
        if (clipData.pitchVariation > 0)
        {
            basePitch += Random.Range(-clipData.pitchVariation, clipData.pitchVariation);
        }
        
        // Модификатор безумия
        if (isInMadness && applyMadnessEffects && clipData.category != SFXCategory.Critical)
        {
            basePitch *= madnessPitchModifier;
        }
        
        return Mathf.Clamp(basePitch, 0.5f, 1.5f);
    }
    
    private IEnumerator ReturnToPoolAfterPlay(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        sfxPool.ReturnSource(source);
    }
    #endregion
    
    #region Debug
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(530, 10, 250, 200));
        GUILayout.Box("SFX Manager (DEBUG)");
        
        GUILayout.Label($"Режим безумия: {(isInMadness ? "Да" : "Нет")}");
        GUILayout.Label($"Свободных источников: {sfxPool?.AvailableSourcesCount ?? 0}");
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("UI:");
        uiVolume = GUILayout.HorizontalSlider(uiVolume, 0f, 1f);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Interaction:");
        interactionVolume = GUILayout.HorizontalSlider(interactionVolume, 0f, 1f);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Madness:");
        madnessVolume = GUILayout.HorizontalSlider(madnessVolume, 0f, 1f);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Тест UI звука"))
            PlaySFX(SFXCategory.UI, null); // Нужен тестовый клип
        
        if (GUILayout.Button("Тест звука безумия"))
            PlaySFX(SFXCategory.Madness, null); // Нужен тестовый клип
        
        GUILayout.EndArea();
    }
    #endregion
}