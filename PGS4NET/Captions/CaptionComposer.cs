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
    
    private readonly Dictionary<int, DisplayObject> StoredObjects = new();
    private readonly Dictionary<byte, StoredWindow> StoredWindows = new();

    private int Size = 0;
    private byte[] PalettePixels = new byte[0];
    private byte[] ClearPalettePixels = new byte[0];
    private YcbcraPixel[] PrimaryYcbcraPixels = new YcbcraPixel[0];
    private YcbcraPixel[] SecondaryYcbcraPixels = new YcbcraPixel[0];
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

        foreach (var storedWindow in StoredWindows.Values)
        {
            if (storedWindow.WindowState.PrimaryPlaneDirty)
            {
                var displayWindow = storedWindow.DisplayWindow;
                var x = displayWindow.X;
                var y = displayWindow.Y;
                var width = displayWindow.Width;
                var height = displayWindow.Height;
                var size = width * height;
                var data = new YcbcraPixel[size];
                var dataIndex = 0;
                var forced = storedWindow.Forced;

                EachWindowPixel(displayWindow, (pixelsIndex, _, _) =>
                {
                    data[dataIndex++] = PrimaryYcbcraPixels[pixelsIndex];
                });

                var caption = new Caption(LastTimeStamp, timeStamp - LastTimeStamp, x, y, width
                    , height, data, forced);

                OnReady(caption);
            }
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
        if (displaySet.CompositionState == CompositionState.EpochStart)
        {
            if (Pending)
                Flush(displaySet.Pts);
            else
                Clear();
        }

        EnlargeIfNecessary(displaySet);

        if (displaySet.PaletteUpdateOnly)
        {
        }
        else
        {

        }
    }

    /// <summary>
    ///     Resets the state of the composer.
    /// </summary>
    public void Reset()
    {
        StoredObjects.Clear();
        StoredWindows.Clear();
        Width = 0;
        Height = 0;
        Size = 0;
        PalettePixels = new byte[Size];
        ClearPalettePixels = new byte[Size];
        PrimaryYcbcraPixels = new YcbcraPixel[Size];
        SecondaryYcbcraPixels = new YcbcraPixel[Size];
        ClearYcbcraPixels = new YcbcraPixel[Size];
        LastTimeStamp = default;
    }

    private void Clear(PgsTimeStamp timeStamp)
    {
        Clear();
        LastTimeStamp = timeStamp;
    }

    private void Clear()
    {
        StoredObjects.Clear();
        StoredWindows.Clear();
        Array.Copy(ClearPalettePixels, PalettePixels, Size);
        Array.Copy(ClearYcbcraPixels, PrimaryYcbcraPixels, Size);
        Array.Copy(ClearYcbcraPixels, SecondaryYcbcraPixels, Size);
    }

    private WindowState DetermineWindowState(DisplayWindow displayWindow)
    {
        bool primaryPlaneDirty = false;
        bool secondaryPlaneDirty = false;
        bool planesDiffer = false;

        EachWindowPixel(displayWindow, (index, _, _) =>
        {
            primaryPlaneDirty |= PrimaryYcbcraPixels[index] != default;
            secondaryPlaneDirty |= SecondaryYcbcraPixels[index] != default;
            planesDiffer |= PrimaryYcbcraPixels[index] != SecondaryYcbcraPixels[index];
        });

        return new WindowState(primaryPlaneDirty, secondaryPlaneDirty, planesDiffer);
    }

    private void EachWindowPixel(DisplayWindow displayWindow, Action<int, int, int> action)
    {
        var iStart = Width * displayWindow.Y;
        var iEnd = Width * (displayWindow.Y + displayWindow.Height);

        for (var i = iStart; i < iEnd; i += Width)
        {
            var jStart = i + displayWindow.X;
            var jEnd = jStart + displayWindow.Width;

            for (var j = jStart; j < jEnd; j++)
                action(j, j % Width, j / Width);
        }
    }

    private void EnlargeIfNecessary(DisplaySet displaySet)
    {
        var newWidth = Math.Max(Width, displaySet.Width);
        var newHeight = Math.Max(Height, displaySet.Height);

        if (newWidth > Width || newHeight > Height)
        {
            var oldPalettePixels = PalettePixels;
            var oldPrimaryYcbcraPixels = PrimaryYcbcraPixels;
            var oldSecondaryYcbcraPixels = SecondaryYcbcraPixels;
            var newSize = newWidth * newHeight;
            var fauxDisplayWindow = new DisplayWindow(0, 0, Width, Height);
            var oldIndex = 0;

            Width = newWidth;
            Height = newHeight;
            Size = newWidth * newHeight;
            PalettePixels = new byte[Size];
            ClearPalettePixels = new byte[Size];
            PrimaryYcbcraPixels = new YcbcraPixel[Size];
            SecondaryYcbcraPixels = new YcbcraPixel[Size];
            ClearYcbcraPixels = new YcbcraPixel[Size];

            EachWindowPixel(fauxDisplayWindow, (newIndex, _, _) =>
            {
                PalettePixels[newIndex] = oldPalettePixels[oldIndex];
                PrimaryYcbcraPixels[newIndex] = oldPrimaryYcbcraPixels[oldIndex];
                SecondaryYcbcraPixels[newIndex] = oldSecondaryYcbcraPixels[oldIndex];

                oldIndex++;
            });
        }
    }

    private void OnReady(Caption caption)
    {
        Ready?.Invoke(this, caption);
    }

    private void PostProcessRegion(PgsTimeStamp timeStamp, StoredWindow storedWindow
        , bool isForced)
    {
        var displayWindow = storedWindow.DisplayWindow;
        var wasForced = storedWindow.Forced;
        var state = DetermineWindowState(displayWindow);

        if (!state.PrimaryPlaneDirty && !state.SecondaryPlaneDirty)
        {
            // We had nothing before and we have nothing now.
        }
        else if (state.PrimaryPlaneDirty && !state.SecondaryPlaneDirty)
        {
            // We had nothing before but we have something now.

            LastTimeStamp = timeStamp;
        }
        else if (!state.PrimaryPlaneDirty && state.SecondaryPlaneDirty)
        {
            // We had something before but we have nothing now.

            var x = displayWindow.X;
            var y = displayWindow.Y;
            var width = displayWindow.Width;
            var height = displayWindow.Height;
            var size = width * height;
            var data = new YcbcraPixel[size];
            var dataIndex = 0;

            EachWindowPixel(displayWindow, (pixelsIndex, _, _) =>
            {
                data[dataIndex++] = SecondaryYcbcraPixels[pixelsIndex];
            });

            var caption = new Caption(LastTimeStamp, timeStamp - LastTimeStamp, x, y, width
                , height, data, wasForced);

            OnReady(caption);
        }
        else if (state.PrimaryPlaneDirty && state.SecondaryPlaneDirty)
        {
            // We had something before and we have something now.

            if (state.PlanesDiffer || wasForced != isForced)
            {
                var x = displayWindow.X;
                var y = displayWindow.Y;
                var width = displayWindow.Width;
                var height = displayWindow.Height;
                var size = width * height;
                var data = new YcbcraPixel[size];
                var dataIndex = 0;

                EachWindowPixel(displayWindow, (pixelsIndex, _, _) =>
                {
                    data[dataIndex++] = SecondaryYcbcraPixels[pixelsIndex];
                });

                var caption = new Caption(LastTimeStamp, timeStamp - LastTimeStamp, x, y, width
                    , height, data, wasForced);

                OnReady(caption);

                LastTimeStamp = timeStamp;
            }
        }
        else
        {
            // We shouldn't ever be here...
            throw new InvalidOperationException("Illegal state.");
        }

        storedWindow.Forced = isForced;
        storedWindow.WindowState = state;

        Array.Copy(PrimaryYcbcraPixels, SecondaryYcbcraPixels, Size);
    }

    /// <summary>
    ///     Fires when a new <see cref="Caption"/> is ready.
    /// </summary>
    public event EventHandler<Caption>? Ready;
}
