using System.Collections.Generic;
using System.Linq;
using GAME.Utilities.StateMachine;
using UnityEngine;

public class DefaultState : PlayerFormationControllerStateBase
{
    public DefaultState(SelectionBox selectionBox, FormationDrawer formationDrawer, PlayerFormationController formationController)
        : base(formationDrawer, selectionBox, formationController)
    {
    }

    public override void OnEnter(object[] @params)
    {
        _selectionBox.IsActive = true;
        _formationDrawer.IsActive = false;
        _formationDrawer.Setup(null);

        _selectionBox.OnBoxDrawn += OnUnitsSelectionEnd;
    }

    private void OnUnitsSelectionEnd(Rect rect, Vector2 startPos)
    {
        if (_formationController.SelectOverlappedTroops(rect, startPos, Input.GetKey(KeyCode.LeftControl), true))
            _formationController.ChangeState(PlayerFormationControllerState.TroopsSelected);
    }

    public override void OnExit()
    {
        _selectionBox.IsActive = false;
        _selectionBox.OnBoxDrawn -= OnUnitsSelectionEnd;
    }
}

/// <summary>
/// Ready to give command
/// </summary>
public class TroopsSelectedState : PlayerFormationControllerStateBase
{
    public TroopsSelectedState(SelectionBox selectionBox, FormationDrawer formationDrawer, PlayerFormationController formationController)
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
        _selectionBox.OnBoxDrawn += OnUnitsSelectionEnd;
    }

    private void OnUnitsSelectionEnd(Rect rect, Vector2 startPos)
    {
        _formationController.SelectOverlappedTroops(rect, startPos, Input.GetKey(KeyCode.LeftControl), true);

        _formationController.ChangeState(_formationController.SelectedTroops.Any()
                                             ? PlayerFormationControllerState.TroopsSelected
                                             : PlayerFormationControllerState.Default);
    }

    private void OnFormationDrawStarted()
    {
        _formationController.ChangeState(PlayerFormationControllerState.FormationDrawing);
    }

    public override void OnExit()
    {
        _selectionBox.IsActive = false;

        _formationDrawer.OnFormationDrawStarted -= OnFormationDrawStarted;
        _selectionBox.OnBoxDrawn -= OnUnitsSelectionEnd;
    }
}

public class FormationDrawingState : PlayerFormationControllerStateBase
{
    public FormationDrawingState(SelectionBox selectionBox, FormationDrawer formationDrawer, PlayerFormationController formationController)
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
        _formationController.ChangeState(s ? PlayerFormationControllerState.FormationDrawEnd : PlayerFormationControllerState.TroopsSelected);
    }

    public override void OnExit()
    {
        _formationDrawer.OnFormationDrawEnd -= OnFormationDrawEnd;
    }
}

public class FormationDrawEndState : PlayerFormationControllerStateBase
{
    public FormationDrawEndState(SelectionBox selectionBox, FormationDrawer formationDrawer, PlayerFormationController formationController)
        : base(formationDrawer, selectionBox, formationController)
    {
    }

    public override void OnEnter(object[] @params)
    {
        // _formationController.MoveCommand((List<FormationData>)@params[0]);

        _formationController.ChangeState(PlayerFormationControllerState.TroopsSelected);
    }

    public override void OnExit()
    {
        _formationDrawer.IsActive = false;
    }
}


public abstract class PlayerFormationControllerStateBase : StateBase<PlayerFormationControllerState>
{
    protected readonly PlayerFormationController _formationController;

    protected readonly SelectionBox _selectionBox;
    protected readonly FormationDrawer _formationDrawer;

    protected PlayerFormationControllerStateBase(FormationDrawer formationDrawer, SelectionBox selectionBox, PlayerFormationController formationController)
    {
        _formationController = formationController;
        _selectionBox = selectionBox;
        _formationDrawer = formationDrawer;
    }
}