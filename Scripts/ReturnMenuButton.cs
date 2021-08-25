using Godot;
using System;

namespace NumberNibbler.Scripts
{
    public class ReturnMenuButton : Button
    {
        private PackedScene _menuScene;

        public override void _Ready()
        {
            _menuScene = GD.Load<PackedScene>("res://MainMenu.tscn");
        }

        public void OnReturnMenuPressed()
        {
            var scene = _menuScene.Instance();
            this.TransitionToScene(scene);
            if (GetTree().Paused)
            {
                GetTree().Paused = false;
            }
        }
    }
}