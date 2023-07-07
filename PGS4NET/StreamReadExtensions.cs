/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PGS4NET;

/// <summary>
///     Contains extensions against <see cref="Stream" /> for reading PGS segments.
/// </summary>
public static partial class StreamExtensions
{
    /// <summary>
    ///     Reads the next PGS segment from a <see cref="Stream" />.
    /// </summary>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment.
    /// </exception>
    public static Segment ReadSegment(this Stream stream)
    {
        var headerBuffer = new byte[13];

        stream.Read(headerBuffer, 0, headerBuffer.Length);

        var magicNumber = ReadUInt16BE(headerBuffer, 0)
            ?? throw new IOException("EOF reading segment magic number.");

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized segment magic number.");

        var pts = ReadUInt32BE(headerBuffer, 2)
            ?? throw new IOException("EOF reading segment PTS.");
        var dts = ReadUInt32BE(headerBuffer, 6)
            ?? throw new IOException("EOF reading segment DTS.");
        var kind = ReadUInt8(headerBuffer, 10)
            ?? throw new IOException("EOF reading segment kind.");
        var size = ReadUInt16BE(headerBuffer, 11)
            ?? throw new IOException("EOF reading segment size.");
        var payloadBuffer = new byte[size];

        stream.Read(payloadBuffer, 0, payloadBuffer.Length);

        return kind switch
        {
            0x14 => ParsePDS(payloadBuffer, pts, dts),
            0x15 => ParseODS(payloadBuffer, pts, dts),
            0x16 => ParsePCS(payloadBuffer, pts, dts),
            0x17 => ParseWDS(payloadBuffer, pts, dts),
            0x80 => new EndSegment { PTS = pts, DTS = dts },
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    /// <summary>
    ///     Asynchronously reads the next PGS segment from a <see cref="Stream" />.
    /// </summary>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to read a segment.
    /// </exception>
    public static async Task<Segment> ReadSegmentAsync(this Stream stream)
    {
        var headerBuffer = new byte[13];

        await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);

        var magicNumber = ReadUInt16BE(headerBuffer, 0)
            ?? throw new IOException("EOF reading segment magic number.");

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized segment magic number.");

        var pts = ReadUInt32BE(headerBuffer, 2)
            ?? throw new IOException("EOF reading segment PTS.");
        var dts = ReadUInt32BE(headerBuffer, 6)
            ?? throw new IOException("EOF reading segment DTS.");
        var kind = ReadUInt8(headerBuffer, 10)
            ?? throw new IOException("EOF reading segment kind.");
        var size = ReadUInt16BE(headerBuffer, 11)
            ?? throw new IOException("EOF reading segment size.");
        var payloadBuffer = new byte[size];

        await stream.ReadAsync(payloadBuffer, 0, payloadBuffer.Length);

        return kind switch
        {
            0x14 => ParsePDS(payloadBuffer, pts, dts),
            0x15 => ParseODS(payloadBuffer, pts, dts),
            0x16 => ParsePCS(payloadBuffer, pts, dts),
            0x17 => ParseWDS(payloadBuffer, pts, dts),
            0x80 => new EndSegment { PTS = pts, DTS = dts },
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    private static PresentationCompositionSegment ParsePCS(byte[] buffer, uint pts, uint dts)
    {
        var width = ReadUInt16BE(buffer, 0)
            ?? throw new IOException("EOF reading PCS width.");
        var height = ReadUInt16BE(buffer, 2)
            ?? throw new IOException("EOF reading PCS height.");
        var frameRate = ReadUInt8(buffer, 4)
            ?? throw new IOException("EOF reading PCS frame rate.");
        var compositionNumber = ReadUInt16BE(buffer, 5)
            ?? throw new IOException("EOF reading PCS composition number.");
        var parsedCompositionState = ReadUInt8(buffer, 7)
            ?? throw new IOException("EOF reading PCS composition state.");
        var compositionState = parsedCompositionState switch
        {
            0x00 => CompositionState.Normal,
            0x40 => CompositionState.AcquisitionPoint,
            0x80 => CompositionState.EpochStart,
            _ => throw new SegmentException("PCS has unrecognized composition state."),
        };
        var parsedPaletteUpdateFlag = ReadUInt8(buffer, 8)
            ?? throw new IOException("EOF reading PCS palette update flag.");
        bool paletteUpdateOnly = parsedPaletteUpdateFlag switch
        {
            0x80 => true,
            0x00 => false,
            _ => throw new SegmentException("PCS has unrecognized palette update flag."),
        };
        var paletteUpdateID = ReadUInt8(buffer, 9)
            ?? throw new IOException("EOF reading PCS palette update ID.");
        var compositionObjectCount = ReadUInt8(buffer, 10)
            ?? throw new IOException("EOF reading PCS composition object count.");
        var compositionObjects = new List<CompositionObject>();
        var offset = 11;

        for (int i = 0; i < compositionObjectCount; i++)
        {
            var objectID = ReadUInt16BE(buffer, offset)
                ?? throw new IOException($"EOF reading PCS[{i}] object ID.");
            var windowID = ReadUInt8(buffer, offset + 2)
                ?? throw new IOException($"EOF reading PCS[{i}] window ID.");
            var flags = ReadUInt8(buffer, offset + 3)
                ?? throw new IOException($"EOF reading PCS[{i}] flags.");
            var x = ReadUInt16BE(buffer, offset + 4)
                ?? throw new IOException($"EOF reading PCS[{i}] X position.");
            var y = ReadUInt16BE(buffer, offset + 6)
                ?? throw new IOException($"EOF reading PCS[{i}] Y position.");
            var forced = (flags & 0x40) != 0;
            CroppedArea? croppedArea;

            if ((flags & 0x80) != 0)
            {
                croppedArea = new CroppedArea
                {
                    X = ReadUInt16BE(buffer, offset + 8)
                        ?? throw new IOException($"EOF reading PCS[{i}] cropped X position."),
                    Y = ReadUInt16BE(buffer, offset + 10)
                        ?? throw new IOException($"EOF reading PCS[{i}] cropped Y position."),
                    Width = ReadUInt16BE(buffer, offset + 12)
                        ?? throw new IOException($"EOF reading PCS[{i}] cropped width."),
                    Height = ReadUInt16BE(buffer, offset + 14)
                        ?? throw new IOException($"EOF reading PCS[{i}] cropped height."),
                };
                offset += 16;
            }
            else
            {
                croppedArea = null;
                offset += 8;
            }

            compositionObjects.Add(new CompositionObject
            {
                ObjectID = objectID,
                WindowID = windowID,
                X = x,
                Y = y,
                Forced = forced,
                Crop = croppedArea,
            });
        }

        return new PresentationCompositionSegment
        {
            PTS = pts,
            DTS = dts,
            Width = width,
            Height = height,
            FrameRate = frameRate,
            Number = compositionNumber,
            State = compositionState,
            PaletteUpdateOnly = paletteUpdateOnly,
            PaletteUpdateID = paletteUpdateID,
            Objects = compositionObjects,
        };
    }

    private static WindowDefinitionSegment ParseWDS(byte[] buffer, uint pts, uint dts)
    {
        var definitions = new List<WindowDefinition>();
        var count = ReadUInt8(buffer, 0)
            ?? throw new IOException("EOF reading WDS count.");
        var offset = 1;

        for (int i = 0; i < count; i++)
        {
            definitions.Add(new WindowDefinition
            {
                ID = ReadUInt8(buffer, offset)
                    ?? throw new IOException($"EOF reading WDS[{i}] ID."),
                X = ReadUInt16BE(buffer, offset + 1)
                    ?? throw new IOException($"EOF reading WDS[{i}] X position."),
                Y = ReadUInt16BE(buffer, offset + 3)
                    ?? throw new IOException($"EOF reading WDS[{i}] Y position."),
                Width = ReadUInt16BE(buffer, offset + 5)
                    ?? throw new IOException($"EOF reading WDS[{i}] width."),
                Height = ReadUInt16BE(buffer, offset + 7)
                    ?? throw new IOException($"EOF reading WDS[{i}] height."),
            });
            offset += 9;
        }

        return new WindowDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            Definitions = definitions,
        };
    }

    private static PaletteDefinitionSegment ParsePDS(byte[] buffer, uint pts, uint dts)
    {
        var count = (buffer.Length - 2) / 5;
        var id = ReadUInt8(buffer, 0)
            ?? throw new IOException("EOF reading PDS ID.");
        var version = ReadUInt8(buffer, 1)
            ?? throw new IOException("EOF reading PDS version.");
        var entries = new List<PaletteEntry>();
        var offset = 2;

        for (int i = 0; i < count; i++)
        {
            entries.Add(new PaletteEntry
            {
                ID = ReadUInt8(buffer, offset)
                    ?? throw new IOException($"EOF reading PDS[{i}] ID."),
                Y = ReadUInt8(buffer, offset + 1)
                    ?? throw new IOException($"EOF reading PDS[{i}] Y value."),
                Cr = ReadUInt8(buffer, offset + 2)
                    ?? throw new IOException($"EOF reading PDS[{i}] Cr value."),
                Cb = ReadUInt8(buffer, offset + 3)
                    ?? throw new IOException($"EOF reading PDS[{i}] Cb value."),
                Alpha = ReadUInt8(buffer, offset + 4)
                    ?? throw new IOException($"EOF reading PDS[{i}] alpha value."),
            });
            offset += 5;
        }

        return new PaletteDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Entries = entries,
        };
    }

    private static ObjectDefinitionSegment ParseODS(byte[] buffer, uint pts, uint dts)
    {
        var id = ReadUInt16BE(buffer, 0)
            ?? throw new IOException("EOF reading ODS ID.");
        var version = ReadUInt8(buffer, 2)
            ?? throw new IOException("EOF reading ODS version.");
        var sequenceFlags = ReadUInt8(buffer, 3)
            ?? throw new IOException("EOF reading ODS sequence flag.");

        return sequenceFlags switch
        {
            0xC0 => ParseSODS(buffer, pts, dts, id, version),
            0x80 => ParseIODS(buffer, pts, dts, id, version),
            0x00 => ParseMODS(buffer, pts, dts, id, version),
            0x40 => ParseFODS(buffer, pts, dts, id, version),
            _ => throw new SegmentException("Unrecognized ODS sequence flag."),
        };
    }

    private static SingleObjectDefinitionSegment ParseSODS(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var dataLength = ReadUInt24BE(buffer, 4)
            ?? throw new IOException("EOF reading S-ODS data length.");

        if (dataLength != buffer.Length - 7)
            throw new SegmentException("Unexpected S-ODS data length.");

        var width = ReadUInt16BE(buffer, 7)
            ?? throw new IOException("EOF reading S-ODS width.");
        var height = ReadUInt16BE(buffer, 9)
            ?? throw new IOException("EOF reading S-ODS height.");
        var data = new byte[buffer.Length - 11];

        Array.Copy(buffer, 11, data, 0, data.Length);

        return new SingleObjectDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Data = data,
            Width = width,
            Height = height,
        };
    }

