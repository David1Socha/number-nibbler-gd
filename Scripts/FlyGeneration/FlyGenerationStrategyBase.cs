using Godot;
using System.Collections.Generic;
using System.Linq;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public abstract class FlyGenerationStrategyBase : IFlyGenerationStrategy
    {
        protected int _answer, _numIncorrect, _numCorrect;
        protected readonly RandomNumberGenerator _random;
        protected List<string> _incorrectAnswers, _correctAnswers;
        protected const int INCORRECT_ANSWER_NEGATIVE_DELTA = 1, INCORRECT_ANSWER_POSITIVE_DELTA = 1;
        protected string _difficulty;

        public FlyGenerationStrategyBase(string difficulty)
        {
            _random = new RandomNumberGenerator();
            _random.Randomize();

            var answerRangeForSelectedDifficulty = AnswerRanges[difficulty];
            _difficulty = difficulty;

            _answer = GetRandomAnswer(answerRangeForSelectedDifficulty.minAnswer, answerRangeForSelectedDifficulty.maxAnswer);
            _correctAnswers = GenerateCorrectAnswerPool(_answer).Select(a => ConvertAnswerPoolValueToAnswer(a)).ToList();
            _incorrectAnswers = GenerateIncorrectAnswerPool(_answer).Select(a => ConvertAnswerPoolValueToAnswer(a)).ToList();
        }

        protected virtual int GetRandomAnswer(int minAnswer, int maxAnswer)
        {
            return _random.RandiRange(minAnswer, maxAnswer);
        }

        protected abstract Dictionary<string, (int minAnswer, int maxAnswer)> AnswerRanges { get; }
        public string GetPrompt()
        {
            return $"Make {_answer}";
        }

        public string GenerateCorrectAnswer()
        {
            // TODO could ensure we draw more uniformly from this pool?
            return _correctAnswers[_random.RandiRange(0, _correctAnswers.Count - 1)];
        }

        public string GenerateIncorrectAnswer()
        {
            // TODO could ensure we draw more uniformly from this pool?
            return _incorrectAnswers[_random.RandiRange(0, _incorrectAnswers.Count - 1)];
        }

        protected abstract List<(int term1, int? term2)> GenerateCorrectAnswerPool(int answer);

        protected abstract List<(int term1, int? term2)> GenerateIncorrectAnswerPool(int answer);

        protected abstract string ConvertAnswerPoolValueToAnswer((int term1, int? term2) answer);
    }
}
