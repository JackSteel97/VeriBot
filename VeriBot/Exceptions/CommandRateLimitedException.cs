﻿using Humanizer;
using System;

namespace VeriBot.Exceptions;

public class CommandRateLimitedException : Exception
{
    /// <inheritdoc />
    public override string Message { get; }

    public CommandRateLimitedException()
    {
    }

    public CommandRateLimitedException(string command, int maxUses, TimeSpan inPeriod, TimeSpan remainingCooldown)
    {
        Message = $"{command} can only be used {maxUses} times in {inPeriod.Humanize()}. Please try again in {remainingCooldown.Humanize()}";
    }
}