/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     A display set (DS) is a collection of segments. Multiple DS's come together to form an
///     epoch.
/// </summary>
public class DisplaySet
{
    /// <summary>
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed.
    /// </summary>
    public uint Pts;

    /// <summary>
    ///     The timestamp indicating when the composition should be displayed. In practice, this
    ///     value is always zero.
    /// </summary>
    public uint Dts;

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
    ///     If set, <see cref="PaletteUpdateId" /> indicates the palette for updating.
    /// </summary>
    public bool PaletteUpdateOnly;

    /// <summary>
    ///     The palette ID to use when rendering the bitmap.
    /// </summary>
    public byte PaletteUpdateId;

    /// <summary>
    ///     The collection of windows referenced by this DS.
    /// </summary>
    public IDictionary<byte, DisplayWindow> Windows = new Dictionary<byte, DisplayWindow>();

    /// <summary>
    ///     The collection of palettes referenced by this DS.
    /// </summary>
    public IDictionary<VersionedId<byte>, DisplayPalette> Palettes
        = new Dictionary<VersionedId<byte>, DisplayPalette>();

    /// <summary>
    ///     The collection of objects referenced by this DS.
    /// </summary>
    public IDictionary<VersionedId<ushort>, DisplayObject> DisplayObjects
        = new Dictionary<VersionedId<ushort>, DisplayObject>();

    /// <summary>
    ///     Starting at zero, this increments each time graphics are updated within an epoch.
    /// </summary>
    public ushort CompositionNumber;

    /// <summary>
    ///     Defines the role of this DS within the larger epoch.
    /// </summary>
    public CompositionState CompositionState;

    /// <summary>
    ///     A collection of composition objects, each mapped according to its compound ID
    ///     (object ID + window ID).
    /// </summary>
    public IDictionary<CompositionId, DisplayComposition> Compositions
        = new Dictionary<CompositionId, DisplayComposition>();
}
