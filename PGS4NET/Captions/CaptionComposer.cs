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
using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

public class CaptionComposer
{
    private readonly Dictionary<byte, Compositor> Compositors = new();
    private readonly Dictionary<byte, DisplayPalette> Palettes = new();
    private readonly Dictionary<ushort, DisplayObject> Objects = new();

    public void Input(DisplaySet displaySet)
    {
        if (displaySet.CompositionState == CompositionState.EpochStart)
        {
            Flush(displaySet.Pts);
            Reset();
        }

        //
        // COMPOSITOR/WINDOW UPDATES
        //

        foreach (var window in displaySet.Windows)
        {
            var windowId = window.Key;
            var displayWindow = window.Value;
            Compositor windowCompositor;

            if (Compositors.TryGetValue(windowId, out Compositor compositor))
            {
                if (compositor.X == displayWindow.X && compositor.Y == displayWindow.Y
                    && compositor.Width == displayWindow.Width
                    && compositor.Height == displayWindow.Height)
                {
                    windowCompositor = compositor;
                }
                else
                {
                    compositor.Clear(displaySet.Pts);

                    var newCompositor = BuildNewCompositor(displayWindow);

                    Compositors[windowId] = newCompositor;
                    windowCompositor = newCompositor;
                }
            }
            else
            {
                var newCompositor = BuildNewCompositor(displayWindow);

                Compositors[windowId] = newCompositor;
                windowCompositor = newCompositor;
            }
        }

        //
        // PALETTE UPDATES
        //

        foreach (var palette in displaySet.Palettes)
            Palettes[palette.Key.Id] = palette.Value;

        //
        // OBJECT UPDATES
        //

        foreach (var object_ in displaySet.Objects)
            Objects[object_.Key.Id] = object_.Value;

        //
        // COMPOSITION PROCESSING
        //
    }

    public void Flush(PgsTimeStamp timeStamp)
    {
        foreach (var compositor in Compositors.Values)
            compositor.Clear(timeStamp);
    }

    public void Reset()
    {
        Compositors.Clear();
    }

    protected virtual void OnNewCaption(Caption caption)
    {
        NewCaption?.Invoke(this, caption);
    }

    private Compositor BuildNewCompositor(DisplayWindow displayWindow)
    {
        var newCompositor = new Compositor(displayWindow);

        newCompositor.NewCaption += (_, caption) =>
        {
            OnNewCaption(caption);
        };

        return newCompositor;
    }

    public event EventHandler<Caption>? NewCaption;
}
