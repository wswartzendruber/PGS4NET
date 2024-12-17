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

using CompositionEnumerable = System.Collections.Generic.IEnumerable<
    System.Tuple<
        PGS4NET.DisplaySets.DisplayObject,
        PGS4NET.DisplaySets.DisplayComposition
    >
>;

namespace PGS4NET;

/// <summary>
///     Renders the compositions for a given window onto a two-dimensional plane.
/// </summary>
/// <remarks>
///     New captions, as they are completed, are pushed to subscribers via the
///     <see cref="NewCaption" /> event.
/// </remarks>
public class Compositor
{
    private readonly long Size;
    private readonly YcbcraPixel[] Palette;
    private readonly YcbcraPixel[] PrimaryPixels;
    private readonly YcbcraPixel[] SecondaryPixels;

    private PgsTimeStamp StartTimeStamp = default;
    private bool Forced = false;
    private bool PrimaryPlaneHasSomething = false;

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
    ///     Initializes a new instance for the provided window.
    /// </summary>
    public Compositor(DisplayWindow displayWindow)
    {
        Size = (long)(displayWindow.Width * displayWindow.Height);
        Palette = new YcbcraPixel[256];
        PrimaryPixels = new YcbcraPixel[Size];
        SecondaryPixels = new YcbcraPixel[Size];

        X = displayWindow.X;
        Y = displayWindow.Y;
        Width = displayWindow.Width;
        Height = displayWindow.Height;
    }

    /// <summary>
    ///     Clears the graphics plane. Any content currently on the plane is completed into a
    ///     new caption and pushed via the <see cref="NewCaption" /> event.
    /// </summary>
    /// <param name="timeStamp">
    ///     The time at which the graphics plane should be cleared.
    /// </param>
    public void Clear(PgsTimeStamp timeStamp)
    {
        if (PrimaryPlaneHasSomething)
        {
            var caption = new Caption(StartTimeStamp, timeStamp - StartTimeStamp, X, Y, Width
                , Height, CopyPlane(PrimaryPixels), Forced);

            OnNewCaption(caption);
        }

        Reset(timeStamp, PrimaryPlaneHasSomething);
    }

    /// <summary>
    ///     Draws a single composition onto the graphics plane, causing any completed captions
    ///     to be pushed via the <see cref="NewCaption" /> event.
    /// </summary>
    /// <param name="timeStamp">
    ///     The time at which the composition should be drawn.
    /// </param>
    /// <param name="displayObject">
    ///     The object to be drawn.
    /// </param>
    /// <param name="displayComposition">
    ///     The composition which defines the object's location within the window along with
    ///     whether or not it is forced. The location defined by the composition is relative to
    ///     the screen.
    /// </param>
    /// <param name="displayPalette">
    ///     The palette to use when drawing the object.
    /// </param>
    public void Draw(PgsTimeStamp timeStamp, DisplayObject displayObject
        , DisplayComposition displayComposition, DisplayPalette displayPalette)
    {
        var composition = Tuple.Create(displayObject, displayComposition);
        var compositions = new Tuple<DisplayObject, DisplayComposition>[] { composition };

        Draw(timeStamp, compositions, displayPalette);
    }

    /// <summary>
    ///     Draws one or more compositions onto the graphics plane, causing any completed
    ///     captions to be pushed via the <see cref="NewCaption" /> event.
    /// </summary>
    /// <param name="timeStamp">
    ///     The singular time at which the compositions should be drawn.
    /// </param>
    /// <param name="compositions">
    ///     A collection of object+composition tuples. Each one is drawn to the graphics plane.
    ///     Each composition defines the corresponding object's location within the window along
    ///     with whether or not it is forced. The location defined by each composition is
    ///     relative to the screen. If any compositions are forced then all compositions in that
    ///     invocation become forced.
    /// </param>
    /// <param name="displayPalette">
    ///     The palette to use when drawing the objects.
    /// </param>
    /// <exception cref="CaptionException">
    ///     The PTS value of a display set is less than or equal to the PTS value of the
    ///     previous display set.
    /// </exception>
    public void Draw(PgsTimeStamp timeStamp, CompositionEnumerable compositions
        , DisplayPalette displayPalette)
    {
        var forced = false;

        if (timeStamp <= StartTimeStamp)
        {
            throw new CaptionException(
                "Current time stamp is less than or equal to previous one.");
        }

        //
        // BUILD PALETTE
        //

        byte i = 0;

        do
        {
            var pixel = displayPalette.Entries.TryGetValue(i, out YcbcraPixel entry)
                ? entry
                : default;

            Palette[i] = pixel;
        }
        while (i++ != 255);

        foreach (var composition in compositions)
        {
            var displayObject = composition.Item1;
            var displayComposition = composition.Item2;

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
                    var pixel = Palette[dot];

                    PrimaryPixels[dBase + x] = pixel;
                }
            }

            forced |= displayComposition.Forced;
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

            OnNewCaption(caption);
        }
        else if (PrimaryPlaneHasSomething && state.PrimaryPlaneHasSomething)
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
        PrimaryPlaneHasSomething = state.PrimaryPlaneHasSomething;

        SyncSecondaryPlane();
    }

    /// <summary>
    ///     Completely resets the internal state of this instance. No otherwise completed
    ///     captions are pushed.
    /// </summary>
    public void Reset()
    {
        Reset(default, true);
    }

    private YcbcraPixel[] CopyPlane(YcbcraPixel[] plane)
    {
        var returnValue = new YcbcraPixel[plane.Length];

        Array.Copy(plane, returnValue, returnValue.Length);

        return returnValue;
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

        for (long i = 0; i < Size; i++)
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

            for (long i = 0; i < Size; i++)
                PrimaryPixels[i] = default;

            SyncSecondaryPlane();
        }
    }

    private void SyncSecondaryPlane()
    {
        Array.Copy(PrimaryPixels, SecondaryPixels, Size);
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
        public bool PrimaryPlaneHasSomething;

        public bool PlanesDiffer;

        public CompositorState(bool primaryPlaneHasSomething, bool planesDiffer)
        {
            PrimaryPlaneHasSomething = primaryPlaneHasSomething;
            PlanesDiffer = planesDiffer;
        }
    }
}
