using Godot;
using System;
using System.Collections.Generic;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public class MultiplesFlyGenerationStrategy : FlyGenerationStrategyBase
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

        public MultiplesFlyGenerationStrategy(string difficulty) : base(difficulty)
        {
        }

        protected override List<(int term1, int? term2)> GenerateCorrectAnswerPool(int answer)
        {
            var correctAnswers = new List<(int term1, int? term2)>();

            for (int i = 2; i <= 8; i++)
            {
                int term1 = answer * i;
                correctAnswers.Add((term1: term1, term2: null));
            }

            return correctAnswers;
        }

        protected override List<(int term1, int? term2)> GenerateIncorrectAnswerPool(int answer)
        {
            var correctAnswers = GenerateCorrectAnswerPool(answer);
            var incorrectAnswers = new List<(int term1, int? term2)>();

            foreach (var a in correctAnswers)
            {
                for (int i = a.term1 - INCORRECT_ANSWER_NEGATIVE_DELTA; i <= a.term1 + INCORRECT_ANSWER_POSITIVE_DELTA; i++)
                {
                    // coherence checks, don't want to give negative terms or give an "incorrect" answer that's actually right
                    if (i > 0 && (i % answer != 0))
                    {
                        incorrectAnswers.Add((term1: i, term2: null));
                    }
                }
            }

            return incorrectAnswers;
        }

        public override string GetPrompt()
        {
            return $"Multiples of {_answer}";
        }

        protected override string ConvertAnswerPoolValueToAnswer((int term1, int? term2) answer)
        {
            return $"{answer.term1}";
        }
    }
}
