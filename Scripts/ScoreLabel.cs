using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class ScoreLabel : Label
    {
        public override void _Ready()
        {

        }

        public void OnScoreChanged(int score)
        {
            Text = $"Score : {score}";
        }
    }
}