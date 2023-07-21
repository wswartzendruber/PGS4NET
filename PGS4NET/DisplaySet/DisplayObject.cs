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
using PGS4NET.Segment;

namespace PGS4NET.DisplaySet;

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
    ///     The line data of this object. Each value refers to a palette entry.
    ///     <c>lines[2][4]</c> would refer to the fifth pixel on the third line.
    /// </summary>
    public byte[] Data = new byte[0];
}
