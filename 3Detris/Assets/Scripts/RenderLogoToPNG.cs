using UnityEngine;
using System.Collections;
using System.IO;

public class RenderLogoToPNG : MonoBehaviour
{
    public Camera renderCamera;
    public RenderTexture renderTexture;

    IEnumerator Start()
    {
        renderCamera.targetTexture = renderTexture;

        yield return new WaitForSeconds(2f);

        renderCamera.Render();

        SaveRenderTextureToPNG(renderTexture, "LogoExport.png");
    }

    void SaveRenderTextureToPNG(RenderTexture rt, string fileName)
    {
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Logo saved to: {path}");

        RenderTexture.active = previous;
    }
}
