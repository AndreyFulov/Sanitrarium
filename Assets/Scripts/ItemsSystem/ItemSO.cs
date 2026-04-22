using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [Tooltip("Уникальный ID")]
    public string itemId;
    public string displayName;
    public Sprite icon;
    
    [Tooltip("Можно ли выбросить предмет")]
    public bool isDroppable = true;
    
    [Tooltip("Размер в клетках (1x1 по умолчанию)")]
    public Vector2Int gridSize = Vector2Int.one;
}