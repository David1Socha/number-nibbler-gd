using Godot;
using System.Collections.Generic;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public class AdditionFlyGenerationStrategy : FlyGenerationStrategyBase
    {
        protected override Dictionary<string, (int minAnswer, int maxAnswer)> AnswerRanges
        {
            get
            {
                return new Dictionary<string, (int minAnswer, int maxAnswer)>()
                {
                    { Global.Difficulties.Easy, (minAnswer: 4, maxAnswer: 12) },
                    { Global.Difficulties.Hard, (minAnswer: 10, maxAnswer: 25) }
                };
            }
        }

        public AdditionFlyGenerationStrategy(string difficulty) : base(difficulty)
        {
        }

        protected override List<(int term1, int? term2)> GenerateCorrectAnswerPool(int answer)
        {
            var correctAnswers = new List<(int term1, int? term2)>();

            for (int addend = 1; addend <= (answer / 2); addend++)
            {
                correctAnswers.Add((term1: addend, term2: answer - addend));
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
                        if (i > 0 && j > 0 && i + j != answer)
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
            return $"{answer.term1} + {answer.term2}";
        }
    }
}
