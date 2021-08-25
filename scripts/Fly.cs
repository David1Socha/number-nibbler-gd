using Godot;
using System;

namespace NumberNibbler.Scripts
{
    public class Fly : Node2D
    {
        public bool HasCorrectAnswer { get; set; }
        public string Text { get { return _label.Text; } set { _label.Text = value; } }

        private Label _label;
        private AnimatedSprite _sprite;

        public override void _Ready()
        {
            _label = GetNode<Label>("Label");
            _sprite = GetNode<AnimatedSprite>("FlySprite");
        }

        public void PlayAnimation()
        {
            _sprite.Frame = 0;
            _sprite.Play();
        }
    }
}