using Godot;
using System;
using System.Threading.Tasks;

public class Level : Node2D
{
    [Export]
    private float ENEMY_SPAWN_TIME_DELAY_BASE;

    /// <summary>
    /// added and substracted from base spawn delay to determine range of possible values
    /// </summary>
    [Export]
    private float ENEMY_SPAWN_TIME_DELAY_NOISE;

    private float _enemySpawnTimeDelay;
    private AudioStreamPlayer _spawnWarningSound;
    private Rect2 _GridRect;
    private TileMap _LilyGrid;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _LilyGrid = GetNode<TileMap>("LilyGrid");
        _GridRect = _LilyGrid.GetUsedRect();
        _spawnWarningSound = GetNode<AudioStreamPlayer>("SpawnWarningSound");

        SpawnEnemyAfterDelay();
    }

    private async void SpawnEnemyAfterDelay()
    {
        var random = new RandomNumberGenerator();
        random.Randomize();

        _enemySpawnTimeDelay = random.RandfRange(ENEMY_SPAWN_TIME_DELAY_BASE - ENEMY_SPAWN_TIME_DELAY_NOISE, ENEMY_SPAWN_TIME_DELAY_BASE + ENEMY_SPAWN_TIME_DELAY_NOISE);

        await ToSignal(GetTree().CreateTimer(_enemySpawnTimeDelay), "timeout");

        WarnEnemySpawn(random);

        // TODO draw rectangle, then sleep 3s before actual spawn

        // TODO spawn gator, hide warningbox (or despawn it entirely :D)
    }

    private void WarnEnemySpawn(RandomNumberGenerator random)
    {
        _spawnWarningSound.Play();

        var spawnGridLocationX = random.RandiRange((int)_GridRect.Position.x, (int)_GridRect.End.x);
        var spawnGridLocationY = random.RandiRange((int)_GridRect.Position.y, (int)_GridRect.End.y);
        var spawnLocation = _LilyGrid.MapToWorld(new Vector2(spawnGridLocationX, spawnGridLocationY));

        var warningBox = GetNode<Line2D>("SpawnWarningBox");
        warningBox.Position = spawnLocation;
        warningBox.Visible = true;
    }

    public Vector2? CanMove(Vector2 currentPos, Vector2 gridMovementDelta)
    {
        var currentGridPos = _LilyGrid.WorldToMap(currentPos);
        var resultingGridPos = currentGridPos + gridMovementDelta;
        return CanMove(resultingGridPos);
    }

    public Vector2? CanMove(Vector2 targetGridCoordinates)
    {
        bool moveWouldExceedBoundaries = targetGridCoordinates.x < _GridRect.Position.x || targetGridCoordinates.y < _GridRect.Position.y || targetGridCoordinates.x >= _GridRect.End.x || targetGridCoordinates.y >= _GridRect.End.y;
        if (!moveWouldExceedBoundaries)
        {
            return targetGridCoordinates;
        }
        else
        {
            return null;
        }
    }

    public Vector2 WorldToMap(Vector2 worldCoordinates)
    {
        return _LilyGrid.WorldToMap(worldCoordinates);
    }

    public Vector2 MapToWorld(Vector2 gridCoordinates)
    {
        return _LilyGrid.MapToWorld(gridCoordinates);
    }
}
