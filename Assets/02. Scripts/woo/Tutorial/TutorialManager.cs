using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<TutorialStep> steps;       // Ʃ�丮�� �ܰ� FSM
    private TutorialStep currentStep;       // Ʃ�丮�� ���� Step
    private int currentStepIndex = 0;       // Ʃ�丮�� Step index

    private void Start()
    {
        TutorialStepInit();
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

    // Tutorial Step FSM Init
    private void TutorialStepInit()
    {
        steps = new List<TutorialStep>()
        {
            new Step1(),
        };

        steps[0].Enter();
    }
}
