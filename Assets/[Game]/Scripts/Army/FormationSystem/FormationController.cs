using System;
using System.Collections.Generic;
using System.Linq;
using GAME.Utilities.StateMachine;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    private List<Troop> Troops => _armyManager.Troops;
    public List<Troop> SelectedTroops { get; private set; }

    private StateMachine<FormationControllerState> _stateMachine;

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

        SelectedTroops = new();
        _stateMachine = new();

        _stateMachine.AddState(FormationControllerState.Default, new DefaultState(_selectionBox, _formationDrawer, this));
        _stateMachine.AddState(FormationControllerState.TroopsSelected, new TroopsSelectedState(_selectionBox, _formationDrawer, this));
        _stateMachine.AddState(FormationControllerState.FormationDrawing, new FormationDrawingState(_selectionBox, _formationDrawer, this));
        _stateMachine.AddState(FormationControllerState.FormationDrawEnd, new FormationDrawEndState(_selectionBox, _formationDrawer, this));

        _stateMachine.Init();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void ChangeState(FormationControllerState state, params object[] @params) => _stateMachine.ChangeState(state, @params);

    public bool SelectOverlappedTroops(Rect rect)
    {
        ResetSelectedTroops();

        var r = false;
        foreach (var troop in Troops)
        {
            var s = false;
            for (var i = 0; i < troop.Units.Count; i++)
            {
                s = rect.Contains(Camera.main!.WorldToScreenPoint(troop.Units[i].Position).FixYAxisForScreen());
                if (s)
                    break;
            }

            troop.SetSelected(s);

            if (s)
            {
                SelectedTroops.Add(troop);
                r = true;
            }
            else
                SelectedTroops.Remove(troop);
        }

        return r;
    }

    private void ResetSelectedTroops()
    {
        SelectedTroops.Clear();
    }

    public void MoveCommand(List<FormationData> formations)
    {
        int i = 0;
        foreach (var formation in formations)
        {
            SelectedTroops[i].SetFormation(formation);
            i++;
        }
    }
}

public enum FormationControllerState
{
    Default,
    TroopsSelected,
    FormationDrawing,
    FormationDrawEnd,
}