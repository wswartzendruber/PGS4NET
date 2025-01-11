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
using PGS4NET.Captions;
using PGS4NET.DisplaySets;

namespace PGS4NET;

internal class GraphicsPlane
{
    private static readonly PgsException IllegalTimeStamp
        = new("Current time stamp is less than or equal to the previous one.");

    private readonly int Width;
    private readonly int Height;
    private readonly long Size;
    private readonly byte[] PalettePixels;
    private readonly byte[] ClearPalettePixels;
    private readonly YcbcraPixel[] PrimaryYcbcraPixels;
    private readonly YcbcraPixel[] SecondaryYcbcraPixels;
    private readonly Dictionary<ulong, DisplayWindow> ActiveWindows = new();

    private bool Forced = false;
    private PgsTimeStamp StartTimeStamp = default;

    internal GraphicsPlane(int width, int height)
    {
        Width = width;
        Height = height;
        Size = width * height;
        PalettePixels = new byte[Size];
        ClearPalettePixels = new byte[Size];
        PrimaryYcbcraPixels = new YcbcraPixel[Size];
        SecondaryYcbcraPixels = new YcbcraPixel[Size];
    }

    internal void Clear(PgsTimeStamp timeStamp)
    {
        
        ActiveWindows.Clear();
    }

    internal void Update(PgsTimeStamp timeStamp, DisplayPalette displayPalette
        , List<DisplayWindow> displayWindows, List<GraphicsOperation> graphicsOperations)
    {
        Array.Copy(ClearPalettePixels, PalettePixels, Size);
    }

    internal void Update(PgsTimeStamp timeStamp, DisplayPalette displayPalette)
    {
    }

    private YcbcraPixel[] CopyWindowToNewYcbcraArray(DisplayWindow displayWindow)
    {
        var returnValue = new YcbcraPixel[displayWindow.Width * displayWindow.Height];
        var index = 0;
        var yStart = Width * displayWindow.Y;
        var yEnd = Width * (displayWindow.Y + displayWindow.Height);

        for (var y = yStart; y < yEnd; y += Width)
        {
            var xStart = y + displayWindow.X;
            var xEnd = xStart + displayWindow.Width;

            for (var x = xStart; x < xEnd; x++)
                returnValue[index++] = PrimaryYcbcraPixels[x];
        }

        return returnValue;
    }

    private GraphicsState GetWindowState(DisplayWindow displayWindow)
    {
        var primaryPlaneDirty = false;
        var secondaryPlaneDirty = false;
        var planesDiffer = false;
        var yStart = Width * displayWindow.Y;
        var yEnd = Width * (displayWindow.Y + displayWindow.Height);

        for (var y = yStart; y < yEnd; y += Width)
        {
            var xStart = y + displayWindow.X;
            var xEnd = xStart + displayWindow.Width;

            for (var x = xStart; x < xEnd; x++)
            {
                primaryPlaneDirty |= PrimaryYcbcraPixels[x] != default;
                secondaryPlaneDirty |= SecondaryYcbcraPixels[x] != default;
                planesDiffer |= PrimaryYcbcraPixels[x] != SecondaryYcbcraPixels[x];
            }
        }

        return new GraphicsState(primaryPlaneDirty, secondaryPlaneDirty, planesDiffer);
    }

    private void OnReady(Caption caption)
    {
        Ready?.Invoke(this, caption);
    }

    internal event EventHandler<Caption>? Ready;

    private static ulong DynamicHash(DisplayWindow displayWindow)
    {
        unchecked
        {
            uint hash1 = 17;
            uint hash2 = 17;

            hash1 = hash1 * 23 + (uint)displayWindow.X.GetHashCode();
            hash1 = hash1 * 23 + (uint)displayWindow.Y.GetHashCode();
            hash2 = hash2 * 23 + (uint)displayWindow.Width.GetHashCode();
            hash2 = hash2 * 23 + (uint)displayWindow.Height.GetHashCode();

            return (hash2 << 32) | hash1;
        }
    }

    private static YcbcraPixel[] PaletteArray(DisplayPalette displayPalette)
    {
        var paletteArray = new YcbcraPixel[256];
        byte i = 0;

        do
        {
            var pixel = displayPalette.Entries.TryGetValue(i, out YcbcraPixel entry)
                ? entry
                : default;

            paletteArray[i] = pixel;
        }
        while (i++ != 255);

        return paletteArray;
    }
}
