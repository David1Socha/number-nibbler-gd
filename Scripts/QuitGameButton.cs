using Godot;

namespace NumberNibbler.Scripts
{
    public class QuitGameButton : Button
    {
        public void OnQuitTriggered()
        {
            this.Quit();
        }
    }
}