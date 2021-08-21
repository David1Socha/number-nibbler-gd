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
                case "Addition":
                    return new AdditionFlyGenerationStrategy(difficultyName);
                case "Subtraction":
                    return new SubtractionFlyGenerationStrategy(difficultyName);
                case "Multiplication":
                    return new MultiplicationFlyGenerationStrategy(difficultyName);
                case "Basic":
                default:
                    return new BasicFlyGenerationStrategy();
            }
        }
    }
}
