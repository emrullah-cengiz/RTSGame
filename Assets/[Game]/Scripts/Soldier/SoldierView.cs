using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;

public class SoldierView : SerializedMonoBehaviour
{
    [OdinSerialize, NonSerialized] private Dictionary<SkinPartType, SpriteRenderer> _skinParts;
    [SerializeField] private SpriteRenderer _selectionHighlight;

    private NavMeshAgent _agent;
    [SerializeField] private SpriteRenderer _weapon;

    [SerializeField] private GameSettings _gameSettings;

    private float _xScale;

    private void Start()
    {
        _agent = GetComponentInParent<NavMeshAgent>();
        _xScale = transform.localScale.x;
    }

    private void Update()
    {
        UpdateSkin();
    }

    void UpdateSkin()
    {
        if (_agent.velocity == Vector3.zero) return;

        var absX = Mathf.Abs(_agent.velocity.x);
        var absY = Mathf.Abs(_agent.velocity.y);

        var total = absX + absY;
        var percentageX = absX / total;
        var percentageY = absY / total;

        ChangeVisualDirection(
            (percentageY > percentageX)
                ? _agent.velocity.y > 0 ? SkinDirection.Back : SkinDirection.Front
                : SkinDirection.Side);

        var scale = transform.localScale;
        scale.x = _xScale * (_agent.velocity.x > 0 ? 1 : -1);
        transform.localScale = scale;
    }

    void ChangeVisualDirection(SkinDirection direction)
    {
        foreach (var skinPart in _skinParts)
            skinPart.Value.sprite = _gameSettings.SoldierViewSettings.Sprites[(int)direction, (int)skinPart.Key];
    }

    public void SetSelected(bool s) => _selectionHighlight.gameObject.SetActive(s);

#if UNITY_EDITOR

    [SerializeField, EnumToggleButtons, OnValueChanged(action: "ApplyDirection")]
    private Direction _testDir;

    public void ApplyDirection()
    {
        ChangeVisualDirection(DirectionToSkinDirection(_testDir));

        var scale = transform.localScale;
        scale.x = scale.x * (_testDir == Direction.Right ? 1 : -1);
        transform.localScale = scale;
    }

    private SkinDirection DirectionToSkinDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Top => SkinDirection.Back,
            Direction.Bottom => SkinDirection.Front,
            Direction.Left or Direction.Right => SkinDirection.Side,
            _ => default
        };
    }
#endif
}