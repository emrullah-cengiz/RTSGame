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

public class ArmyManager : MonoBehaviour
{
    private FormationController _formationController;
    public List<Troop> Troops;
    
    private void Awake()
    {
        _formationController = GetComponent<FormationController>();
    }
}
