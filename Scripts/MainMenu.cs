using Godot;
using System;

namespace NumberNibbler.Scripts
{
	public class MainMenu : Node
	{
		private PackedScene _levelScene;
		private OptionButton _categorySelect, _difficultySelect;
		private ConfigFile _config;

		public override void _Ready()
		{
			base._Ready();

			_levelScene = GD.Load<PackedScene>("res://Level.tscn");
			_categorySelect = GetNode<OptionButton>("MainMenuControl/MainMenuBackground/VBoxContainer/HBoxContainer/CategorySelect");
			_difficultySelect = GetNode<OptionButton>("MainMenuControl/MainMenuBackground/VBoxContainer/HBoxContainer/DifficultySelect");

			_config = new ConfigFile();
			LoadAndApplySavedMenuSettings();
		}

		public void LoadAndApplySavedMenuSettings()
		{
			_config.Load(Global.Config.FilePath);
			int savedCategoryIndex = (int)_config.GetValue(Global.Config.MenuSettingsSection, Global.Config.CategoryValue, 0);
			int savedDifficultyIndex = (int)_config.GetValue(Global.Config.MenuSettingsSection, Global.Config.DifficultyValue, 0);

			_categorySelect.Selected = savedCategoryIndex;
			_difficultySelect.Selected = savedDifficultyIndex;
		}

		public void OnPlayGamePressed()
		{
			SaveMenuSettings();

			var scene = _levelScene.Instance<Level>();
			scene.Category = _categorySelect.Text;
			scene.Difficulty = _difficultySelect.Text;
			this.TransitionToScene(scene);
		}

		/// <summary>
		/// Saves selected category and difficulty, so we can default to those the next time the user opens the main menu
		/// </summary>
		public void SaveMenuSettings()
		{
			_config.SetValue(Global.Config.MenuSettingsSection, Global.Config.CategoryValue, _categorySelect.GetSelectedId());
			_config.SetValue(Global.Config.MenuSettingsSection, Global.Config.DifficultyValue, _difficultySelect.GetSelectedId());
			_config.Save(Global.Config.FilePath);
		}
	}
}
