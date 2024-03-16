using Godot;
using System;
using Novel;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Novel
{
    public class StoryManager
    {
        private readonly Dictionary<string, StoryLine> storyLines = new();
        public StoryLine CurrentStoryLine {get; private set;}
        public StoryStep CurrentStoryStep {get; private set;}
        private static StoryManager instance;
        
        private StoryManager()
        {
            
        }
        
        public static StoryManager GetInstance()
        {
            instance ??= new StoryManager();
            return instance;
        }
        
        public void AddStoryLine(StoryLine storyLine)
        {
            storyLines[storyLine.ID] = storyLine;
        }

        public void RemoveStoryLine(string storyLineID)
        {
            storyLines.Remove(storyLineID);
        }
        
        public void ResetStoryLines()
        {
            storyLines.Clear();
        }

        public StoryLine GetStoryLine(string storyLineID)
        {
            return storyLines.TryGetValue(storyLineID, out StoryLine storyLine) ? storyLine : null;
        }

        public Dictionary<string, StoryLine> getStoryLines()
        {
            return storyLines;
        }

        public bool TryStartStory()
        {
            bool result = false;
            foreach (var storyLine in storyLines.Values.OrderBy(sl => sl.Priority, new DescendingIntComparer()))
            {
                if (PlayerProfile.GetInstance().CheckIsStoryLineCompleted(storyLine.ID))
                {
                    continue;
                }

                StoryStep step = PlayerProfile.GetInstance().FindStepByConditions(storyLine);
                if (step.id != null)
                {
                    CurrentStoryLine = storyLine;
                    CurrentStoryStep = step;
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void ParseStoryLinesFromConfig()
        {
            var file = Godot.FileAccess.Open("res://Resources/story_lines.json", Godot.FileAccess.ModeFlags.Read);
            string allText = file.GetAsText();
            file.Close();

            Dictionary<string, StoryLine> storyLinesData = JsonSerializer.Deserialize<Dictionary<string, StoryLine>>(allText);
        }

        public void SaveStoryLinesToConfig()
        {
            string jsonString = JsonSerializer.Serialize(storyLines);
            var file = Godot.FileAccess.Open("res://Resources/story_lines.json", Godot.FileAccess.ModeFlags.Write);
            file.StoreString(jsonString);
            file.Close();
        }
    }
}