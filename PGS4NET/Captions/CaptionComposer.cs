/*
 * Copyright 2025 William Swartzendruber
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
///     Constructs <see cref="Caption"/>s from <see cref="DisplaySet"/>s.
/// </summary>
public sealed class CaptionComposer
{
    private static readonly CaptionException IllegalTimeStamp
        = new("Current time stamp is less than or equal to the previous one.");
    private static readonly CaptionException PaletteUndefinedException
        = new("A display set referenced an undefined palette ID.");
    private static readonly CaptionException ObjectUndefinedException
        = new("A composition object referenced an undefined object ID.");
    
    private readonly Dictionary<int, AreaState> AreaStates = new();
    private readonly Dictionary<int, DisplayObject> Objects = new();
    private readonly Dictionary<int, DisplayPalette> Palettes = new();
    private readonly HashSet<int> PalettesForced = new();
    private readonly HashSet<int> YcbcrasForced = new();

    private int Size = 0;
    private byte[] PalettePixels = new byte[0];
    private byte[] ClearPalettePixels = new byte[0];
    private YcbcraPixel[] YcbcraPixels = new YcbcraPixel[0];
    private YcbcraPixel[] ClearYcbcraPixels = new YcbcraPixel[0];
    private PgsTimeStamp LastTimeStamp;

    /// <summary>
    ///     The width of the screen, which will be automatically set by the
    ///     <see cref="DisplaySet"/>s coming in, enlarging if necessary.
    /// </summary>
    public int Width { get; private set; } = 0;

    /// <summary>
    ///     The height of the screen, which will be automatically set by the
    ///     <see cref="DisplaySet"/>s coming in, enlarging if necessary.
    /// </summary>
    public int Height { get; private set; } = 0;

    /// <summary>
    ///     Returns <see langword="true"/> if the composer has any buffered graphics that have
    ///     not been written out as one or more <see cref="Caption"/>s.
    /// </summary>
    /// <remarks>
    ///     Buffered graphics can be forced out via the <see cref="Flush(PgsTimeStamp)"/>
    ///     method, if necessary.
    /// </remarks>
    public bool Pending
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Flushes any graphics which are buffered, causing <see cref="Ready"/> to fire for any
    ///     new <see cref="Caption"/>s that become available as a result.
    /// </summary>
    /// <remarks>
    ///     It should not be necessary to call this method with conformant PGS streams as they
    ///     will automatically cause buffered graphics to be flushed out.
    /// </remarks>
    /// <param name="timeStamp">
    ///     The time at which any buffered graphics should disappear from the screen during
    ///     playback.
    /// </param>
    public void Flush(PgsTimeStamp timeStamp)
    {
        if (timeStamp <= LastTimeStamp)
            throw IllegalTimeStamp;

        foreach (var storedArea in AreaStates.Values)
        {
        }

        Clear(timeStamp);
    }

    /// <summary>
    ///     Inputs a <see cref="DisplaySet"/> into the composer, causing <see cref="Ready"/> to
    ///     fire for any new <see cref="Caption"/>s that become available as a result.
    /// </summary>
    /// <param name="displaySet">
    ///     The display set to input.
    /// </param>
    /// <exception cref="CaptionException">
    ///     The <paramref name="displaySet"/> is not valid given the composer's state.
    /// </exception>
    public void Input(DisplaySet displaySet)
    {
        var timeStamp = displaySet.Pts;

        if (displaySet.CompositionState == CompositionState.EpochStart)
        {
            if (Pending)
                Flush(timeStamp);
            else
                Clear();
        }

        EnlargeIfNecessary(displaySet);

        if (!displaySet.PaletteUpdateOnly)
        {
            foreach (var dcEntry in displaySet.Compositions)
            {
                var dcObjectId = dcEntry.Key.ObjectId;
                var version = 0;

                foreach (var doEntry in displaySet.Objects)
                {
                    var doKey = doEntry.Key;

                    if (doKey.Id == dcObjectId && doKey.Version >= version)
                    {
                        version = doKey.Version;
                        Objects[dcObjectId] = doEntry.Value;
                    }
                }
            }

            foreach (var dwEntry in displaySet.Windows)
            {
                var dwId = dwEntry.Key;
                var dw = dwEntry.Value;
                var windowArea = new Area(dw.X, dw.Y, dw.Width, dw.Height);

                EachAreaPixel(windowArea, planeIndex =>
                {
                    PalettePixels[planeIndex] = default;
                });

                foreach (var dcEntry in displaySet.Compositions)
                {
                    if (dcEntry.Key.WindowId == dwId)
                    {
                        var dcObjectId = dcEntry.Key.ObjectId;
                        var dc = dcEntry.Value;

                        if (dc.Forced)
                            PalettesForced.Add(dcObjectId);
                        
                        if (!Objects.TryGetValue(dcObjectId, out var do_))
                            throw ObjectUndefinedException;

                        var objectData = do_.Data;
                        
                        if (dc.Crop is Area crop)
                        {
                            var objectArea = new Area(dc.X, dc.Y, crop.Width, crop.Height);
                            var objectCropData = new byte[crop.Width * crop.Height];
                            var objectReadCropDataIndex = 0;
                            var objectWriteCropDataIndex = 0;

                            EachAreaPixel(do_, crop, objectDataIndex =>
                            {
                                objectCropData[objectWriteCropDataIndex++]
                                    = objectData[objectDataIndex];
                            });
                            EachAreaPixel(objectArea, planeIndex =>
                            {
                                PalettePixels[planeIndex]
                                    = objectCropData[objectReadCropDataIndex++];
                            });

                            AreaStates[dcObjectId] = new AreaState(objectArea, dc.Forced);
                        }
                        else
                        {
                            var objectArea = new Area(dc.X, dc.Y, do_.Width, do_.Height);
                            var objectDataIndex = 0;

                            EachAreaPixel(objectArea, planeIndex =>
                            {
                                PalettePixels[planeIndex] = objectData[objectDataIndex++];
                            });

                            AreaStates[dcObjectId] = new AreaState(objectArea, dc.Forced);
                        }
                    }
                }
            }
        }

        var areaStatesToRemove = new HashSet<int>();
        YcbcraPixel[]? paletteArray = null;

        foreach (var areaStateEntry in AreaStates)
        {
            if (paletteArray is null)
            {
                if (!Palettes.TryGetValue(displaySet.PaletteId, out var displayPalette))
                    throw PaletteUndefinedException;

                paletteArray = PaletteArray(displayPalette);
            }

            var areaState = areaStateEntry.Value;
            var anything = false;
            var different = false;

            EachAreaPixel(areaState.Area, planeIndex =>
            {
                var oldYcbcraPixel = YcbcraPixels[planeIndex];
                var newYcbcraPixel = paletteArray[PalettePixels[planeIndex]];

                anything |= newYcbcraPixel != default;
                different |= newYcbcraPixel != oldYcbcraPixel;

                YcbcraPixels[planeIndex] = newYcbcraPixel;
            });

            if (different)
            {
                OnReady(AreaToCaption(areaState.Area));

                areaState.PendingTimeStamp = anything ? timeStamp : null;

                if (!anything)
                    areaStatesToRemove.Add(areaStateEntry.Key);
            }
        }

        LastTimeStamp = timeStamp;

        foreach (var objectId in areaStatesToRemove)
        { 
            AreaStates.Remove(objectId);

        }
    }

    /// <summary>
    ///     Resets the state of the composer.
    /// </summary>
    public void Reset()
    {
        AreaStates.Clear();
        Objects.Clear();
        Palettes.Clear();
        PaletteForced.Clear();
        YcbcraForced.Clear();
        Width = 0;
        Height = 0;
        Size = 0;
        PalettePixels = new byte[Size];
        ClearPalettePixels = new byte[Size];
        YcbcraPixels = new YcbcraPixel[Size];
        ClearYcbcraPixels = new YcbcraPixel[Size];
        LastTimeStamp = default;
    }

    private Caption AreaToCaption(Area area, PgsTimeStamp timeStamp, PgsTimeStamp duration
        , bool forced)
    {
        var data = new YcbcraPixel[area.Width * area.Height];
        var dataIndex = 0;

        EachAreaPixel(area, planeIndex =>
        {
            data[dataIndex++] = YcbcraPixels[planeIndex];
        });

        return new Caption(timeStamp, duration, area.X, area.Y, area.Width, area.Height, data
            , forced);
    }

    private void Clear(PgsTimeStamp timeStamp)
    {
        Clear();
        LastTimeStamp = timeStamp;
    }

    private void Clear()
    {
        AreaStates.Clear();
        Objects.Clear();
        Palettes.Clear();
        PaletteForced.Clear();
        YcbcraForced.Clear();
        Array.Copy(ClearPalettePixels, PalettePixels, Size);
        Array.Copy(ClearYcbcraPixels, YcbcraPixels, Size);
    }

    private void EachAreaPixel(Area area, Action<int> action)
    {
        var iStart = Width * area.Y;
        var iEnd = Width * (area.Y + area.Height);

        for (var i = iStart; i < iEnd; i += Width)
        {
            var jStart = i + area.X;
            var jEnd = jStart + area.Width;

            for (var j = jStart; j < jEnd; j++)
                action(j);
        }
    }

    private void EnlargeIfNecessary(DisplaySet displaySet)
    {
        var newWidth = Math.Max(Width, displaySet.Width);
        var newHeight = Math.Max(Height, displaySet.Height);

        if (newWidth > Width || newHeight > Height)
        {
            var oldPalettePixels = PalettePixels;
            var oldYcbcraPixels = YcbcraPixels;
            var newSize = newWidth * newHeight;
            var area = new Area(0, 0, Width, Height);
            var oldIndex = 0;

            Width = newWidth;
            Height = newHeight;
            Size = newWidth * newHeight;
            PalettePixels = new byte[Size];
            ClearPalettePixels = new byte[Size];
            YcbcraPixels = new YcbcraPixel[Size];
            ClearYcbcraPixels = new YcbcraPixel[Size];

            EachAreaPixel(area, newIndex =>
            {
                PalettePixels[newIndex] = oldPalettePixels[oldIndex];
                YcbcraPixels[newIndex] = oldYcbcraPixels[oldIndex];

                oldIndex++;
            });
        }
    }

    private void OnReady(Caption caption)
    {
        Ready?.Invoke(this, caption);
    }

    /// <summary>
    ///     Fires when a new <see cref="Caption"/> is ready.
    /// </summary>
    public event EventHandler<Caption>? Ready;

    private static void EachAreaPixel(DisplayObject displayObject, Area area
        , Action<int> action)
    {
        var doWidth = displayObject.Width;
        var iStart = doWidth * area.Y;
        var iEnd = doWidth * (area.Y + area.Height);

        for (var i = iStart; i < iEnd; i += doWidth)
        {
            var jStart = i + area.X;
            var jEnd = jStart + area.Width;

            for (var j = jStart; j < jEnd; j++)
                action(j);
        }
    }

    private static YcbcraPixel[] PaletteArray(DisplayPalette displayPalette)
    {
        var ycbcraPixels = new YcbcraPixel[256];
        byte i = 0;

        do
        {
            var pixel = displayPalette.Entries.TryGetValue(i, out YcbcraPixel entry)
                ? entry
                : default;

            ycbcraPixels[i] = pixel;
        }
        while (i++ != 255);

        return ycbcraPixels;
    }
}
