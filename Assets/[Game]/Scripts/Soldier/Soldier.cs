using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    public SoldierView View;
    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        View = GetComponentInChildren<SoldierView>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }


    public void SetDestination(Vector3 pos) => _agent.SetDestination(pos);
}
