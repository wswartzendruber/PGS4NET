﻿/*
* Copyright 2024 William Swartzendruber
*
* This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
* copy of the MPL was not distributed with this file, You can obtain one at
* https://mozilla.org/MPL/2.0/.
*
* SPDX-License-Identifier: MPL-2.0
*/

//
// Many thanks are in order for cubicibo for offering this file up as a reference point
// for PGS validation.
//
// https://github.com/cubicibo/SUPer/blob/main/SUPer/pgstream.py
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PGS4NET.Segments;

/// <summary>
///     Extension methods against <see cref="System.IO.Stream" /> for writing PGS segments.
/// </summary>
public static class StreamWriteExtensions
{
    private static readonly SegmentException PaletteUpdateOnlyWithNonNormal
        = new("PaletteUpdateOnly paired with non-Normal composition state.");
    private static readonly SegmentException IllegalPaletteId
        = new("PaletteId must be between 0 and 7 inclusive.");
    private static readonly SegmentException TooManyWindowsDefined
        = new("No more than two windows may be defined.");
    private static readonly SegmentException TooManyPaletteEntries
        = new("No more than 256 palette entries may be defined.");
    private static readonly SegmentException IllegalObjectDimensions
        = new("Object width and height must be between 8 and 4096 pixels inclusive.");

    /// <summary>
    ///     Writes all PGS segments in a collection to a <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write all segments to.
    /// </param>
    /// <param name="segments">
    ///     The collection of segments to write.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static void WriteAllSegments(this Stream stream, IEnumerable<Segment> segments)
    {
        foreach (var segment in segments)
            stream.WriteSegment(segment);
    }

