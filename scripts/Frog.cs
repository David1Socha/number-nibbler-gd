using Godot;
using System;

namespace NumberNibbler.Scripts
{
    public class Frog : Area2D
    {
        private Level _level;
        private bool _readyToProcessNewActions;
        private Tween _frogTween;
        private AudioStreamPlayer _frogMoveSound, _frogDamagedSound, _frogEatSound;
        private Vector2? _targetDestination;
        private AnimatedSprite _animSprite;

        private Vector2 GridPosition { get { return _level.WorldToMap(Position); } }

        [Export]
        private readonly float FROG_MOVE_DURATION;
        private float FROG_DIAG_MOVE_DURATION { get { return FROG_MOVE_DURATION * (float)Math.Sqrt(2); } } // a^2 + b^2 = c^2 ; therefore diagonal distance == sqrt(2) * horizontal distance

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _readyToProcessNewActions = true;

            _level = GetParent<Level>();
            _animSprite = GetNode<AnimatedSprite>("FrogSprite");
            _animSprite.Frame = 2;
            _frogTween = GetNode<Tween>("FrogTween");
            _frogMoveSound = GetNode<AudioStreamPlayer>("FrogMoveSound");
            _frogDamagedSound = GetNode<AudioStreamPlayer>("FrogDamagedSound");
            _frogEatSound = GetNode<AudioStreamPlayer>("FrogEatSound");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {

            // TODO - click based movement, queueing movements longer than 1 step, and diagonal moves


            // TODO add eating logic
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            Vector2 currentDestinationOrPosition = _targetDestination != null ? _level.MapToWorld(_targetDestination.Value) : Position;

            if (@event.IsActionPressed("ui_up"))
            {
                _targetDestination = _level.CanMove(currentDestinationOrPosition, new Vector2(0, -1));
            }
            else if (@event.IsActionPressed("ui_down"))
            {
                _targetDestination = _level.CanMove(currentDestinationOrPosition, new Vector2(0, 1));
            }
            else if (@event.IsActionPressed("ui_left"))
            {
                _targetDestination = _level.CanMove(currentDestinationOrPosition, new Vector2(-1, 0));
            }
            else if (@event.IsActionPressed("ui_right"))
            {
                _targetDestination = _level.CanMove(currentDestinationOrPosition, new Vector2(1, 0));
            }
            else if (@event.IsActionPressed("ui_select"))
            {
                EatIfAble();
            }
            else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == (int)ButtonList.Left)
                {
                    var targetGridCoords = _level.WorldToMap(mouseEvent.Position);

                    if (targetGridCoords == GridPosition)
                    {
                        EatIfAble();
                    }
                    else
                    {
                        _targetDestination = _level.CanMove(targetGridCoords);
                    }
                }
            }

            if (_readyToProcessNewActions)
            {
                if (_targetDestination.HasValue)
                {
                    MoveFrogTowardsTargetDestination();
                }
            }
        }

        private void EatIfAble()
        {
            if (_readyToProcessNewActions)
            {
                var eatResult = _level.AttemptToEatAtLocation((int)GridPosition.x, (int)GridPosition.y);
                if (eatResult != null)
                {
                    _readyToProcessNewActions = false;
                    _animSprite.Frame = 0;
                    _animSprite.Play();
                    _frogEatSound.Play();
                }
            }
        }

        private void OnEatAnimationFinished()
        {
            _readyToProcessNewActions = true;
            _level.CheckForLevelCompletion();
            MoveFrogTowardsTargetDestination();
        }

        private void MoveFrogTowardsTargetDestination()
        {
            const int step = 1;

            if (_targetDestination != null && _targetDestination != GridPosition)
            {
                _frogMoveSound.Play();

                _readyToProcessNewActions = false;

                int xDelta = getDeltaTowardsDestination(_targetDestination.Value.x, GridPosition.x, step);
                int yDelta = getDeltaTowardsDestination(_targetDestination.Value.y, GridPosition.y, step);

                var moveDestination = GridPosition + new Vector2(xDelta, yDelta);
                float moveDuration = xDelta != 0 && yDelta != 0 ? FROG_DIAG_MOVE_DURATION : FROG_MOVE_DURATION;
                var destinationWorldCoordinates = _level.MapToWorld(moveDestination);

                _frogTween.InterpolateProperty(this, "position", Position, destinationWorldCoordinates, moveDuration);
                _frogTween.InterpolateCallback(this, moveDuration, nameof(OnFrogMoveCompleted));
                _frogTween.Start();
            }
        }

        public int getDeltaTowardsDestination(float destination, float current, int step)
        {
            int delta;
            switch ((int)(destination - current))
            {
                case int n when n > 0:
                    delta = Math.Min(step, n);
                    break;
                case int n when n < 0:
                    delta = Math.Max(-step, n);
                    break;
                default:
                    delta = 0;
                    break;
            }

            return delta;
        }

        public void OnFrogMoveCompleted()
        {
            if (_targetDestination == GridPosition || _targetDestination == null)
            {
                _readyToProcessNewActions = true;
                _targetDestination = null;
            }
            else
            {
                MoveFrogTowardsTargetDestination();
            }

        }

        public void OnFrogEntered(Area2D other)
        {
            _frogDamagedSound.Play();
            GD.Print("game over !"); // TODO rm this, add gameover/hp loss logic
        }
    }
}