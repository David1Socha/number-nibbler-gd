using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class TimeLabel : Label
    {
        public override void _Ready()
        {

        }

        public void OnTimeLeftChanged(int time)
        {
            Text = $"Time : {time}";
        }
    }
}