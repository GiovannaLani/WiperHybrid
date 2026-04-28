using System;
using System.Collections;
using UnityEngine;

public enum ButtonType
{
   M, P
}
public class CylinderButton : MonoBehaviour
{
    private float pressDepth = 0.3f;
    public Material normalMaterial;
    public Material pressedMaterial;
    public ButtonType type;
    private Vector3 originalPosition;
    private Renderer rend;

    [SerializeField] private WiperManager wiperManager;
    [SerializeField] private WiperManager2Bit wiperManager2Bit;

    void Start()
    {
        originalPosition = transform.localPosition;
        rend = GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        PressButton();
    }

    private void OnMouseUp()
    {
        ReleaseButton();
    }

    private void PressButton()
    {
        transform.localPosition = originalPosition + new Vector3(0, -pressDepth, 0);
        if (rend != null) rend.material = pressedMaterial;
        if (wiperManager != null) wiperManager.SetButton(1, type);
        if (wiperManager2Bit != null) wiperManager2Bit.SetButton(1, type);
    }

    private void ReleaseButton()
    {
        transform.localPosition = originalPosition;
        if (rend != null) rend.material = normalMaterial;
        if (wiperManager != null) wiperManager.SetButton(0, type);
        if (wiperManager2Bit != null) wiperManager2Bit.SetButton(0, type);
    }

}
