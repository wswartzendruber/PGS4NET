/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET;

/// <summary>
///     Defines the visible region within an object.
/// </summary>
public class Crop
{
    /// <summary>
    ///     The horizontal offset of the visible area's top-left corner within the object.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The vertical offset of the visible area's top-left corner within the object.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    ///     The width of the visible area.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     The height of the visible area.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Initializes a new instance with the provided offset and size.
    /// </summary>
    /// <param name="x">
    ///     The horizontal offset of the visible area's top-left corner within the object.
    /// </param>
    /// <param name="y">
    ///     The vertical offset of the visible area's top-left corner within the object.
    /// </param>
    /// <param name="width">
    ///     The width of the visible area.
    /// </param>
    /// <param name="height">
    ///     The height of the visible area.
    /// </param>
    public Crop(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
