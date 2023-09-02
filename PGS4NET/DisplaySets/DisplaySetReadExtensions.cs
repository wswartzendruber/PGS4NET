/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Contains extensions against different classes for intuitively handling display sets.
/// </summary>
public static partial class DisplaySetExtensions
{
    /// <summary>
    ///     Reads all display sets from a <see cref="Stream" /> until the end of the stream is
    ///     reached.
    /// </summary>
    /// <remarks>
    ///     This method works by reading through all segments in the <see cref="Stream" />,
    ///     composing a new <see cref="DisplaySet" /> as each end segment is encountered. The
    ///     stream is assumed to contain only display sets and any trailing data that cannot
    ///     form a complete display set will cause an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="DisplaySet" />s that was read from the
    ///     <see cref="Stream" />, or an empty collection if the stream was already at its end.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment.
    /// </exception>
    public static IList<DisplaySet> ReadAllDisplaySets(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Asynchronously reads all display sets from a <see cref="Stream" /> until the end of
    ///     the stream is reached.
    /// </summary>
    /// <remarks>
    ///     This method works by reading through all segments in the <see cref="Stream" />,
    ///     composing a new <see cref="DisplaySet" /> as each end segment is encountered. The
    ///     stream is assumed to contain only display sets and any trailing data that cannot
    ///     form a complete display set will cause an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="DisplaySet" />s that was read from the
    ///     <see cref="Stream" />, or an empty collection if the stream was already at its end.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment.
    /// </exception>
    public static async Task<IList<DisplaySet>> ReadAllDisplaySetsAsync(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (await stream.ReadDisplaySetAsync() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Reads the next display set from a <see cref="Stream" />.
    /// </summary>
    /// <remarks>
    ///     This method works by reading through the segments in the <see cref="Stream" /> until
    ///     an end segment is encountered, causing the new display set to be composed.
    /// </remarks>
    /// <returns>
    ///     The <see cref="DisplaySet" /> that was read from the <see cref="Stream" />, or
    ///     <see langword="null" /> if the stream was already at its end.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment.
    /// </exception>
    public static DisplaySet? ReadDisplaySet(this Stream stream)
    {
        var read = false;
        var composer = new DisplaySetComposer();

        while (stream.ReadSegment() is Segment segment)
        {
            read = true;

            if (composer.Input(segment) is DisplaySet displaySet)
                return displaySet;
        }

        if (read)
            throw new DisplaySetException("EOF during display set composition.");

        return null;
    }

    /// <summary>
    ///     Asynchronously reads the next display set from a <see cref="Stream" />.
    /// </summary>
    /// <remarks>
    ///     This method works by reading through the segments in the <see cref="Stream" /> until
    ///     an end segment is encountered, causing the new display set to be composed.
    /// </remarks>
    /// <returns>
    ///     The <see cref="DisplaySet" /> that was read from the <see cref="Stream" />, or
    ///     <see langword="null" /> if the stream was already at its end.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment.
    /// </exception>
    public static async Task<DisplaySet?> ReadDisplaySetAsync(this Stream stream)
    {
        var read = false;
        var composer = new DisplaySetComposer();

        while (await stream.ReadSegmentAsync() is Segment segment)
        {
            read = true;

            if (composer.Input(segment) is DisplaySet displaySet)
                return displaySet;
        }

        if (read)
            throw new DisplaySetException("EOF during display set composition.");

        return null;
    }

    /// <summary>
    ///     Composes a collection of <see cref="Segment" />s into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of <see cref="Segment" />s cannot form a
    ///     <see cref="DisplaySet" />.
    /// </exception>
    public static IList<DisplaySet> ToDisplaySetList(this IEnumerable<Segment> segments)
    {
        var returnValue = new List<DisplaySet>();
        var composer = new DisplaySetComposer();

        foreach (var segment in segments)
        {
            if (composer.Input(segment) is DisplaySet displaySet)
                returnValue.Add(displaySet);
        }

        return returnValue;
    }
}
