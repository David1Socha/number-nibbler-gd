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

    private Rect2 _GridRect;
    private TileMap _LilyGrid;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _LilyGrid = GetNode<TileMap>("LilyGrid");
        _GridRect = GetNode<TileMap>("LilyGrid").GetUsedRect();

        SpawnEnemyAfterDelay();
    }

    private async void SpawnEnemyAfterDelay()
    {
        var random = new RandomNumberGenerator();
        _enemySpawnTimeDelay = random.RandfRange(ENEMY_SPAWN_TIME_DELAY_BASE - ENEMY_SPAWN_TIME_DELAY_NOISE, ENEMY_SPAWN_TIME_DELAY_BASE + ENEMY_SPAWN_TIME_DELAY_NOISE);
        GD.Print(_enemySpawnTimeDelay);
        await ToSignal(GetTree().CreateTimer(_enemySpawnTimeDelay), "timeout");
        GD.Print("rawr");
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
