using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class PlayerOrderController
{
    private readonly Dictionary<(InteractableType, PointerInteractionType), Type> _orderInteractionMapping;
    private readonly Dictionary<Type, IPlayerOrder> _orderTypeMapping;

    private readonly FormationDrawer _formationDrawer;
    private readonly FormationBuilder _formationBuilder;

    public PlayerOrderController()
    {
        //TODO: DI Resolve
        
        var playerFormationController = Object.FindFirstObjectByType<PlayerFormationController>();
        _formationDrawer = Object.FindFirstObjectByType<FormationDrawer>();
        _formationBuilder = new FormationBuilder();

        _orderTypeMapping = new Dictionary<Type, IPlayerOrder>
        {
            { typeof(TroopSelection_PlayerOrder), new TroopSelection_PlayerOrder(playerFormationController) },
            { typeof(MoveToDestination_PlayerOrder), new MoveToDestination_PlayerOrder(playerFormationController, _formationBuilder) },
        };

        _orderInteractionMapping = new()
        {
            ///TODO: DI Resolve
            // { (InteractableType.Unit, PointerInteractionType.LeftDown), typeof(TroopSelection_PlayerOrder)},
            // { (InteractableType.Unit, PointerInteractionType.RightDown), new AttackCommand() },
            // { (InteractableType.HidingPlace, PointerInteractionType.RightDown), new HideCommand() },
        };

        Event.OnMouseInteraction += OnMouseInteraction;
        _formationDrawer.OnFormationDrawEnd += OnFormationDrawEnd;
    }

    private void OnMouseInteraction(IInteractable interactable, PointerInteractionType type)
    {
        // Debug.Log($"Interacted by {type.ToString()} with {interactable}");
        
        if (_orderInteractionMapping.TryGetValue((interactable.InteractableType, type), out var orderType))
        {
            _orderTypeMapping[orderType].Execute(interactable);
        }
    }

    private void OnFormationDrawEnd(bool s, List<FormationData> formations)
    {
        if(!s) return;
        
        var moveOrder = (MoveToDestination_PlayerOrder)_orderTypeMapping[typeof(MoveToDestination_PlayerOrder)];
        moveOrder.Execute(formations);
    }
}

public interface IPlayerOrder
{
    void Execute(IInteractable obj);
}

public readonly struct TroopSelection_PlayerOrder : IPlayerOrder
{
    private readonly PlayerFormationController _playerFormationController;

    public TroopSelection_PlayerOrder(PlayerFormationController formationController)
    {
        _playerFormationController = formationController;
    }

    public void Execute(IInteractable obj)
    {
        if (_playerFormationController.State == PlayerFormationControllerState.FormationDrawing)
            return;

        var troop = ((Unit)obj).ParentTroop;

        _playerFormationController.SetTroopSelected(troop, Input.GetKey(KeyCode.LeftControl), true);
    }
}

public readonly struct MoveToDestination_PlayerOrder : IPlayerOrder
{
    private readonly PlayerFormationController _playerFormationController;
    private readonly FormationBuilder _formationBuilder;

    public MoveToDestination_PlayerOrder(PlayerFormationController formationController, FormationBuilder formationBuilder)
    {
        _playerFormationController = formationController;
        _formationBuilder = formationBuilder;
    }

    public void Execute(List<FormationData> formations) => SetFormationsToTroops(formations);

    private void SetFormationsToTroops(IEnumerable<FormationData> formations)
    {
        var i = 0;
        foreach (var formation in formations)
        {
            _playerFormationController.SelectedTroops[i].SetFormation(formation);
            i++;
        }
    }

    public void Execute(IInteractable obj)
    {
        Debug.LogError(typeof(MoveToDestination_PlayerOrder) + " has not implemented with Interaction Interface");
    }
    
    // public void Execute(IInteractable obj, PointerEventData pointerEventData)
    // {
    //     var formations = _formationBuilder.Build(_playerFormationController.SelectedTroops, pointerEventData.position);
    //
    //     SetFormationsToTroops(formations);
    // }

}



[System.Flags]
public enum PointerInteractionType
{
    None = 0,

    LeftClick = 1 << 1,
    LeftDown = 1 << 2,
    LeftUp = 1 << 3,

    RightClick = 1 << 4,
    RightDown = 1 << 5,
    RightUp = 1 << 6,

    WheelClick = 1 << 7,
    WheelDown = 1 << 8,
    WheelUp = 1 << 9,

    Enter = 1 << 10,
    Exit = 1 << 11
}

public enum PointerEventType
{
    Click,
    Down,
    Up,
    Enter,
    Exit
}

public enum InteractableType
{
    Unit,
    Map,
    HidingPlace,
    //..
}