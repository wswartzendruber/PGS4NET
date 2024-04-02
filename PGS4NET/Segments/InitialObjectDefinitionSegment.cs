/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Segments;

/// <summary>
///     Defines an initial object definition segment (I-ODS), the initial portion of an object
///     within an epoch.
/// </summary>
public class InitialObjectDefinitionSegment : ObjectDefinitionSegment
{
    /// <summary>
    ///     The maximum data length this object type can hold.
    /// </summary>
    public const int MaxDataSize = 65_508;

    /// <summary>
    ///     The declared length of this object’s data buffer, including all follow-on portions.
    /// </summary>
    public uint Length { get; set; }

    /// <summary>
    ///     The width of this complete object in pixels, including follow-on portions.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    ///     The height of this complete object in pixels, including follow-on portions.
    /// </summary>
    public ushort Height { get; set; }

    public InitialObjectDefinitionSegment()
    {
    }

    public InitialObjectDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , VersionedId<ushort> versionedId, ushort width, ushort height, uint length
        , byte[] data) : base(pts, dts, versionedId, data)
    {
        Length = length;
        Width = width;
        Height = height;
    }
}
