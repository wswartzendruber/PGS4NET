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
using PGS4NET.DisplaySets;
using PGS4NET.Segments;

namespace PGS4NET.Captions;

/// <summary>
///     Contains extensions against different classes for intuitively handling captions.
/// </summary>
public static partial class CaptionExtensions
{
    /// <summary>
    ///     Reads all <see cref="Caption" />s from a <paramref name="stream" />.
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
    ///                 Composes enough <see cref="DisplaySet" />s until a
    ///                 <see cref="Caption" /> can be composed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Adds the composed <see cref="Caption" /> to the return collection.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     The entire <paramref name="stream" /> is read in this manner until its end is
    ///     reached. Any trailing data that cannot ultimately form a complete
    ///     <see cref="Caption" /> causes an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="Caption" />s that were read from the
    ///     <paramref name="stream" />, or an empty collection if the stream was already at its
    ///     end.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into an <see cref="Caption" />.
    /// </exception>
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
    public static IList<Caption> ReadAllCaptions(this Stream stream)
    {
        var returnValue = new List<Caption>();

        while (stream.ReadCaption() is Caption caption)
            returnValue.Add(caption);

        return returnValue;
    }

    /// <summary>
    ///     Asynchronously reads all <see cref="Caption" />s from a <paramref name="stream" />.
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
    ///                 Composes enough <see cref="DisplaySet" />s until a
    ///                 <see cref="Caption" /> can be composed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Adds the composed <see cref="Caption" /> to the return collection.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     The entire <paramref name="stream" /> is read in this manner until its end is
    ///     reached. Any trailing data that cannot ultimately form a complete
    ///     <see cref="Caption" /> causes an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="Caption" />s that were read from the
    ///     <paramref name="stream" />, or an empty collection if the stream was already at its
    ///     end.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into an <see cref="Caption" />.
    /// </exception>
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
    public static async Task<IList<Caption>> ReadAllCaptionsAsync(this Stream stream)
    {
        var returnValue = new List<Caption>();

        while (await stream.ReadCaptionAsync() is Caption caption)
            returnValue.Add(caption);

        return returnValue;
    }

    /// <summary>
    ///     Reads a <see cref="Caption" /> from a <paramref name="stream" />.
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
    ///                 Composes enough <see cref="DisplaySet" />s until a
    ///                 <see cref="Caption" /> can be composed.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     Only the data necessary to compose a <see cref="Caption" /> is read from the
    ///     <paramref name="stream" />.
    /// </remarks>
    /// <returns>
    ///     The <see cref="Caption" /> that was read from the <paramref name="stream" />.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into an <see cref="Caption" />.
    /// </exception>
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
    public static Caption? ReadCaption(this Stream stream)
    {
        var read = false;
        var composer = new CaptionComposer();

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
        {
            read = true;

            if (composer.Input(displaySet) is Caption caption)
                return caption;
        }

        if (read)
            throw new CaptionException("EOF during caption composition.");

        return null;
    }

    /// <summary>
    ///     Asynchronously reads a <see cref="Caption" /> from a <paramref name="stream" />.
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
    ///                 Composes enough <see cref="DisplaySet" />s until a
    ///                 <see cref="Caption" /> can be composed.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     Only the data necessary to compose a <see cref="Caption" /> is read from the
    ///     <paramref name="stream" />.
    /// </remarks>
    /// <returns>
    ///     The <see cref="Caption" /> that was read from the <paramref name="stream" />.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into an <see cref="Caption" />.
    /// </exception>
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
    public static async Task<Caption?> ReadCaptionAsync(this Stream stream)
    {
        var read = false;
        var composer = new CaptionComposer();

        while (await stream.ReadDisplaySetAsync() is DisplaySet displaySet)
        {
            read = true;

            if (composer.Input(displaySet) is Caption caption)
                return caption;
        }

        if (read)
            throw new CaptionException("EOF during caption composition.");

        return null;
    }

    /// <summary>
    ///     Composes a collection of <see cref="DisplaySet" />s into a collection of
    ///     <see cref="Caption" />s.
    /// </summary>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into a <see cref="Caption" />.
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
