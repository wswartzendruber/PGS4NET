﻿/*
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
    public VersionedId<byte> VersionedId { get; set; }

    /// <summary>
    ///     Defines the individual palette entries in this set.
    /// </summary>
    public IList<PaletteDefinitionEntry> Entries { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    public PaletteDefinitionSegment()
    {
        Entries = new List<PaletteDefinitionEntry>();
    }

    public PaletteDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , VersionedId<byte> versionedId, IList<PaletteDefinitionEntry> entries) : base(pts, dts)
    {
        VersionedId = versionedId;
        Entries = entries;
    }
}
