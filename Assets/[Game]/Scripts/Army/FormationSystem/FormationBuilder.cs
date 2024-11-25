using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class FormationBuilder
{
    private float _minUnitDistance = .2f;
    private float _formationDistance = .25f;
    private int _minFormationX = 2;
    private Vector2 _defaultWorldDirection = new Vector2(.57f, .82f);

    public FormationData Build(Troop troop, Vector2 startPos, Vector2? forward = null)
    {
        return Build(startPos, troop.FormationData.FormationWidth, troop.Units.Count, forward ?? _defaultWorldDirection);
    }

    public IEnumerable<FormationData> Build(List<Troop> troops, Vector2 startPos, Vector2? forward = null)
    {
        return troops.Select(troop => Build(startPos, troop.FormationData.FormationWidth, troop.Units.Count, forward ?? _defaultWorldDirection));
    }

    public List<FormationData> Build(Vector2 startPos, Vector2 endPos, List<int> troopSizes)
    {
        var res = new List<FormationData>();

        var distance = endPos - startPos;

        var totalLength = distance.magnitude;

        //rotate vector 90 degrees to left
        var forward = new Vector2(-distance.y, distance.x).normalized;
        var right = distance.normalized;

        
        var testDir = distance.normalized;
        Debug.DrawRay(startPos, testDir, Color.blue);
        Debug.DrawRay(startPos, new Vector2(-testDir.y, testDir.x), Color.red);

        if (distance == Vector2.zero)
        {
            forward = _defaultWorldDirection;
            //rotate vector 90 degrees to right
            right = new Vector2(forward.y, -forward.x);
        }

        var formationWidth = Mathf.Max(totalLength / troopSizes.Count, _minUnitDistance * _minFormationX);

        for (var i = 0; i < troopSizes.Count; i++)
            res.Add(Build(startPos + (i * _formationDistance * right) + (i * formationWidth * right), formationWidth, troopSizes[i], forward));

        return res;
    }

    private FormationData Build(Vector2 startPos, float formationWidth, int troopSize, Vector2 forward)
    {
        var points = new List<Vector2>();

        var xCount = Mathf.Clamp((int)(formationWidth / _minUnitDistance), _minFormationX, troopSize);
        var yCount = troopSize / xCount;
        var remaining = troopSize % xCount;

        var right = new Vector2(forward.y, -forward.x);
        var back = -forward;

        for (var y = 0; y < yCount; y++)
        for (var x = 0; x < xCount; x++)
            points.Add(GetPointPosition(startPos, x, y, right, back));

        var margin = (formationWidth - remaining * _minUnitDistance) / 2;

        for (var x = 0; x < remaining; x++)
            points.Add(GetPointPosition(startPos, x, yCount, right, back, margin));

        return new FormationData
        {
            Points = points,
            Forward = forward,
            FormationWidth = formationWidth,

            //Center of formation
            Position = Vector2.Lerp(startPos, startPos + right * formationWidth / 2 +
                                              back * ((yCount + (remaining > 0 ? 1 : 0)) * _minUnitDistance / 2), 0.5f)
        };
    }

    private Vector2 GetPointPosition(Vector2 startPos, int x, int y, Vector2 right, Vector2 back, float margin = 0f)
    {
        return startPos + right * (margin + x * _minUnitDistance) + back * (y * _minUnitDistance);
    }
}