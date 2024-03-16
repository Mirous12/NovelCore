using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Novel
{
    public struct NovelLocation
    {

    }

    //Describes conditions to show story step
    public struct NovelCondition
    {
        public List<string> stepsMustBeFinished;
        public List<string> stepsMustNotBeFinished;
        public List<string> availableOnLocations;
        public Dictionary<string, string> needChoises;
        public Dictionary<string, string> customInfo;
    }

    public struct NovelCharacter
    {

    }

    public struct NovelCharacterState
    {

    }

    public struct NovelStepChoose
    {
        public string chooseID;
        public string textID;
        public Dictionary<string, string> options;
    }

    public struct StoryStep
    {
        public string id;
        public string musicID;
        public string textID;
        public float textAnimationTime;
        public string backgroundID;
        public Dictionary<string, NovelCharacterState> characters;
        public NovelCondition conditions;
        public NovelStepChoose choise;
        public string customID;
    }

    public class StoryLine
    {
        public string ID {get; set;}
        public int Priority {get; set;}
        public List<StoryStep> steps;
    }

    public class DescendingIntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x > y)
                    return -1;
                else if (x < y)
                    return 1;
                else
                    return 0;
            }
        }
}