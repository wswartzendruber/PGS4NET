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
            var pixels = CopyPrimaryPixels();
            var caption = new Caption(LastTimeStamp, timeStamp - LastTimeStamp, X, Y, Width
                , Height, pixels, LastForced);

            OnNewCaptionReady(caption);
        }

        Reset(timeStamp, Dirty);
    }

    public void Draw(PgsTimeStamp timeStamp, DisplayObject displayObject
        , DisplayComposition displayComposition, DisplayPalette displayPalette)
    {
        int x, y, width, height;

        if (displayComposition.Crop is Crop crop)
        {
            x = displayComposition.X - X + crop.X;
            y = displayComposition.Y - Y + crop.Y;
            width = crop.Width;
            height = crop.Height;
        }
        else
        {
            x = displayComposition.X - X;
            y = displayComposition.Y - Y;
            width = displayObject.Width;
            height = displayObject.Height;
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
