using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;

public class UnitView : SerializedMonoBehaviour
{
    [OdinSerialize, NonSerialized] private Dictionary<SkinPartType, SpriteRenderer> _skinParts;
    [SerializeField] private SpriteRenderer _selectionHighlight;

    private NavMeshAgent _agent;
    [SerializeField] private SpriteRenderer _weapon;

    [SerializeField] private GameSettings _gameSettings;

    private float _xScale;
    private SkinDirection _lastSkinDirection;
    private Vector3 _lastVelocity;
    private bool _skinAdjustmentForStopFlag;
    private Vector2 _formationFwd;

    private void Start()
    {
        _skinAdjustmentForStopFlag = true;

        _agent = GetComponentInParent<NavMeshAgent>();
        _xScale = transform.localScale.x;
    }

    private void Update()
    {
        if (!_agent.isStopped && !_lastVelocity.Equals(_agent.velocity))
        {
            AdjustSkin(_agent.velocity);
            _skinAdjustmentForStopFlag = false;
        }
        else if (!_skinAdjustmentForStopFlag)
        {
            AdjustSkin(_formationFwd);
            _skinAdjustmentForStopFlag = true;
        }

        _lastVelocity = _agent.velocity;
    }

    void AdjustSkin(Vector3 direction)
    {
        if (_agent.velocity == Vector3.zero) return;

        var absX = Mathf.Abs(direction.x);
        var absY = Mathf.Abs(direction.y);

        var total = absX + absY;
        var percentageX = absX / total;
        var percentageY = absY / total;

        var dir = (percentageY > percentageX)
            ? _agent.velocity.y > 0 ? SkinDirection.Back : SkinDirection.Front
            : SkinDirection.Side;

        if (dir.Equals(_lastSkinDirection))
            return;

        _lastSkinDirection = dir;

        ChangeVisualDirection(dir);

        var scale = transform.localScale;
        scale.x = _xScale * (_agent.velocity.x > 0 ? 1 : -1);
        transform.localScale = scale;
    }

    void ChangeVisualDirection(SkinDirection direction)
    {
        foreach (var skinPart in _skinParts)
            skinPart.Value.sprite = _gameSettings.SoldierViewSettings.Sprites[(int)direction, (int)skinPart.Key];
    }

    public void SetFormationForward(Vector3 fwd) => _formationFwd = fwd;


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