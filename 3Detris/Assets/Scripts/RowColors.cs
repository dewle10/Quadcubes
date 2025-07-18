using System.Reflection;
using UnityEngine;

public class RowColors : MonoBehaviour
{
    private static float outerBrightness = 1f;
    private static float middleBrightness = 0.75f;
    private static float innerBrightness = 0.5f;

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
            0 => Color.red,
            1 => new Color(1f, 0.5f, 0f),  // pomarañczowy
            2 => Color.yellow,
            3 => Color.green,
            4 => Color.cyan,
            5 => Color.blue,
            _ => Color.white
        };

        float brightness;

        if (x == 0 || x == 5 || z == 0 || z == 5) //outer
        {
            brightness = outerBrightness;
        }
        else if ((x >= 2 && x <= 3) && (z >= 2 && z <= 3)) //inner
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

}
