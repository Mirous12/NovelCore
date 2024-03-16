using Godot;
using System;
using Novel;
using System.Collections.Generic;
using System.Formats.Asn1;

public partial class StoryPointRedactorController : Panel, IStoryPointGraphListener
{
    private enum EConnectionType
    {
        UNDEFINED,
        ENTER,
        EXIT
    };
    private GraphEdit graph;
    private PackedScene elementTemplate;
    private readonly List<StoryPointGraphController> storyPoints = new();
    private StoryLine workingStoryLine = new();

    private static EConnectionType GetConnectionType( int port )
    {
        return port == 0 ? EConnectionType.ENTER : EConnectionType.EXIT;
    }

    public override void _Ready()
    {
        graph = GetNode<GraphEdit>("GraphEdit");
        elementTemplate = ResourceManager.GetInstance().GetTemplateScene("story_point_template");
    }

    public void OnAddButtonClicked()
    {
        AddElement();
    }

    public void AddElement(string aStoryPointID = "default")
    {
        var element = elementTemplate.Instantiate<StoryPointGraphController>();
        graph.AddChild(element);

        element.StoryPointID = aStoryPointID;
        element.SetListener( this );
        storyPoints.Add(element);
    }

    public void OnConnectionRequest( string aFromNode, int aFromPort, string aToNode, int aToPort )
    {
        if ( aFromPort == aToPort )
        {
            return;
        }

        bool correct = false;

        StoryPointGraphController elementFirst = graph.GetNode<StoryPointGraphController>( aFromNode );
        StoryPointGraphController elementSecond = graph.GetNode<StoryPointGraphController>( aToNode );

        EConnectionType typeForFirst = GetConnectionType( aFromPort );
        EConnectionType typeForSecond = GetConnectionType( aToPort );

        if ( correct )
        {
            graph.ConnectNode( aFromNode, aFromPort, aToNode, aToPort );
        }
    }

    public void OnDisconnectRequest( string aFromNode, int aFromPort, string aToNode, int aToPort )
    {
        graph.DisconnectNode( aFromNode, aFromPort, aToNode, aToPort );
    }

    public void UpdateAndSaveStoryLine()
    {

    }

    public void OnStoryPointGraphClose(string storyPointID)
    {

    }

    public void OnStoryPointGraphCustomiseCharacters(string storyPointID)
    {

    }

    public void OnStoryPointGraphCustomiseConditions(string storyPointID)
    {

    }

    public void OnStoryPointClearConnections( string storyPointID )
    {
        string fromNode = "";

        foreach ( var child in graph.GetChildren() )
        {
            if ( child is StoryPointGraphController storyPoint )
            {
                if ( storyPoint.StoryPointID == storyPointID )
                {
                    fromNode = storyPoint.Name;
                    break;
                }
            }
        }

        foreach ( Godot.Collections.Dictionary connection in graph.GetConnectionList() )
        {
            bool done = false;

            if ( connection["from_node"].AsString() == fromNode &&  ( connection["from_port"].AsInt16() == 0 || connection["from_port"].AsInt16() == 1 ) )
            {
                graph.DisconnectNode( connection["from_node"].AsString(), connection["from_port"].AsInt16(), connection["to_node"].AsString(), connection["to_port"].AsInt16() );
                done = true;
            }
            else if ( connection["to_node"].AsString() == fromNode && ( connection["to_port"].AsInt16() == 0 || connection["to_port"].AsInt16() == 1 ) )
            {
                graph.DisconnectNode( connection["from_node"].AsString(), connection["from_port"].AsInt16(), connection["to_node"].AsString(), connection["to_port"].AsInt16() );
                done = true;
            }

            if ( done )
            {
                StoryPointGraphController elementFirst = graph.GetNode<StoryPointGraphController>( connection["from_node"].AsString() );
                StoryPointGraphController elementSecond = graph.GetNode<StoryPointGraphController>( connection["to_node"].AsString() );

                elementFirst.SetStepAfter( null );
                elementSecond.SetStepBefore( null );
                elementFirst.SetStepBefore( null );
                elementSecond.SetStepAfter( null );
            }
        }
    }
}
