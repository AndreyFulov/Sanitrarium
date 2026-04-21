using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContextMenuManager : MonoBehaviour
{
    public static ContextMenuManager Instance { get; private set; }

    public Canvas menuCanvas;
    public RectTransform menuTransform;
    public Button btnTake, btnDrop;
    public GameObject overlay; // прозрачный фон для закрытия по клику вне

    private int _currentSlotIndex = -1;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Hide();
    }

    public void ShowAt(Vector2 screenPosition, int slotIndex)
    {
        _currentSlotIndex = slotIndex;
        var slot = Inventory.Instance.GetSlot(slotIndex);
        if (slot.IsEmpty) return;

        menuTransform.position = screenPosition;
        btnTake.gameObject.SetActive(true);
        btnDrop.gameObject.SetActive(slot.item.isDroppable);

        // Привязка действий
        btnTake.onClick.RemoveAllListeners();
        btnTake.onClick.AddListener(() => TakeToHand());

        btnDrop.onClick.RemoveAllListeners();
        btnDrop.onClick.AddListener(() => DropItem());

        Show();
    }

    private void TakeToHand()
    {
        // Здесь вызываете внешнюю систему: PlayerHand.Equip(slotIndex);
        Debug.Log($"[ContextMenu] Взять в руку: слот {_currentSlotIndex}");
        Hide();
    }

    private void DropItem()
    {
        Inventory.Instance.RemoveItem(_currentSlotIndex);
        // Здесь спавните предмет в сцене: WorldItem.Spawn(...);
        Debug.Log($"[ContextMenu] Выбросить из слота {_currentSlotIndex}");
        Hide();
    }

    private void Show()
    {
        menuCanvas.gameObject.SetActive(true);
        overlay.SetActive(true);
    }

    public void Hide()
    {
        menuCanvas.gameObject.SetActive(false);
        overlay.SetActive(false);
    }

    // Закрытие при клике на оверлей
    public void OnOverlayClick() => Hide();
}