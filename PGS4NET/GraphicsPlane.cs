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
using PGS4NET.Captions;
using PGS4NET.DisplaySets;

namespace PGS4NET;

public class GraphicsPlane
{
    private readonly uint Size;
    private readonly PgsPixel[] Palette;
    private readonly PgsPixel[] PrimaryPixels;
    private readonly PgsPixel[] SecondaryPixels;

    private PgsTimeStamp LastTimeStamp = default;
    private bool LastForced = false;
    private bool Dirty = false;

    public event EventHandler<Caption>? NewCaptionReady;

    public ushort X { get; }

    public ushort Y { get; }

    public ushort Width { get; }

    public ushort Height { get; }

    public GraphicsPlane(DisplayWindow displayWindow)
    {
        Size = (uint)(displayWindow.Width * displayWindow.Height);
        Palette = new PgsPixel[256];
        PrimaryPixels = new PgsPixel[Size];
        SecondaryPixels = new PgsPixel[Size];

        X = displayWindow.X;
        Y = displayWindow.Y;
        Width = displayWindow.Width;
        Height = displayWindow.Height;
    }

    public void Clear(PgsTimeStamp timeStamp)
    {
        if (Dirty)
        {
            var caption = new Caption(LastTimeStamp, timeStamp - LastTimeStamp, X, Y, Width
                , Height, CopyPrimaryPixels(), LastForced);

            OnNewCaptionReady(caption);
        }

        Reset(timeStamp, Dirty);
    }

    public void Draw(PgsTimeStamp timeStamp, DisplayObject displayObject
        , DisplayComposition displayComposition, DisplayPalette displayPalette)
    {
        uint soX;
        uint soY;
        uint sWidth = displayObject.Width;
        uint sHeight = displayObject.Height;
        uint doX = (uint)(displayComposition.X - X);
        uint doY = (uint)(displayComposition.Y - Y);
        uint dWidth = Width;
        uint dHeight = Height;
        uint width;
        uint height;

        if (displayComposition.Crop is Crop crop)
        {
            soX = crop.X;
            soY = crop.Y;
            width = crop.Width;
            height = crop.Height;
        }
        else
        {
            soX = 0;
            soY = 0;
            width = displayObject.Width;
            height = displayObject.Height;
        }

        for (byte i = 0; i <= 255; i++)
        {
            var pixel = displayPalette.Entries.TryGetValue(i, out PgsPixel entry)
                ? entry
                : default;

            Palette[i] = pixel;
        }

        for (uint y = 0; y < height; y++)
        {
            uint sBase = sWidth * (y + soY) + soX;
            uint dBase = dWidth * (y + doY) + doX;

            for (uint x = 0; x < height; x++)
            {
                var dot = displayObject.Data[sBase + x];
                var pixel = Palette[dot];

                PrimaryPixels[dBase + x] = pixel;
            }
        }

        if (displayComposition.Forced != LastForced || PlanesDiffer())
        {
        }
    }

    public void Reset()
    {
        Reset(default, true);
    }

    private PgsPixel[] CopyPrimaryPixels()
    {
        var returnValue = new PgsPixel[Size];

        Array.Copy(PrimaryPixels, returnValue, Size);

        return returnValue;
    }

    private void OnNewCaptionReady(Caption caption)
    {
        if (NewCaptionReady is EventHandler<Caption> handler)
            handler(this, caption);
    }

    private bool PlanesDiffer()
    {
        for (uint i = 0; i < Size; i++)
        {
            if (PrimaryPixels[i] != SecondaryPixels[i])
                return false;
        }

        return true;
    }

    private void Reset(PgsTimeStamp timeStamp, bool resetPixels)
    {
        LastTimeStamp = timeStamp;
        LastForced = false;
        Dirty = false;

        if (resetPixels)
        {
            for (uint i = 0; i < Size; i++)
                PrimaryPixels[i] = default;

            UpdateSecondaryPixels();
        }
    }

    private void UpdateSecondaryPixels()
    {
        Array.Copy(PrimaryPixels, SecondaryPixels, Size);
    }
}
