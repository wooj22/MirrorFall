using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<TutorialStep> steps;
    private int currentStepIndex = 0;

    private void Start()
    {
        steps = new List<TutorialStep>()
        {
            //new MoveTutorialStep(),
            //new JumpTutorialStep(),
            //new AttackTutorialStep(),
            //new SkillTutorialStep(),
        };

        steps[0].Enter();
    }

    private void Update()
    {
        if (currentStepIndex >= steps.Count)
            return;

        var currentStep = steps[currentStepIndex];
        currentStep.Update();

        if (currentStep.IsComplete())
        {
            currentStep.Exit();
            currentStepIndex++;

            if (currentStepIndex < steps.Count)
                steps[currentStepIndex].Enter();
            else
                Debug.Log("튜토리얼 완료!");
        }
    }
}
