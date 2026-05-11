using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum WiperSensorType
{
    LEFT, RIGHT
}
public class WiperSensor : MonoBehaviour
{
    public TMP_Text sensorText;
    public Material onMaterial;
    public Material offMaterial;
    public WiperSensorType sensorType;
    private Renderer sensorRenderer;
    public WiperManager wiperManager;
    public WiperManager2Bit wiperManager2bit;

    private bool isBroken = false;
    private bool isActive = false; 
    private void Awake()
    {
        sensorRenderer = GetComponent<Renderer>();
        Debug.Log(sensorRenderer);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Limpiaparabrisa"))
    //    {
    //        SetSensor(true);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Limpiaparabrisa"))
    //    {
    //        SetSensor(false);
    //    }
    //}

    public void SetSensor(bool isOn)
    {
        isActive = isOn;
        if (isOn || isBroken)
        {
            sensorText.text = "1";
            sensorRenderer.material = onMaterial;
            //if (wiperManager !=null) wiperManager.SensorChanged(sensorType, true);
            //if (wiperManager2bit != null) wiperManager2bit.SensorChanged(sensorType, true);
        }
        else
        {
            sensorText.text = "0";
            sensorRenderer.material = offMaterial;
            //if (wiperManager != null) wiperManager.SensorChanged(sensorType, false);
            //if (wiperManager2bit != null) wiperManager2bit.SensorChanged(sensorType, false);
        }
    }

    public void SetIsBroken(bool isBroken)
    {
        this.isBroken = isBroken;
        SetSensor(isActive);
    }
}
