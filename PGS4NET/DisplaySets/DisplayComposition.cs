/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a mapping between an object (or an area of one) and a window within an epoch.
/// </summary>
public class DisplayComposition
{
    /// <summary>
    ///     The horizontal offset of the object's top-left corner relative to the top-left
    ///     corner of the screen. If the object is cropped, then this applies only to the
    ///     visible area.
    /// </summary>
    public ushort X { get; set; }

    /// <summary>
    ///     The vertical offset of the object's top-left corner relative to the top-left corner
    ///     of the screen. If the object is cropped, then this applies only to the visible area.
    /// </summary>
    public ushort Y { get; set; }

    /// <summary>
    ///     Whether or not the composition object is forced. This is typically used to translate
    ///     foreign dialogue or text that appears.
    /// </summary>
    public bool Forced { get; set; }

    /// <summary>
    ///     If set, defines the visible area of the object. Otherwise, the entire object is
    ///     shown.
    /// </summary>
    public Crop? Crop { get; set; }

    public DisplayComposition()
    {
    }

    public DisplayComposition(ushort x, ushort y, bool forced, Crop? crop)
    {
        X = x;
        Y = y;
        Forced = forced;
        Crop = crop;
    }
}
