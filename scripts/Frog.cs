using Godot;
using System;

public class Frog : Area2D
{

    private Level _Level;
    private bool _ReadyToProcessNewActions;
    private Tween _FrogTween;

    public const float FROG_MOVE_DURATION = .6f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _Level = GetParent<Level>();
        _ReadyToProcessNewActions = true;
        _FrogTween = GetNode<Tween>("FrogTween");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2? moveToExecute = null;
        if (_ReadyToProcessNewActions)
        {
            if (Input.IsActionJustPressed("ui_up"))
            {
                moveToExecute = _Level.CanMove(Position, new Vector2(0, -1));
            }
            else if (Input.IsActionJustPressed("ui_down"))
            {
                moveToExecute = _Level.CanMove(Position, new Vector2(0, 1));
            }
            else if (Input.IsActionJustPressed("ui_left"))
            {
                moveToExecute = _Level.CanMove(Position, new Vector2(-1, 0));
            }
            else if (Input.IsActionJustPressed("ui_right"))
            {
                moveToExecute = _Level.CanMove(Position, new Vector2(1, 0));
            }
        }

        // TODO - click based movement, queueing movements longer than 1 step, and diagonal moves

        if (moveToExecute != null)
        {
            _ReadyToProcessNewActions = false;
            _FrogTween.InterpolateProperty(this, "position", Position, moveToExecute, FROG_MOVE_DURATION);
            _FrogTween.InterpolateCallback(this, FROG_MOVE_DURATION, "OnFrogMoveCompleted");
            _FrogTween.Start();
            // TODO start tween, disable canProcess, add callback for reenabling..
        }

        // TODO add eating logic
    }

    public void OnFrogMoveCompleted()
    {
        _ReadyToProcessNewActions = true;
    }
}
