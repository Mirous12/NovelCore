using System;
using Godot;
using System.Collections.Generic;

namespace Novel
{
    public class DebugController
    {
        private NovelSceneScript novelSceneDelegate;
        private StoryLine debugLine;
        private StoryStep currentStep;
        public void SetupNovelSceneDebugging(NovelSceneScript novelScene)
        {
            novelSceneDelegate = novelScene;

            novelSceneDelegate.SetBackgroundByID("BACK_1");

            StoryStep step1 = new()
            {
                id = "step1",
                textID = "TO_TEST_1",
                textAnimationTime = 2.5f,
                conditions = new NovelCondition
                {
                    stepsMustNotBeFinished = new List<string>
                    {
                        "step2", "step1"
                    }
                }
            };

            StoryStep step2 = new()
            {
                id = "step2",
                textID = "TO_TEST_2",
                textAnimationTime = 2.5f,
                conditions = new NovelCondition
                {
                    stepsMustBeFinished = new List<string>
                    {
                        "step1"
                    },
                    stepsMustNotBeFinished = new List<string>
                    {
                        "step3", "step2"
                    }
                }
            };

            StoryStep step3 = new()
            {
                id = "step3",
                textID = "TO_TEST_3",
                textAnimationTime = 2.5f,
                conditions = new NovelCondition
                {
                    stepsMustBeFinished = new List<string>
                    {
                        "step2"
                    },
                    stepsMustNotBeFinished = new List<string>
                    {
                        "step3"
                    }
                }
            };

            debugLine = new()
            {
                ID = "debugSL",
                Priority = 0,
                steps = new List<StoryStep>
                {
                    step1, step2, step3
                }
            };

            novelSceneDelegate.ApplyStep(step1);
            currentStep = step1;
        }

        public void OnDelegateEvent(string eventName)
        {
            switch (eventName)
            {
                case "OnClickReceived":
                    PlayerProfile.GetInstance().SetStepCompleted(currentStep.id);
                    StoryStep nextStep = PlayerProfile.GetInstance().FindStepByConditions(debugLine);
                    if ( nextStep.id != null && nextStep.id != currentStep.id )
                    {
                        novelSceneDelegate.ApplyStep(nextStep);
                        currentStep = nextStep;
                    }
                    else
                    {
                        GD.Print("No next step found!");
                    }
                    PlayerProfile.GetInstance().SaveProfile();
                    break;
                default:
                    break;
            }
        }
    }
}