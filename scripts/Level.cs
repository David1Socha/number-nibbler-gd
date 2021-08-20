using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Level : Node2D
{
    [Export]
    private readonly float ENEMY_SPAWN_TIME_DELAY_BASE;

    /// <summary>
    /// added and substracted from base spawn delay to determine range of possible values
    /// </summary>
    [Export]
    private readonly float ENEMY_SPAWN_TIME_DELAY_NOISE;

    [Export]
    private readonly float ENEMY_SPAWN_TIME_DELAY_AFTER_WARNING;

    [Export]
    private readonly int MINIMUM_CORRECT_ANSWER_COUNT;

    [Export]
    private readonly int MAXIMUM_CORRECT_ANSWER_COUNT;

    private float _enemySpawnTimeDelay;
    private AudioStreamPlayer _spawnWarningSound;
    private Line2D _warningBox;
    private Rect2 _GridRect;
    private TileMap _LilyGrid;
    private PackedScene _GatorPackedScene, _FlyPackedScene;
    private Area2D _Gator;
    private Vector2 _spawnWorldLocation;
    private RandomNumberGenerator _random;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _random = new RandomNumberGenerator();
        _random.Randomize();

        _LilyGrid = GetNode<TileMap>("LilyGrid");
        _GridRect = _LilyGrid.GetUsedRect();
        _spawnWarningSound = GetNode<AudioStreamPlayer>("SpawnWarningSound");
        _warningBox = GetNode<Line2D>("SpawnWarningBox");
        _GatorPackedScene = GD.Load<PackedScene>("res://Gator.tscn");
        _FlyPackedScene = GD.Load<PackedScene>("res://Fly.tscn");

        SpawnAllFlies();
        //TODO start fly buzz timer/logic
        SpawnEnemyAfterDelay();
    }

    private void SpawnAllFlies()
    {
        var numberOfCorrectAnswersToSpawn = _random.RandiRange(MINIMUM_CORRECT_ANSWER_COUNT, MAXIMUM_CORRECT_ANSWER_COUNT);
        var addedFlies = new List<Fly>();

        for (int i = (int)_GridRect.Position.y; i < (int)_GridRect.End.y; i++)
        {
            for (int j = (int)_GridRect.Position.x; j < (int)_GridRect.End.x; j++)
            {
                var spawnPosition = _LilyGrid.MapToWorld(new Vector2(j, i));
                var fly = _FlyPackedScene.Instance<Fly>();
                fly.Position = spawnPosition;
                AddChild(fly);
                addedFlies.Add(fly);
            }
        }

        var addedFliesRandomOrder = addedFlies.OrderBy(fly => _random.Randf());
        foreach (var fly in addedFliesRandomOrder)
        {
            if (numberOfCorrectAnswersToSpawn > 0)
            {
                fly.HasCorrectAnswer = true;
                fly.Text = "good";
                numberOfCorrectAnswersToSpawn--;
            }
            else
            {
                fly.Text = "bad";
            }
        }
    }

    private async void SpawnEnemyAfterDelay()
    {
        _enemySpawnTimeDelay = _random.RandfRange(ENEMY_SPAWN_TIME_DELAY_BASE - ENEMY_SPAWN_TIME_DELAY_NOISE, ENEMY_SPAWN_TIME_DELAY_BASE + ENEMY_SPAWN_TIME_DELAY_NOISE);
        await ToSignal(GetTree().CreateTimer(_enemySpawnTimeDelay), "timeout");

        WarnEnemySpawn();

        await ToSignal(GetTree().CreateTimer(ENEMY_SPAWN_TIME_DELAY_AFTER_WARNING), "timeout");

        _warningBox.QueueFree();

        _Gator = _GatorPackedScene.Instance<Area2D>();
        _Gator.Position = _spawnWorldLocation;
        AddChild(_Gator);
    }

    private void WarnEnemySpawn()
    {
        _spawnWarningSound.Play();

        var spawnGridLocationX = _random.RandiRange((int)_GridRect.Position.x, (int)_GridRect.End.x - 1);
        var spawnGridLocationY = _random.RandiRange((int)_GridRect.Position.y, (int)_GridRect.End.y - 1);
        _spawnWorldLocation = _LilyGrid.MapToWorld(new Vector2(spawnGridLocationX, spawnGridLocationY));

        _warningBox.Position = _spawnWorldLocation;
        _warningBox.Visible = true;
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
