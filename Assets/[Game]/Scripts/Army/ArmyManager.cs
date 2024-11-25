using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    private PlayerOrderController _playerOrderController;
    private PlayerFormationController _formationController;
    public List<Troop> Troops;
    
    private void Awake()
    {
        _playerOrderController = new PlayerOrderController();
        _formationController = GetComponent<PlayerFormationController>();
    }
}
