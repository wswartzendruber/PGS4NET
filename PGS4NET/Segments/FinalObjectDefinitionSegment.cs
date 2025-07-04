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
///     Defines a final object definition segment (F-ODS), the final portion of an object within
///     an epoch.
/// </summary>
public class FinalObjectDefinitionSegment : ObjectDefinitionSegment
{
    /// <summary>
    ///     Initializes a new instance with default values including an empty data buffer.
    /// </summary>
    public FinalObjectDefinitionSegment()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="pts">
    ///     The timestamp indicating when composition decoding should start. In practice, this
    ///     is the time at which the composition is displayed, repeated, modified, or removed.
    ///     All PTS values within a display set should match.
    /// </param>
    /// <param name="dts">
    ///     The timestamp indicating when the composition should be enacted. In practice, this
    ///     value is always zero.
    /// </param>
    /// <param name="versionedId">
    ///     The versioned ID of this object where the version should start at zero and then
    ///     increment by one each time the object is defined differently within a display set.
    /// </param>
    /// <param name="data">
    ///     The RLE-compressed data for this portion of the completed object.
    /// </param>
    public FinalObjectDefinitionSegment(PgsTimeStamp pts, PgsTimeStamp dts,
        VersionedId<int> versionedId, byte[] data) : base(pts, dts, versionedId, data)
    {
    }
}
