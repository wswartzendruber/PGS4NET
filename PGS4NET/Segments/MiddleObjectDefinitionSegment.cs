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
///     Defines a middle object definition segment (M-ODS), a middle portion of an object within
///     an epoch.
/// </summary>
public class MiddleObjectDefinitionSegment : ObjectDefinitionSegment
{
    /// <summary>
    ///     The maximum data length this object type can hold.
    /// </summary>
    public const int MaxDataSize = 65_515;

    /// <summary>
    ///     Initializes a new instance with default values including an empty data buffer.
    /// </summary>
    public MiddleObjectDefinitionSegment()
    {
    }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="presentationTime">
    ///     The time at which a composition is displayed, repeated, updated, or removed. This
    ///     value should be consistent across all segments in a display set.
    /// </param>
    /// <param name="decodeStartTime">
    ///     The time by which the segment needs to be available to the decoder in order to be
    ///     presented in time.
    /// </param>
    /// <param name="versionedId">
    ///     The versioned ID of this object where the version should start at zero and then
    ///     increment by one each time the object is defined differently within a display set.
    /// </param>
    /// <param name="data">
    ///     The RLE-compressed data for this portion of the completed object.
    /// </param>
    public MiddleObjectDefinitionSegment(PgsTime presentationTime, PgsTime decodeStartTime,
        VersionedId<int> versionedId, byte[] data) :
        base(presentationTime, decodeStartTime, versionedId, data)
    {
    }
}
