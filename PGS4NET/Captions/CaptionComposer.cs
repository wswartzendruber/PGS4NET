/*
 * Copyright 2025 William Swartzendruber
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
///     Constructs <see cref="Caption"/>s from <see cref="DisplaySet"/>s.
/// </summary>
public sealed class CaptionComposer
{
    private readonly Dictionary<CompositionId, CompositionArea> CompositionAreas = new();
    private readonly Dictionary<int, DisplayObject> DisplayObjects = new();
    private readonly Dictionary<byte, DisplayPalette> DisplayPalettes = new();
    private readonly Dictionary<byte, DisplayWindow> DisplayWindows = new();

    /// <summary>
    ///     Returns <see langword="true"/> if the composer has any buffered graphics that have
    ///     not been written out as one or more <see cref="Caption"/>s.
    /// </summary>
    /// <remarks>
    ///     Buffered graphics can be forced out via the <see cref="Flush(PgsTime)"/> method, if
    ///     necessary.
    /// </remarks>
    public bool Pending
    {
        get
        {
            foreach (var compositionArea in CompositionAreas.Values)
            {
                if (compositionArea.Pending)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Flushes any graphics which are buffered, causing <see cref="Ready"/> to fire for any
    ///     new <see cref="Caption"/>s that become available as a result.
    /// </summary>
    /// <remarks>
    ///     It should not be necessary to call this method with conformant PGS streams as they
    ///     will automatically cause buffered graphics to be flushed out.
    /// </remarks>
    /// <param name="timeStamp">
    ///     The time at which any buffered graphics should disappear from the screen during
    ///     playback.
    /// </param>
    public void Flush(PgsTime timeStamp)
    {
        foreach (var compositionArea in CompositionAreas.Values)
            compositionArea.Flush(timeStamp);
    }

    /// <summary>
    ///     Inputs a <see cref="DisplaySet"/> into the composer, causing <see cref="Ready"/> to
    ///     fire for any new <see cref="Caption"/>s that become available as a result.
    /// </summary>
    /// <param name="displaySet">
    ///     The display set to input.
    /// </param>
    /// <exception cref="CaptionException">
    ///     The <paramref name="displaySet"/> is not valid given the composer's state.
    /// </exception>
    public void Input(DisplaySet displaySet)
    {
        var timeStamp = displaySet.PresentationTime;

        if (displaySet.CompositionState == CompositionState.EpochStart)
            Reset();

        if (!displaySet.PaletteUpdateOnly)
        {
            foreach (var paletteEntry in displaySet.Palettes)
            {
                var paletteId = paletteEntry.Key.Id;
                var displayPalette = paletteEntry.Value;

                DisplayPalettes[paletteId] = displayPalette;
            }

            foreach (var compositionEntry in displaySet.Compositions)
            {
                var compositionId = compositionEntry.Key;
                var displayComposition = compositionEntry.Value;

                foreach (var objectEntry in displaySet.Objects)
                {
                    var objectId = objectEntry.Key.Id;
                    var displayObject = objectEntry.Value;

                    DisplayObjects[objectId] = displayObject;
                }
            }

            foreach (var windowEntry in displaySet.Windows)
            {
                var windowId = windowEntry.Key;
                var displayWindow = windowEntry.Value;

                foreach (var areaEntry in CompositionAreas)
                {
                    var compositionId = areaEntry.Key;
                    var compositionArea = areaEntry.Value;

                    if (compositionId.WindowId == windowId)
                        compositionArea.Clear();
                }

                foreach (var compositionEntry in displaySet.Compositions)
                {
                    var compositionId = compositionEntry.Key;
                    var displayComposition = compositionEntry.Value;

                    if (compositionId.WindowId == windowId)
                    {
                        var objectId = compositionId.ObjectId;
                        var displayObject = DisplayObjects[objectId];
                        var crop = displayComposition.Crop;

                        if (!CompositionAreas.ContainsKey(compositionId))
                        {
                            var x = displayComposition.X;
                            var y = displayComposition.Y;
                            var width = crop?.Width ?? displayObject.Width;
                            var height = crop?.Height ?? displayObject.Height;
                            var forced = displayComposition.Forced;
                            var newCompositionArea =
                                new CompositionArea(timeStamp, x, y, width,height, forced);

                            newCompositionArea.Ready += CompositionAreaReady;

                            CompositionAreas[compositionId] = newCompositionArea;

                        }

                        var compositionArea = CompositionAreas[compositionId];

                        compositionArea.DrawObject(displayObject, crop);
                    }
                }
            }
        }

        var activePalette = BuildPaletteArray(DisplayPalettes[displaySet.PaletteId]);

        foreach (var compositionArea in CompositionAreas.Values)
            compositionArea.UpdatePalette(timeStamp, activePalette);
    }

    /// <summary>
    ///     Resets the state of the composer.
    /// </summary>
    public void Reset()
    {
        CompositionAreas.Clear();
        DisplayObjects.Clear();
        DisplayPalettes.Clear();
        DisplayWindows.Clear();
    }

    private YcbcraPixel[] BuildPaletteArray(DisplayPalette displayPalette)
    {
        var paletteArray = new YcbcraPixel[256];

        foreach (var entry in displayPalette.Entries)
            paletteArray[entry.Key] = entry.Value;

        paletteArray[255] = default;

        return paletteArray;
    }

    private void CompositionAreaReady(object sender, Caption caption)
    {
        OnReady(caption);
    }

    private void OnReady(Caption caption)
    {
        Ready?.Invoke(this, caption);
    }

    /// <summary>
    ///     Fires when a new <see cref="Caption"/> is ready.
    /// </summary>
    public event EventHandler<Caption>? Ready;
}
