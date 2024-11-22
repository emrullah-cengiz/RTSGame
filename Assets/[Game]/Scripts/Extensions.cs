using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static Vector3 FixYAxisForScreen(this Vector3 pos)
    {
        pos.y = Screen.height - pos.y;
        return pos;
    }

    public static Vector2 FixYAxisForScreen(this Vector2 pos)
    {
        pos.y = Screen.height - pos.y;
        return pos;
    }
    
    public static Vector2 GetRight(this Vector2 direction)
    {
        float newX = direction.y;
        float newY = -direction.x;
        return new Vector2(newX, newY);
    }
}
