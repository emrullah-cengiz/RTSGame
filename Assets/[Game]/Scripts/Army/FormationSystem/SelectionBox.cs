using System;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    public bool IsActive { get; set; }
    
    [SerializeField] private float _borderThickness = 2f;
    [SerializeField] private Color _fillColor = Color.white,
                                   _borderColor = Color.white;


    public Action<Rect> OnUnitsSelectionEnd;

    private ArmyManager _armyController;

    private Rect _rect;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private bool _isSelecting = false;

    private void Awake()
    {
        _armyController = GetComponent<ArmyManager>();
    }

    void Update()
    {
        if (!IsActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _isSelecting = true;
            _startPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isSelecting = false;
            OnUnitsSelectionEnd?.Invoke(_rect);
        }
        
        if (!_isSelecting) return;

        _endPosition = Input.mousePosition;
    }

    private void OnGUI()
    {
        if (!_isSelecting) return;

        _rect = GetScreenRect(_startPosition, _endPosition);

        DrawScreenRect(_rect, _fillColor);
        DrawScreenRectBorder(_rect, _borderThickness, _borderColor);
    }

    private static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;

        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}