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
using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

public class GraphicsPlane
{
    private PgsPixel[] PrimaryPixels;
    private PgsPixel[] SecondaryPixels;

    private PgsTimeStamp LastDrawTime = new PgsTimeStamp(0);

    public event EventHandler<Caption>? NewCaptionReady;

    public ushort X { get; }

    public ushort Y { get; }

    public ushort Width { get; }

    public ushort Height { get; }

    public GraphicsPlane(DisplayWindow displayWindow)
    {
        var size = displayWindow.Width * displayWindow.Height;

        PrimaryPixels = new PgsPixel[size];
        SecondaryPixels = new PgsPixel[size];

        X = displayWindow.X;
        Y = displayWindow.Y;
        Width = displayWindow.Width;
        Height = displayWindow.Height;
    }

    public void Clear(PgsTimeStamp timeStamp)
    {
    }

    public void Draw(PgsTimeStamp timeStamp, DisplayObject displayObject
        , DisplayComposition displayComposition, DisplayPalette displayPalette)
    {
        var x = displayComposition.X - X + (displayComposition.Crop?.X ?? 0);
        var y = displayComposition.Y - Y + (displayComposition.Crop?.Y ?? 0);
    }

    public void Reset()
    {
        var size = PrimaryPixels.Length;

        for (int i = 0; i < size; i++)
        {
            PrimaryPixels[i] = default;
            SecondaryPixels[i] = default;
        }
    }

    protected virtual void OnNewCaptionReady(Caption caption)
    {
        if (NewCaptionReady is EventHandler<Caption> handler)
            handler(this, caption);
    }
}
