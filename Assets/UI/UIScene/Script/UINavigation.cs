using UnityEngine;
using UnityEngine.EventSystems;

public class UINavigation : MonoBehaviour
{
    private EventSystem eventSystem;
    private GameObject selectedObject;
    private GameObject lastSelectedObject;

    void Start()
    {
        eventSystem = EventSystem.current;

        // Set the default selected object
        lastSelectedObject = eventSystem.firstSelectedGameObject;
    }

    void Update()
    {
        // Record last selected element
        if (eventSystem.currentSelectedGameObject)
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
        }
        
        // Return to last selected ui element 
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput != Vector2.zero)
        {
            eventSystem.SetSelectedGameObject(lastSelectedObject);
        }
        
    }
}