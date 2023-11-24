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

namespace PGS4NET.Captions;

/// <summary>
///     Statefully composes captions using sequentially input display sets. Also supports
///     caption decomposition into display sets.
/// </summary>
public class CaptionComposer
{
    private readonly Dictionary<byte, DisplayWindow> Windows = new();
    private readonly Dictionary<byte, DisplayPalette> Palettes = new();
    private readonly Dictionary<ushort, DisplayObject> Objects = new();

    /// <summary>
    ///     Inputs the next <see cref="DisplaySet" /> into the composer, returning a collection
    ///     of any new <see cref="Caption" />s that could be composed, or an empty collection
    ///     if none could be composed from the provided <see cref="DisplaySet" /> alone.
    /// </summary>
    /// <exception cref="CaptionException">
    ///     Thrown when a combination of otherwise valid display sets are invalid when taken
    ///     together.
    /// </exception>
    public IList<Caption> Input(DisplaySet displaySet)
    {
        var returnValue = new List<Caption>();

        if (displaySet.CompositionState == CompositionState.EpochStart)
            Reset();

        foreach (var window in displaySet.Windows)
            Windows[window.Key] = window.Value;
        foreach (var palette in displaySet.Palettes)
            Palettes[palette.Key.Id] = palette.Value;
        foreach (var object_ in displaySet.Objects)
            Objects[object_.Key.Id] = object_.Value;

        foreach (var composition in displaySet.Compositions)
        {
            // TODO: RESUME HERE
        }

        return returnValue;
    }

    /// <summary>
    ///     Resets the composer's internal state.
    /// </summary>
    public void Reset()
    {
        Windows.Clear();
        Palettes.Clear();
        Objects.Clear();
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
