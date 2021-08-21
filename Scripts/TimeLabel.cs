using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class TimeLabel : Label
    {
        public void OnTimeLeftChanged(int time)
        {
            Text = $"Time : {time}";
        }
    }
}