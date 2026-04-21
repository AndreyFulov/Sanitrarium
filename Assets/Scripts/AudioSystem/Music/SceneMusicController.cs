using UnityEngine;

/// <summary>
/// Контроллер музыки сцены
/// Вешается на пустой GameObject в каждой сцене
/// При загрузке сцены автоматически устанавливает нужный плейлист в MusicManager
/// </summary>
public class SceneMusicController : MonoBehaviour
{
    [Header("Настройки сцены")]
    [Tooltip("Плейлист, который должен играть на этой сцене")]
    public MusicPlaylistData scenePlaylist;
    
    [Tooltip("Переключать музыку сразу при загрузке")]
    public bool switchOnLoad = true;
    
    [Tooltip("Задержка перед переключением (секунды)")]
    [Range(0f, 3f)]
    public float loadDelay = 0.5f;
    
    [Tooltip("Плавность перехода между плейлистами (секунды)")]
    [Range(0f, 2f)]
    public float crossfadeOnLoad = 1f;
    
    private static SceneMusicController _currentInstance;
    
    private void Awake()
    {
        // Защита от множественных контроллеров
        if (_currentInstance != null && _currentInstance != this)
        {
            Debug.LogWarning("[SceneMusicController] Обнаружен дубликат на сцене! Удаление.");
            Destroy(gameObject);
            return;
        }
        
        _currentInstance = this;
        
        // Не уничтожать объект до завершения загрузки
        DontDestroyOnLoad(gameObject);
        
        if (switchOnLoad)
        {
            Invoke(nameof(ApplyPlaylist), loadDelay);
        }
    }
    
    private void ApplyPlaylist()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogError("[SceneMusicController] MusicManager не найден!");
            return;
        }
        
        if (scenePlaylist == null)
        {
            Debug.LogWarning($"[SceneMusicController] На сцене {gameObject.scene.name} не задан плейлист!");
            return;
        }
        
        Debug.Log($"[SceneMusicController] Переключение плейлиста: {scenePlaylist.playlistName}");
        MusicManager.Instance.SetPlaylist(scenePlaylist);
        
        // Очищаем ссылку после применения
        _currentInstance = null;
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        // Сброс ссылки при уничтожении
        if (_currentInstance == this)
            _currentInstance = null;
    }
}