using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<TutorialStep> steps;                        // 튜토리얼 단계 FSM
    [SerializeField] private TutorialStep currentStep;       // 튜토리얼 현재 Step
    [SerializeField] private int currentStepIndex = 0;       // 튜토리얼 Step index

    private void Start()
    {
        // Step FSM
        steps = new List<TutorialStep>()
        {
            new Step1(),
        };

        currentStep = steps[currentStepIndex]; // 먼저 현재 Step 설정
        currentStep.coco = this;               // 그 다음에 coco 할당
        currentStep.Enter();                   // 상태 진입
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
            currentStep = steps[++currentStepIndex];

            if (currentStepIndex < steps.Count)
                steps[currentStepIndex].Enter();
            else
                Debug.Log("Tutorial Step Clear");
        }
    }
}
