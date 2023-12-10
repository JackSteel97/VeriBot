using System;

namespace VeriBot.RateLimiting;

public record struct RateLimitStartEntry(int Uses, DateTimeOffset FirstUse);