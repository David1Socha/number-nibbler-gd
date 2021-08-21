using Godot;
using System;
using System.Collections.Generic;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public class MultiplicationFlyGenerationStrategy : FlyGenerationStrategyBase
    {
        // since we override GetRandomAnswer, this doesn't really do anything...
        protected override Dictionary<string, (int minAnswer, int maxAnswer)> AnswerRanges
        {
            get
            {
                return new Dictionary<string, (int minAnswer, int maxAnswer)>()
                {
                    { Global.Difficulties.Easy, (minAnswer: 4, maxAnswer: 20) },
                    { Global.Difficulties.Hard, (minAnswer: 8, maxAnswer: 30) }
                };
            }
        }

        protected override int GetRandomAnswer(int minAnswer, int maxAnswer)
        {
            if (_difficulty == Global.Difficulties.Easy)
            {
                int[] easyAnswerOptions = new int[] { 8, 12, 16, 18, 20 };
                return easyAnswerOptions[_random.RandiRange(0, easyAnswerOptions.Length - 1)];
            }
            else
            {
                int[] hardAnswerOptions = new int[] { 12, 18, 20, 24, 30 };
                return hardAnswerOptions[_random.RandiRange(0, hardAnswerOptions.Length - 1)];
            }
        }

        public MultiplicationFlyGenerationStrategy(string difficulty) : base(difficulty)
        {
        }

        protected override List<(int term1, int? term2)> GenerateCorrectAnswerPool(int answer)
        {
            var correctAnswers = new List<(int term1, int? term2)>();

            int previousLargeFactor = answer;

            for (int smallFactor = 1; smallFactor <= Math.Sqrt(answer); smallFactor++)
            {
                for (int largeFactor = previousLargeFactor; largeFactor >= smallFactor; largeFactor--)
                {
                    if (smallFactor * largeFactor == answer)
                    {
                        correctAnswers.Add((term1: smallFactor, term2: largeFactor));
                        break;
                    }
                }
            }

            return correctAnswers;
        }

        protected override List<(int term1, int? term2)> GenerateIncorrectAnswerPool(int answer)
        {
            var correctAnswers = GenerateCorrectAnswerPool(answer);
            var incorrectAnswers = new List<(int term1, int? term2)>();

            foreach (var answerTerms in correctAnswers)
            {
                for (int i = answerTerms.term1 - INCORRECT_ANSWER_NEGATIVE_DELTA; i <= answerTerms.term1 + INCORRECT_ANSWER_POSITIVE_DELTA; i++)
                {
                    for (int j = answerTerms.term2.Value - INCORRECT_ANSWER_NEGATIVE_DELTA; j <= answerTerms.term2 + INCORRECT_ANSWER_POSITIVE_DELTA; j++)
                    {
                        // coherence checks, don't want to give negative terms or give an "incorrect" answer that's actually right
                        if (i > 0 && j > 0 && i * j != answer)
                        {
                            incorrectAnswers.Add((term1: i, term2: j));
                        }
                    }
                }
            }

            return incorrectAnswers;
        }

        protected override string ConvertAnswerPoolValueToAnswer((int term1, int? term2) answer)
        {
            return $"{answer.term1} × {answer.term2}";
        }
    }
}
