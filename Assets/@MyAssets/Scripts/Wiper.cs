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
    public bool isBroken;

    public TMP_InputField smoothInput;
    public float correction = 0.1f;

    private float currentAngle;
    private float previousAngle;
    private float targetAngle;
    private float velocity;
    private bool isMoving;
    private int numTriangles;

    private float lastPacketTime;
    private float lastPacketAngle;

    private float debugTimer;
    private int packetCount;
    private float minInterval = float.MaxValue;
    private float maxInterval = float.MinValue;
    private float sumInterval;
    private float maxDrift;
    private float maxVelocity;

    void Start()
    {
        Instance = this;
        currentAngle = maxAngle;
        previousAngle = maxAngle;
        targetAngle = maxAngle;
        lastPacketAngle = maxAngle;
        lastPacketTime = Time.time;
        numTriangles = triangles.triangleObjects.Length;
        ApplyRotation();

        smoothInput.text = correction.ToString("F2");
        smoothInput.onEndEdit.AddListener(v =>
        {
            if (float.TryParse(v,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out float val))
            {
                float old = correction;
                correction = Mathf.Clamp(val, 0f, 1f);
                Debug.Log($"[Wiper] correction changed {old:F3} -> {correction:F3}");
            }
            else
            {
                Debug.LogWarning($"[Wiper] correction parse FAILED for input: '{v}'");
            }
        });

        Debug.Log($"[Wiper] START | minAngle={minAngle} maxAngle={maxAngle} correction={correction} numTriangles={numTriangles}");
    }

    public void SetAngle(float angle, bool moving)
    {
        float now = Time.time;
        float dt = now - lastPacketTime;
        float rawAngle = angle;

        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        bool clamped = !Mathf.Approximately(rawAngle, angle);

        float newVelocity = velocity;
        string velocityReason = "unchanged";

        if (dt < 0.05f)
        {
            velocityReason = $"dt too small ({dt * 1000:F1}ms), keeping velocity={velocity:F1}";
        }
        else if (dt > 2f)
        {
            newVelocity = 0f;
            velocityReason = $"dt too large ({dt * 1000:F0}ms = packet gap/reconnect, velocity reset to 0";
        }
        else
        {
            newVelocity = (angle - lastPacketAngle) / dt;
            velocityReason = $"dt={dt * 1000:F1}ms angleDelta={angle - lastPacketAngle:F2} -> velocity={newVelocity:F1}deg/s";
        }

        packetCount++;
        if (dt >= 0.05f && dt < 2f)
        {
            minInterval = Mathf.Min(minInterval, dt * 1000f);
            maxInterval = Mathf.Max(maxInterval, dt * 1000f);
            sumInterval += dt * 1000f;
        }
        maxVelocity = Mathf.Max(maxVelocity, Mathf.Abs(newVelocity));

        Debug.Log($"[Wiper] PKT #{packetCount} | raw={rawAngle:F2}{(clamped ? " CLAMPED" : "")} -> angle={angle:F2} moving={moving} | {velocityReason} | current={currentAngle:F2} drift={currentAngle - angle:F2}");

        velocity = newVelocity;
        lastPacketAngle = angle;
        lastPacketTime = now;
        targetAngle = angle;
        isMoving = moving;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        float beforeVelocity = currentAngle;
        currentAngle += velocity * dt;
        float afterVelocity = currentAngle;

        float drift = targetAngle - currentAngle;
        float beforeCorrection = currentAngle;
        currentAngle += drift * correction;
        float correctionApplied = currentAngle - beforeCorrection;

        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        maxDrift = Mathf.Max(maxDrift, Mathf.Abs(drift));

        if (isMoving) UpdateTriangles();
        ApplyRotation();

        debugTimer += dt;
        if (debugTimer >= 1f)
        {
            float avgInterval = packetCount > 0 ? sumInterval / packetCount : 0f;
            string intervalStr = packetCount > 0
                ? $"min={minInterval:F0}ms avg={avgInterval:F0}ms max={maxInterval:F0}ms"
                : "NO PACKETS";

            Debug.Log($"[Wiper] 1s SUMMARY | packets={packetCount} intervals=[{intervalStr}] | " +
                      $"current={currentAngle:F1} target={targetAngle:F1} drift={currentAngle - targetAngle:F2} maxDrift={maxDrift:F2} | " +
                      $"velocity={velocity:F1}deg/s maxVel={maxVelocity:F1} | " +
                      $"correction={correction} lastCorrApplied={correctionApplied:F3} | " +
                      $"isMoving={isMoving} fps={1f / dt:F0}");

            if (packetCount == 0)
                Debug.LogWarning("[Wiper] WARNING: 0 packets in last second — SetAngle not being called. Check network/message routing.");

            if (packetCount > 0 && maxInterval > 1500f)
                Debug.LogWarning($"[Wiper] WARNING: max packet gap {maxInterval:F0}ms — jitter too high, consider raising correction or adding buffer.");

            if (Mathf.Abs(currentAngle - targetAngle) > 15f)
                Debug.LogWarning($"[Wiper] WARNING: drift {currentAngle - targetAngle:F1}deg too large — correction={correction} may be too low, or velocity={velocity:F1} is wrong.");

            if (Mathf.Abs(velocity) > 200f)
                Debug.LogWarning($"[Wiper] WARNING: velocity={velocity:F1}deg/s seems too high — may indicate duplicate/out-of-order packet.");

            if (1f / dt < 20f)
                Debug.LogWarning($"[Wiper] WARNING: fps={1f / dt:F0} very low — animation will look choppy regardless of network.");

            debugTimer = 0f;
            packetCount = 0;
            minInterval = float.MaxValue;
            maxInterval = float.MinValue;
            sumInterval = 0f;
            maxDrift = 0f;
            maxVelocity = 0f;
        }
    }

    void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(x, y, currentAngle);
    }

    void UpdateTriangles()
    {
        int indexActual = numTriangles - 1 - Mathf.Clamp(
            Mathf.RoundToInt((currentAngle - minAngle) / (maxAngle - minAngle) * (numTriangles - 1)),
            0, numTriangles - 1);

        int indexPrevio = numTriangles - 1 - Mathf.Clamp(
            Mathf.RoundToInt((previousAngle - minAngle) / (maxAngle - minAngle) * (numTriangles - 1)),
            0, numTriangles - 1);

        int min = Mathf.Min(indexPrevio, indexActual);
        int max = Mathf.Max(indexPrevio, indexActual);

        for (int i = min; i <= max; i++)
        {
            if (WeatherController.Instance.IsRaining || CarGlassController.instance.visualRain)
                triangles.triangleObjects[i]
                    .GetComponent<TriangleCollisionHandler>()
                    .CleanTriangle();
        }

        previousAngle = currentAngle;
    }
}