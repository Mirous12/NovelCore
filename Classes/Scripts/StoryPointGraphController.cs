using Godot;
using System;
using Novel;

namespace Novel
{
    public interface IStoryPointGraphListener
    {
        void OnStoryPointGraphClose(string storyPointID);
        void OnStoryPointGraphCustomiseCharacters(string storyPointID);
        void OnStoryPointGraphCustomiseConditions(string storyPointID);
        void OnStoryPointClearConnections(string storyPointID);
    }
}

public partial class StoryPointGraphController : GraphNode
{

    readonly Vector2 collapsedSize = new Vector2(300, 155);
    readonly Vector2 expandedSize = new Vector2(500, 666);
    IStoryPointGraphListener listener = null;
    private LineEdit storyPointIDField;
    private LineEdit musicField;
    private LineEdit textID;
    private LineEdit animationTimeField;
    private LineEdit customIDField;
    private LineEdit backgroundField;
    private Control expandedControl;
    private Control collapsedControl;

    //For choises
    private LineEdit choiseID;
    private LineEdit choiseFirstID;
    private LineEdit choiseSecondID;
    private LineEdit choiseFirstTextID;
    private LineEdit choiseSecondTextID;
    //for choises end

    //Buttons
    private Control buttonExit;
    private Button customiseCharactersButton;
    private Button customiseConditionsButton;
    private Button switchChoiseButton;
    //end buttons

    //For base condition
    private StoryPointGraphController stepBefore;
    private StoryPointGraphController stepAfter;
    //end base condition


    public bool IsDialog = false;

    public string StoryPointID
    {
        get
        {
            return storyPointIDField.Text;
        }
        set
        {
            storyPointIDField.Text ??= "";

            if ( storyPointIDField.Text != value )
            {
                storyPointIDField.Text = value;
            }

            if ( Title != value )
            {
                Title = value;
            }
        }
    }

    public string MusicID
    {
        get
        {
            return musicField.Text;
        }
        set
        {
            musicField.Text = value;
        }
    }

    public string TextID
    {
        get
        {
            return textID.Text;
        }
        set
        {
            textID.Text = value;
        }
    }

    public string AnimationTime
    {
        get
        {
            return animationTimeField.Text;
        }
        set
        {
            animationTimeField.Text = value;
        }
    }

    public string CustomID
    {
        get
        {
            return customIDField.Text;
        }
        set
        {
            customIDField.Text = value;
        }
    }

    public string BackgroundField
    {
        get
        {
            return backgroundField.Text;
        }
        set
        {
            backgroundField.Text = value;
        }
    }

    public override void _Ready()
    {
        collapsedControl = GetNode<Control>("collapsed");
        expandedControl = GetNode<Control>("inside");
        storyPointIDField = expandedControl.GetNode<LineEdit>("text_story_point_id");
        musicField = expandedControl.GetNode<LineEdit>("text_music_id");
        textID = expandedControl.GetNode<LineEdit>("text_id");
        animationTimeField = expandedControl.GetNode<LineEdit>("text_anim_time");
        customIDField = expandedControl.GetNode<LineEdit>("text_custom_id");
        backgroundField = expandedControl.GetNode<LineEdit>("text_background");

        //Buttons
        buttonExit = expandedControl.GetNode<Control>("btn_close");
        customiseCharactersButton = expandedControl.GetNode<Button>("btn_customise_characters");
        customiseConditionsButton = expandedControl.GetNode<Button>("btn_customise_conditions");
        switchChoiseButton = expandedControl.GetNode<Button>("btn_switch_choose");
        //end buttons
    }

    public void SetListener(IStoryPointGraphListener listener)
    {
        this.listener = listener;
    }

    //Buttons callbacks

    private void OnButtonClosePressed()
    {
        listener?.OnStoryPointGraphClose(StoryPointID);
    }

    private void OnButtonCustomiseCharactersPressed()
    {
        listener?.OnStoryPointGraphCustomiseCharacters(StoryPointID);
    }

    private void OnButtonCustomiseConditionsPressed()
    {
        listener?.OnStoryPointGraphCustomiseConditions(StoryPointID);
    }

    private void OnButtonSwitchChoisePressed()
    {
        if (IsDialog)
        {
            switchChoiseButton.Text = "Enable Choose Dialog";
            Control choisePanel = GetNode<Control>("inside/choose_panel");
            choiseID.Visible = false;
            IsDialog = false;
        }
        else
        {
            switchChoiseButton.Text = "Disable Choose Dialog";
            Control choisePanel = GetNode<Control>("inside/choose_panel");
            choiseID.Visible = true;
            IsDialog = true;
        }
    }

    public StoryStep FormStoryStep()
    {
        StoryStep step = new()
        {
            id = StoryPointID,
            musicID = MusicID,
            textID = TextID,
            textAnimationTime = float.Parse(AnimationTime),
            customID = CustomID,
            backgroundID = BackgroundField
        };

        return step;
    }

    public void SwitchCollapseState()
    {
        if (collapsedControl.Visible)
        {
            collapsedControl.Visible = false;
            expandedControl.Visible = true;
            Size = expandedSize;
        }
        else
        {
            collapsedControl.Visible = true;
            expandedControl.Visible = false;
            Size = collapsedSize;
        }
    }

    public void OnStoryPointIDChanged(string newText)
    {
        {
            StoryPointID = storyPointIDField.Text;
        }

        Title = storyPointIDField.Text;
    }

    public void SetStepBefore( StoryPointGraphController aStepBefore )
    {
        stepBefore = aStepBefore;
    }

    public void SetStepAfter( StoryPointGraphController aStepAfter )
    {
        stepAfter = aStepAfter;
    }

    public void ClearConnections()
    {
        listener?.OnStoryPointClearConnections( StoryPointID );
    }
}
