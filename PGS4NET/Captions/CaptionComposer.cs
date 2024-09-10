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
    private static readonly CaptionException PaletteUndefinedException
        = new("A display set referenced an undefined palette ID.");
    private static readonly CaptionException ObjectUndefinedException
        = new("A composition object referenced an undefined object ID.");

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

            if (Compositors.TryGetValue(windowId, out Compositor compositor))
            {
                if (compositor.X != displayWindow.X || compositor.Y != displayWindow.Y
                    || compositor.Width != displayWindow.Width
                    || compositor.Height != displayWindow.Height)
                {
                    compositor.Clear(displaySet.Pts);

                    var newCompositor = BuildNewCompositor(displayWindow);

                    Compositors[windowId] = newCompositor;
                }
            }
            else
            {
                var newCompositor = BuildNewCompositor(displayWindow);

                Compositors[windowId] = newCompositor;
            }
        }

        //
        // PALETTE UPDATES
        //

        foreach (var palette in displaySet.Palettes)
            Palettes[palette.Key.Id] = palette.Value;

        if (!Palettes.TryGetValue(displaySet.PaletteId, out var displayPalette))
            throw PaletteUndefinedException;

        //
        // OBJECT UPDATES
        //

        foreach (var object_ in displaySet.Objects)
            Objects[object_.Key.Id] = object_.Value;

        //
        // COMPOSITION AGGREGATION
        //

        var compositions = new Dictionary<byte, Dictionary<ushort, DisplayComposition>>();

        foreach (var displayComposition in displaySet.Compositions)
        {
            var windowId = displayComposition.Key.WindowId;
            var objectId = displayComposition.Key.ObjectId;

            if (!compositions.ContainsKey(windowId))
                compositions[windowId] = new();

            compositions[windowId][objectId] = displayComposition.Value;
        }

        //
        // COMPOSITING
        //

        foreach (var compositionsByWindow in compositions)
        {
            var windowId = compositionsByWindow.Key;

            if (Compositors.TryGetValue(windowId, out var compositor))
            {
                var pairs = new List<Tuple<DisplayObject, DisplayComposition>>();

                foreach (var compositionsByObject in compositionsByWindow.Value)
                {
                    var objectId = compositionsByObject.Key;
                    var composition = compositionsByObject.Value;

                    if (!Objects.TryGetValue(objectId, out var displayObject))
                        throw ObjectUndefinedException;

                    pairs.Add(Tuple.Create(displayObject, composition));
                }

                compositor.Draw(displaySet.Pts, pairs, displayPalette);
            }
            else
            {
                throw new Exception("Internal state error.");
            }
        }
    }

    public void Flush(PgsTimeStamp timeStamp)
    {
        foreach (var compositor in Compositors.Values)
            compositor.Clear(timeStamp);
    }

    public void Reset()
    {
        Compositors.Clear();
        Palettes.Clear();
        Objects.Clear();
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
