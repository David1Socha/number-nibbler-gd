using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class ScoreLabel : Label
    {
        public void OnScoreChanged(int score)
        {
            Text = $"Score : {score}";
        }
    }
}