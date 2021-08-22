using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class LevelLabel : Label
    {
        public void OnLevelChanged(int level)
        {
            Text = $"Level : {level}";
        }
    }
}