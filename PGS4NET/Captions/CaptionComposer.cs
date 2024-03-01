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

public class CaptionComposer
{
    private readonly Dictionary<byte, DisplayWindow> Windows = new();
    private readonly Dictionary<byte, DisplayPalette> Palettes = new();
    private readonly Dictionary<ushort, DisplayObject> Objects = new();
    private readonly Dictionary<CompositionId, CompositionValue> Compositions = new();
    private readonly Queue<Caption> CaptionQueue = new();

    public IList<Caption> Input(DisplaySet displaySet)
    {
        var returnValue = new List<Caption>();

        if (displaySet.CompositionState == CompositionState.EpochStart)
            Reset();

        foreach (var windowEntry in displaySet.Windows)
            Windows[windowEntry.Key] = windowEntry.Value;
        foreach (var paletteEntry in displaySet.Palettes)
            Palettes[paletteEntry.Key.Id] = paletteEntry.Value;
        foreach (var objectEntry in displaySet.Objects)
            Objects[objectEntry.Key.Id] = objectEntry.Value;

        foreach (var compositionEntry in displaySets.Compositions)
        {

        }

        if (!displaySet.PaletteUpdateOnly)
            Compositions.Clear();
        foreach (var compositionEntry in displaySet.Compositions)
        {
            var compositionValue = new CompositionValue
            {
                Composition = compositionEntry.Value,
                TimeStamp = displaySet.Pts,
            };

            Compositions[compositionEntry.Key] = compositionValue;
        }


        return returnValue;
    }

    public void Reset()
    {
        Windows.Clear();
        Palettes.Clear();
        Objects.Clear();
        Compositions.Clear();
        CaptionQueue.Clear();
    }

    private struct CompositionValue
    {
        internal DisplayComposition Composition;

        internal PgsTimeStamp TimeStamp;
    }
}
