using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Fire : MonoBehaviour
{
    private ParticleSystem fire;
    private ParticleSystem smoke;
    private AudioSource audioSource;
    private Mesh fireMesh;

    private bool ignited = false;
    private float prevTimeScale;

    void Start()
    {
        fire = GetComponent<ParticleSystem>();
        smoke = transform.GetChild(0).GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.time = audioSource.clip.length * Random.value;
        }
    }

    void Update()
    {
        if (audioSource != null && ignited)
        {
            float timeScale = Time.timeScale;
            if (timeScale != prevTimeScale)
            {
                prevTimeScale = timeScale;
                if (timeScale == 0)
                {
                    audioSource.Pause();
                }
                else
                {
                    audioSource.UnPause();
                    audioSource.pitch = timeScale;
                }
            }
        }
    }

    public void Ignite()
    {
        if (ComponentsFound())
        {
            ParticleSystem.ShapeModule fireShape = fire.shape;
            fireShape.mesh = fireMesh;

            ParticleSystem.ShapeModule smokeShape = smoke.shape;
            smokeShape.mesh = fireMesh;

            fire.Play();
            smoke.Play();

            if (audioSource != null)
            {
                audioSource.Play();
            }

            ignited = true;
        }
    }

    public void Extinguish()
    {
        if (ComponentsFound())
        {
            fire.Stop();
            smoke.Stop();
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            ignited = false;
        }
    }

    public void SetMesh(Mesh mesh)
    {
        fireMesh = mesh;
    }

    private bool ComponentsFound()
    {
        return fire != null && smoke != null;
    }
}
