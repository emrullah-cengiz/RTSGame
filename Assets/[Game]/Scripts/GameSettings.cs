using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = nameof(GameSettings))]
public class GameSettings : SerializedScriptableObject
{
    [OdinSerialize, NonSerialized]
    public SoldierViewSettings SoldierViewSettings;
}

[Serializable]
public class SoldierViewSettings
{
    //public Sprite UnitPlacementPointSprite;

    [TableMatrix(HorizontalTitle = "Skin Sprites By Directions", Labels = "GetMatrixLabel", SquareCells = true)]
    public Sprite[,] Sprites = new Sprite[3,5];
    
    public Dictionary<UnitType, Sprite[,]> SpritesByUnit;

    public Vector2 HeadPosOffsetOnSide;
    public Vector2 WeaponPosOffsetOnSide;
    
    private (string, LabelDirection) GetMatrixLabel(Sprite[,] array, TableAxis axis, int index)
    {
        return axis switch
        {
            TableAxis.X => (((SkinDirection)index).ToString(), LabelDirection.LeftToRight),
            TableAxis.Y => (((SkinPartType)index).ToString(), LabelDirection.BottomToTop),
            _ => default
        };
    }
}

public enum UnitType
{
    Infantry,
    Archer
}

public enum SkinPartType
{
    Helmet,
    Armor,
    Panths,
    Head,
    Body,
}
public enum SkinDirection
{
    Side,
    Front,
    Back
}


