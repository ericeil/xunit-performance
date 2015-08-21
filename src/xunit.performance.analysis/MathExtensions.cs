﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;

namespace Microsoft.Xunit.Performance.Analysis
{
    public static class MathExtensions
    {
        public static double MarginOfError(this RunningStatistics stats, double confidence)
        {
            if (stats.Count < 2)
                return double.NaN;

            var stderr = stats.StandardDeviation / Math.Sqrt(stats.Count);
            var t = TInv(1.0 - confidence, (int)stats.Count - 1);
            var mean = stats.Mean;
            var interval = t * stderr;

            return interval / mean;
        }

        [ThreadStatic]
        private static Dictionary<double, Dictionary<int, double>> t_TInvCache = new Dictionary<double, Dictionary<int, double>>();

        private static double TInv(double probability, int degreesOfFreedom)
        {
            Dictionary<int, double> dofCache;
            if (!t_TInvCache.TryGetValue(probability, out dofCache))
                t_TInvCache[probability] = dofCache = new Dictionary<int, double>();
            double result;
            if (!dofCache.TryGetValue(degreesOfFreedom, out result))
                dofCache[degreesOfFreedom] = result = ExcelFunctions.TInv(probability, degreesOfFreedom);
            return result;
        }
    }
}