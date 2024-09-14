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

/// <summary>
///     Extension methods against <see cref="System.IO.Stream" /> for writing PGS display sets.
/// </summary>
public static partial class DisplaySetExtensions
{
    /// <summary>
    ///     Writes all PGS display sets in a collection to a <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write all display sets to.
    /// </param>
    /// <param name="displaySets">
    ///     The collection of display sets to write.
    /// </param>
    /// <exception cref="DisplaySetException">
    ///     A property of a <see cref="DisplaySet" /> is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static void WriteAllDisplaySets(this Stream stream
        , IEnumerable<DisplaySet> displaySets)
    {
        foreach (var displaySet in displaySets)
            stream.WriteDisplaySet(displaySet);
    }

    /// <summary>
    ///     Asynchronously writes all PGS display sets in a collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write all display sets to.
    /// </param>
    /// <param name="displaySets">
    ///     The collection of display sets to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="DisplaySetException">
    ///     A property of a <see cref="DisplaySet" /> is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IEnumerable<DisplaySet> displaySets, CancellationToken cancellationToken = default)
    {
        foreach (var displaySet in displaySets)
            await stream.WriteDisplaySetAsync(displaySet, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Asynchronously writes all PGS display sets in an asynchronous collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write all display sets to.
    /// </param>
    /// <param name="displaySets">
    ///     The asynchronous collection of display sets to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="DisplaySetException">
    ///     A property of a <see cref="DisplaySet" /> is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IAsyncEnumerable<DisplaySet> displaySets
        , CancellationToken cancellationToken = default)
    {
        await foreach (var displaySet in displaySets)
            await stream.WriteDisplaySetAsync(displaySet, cancellationToken);
    }
#endif

    /// <summary>
    ///     Writes a PGS display set to a <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write the display set to.
    /// </param>
    /// <param name="displaySet">
    ///     The display set to write.
    /// </param>
    /// <exception cref="DisplaySetException">
    ///     A property of the <see cref="DisplaySet" /> is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static void WriteDisplaySet(this Stream stream, DisplaySet displaySet)
    {
        foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
            stream.WriteSegment(segment);
    }

    /// <summary>
    ///     Asynchronously writes a PGS display set to a <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write the display set to.
    /// </param>
    /// <param name="displaySet">
    ///     The display set to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="DisplaySetException">
    ///     A property of the <see cref="DisplaySet" /> is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static async Task WriteDisplaySetAsync(this Stream stream, DisplaySet displaySet
        , CancellationToken cancellationToken = default)
    {
        foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
            await stream.WriteSegmentAsync(segment, cancellationToken);
    }
}
