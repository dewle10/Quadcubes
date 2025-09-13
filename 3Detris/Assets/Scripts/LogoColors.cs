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

            Color baseColor = Random.Range(0, 12) switch
            {
                0 => HexToColor("#FF3945"),
                1 => HexToColor("#24BE5F"),
                2 => HexToColor("#FF7A34"),
                3 => HexToColor("#479AFF"),
                4 => HexToColor("#FFDD19"),
                5 => HexToColor("#D588F2"),
                6 => HexToColor("#FF3945"),
                7 => HexToColor("#24BE5F"),
                8 => HexToColor("#FF7A34"),
                9 => HexToColor("#479AFF"),
                10 => HexToColor("#FFDD19"),
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
