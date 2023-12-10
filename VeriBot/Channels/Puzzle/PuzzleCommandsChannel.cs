using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.DiscordModules.Puzzle.Services;
using VeriBot.Services;

namespace VeriBot.Channels.Puzzle;

public class PuzzleCommandsChannel : BaseChannel<PuzzleCommandAction>
{
    private readonly PuzzleService _puzzleService;

    /// <inheritdoc />
    public PuzzleCommandsChannel(PuzzleService puzzleService, ILogger<PuzzleCommandsChannel> logger, ErrorHandlingService errorHandlingService, string channelLabel = "Puzzle") : base(logger,
        errorHandlingService, channelLabel)
    {
        _puzzleService = puzzleService;
    }

    /// <inheritdoc />
    protected override async ValueTask HandleMessage(PuzzleCommandAction message)
    {
        switch (message.Action)
        {
            case PuzzleCommandActionType.Puzzle:
                await _puzzleService.Question(message);
                break;
            case PuzzleCommandActionType.Clue:
                await _puzzleService.Clue(message);
                break;
            case PuzzleCommandActionType.Answer:
                await _puzzleService.Answer(message);
                break;
        }
    }
}