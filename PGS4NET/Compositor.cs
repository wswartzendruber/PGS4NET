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
using PGS4NET.Captions;
using PGS4NET.DisplaySets;

namespace PGS4NET;

/// <summary>
///     Renders the compositions for a given window onto a two-dimensional plane.
/// </summary>
/// <remarks>
///     New captions, as they are completed, are pushed to subscribers via the
///     <see cref="NewCaption"/> event.
/// </remarks>
public class Compositor
{
    private readonly long Size;
    private readonly YcbcraPixel[] PrimaryPixels;
    private readonly YcbcraPixel[] SecondaryPixels;
    private readonly YcbcraPixel[] ClearPixels;

    private bool Forced = false;
    private PgsTimeStamp StartTimeStamp = default;
    private CompositorState LastCompositorState = new(false, false, false);

    /// <summary>
    ///     The horizontal offset of the window's top-left corner within the screen.
    /// </summary>
    public int X { get; }

    /// <summary>
    ///     The vertical offset of the window's top-left corner within the screen.
    /// </summary>
    public int Y { get; }

    /// <summary>
    ///     The width of the window.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     The height of the window.
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Gets whether or not the graphics plane has anything drawn to it.
    /// </summary>
    public bool Pending => LastCompositorState.PrimaryPlaneDirty;

    /// <summary>
    ///     Initializes a new instance for the provided window.
    /// </summary>
    public Compositor(DisplayWindow displayWindow)
    {
        Size = displayWindow.Width * displayWindow.Height;
        PrimaryPixels = new YcbcraPixel[Size];
        SecondaryPixels = new YcbcraPixel[Size];
        ClearPixels = new YcbcraPixel[Size];

        X = displayWindow.X;
        Y = displayWindow.Y;
        Width = displayWindow.Width;
        Height = displayWindow.Height;
    }

    /// <summary>
    ///     Draws a single composition onto the graphics plane, causing any completed captions
    ///     to be pushed via the <see cref="NewCaption"/> event.
    /// </summary>
    /// <param name="timeStamp">
    ///     The time at which the composition should be drawn.
    /// </param>
    /// <param name="composition">
    ///     The composition operation to draw.
    /// </param>
    public void Draw(PgsTimeStamp timeStamp, CompositorComposition composition)
    {
        var compositions = new[] { composition };

        Draw(timeStamp, compositions);
    }

