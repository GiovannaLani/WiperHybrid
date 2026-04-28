using System;
using System.Collections;
using UnityEngine;

public class TriangleWaterRemover : MonoBehaviour
{
    public Material beamMaterial;
    public Transform startPoint;
    public float baseWidth = 2f;
    public float length = 5f;
    public int segments = 10;
    public float angle = 15f;
    public Color hitColor = Color.white;
    public Color defaultColor = Color.black;

    public GameObject[] triangleObjects;

    void Awake()
    {
        GenerateTriangles();
    }

    void GenerateTriangles()
    {
        triangleObjects = new GameObject[segments];
        float stepAngle = baseWidth / segments;
        Quaternion baseRotation = Quaternion.AngleAxis(-baseWidth / 2, transform.up);
        Quaternion stepRotation = Quaternion.AngleAxis(stepAngle, transform.up);
        Quaternion currentRotation = baseRotation;

        for (int i = 0; i < segments; i++)
        {
            Vector3 p0 = startPoint.position;
            Vector3 p1 = startPoint.position + (currentRotation * (transform.forward * length));
            currentRotation *= stepRotation;
            Vector3 p2 = startPoint.position + (currentRotation * (transform.forward * length));

            GameObject triangle = new GameObject("TriangleSegment" + i);
            triangle.layer = LayerMask.NameToLayer("WiperLayer");
            triangle.transform.parent = transform;
            triangle.transform.localPosition = Vector3.zero;
            triangle.transform.localRotation = Quaternion.identity;

            MeshFilter meshFilter = triangle.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = triangle.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(beamMaterial);

            meshRenderer.material.color = defaultColor;

            Mesh mesh = new Mesh();
            Vector3[] localVertices = new Vector3[] { transform.InverseTransformPoint(p0), transform.InverseTransformPoint(p1), transform.InverseTransformPoint(p2) };
            mesh.vertices = localVertices;
            mesh.triangles = new int[] { 0, 1, 2 };
            mesh.RecalculateNormals();
            mesh.uv = new Vector2[]{
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0.5f, 1)
            };
            meshFilter.mesh = mesh;

            TriangleCollisionHandler handler = triangle.AddComponent<TriangleCollisionHandler>();
            handler.Init(meshRenderer);

            triangleObjects[i] = triangle;
        }
    }
}

public class TriangleCollisionHandler : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private bool isTransitioning = false;
    private float targetCoverAmount = 1.0f;
    private float currentCoverAmount = 0.0f;
    private Coroutine currentCoroutine;
    private Material materialInstance;

    public float transitionSpeed = 0.2f;

    public void Init(MeshRenderer renderer)
    {
        this.meshRenderer = renderer;
        materialInstance = meshRenderer.material;
    }

    public void CleanTriangle()
    { 
        if (isTransitioning && currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); 
            isTransitioning = false;
        }

        materialInstance.SetFloat("_CoverAmount", 2f);
        WetTriangle();
    }

    public void WetTriangle()
    {
        if (!isTransitioning)
        {
            targetCoverAmount = 0.90f;
            currentCoroutine = StartCoroutine(ChangeCoverAmountGradually());
        }
        
    }

    private IEnumerator ChangeCoverAmountGradually()
    {
        isTransitioning = true;
        currentCoverAmount = materialInstance.GetFloat("_CoverAmount");
        while (currentCoverAmount > targetCoverAmount)
        {
            currentCoverAmount = Mathf.Lerp(currentCoverAmount, targetCoverAmount, Time.deltaTime * transitionSpeed);
            materialInstance.SetFloat("_CoverAmount", currentCoverAmount);
            yield return null;
        }

        materialInstance.SetFloat("_CoverAmount", targetCoverAmount);
        isTransitioning = false;
    }

}
