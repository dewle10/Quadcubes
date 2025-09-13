using System.Collections;
using System.Reflection;
using UnityEngine;

public class RowColors : MonoBehaviour
{
    readonly private static float outerBrightness = 1f;
    readonly private static float middleBrightness = 0.9f;
    readonly private static float innerBrightness = 0.8f;

    public static void ChangeColor(GameObject cube)
    {
        Vector3 pos = cube.transform.position;
        MeshRenderer renderer = cube.GetComponentInChildren<MeshRenderer>();
        ChangeColor changeColor = cube.GetComponentInChildren<ChangeColor>();

        if (renderer == null) return;

        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        Color baseColor = y switch
        {
            0 => HexToColor("#FF3945"),
            1 => HexToColor("#2AE371"),
            2 => HexToColor("#FF7A34"),
            3 => HexToColor("#479AFF"),
            4 => HexToColor("#FFDD19"),
            5 => HexToColor("#D588F2"),
            6 => HexToColor("#FF3945"),
            7 => HexToColor("#2AE371"),
            8 => HexToColor("#FF7A34"),
            9 => HexToColor("#479AFF"),
            10 => HexToColor("#FFDD19"),
            11 => HexToColor("#D588F2"),
            _ => Color.red
        };

        float brightness;

        if (x == 0 || x == GridManager.gameWidth - 1 || z == 0 || z == GridManager.gameWidth - 1) //outer
        {
            brightness = outerBrightness;
        }
        else if (GridManager.gameWidth == 5 && x == 2 && z == 2) //inner for game size 5
        {
            brightness = innerBrightness;
        }
        else if (GridManager.gameWidth != 5 && 
            (x >= GridManager.gameWidth/2 - 1 && x <= GridManager.gameWidth/2) && 
            (z >= GridManager.gameWidth/2 - 1 && z <= GridManager.gameWidth/2) ) //inner
        {
            brightness = innerBrightness;
        }
        else //middle
        {
            brightness = middleBrightness;
        }

        Color newColor = baseColor * brightness;
        newColor.a = 1f;

        changeColor.StartCoroutine(changeColor.Change(newColor));
    }
    private static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color col))
            return col;
        else
            return Color.magenta;
    }

    //if (x == 0 || x == 5 || z == 0 || z == 5) //outer
    //{
    //    finalColor.a = 0.15f;
    //}
    //else if ((x >= 2 && x <= 3) && (z >= 2 && z <= 3)) //inner
    //{
    //    finalColor.a = 0.15f;
    //}
    //else //middle
    //{
    //    finalColor.a = 0.15f;
    //}
}
