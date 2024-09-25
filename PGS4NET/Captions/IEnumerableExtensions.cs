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

namespace PGS4NET.Captions;

/// <summary>
///     Extension methods against <see cref="System.Collections.Generic.IEnumerable{T}" /> for
///     constructing captions;
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    ///     Constructs all display sets in a collection into captions, multiple display sets at
    ///     a time, as the captions are consumed by an enumerator.
    /// </summary>
    /// <param name="displaySets">
    ///     The collection of display sets to construct.
    /// </param>
    /// <returns>
    ///     An enumerator over the constructed captions.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A display set is not valid to compose a caption it should be in.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 The PTS value of a display set is less than or equal to the PTS value of
    ///                 the previous display set.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    public static IEnumerable<Caption> Captions(this IEnumerable<DisplaySet> displaySets)
    {
        var composer = new CaptionComposer();
        Caption? caption = null;

        composer.NewCaption += (_, caption_) =>
        {
            caption = caption_;
        };

        foreach (var displaySet in displaySets)
        {
            composer.Input(displaySet);

            if (caption is Caption caption_)
            {
                yield return caption_;

                caption = null;
            }
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Constructs all display sets in an asynchronous collection into captions, multiple
    ///     display sets at a time, as the captions are consumed by an asynchronous enumerator.
    /// </summary>
    /// <param name="displaySets">
    ///     The asynchronous collection of display sets to construct.
    /// </param>
    /// <returns>
    ///     An asynchronous enumerator over the constructed captions.
    /// </returns>
    /// <exception cref="CaptionException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A display set is not valid to compose a caption it should be in.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 The PTS value of a display set is less than or equal to the PTS value of
    ///                 the previous display set.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    public static async IAsyncEnumerable<Caption> CaptionsAsync(
        this IAsyncEnumerable<DisplaySet> displaySets)
    {
        var composer = new CaptionComposer();
        Caption? caption = null;

        composer.NewCaption += (_, caption_) =>
        {
            caption = caption_;
        };

        await foreach (var displaySet in displaySets)
        {
            composer.Input(displaySet);

            if (caption is Caption caption_)
            {
                yield return caption_;

                caption = null;
            }
        }
    }
#endif
}
