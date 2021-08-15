using Godot;
using System;

public class Level : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Rect2 _GridRect;
    private TileMap _LilyGrid;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _LilyGrid = GetNode<TileMap>("LilyGrid");
        _GridRect = GetNode<TileMap>("LilyGrid").GetUsedRect();
    }

    public Vector2? CanMove(Vector2 currentPos, Vector2 gridMovementDelta)
    {
        var currentGridPos = _LilyGrid.WorldToMap(currentPos);
        var resultingGridPos = currentGridPos + gridMovementDelta;
        bool moveWouldExceedBoundaries = resultingGridPos.x < _GridRect.Position.x || resultingGridPos.y < _GridRect.Position.y || resultingGridPos.x >= _GridRect.End.x || resultingGridPos.y >= _GridRect.End.y;
        if (!moveWouldExceedBoundaries)
        {
            return _LilyGrid.MapToWorld(resultingGridPos);
        }
        else
        {
            return null;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
