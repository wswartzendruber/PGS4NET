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
///     Defines an object within a display set.
/// </summary>
public class DisplayObject
{
    /// <summary>
    ///     The width of this object in pixels.
    /// </summary>
    public ushort Width;

    /// <summary>
    ///     The height of this object in pixels.
    /// </summary>
    public ushort Height;

    /// <summary>
    ///     An ordered list of object pixel data where each byte addresses a palette entry
    ///     during playback. The length should be the product of the <see cref="Width" />
    ///     and the <see cref="Height" />.
    /// </summary>
    public byte[] Data = new byte[0];
}
