/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using PGS4NET.Segment;

namespace PGS4NET.DisplaySet;

/// <summary>
///     Defines a palette entry within a palette set.
/// </summary>
/// <remarks>
///     The role of a palette entry is to define or update exact pixel color, as later
///     referenced by any objects also defined within an epoch.
/// </remarks>
public struct PaletteEntry
{
    /// <summary>
    ///     The range-limited, gamma-corrected luminosity value of this entry. Black is
    ///     represented by a value of <c>16</c> while white is represented by a value of
    ///     <c>235</c>. For standard Blu-ray discs, the BT.709 gamma function is typically used.
    ///     However, 4K UltraHD discs seem to use the ST.2084 gamma function instead.
    /// </summary>
    public byte Y;

    /// <summary>
    ///     The vertical position of this entry on the YCbCr color plane, starting from the
    ///     bottom and going up.
    /// </summary>
    public byte Cr;

    /// <summary>
    ///     The horizontal position of this entry on the YCbCr color plane, starting from the
    ///     left and going to the right.
    /// </summary>
    public byte Cb;

    /// <summary>
    ///     The alpha value (transparency ratio) of this entry.
    /// </summary>
    public byte Alpha;
}
