using UnityEngine;

public class LogoColors : MonoBehaviour
{
    //readonly private static float outerBrightness = 1f;
    //readonly private static float middleBrightness = 0.8f;
    //readonly private static float innerBrightness = 0.6f;

    [SerializeField] private GameObject[] meshes;
    public Color color;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeColors();
            SoundManager.PlaySound(SoundType.Rotate);
        }
    }
    private void ChangeColors()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3 pos = meshes[i].transform.position;
            MeshRenderer renderer = meshes[i].GetComponentInChildren<MeshRenderer>();
            if (renderer == null) return;
            
            int x = (int)pos.x;
            int y = (int)pos.y;
            int z = (int)pos.z;

            Color baseColor = (int)Random.Range(0, 12) switch
            {
                0 => HexToColor("#DD2C4B"),
                1 => HexToColor("#45B88E"),
                2 => HexToColor("#FF7A34"),
                3 => HexToColor("#588FD2"),
                4 => HexToColor("#FFC93C"),
                5 => HexToColor("#D588F2"),
                6 => HexToColor("#DD2C4B"),
                7 => HexToColor("#45B88E"),
                8 => HexToColor("#FF7A34"),
                9 => HexToColor("#588FD2"),
                10 => HexToColor("#FFC93C"),
                11 => HexToColor("#D588F2"),
                _ => Color.red
            };
            renderer.material.color = baseColor;
        }

    }
    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color col))
            return col;
        else
            return Color.magenta;
    }
}
