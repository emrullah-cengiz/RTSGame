using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

public class InteractableObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, EnumToggleButtons] private PointerInteractionType _activeInteractionTypesBitmask;
    private IInteractable _obj;

    ///TODO: PointerEnter cooldown 
    
    private void Start()
    {
        if(!TryGetComponent<IInteractable>(out _obj))
            Debug.LogError($"{nameof(InteractableObject)} has not IInteractable component!");
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsValid(eventData, PointerEventType.Click, out var type))
            return;
        
        Event.OnMouseInteraction.Invoke(_obj, type);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsValid(eventData, PointerEventType.Down, out var type))
            return;
        
        Event.OnMouseInteraction.Invoke(_obj, type);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsValid(eventData, PointerEventType.Up, out var type))
            return;
        
        Event.OnMouseInteraction.Invoke(_obj, type);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!BitMaskCheck(PointerInteractionType.Enter))
            return;
        
        Event.OnMouseInteraction.Invoke(_obj, PointerInteractionType.Enter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!BitMaskCheck(PointerInteractionType.Exit))
            return;
        
        Event.OnMouseInteraction.Invoke(_obj, PointerInteractionType.Exit);
    }

    private bool IsValid(PointerEventData eventData, PointerEventType type, out PointerInteractionType pointerInteractionType)
    {
        pointerInteractionType = (eventData.button, type) switch
        {
            (PointerEventData.InputButton.Left, PointerEventType.Click) => PointerInteractionType.LeftDown,
            (PointerEventData.InputButton.Left, PointerEventType.Down) => PointerInteractionType.LeftDown,
            (PointerEventData.InputButton.Left, PointerEventType.Up) => PointerInteractionType.LeftUp,
            (PointerEventData.InputButton.Right, PointerEventType.Click) => PointerInteractionType.RightDown,
            (PointerEventData.InputButton.Right, PointerEventType.Down) => PointerInteractionType.RightDown,
            (PointerEventData.InputButton.Right, PointerEventType.Up) => PointerInteractionType.RightUp,
            (PointerEventData.InputButton.Middle, PointerEventType.Click) => PointerInteractionType.WheelDown,
            (PointerEventData.InputButton.Middle, PointerEventType.Down) => PointerInteractionType.WheelDown,
            (PointerEventData.InputButton.Middle, PointerEventType.Up) => PointerInteractionType.WheelUp,
            _ => throw new ArgumentOutOfRangeException()
        };

        return BitMaskCheck(pointerInteractionType);
    }

    private bool BitMaskCheck(PointerInteractionType pointerInteractionType) => 
        (_activeInteractionTypesBitmask & pointerInteractionType) != 0;

}
