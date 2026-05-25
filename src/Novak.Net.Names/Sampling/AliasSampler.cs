using System;
using Novak.Net.Common.Randomization;

namespace Novak.Net.Names.Sampling;

/// <summary>
/// Vose Alias method for O(1) discrete sampling.
/// </summary>
internal sealed class AliasSampler
{
    private readonly int[] _alias;
    private readonly double[] _prob;

    public int Length => _prob.Length;

    public AliasSampler(ReadOnlySpan<double> weights)
    {
        int n = weights.Length;
        if (n == 0) throw new ArgumentException("Empty weights.");
        _alias = new int[n];
        _prob = new double[n];

        // normalize and classify
        double sum = 0;
        for (int i = 0; i < n; i++)
            sum += weights[i] < 0 ? 0 : weights[i];
        if (sum <= 0) throw new ArgumentException("All weights are non-positive.");

        var scaled = new double[n];
        var small = new System.Collections.Generic.Stack<int>(n);
        var large = new System.Collections.Generic.Stack<int>(n);
        for (int i = 0; i < n; i++)
        {
            double p = (weights[i] < 0 ? 0 : weights[i]) * n / sum;
            scaled[i] = p;
            if (p < 1.0) small.Push(i); else large.Push(i);
        }

        while (small.Count > 0 && large.Count > 0)
        {
            int s = small.Pop();
            int l = large.Pop();
            _prob[s] = scaled[s];
            _alias[s] = l;
            scaled[l] = (scaled[l] + scaled[s]) - 1.0;
            if (scaled[l] < 1.0) small.Push(l); else large.Push(l);
        }

        while (large.Count > 0) _prob[large.Pop()] = 1.0;
        while (small.Count > 0) _prob[small.Pop()] = 1.0;
    }

    public int Next(IRandom rng)
    {
        int n = _prob.Length;
        int i = rng.Next(n);
        return rng.NextDouble() < _prob[i] ? i : _alias[i];
    }
}