    private static InitialObjectDefinitionSegment ParseIODS(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var dataLength = ReadUInt24BE(buffer, 4)
            ?? throw new IOException("EOF reading I-ODS data length.");
        var width = ReadUInt16BE(buffer, 7)
            ?? throw new IOException("EOF reading I-ODS width.");
        var height = ReadUInt16BE(buffer, 9)
            ?? throw new IOException("EOF reading I-ODS height.");
        var data = new byte[buffer.Length - 11];

        Array.Copy(buffer, 11, data, 0, data.Length);

        return new InitialObjectDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Data = data,
            Length = dataLength,
            Width = width,
            Height = height,
        };
    }

    private static MiddleObjectDefinitionSegment ParseMODS(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var data = new byte[buffer.Length - 4];

        Array.Copy(buffer, 4, data, 0, data.Length);

        return new MiddleObjectDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Data = data,
        };
    }

    private static FinalObjectDefinitionSegment ParseFODS(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var data = new byte[buffer.Length - 4];

        Array.Copy(buffer, 4, data, 0, data.Length);

        return new FinalObjectDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Data = data,
        };
    }

    private static byte? ReadUInt8(byte[] buffer, int offset) =>
        (offset + 1 <= buffer.Length)
            ? buffer[offset]
            : null;

    private static ushort? ReadUInt16BE(byte[] buffer, int offset) =>
        (offset + 2 <= buffer.Length)
            ? (ushort)(buffer[offset] << 8 | buffer[offset + 1])
            : null;

    private static uint? ReadUInt24BE(byte[] buffer, int offset) =>
        (offset + 3 <= buffer.Length)
            ? (uint)(buffer[offset] << 16 | buffer[offset + 1] << 8 | buffer[offset + 2])
            : null;

    private static uint? ReadUInt32BE(byte[] buffer, int offset) =>
        (offset + 4 <= buffer.Length)
            ? (uint)(buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8
                | buffer[offset + 3])
            : null;
}