    /// <summary>
    ///     Asynchronously writes all PGS segments in a collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write all segments to.
    /// </param>
    /// <param name="segments">
    ///     The collection of segments to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static async Task WriteAllSegmentsAsync(this Stream stream
        , IEnumerable<Segment> segments, CancellationToken cancellationToken = default)
    {
        foreach (var segment in segments)
            await stream.WriteSegmentAsync(segment, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    ///     Asynchronously writes all PGS segments in an asynchronous collection to a
    ///     <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write all segments to.
    /// </param>
    /// <param name="segments">
    ///     The asynchronous collection of segments to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of a <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write a segment.
    /// </exception>
    public static async Task WriteAllSegmentsAsync(this Stream stream
        , IAsyncEnumerable<Segment> segments, CancellationToken cancellationToken = default)
    {
        await foreach (var segment in segments)
            await stream.WriteSegmentAsync(segment, cancellationToken);
    }
#endif

    /// <summary>
    ///     Writes a PGS segment to a <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write the segment to.
    /// </param>
    /// <param name="segment">
    ///     The segment to write.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of the <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write the segment.
    /// </exception>
    public static void WriteSegment(this Stream stream, Segment segment)
    {
        using var buffer = new MemoryStream();

        WriteUInt16Be(buffer, 0x5047);

        switch (segment)
        {
            case PresentationCompositionSegment pcs:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x16);
                WritePcs(buffer, pcs);
                break;
            case WindowDefinitionSegment wds:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x17);
                WriteWds(buffer, wds);
                break;
            case PaletteDefinitionSegment pcs:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x14);
                WritePds(buffer, pcs);
                break;
            case SingleObjectDefinitionSegment sods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteSods(buffer, sods);
                break;
            case InitialObjectDefinitionSegment iods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteIods(buffer, iods);
                break;
            case MiddleObjectDefinitionSegment mods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteMods(buffer, mods);
                break;
            case FinalObjectDefinitionSegment fods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteFods(buffer, fods);
                break;
            case EndSegment:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x80);
                WriteUInt16Be(buffer, 0);
                break;
            default:
                throw new ArgumentException("Segment type is not recognized.");
        }

        buffer.Seek(0, SeekOrigin.Begin);
        buffer.CopyTo(stream);
    }

    /// <summary>
    ///     Asynchronously writes a PGS segment to a <paramref name="stream" />.
    /// </summary>
    /// <param name="stream">
    ///     The stream to write the segment to.
    /// </param>
    /// <param name="segment">
    ///     The segment to write.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="SegmentException">
    ///     A property of the <see cref="Segment" /> is invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     An underlying I/O error occurs while attempting to write the segment.
    /// </exception>
    public static async Task WriteSegmentAsync(this Stream stream, Segment segment,
        CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();

        WriteUInt16Be(buffer, 0x5047);

        switch (segment)
        {
            case PresentationCompositionSegment pcs:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x16);
                WritePcs(buffer, pcs);
                break;
            case WindowDefinitionSegment wds:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x17);
                WriteWds(buffer, wds);
                break;
            case PaletteDefinitionSegment pcs:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x14);
                WritePds(buffer, pcs);
                break;
            case SingleObjectDefinitionSegment sods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteSods(buffer, sods);
                break;
            case InitialObjectDefinitionSegment iods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteIods(buffer, iods);
                break;
            case MiddleObjectDefinitionSegment mods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteMods(buffer, mods);
                break;
            case FinalObjectDefinitionSegment fods:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteFods(buffer, fods);
                break;
            case EndSegment:
                WriteTs(buffer, segment);
                WriteUInt8(buffer, 0x80);
                WriteUInt16Be(buffer, 0);
                break;
            default:
                throw new ArgumentException("Segment type is not recognized.");
        }

        buffer.Seek(0, SeekOrigin.Begin);
        await buffer.CopyToAsync(stream, 81_920, cancellationToken);
    }

    private static void WriteTs(Stream stream, Segment segment)
    {
        WriteUInt32Be(stream, segment.Pts);
        WriteUInt32Be(stream, segment.Dts);
    }

    private static void WritePcs(Stream stream, PresentationCompositionSegment pcs)
    {
        if (pcs.State != CompositionState.Normal)
        {
            if (pcs.PaletteUpdateOnly)
                throw PaletteUpdateOnlyWithNonNormal;
            if (pcs.PaletteId < 0 || 7 < pcs.PaletteId)
                throw IllegalPaletteId;
        }

        using var ms = new MemoryStream();

        WriteUInt16Be(ms, pcs.Width);
        WriteUInt16Be(ms, pcs.Height);
        WriteUInt8(ms, pcs.FrameRate);
        WriteUInt16Be(ms, pcs.Number);
        WriteUInt8(ms, pcs.State switch
        {
            CompositionState.Normal => 0x00,
            CompositionState.AcquisitionPoint => 0x40,
            CompositionState.EpochStart => 0x80,
            _ => throw new ArgumentException("PCS has unrecognized composition state."),
        });
        WriteUInt8(ms, (byte)(pcs.PaletteUpdateOnly ? 0x80 : 0x00));
        WriteUInt8(ms, pcs.PaletteId);

        if (pcs.CompositionObjects.Count < 256)
            WriteUInt8(ms, (byte)pcs.CompositionObjects.Count);
        else
            throw new SegmentException("PCS defines too many composition objects.");

        foreach (var co in pcs.CompositionObjects)
        {
            WriteUInt16Be(ms, co.Id.ObjectId);
            WriteUInt8(ms, co.Id.WindowId);
            WriteUInt8(ms, (byte)(
                (co.Crop is not null ? 0x80 : 0x00) | (co.Forced ? 0x40 : 0x00)
            ));
            WriteUInt16Be(ms, co.X);
            WriteUInt16Be(ms, co.Y);

            if (co.Crop is Crop crop)
            {
                WriteUInt16Be(ms, crop.X);
                WriteUInt16Be(ms, crop.Y);
                WriteUInt16Be(ms, crop.Width);
                WriteUInt16Be(ms, crop.Height);
            }
        }

        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteWds(Stream stream, WindowDefinitionSegment wds)
    {
        if (2 < wds.Definitions.Count)
            throw TooManyWindowsDefined;

        using var ms = new MemoryStream();

        if (wds.Definitions.Count < 256)
            WriteUInt8(ms, (byte)wds.Definitions.Count);
        else
            throw new SegmentException("WDS defines too many window definitions.");

        foreach (var wd in wds.Definitions)
        {
            WriteUInt8(ms, wd.Id);
            WriteUInt16Be(ms, wd.X);
            WriteUInt16Be(ms, wd.Y);
            WriteUInt16Be(ms, wd.Width);
            WriteUInt16Be(ms, wd.Height);
        }

        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WritePds(Stream stream, PaletteDefinitionSegment pds)
    {
        if (pds.VersionedId.Id < 0 || 7 < pds.VersionedId.Id)
            throw IllegalPaletteId;
        if (256 < pds.Entries.Count)
            throw TooManyPaletteEntries;

        using var ms = new MemoryStream();

        WriteUInt8(ms, pds.VersionedId.Id);
        WriteUInt8(ms, pds.VersionedId.Version);

        foreach (var entry in pds.Entries)
        {
            WriteUInt8(ms, entry.Id);
            WriteUInt8(ms, entry.Pixel.Y);
            WriteUInt8(ms, entry.Pixel.Cr);
            WriteUInt8(ms, entry.Pixel.Cb);
            WriteUInt8(ms, entry.Pixel.Alpha);
        }

        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteSods(Stream stream, SingleObjectDefinitionSegment sods)
    {
        if (Math.Min(sods.Width, sods.Height) < 8 || 4096 < Math.Max(sods.Width, sods.Height))
            throw IllegalObjectDimensions;

        var dataLength = (sods.Data.Length <= 65_524)
            ? (long)sods.Data.Length + 4
            : throw new SegmentException("S-ODS data length exceeds 65,524.");

        using var ms = new MemoryStream();

        WriteUInt16Be(ms, sods.VersionedId.Id);
        WriteUInt8(ms, sods.VersionedId.Version);
        WriteUInt8(ms, 0xC0);
        WriteUInt24Be(ms, dataLength);
        WriteUInt16Be(ms, sods.Width);
        WriteUInt16Be(ms, sods.Height);
        ms.Write(sods.Data, 0, sods.Data.Length);
        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteIods(Stream stream, InitialObjectDefinitionSegment iods)
    {
        if (Math.Min(iods.Width, iods.Height) < 8 || 4096 < Math.Max(iods.Width, iods.Height))
            throw IllegalObjectDimensions;

        if (iods.Length > 16_777_216)
            throw new SegmentException("I-ODS declared data length exceeds 16,777,216.");
        if (iods.Data.Length > 65_524)
            throw new SegmentException("I-ODS contained data length exceeds 65,524.");

        using var ms = new MemoryStream();

        WriteUInt16Be(ms, iods.VersionedId.Id);
        WriteUInt8(ms, iods.VersionedId.Version);
        WriteUInt8(ms, 0x80);
        WriteUInt24Be(ms, iods.Length);
        WriteUInt16Be(ms, iods.Width);
        WriteUInt16Be(ms, iods.Height);
        ms.Write(iods.Data, 0, iods.Data.Length);
        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteMods(Stream stream, MiddleObjectDefinitionSegment mods)
    {
        if (mods.Data.Length > 65_531)
            throw new SegmentException("M-ODS contained data length exceeds 65,531.");

        using var ms = new MemoryStream();

        WriteUInt16Be(ms, mods.VersionedId.Id);
        WriteUInt8(ms, mods.VersionedId.Version);
        WriteUInt8(ms, 0x00);
        ms.Write(mods.Data, 0, mods.Data.Length);

        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteFods(Stream stream, FinalObjectDefinitionSegment fods)
    {
        if (fods.Data.Length > 65_531)
            throw new SegmentException("F-ODS contained data length exceeds 65,531.");

        using var ms = new MemoryStream();

        WriteUInt16Be(ms, fods.VersionedId.Id);
        WriteUInt8(ms, fods.VersionedId.Version);
        WriteUInt8(ms, 0x40);
        ms.Write(fods.Data, 0, fods.Data.Length);

        WriteUInt16Be(stream, (int)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteUInt8(Stream stream, byte value)
    {
        stream.WriteByte(value);
    }

    private static void WriteUInt16Be(Stream stream, int value)
    {
        if (value < 0x0 || value > 0xFFFF)
            throw new OverflowException($"{value} is not a 16-bit unsigned integer.");

        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }

    private static void WriteUInt24Be(Stream stream, long value)
    {
        if (value < 0x0 || value > 0xFFFFFF)
            throw new OverflowException($"{value} is not a 24-bit unsigned integer.");

        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }

    private static void WriteUInt32Be(Stream stream, long value)
    {
        if (value < 0x0 || value > 0xFFFFFFFF)
            throw new OverflowException($"{value} is not a 32-bit unsigned integer.");

        stream.WriteByte((byte)(value >> 24));
        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }
}
