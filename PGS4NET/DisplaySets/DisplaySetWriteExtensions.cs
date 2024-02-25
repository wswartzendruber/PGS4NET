/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

public static partial class DisplaySetExtensions
{
    public static void WriteAllDisplaySets(this Stream stream
        , IEnumerable<DisplaySet> displaySets)
    {
        foreach (var displaySet in displaySets)
            stream.WriteDisplaySet(displaySet);
    }

    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IEnumerable<DisplaySet> displaySets, CancellationToken cancellationToken = default)
    {
        foreach (var displaySet in displaySets)
            await stream.WriteDisplaySetAsync(displaySet, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER
    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IAsyncEnumerable<DisplaySet> displaySets
        , CancellationToken cancellationToken = default)
    {
        await foreach (var displaySet in displaySets)
            await stream.WriteDisplaySetAsync(displaySet, cancellationToken);
    }
#endif

    public static void WriteDisplaySet(this Stream stream, DisplaySet displaySet)
    {
        foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
            stream.WriteSegment(segment);
    }

    public static async Task WriteDisplaySetAsync(this Stream stream, DisplaySet displaySet
        , CancellationToken cancellationToken = default)
    {
        foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
            await stream.WriteSegmentAsync(segment, cancellationToken);
    }
}
