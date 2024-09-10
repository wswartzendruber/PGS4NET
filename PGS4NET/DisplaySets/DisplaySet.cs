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
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

/// <summary>
///     A collection of segments that, when taken together, perform an operation.
/// </summary>
/// <remarks>
///     A display set is principally responsible for performing four distinct functions:
///     <list type="number">
///         <item>
///             <description>
///                 Define and apply a new composition to the screen.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Repeat an existing composition (allows for player seeking).
///             </description>
///         </item>
///         <item>
///             <description>
///                 Modify a composition that is already on the screen.
///             </description>
///         </item>
///         <item>
///             <description>
///                 Remove a composition from the screen.
///             </description>
///         </item>
///     </list>
/// </remarks>
public class DisplaySet
{
    /// <summary>
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed.
    /// </summary>
    public PgsTimeStamp Pts { get; set; }

    /// <summary>
    ///     The timestamp indicating when the composition should be displayed. In practice, this
    ///     value is always zero.
    /// </summary>
    public PgsTimeStamp Dts { get; set; }

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
    ///     If set, defines that this PCS is responsible for a palette udpate on an existing
    ///     object.
    /// </summary>
    public bool PaletteUpdateOnly { get; set; }

    /// <summary>
    ///     The palette ID to use when rendering objects.
    /// </summary>
    public byte PaletteId { get; set; }

    /// <summary>
    ///     The collection of windows referenced by this DS.
    /// </summary>
    public IDictionary<byte, DisplayWindow> Windows { get; set; }

    /// <summary>
    ///     The collection of palettes referenced by this DS.
    /// </summary>
    public IDictionary<VersionedId<byte>, DisplayPalette> Palettes { get; set; }

    /// <summary>
    ///     The collection of objects referenced by this DS.
    /// </summary>
    public IDictionary<VersionedId<ushort>, DisplayObject> Objects { get; set; }

    /// <summary>
    ///     Starting at zero, this increments each time graphics are updated within an epoch.
    /// </summary>
    public ushort CompositionNumber { get; set; }

    /// <summary>
    ///     Defines the role of this DS within the larger epoch.
    /// </summary>
    public CompositionState CompositionState { get; set; }

    /// <summary>
    ///     A collection of composition objects, each mapped according to its compound ID
    ///     (object ID + window ID).
    /// </summary>
    public IDictionary<CompositionId, DisplayComposition> Compositions { get; set; }

    public DisplaySet()
    {
        Windows = new Dictionary<byte, DisplayWindow>();
        Palettes = new Dictionary<VersionedId<byte>, DisplayPalette>();
        Objects = new Dictionary<VersionedId<ushort>, DisplayObject>();
        Compositions = new Dictionary<CompositionId, DisplayComposition>();
    }

    public DisplaySet(PgsTimeStamp pts, PgsTimeStamp dts, ushort width, ushort height
        , byte frameRate, bool paletteUpdateOnly, byte paletteId
        , IDictionary<byte, DisplayWindow> windows
        , IDictionary<VersionedId<byte>, DisplayPalette> palettes
        , IDictionary<VersionedId<ushort>, DisplayObject> objects, ushort compositionNumber
        , CompositionState compositionState
        , IDictionary<CompositionId, DisplayComposition> compositions)
    {
        Pts = pts;
        Dts = dts;
        Width = width;
        Height = height;
        FrameRate = frameRate;
        PaletteUpdateOnly = paletteUpdateOnly;
        PaletteId = paletteId;
        Windows = windows;
        Palettes = palettes;
        Objects = objects;
        CompositionNumber = compositionNumber;
        CompositionState = compositionState;
        Compositions = compositions;
    }
}
