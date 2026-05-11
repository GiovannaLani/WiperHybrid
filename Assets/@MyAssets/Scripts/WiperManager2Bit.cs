using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WiperManager2Bit : MonoBehaviour
{
    public static WiperManager2Bit instance;

    private int motor1 = 0;
    private int motor2 = 0;

    private int m, p = 0;

    private int sensorLeft = 0;
    private int sensorRight = 0;

    private int isRaining = 0;

    public TMP_Text motorText;

    public Wiper wiper;
    private void Start()
    {
        instance = this;
    }
    public void UpdateWiper(string message)
    {
        string[] values = message.Split('&');

        motor1 = int.Parse(values[0]);
        motor2 = int.Parse(values[1]);

        SetMotorText();
        ChangeWiperState();
    }

    private void ChangeWiperState()
    {
        //if(motor1 == 0 && motor2 == 0)
        //{
        //    wiper.Stop();
        //}else if(motor1 == 1 && motor2 == 0)
        //{
        //    wiper.MoveRigth();
        //}
        //else if (motor1 == 0 && motor2 == 1)
        //{
        //    wiper.MoveLeft();
        //}
        //else if (motor1 == 1 && motor2 == 1)
        //{
        //    wiper.Wrong();
        //}
    }

    private void SetMotorText()
    {
        motorText.text = motor1.ToString() + motor2.ToString();
    }

    public void WeatherChanged(bool isRaining)
    {
        SetIsRaining(isRaining ? 1 : 0);
    }

    public void SensorChanged(WiperSensorType type, bool isActive)
    {
        if (type == WiperSensorType.LEFT)
        {
            SetSensorLeft(isActive ? 1 : 0);
        }
        else
        {
            SetSensorRight(isActive ? 1 : 0);
        }
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
    public void SetSensorLeft(int value) => UpdateValue(ref sensorLeft, value);
    public void SetSensorRight(int value) => UpdateValue(ref sensorRight, value);
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
        string message = isRaining + "&" + sensorLeft + "&" + sensorRight + "&" + m + "&" + p;
        Debug.Log("MESSAGE: " + message);

        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalCall("handleMessageFromUnity", message);
        #endif
    }
}
