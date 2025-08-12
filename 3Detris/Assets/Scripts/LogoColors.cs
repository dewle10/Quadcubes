using UnityEngine;

public class LogoColors : MonoBehaviour
{
    //readonly private static float outerBrightness = 1f;
    //readonly private static float middleBrightness = 0.8f;
    //readonly private static float innerBrightness = 0.6f;

    [SerializeField] private GameObject[] meshes;

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

            Color baseColor = (int)Random.Range(0,12) switch
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

            //float brightness;

            //if (x == 0 || x == 5 || z == 0 || z == 5) //outer
            //{
            //    brightness = outerBrightness;
            //}
            //else if ((x >= 2 && x <= 3) && (z >= 2 && z <= 3)) //inner
            //{
            //    brightness = innerBrightness;
            //}
            //else //middle
            //{
            //    brightness = middleBrightness;
            //}

            //Color finalColor = baseColor * brightness;
            //finalColor.a = 1f;

            //renderer.material.color = finalColor;
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
