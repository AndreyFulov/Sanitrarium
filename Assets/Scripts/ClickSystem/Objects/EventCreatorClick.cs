using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventCreatorClick : ClickableObject
{
    public UnityEvent _event;
    public override bool OnClick()
    {
        if(!base.OnClick()) return false;
        _event.Invoke();
        return true;
    }
}
