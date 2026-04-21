using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;

public abstract class ClickableObject : MonoBehaviour
{
    [Header("Предмет, который необходим для взаимодействия")]
    [SerializeField] private bool needToTake = false;
    [SerializeField] private ItemSO item;
    
    public virtual bool OnClick()
    {
        if(item != null)
        {
            if(Inventory.Instance.GetSlotByItemInfo(item) == -1){
                Inventory.Instance.noItem.Show("Нет предмета!");
                return false;
            }
            if(needToTake)
            {
                bool success = Inventory.Instance.RemoveItemByInfo(item);
                if(!success) return false;
            }
        }
        StartCoroutine(ChangeColor());
        Debug.Log(this.gameObject.name + " was clicked!");
        return true;
    }
    IEnumerator ChangeColor()
    {
        Color baseColor = this.gameObject.GetComponent<SpriteRenderer>().color;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(.3f);
        this.gameObject.GetComponent<SpriteRenderer>().color = baseColor;
    }
}