    /// <summary>
    ///     Draws one or more compositions onto the graphics plane, causing any completed
    ///     captions to be pushed via the <see cref="NewCaption"/> event.
    /// </summary>
    /// <param name="timeStamp">
    ///     The singular time at which the compositions should be drawn.
    /// </param>
    /// <param name="compositions">
    ///     A collection of operations to draw. If any operations are forced (via the
    ///     <see cref="CompositorComposition.DisplayComposition"/> property), then all
    ///     compositions in the collection become forced.
    /// </param>
    /// <exception cref="CaptionException">
    ///     The PTS value of a display set is less than or equal to the PTS value of the
    ///     previous display set.
    /// </exception>
    public void Draw(PgsTimeStamp timeStamp, IEnumerable<CompositorComposition> compositions)
    {
        var forced = false;

        if (timeStamp <= StartTimeStamp)
        {
            throw new CaptionException(
                "Current time stamp is less than or equal to previous one.");
        }

        //
        // CLEAR PRIMARY PLANE
        //

        Array.Copy(ClearPixels, PrimaryPixels, Size);

        //
        // DRAW NEW/CURRENT COMPOSITIONS
        //

        foreach (var composition in compositions)
        {
            var displayObject = composition.DisplayObject;
            var displayComposition = composition.DisplayComposition;
            var displayPalette = composition.DisplayPalette;
            var generatedPalette = new YcbcraPixel[256];

            //
            // GENERATE PALETTE
            //

            byte i = 0;

            do
            {
                var pixel = displayPalette.Entries.TryGetValue(i, out YcbcraPixel entry)
                    ? entry
                    : default;

                generatedPalette[i] = pixel;
            }
            while (i++ != 255);

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
            var compositionArea = new Area(displayComposition.X, displayComposition.Y
                , sourceArea.Width, sourceArea.Height);
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
            // DRAW
            //

            for (int y = 0; y < height; y++)
            {
                int sBase = objectArea.Width * (y + soY) + soX;
                int dBase = windowArea.Width * (y + doY) + doX;

                for (int x = 0; x < width; x++)
                {
                    var dot = displayObject.Data[sBase + x];
                    var pixel = generatedPalette[dot];

                    PrimaryPixels[dBase + x] = pixel;
                }
            }

            forced |= displayComposition.Forced;
        }

        //
        // POST-DRAW HANDLING
        //

        var state = GetCompositorState();

        if (!state.PrimaryPlaneDirty && !state.SecondaryPlaneDirty)
        {
            // We had nothing before and we have nothing now.
        }
        else if (state.PrimaryPlaneDirty && !state.SecondaryPlaneDirty)
        {
            // We had nothing before but we have something now.

            StartTimeStamp = timeStamp;
        }
        else if (!state.PrimaryPlaneDirty && state.SecondaryPlaneDirty)
        {
            // We had something before but we have nothing now.

            var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y, Width
                , Height, CopyPlane(SecondaryPixels), Forced);

            OnNewCaption(caption);
        }
        else if (state.PrimaryPlaneDirty && state.SecondaryPlaneDirty)
        {
            // We had something before and we have something now.

            if (state.PlanesDiffer || Forced != forced)
            {
                var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y
                    , Width, Height, CopyPlane(SecondaryPixels), Forced);

                OnNewCaption(caption);

                StartTimeStamp = timeStamp;
            }
        }
        else
        {
            // We shouldn't ever be here...
            throw new InvalidOperationException("Illegal state.");
        }

        Forced = forced;
        LastCompositorState = state;

        Array.Copy(PrimaryPixels, SecondaryPixels, Size);
    }

    /// <summary>
    ///     Flushes the graphics plane. Any content currently on the plane is completed into a
    ///     new caption and pushed via the <see cref="NewCaption"/> event.
    /// </summary>
    /// <param name="timeStamp">
    ///     The time at which any remaining graphics should be flushed out.
    /// </param>
    public void Flush(PgsTimeStamp timeStamp)
    {
        if (LastCompositorState.PrimaryPlaneDirty)
        {
            var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y, Width
                , Height, CopyPlane(PrimaryPixels), Forced);

            OnNewCaption(caption);
        }

        Reset();
    }

    /// <summary>
    ///     Completely resets the internal state of this instance. No otherwise completed
    ///     captions are flushed.
    /// </summary>
    public void Reset()
    {
        Forced = false;
        StartTimeStamp = default;
        LastCompositorState = new(false, false, false);

        Array.Copy(ClearPixels, PrimaryPixels, Size);
        Array.Copy(ClearPixels, SecondaryPixels, Size);
    }

    /// <summary>
    ///     Called when a new caption becomes available.
    /// </summary>
    /// <param name="caption">
    ///     The newly completed caption that is available.
    /// </param>
    protected virtual void OnNewCaption(Caption caption)
    {
        NewCaption?.Invoke(this, caption);
    }

    private YcbcraPixel[] CopyPlane(YcbcraPixel[] plane)
    {
        var returnValue = new YcbcraPixel[plane.Length];

        Array.Copy(plane, returnValue, returnValue.Length);

        return returnValue;
    }

    private CompositorState GetCompositorState()
    {
        bool primaryPlaneDirty = false;
        bool secondaryPlaneDirty = false;
        bool planesDiffer = false;

        for (long i = 0; i < Size; i++)
        {
            primaryPlaneDirty |= PrimaryPixels[i] != default;
            secondaryPlaneDirty |= SecondaryPixels[i] != default;
            planesDiffer |= PrimaryPixels[i] != SecondaryPixels[i];
        }

        return new CompositorState(primaryPlaneDirty, secondaryPlaneDirty, planesDiffer);
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

    /// <summary>
    ///     Triggered when a new caption becomes available.
    /// </summary>
    public event EventHandler<Caption>? NewCaption;

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
        internal readonly bool PrimaryPlaneDirty;
        internal readonly bool SecondaryPlaneDirty;
        internal readonly bool PlanesDiffer;

        public CompositorState(bool primaryPlaneDirty, bool secondaryPlaneDirty
            , bool planesDiffer)
        {
            PrimaryPlaneDirty = primaryPlaneDirty;
            SecondaryPlaneDirty = secondaryPlaneDirty;
            PlanesDiffer = planesDiffer;
        }
    }
}
