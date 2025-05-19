using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<TutorialStep> steps;                        // Ʃ�丮�� �ܰ� FSM
    [SerializeField] private TutorialStep currentStep;       // Ʃ�丮�� ���� Step
    [SerializeField] private int currentStepIndex = 0;       // Ʃ�丮�� Step index

    private void Start()
    {
        // Step FSM
        steps = new List<TutorialStep>()
        {
            new Step1(),
        };

        currentStep = steps[currentStepIndex]; // ���� ���� Step ����
        currentStep.coco = this;               // �� ������ coco �Ҵ�
        currentStep.Enter();                   // ���� ����
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
