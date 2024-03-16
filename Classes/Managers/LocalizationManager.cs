using System;
using Godot;
using System.Collections.Generic;

namespace Novel
{
    public class LocalizationManager
    {
        private Dictionary<string, string> localizedTexts = new();
        private static LocalizationManager instance = null;
        private string currentLanguage = "en";

        public static LocalizationManager GetInstance()
        {
            instance ??= new();
            return instance;
        }

        public LocalizationManager()
        {
            List<string> availableLangs = GetAvailableLanguages();
            if ( availableLangs.Count > 0 )
            {
                currentLanguage = availableLangs[0];
            }
            else
            {
                GD.Print("No available languages found!");
            }
            LoadLocalizedTexts();
        }

        public void LoadLocalizedTexts()
        {
            localizedTexts.Clear();
            var file = Godot.FileAccess.Open("res://Resources/Localization/" + currentLanguage + ".json", Godot.FileAccess.ModeFlags.Read);
            string resFromFile = file.GetAsText();
            file.Close();

            localizedTexts = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>( resFromFile );
        }

        public string GetLocalizedText(string textID)
        {
            string result = "";

            if (localizedTexts != null)
            {
                result = localizedTexts.GetValueOrDefault(textID, "");
            }
            else
            {
                GD.Print("Localized texts are not loaded!");
            }

            return result;
        }

        public void SetLanguage(string language)
        {
            currentLanguage = language;
            LoadLocalizedTexts();
        }

        public string GetLanguage() => currentLanguage;

        public List<string> GetAvailableLanguages()
        {
            List<string> result = new();
            var file = Godot.FileAccess.Open("res://Resources/Localization/available_languages.json", Godot.FileAccess.ModeFlags.Read);
            string resFromFile = file.GetAsText();
            file.Close();

            result = System.Text.Json.JsonSerializer.Deserialize<List<string>>( resFromFile );

            return result;
        }
    }
}