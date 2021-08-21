using Godot;
using System;
using System.Collections.Generic;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public class DivisionFlyGenerationStrategy : FlyGenerationStrategyBase
    {
        // since we override GetRandomAnswer, this doesn't really do anything...
        protected override Dictionary<string, (int minAnswer, int maxAnswer)> AnswerRanges
        {
            get
            {
                return new Dictionary<string, (int minAnswer, int maxAnswer)>()
                {
                    { Global.Difficulties.Easy, (minAnswer: 2, maxAnswer: 8) },
                    { Global.Difficulties.Hard, (minAnswer: 6, maxAnswer: 15) }
                };
            }
        }

        public DivisionFlyGenerationStrategy(string difficulty) : base(difficulty)
        {
        }

        protected override List<(int term1, int? term2)> GenerateCorrectAnswerPool(int answer)
        {
            var correctAnswers = new List<(int term1, int? term2)>();

            for (int denominator = 2; denominator <= 9; denominator++)
            {
                int numerator = answer * denominator;
                correctAnswers.Add((term1: numerator, term2: denominator));
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
                        if (i > 0 && j > 0 && i / j != answer)
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
            return $"{answer.term1} ÷ {answer.term2}";
        }
    }
}
