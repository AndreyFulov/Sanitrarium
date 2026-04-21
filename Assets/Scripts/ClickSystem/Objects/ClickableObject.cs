using System.Collections;
using UnityEngine;

public abstract class ClickableObject : MonoBehaviour
{
    public virtual void OnClick()
    {
        StartCoroutine(ChangeColor());
        Debug.Log(this.gameObject.name + " was clicked!");
    }
    IEnumerator ChangeColor()
    {
        Color baseColor = this.gameObject.GetComponent<SpriteRenderer>().color;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(.3f);
        this.gameObject.GetComponent<SpriteRenderer>().color = baseColor;
    }
}
