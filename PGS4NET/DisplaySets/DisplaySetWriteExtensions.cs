﻿/*
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
    ///     Writes all <see cref="DisplaySet" />s in a collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method iterates through each <see cref="DisplaySet" /> and:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static void WriteAllDisplaySets(this Stream stream
        , IEnumerable<DisplaySet> displaySets)
    {
        foreach (var displaySet in displaySets)
            stream.WriteDisplaySet(displaySet);
    }

    /// <summary>
    ///     Asynchronously writes all <see cref="DisplaySet" />s in a collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method iterates through each <see cref="DisplaySet" /> and:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static async Task WriteAllDisplaySetsAsync(this Stream stream
        , IEnumerable<DisplaySet> displaySets)
    {
        foreach (var displaySet in displaySets)
            await stream.WriteDisplaySetAsync(displaySet);
    }

    /// <summary>
    ///     Writes a <see cref="DisplaySet" /> to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static void WriteDisplaySet(this Stream stream, DisplaySet displaySet)
    {
        foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
            stream.WriteSegment(segment);
    }

    /// <summary>
    ///     Asynchronously writes a <see cref="DisplaySet" /> to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="DisplaySet" /> into a collection of
    ///                 <see cref="Segments" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Writes each <see cref="Segment" /> to the <paramref name="stream" />.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    /// <exception cref="SegmentException">
    ///     Thrown when the properties of a <see cref="Segment" /> cannot be written to the
    ///     <paramref name="stream" />.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a
    ///     <see cref="Segment" /> to the <paramref name="stream" />.
    /// </exception>
    public static async Task WriteDisplaySetAsync(this Stream stream, DisplaySet displaySet)
    {
        foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
            await stream.WriteSegmentAsync(segment);
    }

    /// <summary>
    ///     Decomposes a collection of <see cref="DisplaySet" />s into a collection of
    ///     <see cref="Segment" />s.
    /// </summary>
    /// <exception cref="DisplaySetException">
    ///     Thrown when a <see cref="DisplaySet" /> cannot be decomposed into a collection of
    ///     <see cref="Segment" />s.
    /// </exception>
    public static IList<Segment> ToSegmentList(this IEnumerable<DisplaySet> displaySets)
    {
        var returnValue = new List<Segment>();

        foreach (var displaySet in displaySets)
            returnValue.AddRange(DisplaySetDecomposer.Decompose(displaySet));

        return returnValue;
    }
}
