using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormationDrawer : MonoBehaviour
{
    public bool IsActive { get; set; }
    public Action OnFormationDrawStarted;
    public Action<bool, List<FormationData>> OnFormationDrawEnd;

    #region Private fields

    [SerializeField] private Transform _unitPlacementPointPrefab;

    private Pool<Transform> _placementPointPool;
    private List<Transform> _pointObjs;
    private List<FormationData> _formations;

    private Vector2 _startPos;
    private Vector2 _endPos;
    private bool _isDrawing = false;
    private List<int> _troopSizes;
    private Camera mainCam;

    private FormationBuilder _formationBuilder;

    #endregion

    private void Awake()
    {
        _formationBuilder = new FormationBuilder();

        mainCam = Camera.main;

        _formations = new();
        _pointObjs = new();
        _placementPointPool = new Pool<Transform>(new Pool<Transform>.PoolProperties()
        {
            ExpansionSize = 5,
            FillOnInit = true,
            Prefab = _unitPlacementPointPrefab
        });
    }

    public void Setup(List<int> troopSizes) => _troopSizes = troopSizes;

    private void Update()
    {
        if (!IsActive)
            return;

        if (Input.GetMouseButtonDown(1))
            StartDrawing();

        _endPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (_isDrawing)
        {
            _formations = _formationBuilder.Build(_startPos, _endPos, _troopSizes);
            RenderFormations();
            
            Debug.Log((_endPos - _startPos).normalized);

            if (Input.GetKeyDown(KeyCode.Escape))
                EndDrawing(false);
        }
        else return;

        if (Input.GetMouseButtonUp(1))
            EndDrawing(true);
    }


    private void StartDrawing()
    {
        _isDrawing = true;
        _startPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        OnFormationDrawStarted?.Invoke();

        SpawnPoints();
    }

    private void EndDrawing(bool s)
    {
        _isDrawing = false;

        OnFormationDrawEnd?.Invoke(s, _formations);

        DespawnPoints();
    }


    private void RenderFormations()
    {
        for (int i = 0, j = 0; i < _formations.Count; i++)
            foreach (var point in _formations[i].Points)
            {
                _pointObjs[j].position = point;
                j++;
            }
    }


    private void SpawnPoints()
    {
        for (int i = 0; i < _troopSizes.Sum(x => x); i++)
            _pointObjs.Add(_placementPointPool.Spawn());
    }

    private void DespawnPoints()
    {
        foreach (var point in _pointObjs)
            _placementPointPool.Despawn(point);

        _pointObjs.Clear();
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(_startPos, _startPos + .5f * new Vector2(.82f, -.57f));
        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(_startPos, _startPos + .5f * new Vector2(-.82f, -.57f));
    }
}