/*
 * Copyright 2023 William Swartzendruber
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

namespace PGS4NET.Epochs;

/// <summary>
///     Statefully composes epochs using sequentially input display sets. Also supports
///     immediate epoch decomposition into display sets.
/// </summary>
public class EpochComposer
{
    /// <summary>
    ///     Inputs the next <see cref="DisplaySet" /> into the composer, returning a new
    ///     <see cref="Epoch" /> instance if one can be composed, or <see langword="null" /> if
    ///     more display sets are required.
    /// </summary>
    /// <exception cref="EpochException">
    ///     Thrown when a combination of otherwise valid display sets cannot be combined into an
    ///     epoch.
    /// </exception>
    public Epoch? Input(DisplaySet displaySet)
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
    ///     Decomposes a single epoch into a collection of display sets.
    /// </summary>
    public static IList<DisplaySet> Decompose(Epoch epoch)
    {
        var returnValue = new List<DisplaySet>();

        return returnValue;
    }
}
