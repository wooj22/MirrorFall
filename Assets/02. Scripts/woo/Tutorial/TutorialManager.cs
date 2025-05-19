using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<TutorialStep> steps;       // Æ©Åä¸®¾ó ´Ü°è FSM
    private TutorialStep currentStep;       // Æ©Åä¸®¾ó ÇöÀç Step
    private int currentStepIndex = 0;       // Æ©Åä¸®¾ó Step index

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
