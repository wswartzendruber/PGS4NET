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

public class Compositor
{
    private readonly uint Size;
    private readonly PgsPixel[] Palette;
    private readonly PgsPixel[] PrimaryPixels;
    private readonly PgsPixel[] SecondaryPixels;

    private PgsTimeStamp StartTimeStamp = default;
    private bool Forced = false;
    private bool PrimaryPlaneHasSomething = false;

    public event EventHandler<Caption>? NewCaptionReady;

    public ushort X { get; }

    public ushort Y { get; }

    public ushort Width { get; }

    public ushort Height { get; }

    public Compositor(DisplayWindow displayWindow)
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
        if (PrimaryPlaneHasSomething)
        {
            var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y, Width
                , Height, CopyPlane(PrimaryPixels), Forced);

            OnNewCaptionReady(caption);
        }

        Reset(timeStamp, PrimaryPlaneHasSomething);
    }

    public void Draw(PgsTimeStamp timeStamp, DisplayObject displayObject
        , DisplayComposition displayComposition, DisplayPalette displayPalette)
    {
        if (timeStamp <= StartTimeStamp)
        {
            throw new CaptionException(
                "Current time stamp is less than or equal to previous one.");
        }

        //
        // CALCULATE POSITIONS
        //

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

        //
        // BUILD PALETTE
        //

        byte i = 0;

        do
        {
            var pixel = displayPalette.Entries.TryGetValue(i, out PgsPixel entry)
                ? entry
                : default;

            Palette[i] = pixel;
        }
        while (i++ != 255);

        //
        // DRAW
        //

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

        //
        // POST-DRAW HANDLING
        //

        var state = GetCompositorState();

        if (!PrimaryPlaneHasSomething && !state.PrimaryPlaneHasSomething)
        {
            // We had nothing before and we have nothing now.
        }
        else if (!PrimaryPlaneHasSomething && state.PrimaryPlaneHasSomething)
        {
            // We had nothing before but we have something now.

            StartTimeStamp = timeStamp;
        }
        else if (PrimaryPlaneHasSomething && !state.PrimaryPlaneHasSomething)
        {
            // We had something before but we have nothing now.

            var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y, Width
                , Height, CopyPlane(SecondaryPixels), Forced);

            OnNewCaptionReady(caption);
        }
        else if (PrimaryPlaneHasSomething && state.PrimaryPlaneHasSomething)
        {
            // We had something before and we have something now.

            if (state.PlanesDiffer || Forced != displayComposition.Forced)
            {
                var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y
                    , Width, Height, CopyPlane(SecondaryPixels), Forced);

                OnNewCaptionReady(caption);

                StartTimeStamp = timeStamp;
            }
        }
        else
        {
            // We shouldn't ever be here...
            throw new InvalidOperationException("Illegal state.");
        }

        Forced = displayComposition.Forced;
        PrimaryPlaneHasSomething = state.PrimaryPlaneHasSomething;

        SyncSecondaryPlane();
    }

    public void Reset()
    {
        Reset(default, true);
    }

    private PgsPixel[] CopyPlane(PgsPixel[] plane)
    {
        var returnValue = new PgsPixel[plane.Length];

        Array.Copy(plane, returnValue, returnValue.Length);

        return returnValue;
    }

    private void OnNewCaptionReady(Caption caption)
    {
        if (NewCaptionReady is EventHandler<Caption> handler)
            handler(this, caption);
    }

    private CompositorState GetCompositorState()
    {
        bool primaryPlaneHasSomething = false;
        bool planesDiffer = false;

        for (uint i = 0; i < Size; i++)
        {
            if (PrimaryPixels[i] != SecondaryPixels[i])
                planesDiffer = true;

            if (PrimaryPixels[i] != default)
                primaryPlaneHasSomething = true;
        }

        return new CompositorState(primaryPlaneHasSomething, planesDiffer);
    }

    private void Reset(PgsTimeStamp timeStamp, bool resetPixels)
    {
        StartTimeStamp = timeStamp;
        Forced = false;

        if (resetPixels)
        {
            for (uint i = 0; i < Size; i++)
                PrimaryPixels[i] = default;

            SyncSecondaryPlane();
        }
    }

    private void SyncSecondaryPlane()
    {
        Array.Copy(PrimaryPixels, SecondaryPixels, Size);
    }

    private struct CompositorState
    {
        public bool PrimaryPlaneHasSomething;

        public bool PlanesDiffer;

        public CompositorState(bool primaryPlaneHasSomething, bool planesDiffer)
        {
            PrimaryPlaneHasSomething = primaryPlaneHasSomething;
            PlanesDiffer = planesDiffer;
        }
    }
}
