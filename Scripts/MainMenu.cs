using Godot;
using System;

namespace NumberNibbler.Scripts
{
    public class MainMenu : Node
    {
        private PackedScene _levelScene;
        private OptionButton _categorySelect, _difficultySelect;

        public override void _Ready()
        {
            base._Ready();

            _levelScene = GD.Load<PackedScene>("res://Level.tscn");
            _categorySelect = GetNode<OptionButton>("MainMenuControl/MainMenuBackground/VBoxContainer/HBoxContainer/CategorySelect");
            _difficultySelect = GetNode<OptionButton>("MainMenuControl/MainMenuBackground/VBoxContainer/HBoxContainer/DifficultySelect");
        }

        public void OnPlayGamePressed()
        {
            var scene = _levelScene.Instance<Level>();
            scene.Category = _categorySelect.Text;
            scene.Difficulty = _difficultySelect.Text;
            this.TransitionToScene(scene);
        }
    }
}