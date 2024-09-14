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
///     Defines palette definition segment (PDS), consisting of multiple palette entries.
/// </summary>
/// <remarks>
///     Palette entries can be broken apart into sets so that they can be modified as a group
///     within an epoch. This can be used, for example, to provide a fade-out effect by
///     continuously updating the palette entries referenced by an object currently on the
///     screen.
/// </remarks>
public class PaletteDefinitionSegment : Segment
{
    /// <summary>
    ///     The versioned ID of this palette.
    /// </summary>
    public VersionedId<byte> VersionedId { get; set; }

    /// <summary>
    ///     The individual palette entries in this set.
    /// </summary>
    public IList<PaletteDefinitionEntry> Entries { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values and an empty list of entries.
    /// </summary>
    public PaletteDefinitionSegment()
    {
        Entries = new List<PaletteDefinitionEntry>();
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="pts">
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed, repeated, modified, or removed.
    ///     All PTS values within a display set should match.
    /// </param>
    /// <param name="dts">
    ///     The timestamp indicating when the composition should be enacted. In practice, this
    ///     value is always zero.
    /// </param>
    /// <param name="versionedId">
    ///     The versioned ID of this palette.
    /// </param>
    /// <param name="entries">
    ///     The individual palette entries in this set.
    /// </param>
    public PaletteDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , VersionedId<byte> versionedId, IList<PaletteDefinitionEntry> entries) : base(pts, dts)
    {
        VersionedId = versionedId;
        Entries = entries;
    }
}
