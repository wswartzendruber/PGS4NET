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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

public static class DisplaySetDecomposer
{
    public static IList<Segment> Decompose(DisplaySet displaySet)
    {
        var returnValue = new List<Segment>();
        var compositionObjects = new List<CompositionObject>();

        foreach (var item in displaySet.Compositions)
        {
            compositionObjects.Add(new CompositionObject
            {
                ObjectId = item.Key.ObjectId,
                WindowId = item.Key.WindowId,
                X = item.Value.X,
                Y = item.Value.Y,
                Forced = item.Value.Forced,
                Crop = item.Value.Crop,
            });
        }

        returnValue.Add(new PresentationCompositionSegment
        {
            Pts = displaySet.Pts,
            Dts = displaySet.Dts,
            Width = displaySet.Width,
            Height = displaySet.Height,
            FrameRate = displaySet.FrameRate,
            Number = displaySet.CompositionNumber,
            State = displaySet.CompositionState,
            PaletteUpdateOnly = displaySet.PaletteUpdateOnly,
            PaletteId = displaySet.PaletteId,
            CompositionObjects = compositionObjects,
        });

        if (displaySet.Windows.Count > 0)
        {
            var windowEntries = new List<WindowDefinitionEntry>();

            foreach (var window in displaySet.Windows)
            {
                windowEntries.Add(new WindowDefinitionEntry
                {
                    Id = window.Key,
                    X = window.Value.X,
                    Y = window.Value.Y,
                    Width = window.Value.Width,
                    Height = window.Value.Height,
                });
            }

            returnValue.Add(new WindowDefinitionSegment
            {
                Pts = displaySet.Pts,
                Dts = displaySet.Dts,
                Definitions = windowEntries,
            });
        }

        foreach (var palette in displaySet.Palettes)
        {
            var paletteEntries = new List<PaletteDefinitionEntry>();

            foreach (var paletteEntry in palette.Value.Entries)
            {
                paletteEntries.Add(new PaletteDefinitionEntry
                {
                    Id = paletteEntry.Key,
                    Pixel = new PgsPixel(paletteEntry.Value.Y, paletteEntry.Value.Cr
                        , paletteEntry.Value.Cb, paletteEntry.Value.Alpha),
                });
            }

            returnValue.Add(new PaletteDefinitionSegment
            {
                Pts = displaySet.Pts,
                Dts = displaySet.Dts,
                Id = palette.Key.Id,
                Version = palette.Key.Version,
                Entries = paletteEntries,
            });
        }

        foreach (var displayObject in displaySet.Objects)
        {
            var data = Rle.Compress(displayObject.Value.Data, displayObject.Value.Width
                , displayObject.Value.Height);

            if (data.Length > InitialObjectDefinitionSegment.MaxDataSize)
            {
                var index = 0;
                var size = data.Length;
                var iodsBuffer = new byte[InitialObjectDefinitionSegment.MaxDataSize];

                Array.Copy(data, iodsBuffer, iodsBuffer.Length);

                returnValue.Add(new InitialObjectDefinitionSegment
                {
                    Pts = displaySet.Pts,
                    Dts = displaySet.Dts,
                    Id = displayObject.Key.Id,
                    Version = displayObject.Key.Version,
                    Data = iodsBuffer,
                    Length = (uint)data.Length + 4,
                    Width = displayObject.Value.Width,
                    Height = displayObject.Value.Height,
                });

                index += InitialObjectDefinitionSegment.MaxDataSize;
                size -= InitialObjectDefinitionSegment.MaxDataSize;

                while (size > MiddleObjectDefinitionSegment.MaxDataSize)
                {
                    var modsBuffer = new byte[MiddleObjectDefinitionSegment.MaxDataSize];

                    Array.Copy(data, index, modsBuffer, 0, modsBuffer.Length);

                    returnValue.Add(new MiddleObjectDefinitionSegment
                    {
                        Pts = displaySet.Pts,
                        Dts = displaySet.Dts,
                        Id = displayObject.Key.Id,
                        Version = displayObject.Key.Version,
                        Data = modsBuffer,
                    });

                    index += MiddleObjectDefinitionSegment.MaxDataSize;
                    size -= MiddleObjectDefinitionSegment.MaxDataSize;
                }

                var fodsBuffer = new byte[size];

                Array.Copy(data, index, fodsBuffer, 0, fodsBuffer.Length);

                returnValue.Add(new FinalObjectDefinitionSegment
                {
                    Pts = displaySet.Pts,
                    Dts = displaySet.Dts,
                    Id = displayObject.Key.Id,
                    Version = displayObject.Key.Version,
                    Data = fodsBuffer,
                });
            }
            else
            {
                returnValue.Add(new SingleObjectDefinitionSegment
                {
                    Pts = displaySet.Pts,
                    Dts = displaySet.Dts,
                    Id = displayObject.Key.Id,
                    Version = displayObject.Key.Version,
                    Data = data,
                    Width = displayObject.Value.Width,
                    Height = displayObject.Value.Height,
                });
            }
        }

        returnValue.Add(new EndSegment
        {
            Pts = displaySet.Pts,
            Dts = displaySet.Dts,
        });

        return returnValue;
    }
}
