using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VeriBot.Database;
using VeriBot.Database.Models;

namespace VeriBot.DataProviders.SubProviders;

public class ExceptionProvider
{
    private readonly IDbContextFactory<VeriBotContext> _dbContextFactory;

    public ExceptionProvider(IDbContextFactory<VeriBotContext> contextFactory)
    {
        _dbContextFactory = contextFactory;
    }

    public async Task InsertException(ExceptionLog ex)
    {
        await using (var db = _dbContextFactory.CreateDbContext())
        {
            db.LoggedErrors.Add(ex);
            await db.SaveChangesAsync();
        }
    }
}