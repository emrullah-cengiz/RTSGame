using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    [SerializeField] private float _minUnitDistance = .1f;
    [SerializeField] private int _minFormationX = 3;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _unitPlacementPointObj;

    public Action OnFormationDrawStarted;
    public Action<HashSet<Vector3>> OnFormationDrawn;

    private ArmyController _armyController;
    private Pool<Transform> _placementPointPool;
    private List<Transform> _points;

    private Vector2 _startPos;
    private Vector2 _endPos;
    private bool _isDrawing = false;

    [SerializeField] private Vector2 DEFAULT_WORLD_DIRECTION = (Vector2.right + Vector2.up).normalized;

    private void Awake()
    {
        _armyController = GetComponent<ArmyController>();
        _points = new();
        _placementPointPool = new Pool<Transform>(new Pool<Transform>.PoolProperties()
        {
            ExpansionSize = 5,
            FillOnInit = true,
            Prefab = _unitPlacementPointObj
        });
    }

    private void Update()
    {
        if (_armyController.State == ArmyControllerState.Default)
            return;

        if (Input.GetMouseButtonDown(1) &&
            _armyController.State != ArmyControllerState.FormationDrawing)
        {
            _isDrawing = true;
            _startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //_lineRenderer.SetPosition(0, _startPos);

            OnFormationDrawStarted?.Invoke();

            SpawnPoints();
        }

        _endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_isDrawing) DrawFormation();
        else return;
        if (Input.GetMouseButtonUp(1))
        {
            _isDrawing = false;

            OnFormationDrawn?.Invoke(_points.Select(x => x.position).ToHashSet());

            DespawnPoints();
        }
    }


    private void DrawFormation()
    {
        //_lineRenderer.SetPosition(1, _endPos);
        int selectedUnitCount = _armyController.SelectedUnitsCount;

        float length = (_endPos - _startPos).magnitude;
        length = Mathf.Max(length, _minUnitDistance * _minFormationX, length);

        int xCount = Mathf.Min((int)(length / _minUnitDistance), selectedUnitCount);
        if (xCount < _minFormationX && selectedUnitCount > _minFormationX)
            xCount = _minFormationX;

        int yCount = selectedUnitCount / xCount;

        int remaining = selectedUnitCount - xCount * yCount;

        Vector2 xDirection = (_endPos - _startPos).normalized;
        xDirection = xDirection.Equals(Vector2.zero) ? GetBackDirection(DEFAULT_WORLD_DIRECTION) : xDirection;

        Vector2 yDirection = GetBackDirection(xDirection);

        int i = 0, y = 0;
        for (y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                _points[i].position = GetPointPosition(x, y, xDirection, yDirection);
                i++;
            }
        }

        float margin = (xCount * _minUnitDistance - _minUnitDistance * remaining) / 2;

        for (int x = 0; x < remaining; x++)
        {
            _points[i].position = GetPointPosition(x, y, xDirection, yDirection, margin);
            i++;
        }
    }

    private Vector2 GetPointPosition(int x, int y, Vector2 xDirection, Vector2 yDirection, float margin = 0f)
    {
        return _startPos + (xDirection * (x * _minUnitDistance + margin)) + (yDirection * y * _minUnitDistance);
    }

    private void SpawnPoints()
    {
        for (int i = 0; i < _armyController.SelectedUnitsCount; i++)
            _points.Add(_placementPointPool.Spawn());
    }

    private void DespawnPoints()
    {
        foreach (var point in _points)
            _placementPointPool.Despawn(point);

        _points.Clear();
    }

    Vector2 GetBackDirection(Vector2 direction)
    {
        float newX = direction.y;
        float newY = -direction.x;
        return new Vector2(newX, newY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var center = Camera.main.transform.position + Vector3.forward *.2f;

        Vector3 dir = new(DEFAULT_WORLD_DIRECTION.x, DEFAULT_WORLD_DIRECTION.y);

        Gizmos.DrawLine(center, center + dir);
        Gizmos.DrawLine(center + Vector3.left * .05f, center + Vector3.left * .05f + dir);
    }

}
