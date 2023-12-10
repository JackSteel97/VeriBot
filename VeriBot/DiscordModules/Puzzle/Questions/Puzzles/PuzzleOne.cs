using DSharpPlus.Entities;
using System;
using VeriBot.Channels.Puzzle;
using VeriBot.Helpers;
using VeriBot.Services.Configuration;

namespace VeriBot.DiscordModules.Puzzle.Questions.Puzzles;

public class PuzzleOne : IQuestion
{
    private const int _number = 1;
    private const string _puzzleText = "UifBotxfs";
    private const string _clueText = "Shakespeare’s Caesar knows the answer every day of the year, but only if you whisper nicely in the correct ear.";
    private readonly string _answerText;

    public PuzzleOne(AppConfigurationService config)
    {
        _answerText = config.Puzzle.Answers[_number - 1];
    }

    /// <inheritdoc />
    public int GetPuzzleNumber() => _number;

    /// <inheritdoc />
    public void PostPuzzle(PuzzleCommandAction request)
    {
        var message = new DiscordMessageBuilder()
            .WithEmbed(EmbedGenerator.Primary(_puzzleText, _number.ToString()));
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