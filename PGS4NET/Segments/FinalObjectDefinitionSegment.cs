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
///     Defines a final object definition segment (F-ODS), the final portion of an object within
///     an epoch.
/// </summary>
public class FinalObjectDefinitionSegment : ObjectDefinitionSegment
{
    public FinalObjectDefinitionSegment()
    {
    }

    public FinalObjectDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts
        , VersionedId<ushort> versionedId, byte[] data) : base(pts, dts, versionedId, data)
    {
    }
}
