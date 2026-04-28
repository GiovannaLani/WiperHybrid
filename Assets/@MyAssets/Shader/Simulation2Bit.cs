using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation2Bit : MonoBehaviour
{
    public WiperManager2Bit wiperManager;

    public int motor1, motor2 = -1;
    private int m, p, a, u, rst = 0;
    private int isRaining;
    private int sensorLeft, sensorRight = 0;
    private int previousMotor1, previousMotor2;

    void Start()
    {
        previousMotor1 = motor1;
        previousMotor2 = motor2;
    }

    void Update()
    {
        //if (isRaining == 1) motor1 = 1;
        //else
        //{
        //    if (sensorRight == 1 || sensorLeft == 1)
        //        motor1 = 0;
        //}
        //if (previousMotor1 != motor1 || previousMotor2 != motor2)
        //{
        //    previousMotor1 = motor1;
        //    previousMotor2 = motor2;

            wiperManager.UpdateWiper(motor1 + "&" + motor2);
        //}
    }

    public void receiveMessage(string message)
    {
        string[] values = message.Split('&');
        isRaining = int.Parse(values[0]);
        sensorLeft = int.Parse(values[1]);
        sensorRight = int.Parse(values[2]);
    }
}
