using Godot;

namespace NumberNibbler.Scripts
{
	public class QuitGameButton : Button
	{
		public override void _Ready()
		{
			base._Ready();
			if (OS.GetName() == "HTML5")
			{
				Disabled = true;
			} else if (OS.GetName() == "iOS") {
				Visible = false;
			}
		}

		public void OnQuitTriggered()
		{
			this.Quit();
		}
	}
}
