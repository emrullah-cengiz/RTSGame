using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Event
{
    public static Action<IInteractable, PointerInteractionType> OnMouseInteraction;
}