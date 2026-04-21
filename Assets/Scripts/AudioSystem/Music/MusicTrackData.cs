using UnityEngine;

/// <summary>
/// Данные музыкального трека для системы безумия
/// Позволяет привязать трек к определённому диапазону рассудка
/// </summary>
[CreateAssetMenu(fileName = "NewMusicTrack", menuName = "Sanatorium/Audio/Music Track")]
public class MusicTrackData : ScriptableObject
{
    [Header("Основная информация")]
    [Tooltip("Название трека (для отладки)")]
    public string trackName = "New Track";
    
    [Tooltip("Аудиоклип")]
    public AudioClip audioClip;
    
    [Header("Привязка к рассудку")]
    [Tooltip("Минимальный порог рассудка для этого трека (0-100)")]
    [Range(0, 100)]
    public int minSanityThreshold = 0;
    
    [Tooltip("Максимальный порог рассудка для этого трека (0-100)")]
    [Range(0, 100)]
    public int maxSanityThreshold = 100;
    
    [Tooltip("Приоритет трека (выше = важнее при перекрытии диапазонов)")]
    [Range(1, 10)]
    public int priority = 5;
    
    [Header("Настройки воспроизведения")]
    [Tooltip("Громкость трека (0-1)")]
    [Range(0f, 1f)]
    public float volume = 1f;
    
    [Tooltip("Зацикливание трека")]
    public bool loop = true;
    
    [Tooltip("Плавное затухание при переключении (секунды)")]
    [Range(0.1f, 5f)]
    public float fadeDuration = 1f;
    
    [Header("Состояние безумия")]
    [Tooltip("Этот трек играет только в состоянии безумия")]
    public bool madnessOnly = false;
    
    [Tooltip("Этот трек играет только в нормальном состоянии")]
    public bool normalOnly = false;
}