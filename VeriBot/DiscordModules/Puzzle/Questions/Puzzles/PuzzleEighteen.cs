using DSharpPlus.Entities;
using System;
using VeriBot.Channels.Puzzle;
using VeriBot.DiscordModules.Puzzle.Helpers;
using VeriBot.DiscordModules.Puzzle.Questions;
using VeriBot.Helpers;
using VeriBot.Services.Configuration;

namespace VeriBot.DiscordModules.Puzzle.Questions.Puzzles;

public class PuzzleEighteen : IQuestion
{
    private const int _number = 18;
    private const string _puzzleFile = "Goodbye.png";
    private const string _clueText = "Where did we first meet?";
    private readonly string _answerText;

    public PuzzleEighteen(AppConfigurationService config)
    {
        _answerText = config.Puzzle.Answers[_number - 1];
    }

    /// <inheritdoc />
    public int GetPuzzleNumber() => _number;

    /// <inheritdoc />
    public void PostPuzzle(PuzzleCommandAction request)
    {
        var message = new DiscordMessageBuilder().WithContent(_number.ToString());
        QuestionConstructionHelpers.AddFile(message, _puzzleFile);
        request.Responder.Respond(message);
    }

    /// <inheritdoc />
    public void PostClue(PuzzleCommandAction request)
    {
        var message = new DiscordMessageBuilder()
            .WithEmbed(EmbedGenerator.Info(_clueText, "Clue"));
        request.Responder.Respond(message);
    }

    /// <inheritdoc />
    public bool AnswerIsCorrect(string answer) => string.Equals(answer, _answerText, StringComparison.OrdinalIgnoreCase);
}