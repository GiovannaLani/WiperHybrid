using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGlassController : MonoBehaviour
{
    [SerializeField] private Material glassMaterial;
    [SerializeField] private Wiper wiper;
    public float dropletsStrengthMin = 0;
    public float rivuletStrengthMin = 0;
    public float grazingStrengthMin = 0;
    public float dropletsStrengthMax = 0.98f;
    public float rivuletStrengthMax = 10;
    public float grazingStrengthMax = 0.235f;

    private float currentDropletStrength;
    private float currentRivuletStrength;
    private float currentGrazingStrength;
    public float transitionSpeed = 3.0f;

    private float targetDropletsStrength;
    private float targetRivuletStrength;
    private float targetGrazingtrength;

    public static CarGlassController instance;
    public bool visualRain = false;
    private void Start()
    {
        instance = this;
    }
    void Update()
    {
        if (WeatherController.Instance.IsRaining)
        {
            targetDropletsStrength = dropletsStrengthMax;
            targetRivuletStrength = rivuletStrengthMax;
            targetGrazingtrength = grazingStrengthMax;
            visualRain = true;
        }
        else
        {
            targetDropletsStrength = dropletsStrengthMin;
            targetRivuletStrength = rivuletStrengthMin;
            targetGrazingtrength = grazingStrengthMin;
            if(Mathf.Abs(currentGrazingStrength- targetGrazingtrength)<=0.1)
            {
                visualRain = false;
            }
        }
        currentDropletStrength = Mathf.Lerp(currentDropletStrength, targetDropletsStrength, Time.deltaTime * transitionSpeed);
        currentRivuletStrength = Mathf.Lerp(currentRivuletStrength, targetRivuletStrength, Time.deltaTime * transitionSpeed *3);
        currentGrazingStrength = Mathf.Lerp(currentGrazingStrength, targetGrazingtrength, Time.deltaTime * transitionSpeed);
        glassMaterial.SetFloat("_Droplets_Strength", currentDropletStrength);
        glassMaterial.SetFloat("_RivuletsStrength", currentRivuletStrength);
        glassMaterial.SetFloat("_grazingTerm", currentGrazingStrength);
        
    }
}
