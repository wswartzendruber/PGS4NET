/*
 * Copyright 2024 William Swartzendruber
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
///     A PCS marks the beginning of a new display set.
/// </remarks>
public class PresentationCompositionSegment : Segment
{
    /// <summary>
    ///     The width of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    ///     The height of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    ///     This value should be set to <c>0x10</c> but can otherwise be typically ignored.
    /// </summary>
    public byte FrameRate { get; set; }

    /// <summary>
    ///     Starting at zero, this increments each time graphics are updated within an epoch.
    /// </summary>
    public ushort Number { get; set; }

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

    public PresentationCompositionSegment()
    {
        CompositionObjects = new List<CompositionObject>();
    }

    public PresentationCompositionSegment(PgsTimeStamp pts, PgsTimeStamp dts, ushort width
        , ushort height, byte frameRate, ushort number, CompositionState state
        , bool paletteUpdateOnly, byte paletteId, IList<CompositionObject> compositionObjects)
        : base(pts, dts)
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
