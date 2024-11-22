using System.Collections.Generic;
using System.Linq;
using GAME.Utilities.StateMachine;
using UnityEngine;

public class DefaultState : FormationControllerStateBase
{
    public DefaultState(SelectionBox selectionBox, FormationDrawer formationDrawer, FormationController formationController) 
        : base(formationDrawer, selectionBox, formationController)
    {
    }
    public override void OnEnter(object[] @params)
    {
        _selectionBox.IsActive = true;
        _formationDrawer.IsActive = false;
        _formationDrawer.Setup(null);
        
        _selectionBox.OnUnitsSelectionEnd += OnUnitsSelectionEnd;
    }

    private void OnUnitsSelectionEnd(Rect rect)
    {
        if (_formationController.SelectOverlappedTroops(rect))
            _formationController.ChangeState(FormationControllerState.TroopsSelected);
    }

    public override void OnExit()
    {
        _selectionBox.IsActive = false;
        _selectionBox.OnUnitsSelectionEnd -= OnUnitsSelectionEnd;
    }
}

/// <summary>
/// Ready to give command
/// </summary>
public class TroopsSelectedState : FormationControllerStateBase
{
    public TroopsSelectedState(SelectionBox selectionBox, FormationDrawer formationDrawer, FormationController formationController) 
        : base(formationDrawer, selectionBox, formationController)
    {
    }

    public override void OnEnter(object[] @params)
    {
        //Cursor change -> point target
        _selectionBox.IsActive = true;

        _formationDrawer.IsActive = true;
        _formationDrawer.Setup(
            _formationController.SelectedTroops.Select(x => x.Units.Count).ToList());
        
        _formationDrawer.OnFormationDrawStarted += OnFormationDrawStarted;
        _selectionBox.OnUnitsSelectionEnd += OnUnitsSelectionEnd;
    }

    private void OnUnitsSelectionEnd(Rect rect)
    {
        _formationController.ChangeState(_formationController.SelectOverlappedTroops(rect)
                                             ? FormationControllerState.TroopsSelected
                                             : FormationControllerState.Default);
    }

    private void OnFormationDrawStarted()
    {
        _formationController.ChangeState(FormationControllerState.FormationDrawing);
    }

    public override void OnExit()
    {
        _selectionBox.IsActive = false;
        
        _formationDrawer.OnFormationDrawStarted -= OnFormationDrawStarted;
        _selectionBox.OnUnitsSelectionEnd -= OnUnitsSelectionEnd;
    }
}

public class FormationDrawingState : FormationControllerStateBase
{
    public FormationDrawingState(SelectionBox selectionBox, FormationDrawer formationDrawer, FormationController formationController) 
        : base(formationDrawer, selectionBox, formationController)
    {
    }

    public override void OnEnter(object[] @params)
    {
        _selectionBox.IsActive = false;
   
        _formationDrawer.OnFormationDrawEnd += OnFormationDrawEnd;
    }

    private void OnFormationDrawEnd(bool s, List<FormationData> formations)
    {
        _formationController.ChangeState(s ? FormationControllerState.FormationDrawEnd : FormationControllerState.TroopsSelected, formations);
    }

    public override void OnExit()
    {
        _formationDrawer.OnFormationDrawEnd -= OnFormationDrawEnd;
    }
}

public class FormationDrawEndState : FormationControllerStateBase
{
    public FormationDrawEndState(SelectionBox selectionBox, FormationDrawer formationDrawer, FormationController formationController) 
        : base(formationDrawer, selectionBox, formationController)
    {
    }

    public override void OnEnter(object[] @params)
    {
        _formationController.MoveCommand((List<FormationData>)@params[0]);

        _formationController.ChangeState(FormationControllerState.TroopsSelected);
    }

    public override void OnExit()
    {
        _formationDrawer.IsActive = false;
    }
}


public abstract class FormationControllerStateBase : StateBase<FormationControllerState>
{
    protected readonly FormationController _formationController;
    
    protected readonly SelectionBox _selectionBox;
    protected readonly FormationDrawer _formationDrawer;

    protected FormationControllerStateBase(FormationDrawer formationDrawer, SelectionBox selectionBox, FormationController formationController)
    {
        _formationController = formationController;
        _selectionBox = selectionBox;
        _formationDrawer = formationDrawer;
    }
}