using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashLight : MonoBehaviour
{
    [Header("Flash Light")]
    public float curLiahtAngle;
    private int curIndex = 0;
    public float[] lightAngleArr = new float[4];        // inner (out = inner + 20)

    [Header("Base Light")]
    public float curBaseRadius;
    private int curBaseIndex = 0;
    public float[] lightRadiusArr = new float[6];       // inner (out = inner + 1.0)

    [Header("Asset")]
    public Light2D spotLight;
    public Light2D baseLight;
    [SerializeField] public Transform flashLight;

    [Header("Base Light Skill")]
    [SerializeField] private float brightDuration = 1f;
    [SerializeField] private float targetInBaseLightRadius;       // inner (out = inner + 0.6)
    [SerializeField] private float targetOutBaseLightRadius;
    private float originInBaseLightRadius;      // 0.4
    private float originOutBaseLightRadius;     // 0.8
    private Coroutine braightCoroutine;
    private float     add_duration; // 양초 스킬 중복 발동시 추가시간

    // Hit lihgt 연출 data
    private float defaultSpotIntensity;
    private float defaultBaseIntensity;
    private float lightEffectDuration = 0.5f;

    // player
    PlayerController player;

    private void Start()
    {
        // init
        curIndex = 0;
        curBaseIndex = 0;
        curLiahtAngle = lightAngleArr[curIndex];
        curBaseRadius = lightRadiusArr[curIndex];

        // effect data
        defaultSpotIntensity = spotLight.intensity;
        defaultBaseIntensity = baseLight.intensity;
        originInBaseLightRadius = baseLight.pointLightInnerRadius;
        originOutBaseLightRadius = baseLight.pointLightOuterRadius;

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

    // ## 보스 save, init data ##
    public int GetCurIndex() { return curIndex; }
    public int GetCurBaseIndex() { return curBaseIndex; }
    public void SetCurIndex(int index) {  
        curIndex = index;
        curLiahtAngle = lightAngleArr[curIndex];

        // update
        spotLight.pointLightInnerAngle = curLiahtAngle;
        spotLight.pointLightOuterAngle = curLiahtAngle + 20f;

    }   

    public void SetCurBaseIndex(int index) { 
        curBaseIndex = index;
        curBaseRadius = lightRadiusArr[curBaseIndex];

        // update
        baseLight.pointLightInnerRadius = curBaseRadius;
        baseLight.pointLightOuterRadius = curBaseRadius + 1.0f;
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

    /*-------------------- Light Expansion --------------------*/
    // 거울조각 먹었을 때 베이스 영역 넓어짐
    public void LightExpansion()
    {
        // angle set
        curBaseIndex++;
        curBaseRadius = lightRadiusArr[curBaseIndex];

        StartCoroutine(LightExpansionCo());
    }

    private IEnumerator LightExpansionCo()
    {
        float duration = 1f;
        float elapsed = 0f;

        float startIn = baseLight.pointLightInnerRadius;
        float startOut = baseLight.pointLightOuterRadius;

        float endIn = curBaseRadius;
        float endOut = curBaseRadius + 1.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            baseLight.pointLightInnerRadius = Mathf.Lerp(startIn, endIn, t);
            baseLight.pointLightOuterRadius = Mathf.Lerp(startOut, endOut, t);

            yield return null;
        }

        // set
        baseLight.pointLightInnerRadius = endIn;
        baseLight.pointLightOuterRadius = endOut;
    }


    /*-------------------- Hit Event --------------------*/
    /// Hit Light Logic
    public void HitLightLogic()
    {
        StartCoroutine(HitLightEffectCo());
    }

    /// Angle Down
    private void LightAngleDown()
    {
        // angle set
        curIndex++;
        curLiahtAngle = lightAngleArr[curIndex];

        // update
        spotLight.pointLightInnerAngle = curLiahtAngle;
        spotLight.pointLightOuterAngle = curLiahtAngle + 20f;
    }

    // Hit Right Effect
    private IEnumerator HitLightEffectCo()
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

    /*-------------------- Bright Skill Event --------------------*/
    /// 양초 스킬 사용시 밝아짐
    public void Brightness()
    {
        if (player.isBright)
        {
            // 스킬 발동중일땐 유지시간
            add_duration += player.brightDurationTime;
            return;
        }
        braightCoroutine = StartCoroutine(BrightnessCo());
        player.isBright = true;
    }

    private IEnumerator BrightnessCo()
    {
        float startIn = baseLight.pointLightInnerRadius;
        float startOut = baseLight.pointLightOuterRadius;

        float endIn = targetInBaseLightRadius;
        float endOut = targetOutBaseLightRadius;

        float elapsed = 0f;
        
        // on
        while (elapsed < brightDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / brightDuration);
            baseLight.pointLightInnerRadius = Mathf.Lerp(startIn, endIn, t);
            baseLight.pointLightOuterRadius = Mathf.Lerp(startOut, endOut, t);
            yield return null;
        }

        // max set
        baseLight.pointLightInnerRadius = endIn;
        baseLight.pointLightOuterRadius = endOut;

        yield return new WaitForSeconds(player.brightDurationTime);
        yield return new WaitForSeconds(add_duration);      // 추가시간

        // off
        elapsed = 0f;
        while (elapsed < brightDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / brightDuration);
            baseLight.pointLightInnerRadius = Mathf.Lerp(endIn, originInBaseLightRadius, t);
            baseLight.pointLightOuterRadius = Mathf.Lerp(endOut, originOutBaseLightRadius, t);
            yield return null;
        }

        // origin set
        baseLight.pointLightInnerRadius = originInBaseLightRadius;
        baseLight.pointLightOuterRadius = originOutBaseLightRadius;

        player.isBright = false;
    }
}
