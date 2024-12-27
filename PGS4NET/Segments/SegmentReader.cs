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
using System.Threading;
using System.Threading.Tasks;

namespace PGS4NET.Segments;

/// <summary>
///     Reads PGS <see cref="Segment"/>s from an input <see cref="Stream"/>.
/// </summary>
#if NETSTANDARD2_1_OR_GREATER
public class SegmentReader : IDisposable, IAsyncDisposable
#else
public class SegmentReader : IDisposable
#endif
{
    /// <summary>
    ///     The stream that segments are being read from.
    /// </summary>
    public Stream Input { get; }

    /// <summary>
    ///     Whether or not the <see cref="Input"/> will be left open once all segments have been
    ///     read.
    /// </summary>
    public bool LeaveOpen { get; }

    /// <summary>
    ///     Initializes a new instance.
    /// </summary>
    /// <param name="input">
    ///     The stream that segments will be read from.
    /// </param>
    /// <param name="leaveOpen">
    ///     Whether or not the <paramref name="input"/> stream will be left open once all
    ///     segments have been read.
    /// </param>
    public SegmentReader(Stream input, bool leaveOpen = false)
    {
        Input = input;
        LeaveOpen = leaveOpen;
    }

    /// <summary>
    ///     Attempts to read the next PGS segment from the <see cref="Input"/> stream.
    /// </summary>
    /// <remarks>
    ///     The current position of the <see cref="Input"/> stream must be at the beginning of a
    ///     a segment.
    /// </remarks>
    /// <returns>
    ///     The PGS segment that was read or <see langword="null"/> if the <see cref="Input"/>
    ///     stream is already EOF.
    /// </returns>
    /// <exception cref="SegmentException">
    ///     An encoded value within the segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read the segment.
    /// </exception>
    public Segment? Read()
    {
        var headerBuffer = new byte[13];
        var headerBytesRead = Input.Read(headerBuffer, 0, headerBuffer.Length);

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
        var payloadBytesRead = Input.Read(payloadBuffer, 0, payloadBuffer.Length);

        if (payloadBytesRead != payloadBuffer.Length)
            throw new IOException("EOF reading segment payload.");

        return kind switch
        {
            0x14 => ParsePds(payloadBuffer, pts, dts),
            0x15 => ParseOds(payloadBuffer, pts, dts),
            0x16 => ParsePcs(payloadBuffer, pts, dts),
            0x17 => ParseWds(payloadBuffer, pts, dts),
            0x80 => new EndSegment(pts, dts),
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    /// <summary>
    ///     Attempts to asynchronously read the next PGS segment from the <see cref="Input"/>
    ///     stream.
    /// </summary>
    /// <remarks>
    ///     The current position of the <see cref="Input"/> stream must be at the beginning of a
    ///     a segment.
    /// </remarks>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The PGS segment that was read or <see langword="null"/> if the <see cref="Input"/>
    ///     stream is already EOF.
    /// </returns>
    /// <exception cref="SegmentException">
    ///     An encoded value within the segment is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to read the segment.
    /// </exception>
    public async Task<Segment?> ReadAsync(CancellationToken cancellationToken = default)
    {
        var headerBuffer = new byte[13];
        var headerBytesRead = await Input.ReadAsync(headerBuffer, 0, headerBuffer.Length
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
        var payloadBytesRead = await Input.ReadAsync(payloadBuffer, 0, payloadBuffer.Length
            , cancellationToken);

        if (payloadBytesRead != payloadBuffer.Length)
            throw new IOException("EOF reading segment payload.");

        return kind switch
        {
            0x14 => ParsePds(payloadBuffer, pts, dts),
            0x15 => ParseOds(payloadBuffer, pts, dts),
            0x16 => ParsePcs(payloadBuffer, pts, dts),
            0x17 => ParseWds(payloadBuffer, pts, dts),
            0x80 => new EndSegment(pts, dts),
            _ => throw new SegmentException("Unrecognized segment kind."),
        };
    }

    /// <summary>
    ///     Disposes the <see cref="Input"/> stream if <see cref="LeaveOpen"/> is
    ///     <see langword="false"/>.
    /// </summary>
    public void Dispose()
    {
        if (!LeaveOpen)
            Input.Dispose();
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Asynchronously disposes the <see cref="Input"/> stream if <see cref="LeaveOpen"/> is
    ///     <see langword="false"/>.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (!LeaveOpen)
            await Input.DisposeAsync();
    }
#endif

    private static PresentationCompositionSegment ParsePcs(byte[] buffer, long pts, long dts)
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
        var paletteId = ReadUInt8(buffer, 9)
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
            Crop? crop;

            if ((flags & 0x80) != 0)
            {
                var cropX = ReadUInt16Be(buffer, offset + 8)
                    ?? throw new IOException($"EOS reading PCS[{i}] cropped X position.");
                var cropY = ReadUInt16Be(buffer, offset + 10)
                    ?? throw new IOException($"EOS reading PCS[{i}] cropped Y position.");
                var cropWidth = ReadUInt16Be(buffer, offset + 12)
                    ?? throw new IOException($"EOS reading PCS[{i}] cropped width.");
                var cropHeight = ReadUInt16Be(buffer, offset + 14)
                    ?? throw new IOException($"EOS reading PCS[{i}] cropped height.");

                crop = new Crop(cropX, cropY, cropWidth, cropHeight);
                offset += 16;
            }
            else
            {
                crop = null;
                offset += 8;
            }

            var cid = new CompositionId(objectId, windowId);
            var co = new CompositionObject(cid, x, y, forced, crop);

            compositionObjects.Add(co);
        }

        return new PresentationCompositionSegment(pts, dts, width, height, frameRate
            , compositionNumber, compositionState, paletteUpdateOnly, paletteId
            , compositionObjects);
    }

    private static WindowDefinitionSegment ParseWds(byte[] buffer, long pts, long dts)
    {
        var definitions = new List<WindowDefinitionEntry>();
        var count = ReadUInt8(buffer, 0)
            ?? throw new IOException("EOS reading WDS count.");
        var offset = 1;

        for (int i = 0; i < count; i++)
        {
            var id = ReadUInt8(buffer, offset)
                ?? throw new IOException($"EOS reading WDS[{i}] ID.");
            var x = ReadUInt16Be(buffer, offset + 1)
                ?? throw new IOException($"EOS reading WDS[{i}] X position.");
            var y = ReadUInt16Be(buffer, offset + 3)
                ?? throw new IOException($"EOS reading WDS[{i}] Y position.");
            var width = ReadUInt16Be(buffer, offset + 5)
                ?? throw new IOException($"EOS reading WDS[{i}] width.");
            var height = ReadUInt16Be(buffer, offset + 7)
                ?? throw new IOException($"EOS reading WDS[{i}] height.");
            var window = new WindowDefinitionEntry(id, x, y, width, height);

            definitions.Add(window);
            offset += 9;
        }

        return new WindowDefinitionSegment(pts, dts, definitions);
    }

    private static PaletteDefinitionSegment ParsePds(byte[] buffer, long pts, long dts)
    {
        var count = (buffer.Length - 2) / 5;
        var paletteId = ReadUInt8(buffer, 0)
            ?? throw new IOException("EOS reading PDS ID.");
        var version = ReadUInt8(buffer, 1)
            ?? throw new IOException("EOS reading PDS version.");
        var versionedId = new VersionedId<byte>(paletteId, version);
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
            var pixel = new YcbcraPixel(y, cb, cr, alpha);
            var pde = new PaletteDefinitionEntry(entryId, pixel);

            entries.Add(pde);
            offset += 5;
        }

        return new PaletteDefinitionSegment(pts, dts, versionedId, entries);
    }

    private static ObjectDefinitionSegment ParseOds(byte[] buffer, long pts, long dts)
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

    private static SingleObjectDefinitionSegment ParseSods(byte[] buffer, long pts, long dts
        , int id, byte version)
    {
        var versionedId = new VersionedId<int>(id, version);
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

        return new SingleObjectDefinitionSegment(pts, dts, versionedId, width, height, data);
    }

    private static InitialObjectDefinitionSegment ParseIods(byte[] buffer, long pts, long dts
        , int id, byte version)
    {
        var versionedId = new VersionedId<int>(id, version);
        var dataLength = ReadUInt24Be(buffer, 4)
            ?? throw new IOException("EOS reading I-ODS data length.");
        var width = ReadUInt16Be(buffer, 7)
            ?? throw new IOException("EOS reading I-ODS width.");
        var height = ReadUInt16Be(buffer, 9)
            ?? throw new IOException("EOS reading I-ODS height.");
        var data = new byte[buffer.Length - 11];

        Array.Copy(buffer, 11, data, 0, data.Length);

        return new InitialObjectDefinitionSegment(pts, dts, versionedId, width, height
            , dataLength, data);
    }

    private static MiddleObjectDefinitionSegment ParseMods(byte[] buffer, long pts, long dts
        , int id, byte version)
    {
        var versionedId = new VersionedId<int>(id, version);
        var data = new byte[buffer.Length - 4];

        Array.Copy(buffer, 4, data, 0, data.Length);

        return new MiddleObjectDefinitionSegment(pts, dts, versionedId, data);
    }

    private static FinalObjectDefinitionSegment ParseFods(byte[] buffer, long pts, long dts
        , int id, byte version)
    {
        var versionedId = new VersionedId<int>(id, version);
        var data = new byte[buffer.Length - 4];

        Array.Copy(buffer, 4, data, 0, data.Length);

        return new FinalObjectDefinitionSegment(pts, dts, versionedId, data);
    }

    private static byte? ReadUInt8(byte[] buffer, int offset) =>
        (offset + 1 <= buffer.Length)
            ? buffer[offset]
            : null;

    private static int? ReadUInt16Be(byte[] buffer, int offset) =>
        (offset + 2 <= buffer.Length)
            ? buffer[offset] << 8 | buffer[offset + 1]
            : null;

    private static int? ReadUInt24Be(byte[] buffer, int offset) =>
        (offset + 3 <= buffer.Length)
            ? buffer[offset] << 16 | buffer[offset + 1] << 8 | buffer[offset + 2]
            : null;

    private static long? ReadUInt32Be(byte[] buffer, int offset) =>
        (offset + 4 <= buffer.Length)
            ? buffer[offset] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8
                | buffer[offset + 3]
            : null;
}
