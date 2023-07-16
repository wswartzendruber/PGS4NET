/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Segment;

/// <summary>
///     Represents the common aspects shared by all types of object definition segments.
/// </summary>
public abstract class ObjectDefinitionSegment : Segment
{
    /// <summary>
    ///     The ID of this object, which may be redefined within an epoch. All portions of any
    ///     single object should have the same ID.
    /// </summary>
    public ushort Id;

    /// <summary>
    ///     The version increment of this object. All portions of any single object should have
    ///     the same version.
    /// </summary>
    public byte Version;

    /// <summary>
    ///     The RLE-compressed data for this portion of the completed object.
    /// </summary>
    public byte[] Data = new byte[0];
}
