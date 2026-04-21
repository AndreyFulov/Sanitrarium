using UnityEngine;

public class TakebleObject : ClickableObject
{
    public bool destroyAfterClick;
    public ItemSO itemInfo;
    public override void OnClick()
    {
        if(destroyAfterClick)
        {
            Destroy(gameObject);
        }
        Inventory.Instance.AddItem(itemInfo);
        base.OnClick();
    }
}
