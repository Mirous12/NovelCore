using System;
using System.Collections.Generic;
using Godot;
using System.Text.Json;

namespace Novel
{
    //Contains player save data, finished steps and all other information about player's progress
    public class PlayerProfile
    {
        static PlayerProfile instance = null;

        private Dictionary<string, object> profileData = new();
        private string CurrentLocationID { get; set; }

        public static PlayerProfile GetInstance()
        {
            instance ??= new();
            return instance;
        }

        public PlayerProfile()
        {
            LoadProfile();
        }

        public void LoadProfile()
        {
            var file = Godot.FileAccess.Open("user://profile.json", Godot.FileAccess.ModeFlags.Read);
            if (file != null)
            {
                string resFromFile = file.GetAsText();
                file.Close();

                profileData = JsonSerializer.Deserialize<Dictionary<string, object>>( resFromFile );
            }
        }

        public void SaveProfile()
        {
            string jsonString = JsonSerializer.Serialize(profileData);
            var file = Godot.FileAccess.Open("user://profile.json", Godot.FileAccess.ModeFlags.Write);
            file.StoreString(jsonString);
            file.Close();
        }

        public void ResetProfile()
        {
            profileData = new Dictionary<string, object>();
            SaveProfile();
        }

        public void SetData(string key, object value)
        {
            profileData[key] = value;
        }

        public object GetData(string key)
        {
            return profileData[key];
        }

        public string GetString(string key)
        {
            string result = "";
           
            object data = profileData.GetValueOrDefault(key, null);

            if ( data != null )
            {
                if ( data.GetType() == typeof(string) )
                {
                    result = (string)data;
                }
                else
                {
                    GD.Print("No string data for: " + key);
                }
            }

            return result;
        }

        public int GetInt(string key)
        {
            int result = 0;
           
            object data = profileData.GetValueOrDefault(key, null);

            if ( data != null )
            {
                if ( data.GetType() == typeof(int) )
                {
                    result = (int)data;
                }
                else
                {
                    GD.Print("No int data for: " + key);
                }
            }

            return result;
        }

        public Dictionary<string, object> GetInnerDictionary(string key)
        {
            Dictionary<string, object> result = null;
           
            object data = profileData.GetValueOrDefault(key, new object());

            if ( data != null )
            {
                if ( data.GetType() == typeof(Dictionary<string, object>) )
                {
                    result = (Dictionary<string, object>)data;
                }
                else
                {
                    GD.Print("No dictionary data for: " + key);
                }
            }

            return result;
        }

        public void SetStepCompleted(string stepID, bool isCompleted = true)
        {
            Dictionary<string, object> completedSteps = GetInnerDictionary("completed_steps");
            if ( completedSteps == null )
            {
                completedSteps = new Dictionary<string, object>();
                profileData["completed_steps"] = completedSteps;
            }

            completedSteps[stepID] = isCompleted;
            SetData("completed_steps", completedSteps);
        }

        public bool IsStepCompleted(string stepID)
        {
            bool result = false;

            Dictionary<string, object> completedSteps = GetInnerDictionary("completed_steps");

            if ( completedSteps != null && completedSteps.ContainsKey(stepID) )
            {
                return (bool)completedSteps[stepID];
            }

            return result;
        }

        public StoryStep FindStepByConditions(StoryLine storyLine)
        {
            foreach ( StoryStep step in storyLine.steps )
            {
                bool isAvailable = true;

                if ( step.conditions.stepsMustBeFinished != null )
                {
                    foreach ( string stepID in step.conditions.stepsMustBeFinished )
                    {
                        if ( !IsStepCompleted(stepID) )
                        {
                            isAvailable = false;
                            break;
                        }
                    }
                }

                if ( step.conditions.stepsMustNotBeFinished != null )
                {
                    foreach ( string stepID in step.conditions.stepsMustNotBeFinished )
                    {
                        if ( IsStepCompleted(stepID) )
                        {
                            isAvailable = false;
                            break;
                        }
                    }
                }

                if ( step.conditions.availableOnLocations != null )
                {
                    string loc = step.conditions.availableOnLocations.Find(locationID => locationID == CurrentLocationID);
                    if (loc == null && CurrentLocationID != null)
                    {
                        isAvailable = false;
                    }
                }

                if ( step.conditions.needChoises != null )
                {
                    foreach ( KeyValuePair<string, string> choise in step.conditions.needChoises )
                    {
                        string choiseID = choise.Key;
                        string choiseOption = choise.Value;

                        string choosenOption = GetChoosenOption(choiseID);

                        if ( choosenOption != choiseOption )
                        {
                            isAvailable = false;
                            break;
                        }
                    }
                }

                if ( isAvailable )
                {
                    return step;
                }
            }

            return new StoryStep();
        }

        public string GetChoosenOption(string choiseID)
        {
            string result = "";

            Dictionary<string, object> choises = GetInnerDictionary("choises");

            if ( choises != null && choises.ContainsKey(choiseID) )
            {
                return (string)choises[choiseID];
            }

            return result;
        }

        public bool CheckIsStoryLineCompleted(string storyLineID)
        {
            bool result = false;

            Dictionary<string, object> completedStories = GetInnerDictionary("completed_stories");
            if (completedStories != null && completedStories.ContainsKey(storyLineID))
            {
                result = (bool)completedStories[storyLineID];
            }
            
            return result;
        }

        public void SetStoryLineCompleted(string storyLineID, bool isCompleted = true)
        {
            Dictionary<string, object> completedStories = GetInnerDictionary("completed_stories");
            if ( completedStories == null )
            {
                completedStories = new Dictionary<string, object>();
                profileData["completed_stories"] = completedStories;
            }

            completedStories[storyLineID] = isCompleted;
            SetData("completed_stories", completedStories);
        }
    }
}