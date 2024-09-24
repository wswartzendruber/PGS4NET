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
using PGS4NET.DisplaySets;

namespace PGS4NET.Segments;

/// <summary>
///     Extension methods against <see cref="System.Collections.Generic.IEnumerable{T}" /> for
///     deconstructing display sets;
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    ///     Deconstructs all display sets in a collection into segments, one display set at a
    ///     time, as each sequence of segments is consumed by an enumerator.
    /// </summary>
    /// <param name="displaySets">
    ///     The collection of display sets to deconstruct.
    /// </param>
    /// <returns>
    ///     An enumerator over the sequence deconstructed segments.
    /// </returns>
    public static IEnumerable<Segment> Segments(this IEnumerable<DisplaySet> displaySets)
    {
        foreach (var displaySet in displaySets)
        {
            foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
                yield return segment;
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Deconstructs all display sets in an asynchronous collection into segments, one
    ///     display set at a time, as each sequence of segments is consumed by an asynchronous
    ///     enumerator.
    /// </summary>
    /// <param name="displaySets">
    ///     The asynchronous collection of display sets to deconstruct.
    /// </param>
    /// <returns>
    ///     An asynchronous enumerator over the sequence deconstructed segments.
    /// </returns>
    public static async IAsyncEnumerable<Segment> SegmentsAsync(
        this IAsyncEnumerable<DisplaySet> displaySets)
    {
        await foreach (var displaySet in displaySets)
        {
            foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
                yield return segment;
        }
    }
#endif
}
