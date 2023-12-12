using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VeriBot.Services;

namespace VeriBot.Channels.GuildMemberUpdated;

public class GuildMemberUpdatedChannel : BaseChannel<GuildMemberUpdateEventArgs>
{
    private readonly MemberUpdatedHandler _memberUpdatedHandler;
    
    /// <inheritdoc />
    public GuildMemberUpdatedChannel(ILogger<GuildMemberUpdatedChannel> logger, ErrorHandlingService errorHandlingService, MemberUpdatedHandler memberUpdatedHandler) : base(logger, errorHandlingService, "Guild Member Updated")
    {
        _memberUpdatedHandler = memberUpdatedHandler;
    }

    /// <inheritdoc />
    protected override async ValueTask HandleMessage(GuildMemberUpdateEventArgs message)
    {
        try
        {
            await _memberUpdatedHandler.HandleUpdate(message);
        }
        catch (Exception exception)
        {
           await  ErrorHandlingService.Log(exception, "Handling Member Updated");
        }
    }
}