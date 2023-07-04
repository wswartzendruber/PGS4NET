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
        var header = new byte[13];

        if (stream.Read(header, 0, header.Length) != header.Length)
            throw new IOException("EOF while reading segment header.");

        var magicNumber = ReadUInt16BE(header, 0);

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized magic number.");

        var pts = ReadUInt32BE(header, 2);
        var dts = ReadUInt32BE(header, 6);
        var kind = ReadUInt8(header, 10);
        var size = ReadUInt16BE(header, 11);
        var payload = new byte[size];

        if (stream.Read(payload, 0, payload.Length) != payload.Length)
            throw new IOException("EOF while reading segment payload.");

        return kind switch
        {
            0x14 => ParsePDS(payload, pts, dts),
            0x15 => ParseODS(payload, pts, dts),
            0x16 => ParsePCS(payload, pts, dts),
            0x17 => ParseWDS(payload, pts, dts),
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
        var header = new byte[13];

        if (await stream.ReadAsync(header, 0, header.Length) != header.Length)
            throw new IOException("EOF while reading segment header.");

        var magicNumber = ReadUInt16BE(header, 0);

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized magic number.");

        var pts = ReadUInt32BE(header, 2);
        var dts = ReadUInt32BE(header, 6);
        var kind = ReadUInt8(header, 10);
        var size = ReadUInt16BE(header, 11);
        var payload = new byte[size];

        if (await stream.ReadAsync(payload, 0, payload.Length) != payload.Length)
            throw new IOException("EOF while reading segment payload.");

        return kind switch
        {
            0x14 => ParsePDS(payload, pts, dts),
            0x15 => ParseODS(payload, pts, dts),
            0x16 => ParsePCS(payload, pts, dts),
            0x17 => ParseWDS(payload, pts, dts),
            0x80 => new EndSegment { PTS = pts, DTS = dts },
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    private static PresentationCompositionSegment ParsePCS(byte[] buffer, uint pts, uint dts)
    {
        var width = ReadUInt16BE(buffer, 0);
        var height = ReadUInt16BE(buffer, 2);
        var frameRate = ReadUInt8(buffer, 4);
        var compositionNumber = ReadUInt16BE(buffer, 5);
        var parsedCompositionState = ReadUInt8(buffer, 7);
        var compositionState = parsedCompositionState switch
        {
            0x00 => CompositionState.Normal,
            0x40 => CompositionState.AcquisitionPoint,
            0x80 => CompositionState.EpochStart,
            _ => throw new SegmentException("PCS has unrecognized composition state."),
        };
        var parsedPaletteUpdateFlag = ReadUInt8(buffer, 8);
        bool paletteUpdateOnly = parsedPaletteUpdateFlag switch
        {
            0x80 => true,
            0x00 => false,
            _ => throw new SegmentException("PCS has unrecognized palette update flag."),
        };
        var paletteUpdateID = ReadUInt8(buffer, 9);
        var compositionObjectCount = ReadUInt8(buffer, 10);
        var compositionObjects = new List<CompositionObject>();
        var offset = 11;

        for (int i = 0; i < compositionObjectCount; i++)
        {
            var objectID = ReadUInt16BE(buffer, offset);
            var windowID = ReadUInt8(buffer, offset + 2);
            var flags = ReadUInt8(buffer, offset + 3);
            var x = ReadUInt16BE(buffer, offset + 4);
            var y = ReadUInt16BE(buffer, offset + 6);
            var forced = (flags & 0x40) != 0;
            CroppedArea? croppedArea;

            if ((flags & 0x80) != 0)
            {
                croppedArea = new CroppedArea
                {
                    X = ReadUInt16BE(buffer, offset + 8),
                    Y = ReadUInt16BE(buffer, offset + 10),
                    Width = ReadUInt16BE(buffer, offset + 12),
                    Height = ReadUInt16BE(buffer, offset + 14),
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
        var count = ReadUInt8(buffer, 0);
        var offset = 1;

        for (int i = 0; i < count; i++)
        {
            definitions.Add(new WindowDefinition
            {
                ID = ReadUInt8(buffer, offset),
                X = ReadUInt16BE(buffer, offset + 1),
                Y = ReadUInt16BE(buffer, offset + 3),
                Width = ReadUInt16BE(buffer, offset + 5),
                Height = ReadUInt16BE(buffer, offset + 7),
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
        var id = ReadUInt8(buffer, 0);
        var version = ReadUInt8(buffer, 1);
        var entries = new List<PaletteEntry>();
        var offset = 2;

        for (int i = 0; i < count; i++)
        {
            entries.Add(new PaletteEntry
            {
                ID = ReadUInt8(buffer, offset),
                Y = ReadUInt8(buffer, offset + 1),
                Cr = ReadUInt8(buffer, offset + 2),
                Cb = ReadUInt8(buffer, offset + 3),
                Alpha = ReadUInt8(buffer, offset + 4),
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
        var id = ReadUInt16BE(buffer, 0);
        var version = ReadUInt8(buffer, 2);
        var sequenceFlags = ReadUInt8(buffer, 3);

        return sequenceFlags switch
        {
            0xC0 => ParseSODS(buffer, pts, dts, id, version),
            0x80 => ParseIODS(buffer, pts, dts, id, version),
            0x00 => ParseMODS(buffer, pts, dts, id, version),
            0x40 => ParseFODS(buffer, pts, dts, id, version),
            _ => throw new SegmentException("Unrecognized ODS sequence flags."),
        };
    }

    private static SingleObjectDefinitionSegment ParseSODS(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var dataLength = ReadUInt24BE(buffer, 4);

        if (dataLength != buffer.Length - 7)
            throw new SegmentException("Unexpected S-ODS data length.");

        var width = ReadUInt16BE(buffer, 7);
        var height = ReadUInt16BE(buffer, 9);
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
        var dataLength = ReadUInt24BE(buffer, 4);
        var width = ReadUInt16BE(buffer, 7);
        var height = ReadUInt16BE(buffer, 9);
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

    private static byte ReadUInt8(byte[] buffer, int offset)
    {
        return buffer[offset];
    }

    private static ushort ReadUInt16BE(byte[] buffer, int offset)
    {
        return (ushort)
        (
            buffer[offset] << 8
            | buffer[offset + 1]
        );
    }

    private static uint ReadUInt24BE(byte[] buffer, int offset)
    {
        return (uint)
        (
            buffer[offset] << 16
            | buffer[offset + 1] << 8
            | buffer[offset + 2]
        );
    }

    private static uint ReadUInt32BE(byte[] buffer, int offset)
    {
        return (uint)
        (
            buffer[offset] << 24
            | buffer[offset + 1] << 16
            | buffer[offset + 2] << 8
            | buffer[offset + 3]
        );
    }
}
