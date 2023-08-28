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
public static class DisplaySetExtensions
{
    /// <summary>
    ///     Reads all display sets from a <see cref="Stream" />. The stream is assumed to
    ///     contain only display sets and trailing data that is not a display set will cause an
    ///     exception to be thrown.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of otherwise valid PGS segments cannot be combined into a
    ///     display set.
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
    ///     Asynchronously reads all display sets from a <see cref="Stream" />. The stream is
    ///     assumed to contain only display sets and trailing data that is not a display set
    ///     will cause an exception to be thrown.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of otherwise valid PGS segments cannot be combined into a
    ///     display set.
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
    ///     Reads the next display set from a <see cref="Stream" /> by reading a series of
    ///     segments and composing them.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of segments cannot form a display set.
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
    ///     Asynchronously reads the next display set from a <see cref="Stream" /> by reading
    ///     a series of segments and composing them.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of segments cannot form a display set.
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
            throw new DisplaySetException("EOF with incomplete display set.");

        return null;
    }

    /// <summary>
    ///     Composes a collection of segments into a collection of display sets.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a sequence of segments cannot form a display set.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
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

    /// <summary>
    ///     Decomposes a collection of display sets into a collection of segments.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a display set cannot be transformed into a collection of segments.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment cannot be formed into a bitstream.
    /// </exception>
    public static IList<Segment> ToSegmentList(this IEnumerable<DisplaySet> displaySets)
    {
        var returnValue = new List<Segment>();

        foreach (var displaySet in displaySets)
            returnValue.AddRange(DisplaySetComposer.Decompose(displaySet));

        return returnValue;
    }

    /// <summary>
    ///     Decomposes a display set into a collection of segments and then writes each one to
    ///     a <see cref="Stream" />.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a display set cannot be transformed into a collection of segments.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment cannot be formed into a bitstream.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a segment.
    /// </exception>
    public static void WriteDisplaySet(this Stream stream, DisplaySet displaySet)
    {
        foreach (var segment in DisplaySetComposer.Decompose(displaySet))
            stream.WriteSegment(segment);
    }
}
