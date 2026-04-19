using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventCreatorClick : ClickableObject
{
    public UnityEvent _event;
    public override void OnClick()
    {
        _event.Invoke();
        base.OnClick();
    }
}
