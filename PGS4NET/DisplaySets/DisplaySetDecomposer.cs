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

/// <summary>
///     Deconstructs PGS display sets into segments.
/// </summary>
public static class DisplaySetDecomposer
{
    /// <summary>
    ///     Deconstructs a PGS display set into a collection of segments.
    /// </summary>
    /// <param name="displaySet">
    ///     The display set to deconstruct.
    /// </param>
    /// <returns>
    ///     A list of segments representing the display set.
    /// </returns>
    public static IList<Segment> Decompose(DisplaySet displaySet)
    {
        var returnValue = new List<Segment>();
        var compositionObjects = new List<CompositionObject>();

        foreach (var item in displaySet.Compositions)
        {
            var cid = new CompositionId(item.Key.ObjectId, item.Key.WindowId);
            var co = new CompositionObject(cid, item.Value.X, item.Value.Y, item.Value.Forced
                , item.Value.Crop);

            compositionObjects.Add(co);
        }

        var pcs = new PresentationCompositionSegment(displaySet.Pts, displaySet.Dts
            , displaySet.Width, displaySet.Height, displaySet.FrameRate
            , displaySet.CompositionNumber, displaySet.CompositionState
            , displaySet.PaletteUpdateOnly, displaySet.PaletteId, compositionObjects);

        returnValue.Add(pcs);

        if (displaySet.Windows.Count > 0)
        {
            var windowEntries = new List<WindowDefinitionEntry>();

            foreach (var window in displaySet.Windows)
            {
                windowEntries.Add(new WindowDefinitionEntry(window.Key, window.Value.X
                    , window.Value.Y, window.Value.Width, window.Value.Height));
            }

            var wds = new WindowDefinitionSegment(displaySet.Pts, displaySet.Dts
                , windowEntries);

            returnValue.Add(wds);
        }

        foreach (var palette in displaySet.Palettes)
        {
            var paletteEntries = new List<PaletteDefinitionEntry>();

            foreach (var paletteEntry in palette.Value.Entries)
            {
                var pixel = new YcbcraPixel(paletteEntry.Value.Y, paletteEntry.Value.Cb
                    , paletteEntry.Value.Cr, paletteEntry.Value.Alpha);
                var pde = new PaletteDefinitionEntry(paletteEntry.Key, pixel);

                paletteEntries.Add(pde);
            }

            var pds = new PaletteDefinitionSegment(displaySet.Pts, displaySet.Dts, palette.Key
                , paletteEntries);

            returnValue.Add(pds);
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

                var iods = new InitialObjectDefinitionSegment(displaySet.Pts, displaySet.Dts
                    , displayObject.Key, displayObject.Value.Width, displayObject.Value.Height
                    , (uint)data.Length + 4, iodsBuffer);

                returnValue.Add(iods);

                index += InitialObjectDefinitionSegment.MaxDataSize;
                size -= InitialObjectDefinitionSegment.MaxDataSize;

                while (size > MiddleObjectDefinitionSegment.MaxDataSize)
                {
                    var modsBuffer = new byte[MiddleObjectDefinitionSegment.MaxDataSize];

                    Array.Copy(data, index, modsBuffer, 0, modsBuffer.Length);

                    var mods = new MiddleObjectDefinitionSegment(displaySet.Pts, displaySet.Dts
                        , displayObject.Key,  modsBuffer);

                    returnValue.Add(mods);

                    index += MiddleObjectDefinitionSegment.MaxDataSize;
                    size -= MiddleObjectDefinitionSegment.MaxDataSize;
                }

                var fodsBuffer = new byte[size];

                Array.Copy(data, index, fodsBuffer, 0, fodsBuffer.Length);

                var fods = new FinalObjectDefinitionSegment(displaySet.Pts, displaySet.Dts
                    , displayObject.Key, fodsBuffer);

                returnValue.Add(fods);
            }
            else
            {
                var sods = new SingleObjectDefinitionSegment(displaySet.Pts, displaySet.Dts
                    , displayObject.Key, displayObject.Value.Width, displayObject.Value.Height
                    , data);

                returnValue.Add(sods);
            }
        }

        var es = new EndSegment(displaySet.Pts, displaySet.Dts);

        returnValue.Add(es);

        return returnValue;
    }
}
