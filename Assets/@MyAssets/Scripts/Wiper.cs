using UnityEngine;
using TMPro;

public class Wiper : MonoBehaviour
{
    public static Wiper Instance;

    public float x = -45;
    public float y = 0;

    public float minAngle = 5f;
    public float maxAngle = 160f;

    public TriangleWaterRemover triangles;
    public ParticleSystem smoke;

    private bool isMoving = false;

    private float currentAngle;
    private float previousAngle;

    private float networkAngle;

    private float estimatedSpeed;

    private float lastPacketTime;

    private int numTriangles;

    public bool isBroken;

    public float lerp1 = 0.05f;
    public float lerp2 = 0.15f;
    public float lerp3 = 0.5f;

    void Start()
    {
        Instance = this;

        currentAngle = maxAngle;
        previousAngle = currentAngle;
        networkAngle = currentAngle;

        numTriangles = triangles.triangleObjects.Length;

        ApplyRotation();
    }

    public void SetAngle(float angle, bool moving)
    {
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        float now = Time.time;
        float deltaTime = now - lastPacketTime;

        if (deltaTime > 0.01f)
        {
            float instantSpeed = (angle - networkAngle) / deltaTime;
            estimatedSpeed = Mathf.Lerp(estimatedSpeed, instantSpeed, lerp3);
            estimatedSpeed = Mathf.Clamp(estimatedSpeed, -40f, 40f);
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
        }

        networkAngle = angle;
        lastPacketTime = now;

        isMoving = moving;
    }

    void Update()
    {
        UpdateMovement();
        if (isMoving) UpdateTriangles();
        ApplyRotation();
    }

    void UpdateMovement()
    {
        if (isMoving)
        {
            if ((currentAngle >= maxAngle && estimatedSpeed>=0) || (currentAngle <= minAngle && estimatedSpeed <= 0))
            {
                estimatedSpeed *= -1;
            }

            currentAngle += estimatedSpeed * Time.deltaTime;

            currentAngle = Mathf.Lerp(currentAngle, networkAngle, lerp1);
        }
        else
        {
            currentAngle = Mathf.Lerp(currentAngle, networkAngle, lerp2);
        }

        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
    }

    void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(x, y, currentAngle);
    }

    void UpdateTriangles()
    {
        int indexActual = numTriangles - 1 - Mathf.Clamp(
                Mathf.RoundToInt(
                    (currentAngle - minAngle) /
                    (maxAngle - minAngle) *
                    (numTriangles - 1)
                ),
                0,
                numTriangles - 1
            );

        int indexPrevio = numTriangles - 1 -
            Mathf.Clamp(
                Mathf.RoundToInt(
                    (previousAngle - minAngle) /
                    (maxAngle - minAngle) *
                    (numTriangles - 1)
                ),
                0,
                numTriangles - 1
            );

        int min = Mathf.Min(indexPrevio, indexActual);
        int max = Mathf.Max(indexPrevio, indexActual);

        for (int i = min; i <= max; i++)
        {
            if (WeatherController.Instance.IsRaining ||
                CarGlassController.instance.visualRain)
            {
                triangles.triangleObjects[i]
                    .GetComponent<TriangleCollisionHandler>()
                    .CleanTriangle();
            }
        }

        previousAngle = currentAngle;
    }
}