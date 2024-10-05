/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Captions;

/// <summary>
///     Represents a graphic with a defined location that appears for a specific amount of time.
/// </summary>
public class Caption
{
    /// <summary>
    ///     The at which the caption appears.
    /// </summary>
    public PgsTimeStamp TimeStamp { get; set; }

    /// <summary>
    ///     The duration for which the caption is visible.
    /// </summary>
    public PgsTimeStamp Duration { get; set; }

    /// <summary>
    ///     The horizontal offset of the object's top-left corner within the screen.
    /// </summary>
    public ushort X { get; set; }

    /// <summary>
    ///     The vertical offset of the object's top-left corner within the screen.
    /// </summary>
    public ushort Y { get; set; }

    /// <summary>
    ///     The width of the caption.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    ///     The height of the caption.
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    ///     An ordered list of object pixel data where each value contains a YCbCr pixel. The
    ///     collection scans horizontally from left to right, top to bottom. The length should
    ///     be the product of the <see cref="Width" /> and the <see cref="Height" />.
    /// </summary>
    public YcbcraPixel[] Data { get; set; }

    /// <summary>
    ///     Whether or not the caption is forced. This is typically used to translate foreign
    ///     dialogue or text that appears.
    /// </summary>
    public bool Forced { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values including an empty data buffer.
    /// </summary>
    public Caption()
    {
        Data = new YcbcraPixel[0];
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="timeStamp">
    ///     The at which the caption appears.
    /// </param>
    /// <param name="duration">
    ///     The duration for which the caption is visible.
    /// </param>
    /// <param name="x">
    ///     The horizontal offset of the object's top-left corner within the screen.
    /// </param>
    /// <param name="y">
    ///     The vertical offset of the object's top-left corner within the screen.
    /// </param>
    /// <param name="width">
    ///     The width of the caption.
    /// </param>
    /// <param name="height">
    ///     The height of the caption.
    /// </param>
    /// <param name="data">
    ///     An ordered list of object pixel data where each value contains a YCbCr pixel. The
    ///     collection scans horizontally from left to right, top to bottom. The length should
    ///     be the product of the <see cref="Width" /> and the <see cref="Height" />.
    /// </param>
    /// <param name="forced">
    ///     Whether or not the caption is forced. This is typically used to translate foreign
    ///     dialogue or text that appears.
    /// </param>
    public Caption(PgsTimeStamp timeStamp, PgsTimeStamp duration, ushort x, ushort y
        , ushort width, ushort height, YcbcraPixel[] data, bool forced)
    {
        TimeStamp = timeStamp;
        Duration = duration;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Data = data;
        Forced = forced;
    }
}
