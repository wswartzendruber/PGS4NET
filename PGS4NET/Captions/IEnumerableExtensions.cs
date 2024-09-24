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
}
