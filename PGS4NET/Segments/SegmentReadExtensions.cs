﻿/*
 * Copyright 2024 William Swartzendruber
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PGS4NET.Segments;

public static partial class SegmentExtensions
{
    public static IList<Segment> ReadAllSegments(this Stream stream)
    {
        var returnValue = new List<Segment>();

        while (stream.ReadSegment() is Segment segment)
            returnValue.Add(segment);

        return returnValue;
    }

    public static async Task<IList<Segment>> ReadAllSegmentsAsync(this Stream stream,
        CancellationToken cancellationToken = default)
    {
        var returnValue = new List<Segment>();

        while (await stream.ReadSegmentAsync(cancellationToken) is Segment segment)
            returnValue.Add(segment);

        return returnValue;
    }

    public static Segment? ReadSegment(this Stream stream)
    {
        var headerBuffer = new byte[13];
        var headerBytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length);

        if (headerBytesRead == 0)
            return null;
        else if (headerBytesRead != headerBuffer.Length)
            throw new IOException("EOF reading segment header.");

        var magicNumber = ReadUInt16Be(headerBuffer, 0)
            ?? throw new InvalidOperationException();

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized segment magic number.");

        var pts = ReadUInt32Be(headerBuffer, 2)
            ?? throw new InvalidOperationException();
        var dts = ReadUInt32Be(headerBuffer, 6)
            ?? throw new InvalidOperationException();
        var kind = ReadUInt8(headerBuffer, 10)
            ?? throw new InvalidOperationException();
        var size = ReadUInt16Be(headerBuffer, 11)
            ?? throw new InvalidOperationException();
        var payloadBuffer = new byte[size];
        var payloadBytesRead = stream.Read(payloadBuffer, 0, payloadBuffer.Length);

        if (payloadBytesRead != payloadBuffer.Length)
            throw new IOException("EOF reading segment payload.");

        return kind switch
        {
            0x14 => ParsePds(payloadBuffer, pts, dts),
            0x15 => ParseOds(payloadBuffer, pts, dts),
            0x16 => ParsePcs(payloadBuffer, pts, dts),
            0x17 => ParseWds(payloadBuffer, pts, dts),
            0x80 => new EndSegment { Pts = pts, Dts = dts },
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    public static async Task<Segment?> ReadSegmentAsync(this Stream stream,
        CancellationToken cancellationToken = default)
    {
        var headerBuffer = new byte[13];
        var headerBytesRead = await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length
            , cancellationToken);

        if (headerBytesRead == 0)
            return null;
        else if (headerBytesRead != headerBuffer.Length)
            throw new IOException("EOF reading segment header.");

        var magicNumber = ReadUInt16Be(headerBuffer, 0)
            ?? throw new InvalidOperationException();

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized segment magic number.");

        var pts = ReadUInt32Be(headerBuffer, 2)
            ?? throw new InvalidOperationException();
        var dts = ReadUInt32Be(headerBuffer, 6)
            ?? throw new InvalidOperationException();
        var kind = ReadUInt8(headerBuffer, 10)
            ?? throw new InvalidOperationException();
        var size = ReadUInt16Be(headerBuffer, 11)
            ?? throw new InvalidOperationException();
        var payloadBuffer = new byte[size];
        var payloadBytesRead = await stream.ReadAsync(payloadBuffer, 0, payloadBuffer.Length
            , cancellationToken);

        if (payloadBytesRead != payloadBuffer.Length)
            throw new IOException("EOF reading segment payload.");

        return kind switch
        {
            0x14 => ParsePds(payloadBuffer, pts, dts),
            0x15 => ParseOds(payloadBuffer, pts, dts),
            0x16 => ParsePcs(payloadBuffer, pts, dts),
            0x17 => ParseWds(payloadBuffer, pts, dts),
            0x80 => new EndSegment { Pts = pts, Dts = dts },
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    private static PresentationCompositionSegment ParsePcs(byte[] buffer, uint pts, uint dts)
    {
        var width = ReadUInt16Be(buffer, 0)
            ?? throw new IOException("EOS reading PCS width.");
        var height = ReadUInt16Be(buffer, 2)
            ?? throw new IOException("EOS reading PCS height.");
        var frameRate = ReadUInt8(buffer, 4)
            ?? throw new IOException("EOS reading PCS frame rate.");
        var compositionNumber = ReadUInt16Be(buffer, 5)
            ?? throw new IOException("EOS reading PCS composition number.");
        var parsedCompositionState = ReadUInt8(buffer, 7)
            ?? throw new IOException("EOS reading PCS composition state.");
        var compositionState = parsedCompositionState switch
        {
            0x00 => CompositionState.Normal,
            0x40 => CompositionState.AcquisitionPoint,
            0x80 => CompositionState.EpochStart,
            _ => throw new SegmentException("PCS has unrecognized composition state."),
        };
        var parsedPaletteUpdateFlag = ReadUInt8(buffer, 8)
            ?? throw new IOException("EOS reading PCS palette update flag.");
        bool paletteUpdateOnly = parsedPaletteUpdateFlag switch
        {
            0x80 => true,
            0x00 => false,
            _ => throw new SegmentException("PCS has unrecognized palette update flag."),
        };
        var paletteUpdateId = ReadUInt8(buffer, 9)
            ?? throw new IOException("EOS reading PCS palette ID.");
        var compositionObjectCount = ReadUInt8(buffer, 10)
            ?? throw new IOException("EOS reading PCS composition object count.");
        var compositionObjects = new List<CompositionObject>();
        var offset = 11;

        for (int i = 0; i < compositionObjectCount; i++)
        {
            var objectId = ReadUInt16Be(buffer, offset)
                ?? throw new IOException($"EOS reading PCS[{i}] object ID.");
            var windowId = ReadUInt8(buffer, offset + 2)
                ?? throw new IOException($"EOS reading PCS[{i}] window ID.");
            var flags = ReadUInt8(buffer, offset + 3)
                ?? throw new IOException($"EOS reading PCS[{i}] flags.");
            var x = ReadUInt16Be(buffer, offset + 4)
                ?? throw new IOException($"EOS reading PCS[{i}] X position.");
            var y = ReadUInt16Be(buffer, offset + 6)
                ?? throw new IOException($"EOS reading PCS[{i}] Y position.");
            var forced = (flags & 0x40) != 0;
            CroppedArea? croppedArea;

            if ((flags & 0x80) != 0)
            {
                croppedArea = new CroppedArea
                {
                    X = ReadUInt16Be(buffer, offset + 8)
                        ?? throw new IOException($"EOS reading PCS[{i}] cropped X position."),
                    Y = ReadUInt16Be(buffer, offset + 10)
                        ?? throw new IOException($"EOS reading PCS[{i}] cropped Y position."),
                    Width = ReadUInt16Be(buffer, offset + 12)
                        ?? throw new IOException($"EOS reading PCS[{i}] cropped width."),
                    Height = ReadUInt16Be(buffer, offset + 14)
                        ?? throw new IOException($"EOS reading PCS[{i}] cropped height."),
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
                ObjectId = objectId,
                WindowId = windowId,
                X = x,
                Y = y,
                Forced = forced,
                Crop = croppedArea,
            });
        }

        return new PresentationCompositionSegment
        {
            Pts = pts,
            Dts = dts,
            Width = width,
            Height = height,
            FrameRate = frameRate,
            Number = compositionNumber,
            State = compositionState,
            PaletteUpdateOnly = paletteUpdateOnly,
            PaletteId = paletteUpdateId,
            CompositionObjects = compositionObjects,
        };
    }

    private static WindowDefinitionSegment ParseWds(byte[] buffer, uint pts, uint dts)
    {
        var definitions = new List<WindowDefinitionEntry>();
        var count = ReadUInt8(buffer, 0)
            ?? throw new IOException("EOS reading WDS count.");
        var offset = 1;

        for (int i = 0; i < count; i++)
        {
            definitions.Add(new WindowDefinitionEntry
            {
                Id = ReadUInt8(buffer, offset)
                    ?? throw new IOException($"EOS reading WDS[{i}] ID."),
                X = ReadUInt16Be(buffer, offset + 1)
                    ?? throw new IOException($"EOS reading WDS[{i}] X position."),
                Y = ReadUInt16Be(buffer, offset + 3)
                    ?? throw new IOException($"EOS reading WDS[{i}] Y position."),
                Width = ReadUInt16Be(buffer, offset + 5)
                    ?? throw new IOException($"EOS reading WDS[{i}] width."),
                Height = ReadUInt16Be(buffer, offset + 7)
                    ?? throw new IOException($"EOS reading WDS[{i}] height."),
            });
            offset += 9;
        }

        return new WindowDefinitionSegment
        {
            Pts = pts,
            Dts = dts,
            Definitions = definitions,
        };
    }

    private static PaletteDefinitionSegment ParsePds(byte[] buffer, uint pts, uint dts)
    {
        var count = (buffer.Length - 2) / 5;
        var paletteId = ReadUInt8(buffer, 0)
            ?? throw new IOException("EOS reading PDS ID.");
        var version = ReadUInt8(buffer, 1)
            ?? throw new IOException("EOS reading PDS version.");
        var entries = new List<PaletteDefinitionEntry>();
        var offset = 2;

        for (int i = 0; i < count; i++)
        {
            var entryId = ReadUInt8(buffer, offset)
                ?? throw new IOException($"EOS reading PDS[{i}] ID.");
            var y = ReadUInt8(buffer, offset + 1)
                ?? throw new IOException($"EOS reading PDS[{i}] Y value.");
            var cr = ReadUInt8(buffer, offset + 2)
                ?? throw new IOException($"EOS reading PDS[{i}] Cr value.");
            var cb = ReadUInt8(buffer, offset + 3)
                ?? throw new IOException($"EOS reading PDS[{i}] Cb value.");
            var alpha = ReadUInt8(buffer, offset + 4)
                ?? throw new IOException($"EOS reading PDS[{i}] alpha value.");

            entries.Add(new PaletteDefinitionEntry
            {
                Id = entryId,
                Pixel = new PgsPixel(y, cr, cb, alpha),
            });
            offset += 5;
        }

        return new PaletteDefinitionSegment
        {
            Pts = pts,
            Dts = dts,
            Id = paletteId,
            Version = version,
            Entries = entries,
        };
    }

    private static ObjectDefinitionSegment ParseOds(byte[] buffer, uint pts, uint dts)
    {
        var id = ReadUInt16Be(buffer, 0)
            ?? throw new IOException("EOS reading ODS ID.");
        var version = ReadUInt8(buffer, 2)
            ?? throw new IOException("EOS reading ODS version.");
        var sequenceFlags = ReadUInt8(buffer, 3)
            ?? throw new IOException("EOS reading ODS sequence flag.");

        return sequenceFlags switch
        {
            0xC0 => ParseSods(buffer, pts, dts, id, version),
            0x80 => ParseIods(buffer, pts, dts, id, version),
            0x00 => ParseMods(buffer, pts, dts, id, version),
            0x40 => ParseFods(buffer, pts, dts, id, version),
            _ => throw new SegmentException("Unrecognized ODS sequence flag."),
        };
    }

    private static SingleObjectDefinitionSegment ParseSods(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var dataLength = ReadUInt24Be(buffer, 4)
            ?? throw new IOException("EOS reading S-ODS data length.");

        if (dataLength != buffer.Length - 7)
            throw new SegmentException("Unexpected S-ODS data length.");

        var width = ReadUInt16Be(buffer, 7)
            ?? throw new IOException("EOS reading S-ODS width.");
        var height = ReadUInt16Be(buffer, 9)
            ?? throw new IOException("EOS reading S-ODS height.");
        var data = new byte[buffer.Length - 11];

        Array.Copy(buffer, 11, data, 0, data.Length);

        return new SingleObjectDefinitionSegment
        {
            Pts = pts,
            Dts = dts,
            Id = id,
            Version = version,
            Data = data,
            Width = width,
            Height = height,
        };
    }

    private static InitialObjectDefinitionSegment ParseIods(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var dataLength = ReadUInt24Be(buffer, 4)
            ?? throw new IOException("EOS reading I-ODS data length.");
        var width = ReadUInt16Be(buffer, 7)
            ?? throw new IOException("EOS reading I-ODS width.");
        var height = ReadUInt16Be(buffer, 9)
            ?? throw new IOException("EOS reading I-ODS height.");
        var data = new byte[buffer.Length - 11];

        Array.Copy(buffer, 11, data, 0, data.Length);

        return new InitialObjectDefinitionSegment
        {
            Pts = pts,
            Dts = dts,
            Id = id,
            Version = version,
            Data = data,
            Length = dataLength,
            Width = width,
            Height = height,
        };
    }

    private static MiddleObjectDefinitionSegment ParseMods(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var data = new byte[buffer.Length - 4];

        Array.Copy(buffer, 4, data, 0, data.Length);

        return new MiddleObjectDefinitionSegment
        {
            Pts = pts,
            Dts = dts,
            Id = id,
            Version = version,
            Data = data,
        };
    }

    private static FinalObjectDefinitionSegment ParseFods(byte[] buffer, uint pts, uint dts
        , ushort id, byte version)
    {
        var data = new byte[buffer.Length - 4];

        Array.Copy(buffer, 4, data, 0, data.Length);

        return new FinalObjectDefinitionSegment
        {
            Pts = pts,
            Dts = dts,
            Id = id,
            Version = version,
            Data = data,
        };
    }

    private static byte? ReadUInt8(byte[] buffer, int offset) =>
        (offset + 1 <= buffer.Length)
            ? buffer[offset]
            : null;

    private static ushort? ReadUInt16Be(byte[] buffer, int offset) =>
        (offset + 2 <= buffer.Length)
            ? (ushort)(buffer[offset] << 8 | buffer[offset + 1])
            : null;

    private static uint? ReadUInt24Be(byte[] buffer, int offset) =>
        (offset + 3 <= buffer.Length)
            ? (uint)(buffer[offset] << 16 | buffer[offset + 1] << 8 | buffer[offset + 2])
            : null;

    private static uint? ReadUInt32Be(byte[] buffer, int offset) =>
        (offset + 4 <= buffer.Length)
            ? (uint)(buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8
                | buffer[offset + 3])
            : null;
}
