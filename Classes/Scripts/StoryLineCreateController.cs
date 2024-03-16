using Godot;
using System;
using Novel;

public partial class StoryLineCreateController : Panel, Novel.IStoryLineCreateListener
{
    CreateStoryLineTemplateController selectedElement = null;
    ScrollContainer scrollContainer;
    VBoxContainer storyLinesContainer;
    Panel createStoryLinePanel;
    Timer createButtonTimer;
    bool needUpdateScroll = false;
    float clickedTiming = 0.0f;
    float clickedTimingMax = 0.3f;
    PackedScene templateElement = null;

    public override void _Ready()
    {
        ResourceManager.GetInstance().LoadSpriteResources();
        ResourceManager.GetInstance().LoadTemplateResources();
        
        scrollContainer = GetNode<ScrollContainer>("scroll");
        createStoryLinePanel = GetNode<Panel>("panel_on_create");
        createButtonTimer = GetNode<Timer>("create_btn_timer");
        storyLinesContainer = GetNode<VBoxContainer>("scroll/box");
        templateElement = ResourceManager.GetInstance().GetTemplateScene("create_story_line_template");
    }

    public bool IsTimerWorking()
    {
        return createButtonTimer.IsStopped() == false;
    }

    public void OnTimerTimeout()
    {
        Button buttonCreate = GetNode<Button>("panel_on_create/btn_create");
        if (buttonCreate != null)
        {
            buttonCreate.Text = "Create";
            buttonCreate.Disabled = false;
        }

        createButtonTimer.QueueFree();
        createButtonTimer = null;
    }

    public override void _Process(double delta)
    {
        if (needUpdateScroll)
        {
            scrollContainer?.SetDeferred("scroll_vertical", scrollContainer.GetVScrollBar().MaxValue);
            needUpdateScroll = false;
        }

        if ( clickedTiming > 0.0f )
        {
            clickedTiming -= (float)delta;
        }
        else if ( clickedTiming < 0.0f )
        {
            clickedTiming = 0.0f;
        }
    }

    public void UpdateStoryLinesScroll()
    {
        if (scrollContainer != null && storyLinesContainer != null)
        {
            storyLinesContainer.QueueFree();
            storyLinesContainer = new VBoxContainer();
            scrollContainer.AddChild(storyLinesContainer);

            foreach (var storyLine in StoryManager.GetInstance().getStoryLines().Values)
            {
                CreateStoryLineTemplateController storyLinePanel = templateElement.Instantiate<CreateStoryLineTemplateController>();
                storyLinePanel.Name = storyLine.ID;
                storyLinePanel.Visible = true;

                storyLinesContainer.AddChild(storyLinePanel);

                storyLinePanel.AddListener(this);
                storyLinePanel.InitWithInfo(storyLine.ID, storyLine.Priority);
            }

            needUpdateScroll = true;
            selectedElement = null;
        }
    }

    public void OnButtonCreateNewStoryLinePressed()
    {
        if (!createStoryLinePanel.Visible)
        {
            return;
        }

        TextEdit textEditID = GetNode<TextEdit>("panel_on_create/text_id");
        TextEdit textEditPriority = GetNode<TextEdit>("panel_on_create/text_priority");

        if (textEditID != null && textEditPriority != null)
        {
            StoryLine strLine = StoryManager.GetInstance().GetStoryLine(textEditID.Text);

            if (strLine != null)
            {
                Button buttonCreate = GetNode<Button>("panel_on_create/btn_create");
                if (buttonCreate != null)
                {
                    buttonCreate.Text = "Already exists!";
                    buttonCreate.Disabled = true;

                    Timer timer = new Timer();
                    timer.WaitTime = 1.5f;
                    timer.OneShot = true;
                    timer.Name = "create_btn_timer";
                    AddChild(timer);

                    var callable = new Callable(this, "OnTimerTimeout");
                    timer.Connect("timeout", callable);
                    timer.Start();
                }
            }
            else
            {
                string id = textEditID.Text;
                int priority = Convert.ToInt32(textEditPriority.Text);

                StoryLine storyLine = new StoryLine
                {
                    ID = id,
                    Priority = priority
                };

                StoryManager.GetInstance().AddStoryLine(storyLine);

                textEditID.Text = "";
                textEditPriority.Text = "";
                createStoryLinePanel.Visible = false;

                Button addButton = GetNode<Button>("btn_add");
                if (addButton != null)
                {
                    addButton.Disabled = false;
                }

                UpdateStoryLinesScroll();
            }
        }
    }

    public void OnButtonAddPressed()
    {
        if (createStoryLinePanel.Visible)
        {
            return;
        }

        createStoryLinePanel.Visible = true;
    }

    public void OnButtonCancelPressed()
    {
        if (createStoryLinePanel != null && createStoryLinePanel.Visible)
        {
            TextEdit textEditID = GetNode<TextEdit>("panel_on_create/text_id");
            TextEdit textEditPriority = GetNode<TextEdit>("panel_on_create/text_priority");

            if (textEditID != null && textEditPriority != null)
            {
                textEditID.Text = "";
                textEditPriority.Text = "";
            }

            createStoryLinePanel.Visible = false;
            Button addButton = GetNode<Button>("btn_add");
            if (addButton != null)
            {
                addButton.Disabled = false;
            }
        }
    }

    public void OnStoryLineElementClicked(CreateStoryLineTemplateController element)
    {
        UpdateSelectedElement(element);
    }

    public void UpdateSelectedElement(CreateStoryLineTemplateController element)
    {
        if (selectedElement != element)
        {
            if (selectedElement != null)
            {
                selectedElement.Selected = false;
            }

            selectedElement = element;

            if (element != null)
            {
                selectedElement.Selected = true;
            }

            clickedTiming = 0.5f;
        }
        else if ( clickedTiming > 0.0f )
        {
            SwitchToStoryPointsEditor();
        }
        else
        {
            clickedTiming = 0.5f;
        }
    }

    public void OnRemoveItemPressed()
    {
        if (selectedElement != null)
        {
            StoryManager.GetInstance().RemoveStoryLine(selectedElement.GetStoryLineID());
            UpdateStoryLinesScroll();
        }
    }

    public void OnChooseButtonClicked()
    {
        SwitchToStoryPointsEditor();
    }

    public void SwitchToStoryPointsEditor()
    {
        GD.Print("SwitchToStoryPointsEditor");
    }
}
