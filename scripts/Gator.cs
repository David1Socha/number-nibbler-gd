using Godot;
using System;
using System.Linq;

namespace NumberNibbler.Scripts
{
    public class Gator : Area2D
    {
        [Export]
        private readonly float MOVE_TIME;

        [Export]
        private readonly float POST_MOVE_DELAY_TIME;

        [Export]
        private readonly int GATOR_COLOR_OPTIONS;

        private Tween _gatorTween;
        private Level _level;
        private RandomNumberGenerator _random;
        private AnimatedSprite _sprite;

        private readonly Vector2[] PossibleMoveDeltas = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(1, 1), new Vector2(1, -1), new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 1) };

        public override void _Ready()
        {
            _level = GetParent<Level>();
            _gatorTween = GetNode<Tween>("GatorTween");
            _sprite = GetNode<AnimatedSprite>("GatorSprite");

            _random = new RandomNumberGenerator();
            _random.Randomize();

            _sprite.Frame = _random.RandiRange(0, GATOR_COLOR_OPTIONS - 1);

            QueueRandomMoveAfterDelay();
        }

        public async void QueueRandomMoveAfterDelay()
        {
            await ToSignal(GetTree().CreateTimer(POST_MOVE_DELAY_TIME), "timeout");

            MoveGatorRandomly();
        }

        public void MoveGatorRandomly()
        {
            var validMoves = PossibleMoveDeltas
                .Select(move => (gridDestination: _level.CanMove(Position, move), delta: move))
                .Where(move => move.gridDestination.HasValue)
                .ToList();
            int moveToRandomlySelect = _random.RandiRange(0, validMoves.Count - 1);
            var selectedMove = validMoves[moveToRandomlySelect];
            var destination = _level.MapToWorld(validMoves[moveToRandomlySelect].gridDestination.Value);

            float moveDuration = selectedMove.delta.x != 0 && selectedMove.delta.y != 0 ? (MOVE_TIME * (float)Math.Sqrt(2)) : MOVE_TIME;

            _gatorTween.InterpolateProperty(this, "position", Position, destination, moveDuration);
            _gatorTween.InterpolateCallback(this, moveDuration, nameof(OnGatorMoveCompleted));
            _gatorTween.Start();
        }

        public void OnGatorMoveCompleted()
        {
            QueueRandomMoveAfterDelay();
        }
    }
}