using UnityEngine;

public interface IInteractable
{
    public InteractableType InteractableType { get; }
    public MonoBehaviour Obj { get; }
}