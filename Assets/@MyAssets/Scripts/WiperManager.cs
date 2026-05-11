using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WiperManager : MonoBehaviour
{
    public static WiperManager instance;

    private int motor1 = 0;

    private int m, p = 0;

    public WiperSensor sensorLeft;
    public WiperSensor sensorRight;

    private int isRaining = 0;

    public TMP_Text motorText;

    public Wiper wiper;

    public int error = 0;

    private void Start()
    {
        instance = this;
        sensorLeft.SetSensor(true);
    }

    public void UpdateWiper(string message)
    {
        string[] values = message.Split('&');

        motor1 = int.Parse(values[0]);
        float angle = float.Parse(values[1]);
        int leftSensor = int.Parse(values[2]);
        int rightSensor = int.Parse(values[3]);


        SetMotorText();
        sensorLeft.SetSensor(leftSensor == 1);
        sensorRight.SetSensor(rightSensor == 1);

        wiper.SetAngle(angle, motor1==1);
    }

    private void SetMotorText()
    {
        motorText.text = motor1.ToString();
    }

    public void WeatherChanged(bool isRaining)
    {
        SetIsRaining(isRaining ? 1 : 0);
    }

    void UpdateValue(ref int variable, int newValue)
    {
        if (variable != newValue)
        {
            variable = newValue;
            SendMessage();
        }
    }

    public void SetIsRaining(int value) => UpdateValue(ref isRaining, value);

    public void SetButton(int value, ButtonType type)
    {
        switch (type)
        {
            case ButtonType.M:
                UpdateValue(ref m, value);
                break;
            case ButtonType.P:
                UpdateValue(ref p, value);
                break;
        }
    }

    void SendMessage()
    {
        string message = isRaining + "&" +  m + "&" + p + "&" + error ;
        Debug.Log("MESSAGE: " + message);
        #if UNITY_WEBGL && !UNITY_EDITOR
                    Application.ExternalCall("handleMessageFromUnity", message);
        #endif
    }

    public void triggerError()
    {
        UpdateValue(ref error, 1);
    }

    public void resetError()
    {
        UpdateValue(ref error, 0);
    }
}
