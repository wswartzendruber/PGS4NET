﻿/*
 * Copyright 2023 William Swartzendruber
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
///     Defines a Presentation Composition Segment (PCS).
/// </summary>
public class PresentationCompositionSegment : Segment
{
    /// <summary>
    ///     The width of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of the screen in pixels. This value should be consistent within a
    ///     presentation.
    /// </summary>
    public ushort Height;

    /// <summary>
    ///     This value should be set to <c>0x10</c> but can otherwise be typically ignored.
    /// </summary>
    public byte FrameRate;

    /// <summary>
    ///     Starting at zero, this increments each time graphics are updated within an epoch.
    /// </summary>
    public ushort Number;

    /// <summary>
    ///     Defines the role of the current DS within the larger epoch.
    /// </summary>
    public CompositionState State;

    /// <summary>
    ///     If set, <see cref="PaletteUpdateId" /> indicates the palette for updating.
    /// </summary>
    public bool PaletteUpdateOnly;

    /// <summary>
    ///     The palette ID to use when rendering the bitmap.
    /// </summary>
    public byte PaletteUpdateId;

    /// <summary>
    ///     Maps an epoch’s objects (or areas within them) to its windows.
    /// </summary>
    public IList<CompositionObject> CompositionObjects = new List<CompositionObject>();
}