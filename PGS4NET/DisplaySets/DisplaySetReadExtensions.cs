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
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Contains extensions against different classes for intuitively handling display sets.
/// </summary>
public static partial class DisplaySetExtensions
{
    /// <summary>
    ///     Reads all <see cref="DisplaySet" />s from a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Reads <see cref="Segment" />s from the <paramref name="stream" /> until
    ///                 a <see cref="DisplaySet" /> can be composed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Adds the composed <see cref="DisplaySet" /> to the return collection.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     The entire <paramref name="stream" /> is read in this manner until its end is
    ///     reached. Any trailing data that cannot ultimately form a complete
    ///     <see cref="DisplaySet" /> causes an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="DisplaySet" />s that were read from the
    ///     <paramref name="stream" />, or an empty collection if the stream was already at its
    ///     end.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment's buffer are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment from
    ///     the <paramref name="stream" />.
    /// </exception>
    public static IList<DisplaySet> ReadAllDisplaySets(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Asynchronously reads all <see cref="DisplaySet" />s from a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Reads <see cref="Segment" />s from the <paramref name="stream" /> until
    ///                 a <see cref="DisplaySet" /> can be composed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Adds the composed <see cref="DisplaySet" /> to the return collection.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     The entire <paramref name="stream" /> is read in this manner until its end is
    ///     reached. Any trailing data that cannot ultimately form a complete
    ///     <see cref="DisplaySet" /> causes an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="DisplaySet" />s that were read from the
    ///     <paramref name="stream" />, or an empty collection if the stream was already at its
    ///     end.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment's buffer are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment from
    ///     the <paramref name="stream" />.
    /// </exception>
    public static async Task<IList<DisplaySet>> ReadAllDisplaySetsAsync(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (await stream.ReadDisplaySetAsync() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    /// <summary>
    ///     Reads a <see cref="DisplaySet" /> from a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method reads <see cref="Segment" />s from the
    ///     <paramref name="stream" /> until a <see cref="DisplaySet" /> can be composed. Only
    ///     the data necessary to compose a <see cref="DisplaySet" /> is read from the
    ///     <paramref name="stream" />.
    /// </remarks>
    /// <returns>
    ///     The <see cref="DisplaySet" /> that was read from the <paramref name="stream" />.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment's buffer are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment from
    ///     the <paramref name="stream" />.
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
    ///     Asynchronously reads a <see cref="DisplaySet" /> from a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method reads <see cref="Segment" />s from the
    ///     <paramref name="stream" /> until a <see cref="DisplaySet" /> can be composed. Only
    ///     the data necessary to compose a <see cref="DisplaySet" /> is read from the
    ///     <paramref name="stream" />.
    /// </remarks>
    /// <returns>
    ///     The <see cref="DisplaySet" /> that was read from the <paramref name="stream" />.
    /// </returns>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment's buffer are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment from
    ///     the <paramref name="stream" />.
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
    ///     Thrown when a combination of individually valid <see cref="Segment" />s cannot be
    ///     composed into a <see cref="DisplaySet" />.
    /// </exception>
    public static IList<DisplaySet> ToDisplaySetList(this IEnumerable<Segment> segments)
    {
        var pending = false;
        var returnValue = new List<DisplaySet>();
        var composer = new DisplaySetComposer();

        foreach (var segment in segments)
        {
            pending = true;

            if (composer.Input(segment) is DisplaySet displaySet)
            {
                pending = false;
                returnValue.Add(displaySet);
            }
        }

        if (pending)
            throw new DisplaySetException("Incomplete display set from trailing segments.");

        return returnValue;
    }
}
