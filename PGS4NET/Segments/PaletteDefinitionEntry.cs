﻿/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;

namespace PGS4NET.Segments;

/// <summary>
///     Defines a palette entry within a palette set.
/// </summary>
/// <remarks>
///     The role of a palette entry is to define or update exact pixel color, as later
///     referenced by any objects also defined within an epoch.
/// </remarks>
public struct PaletteDefinitionEntry
{
    /// <summary>
    ///     The ID of this palette entry, which should be unique within an epoch.
    /// </summary>
    public byte Id { get; private set; }

    /// <summary>
    ///     Defines the color properties of the palette entry.
    /// </summary>
    public PgsPixel Pixel { get; private set; }

    public PaletteDefinitionEntry(byte id, PgsPixel pixel)
    {
        Id = id;
        Pixel = pixel;
    }
}
