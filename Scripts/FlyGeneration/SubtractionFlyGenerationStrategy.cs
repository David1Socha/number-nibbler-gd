using Godot;
using System.Collections.Generic;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public class SubtractionFlyGenerationStrategy : FlyGenerationStrategyBase
    {
        protected override Dictionary<string, (int minAnswer, int maxAnswer)> AnswerRanges
        {
            get
            {
                return new Dictionary<string, (int minAnswer, int maxAnswer)>()
                {
                    { Global.Difficulties.Easy, (minAnswer: 2, maxAnswer: 8) },
                    { Global.Difficulties.Hard, (minAnswer: 5, maxAnswer: 15) }
                };
            }
        }

        public SubtractionFlyGenerationStrategy(string difficulty) : base(difficulty)
        {
        }

        protected override List<(int term1, int? term2)> GenerateCorrectAnswerPool(int answer)
        {
            var correctAnswers = new List<(int term1, int? term2)>();

            for (int minuend = answer + 1; minuend < answer + (_difficulty == Global.Difficulties.Easy ? 6 : 10); minuend++)
            {
                int subtrahend = minuend - answer;
                correctAnswers.Add((term1: minuend, term2: subtrahend));
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
                        if (i > 0 && j > 0 && i - j != answer)
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
            return $"{answer.term1} - {answer.term2}";
        }
    }
}
