using Godot;
using System;
using Novel;
using System.Runtime.CompilerServices;

public partial class NovelSceneScript : Control
{
    private DebugController debugController;

    private string currentStepID;
    private Label textField;
    private TextAnimator currentTextAnimator;

    public override void _Ready()
    {
        textField = GetNode<Label>("text_panel/main_text_label");
        currentTextAnimator = null;

        ResourceManager.GetInstance().LoadSpriteResources();
        ResourceManager.GetInstance().LoadTemplateResources();

        debugController = new DebugController();
        debugController.SetupNovelSceneDebugging(this);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (currentTextAnimator != null)
        {
            currentTextAnimator.Update(delta);
        }

        if (currentTextAnimator != null && currentTextAnimator.IsAnimationFinished())
        {
            currentTextAnimator = null;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if ( @event.IsPressed() )
        {
            OnClickReceived();
        }
    }

    public void SetNovelStep( StoryStep aStep )
    {
        currentStepID = aStep.id;
        ApplyStep(aStep);
    }

    public void ApplyStep( StoryStep aStep )
    {
        SetTextToTextField(aStep.textID, aStep.textAnimationTime);
    }

    public void SetBackgroundByID(string bgID)
    {
        Sprite2D background = ResourceManager.GetInstance().GetSpriteByResourceName(bgID);
        if (background != null)
        {
            Node oldBack = GetNode("background");
            oldBack?.QueueFree();

            background.Position = new Godot.Vector2();
            background.Name = "background";
            background.Centered = false;
            AddChild(background);
        }
    }

    public void SetTextToTextField(string text, float animationTime)
    {
        if ( animationTime < 0.1f )
        {
            textField.Text = LocalizationManager.GetInstance().GetLocalizedText(text);
        }
        else currentTextAnimator ??= new TextAnimator(textField, LocalizationManager.GetInstance().GetLocalizedText(text), animationTime);
    }

    public void SkipTextAnimation()
    {
        currentTextAnimator?.SkipAnimation();
    }

    public void OnClickReceived()
    {
        if (currentTextAnimator != null)
        {
            currentTextAnimator.SkipAnimation();
        }
        else
        {
            debugController.OnDelegateEvent("OnClickReceived");
        }
    }
}
