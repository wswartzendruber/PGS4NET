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
    private readonly Dictionary<byte, int> WindowHashCodes = new();
    private readonly Dictionary<byte, DisplayPalette> Palettes = new();
    private readonly Dictionary<byte, int> PaletteHashCodes = new();
    private readonly Dictionary<ushort, DisplayObject> Objects = new();
    private readonly Dictionary<ushort, int> ObjectHashCodes = new();
    private readonly Dictionary<CompositionId, BufferedImage> BufferedImages = new();

    public IList<Caption> Input(DisplaySet displaySet)
    {
        var returnValue = new List<Caption>();

        if (displaySet.CompositionState == CompositionState.EpochStart)
            Reset();

        if (!displaySet.PaletteUpdateOnly)
        {
            var trashKeys = new List<CompositionId>();

            foreach (var entry in displaySet.Windows)
            {
                Windows[entry.Key] = entry.Value;
                WindowHashCodes[entry.Key] = entry.Value.GetHashCode();
            }
            foreach (var entry in displaySet.Palettes)
            {
                Palettes[entry.Key.Id] = entry.Value;
                PaletteHashCodes[entry.Key.Id] = entry.Value.GetHashCode();
            }
            foreach (var entry in displaySet.Objects)
            {
                Objects[entry.Key.Id] = entry.Value;
                ObjectHashCodes[entry.Key.Id] = entry.Value.GetHashCode();
            }

            foreach (var entry in BufferedImages)
            {
                var compositionId = entry.Key;
                var bufferedImage = entry.Value;

                if (!displaySet.Compositions.ContainsKey(compositionId)
                    || displaySet.Compositions[compositionId].GetHashCode()
                        != bufferedImage.CompositionHashCode
                    || Windows[compositionId.WindowId].GetHashCode()
                        != bufferedImage.WindowHashCode
                    || Palettes[displaySet.PaletteId].GetHashCode()
                        != bufferedImage.PaletteHashCode
                    || Objects[compositionId.ObjectId].GetHashCode()
                        != bufferedImage.ObjectHashCode)
                {
                    trashKeys.Add(compositionId);
                    returnValue.Add(bufferedImage.ToCaption(displaySet.Dts));
                }
            }

            foreach (var trashKey in trashKeys)
                BufferedImages.Remove(trashKey);

            foreach (var entry in displaySet.Compositions)
            {
                var compositionId = entry.Key;
                var displayComposition = entry.Value;

                if (!BufferedImages.ContainsKey(compositionId))
                {
                    if (!Windows.TryGetValue(compositionId.WindowId
                        , out DisplayWindow displayWindow))
                    {
                        throw new CaptionException("Composition object references undefined "
                            + "window.");
                    }
                    if (!Palettes.TryGetValue(displaySet.PaletteId
                        , out DisplayPalette displayPalette))
                    {
                        throw new CaptionException("Composition object references undefined "
                            + "palette.");
                    }
                    if (!Objects.TryGetValue(compositionId.ObjectId
                        , out DisplayObject displayObject))
                    {
                        throw new CaptionException("Composition object references undefined "
                            + "object.");
                    }

                    BufferedImages[compositionId] = new BufferedImage(displaySet.Dts
                        , displayWindow, displayPalette, displayObject, displayComposition);
                }
            }
        }

        return returnValue;
    }

    public void Reset()
    {
        Windows.Clear();
        WindowHashCodes.Clear();
        Palettes.Clear();
        PaletteHashCodes.Clear();
        Objects.Clear();
        ObjectHashCodes.Clear();
        BufferedImages.Clear();
    }

    private class BufferedImage
    {
        public int WindowHashCode;
        public int PaletteHashCode;
        public int ObjectHashCode;
        public int CompositionHashCode;

        private readonly PgsTimeStamp TimeStamp;
        private readonly ushort X;
        private readonly ushort Y;
        private readonly ushort Width;
        private readonly ushort Height;
        private readonly byte[] Data;
        private readonly bool Forced;

        private PgsPixel[] PaletteLut;

        public BufferedImage(PgsTimeStamp timeStamp, DisplayWindow displayWindow
            , DisplayPalette displayPalette, DisplayObject displayObject
            , DisplayComposition displayComposition)
        {
            TimeStamp = timeStamp;
            X = displayWindow.X;
            Y = displayWindow.Y;
            Width = displayObject.Width;
            Height = displayObject.Height;
            Data = displayObject.Data;
            PaletteLut = GeneratePaletteLut(displayPalette.Entries);
            Forced = displayComposition.Forced;

            WindowHashCode = displayWindow.GetHashCode();
            PaletteHashCode = displayPalette.GetHashCode();
            ObjectHashCode = displayObject.GetHashCode();
            CompositionHashCode = displayComposition.GetHashCode();
        }

        public PgsPixel[] UpdatePalette(IDictionary<byte, PgsPixel> palette)
        {
            var returnValue = Render();

            PaletteLut = GeneratePaletteLut(palette);

            return returnValue;
        }

        public Caption ToCaption(PgsTimeStamp endingTimeStamp)
        {
            return new Caption
            {
                TimeStamp = TimeStamp,
                Duration = endingTimeStamp - TimeStamp,
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                Data = Render(),
                Forced = Forced,
            };
        }

        private PgsPixel[] GeneratePaletteLut(IDictionary<byte, PgsPixel> palette)
        {
            var returnValue = new PgsPixel[256];

            foreach (var entry in palette)
                returnValue[entry.Key] = entry.Value;

            return returnValue;
        }

        private PgsPixel[] Render()
        {
            var returnValue = new PgsPixel[Data.Length];

            for (int i = 0; i < returnValue.Length; i++)
                returnValue[i] = PaletteLut[Data[i]];

            return returnValue;
        }
    }
}
