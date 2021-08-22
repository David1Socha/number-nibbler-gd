using Godot;

namespace NumberNibbler.Scripts
{
    public class Pause : Control
    {
        private bool _paused;

        public override void _Ready()
        {
            _paused = false;

            base._Ready();
        }

        public void OnPauseTriggered()
        {
            GetTree().Paused = true;
            _paused = true;
            Visible = true;
        }

        public void OnUnpauseTriggered()
        {
            GetTree().Paused = false;
            _paused = false;
            Visible = false;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (@event.IsActionPressed("ui_pause"))
            {
                if (_paused)
                {
                    OnUnpauseTriggered();
                }
                else
                {
                    OnPauseTriggered();
                }
            }
        }
    }
}