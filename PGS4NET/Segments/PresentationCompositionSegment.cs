﻿/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;

namespace PGS4NET.Segments;

/// <summary>
///     Defines a presentation composition segment (PCS).
/// </summary>
/// <remarks>
///     A PCS marks the beginning of a new display set (DS).
/// </remarks>
public class PresentationCompositionSegment : Segment
{
    /// <summary>
    ///     The width of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     The height of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     This value should be set to <c>0x10</c> but can otherwise be typically ignored.
    /// </summary>
    public byte FrameRate { get; set; }

    /// <summary>
    ///     Starting at zero, this increments each time graphics are updated within a
    ///     presentation.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    ///     Defines the role of the current DS within the larger epoch.
    /// </summary>
    public CompositionState State { get; set; }

    /// <summary>
    ///     If set, defines that this PCS is responsible for a palette udpate on an existing
    ///     object.
    /// </summary>
    public bool PaletteUpdateOnly { get; set; }

    /// <summary>
    ///     The palette ID to use when rendering objects.
    /// </summary>
    public byte PaletteId { get; set; }

    /// <summary>
    ///     Maps an epoch's objects (or areas within them) to its windows.
    /// </summary>
    public IList<CompositionObject> CompositionObjects { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values and an empty list of composition
    ///     objects.
    /// </summary>
    public PresentationCompositionSegment()
    {
        CompositionObjects = new List<CompositionObject>();
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="presentationTime">
    ///     The time at which a composition is displayed, repeated, updated, or removed. This
    ///     value should be consistent across all segments in a display set.
    /// </param>
    /// <param name="decodeStartTime">
    ///     The time by which the segment needs to be available to the decoder in order to be
    ///     presented in time.
    /// </param>
    /// <param name="width">
    ///     The width of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </param>
    /// <param name="height">
    ///     The height of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </param>
    /// <param name="frameRate">
    ///     This value should be set to <c>0x10</c> but can otherwise be typically ignored.
    /// </param>
    /// <param name="number">
    ///     Starting at zero, this increments each time graphics are updated within a
    ///     presentation.
    /// </param>
    /// <param name="state">
    ///     Defines the role of the current DS within the larger epoch.
    /// </param>
    /// <param name="paletteUpdateOnly">
    ///     If set, defines that this PCS is responsible for a palette udpate on an existing
    ///     object.
    /// </param>
    /// <param name="paletteId">
    ///     The palette ID to use when rendering objects.
    /// </param>
    /// <param name="compositionObjects">
    ///     Maps an epoch's objects (or areas within them) to its windows.
    /// </param>
    public PresentationCompositionSegment(PgsTime presentationTime, PgsTime decodeStartTime,
        int width, int height, byte frameRate, int number, CompositionState state,
        bool paletteUpdateOnly, byte paletteId, IList<CompositionObject> compositionObjects) :
        base(presentationTime, decodeStartTime)
    {
        Width = width;
        Height = height;
        FrameRate = frameRate;
        Number = number;
        State = state;
        PaletteUpdateOnly = paletteUpdateOnly;
        PaletteId = paletteId;
        CompositionObjects = compositionObjects;
    }
}
