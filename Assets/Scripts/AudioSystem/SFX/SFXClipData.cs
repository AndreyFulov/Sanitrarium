using UnityEngine;

/// <summary>
/// Данные звукового эффекта для системы SFX.
/// Позволяет создавать переиспользуемые ассеты звуков.
/// </summary>
[CreateAssetMenu(fileName = "NewSFXClip", menuName = "Sanatorium/Audio/SFX Clip")]
public class SFXClipData : ScriptableObject
{
    [Header("Основная информация")]
    [Tooltip("Название звука (для отладки)")]
    public string clipName = "New SFX";
    
    [Tooltip("Аудиоклип")]
    public AudioClip audioClip;
    
    [Header("Настройки воспроизведения")]
    [Tooltip("Громкость звука (0-1)")]
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Tooltip("Питч (вариация для естественности)")]
    [Range(0.5f, 1.5f)]
    public float pitchVariation = 0.1f;
    
    [Tooltip("3D звук или 2D")]
    public bool is3D = false;
    
    [Tooltip("Минимальная дистанция для 3D звука")]
    public float minDistance = 1f;
    
    [Tooltip("Максимальная дистанция для 3D звука")]
    public float maxDistance = 50f;
    
    [Header("Категория")]
    [Tooltip("Категория звука для групповой настройки громкости")]
    public SFXCategory category = SFXCategory.Interaction;
    
    [Tooltip("Приоритет (выше = важнее при лимите источников)")]
    [Range(1, 10)]
    public int priority = 5;
}

/// <summary>
/// Категории звуков для группового управления громкостью
/// </summary>
public enum SFXCategory
{
    UI,           // Кнопки, меню, инвентарь
    Interaction,  // Подбор предметов, рычаги, двери
    Madness,      // Звуки безумия, шепот, галлюцинации
    Environment,  // Шаги, скрипы, окружение
    Critical      // Скримеры, важные сюжетные звуки
}