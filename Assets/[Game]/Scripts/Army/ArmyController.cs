using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ArmyControllerState
{
    Default,
    UnitsSelected,
    FormationDrawing,
}

public class ArmyController : MonoBehaviour
{
    private SelectionBox _selectionBox;
    private FormationController _formationController;

    [SerializeField] private List<Soldier> _soldiers;
    [SerializeField] private List<Soldier> _selectedSoldiers;

    public ArmyControllerState State { get; private set; }

    public int SelectedUnitsCount => _selectedSoldiers.Count;

    private void Awake()
    {
        _selectionBox = GetComponent<SelectionBox>();
        _formationController = GetComponent<FormationController>();
    }

    private void OnEnable()
    {
        _selectionBox.OnUnitsSelectionCompleted += OnUnitsSelectionCompleted;
        _formationController.OnFormationDrawStarted += OnFormationDrawStarted;
        _formationController.OnFormationDrawn += OnFormationDrawn;
    }

    private void OnDisable()
    {
        _selectionBox.OnUnitsSelectionCompleted -= OnUnitsSelectionCompleted;
        _formationController.OnFormationDrawStarted -= OnFormationDrawStarted;
        _formationController.OnFormationDrawn -= OnFormationDrawn;

    }

    private void OnUnitsSelectionCompleted(Rect rect)
    {
        ResetSelection();

        foreach (var soldier in _soldiers)
        {
            bool s = rect.Contains(Camera.main.WorldToScreenPoint(soldier.transform.position).FixYAxisForScreen());

            soldier.View.SetSelected(s);

            if (s)
                _selectedSoldiers.Add(soldier);
        }

        State = _selectedSoldiers.Any() ? ArmyControllerState.UnitsSelected : ArmyControllerState.Default;
    }

    private void OnFormationDrawStarted()
    {
        State = ArmyControllerState.FormationDrawing;
    }

    //MoveCommand
    private void OnFormationDrawn(HashSet<Vector3> points)
    {
        int i = 0;
        foreach (var point in points)
        {
            _selectedSoldiers[i].SetDestination(point);
            i++;
        }

        State = ArmyControllerState.UnitsSelected;
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //    MoveCommand();
    }

    private void MoveCommand()
    {
        if (!_selectedSoldiers.Any()) return;

        var targetPos = Camera.main!.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        foreach (var soldier in _selectedSoldiers)
            soldier.SetDestination(targetPos);
    }

    private void ResetSelection()
    {
        _selectedSoldiers.Clear();

        State = ArmyControllerState.Default;
    }
}
