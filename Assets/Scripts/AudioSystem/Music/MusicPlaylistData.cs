using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Плейлист для сцены. Содержит список треков с разными условиями воспроизведения
/// </summary>
[CreateAssetMenu(fileName = "NewMusicPlaylist", menuName = "Sanatorium/Audio/Music Playlist")]
public class MusicPlaylistData : ScriptableObject
{
    [Header("Информация о плейлисте")]
    [Tooltip("Название плейлиста")]
    public string playlistName = "New Playlist";
    
    [Tooltip("Описание (для разработчиков)")]
    [TextArea(2, 5)]
    public string description;
    
    [Header("Треки плейлиста")]
    [Tooltip("Список треков в плейлисте")]
    public List<MusicTrackData> tracks = new List<MusicTrackData>();
    
    [Header("Настройки плейлиста")]
    [Tooltip("Автоматически переключать треки по рассудку")]
    public bool autoSwitchBySanity = true;
    
    [Tooltip("Минимальная задержка между переключениями треков (секунды)")]
    [Range(0f, 5f)]
    public float switchCooldown = 0.5f;
    
    [Tooltip("Воспроизводить в случайном порядке")]
    public bool shuffle = false;
    
    /// <summary>
    /// Получить трек, подходящий под текущий уровень рассудка
    /// </summary>
    public MusicTrackData GetTrackForSanity(float sanity, bool isInMadness)
    {
        MusicTrackData bestTrack = null;
        int bestPriority = -1;
        
        foreach (var track in tracks)
        {
            if (track == null || track.audioClip == null)
                continue;
            
            // Проверка режима безумия
            if (track.madnessOnly && !isInMadness)
                continue;
            if (track.normalOnly && isInMadness)
                continue;
            
            // Проверка порога рассудка
            if (sanity < track.minSanityThreshold || sanity > track.maxSanityThreshold)
                continue;
            
            // Выбор трека с наивысшим приоритетом
            if (track.priority > bestPriority)
            {
                bestTrack = track;
                bestPriority = track.priority;
            }
        }
        
        return bestTrack;
    }
}