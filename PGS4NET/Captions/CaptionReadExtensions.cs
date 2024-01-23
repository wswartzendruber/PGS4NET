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
    ///     Reads all <see cref="Caption" />s from a <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method reads <see cref="Segment" />s from the
    ///     <paramref name="stream" />, composing them into <see cref="DisplaySet" />s, and then
    ///     composing those into <see cref="Caption" />s. The entire <paramref name="stream" />
    ///     is read in this manner until its end is reached. Any trailing data that cannot
    ///     ultimately form a complete <see cref="Caption" /> causes an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="Caption" />s that were read from the
    ///     <paramref name="stream" />, or an empty collection if the stream was already at its
    ///     end.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into a <see cref="Caption" />.
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

        // TODO

        return returnValue;
    }

    /// <summary>
    ///     Asynchronously reads all <see cref="Caption" />s from a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <remarks>
    ///     Internally, this method reads <see cref="Segment" />s from the
    ///     <paramref name="stream" />, composing them into <see cref="DisplaySet" />s, and then
    ///     composing those into <see cref="Caption" />s. The entire <paramref name="stream" />
    ///     is read in this manner until its end is reached. Any trailing data that cannot
    ///     ultimately form a complete <see cref="Caption" /> causes an exception to be thrown.
    /// </remarks>
    /// <returns>
    ///     A collection <see cref="Caption" />s that were read from the
    ///     <paramref name="stream" />, or an empty collection if the stream was already at its
    ///     end.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into a <see cref="Caption" />.
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

        // TODO

        return returnValue;
    }

    /// <summary>
    ///     Composes a collection of <see cref="DisplaySet" />s into a collection of
    ///     <see cref="Caption" />s.
    /// </summary>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of individually valid <see cref="DisplaySet" />s cannot be
    ///     composed into a <see cref="Caption" />.
    /// </exception>
    public static IList<Caption> ToCaptionList(this IEnumerable<DisplaySet> displaySets)
    {
        var pending = false;
        var returnValue = new List<Caption>();
        var composer = new CaptionComposer();

        foreach (var displaySet in displaySets)
        {
            pending = true;

            if (composer.Input(displaySet) is Caption caption)
            {
                pending = false;
                returnValue.Add(caption);
            }
        }

        if (pending)
            throw new CaptionException("Incomplete caption from trailing display sets.");

        return returnValue;
    }
}
