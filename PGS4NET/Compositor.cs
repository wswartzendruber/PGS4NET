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
        // CALCULATE POSITIONING
        //

        // Source Area (relative to object)

        int esx = Math.Max(X - displayComposition.X, 0);
        int esy = Math.Max(Y - displayComposition.Y, 0);
        var objectArea = new Area(0, 0, displayObject.Width, displayObject.Height);
        var cropArea = displayComposition.Crop is Crop crop ? new Area(crop) : objectArea;
        var sourceArea_ = GetOverlappingArea(objectArea, cropArea.AddOffset(esx, esy));

        if (sourceArea_ is null)
            return;

        var sourceArea = sourceArea_.Value;
        int soX = sourceArea.X;
        int soY = sourceArea.Y;

        // Destination Area (relative to screen)

        var windowArea = new Area(X, Y, Width, Height);
        var compositionArea = new Area(displayComposition.X, displayComposition.Y, sourceArea.Width, sourceArea.Height);
        var drawArea_ = GetOverlappingArea(windowArea, compositionArea);

        if (drawArea_ is null)
            return;

        var drawArea = drawArea_.Value;
        int doX = drawArea.X - X;
        int doY = drawArea.Y - Y;

        // Common (agnostic)

        int width = Math.Min(sourceArea.Width, drawArea.Width);
        int height = Math.Min(sourceArea.Height, drawArea.Height);

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

        for (int y = 0; y < height; y++)
        {
            int sBase = objectArea.Width * (y + soY) + soX;
            int dBase = windowArea.Width * (y + doY) + doX;

            for (int x = 0; x < height; x++)
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

    private Area? GetOverlappingArea(Area one, Area two)
    {
        int x1s = one.X;
        int x1e = one.X + one.Width;
        int y1s = one.Y;
        int y1e = one.Y + one.Height;
        int x2s = two.X;
        int x2e = two.X + two.Width;
        int y2s = two.Y;
        int y2e = two.Y + two.Height;

        if (x1e <= x2s || x2e <= x1s || y1e <= y2s || y2e <= y1s)
            return null;

        int xos = Math.Max(x1s, x2s);
        int xoe = Math.Min(x1e, x2e);
        int yos = Math.Max(y1s, y2s);
        int yoe = Math.Min(y1e, y2e);

        return new Area(xos, yos, xoe - xos, yoe - yos);
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
            PrimaryPlaneHasSomething = false;

            for (uint i = 0; i < Size; i++)
                PrimaryPixels[i] = default;

            SyncSecondaryPlane();
        }
    }

    private void SyncSecondaryPlane()
    {
        Array.Copy(PrimaryPixels, SecondaryPixels, Size);
    }

    private struct Area
    {
        public int X;

        public int Y;

        public int Width;

        public int Height;

        public Area(Crop crop)
        {
            X = crop.X;
            Y = crop.Y;
            Width = crop.Width;
            Height = crop.Height;
        }

        public Area(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Area AddOffset(int x, int y)
        {
            return new Area(X + x, Y + y, Width, Height);
        }
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
