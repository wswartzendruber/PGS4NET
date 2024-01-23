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
using PGS4NET.DisplaySets;
using PGS4NET.Segments;

namespace PGS4NET.Captions;

/// <summary>
///     Contains extensions against different classes for intuitively handling captions.
/// </summary>
public static partial class CaptionExtensions
{
    /// <summary>
    ///     Writes all <see cref="Caption" />s in a collection to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method iterates through each <see cref="Caption" /> and:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Caption" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
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
    /// <exception cref="CaptionException">
    ///     Thrown when a <see cref="Caption" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
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
    public static void WriteAllCaptions(this Stream stream, IEnumerable<Caption> captions)
    {
        foreach (var caption in captions)
            stream.WriteCaption(caption);
    }

    /// <summary>
    ///     Asynchronously writes all <see cref="Caption" />s in a collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method iterates through each <see cref="Caption" /> and:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Caption" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
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
    /// <exception cref="CaptionException">
    ///     Thrown when a <see cref="Caption" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
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
    public static async Task WriteAllCaptionsAsync(this Stream stream
        , IEnumerable<Caption> captions)
    {
        foreach (var caption in captions)
            await stream.WriteCaptionAsync(caption);
    }

    /// <summary>
    ///     Writes a <see cref="Caption" /> to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Caption" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
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
    /// <exception cref="CaptionException">
    ///     Thrown when a <see cref="Caption" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
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
    public static void WriteCaption(this Stream stream, Caption caption)
    {
        foreach (var displaySet in CaptionComposer.Decompose(caption))
            stream.WriteDisplaySet(displaySet);
    }

    /// <summary>
    ///     Asynchronously writes a <see cref="Caption" /> to a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method:
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 Decomposes the <see cref="Caption" /> into a collection of
    ///                 <see cref="DisplaySet" />s.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Decomposes each <see cref="DisplaySet" /> into a collection of
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
    /// <exception cref="CaptionException">
    ///     Thrown when a <see cref="Caption" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
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
    public static async Task WriteCaptionAsync(this Stream stream, Caption caption)
    {
        foreach (var displaySet in CaptionComposer.Decompose(caption))
            await stream.WriteDisplaySetAsync(displaySet);
    }

    /// <summary>
    ///     Decomposes a collection of <see cref="Caption" />s into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </summary>
    /// <exception cref="CaptionException">
    ///     Thrown when a <see cref="Caption" /> cannot be decomposed into a collection of
    ///     <see cref="DisplaySet" />s.
    /// </exception>
    public static IList<DisplaySet> ToSegmentList(this IEnumerable<Caption> captions)
    {
        var returnValue = new List<DisplaySet>();

        foreach (var caption in captions)
            returnValue.AddRange(CaptionComposer.Decompose(caption));

        return returnValue;
    }
}
