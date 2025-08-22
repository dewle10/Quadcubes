using UnityEngine;

public class Testlerpcolor : MonoBehaviour
{
    private MeshRenderer ren;
    private Color lerpedColor = Color.white;

    void Start()
    {
        ren = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        lerpedColor = Color.Lerp(Color.white, Color.black, Time.time * 0.2f);
        ren.material.color = lerpedColor;
    }
}