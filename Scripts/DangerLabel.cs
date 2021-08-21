using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class DangerLabel : Label
    {
        public void OnDangerChanged(bool danger)
        {
            Text = danger ? "DANGER" : "";
        }
    }
}