using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static Vector2 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 wPoint = Camera.main.ScreenToWorldPoint(mousePos);
        return wPoint;
    }

    public static Vector2 Bezier(float t, Vector2 a, Vector2 b, Vector2 c)
    {
        var ab = Vector2.Lerp(a, b, t);
        var bc = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(ab, bc, t);
    }
}
