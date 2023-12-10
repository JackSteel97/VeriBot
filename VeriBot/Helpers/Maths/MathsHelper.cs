﻿using System;
using System.Security.Cryptography;

namespace VeriBot.Helpers.Maths;

public static class MathsHelper
{
    public static decimal PercentageChange(decimal oldValue, decimal newValue) => oldValue != 0 ? (newValue - oldValue) / Math.Abs(oldValue) : newValue / 100;

    public static double GetPercentageOfDuration(TimeSpan smallerDuration, TimeSpan largerDuration) => smallerDuration.TotalSeconds / largerDuration.TotalSeconds;

    public static bool TrueWithProbability(double probability)
    {
        const int maxBound = 10000000;
        return RandomNumberGenerator.GetInt32(maxBound) <= maxBound * probability;
    }

    public static int Modulo(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}