using System;
using System.Collections.Generic;
using System.Linq;
using GAME.Utilities.StateMachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFormationController : MonoBehaviour
{
    public PlayerFormationControllerState State => _stateMachine.CurrentState;
    public List<Troop> SelectedTroops { get; private set; }

    private StateMachine<PlayerFormationControllerState> _stateMachine;

    private Camera _mainCam;
    private int _unitLayer;

    ///TODO: Inject 

    #region Injections

    private ArmyManager _armyManager;

    private SelectionBox _selectionBox;
    private FormationDrawer _formationDrawer;

    #endregion

    private void Awake()
    {
        _selectionBox = GetComponent<SelectionBox>();
        _formationDrawer = GetComponent<FormationDrawer>();
        _armyManager = GetComponent<ArmyManager>();

        _mainCam = Camera.main;
        _unitLayer = LayerMask.NameToLayer("Unit");

        SelectedTroops = new();
        _stateMachine = new();

        _stateMachine.AddState(PlayerFormationControllerState.Default, new DefaultState(_selectionBox, _formationDrawer, this));
        _stateMachine.AddState(PlayerFormationControllerState.TroopsSelected, new TroopsSelectedState(_selectionBox, _formationDrawer, this));
        _stateMachine.AddState(PlayerFormationControllerState.FormationDrawing, new FormationDrawingState(_selectionBox, _formationDrawer, this));
        _stateMachine.AddState(PlayerFormationControllerState.FormationDrawEnd, new FormationDrawEndState(_selectionBox, _formationDrawer, this));

        _stateMachine.Init();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void ChangeState(PlayerFormationControllerState state, params object[] @params) => _stateMachine.ChangeState(state, @params);

    #region Troop Selection / Deselection

    public void SetTroopSelected(Troop troop, bool additive, bool deselectIfSelected)
    {
        if (SelectedTroops.Contains(troop))
        {
            if (!deselectIfSelected) return;

            SelectedTroops.Remove(troop);
            troop.SetSelected(false);

            return;
        }

        if (!additive)
            ResetSelectedTroops();

        SelectedTroops.Add(troop);
        troop.SetSelected(true);
    }

    public bool SelectOverlappedTroops(Rect rect, Vector2 startPos, bool additive, bool deselectIfSelected)
    {
        if (!additive)
            ResetSelectedTroops();

        return rect.width < 0.1f || rect.height < 0.1f ? 
            HandleSingleSelection(startPos, deselectIfSelected) : 
            HandleRectSelection(rect, additive, deselectIfSelected);
    }

    private bool HandleSingleSelection(Vector2 startPos, bool deselectIfSelected)
    {
        var raycastResults = new RaycastHit2D[1];
        Physics2D.RaycastNonAlloc(_mainCam.ScreenToWorldPoint(startPos), Vector3.forward, raycastResults, 1, 1 << _unitLayer);

        var obj = raycastResults[0].transform;

        if (!obj || !obj.TryGetComponent<Unit>(out var unit)) return false;
        
        var troop = unit.ParentTroop;
        if (SelectedTroops.Contains(troop))
        {
            if (!deselectIfSelected) return false;

            DeselectTroop(troop);
            return true;
        }

        SelectTroop(troop);
        return true;
    }

    private bool HandleRectSelection(Rect rect, bool additive, bool deselectIfSelected)
    {
        var selectionMade = false;

        foreach (var troop in _armyManager.Troops)
        {
            if (IsTroopInsideRect(troop, rect))
            {
                if (SelectedTroops.Contains(troop))
                {
                    if (deselectIfSelected)
                        DeselectTroop(troop);
                }
                else
                {
                    SelectTroop(troop);
                    selectionMade = true;
                }
            }
            else if (!additive) 
                DeselectTroop(troop);
        }

        return selectionMade;
    }

    private bool IsTroopInsideRect(Troop troop, Rect rect)
    {
        foreach (var unit in troop.Units)
            if (rect.Contains(_mainCam!.WorldToScreenPoint(unit.Position).FixYAxisForScreen()))
                return true;

        return false;
    }

    private void SelectTroop(Troop troop)
    {
        troop.SetSelected(true);
        SelectedTroops.Add(troop);
    }

    private void DeselectTroop(Troop troop)
    {
        troop.SetSelected(false);
        SelectedTroops.Remove(troop);
    }


    private void ResetSelectedTroops()
    {
        foreach (var t in SelectedTroops)
            t.SetSelected(false);

        SelectedTroops.Clear();
    }

    #endregion
}

public enum PlayerFormationControllerState
{
    Default,
    TroopsSelected,
    FormationDrawing,
    FormationDrawEnd,
}