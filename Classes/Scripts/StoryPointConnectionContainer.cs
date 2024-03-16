using Godot;
using System;

public partial class StoryPointConnectionContainer : Container
{
    private Label labelEnter;
    private Label labelExit;
    private Button buttonClear;

    public override void _Ready()
    {
        labelEnter = GetNode<Label>( "label_enter" );
        labelExit = GetNode<Label>( "label_exit" );
        buttonClear = GetNode<Button>( "btn_clear_connections" );
    }

    public void OnSortChildren()
    {
        labelEnter?.SetPosition( new Vector2( 0, 0 ) );
        labelExit?.SetPosition( new Vector2( Size.X - labelExit.Size.X, 0 ) );
        buttonClear?.SetPosition( new Vector2( Size.X / 2 - buttonClear.Size.X / 2, 0 ) );
    }
}
