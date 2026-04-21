using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Slot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    public Image iconImage;

    private void OnEnable()
    {
        // Мгновенное обновление при изменении конкретного слота
        Inventory.Instance.OnSlotChanged += HandleSlotChanged;
    }

    private void OnDisable()
    {
        Inventory.Instance.OnSlotChanged -= HandleSlotChanged;
    }

    private void HandleSlotChanged(int changedIndex, Inventory.SlotData data)
    {
        if (changedIndex == slotIndex)
            ForceRefresh();
    }

    // 🟢 Метод для полной перерисовки (вызывается при OnInventoryRefresh)
    public void ForceRefresh()
    {
        if (Inventory.Instance == null) return;

        var slot = Inventory.Instance.GetSlot(slotIndex);
        if (slot.item != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.gameObject.SetActive(true);
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        var slot = Inventory.Instance.GetSlot(slotIndex);
        if (!slot.IsEmpty)
        {
            ContextMenuManager.Instance.ShowAt(eventData.position, slotIndex);
        }
    }
}