using UnityEngine;

public class RowColors
{
    private const float OuterBrightness = 1f;
    private const float MiddleBrightness = 0.9f;
    private const float InnerBrightness = 0.8f;

    public static void ChangeColor(GameObject cube)
    {
        VisualEffects visualEffects = cube.GetComponentInChildren<VisualEffects>();

        Vector3 pos = cube.transform.position;
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        Color baseColor = GetBaseColor(y);

        float brightness = GetBrightness(x, z);

        Color newColor = baseColor * brightness;
        newColor.a = 1f;

        visualEffects.StartCoroutine(visualEffects.ChangeColor(newColor));
    }
    private static Color GetBaseColor(int y)
    {
        return y switch
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
    }
    private static float GetBrightness(int x, int z)
    {
        int width = GridManager.gameWidth;

        // Outer layer
        if (x == 0 || x == width - 1 || z == 0 || z == width - 1)
            return OuterBrightness;

        // Inner layer for 5x5
        if (width == 5 && x == 2 && z == 2)
            return InnerBrightness;

        // Inner layer for other sizes
        if (width != 5 && (x >= width / 2 - 1 && x <= width / 2) && (z >= width / 2 - 1 && z <= width / 2))
            return InnerBrightness;

        // Middle layer
        return MiddleBrightness;
    }
    private static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color col))
            return col;
        else
            return Color.magenta;
    }
}
