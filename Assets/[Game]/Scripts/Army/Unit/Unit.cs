using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : TransformObject
{
    public UnitView View;
    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        View = GetComponentInChildren<UnitView>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }


    public void SetDestination(Vector3 pos) => _agent.SetDestination(pos);

}
