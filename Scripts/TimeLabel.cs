using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class TimeLabel : Label
    {
        private readonly Color _clear = new Color(0, 0, 0, 0);
        private Color _danger;
        private StyleBoxFlat _styleBox;

        public override void _Ready()
        {
            _styleBox = GetStylebox("normal") as StyleBoxFlat;
            _danger = _styleBox.BorderColor;
        }

        public void OnTimeLeftChanged(int time)
        {
            Text = $"Time : {time}";
        }

        public void OnTimeLowChanged(bool isTimeLow)
        {
            _styleBox.BorderColor = isTimeLow ? _danger : _clear;
        }
    }
}