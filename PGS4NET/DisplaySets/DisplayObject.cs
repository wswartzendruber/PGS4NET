/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.DisplaySets;

/// <summary>
///     Defines an object within a <see cref="DisplaySet"/>.
/// </summary>
public class DisplayObject
{
    /// <summary>
    ///     The width of the object in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     The height of the object in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     An ordered list of object pixel data where each value addresses a key in
    ///     <see cref="DisplayPalette.Entries"/>. The collection scans horizontally from left to
    ///     right, top to bottom. Its length should be the product of the <see cref="Width"/>
    ///     and the <see cref="Height"/>.
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values including an empty data buffer.
    /// </summary>
    public DisplayObject()
    {
        Data = new byte[0];
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="width">
    ///     The width of the object in pixels.
    /// </param>
    /// <param name="height">
    ///     The height of the object in pixels.
    /// </param>
    /// <param name="data">
    ///     An ordered list of object pixel data where each value addresses a key in
    ///     <see cref="DisplayPalette.Entries"/>. The collection scans horizontally from left to
    ///     right, top to bottom. Its length should be the product of the
    ///     <paramref name="width"/> and the <paramref name="height"/>.
    /// </param>
    public DisplayObject(int width, int height, byte[] data)
    {
        Width = width;
        Height = height;
        Data = data;
    }
}
