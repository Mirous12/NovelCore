using Godot;
using System;
using Novel;
using System.Collections.Generic;

namespace Novel
{
    public interface IStoryLineCreateListener
    {
        void OnStoryLineElementClicked(CreateStoryLineTemplateController element);
    }
}

public partial class CreateStoryLineTemplateController : MarginContainer
{
    private readonly List<IStoryLineCreateListener> listeners = new();
    private Label textIDLabel;
    private Label priorityLabel;
    private bool _selected = false;
    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (_selected != value)
            {
                ColorRect selection = GetNode<ColorRect>("selected");
                if (selection != null)
                {
                    selection.Visible = value;
                }
            }
            _selected = value;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        textIDLabel = GetNode<Label>("main_button/text_label");
        priorityLabel = GetNode<Label>("main_button/priority_label");


    }
    public void InitWithInfo(string textID, int priority)
    {
        if ( textIDLabel == null || priorityLabel == null )
        {
            return;
        }

        textIDLabel.Text = textID;
        priorityLabel.Text = priority.ToString();
    }

    public string GetStoryLineID()
    {
        return textIDLabel.Text;
    }

    public int GetPriority()
    {
        return int.Parse(priorityLabel.Text);
    }

    public void AddListener(IStoryLineCreateListener listener)
    {
        if ( !listeners.Contains(listener) )
        {
            listeners.Add(listener);
        }
    }

    public void RemoveListener(IStoryLineCreateListener listener)
    {
        if ( listeners.Contains(listener) )
        {
            listeners.Remove(listener);
        }
    }

    public void OnElementClicked()
    {
        foreach ( var listener in listeners )
        {
            listener.OnStoryLineElementClicked(this);
        }
    }
}
