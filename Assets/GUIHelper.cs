using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class GUIHelper
{
    public static void StereoMessage(string message)
    {
        Vector2 size = GUI.skin.box.CalcSize(new GUIContent(message));
        size += new Vector2(4,4);

        float StereoOffset = -100;

        Vector2 area = new Vector2(Screen.width / 2, Screen.height);

        GUI.Box(new Rect(((area.x / 2) - (size.x / 2)) - StereoOffset, ((area.y / 2) - (size.y / 2)), size.x, size.y), message);
        GUI.Box(new Rect((area.x + (area.x / 2) - (size.x / 2)) + StereoOffset, ((area.y / 2) - (size.y / 2)), size.x, size.y), message);       
    }
}

