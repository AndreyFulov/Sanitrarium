using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{

    // Прикручиваю клик мыши к ивенту.
    public InputAction mouseClick;
    void OnEnable()
    {
        mouseClick.performed += OnMouseClick;
        mouseClick.Enable();
    }
    void OnDisable()
    {
        mouseClick.Disable();
        mouseClick.performed -= OnMouseClick;
    }
    private void OnMouseClick(InputAction.CallbackContext context)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if(hit.collider != null)
        {
            ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
            if(clickable != null)
            {
                clickable.OnClick();
            }
        }
    }
}
