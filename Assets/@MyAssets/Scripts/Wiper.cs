using UnityEngine;
using System.Collections;

public class Wiper : MonoBehaviour
{
    public static Wiper Instance;
    public float velocidad = 50f;
    public float anguloMin = -45f;
    public float anguloMax = 45f;
    public float x, y;

    public Transform muelle;
    public float compresionMuelle = 0.3f;
    public float velocidadCompresion = 1f;
    public float velocidadDescompresion = 1f;

    private float anguloActual = 0;
    private float anguloPrevio;
    public int direccion = 1;

    public bool move = false;
    private bool estaEnMuelle = false;

    private Vector3 escalaInicial;
    private Vector3 posicionInicial;

    public Material onMaterial;
    public Material offMaterial;

    private bool changeDirection;

    public bool isBroken;

    public TriangleWaterRemover triangles;
    //public TriangleWaterFader trianglesFader;

    public ParticleSystem smoke;

    private Coroutine isBrokeCoroutine = null;
    void Start()
    {
        Instance = this;
        anguloActual = anguloMax;
        anguloPrevio = anguloActual;
        if (muelle != null)
        {
            escalaInicial = muelle.localScale;
            posicionInicial = muelle.localPosition;
        }
        numTriangles = triangles.triangleObjects.Length;
    }

    private int numTriangles;

    void Update()
    {
        if (isBroken)
        {
            if (!smoke.isPlaying) smoke.Play();
        }
        else
        {
            if (smoke.isPlaying) smoke.Stop();
        }
        if (move)
        {
            anguloActual += direccion * velocidad * Time.deltaTime;

            if ((anguloActual > anguloMax || anguloActual < anguloMin) && changeDirection)
            {
                direccion *= -1;
            }
            anguloActual = Mathf.Clamp(anguloActual, anguloMin, anguloMax);
            if ((anguloActual >= anguloMax && direccion == 1) || (anguloActual <= anguloMin && direccion == -1))
            {
                if (isBrokeCoroutine == null)
                {
                    isBrokeCoroutine = StartCoroutine(IsBrokenRutine());
                }
            }
            else
            {
                if (isBrokeCoroutine != null)
                {
                    StopCoroutine(isBrokeCoroutine);
                    isBrokeCoroutine = null;
                }
                isBroken = false;

                gameObject.transform.localRotation = Quaternion.Euler(x, y, anguloActual);

                int indexActual = numTriangles - 1 - Mathf.Clamp(
    Mathf.RoundToInt((anguloActual - anguloMin) / (anguloMax - anguloMin) * (numTriangles - 1)),
    0,
    numTriangles - 1
);

                int indexPrevio = numTriangles - 1 - Mathf.Clamp(
                    Mathf.RoundToInt((anguloPrevio - anguloMin) / (anguloMax - anguloMin) * (numTriangles - 1)),
                    0,
                    numTriangles - 1
                );

                int min = Mathf.Min(indexPrevio, indexActual);
                int max = Mathf.Max(indexPrevio, indexActual);

                for (int i = min; i <= max; i++)
                {
                    if (WeatherController.Instance.IsRaining || CarGlassController.instance.visualRain)
                    {

                        triangles.triangleObjects[i].GetComponent<TriangleCollisionHandler>().CleanTriangle();
                        //trianglesFader.triangleObjects[i].GetComponent<TriangleFadeHandler>().WetTriangle();
                    }
                    if (i + direccion >= 0 && i + direccion < numTriangles)
                    {
                        //triangles.triangleObjects[i + direccion].GetComponent<TriangleCollisionHandler>().WetTriangle();
                        //trianglesFader.triangleObjects[index + direccion].GetComponent<TriangleFadeHandler>().CleanTriangle();
                    }
                }
                anguloPrevio = anguloActual;

            }

            /*
            if (anguloActual <= anguloMin + 10f && direccion == -1) // Justo antes de tocar el borde derecho
            {
                estaEnMuelle = true; // Está presionando el muelle

                // Comprimir muelle
                float nuevaEscalaY = Mathf.Lerp(muelle.localScale.y, escalaInicial.y * compresionMuelle, Time.deltaTime * velocidadCompresion);
                muelle.localScale = new Vector3(escalaInicial.x, nuevaEscalaY, escalaInicial.z);

                // Ajustar posición para mantener la parte inferior fija
                float desplazamientoY = (escalaInicial.y - nuevaEscalaY) / 2f;
                muelle.localPosition = new Vector3(posicionInicial.x, posicionInicial.y - desplazamientoY, posicionInicial.z);
            }
            else
            {
                // Relajar el muelle lentamente
                float nuevaEscalaY = Mathf.Lerp(muelle.localScale.y, escalaInicial.y, Time.deltaTime * velocidadDescompresion);
                muelle.localScale = new Vector3(escalaInicial.x, nuevaEscalaY, escalaInicial.z);

                // Ajustar posición para mantener la parte inferior fija
                float desplazamientoY = (escalaInicial.y - nuevaEscalaY) / 2f;
                muelle.localPosition = new Vector3(posicionInicial.x, posicionInicial.y - desplazamientoY, posicionInicial.z);

                // Verificar si el muelle está completamente descomprimido
                estaEnMuelle = nuevaEscalaY < escalaInicial.y-0.001; // true si no está completamente relajado
            }
            if (estaEnMuelle)
            {
                muelle.GetComponent<MeshRenderer>().material = onMaterial;
            }
            else
            {
                muelle.GetComponent<MeshRenderer>().material = offMaterial;
            }
            // Mostrar estado en consola (1 = muelle comprimido o relajándose, 0 = completamente relajado)
            //Debug.Log(estaEnMuelle ? 1 : 0);*/
        }
    }

    private IEnumerator IsBrokenRutine()
    {
        yield return new WaitForSeconds(1.5f);
        isBroken = true;
        isBrokeCoroutine = null;
    }
    public void MoveLeft()
    {
        move = true;
        changeDirection = false;
        direccion = -1;
    }
    public void MoveRigth()
    {
        move = true;
        changeDirection = false;
        direccion = 1;
    }

    public void Move()
    {
        move = true;
        changeDirection = true;
    }
    public void Stop()
    {
        move = false;
        isBroken = false;
        if (isBrokeCoroutine != null)
        {
            StopCoroutine(isBrokeCoroutine);
            isBrokeCoroutine = null;
        }
        int index = numTriangles - 1 - Mathf.Clamp(Mathf.RoundToInt((anguloActual - anguloMin) / (anguloMax - anguloMin) * (numTriangles - 1)), 0, numTriangles - 1);
        triangles.triangleObjects[index].GetComponent<TriangleCollisionHandler>().WetTriangle();
    }

    public void Wrong()
    {
        move = false;
        isBroken = true;
        int index = numTriangles - 1 - Mathf.Clamp(Mathf.RoundToInt((anguloActual - anguloMin) / (anguloMax - anguloMin) * (numTriangles - 1)), 0, numTriangles - 1);
        triangles.triangleObjects[index].GetComponent<TriangleCollisionHandler>().WetTriangle();
    }

}
