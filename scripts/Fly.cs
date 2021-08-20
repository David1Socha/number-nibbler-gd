using Godot;
using System;

public class Fly : Node2D
{
    public bool HasCorrectAnswer { get; set; }
    public string Text { get { return _label.Text; } set { _label.Text = value; } }

    private Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
    }
}
