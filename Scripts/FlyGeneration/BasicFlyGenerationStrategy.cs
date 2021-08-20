using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberNibbler.Scripts.FlyGeneration
{
    public class BasicFlyGenerationStrategy : IFlyGenerationStrategy
    {
        public string GenerateCorrectAnswer()
        {
            return "Good";
        }

        public string GenerateIncorrectAnswer()
        {
            return "Bad";
        }

        public string GetPrompt()
        {
            return "Pick good flies";
        }
    }
}
