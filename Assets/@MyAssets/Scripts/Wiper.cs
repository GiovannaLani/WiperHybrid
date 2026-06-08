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

    private float targetAngle;
    private float estimatedSpeed;

    private float lastPacketTime;

    private int numTriangles;

    public bool isBroken;

    public float lerp1 = 0.05f;
    public float lerp2 = 0.15f;
    public float lerp3 = 0.5f;


    public float maxDegreesPerSecond = 120f;
    public float minDeltaForSpeed = 0.05f;
    public float targetLerpSpeed = 8f;

    void Start()
    {
        Instance = this;

        currentAngle = maxAngle;
        previousAngle = currentAngle;
        networkAngle = currentAngle;

        targetAngle = currentAngle;
        numTriangles = triangles.triangleObjects.Length;

        ApplyRotation();
    }

    public void SetAngle(float angle, bool moving)
    {
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        float now = Time.time;
        float deltaTime = now - lastPacketTime;

        if (deltaTime >= minDeltaForSpeed)
        {
            float instantSpeed = (angle - networkAngle) / deltaTime;

            float maxPlausibleSpeed = maxDegreesPerSecond * 1.5f;
            if (Mathf.Abs(instantSpeed) < maxPlausibleSpeed)
            {
                estimatedSpeed = Mathf.Lerp(estimatedSpeed, instantSpeed, lerp3);
                estimatedSpeed = Mathf.Clamp(estimatedSpeed, -maxDegreesPerSecond, maxDegreesPerSecond);
            }
        }

        networkAngle = angle;
        lastPacketTime = now;

        isMoving = moving;
    }

    void Update()
    {
        targetAngle = Mathf.Lerp(targetAngle, networkAngle, targetLerpSpeed * Time.deltaTime);
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

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

            float speedStep = estimatedSpeed * Time.deltaTime;
            speedStep = Mathf.Clamp(speedStep, -maxDegreesPerSecond * Time.deltaTime, maxDegreesPerSecond * Time.deltaTime);

            currentAngle += speedStep;

            currentAngle = Mathf.Lerp(currentAngle, targetAngle, lerp1);

            Debug.Log("current angle: " + currentAngle + "  network angle: " + networkAngle + "   speestep: " + speedStep);
        }
        else
        {
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, lerp2);
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