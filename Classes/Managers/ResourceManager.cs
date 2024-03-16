using System;
using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace Novel
{
    public class ResourceManager
    {
        private Dictionary<string, string> spritePathes;
        private Dictionary<string, string> templatePathes; // New variable for template paths
        private static ResourceManager instance = null;

        public static ResourceManager GetInstance()
        {
            if ( instance == null )
            {
                instance = new();
                instance.LoadSpriteResources();
                instance.LoadTemplateResources();
            }
            return instance;
        }

        public ResourceManager()
        {
            
        }

        public void LoadSpriteResources()
        {
            var file = Godot.FileAccess.Open("res://Resources/SpriteDB.json", Godot.FileAccess.ModeFlags.Read);
            string resFromFile = file.GetAsText();
            file.Close();

            spritePathes = JsonSerializer.Deserialize<Dictionary<string, string>>( resFromFile );
        }

        public void LoadTemplateResources() // New function to load template resources
        {
            var file = Godot.FileAccess.Open("res://Resources/TemplateDB.json", Godot.FileAccess.ModeFlags.Read);
            string resFromFile = file.GetAsText();
            file.Close();

            templatePathes = JsonSerializer.Deserialize<Dictionary<string, string>>( resFromFile );
        }

        public string GetResourcePath(string resourceID) => spritePathes.GetValueOrDefault(resourceID);
        public Sprite2D GetSpriteByResourceName(string resourceID)
        {
            Sprite2D result = null;

            string path = spritePathes.GetValueOrDefault(resourceID);
            if (path.Length > 0)
            {
                Texture2D texture = (Texture2D)ResourceLoader.Load(path);
                result = new Sprite2D
                {
                    Texture = texture
                };
            }
            else
            {
                GD.Print("Cannot find resource path from resourceID: " + resourceID );
            }

            return result;
        }

        public string GetTemplatePath(string templateID) => templatePathes.GetValueOrDefault(templateID); // New function to get template path
        public PackedScene GetTemplateScene(string templateID) => (PackedScene)ResourceLoader.Load(GetTemplatePath(templateID)); // New function to get template scene

        public void SaveTemplateResources()
        {
            string json = JsonSerializer.Serialize(templatePathes);
            var file = Godot.FileAccess.Open("res://Resources/TemplateDB.json", Godot.FileAccess.ModeFlags.Write);
            file.StoreString(json);
            file.Close();
        }
    }
}