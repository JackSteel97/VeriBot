using Zalgo;

namespace VeriBot.Helpers.Extensions;

public static class StringExtensions
{
    public static string ToZalgo(this string input, bool zalgoify = true) => zalgoify ? new ZalgoString(input).ToString() : input;
}