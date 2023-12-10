using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VeriBot.Database;
using VeriBot.Database.Models.Puzzle;

namespace VeriBot.DataProviders.SubProviders;

public class PuzzleProvider
{
    private readonly IDbContextFactory<VeriBotContext> _dbContextFactory;
    private readonly ILogger<PuzzleProvider> _logger;

    public PuzzleProvider(ILogger<PuzzleProvider> logger, IDbContextFactory<VeriBotContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<int> GetUserPuzzleLevel(ulong userId)
    {
        await using (var db = await _dbContextFactory.CreateDbContextAsync())
        {
            var progress = await db.PuzzleProgress.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
            if (progress != default) return progress.CurrentLevel;
        }

        return 1;
    }

    public async Task SetUserPuzzleLevel(ulong userId, int newLevel)
    {
        await using (var db = await _dbContextFactory.CreateDbContextAsync())
        {
            var progress = await db.PuzzleProgress.FirstOrDefaultAsync(x => x.UserId == userId);
            if (progress != default)
            {
                progress.CurrentLevel = newLevel;
                db.PuzzleProgress.Update(progress);
            }
            else
            {
                db.PuzzleProgress.Add(new Progress { UserId = userId, CurrentLevel = newLevel });
            }

            await db.SaveChangesAsync();
        }
    }

    public async Task RecordGuess(ulong userId, int puzzleLevel, string guess)
    {
        await using (var db = await _dbContextFactory.CreateDbContextAsync())
        {
            var guessRecord = new Guess { UserId = userId, PuzzleLevel = puzzleLevel, GuessContent = guess };
            db.Guesses.Add(guessRecord);
            await db.SaveChangesAsync();
        }
    }
}