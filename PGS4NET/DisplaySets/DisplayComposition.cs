/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines a mapping between an object (or an area of one) and a window within an epoch.
/// </summary>
public class DisplayComposition
{
    /// <summary>
    ///     The horizontal offset of the object's top-left corner within the screen. If the
    ///     object is cropped, then this applies only to the visible area.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The vertical offset of the object's top-left corner within the screen. If the
    ///     object is cropped, then this applies only to the visible area.
    /// </summary>
    public int Y { get; set; }

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

    /// <summary>
    ///     Initializes a new instance with default values.
    /// </summary>
    public DisplayComposition()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="x">
    ///     The horizontal offset of the object's top-left corner within the screen. If the
    ///     object is cropped, then this applies only to the visible area.
    /// </param>
    /// <param name="y">
    ///     The vertical offset of the object's top-left corner within the screen. If the
    ///     object is cropped, then this applies only to the visible area.
    /// </param>
    /// <param name="forced">
    ///     Whether or not the composition object is forced. This is typically used to translate
    ///     foreign dialogue or text that appears.
    /// </param>
    /// <param name="crop">
    ///     If set, defines the visible area of the object. Otherwise, the entire object is
    ///     shown.
    /// </param>
    public DisplayComposition(int x, int y, bool forced, Crop? crop)
    {
        X = x;
        Y = y;
        Forced = forced;
        Crop = crop;
    }
}
