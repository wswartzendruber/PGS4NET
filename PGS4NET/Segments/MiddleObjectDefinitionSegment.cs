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
///     Defines a middle object definition segment (M-ODS), a middle portion of an object within
///     an epoch.
/// </summary>
public class MiddleObjectDefinitionSegment : ObjectDefinitionSegment
{
    /// <summary>
    ///     The maximum data length this object type can hold.
    /// </summary>
    public const int MaxDataSize = 65_515;

    public MiddleObjectDefinitionSegment()
    {
    }

    public MiddleObjectDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , VersionedId<ushort> versionedId, byte[] data) : base(pts, dts, versionedId, data)
    {
    }
}
