using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class ResourceSceneController : Control
{
    enum ConfigType
    {
        SPRITE
    }
    private ScrollContainer scroll;
    private PackedScene spriteTemplate;
    private bool needScrollBottom = false;

    public override void _Ready()
    {
        base._Ready();

        spriteTemplate = (PackedScene)ResourceLoader.Load("res://Resources/Template/sprite_template_panel.tscn");
        scroll = GetNode<ScrollContainer>("main_controller/main_scroll");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if ( needScrollBottom )
        {
            scroll.SetDeferred("scroll_vertical", scroll.GetVScrollBar().MaxValue);
            needScrollBottom = false;
        }
    }

    private Panel GetNewSpritePanel()
    {
        return spriteTemplate.Instantiate<Panel>();
    }

    public void OnAddButtonClicked()
    {
        Panel newSpritePanel = GetNewSpritePanel();
        VBoxContainer container = scroll.GetNode<VBoxContainer>("container");
        container?.AddChild( newSpritePanel );
        needScrollBottom = true;
    }

    private void SaveCurrentConfig()
    {
        Dictionary<string, string> resultingInfo = new();

        VBoxContainer container = scroll.GetNode<VBoxContainer>("container");
        if (container != null)
        {
            foreach ( Node child in container.GetChildren() )
            {
                TextEdit resourceIdField = child.GetNode<TextEdit>("resource_id_field");
                TextEdit resourcePathField = child.GetNode<TextEdit>("resource_path_field");
                if ( resourceIdField != null && resourcePathField != null )
                {
                    resultingInfo[ resourceIdField.Text ] = resourcePathField.Text;
                }
            }
        }

        string jsonString = JsonSerializer.Serialize(resultingInfo);
        var file = Godot.FileAccess.Open("res://Resources/SpriteDB.json", Godot.FileAccess.ModeFlags.Write);
        file.StoreString(jsonString);
        file.Close();

        string s = "Cat";
        string nameCat = "Kelly";
        GD.Print("This animal is " + s + " and its name is " + nameCat);
    }

    private void LoadCurrentConfig()
    {
        Dictionary<string, string> resultingInfo;
        string stringFromFile = "";

        var file = Godot.FileAccess.Open("res://Resources/SpriteDB.json", Godot.FileAccess.ModeFlags.Read);
        stringFromFile = file.GetAsText();
        file.Close();

        resultingInfo = JsonSerializer.Deserialize<Dictionary<string, string>>( stringFromFile );
        VBoxContainer container = scroll.GetNode<VBoxContainer>("container");

        foreach( var element in resultingInfo )
        {
            var newSpritePanel = GetNewSpritePanel();
            container?.AddChild( newSpritePanel );

            TextEdit resourceIdField = newSpritePanel.GetNode<TextEdit>("resource_id_field");
            TextEdit resourcePathField = newSpritePanel.GetNode<TextEdit>("resource_path_field");

            if ( resourceIdField != null && resourcePathField != null )
            {
                resourceIdField.Text = element.Key;
                resourcePathField.Text = element.Value;
            }
        }

        needScrollBottom = true;
    }
}
