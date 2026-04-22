using UnityEngine;

public class TakebleObject : ClickableObject
{
    public bool destroyAfterClick;
    public ItemSO itemInfo;
    public override bool OnClick()
    {
        if(!base.OnClick()) return false;
        if(destroyAfterClick)
        {
            Destroy(gameObject);
        }
        Inventory.Instance.AddItem(itemInfo);
        return true;
    }
}
