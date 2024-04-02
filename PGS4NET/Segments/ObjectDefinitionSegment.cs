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
///     Represents the common aspects shared by all types of object definition segments (ODS).
/// </summary>
public abstract class ObjectDefinitionSegment : Segment
{
    public VersionedId<ushort> VersionedId { get; set; }

    /// <summary>
    ///     The RLE-compressed data for this portion of the completed object.
    /// </summary>
    public byte[] Data { get; set; }

    public ObjectDefinitionSegment()
    {
        Data = new byte[0];
    }

    public ObjectDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , VersionedId<ushort> versionedId, byte[] data) : base(pts, dts)
    {
        VersionedId = versionedId;
        Data = data;
    }
}
