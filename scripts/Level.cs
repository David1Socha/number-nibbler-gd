using Godot;
using NumberNibbler.Scripts.FlyGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NumberNibbler.Scripts
{
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

        [Export]
        private readonly int POINTS_FOR_CORRECT_ANSWER;

        [Export]
        private readonly int POINTS_LOST_FOR_WRONG_ANSWER;

        [Export]
        private readonly int HARD_DIFFICULTY_POINTS_MULTIPLIER;

        [Export(PropertyHint.Enum, "Addition,Subtraction,Multiplication,Division,Multiples,Basic")]
        private readonly string CATEGORY;

        [Export(PropertyHint.Enum, "Easy,Hard")]
        private readonly string DIFFICULTY_LEVEL;

        [Export]
        private readonly int INITIAL_TIME_LIMIT;

        [Export]
        private readonly int SECONDS_LOST_PER_LEVEL_INCREASE;

        [Export]
        private readonly int MINIMUM_TIME_LIMIT;

        [Export]
        private readonly float TIME_POINTS_MULTIPLIER;

        [Signal]
        public delegate void ScoreChanged(int score);

        [Signal]
        public delegate void LevelChanged(int level);

        [Signal]
        public delegate void TimeLeftChanged(int time);

        [Signal]
        public delegate void PromptChanged(string prompt);

        [Signal]
        public delegate void DangerChanged(bool danger);

        [Signal]
        public delegate void TimeLowChanged(bool isTimeLow);

        private int _level;
        private float _currentTimeLimit, _timeRemaining;
        private float _enemySpawnTimeDelay;
        private AudioStreamPlayer _warningSound, _levelCompleteSound;
        private Line2D _spawnWarningBox;
        private Rect2 _gridRect;
        private TileMap _lilyGrid;
        private PackedScene _gatorPackedScene, _flyPackedScene, _gameOverScene;
        private Area2D _gator;
        private Frog _frog;
        private Vector2 _spawnWorldLocation;
        private RandomNumberGenerator _random;
        private Fly[][] _flyGrid;
        private IFlyGenerationStrategy _flyGenerationStrategy;
        private bool _isDanger;

        public int Score { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _random = new RandomNumberGenerator();
            _random.Randomize();

            _lilyGrid = GetNode<TileMap>("LilyGrid");
            _gridRect = _lilyGrid.GetUsedRect();
            _warningSound = GetNode<AudioStreamPlayer>("WarningSound");
            _levelCompleteSound = GetNode<AudioStreamPlayer>("LevelCompleteSound");
            _spawnWarningBox = GetNode<Line2D>("SpawnWarningBox");
            _gatorPackedScene = GD.Load<PackedScene>("res://Gator.tscn");
            _flyPackedScene = GD.Load<PackedScene>("res://Fly.tscn");
            _gameOverScene = GD.Load<PackedScene>("res://GameOver.tscn");
            _frog = GetNode<Frog>("Frog");

            Score = 0;
            UpdateScore(0);

            Difficulty = Difficulty ?? DIFFICULTY_LEVEL;
            Category = Category ?? CATEGORY;

            InitializeLevel(level: 1);
        }

        public void InitializeLevel(int level)
        {
            _level = level;
            UpdateLevel(level);

            UpdateDanger(danger: false);
            EmitSignal("TimeLowChanged", false);

            _flyGenerationStrategy = FlyGenerationStrategyFactory.GetFlyGenerationStrategy(Category, Difficulty);
            EmitSignal("PromptChanged", _flyGenerationStrategy.GetPrompt());

            _currentTimeLimit = INITIAL_TIME_LIMIT - (level - 1) * SECONDS_LOST_PER_LEVEL_INCREASE;
            if (_currentTimeLimit < MINIMUM_TIME_LIMIT)
            {
                _currentTimeLimit = MINIMUM_TIME_LIMIT;
            }

            _timeRemaining = _currentTimeLimit;

            SpawnAllFlies();
            //TODO start fly buzz timer/logic here ??
            SpawnEnemyAfterDelay();

            _frog.Position = _lilyGrid.MapToWorld(_gridRect.Position);
            _frog.ReadyToProcessNewActions = true;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _timeRemaining -= delta;
            EmitSignal("TimeLeftChanged", (int)_timeRemaining);

            const float TimeLeftBeforeWarningShown = 11f;

            if (_timeRemaining < TimeLeftBeforeWarningShown && _isDanger == false)
            {
                _warningSound.Play();
                UpdateDanger(danger: true);
                EmitSignal("TimeLowChanged", true);
            }

            if (_timeRemaining <= 0)
            {
                TriggerGameOver();
            }
        }

        private void TriggerGameOver()
        {
            // this could be extracted to it's own class, but we're only saving like 3 values to config so keeping as-is for convenience
            var config = new ConfigFile();
            config.Load(Global.Config.FilePath);
            int highScore = (int)config.GetValue(Difficulty, Category, 0);
            if (Score > highScore)
            {
                highScore = Score;
                config.SetValue(Difficulty, Category, highScore);
                config.Save(Global.Config.FilePath);
            }

            var gameOverScene = _gameOverScene.Instance<GameOver>();
            gameOverScene.Initialize(Category, Difficulty, Score, highScore);
            this.TransitionToScene(gameOverScene);
        }

        private void SpawnAllFlies()
        {
            var numberOfCorrectAnswersToSpawn = _random.RandiRange(MINIMUM_CORRECT_ANSWER_COUNT, MAXIMUM_CORRECT_ANSWER_COUNT);
            var addedFlies = new List<Fly>();

            int startCol = (int)_gridRect.Position.x;
            int startRow = (int)_gridRect.Position.y;
            int endCol = (int)_gridRect.End.x;
            int endRow = (int)_gridRect.End.y;
            _flyGrid = new Fly[endCol][];

            for (int i = startCol; i < endCol; i++)
            {
                _flyGrid[i] = new Fly[endRow];
                for (int j = startRow; j < endRow; j++)
                {
                    var spawnPosition = _lilyGrid.MapToWorld(new Vector2(i, j));
                    var fly = _flyPackedScene.Instance<Fly>();
                    fly.Position = spawnPosition;
                    AddChild(fly);
                    addedFlies.Add(fly);
                    _flyGrid[i][j] = fly;
                }
            }

            var addedFliesRandomOrder = addedFlies.OrderBy(fly => _random.Randf());
            foreach (var fly in addedFliesRandomOrder)
            {
                if (numberOfCorrectAnswersToSpawn > 0)
                {
                    fly.HasCorrectAnswer = true;
                    fly.Text = _flyGenerationStrategy.GenerateCorrectAnswer();
                    numberOfCorrectAnswersToSpawn--;
                }
                else
                {
                    fly.Text = _flyGenerationStrategy.GenerateIncorrectAnswer();
                }
            }
        }

        public void BuzzRandomFly()
        {
            var flyRow = GDUtils.PickRandomElement(_flyGrid, _random);
            var fly = GDUtils.PickRandomElement(flyRow, _random);
            if (fly != null)
            {
                fly.PlayAnimation();
            }
        }

        private bool AreAllCorrectFliesEaten()
        {
            return _flyGrid.All(flyRow => flyRow?.All(fly => fly == null || fly.HasCorrectAnswer == false) ?? true);
        }

        private async void SpawnEnemyAfterDelay()
        {
            _enemySpawnTimeDelay = _random.RandfRange(ENEMY_SPAWN_TIME_DELAY_BASE - ENEMY_SPAWN_TIME_DELAY_NOISE, ENEMY_SPAWN_TIME_DELAY_BASE + ENEMY_SPAWN_TIME_DELAY_NOISE);
            await GDUtils.Wait(this, _enemySpawnTimeDelay);

            WarnEnemySpawn();

            await GDUtils.Wait(this, ENEMY_SPAWN_TIME_DELAY_AFTER_WARNING);

            UpdateDanger(danger: false);
            _spawnWarningBox.Visible = false;

            _gator = _gatorPackedScene.Instance<Area2D>();
            _gator.Position = _spawnWorldLocation;
            AddChild(_gator);
        }

        /// <summary>
        /// Returns null if no fly was present, false if an incorrect fly was eaten, and true if a correct fly was eaten.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool? AttemptToEatAtLocation(int row, int col)
        {
            var flyAtLocation = _flyGrid[row][col];
            _flyGrid[row][col] = null;

            if (flyAtLocation == null)
            {
                // fly has already been eaten. Do nothing
                return null;
            }
            else if (flyAtLocation.HasCorrectAnswer == false)
            {
                UpdateScore(-POINTS_LOST_FOR_WRONG_ANSWER);
                flyAtLocation.QueueFree();
                return false;
            }
            else
            {
                var pointsGained = POINTS_FOR_CORRECT_ANSWER * (Difficulty == Global.Difficulties.Hard ? HARD_DIFFICULTY_POINTS_MULTIPLIER : 1);
                UpdateScore(pointsGained);

                flyAtLocation.QueueFree();
                return true;
            }
        }

        private void UpdateScore(int delta)
        {
            Score += delta;
            // don't let scores go negative (that would be mean...)
            Score = Math.Max(0, Score);

            EmitSignal("ScoreChanged", Score);
        }

        private void UpdateLevel(int level)
        {
            _level = level;
            EmitSignal("LevelChanged", _level);
        }

        private void UpdateDanger(bool danger)
        {
            _isDanger = danger;
            EmitSignal("DangerChanged", _isDanger);
        }

        public bool CheckForLevelCompletion()
        {
            if (AreAllCorrectFliesEaten())
            {
                _levelCompleteSound.Play();
                int pointsGainedFromExtraTime = (int)(_timeRemaining * TIME_POINTS_MULTIPLIER);
                UpdateScore(pointsGainedFromExtraTime);

                CleanupCurrentLevel();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CleanupCurrentLevel()
        {
            GDUtils.CancelAllActiveTimers();
            _gator?.QueueFree();
            _gator = null;
            _spawnWarningBox.Visible = false;
            foreach (var flyRow in _flyGrid)
            {
                if (flyRow != null)
                {
                    foreach (var fly in flyRow)
                    {
                        fly?.QueueFree();
                    }
                }
            }
            InitializeLevel(_level + 1);
        }

        private void WarnEnemySpawn()
        {
            _warningSound.Play();

            var spawnGridLocationX = _random.RandiRange((int)_gridRect.Position.x, (int)_gridRect.End.x - 1);
            var spawnGridLocationY = _random.RandiRange((int)_gridRect.Position.y, (int)_gridRect.End.y - 1);
            _spawnWorldLocation = _lilyGrid.MapToWorld(new Vector2(spawnGridLocationX, spawnGridLocationY));

            UpdateDanger(danger: true);
            _spawnWarningBox.Position = _spawnWorldLocation;
            _spawnWarningBox.Visible = true;
        }

        public Vector2? CanMove(Vector2 currentPos, Vector2 gridMovementDelta)
        {
            var currentGridPos = _lilyGrid.WorldToMap(currentPos);
            var resultingGridPos = currentGridPos + gridMovementDelta;
            return CanMove(resultingGridPos);
        }

        public Vector2? CanMove(Vector2 targetGridCoordinates)
        {
            bool moveWouldExceedBoundaries = targetGridCoordinates.x < _gridRect.Position.x || targetGridCoordinates.y < _gridRect.Position.y || targetGridCoordinates.x >= _gridRect.End.x || targetGridCoordinates.y >= _gridRect.End.y;
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
            return _lilyGrid.WorldToMap(worldCoordinates);
        }

        public Vector2 MapToWorld(Vector2 gridCoordinates)
        {
            return _lilyGrid.MapToWorld(gridCoordinates);
        }

        public void OnFrogHealthChanged(int frogHealth)
        {
            if (frogHealth <= 0)
            {
                TriggerGameOver();
            }
        }
    }
}