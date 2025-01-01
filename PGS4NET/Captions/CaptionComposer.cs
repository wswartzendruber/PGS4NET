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

/// <summary>
///     Constructs <see cref="Caption"/>s from PGS <see cref="DisplaySet"/>s.
/// </summary>
public class CaptionComposer
{
    private static readonly CaptionException PaletteUndefinedException
        = new("A display set referenced an undefined palette ID.");
    private static readonly CaptionException ObjectUndefinedException
        = new("A composition object referenced an undefined object ID.");

    private readonly Dictionary<byte, Compositor> Compositors = new();
    private readonly Dictionary<byte, DisplayPalette> Palettes = new();
    private readonly Dictionary<int, DisplayObject> Objects = new();

    /// <summary>
    ///     Returns <see langword="true"/> if the composer has any buffered graphics that have
    ///     not been written out as one or more <see cref="Caption"/>s.
    /// </summary>
    /// <remarks>
    ///     Buffered graphics can be forced out via the <see cref="Flush(PgsTimeStamp)"/>
    ///     method, if necessary.
    /// </remarks>
    public bool Pending
    {
        get
        {
            foreach (var compositor in Compositors.Values)
            {
                if (compositor.Pending)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Flushes any graphics which are buffered, causing <see cref="Ready"/> to fire for any
    ///     new <see cref="Caption"/> that becomes available as a result.
    /// </summary>
    /// <remarks>
    ///     It should not be necessary to call this method with conformant PGS streams as they
    ///     will automatically cause buffered graphics to be flushed out.
    /// </remarks>
    /// <param name="timeStamp">
    ///     The time at which any buffered captions should disappear from the screen during
    ///     playback.
    /// </param>
    public void Flush(PgsTimeStamp timeStamp)
    {
        foreach (var compositor in Compositors.Values)
            compositor.Flush(timeStamp);
    }

    /// <summary>
    ///     Inputs a PGS display set into the composer, causing <see cref="Ready"/> to fire for
    ///     any new <see cref="Caption"/> that becomes available as a result.
    /// </summary>
    /// <param name="displaySet">
    ///     The display set to input.
    /// </param>
    /// <exception cref="CaptionException">
    ///     The <paramref name="displaySet"/> is not valid given the composer's state.
    /// </exception>
    /// <exception cref="CompositorException">
    ///     The internal compositor encountered an error attempting to render a composition.
    /// </exception>
    public void Input(DisplaySet displaySet)
    {
        var timeStamp = displaySet.Pts;

        if (displaySet.CompositionState == CompositionState.EpochStart)
        {
            Flush(timeStamp);
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
                    compositor.Flush(timeStamp);
                    Compositors[windowId] = BuildNewCompositor(displayWindow);
                }
            }
            else
            {
                Compositors[windowId] = BuildNewCompositor(displayWindow);
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

        var allCompositorCompositions = new Dictionary<byte, List<CompositorComposition>>();

        foreach (var composition in displaySet.Compositions)
        {
            var windowId = composition.Key.WindowId;
            var objectId = composition.Key.ObjectId;
            var displayComposition = composition.Value;

            if (!Objects.TryGetValue(objectId, out var displayObject))
                throw ObjectUndefinedException;

            var compositorComposition = new CompositorComposition(displayComposition
                , displayObject, displayPalette);

            if (!allCompositorCompositions.ContainsKey(windowId))
                allCompositorCompositions[windowId] = new();

            allCompositorCompositions[windowId].Add(compositorComposition);
        }

        //
        // COMPOSITING
        //

        foreach (var compositorByWindowId in Compositors)
        {
            var windowId = compositorByWindowId.Key;
            var compositor = compositorByWindowId.Value;

            if (allCompositorCompositions.TryGetValue(windowId, out var compositorCompositions))
                compositor.Draw(timeStamp, compositorCompositions);
            else
                compositor.Flush(timeStamp);
        }
    }

    /// <summary>
    ///     Resets the state of the composer.
    /// </summary>
    public void Reset()
    {
        Compositors.Clear();
        Palettes.Clear();
        Objects.Clear();
    }

    /// <summary>
    ///     Invoked when a new caption is ready.
    /// </summary>
    protected virtual void OnReady(Caption caption)
    {
        Ready?.Invoke(this, caption);
    }

    private Compositor BuildNewCompositor(DisplayWindow displayWindow)
    {
        var newCompositor = new Compositor(displayWindow);

        newCompositor.NewCaption += (_, caption) =>
        {
            OnReady(caption);
        };

        return newCompositor;
    }

    /// <summary>
    ///     Fires when a new caption is ready.
    /// </summary>
    public event EventHandler<Caption>? Ready;
}
