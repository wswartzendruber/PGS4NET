/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Segments;

/// <summary>
///     Defines a mapping between an object (or an area of one) and a window within an epoch.
/// </summary>
public class CompositionObject
{
    /// <summary>
    ///     The ID of the composition object.
    /// </summary>
    public CompositionId Id { get; set; }

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
    public CompositionObject()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="id">
    ///     The ID of the composition object.
    /// </param>
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
    public CompositionObject(CompositionId id, int x, int y, bool forced, Crop? crop)
    {
        Id = id;
        X = x;
        Y = y;
        Forced = forced;
        Crop = crop;
    }
}
