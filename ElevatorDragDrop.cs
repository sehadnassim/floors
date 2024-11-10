using UnityEngine;
using UnityEngine.EventSystems;

public class ElevatorDragDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 offset;
    private bool isDragging = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Called when the user taps or clicks on the elevator
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        // Calculate the offset from the click position to the object's position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, screenPosition.z);
        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePosition);
    }

    // Called while the user drags the elevator
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
            Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition) + offset;

            // Move the elevator to the new position
            transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);
        }
    }

    // Called when the user releases the elevator
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}