/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;
using System.Collections.Generic;
using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

/// <summary>
///     Statefully composes captions using sequentially input display sets. Also supports
///     immediate caption decomposition into display sets.
/// </summary>
public class CaptionComposer
{

    /// <summary>
    ///     Inputs the next <see cref="DisplaySet" /> into the composer, returning a new
    ///     <see cref="Caption" /> instance if one can be composed, or <see langword="null" />
    ///     if more display sets are required.
    /// </summary>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of otherwise valid PGS display sets cannot be combined
    ///     into a caption.
    /// </exception>
    public Caption? Input(DisplaySet displaySet)
    {
        return null;
    }

    /// <summary>
    ///     Resets the composer's internal state.
    /// </summary>
    public void Reset()
    {
    }

    /// <summary>
    ///     Decomposes a single caption into a collection of display sets.
    /// </summary>
    public static IList<DisplaySet> Decompose(Caption caption)
    {
        var returnValue = new List<DisplaySet>();

        return returnValue;
    }
}
