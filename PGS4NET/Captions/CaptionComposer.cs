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
    private readonly Dictionary<byte, PgsPixel[]> Palettes = new();
    private readonly Dictionary<ushort, DisplayObject> Objects = new();
    private readonly Dictionary<CompositionId, CompositionValue> Compositions = new();

    public IList<Caption> Input(DisplaySet displaySet)
    {
        var returnValue = new List<Caption>();

        if (displaySet.CompositionState == CompositionState.EpochStart)
        {
            // EPOCH_START resets everything.
            Reset();
        }

        foreach (var compositionEntry in Compositions)
        {
            if (!displaySet.Compositions.ContainsKey(compositionEntry.Key))
            {
                // We have a composition that we've placed onto the screen that isn't there
                // anymore; we now know its appearance timestamp and its duration so we can
                // compose a caption.

                var compositionId = compositionEntry.Key;
                var compositionValue = compositionEntry.Value;

                if (!Palettes.TryGetValue(compositionValue.PaletteId, out PgsPixel[] palette))
                {
                    throw new CaptionException("PCS has invalid palette ID.");
                }
                if (!Objects.TryGetValue(compositionId.ObjectId
                    , out DisplayObject displayObject))
                {
                    throw new CaptionException("Composition object has invalid object ID.");
                }

                var caption = new Caption
                {
                    TimeStamp = compositionValue.TimeStamp,
                    Duration = compositionValue.TimeStamp - displaySet.Pts,
                    X = compositionValue.Composition.X,
                    Y = compositionValue.Composition.Y,
                    Width = displayObject.Width,
                    Height = displayObject.Height,
                    Data = ComposeCaptionImage(palette, displayObject.Data),
                };

                returnValue.Add(caption);
            }
        }

        foreach (var paletteEntry in displaySet.Palettes)
            Palettes[paletteEntry.Key.Id] = GeneratePaletteLut(paletteEntry.Value.Entries);
        foreach (var objectEntry in displaySet.Objects)
            Objects[objectEntry.Key.Id] = objectEntry.Value;

        if (!displaySet.PaletteUpdateOnly)
            Compositions.Clear();
        foreach (var compositionEntry in displaySet.Compositions)
        {
            var compositionValue = new CompositionValue
            {
                Composition = compositionEntry.Value,
                TimeStamp = displaySet.Pts,
                PaletteId = displaySet.PaletteId,
            };

            Compositions[compositionEntry.Key] = compositionValue;
        }


        return returnValue;
    }

    public void Reset()
    {
        Palettes.Clear();
        Objects.Clear();
        Compositions.Clear();
    }

    private PgsPixel[] GeneratePaletteLut(IDictionary<byte, PgsPixel> entries)
    {
        var lut = new PgsPixel[256];

        foreach (var entry in entries)
            lut[entry.Key] = entry.Value;

        return lut;
    }

    private PgsPixel[] ComposeCaptionImage(PgsPixel[] palette, byte[] values)
    {
        var pixels = new PgsPixel[values.Length];

        for (int i = 0; i < values.Length; i++)
            pixels[i] = palette[values[i]];

        return pixels;
    }

    private struct CompositionValue
    {
        internal DisplayComposition Composition;

        internal PgsTimeStamp TimeStamp;

        internal byte PaletteId;
    }
}
