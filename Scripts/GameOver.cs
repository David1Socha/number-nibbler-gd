using Godot;
using System;

namespace NumberNibbler.Scripts
{
    public class GameOver : Node
    {
        private bool _initialized = false;
        private string _category, _difficulty;
        private int _score;
        private PackedScene _menuScene, _levelScene;

        public void Initialize(string category, string difficulty, int score)
        {
            _initialized = true;
            _category = category;
            _difficulty = difficulty;
            _score = score;
        }

        public override void _Ready()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Must call initialize on GameOver scene before adding to scene tree");
            }

            base._Ready();

            _levelScene = GD.Load<PackedScene>("res://Level.tscn");
            //_levelScene = GD.Load<PackedScene>("res://MainMenu.tscn");

            var gameModeLabel = GetNode<Label>("GameOverControl/GameOverBackground/VBoxContainer/GameModeLabel");
            gameModeLabel.Text = $"{_category} - {_difficulty}";

            var scoreLabel = GetNode<Label>("GameOverControl/GameOverBackground/VBoxContainer/ScoreLabel");
            scoreLabel.Text = $"Score : {_score}";

            // TODO fetch high score and display that here as well. will need a new Hboxcontainer and highscore label
        }

        public void OnPlayAgain()
        {
            var levelScene = _levelScene.Instance<Level>();
            levelScene.Category = _category;
            levelScene.Difficulty = _difficulty;
            this.TransitionToScene(levelScene);
        }
    }
}