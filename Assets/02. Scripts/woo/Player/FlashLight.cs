using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashLight : MonoBehaviour
{
    [Header("Data")]
    public float curLiahtAngle;
    private int curIndex = 0;
    public float[] lightAngleArr = new float[4];        // inner (out = inner + 20)

    [Header("Asset")]
    public Light2D spotLight;
    public Light2D baseLight;
    [SerializeField] public Transform flashLight;

    // lihgt π‡±‚ ø¨√‚ data
    private float defaultSpotIntensity;
    private float defaultBaseIntensity;
    private float lightEffectDuration = 0.5f;

    // player
    PlayerController player;

    private void Start()
    {
        // init
        curIndex = 0;
        curLiahtAngle = lightAngleArr[curIndex];

        defaultSpotIntensity = spotLight.intensity;
        defaultBaseIntensity = baseLight.intensity;

        // component
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!player.isDie)
        {
            RotationFlashLight();
        }
    }

    /// Flash Light Rotation
    public void RotationFlashLight()
    {
        // mouse pos
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 direction = mousePos - flashLight.position;
        direction.z = 0f;

        // ratation (up set)
        flashLight.up = direction.normalized;
    }


    /*-------------------- Event --------------------*/
    /// Hit Light Logic
    public void HitLightLogic()
    {
        StartCoroutine(HitLightEffectCo());
    }

    /// Angle Down
    public void LightAngleDown()
    {
        // angle set
        curIndex++;
        curLiahtAngle = lightAngleArr[curIndex];

        // update
        spotLight.pointLightInnerAngle = curLiahtAngle;
        spotLight.pointLightOuterAngle = curLiahtAngle + 20f;
    }

    // Hit Right Effect
    IEnumerator HitLightEffectCo()
    {
        // off
        spotLight.intensity = 0;
        baseLight.intensity = 0;

        // angle down
        LightAngleDown();
        yield return new WaitForSeconds(1.5f);

        // on 
        float elapsed = 0f;
        while (elapsed < lightEffectDuration)
        {
            float t = elapsed / lightEffectDuration;
            if (spotLight != null)
                spotLight.intensity = Mathf.Lerp(0f, defaultSpotIntensity, t);
            if (baseLight != null)
                baseLight.intensity = Mathf.Lerp(0f, defaultBaseIntensity, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
