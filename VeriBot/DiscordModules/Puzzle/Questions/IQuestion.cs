using VeriBot.Channels.Puzzle;

namespace VeriBot.DiscordModules.Puzzle.Questions;

public interface IQuestion
{
    int GetPuzzleNumber();
    void PostPuzzle(PuzzleCommandAction request);
    void PostClue(PuzzleCommandAction request);
    bool AnswerIsCorrect(string answer);
}