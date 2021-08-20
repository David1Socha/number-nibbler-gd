
namespace NumberNibbler.Scripts.FlyGeneration
{
    public interface IFlyGenerationStrategy
    {
        string GenerateCorrectAnswer();
        string GenerateIncorrectAnswer();
        string GetPrompt();
    }
}
