using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : CommandableActorBase, IInteractable
{
    public InteractableType InteractableType => InteractableType.Unit;
    public MonoBehaviour Obj => this;

    public UnitView View;
    public Troop ParentTroop { get; private set; }
    
    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        View = GetComponentInChildren<UnitView>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void SetDestination(Vector3 pos) => _agent.SetDestination(pos);

    public void SetTroop(Troop troop) => ParentTroop = troop;
}
