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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Extension methods against <see cref="System.IO.Stream" /> for reading PGS display sets.
/// </summary>
public static partial class DisplaySetExtensions
{
    /// <summary>
    ///     Attempts to read all PGS display sets from a <paramref name="stream" />, one at a
    ///     time, as each one is consumed by an enumerator.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all display sets from.
    /// </param>
    /// <returns>
    ///     An enumerator over display sets being read.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static IEnumerable<DisplaySet> DisplaySets(this Stream stream)
    {
        while (stream.ReadDisplaySet() is DisplaySet displaySet)
            yield return displaySet;
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Attempts to asynchronously read all PGS display sets from a
    ///     <paramref name="stream" />, one at a time, as each one is consumed by an
    ///     asynchronously enumerator.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all display sets from.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     An asynchronous enumerator over display sets being read.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static async IAsyncEnumerable<DisplaySet> DisplaySetsAsync(this Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (await stream.ReadDisplaySetAsync(cancellationToken) is DisplaySet displaySet)
            yield return displaySet;
    }
#endif

    /// <summary>
    ///     Attempts to read all PGS display sets from a <paramref name="stream" /> in a single
    ///     operation.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all display sets from.
    /// </param>
    /// <returns>
    ///     The collection of PGS display sets that were read.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static IList<DisplaySet> ReadAllDisplaySets(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Attempts to asynchronously read all PGS display sets from a
    ///     <paramref name="stream" /> in a single operation.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     display set. The stream must contain only complete PGS display sets from this point
    ///     on and there must be no trailing data.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read all display sets from.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The collection of PGS display sets that were read.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within a display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static async Task<IList<DisplaySet>> ReadAllDisplaySetsAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        var returnValue = new List<DisplaySet>();

        while (await stream.ReadDisplaySetAsync(cancellationToken) is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Attempts to read the next PGS display set from a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     a display set.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read the next display set from.
    /// </param>
    /// <returns>
    ///     The PGS display set that was read or <see langword="null" /> if the
    ///     <paramref name="stream" /> is already EOF.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within the display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static DisplaySet? ReadDisplaySet(this Stream stream)
    {
        var read = false;
        var composer = new DisplaySetComposer();
        DisplaySet? displaySet = null;

        composer.NewDisplaySet += (_, displaySet_) =>
        {
            displaySet = displaySet_;
        };

        while (stream.ReadSegment() is Segment segment)
        {
            read = true;
            composer.Input(segment);

            if (displaySet is DisplaySet displaySet_)
                return displaySet_;
        }

        if (read)
            throw new DisplaySetException("EOF during display set composition.");

        return null;
    }

    /// <summary>
    ///     Attempts to asynchronously read the next PGS display set from a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     The current position of the <paramref name="stream" /> must be at the beginning of a
    ///     a display set.
    /// </remarks>
    /// <param name="stream">
    ///     The stream to read the next display set from.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The PGS display set that was read or <see langword="null" /> if the
    ///     <paramref name="stream" /> is already EOF.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     An encoded value within the display set is invalid.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     An encoded value within a segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read a segment.
    /// </exception>
    public static async Task<DisplaySet?> ReadDisplaySetAsync(this Stream stream
        , CancellationToken cancellationToken = default)
    {
        var read = false;
        var composer = new DisplaySetComposer();
        DisplaySet? displaySet = null;

        composer.NewDisplaySet += (_, displaySet_) =>
        {
            displaySet = displaySet_;
        };

        while (await stream.ReadSegmentAsync() is Segment segment)
        {
            read = true;
            composer.Input(segment);

            if (displaySet is DisplaySet displaySet_)
                return displaySet_;
        }

        if (read)
            throw new DisplaySetException("EOF during display set composition.");

        return null;
    }
}
