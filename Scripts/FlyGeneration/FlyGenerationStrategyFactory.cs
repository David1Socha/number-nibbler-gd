using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public static class FlyGenerationStrategyFactory
    {
        public static IFlyGenerationStrategy GetFlyGenerationStrategy(string strategyName, string difficultyName)
        {
            switch (strategyName)
            {
                case Global.Categories.Addition:
                    return new AdditionFlyGenerationStrategy(difficultyName);
                case Global.Categories.Subtraction:
                    return new SubtractionFlyGenerationStrategy(difficultyName);
                case Global.Categories.Multiplication:
                    return new MultiplicationFlyGenerationStrategy(difficultyName);
                case Global.Categories.Division:
                    return new DivisionFlyGenerationStrategy(difficultyName);
                case Global.Categories.Multiples:
                    return new MultiplesFlyGenerationStrategy(difficultyName);
                case Global.Categories.Basic:
                default:
                    return new BasicFlyGenerationStrategy();
            }
        }
    }
}
