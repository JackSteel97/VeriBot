﻿using VeriBot.Helpers.Algorithms;
using Xunit;

namespace VeriBot.Test.Helpers.Algorithms;

public class DadJokeExtractorTests
{
    [Theory]
    [InlineData("I'm back", "back")]
    [InlineData("I'm not going out", "not going out")]
    [InlineData("I'm having some fish, for dinner", "having some fish")]
    [InlineData("I’m having some fish, for dinner", "having some fish")]
    [InlineData("I‘m having some fish, for dinner", "having some fish")]
    [InlineData("Some more text that is not relevant. I'm having some fish, for dinner", "having some fish")]
    [InlineData("Some more text that is not relevant. I’m having some fish, for dinner", "having some fish")]
    [InlineData("Some more text that is not relevant. I‘m having some fish, for dinner", "having some fish")]
    [InlineData("Some more text that is not relevat. i'm HAVING SOME FISH, for dinner", "HAVING SOME FISH")]
    [InlineData("Some more text that is not relevat. i’m HAVING SOME FISH, for dinner", "HAVING SOME FISH")]
    [InlineData("Some more text that is not relevat. i‘m HAVING SOME FISH, for dinner", "HAVING SOME FISH")]
    [InlineData("this is it i'm not having any of this", "not having any of this")]
    [InlineData("this is it i’m not having any of this", "not having any of this")]
    [InlineData("this is it i'm", "")]
    [InlineData("this is it i'm,", "")]
    [InlineData("this is it i'm.", "")]
    [InlineData("this is it i‘m.", "")]
    [InlineData("this is it i’m.", "")]
    [InlineData("this is it i'm   ", "")]
    [InlineData("this is it i'm not here   ", "not here")]
    [InlineData("this is it i’m not here   ", "not here")]
    [InlineData("this is it i‘m not here   ", "not here")]
    [InlineData("this is it", "")]
    [InlineData("That's it I'm not doing anything it's silly", "not doing anything it's silly")]
    [InlineData("That's it I'm \"not doing anything it's silly\"", "\"not doing anything it's silly\"")]
    [InlineData("That's it I'm giving you $500, go away", "giving you $500")]
    [InlineData("That's it I'm giving you £500, go away", "giving you £500")]
    [InlineData("That's it I'm giving it 100%, go away", "giving it 100%")]
    [InlineData("That's it I'm <@1234556> a test, go away", "<@1234556> a test")]
    public void Extract(string input, string expectedOutput)
    {
        string actualOutput = DadJokeExtractor.Extract(input);
        Assert.Equal(expectedOutput, actualOutput);
    }
}