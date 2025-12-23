using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    /// <summary>
    /// µ÷ÕûÎ»ÖÃ
    /// </summary>
    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mousePosition.x >= Screen.width / 2)
            xOffset = -Screen.width / 4;
        else
            xOffset = Screen.width / 4;

        if (mousePosition.y >= Screen.height / 2)
            yOffset = -Screen.height / 4;
        else
            yOffset = Screen.height / 4;

        transform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);

    }
}
