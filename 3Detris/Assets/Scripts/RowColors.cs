using System.Reflection;
using UnityEngine;

public class RowColors : MonoBehaviour
{
    readonly private static float outerBrightness = 1f;
    readonly private static float middleBrightness = 0.8f;
    readonly private static float innerBrightness = 0.6f;

    public static void ChangeColor(GameObject cube)
    {
        Vector3 pos = cube.transform.position;
        MeshRenderer renderer = cube.GetComponentInChildren<MeshRenderer>();

        if (renderer == null) return;

        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        Color baseColor = y switch
        {
            0 => HexToColor("#B01730"),
            1 => HexToColor("#32936F"),
            2 => HexToColor("#EF6F2B"),
            3 => HexToColor("#3D7BA8"),
            4 => HexToColor("#F6AE2D"),
            5 => HexToColor("#938FC6"),
            6 => HexToColor("#B01730"),
            7 => HexToColor("#32936F"),
            8 => HexToColor("#EF6F2B"),
            9 => HexToColor("#3D7BA8"),
            10 => HexToColor("#F6AE2D"),
            11 => HexToColor("#938FC6"),
            _ => Color.red
        };

        //Color baseColor = y switch
        //{
        //    0 => HexToColor("#801426"),
        //    1 => HexToColor("#7BB22E"),
        //    2 => HexToColor("#587B7F"),
        //    3 => HexToColor("#FED766"),
        //    4 => HexToColor("#7A8450"),
        //    5 => HexToColor("#9188CF"),
        //    6 => HexToColor("#801426"),
        //    7 => HexToColor("#7BB22E"),
        //    8 => HexToColor("#587B7F"),
        //    9 => HexToColor("#FED766"),
        //    10 => HexToColor("#7A8450"),
        //    11 => HexToColor("#9188CF"),
        //    _ => Color.red
        //};

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

        Color finalColor = baseColor * brightness;
        finalColor.a = 1f;

        renderer.material.color = finalColor;
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
