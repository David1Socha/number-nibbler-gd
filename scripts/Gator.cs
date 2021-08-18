using Godot;
using System;

public class Gator : Area2D
{
    [Export]
    private readonly float MOVE_TIME;

    [Export]
    private readonly float POST_MOVE_DELAY_TIME;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
