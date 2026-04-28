using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance { get; private set; }
    public bool IsRaining { get => isRaining; set => isRaining = value; }

    [SerializeField] private ParticleSystem[] rainParticles;
    private bool isRaining = false;

    [SerializeField] private WiperManager wiperManager;
    [SerializeField]  private WiperManager2Bit wiperManager2Bit;

    private float secondsChange = 5;
    void Awake()
    {
        Instance = this;
        StartCoroutine(ChangeWeatherCoroutine());
    }

    private IEnumerator ChangeWeatherCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(secondsChange);
            if (isRaining)
            {
                StopRain();
                secondsChange = UnityEngine.Random.Range(3, 6);
            }
            else
            {
                StartRain();
                secondsChange = UnityEngine.Random.Range(5, 15);
            }
        }
    }

    private void StartRain()
    {
        foreach(ParticleSystem rainParticle in rainParticles)
        {
            rainParticle.Play();
        }
        isRaining = true;
        if (wiperManager!=null) wiperManager.WeatherChanged(isRaining);
        if (wiperManager2Bit != null) wiperManager2Bit.WeatherChanged(isRaining);
    }

    private void StopRain()
    {
        foreach (ParticleSystem rainParticle in rainParticles)
        {
            rainParticle.Stop();
        }
        isRaining = false;
        if (wiperManager != null) wiperManager.WeatherChanged(isRaining);
        if (wiperManager2Bit != null) wiperManager2Bit.WeatherChanged(isRaining);
    }

}
