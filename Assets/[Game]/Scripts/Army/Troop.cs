using System;
using System.Collections.Generic;
using UnityEngine;

public class Troop : CommandableActorBase
{
    [SerializeField] private List<Unit> _units;

    public List<Unit> Units
    {
        get => _units;
        private set => _units = value;
    }

    public UnitType Type { get; private set; }

    public FormationData FormationData { get; private set; }

    private void Start()
    {
        FormationData = new FormationData()
        {
            Position = transform.position
        };

        foreach (var unit in Units) 
            unit.SetTroop(this);
    }

    public void SetSelected(bool b)
    {
        foreach (var unit in Units) 
            unit.View.SetSelected(b);
    }

    public void SetFormation(FormationData formation)
    {
        FormationData = formation;

        //TODO: implement Command system

        var i = 0;
        foreach (var point in formation.Points)
        {
            var unit = Units[i];
            unit.SetDestination(point);
            unit.View.SetFormationForward(formation.Forward);
            i++;
        }

        //Make units look to fwd..
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(FormationData.Position, .07f);
    }
}