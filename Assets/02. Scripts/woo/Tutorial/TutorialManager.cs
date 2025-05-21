using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header ("Step")]
    private List<TutorialStep> steps;                        // Ʃ�丮�� �ܰ� FSM
    [SerializeField] private TutorialStep currentStep;       // Ʃ�丮�� ���� Step
    [SerializeField] private int currentStepIndex = 0;       // Ʃ�丮�� Step index
    public bool isClear;

    [Header("Object")]
    [SerializeField] public TutorialPlayer player;         
    [SerializeField] public TutorialAI ai;
    [SerializeField] public GameObject apple;
    [SerializeField] public Transform aiRespawnPos; 

    [Header("MapCollider")]
    [SerializeField] public BoxCollider2D zone2;
    [SerializeField] public BoxCollider2D zone3;
    [SerializeField] public BoxCollider2D zone4;
    [SerializeField] public BoxCollider2D zone5;

    [Header("UI")]
    [SerializeField] public Text playerText;
    [SerializeField] public Text aiText;
    [SerializeField] public Text narrationText;
    [SerializeField] public Image player_background;
    [SerializeField] public Image ai_background;
    [SerializeField] public Image narration_background;

    private Coroutine tutorialClearCo;

    private void Start()
    {
        ai.playerPos = player.gameObject.transform;
        ai.reSpawnPos = aiRespawnPos;

        // Step FSM
        steps = new List<TutorialStep>()
        {
            new Step1(this, this)
            , new Step2(this, this)
            , new Step3(this, this)
            , new Step4(this, this)
            , new Step5(this, this)
            , new Step6(this, this)
            , new Step7(this, this)
        };

        currentStep = steps[currentStepIndex];
        currentStep.Enter();

        SoundManager2.Instance.SetBGM("BGM_InGame");
        SoundManager2.Instance.FadeInBGM();
    }

    private void Update()
    {
        if (currentStepIndex >= steps.Count)
            return;

        // update
        currentStep.Update();

        // next step
        if (currentStep.IsComplete())
        {
            currentStep.Exit();
            currentStepIndex++;

            if (currentStepIndex < steps.Count)
            {
                currentStep = steps[currentStepIndex];
                currentStep.Enter();
            }
        }
    }

    public void PlayerScriptUpdate(string text)
    {
        player_background.enabled = true;
        playerText.text = text;
    }

    public void AiScriptUpdate(string text)
    {
        ai_background.enabled = true;
        aiText.text = text;
    }

    public void NarrationScriptUpdate(string text)
    {
        narration_background.enabled = true;
        narrationText.text = text;
    }

    public void PlayerScriptOff()
    {
        player_background.enabled = false;
        playerText.text = "";
    }

    public void AiScriptOff()
    {
        ai_background.enabled = false;
        aiText.text = "";
    }

    public void NarrationScriptOff()
    {
        narration_background.enabled = false;
        narrationText.text = "";
    }

    // Ʃ�丮�� ��ŵ
    public void TutorialSkip()
    {
        isClear = true;
        ai.gameObject.SetActive(false);
        FadeManager.Instance.FadeOutSceneChange("04_Play0");
    }

    // Ʃ�丮�� Ŭ���� (Tutorial WarpMirror ������)
    public void TutorialClear()
    {
        if (tutorialClearCo == null)
        {
            isClear = true;
            ai.gameObject.SetActive(false);
            tutorialClearCo = StartCoroutine(TuTorialLastCo());
            Debug.Log("Tutorial Step Clear");
        }
    }

    IEnumerator TuTorialLastCo()
    {
        narrationText.text = "";
        FadeManager.Instance.FadeOut();
        yield return new WaitForSeconds(1.2f);

        aiText.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        aiText.fontSize = 45;
        aiText.text = "�ƴ�? ��� ���� ������ �� �󱼷� ����ٰ�.. \n����ó�� ���ڰ�";
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_6");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());
        yield return new WaitForSeconds(2f);
        SceneSwitch.Instance.SceneSwithcing("04_Play0");
    }
}
