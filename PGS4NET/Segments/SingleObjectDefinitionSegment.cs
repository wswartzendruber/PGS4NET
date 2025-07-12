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
///     Defines a single object definition segment (S-ODS), a complete object within an epoch.
/// </summary>
public class SingleObjectDefinitionSegment : ObjectDefinitionSegment
{
    /// <summary>
    ///     The width of this object in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     The height of this object in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Initializes a new instance with default values including an empty data buffer.
    /// </summary>
    public SingleObjectDefinitionSegment()
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
    /// <param name="width">
    ///     The width of this complete object in pixels.
    /// </param>
    /// <param name="height">
    ///     The height of this complete object in pixels.
    /// </param>
    /// <param name="data">
    ///     The RLE-compressed data for this portion of the completed object.
    /// </param>
    public SingleObjectDefinitionSegment(PgsTime presentationTime, PgsTime decodeStartTime,
        VersionedId<int> versionedId, int width, int height, byte[] data) :
        base(presentationTime, decodeStartTime, versionedId, data)
    {
        Width = width;
        Height = height;
    }
}
