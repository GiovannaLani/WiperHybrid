using UnityEngine;
using System.Collections;
using TMPro;

public class Wiper : MonoBehaviour
{
    public static Wiper Instance;
    public float x = -45;
    public float y = 0;

    private bool isMoving = false;
    private float targetAngle;
    private float currentAngle;
    private float previousangle;

    private float anguleMin = 5f;
    private float anguleMax = 160f;

    public bool isBroken;

    public TriangleWaterRemover triangles;

    public ParticleSystem smoke;

    public float lerpSpeed = 25;
    void Start()
    {
        Instance = this;
        numTriangles = triangles.triangleObjects.Length;
        currentAngle = 160;
        targetAngle = currentAngle;
        previousangle = currentAngle;
    }

    private int numTriangles;

    public void SetAngle(float angle, bool isMoving)
    {
        targetAngle = angle;
        this.isMoving = isMoving;
    }

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
        if (isMoving)
        {
            currentAngle = Mathf.MoveTowards( currentAngle, targetAngle, lerpSpeed * Time.deltaTime);

            gameObject.transform.localRotation = Quaternion.Euler( x, y, currentAngle);


            int indexActual = numTriangles - 1 - Mathf.Clamp(Mathf.RoundToInt((currentAngle - anguleMin) / (anguleMax - anguleMin) * (numTriangles - 1)), 0, numTriangles - 1);
            int indexPrevio = numTriangles - 1 - Mathf.Clamp(Mathf.RoundToInt((previousangle - anguleMin) / (anguleMax - anguleMin) * (numTriangles - 1)), 0, numTriangles - 1);

            int min = Mathf.Min(indexPrevio, indexActual);
            int max = Mathf.Max(indexPrevio, indexActual);

            for (int i = min; i <= max; i++)
            {
                if (WeatherController.Instance.IsRaining || CarGlassController.instance.visualRain)
                {

                    triangles.triangleObjects[i].GetComponent<TriangleCollisionHandler>().CleanTriangle();
                }

            }
            previousangle = currentAngle;

        }
    }

}
