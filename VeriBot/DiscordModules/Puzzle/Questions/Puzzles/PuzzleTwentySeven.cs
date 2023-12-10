using DSharpPlus.Entities;
using System;
using VeriBot.Channels.Puzzle;
using VeriBot.DiscordModules.Puzzle.Helpers;
using VeriBot.Helpers;
using VeriBot.Services.Configuration;

namespace VeriBot.DiscordModules.Puzzle.Questions.Puzzles;

public class PuzzleTwentySeven : IQuestion
{
    private const int _number = 27;
    private const string _puzzleFile = "ISO8601.mp3";
    private const string _clueText = "There is no extra clue available for this one.";

    public PuzzleTwentySeven(AppConfigurationService config)
    {
        // Do nothing, do not remove this constructor for factory compatibility.
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
            .WithEmbed(EmbedGenerator.Info(_clueText, "Good Luck"));
        request.Responder.Respond(message);
    }

    /// <inheritdoc />
    public bool AnswerIsCorrect(string answer) => string.Equals(answer, DateTime.UtcNow.ToString("HH:mm"), StringComparison.OrdinalIgnoreCase);
}