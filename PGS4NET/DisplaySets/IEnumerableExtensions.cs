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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Extension methods against <see cref="System.Collections.Generic.IEnumerable{T}" /> for
///     constructing and deconstructing display sets;
/// </summary>
public static class IEnumerableExtensions
{
    public static IEnumerable<DisplaySet> DisplaySets(this IEnumerable<Segment> segments)
    {
        var complete = true;
        var composer = new DisplaySetComposer();
        DisplaySet? displaySet = null;

        composer.NewDisplaySet += (_, displaySet_) =>
        {
            displaySet = displaySet_;
            complete = true;
        };

        foreach (var segment in segments)
        {
            complete = false;
            composer.Input(segment);

            if (displaySet is DisplaySet displaySet_)
            {
                yield return displaySet_;

                displaySet = null;
            }
        }

        if (!complete)
            throw new DisplaySetException("Premature end to display set.");
    }

    // TODO: Decompose Captions into DisplaySet instances.
}
