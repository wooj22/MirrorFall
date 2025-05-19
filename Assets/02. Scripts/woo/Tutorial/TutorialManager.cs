using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header ("Step")]
    private List<TutorialStep> steps;                        // 튜토리얼 단계 FSM
    [SerializeField] private TutorialStep currentStep;       // 튜토리얼 현재 Step
    [SerializeField] private int currentStepIndex = 0;       // 튜토리얼 Step index

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
            else
            {
                Debug.Log("Tutorial Step Clear");
            }
        }
    }
}
