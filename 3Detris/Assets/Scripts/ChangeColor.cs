using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ChangeColor : MonoBehaviour
{
    private MeshRenderer ren;
    private Color startColor;
    [SerializeField] private GameObject placeParticle;
    [SerializeField] private GameObject placeParticleColor;
    [SerializeField] private GameObject lineClearParticle;
    [SerializeField] private float colorChangeTime = 1f;
    [SerializeField] private float colorChangeRate = 1f;
    [SerializeField] private float colorChangeStart = 0.2f; 

    private LayerMask layerMask;
    private GameObject hitObject;

    void Start()
    {
        layerMask = LayerMask.GetMask("Solid");
        ren = GetComponent<MeshRenderer>();
        startColor = ren.material.color;
    }

    public IEnumerator Change(Color newColor)
    {
        float elapsed = 0f;
        while (elapsed < colorChangeTime)
        {
            elapsed += Time.deltaTime;
            Color lerpedColor = Color.Lerp(startColor, newColor, elapsed * colorChangeRate + colorChangeStart);
            ren.material.color = lerpedColor;
            yield return null;
        }
        ren.material.color = newColor;
    }
    public void PlayPlaceParticle()
    {
        if (CheckBelow())
        {
            MeshRenderer render = GetComponent<MeshRenderer>();
            Color newColor = render.material.color;
            Vector3 pos = new(transform.position.x, transform.position.y - 0.5f, transform.position.z);

            GameObject particleObjectColor = Instantiate(placeParticleColor, pos, Quaternion.identity);
            ParticleSystem particleColor = particleObjectColor.GetComponent<ParticleSystem>();
            ParticleSystemRenderer psr = particleColor.GetComponent<ParticleSystemRenderer>();
            Material mat = psr.material;
            psr.trailMaterial = new Material(psr.trailMaterial);
            Material trailmat = psr.trailMaterial;
            trailmat.color = mat.color = newColor;
            particleColor.Play();

            MeshRenderer renderBelow = hitObject.GetComponentInChildren<MeshRenderer>();
            if (renderBelow.material.HasProperty("_Color"))
            {
                Color newColorBelow = renderBelow.material.color;

                GameObject particleObjectColorBelow = Instantiate(placeParticleColor, pos, Quaternion.Euler(180f, 0f, 0f));
                ParticleSystem particleColorBelow = particleObjectColorBelow.GetComponent<ParticleSystem>();
                ParticleSystemRenderer psrBelow = particleColorBelow.GetComponent<ParticleSystemRenderer>();
                Material matBelow = psrBelow.material;
                psrBelow.trailMaterial = new Material(psrBelow.trailMaterial);
                Material trailmatBelow = psrBelow.trailMaterial;
                trailmatBelow.color = matBelow.color = newColorBelow;
                particleColorBelow.Play();
            }
        }
        GameObject particleObjectWhite = Instantiate(placeParticle, transform.position, Quaternion.identity);
        ParticleSystem particleWhite = particleObjectWhite.GetComponent<ParticleSystem>();
        particleWhite.Play();
    }
    public void PlayLCParticle()
    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        Color startColorLC = render.material.color;
        Color newColor = startColorLC;
        newColor.a = 0f;
        GameObject particleObject = Instantiate(lineClearParticle, transform.position, Quaternion.identity);
        ChangeAlpha changeAlpha = particleObject.GetComponent<ChangeAlpha>();
        changeAlpha.StartCoroutine(changeAlpha.LerpAlpha(startColorLC, newColor));
    }
    private bool CheckBelow()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, layerMask))
        {
            hitObject = hit.collider.gameObject;
            return true;
        }
        return false;
    }
}
