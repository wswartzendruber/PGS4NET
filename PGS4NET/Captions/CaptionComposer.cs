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
    private readonly Dictionary<VersionedId<byte>, DisplayPalette> Palettes = new();
    private readonly Dictionary<VersionedId<ushort>, DisplayObject> Objects = new();
    private readonly Dictionary<CompositionId, BufferedImage> BufferedImages = new();

    public IList<Caption> Input(DisplaySet displaySet)
    {
        var returnValue = new List<Caption>();

        if (displaySet.CompositionState == CompositionState.EpochStart)
            Reset();

        foreach (var entry in displaySet.Windows)
            Windows[entry.Key] = entry.Value;
        foreach (var entry in displaySet.Palettes)
            Palettes[entry.Key] = entry.Value;
        foreach (var entry in displaySet.Objects)
            Objects[entry.Key] = entry.Value;

        foreach (var compositionEntry in displaySet.Compositions)
        {
            var paletteId = displaySet.PaletteId;
            var compositionId = compositionEntry.Key;
            var composition = compositionEntry.Value;
            var objectId = compositionId.ObjectId;
            var windowId = compositionId.WindowId;

            if (!Palettes.TryGetLatestValue(paletteId, out var displayPalette))
                throw new CaptionException("Display set does not define the set palette.");

            if (!Windows.TryGetValue(windowId, out var displayWindow))
                throw new CaptionException("Composition object references undefined window.");

            if (!Objects.TryGetLatestValue(objectId, out var displayObject))
                throw new CaptionException("Composition object references undefined object.");
        }

        return returnValue;
    }

    public void Reset()
    {
        BufferedImages.Clear();
    }

    private class BufferedImage
    {
        private readonly PgsTimeStamp TimeStamp;
        private readonly ushort X;
        private readonly ushort Y;
        private readonly ushort Width;
        private readonly ushort Height;
        private readonly byte[] Data;

        private PgsPixel[] PaletteLut;

        public BufferedImage(PgsTimeStamp timeStamp, ushort x, ushort y, ushort width
            , ushort height, byte[] data, IDictionary<byte, PgsPixel> palette)
        {
            TimeStamp = timeStamp;
            X = y;
            Y = y;
            Width = width;
            Height = height;
            Data = data;
            PaletteLut = GeneratePaletteLut(palette);
        }

        public PgsPixel[] Render()
        {
            var returnValue = new PgsPixel[Data.Length];

            for (int i = 0; i < returnValue.Length; i++)
                returnValue[i] = PaletteLut[Data[i]];

            return returnValue;
        }

        public PgsPixel[] UpdatePalette(IDictionary<byte, PgsPixel> palette)
        {
            var returnValue = Render();

            PaletteLut = GeneratePaletteLut(palette);

            return returnValue;
        }

        private PgsPixel[] GeneratePaletteLut(IDictionary<byte, PgsPixel> palette)
        {
            var returnValue = new PgsPixel[256];

            foreach (var entry in palette)
                returnValue[entry.Key] = entry.Value;

            return returnValue;
        }
    }
}
