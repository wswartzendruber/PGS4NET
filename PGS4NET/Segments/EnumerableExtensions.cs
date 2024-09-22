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

namespace PGS4NET.Segments;

/// <summary>
///     Extension methods against <see cref="System.Collections.Generic.IEnumerable{T}" /> for
///     deconstructing display sets;
/// </summary>
public static class EnumerableExtensions
{
    public static IEnumerable<Segment> Segments(this IEnumerable<DisplaySet> displaySets)
    {
        foreach (var displaySet in displaySets)
        {
            foreach (var segment in DisplaySetDecomposer.Decompose(displaySet))
                yield return segment;
        }
    }
}
