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
using System.IO;
using System.Threading.Tasks;

namespace PGS4NET.Segment;

/// <summary>
///     Contains extensions against <see cref="Stream" /> for writing PGS segments.
/// </summary>
public static partial class StreamExtensions
{
    /// <summary>
    ///     Writes a <see cref="Segment" /> to a <see cref="Stream" />.
    /// </summary>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a segment.
    /// </exception>
    public static void WriteSegment(this Stream stream, Segment segment)
    {
        using var buffer = new MemoryStream();

        WriteUInt16BE(buffer, 0x5047);

        switch (segment)
        {
            case PresentationCompositionSegment pcs:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x16);
                WritePCS(buffer, pcs);
                break;
            case WindowDefinitionSegment wds:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x17);
                WriteWDS(buffer, wds);
                break;
            case PaletteDefinitionSegment pcs:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x14);
                WritePDS(buffer, pcs);
                break;
            case SingleObjectDefinitionSegment sods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteSODS(buffer, sods);
                break;
            case InitialObjectDefinitionSegment iods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteIODS(buffer, iods);
                break;
            case MiddleObjectDefinitionSegment mods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteMODS(buffer, mods);
                break;
            case FinalObjectDefinitionSegment fods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteFODS(buffer, fods);
                break;
            case EndSegment:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x80);
                WriteUInt16BE(buffer, 0);
                break;
            default:
                throw new ArgumentException("Segment type is not recognized.");
        }

        buffer.Seek(0, SeekOrigin.Begin);
        buffer.CopyTo(stream);
    }

    /// <summary>
    ///     Asynchronously writes a <see cref="Segment" /> to a <see cref="Stream" />.
    /// </summary>
    /// <exception cref="SegmentException">
    ///     Thrown when the flags inside of a segment are invalid.
    /// </exception>
    /// <exception cref="IOException">
    ///     Thrown when an underlying IO error occurs while attempting to write a segment.
    /// </exception>
    public static async Task WriteSegmentAsync(this Stream stream, Segment segment)
    {
        using var buffer = new MemoryStream();

        WriteUInt16BE(buffer, 0x5047);

        switch (segment)
        {
            case PresentationCompositionSegment pcs:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x16);
                WritePCS(buffer, pcs);
                break;
            case WindowDefinitionSegment wds:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x17);
                WriteWDS(buffer, wds);
                break;
            case PaletteDefinitionSegment pcs:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x14);
                WritePDS(buffer, pcs);
                break;
            case SingleObjectDefinitionSegment sods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteSODS(buffer, sods);
                break;
            case InitialObjectDefinitionSegment iods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteIODS(buffer, iods);
                break;
            case MiddleObjectDefinitionSegment mods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteMODS(buffer, mods);
                break;
            case FinalObjectDefinitionSegment fods:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x15);
                WriteFODS(buffer, fods);
                break;
            case EndSegment:
                WriteTS(buffer, segment);
                WriteUInt8(buffer, 0x80);
                WriteUInt16BE(buffer, 0);
                break;
            default:
                throw new ArgumentException("Segment type is not recognized.");
        }

        buffer.Seek(0, SeekOrigin.Begin);
        await buffer.CopyToAsync(stream);
    }

    private static void WriteTS(Stream stream, Segment segment)
    {
        WriteUInt32BE(stream, segment.PTS);
        WriteUInt32BE(stream, segment.DTS);
    }

    private static void WritePCS(Stream stream, PresentationCompositionSegment pcs)
    {
        using var ms = new MemoryStream();

        WriteUInt16BE(ms, pcs.Width);
        WriteUInt16BE(ms, pcs.Height);
        WriteUInt8(ms, pcs.FrameRate);
        WriteUInt16BE(ms, pcs.Number);
        WriteUInt8(ms, pcs.State switch
        {
            CompositionState.Normal => 0x00,
            CompositionState.AcquisitionPoint => 0x40,
            CompositionState.EpochStart => 0x80,
            _ => throw new ArgumentException("PCS has unrecognized composition state."),
        });
        WriteUInt8(ms, (byte)(pcs.PaletteUpdateOnly ? 0x80 : 0x00));
        WriteUInt8(ms, pcs.PaletteUpdateId);

        if (pcs.Objects.Count < 256)
            WriteUInt8(ms, (byte)pcs.Objects.Count);
        else
            throw new SegmentException("PCS defines too many composition objects.");

        foreach (var co in pcs.Objects)
        {
            WriteUInt16BE(ms, co.ObjectID);
            WriteUInt8(ms, co.WindowID);
            WriteUInt8(ms, (byte)(
                (co.Crop is not null ? 0x80 : 0x00) | (co.Forced ? 0x40 : 0x00)
            ));
            WriteUInt16BE(ms, co.X);
            WriteUInt16BE(ms, co.Y);

            if (co.Crop is CroppedArea ca)
            {
                WriteUInt16BE(ms, ca.X);
                WriteUInt16BE(ms, ca.Y);
                WriteUInt16BE(ms, ca.Width);
                WriteUInt16BE(ms, ca.Height);
            }
        }

        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteWDS(Stream stream, WindowDefinitionSegment wds)
    {
        using var ms = new MemoryStream();

        if (wds.Definitions.Count < 256)
            WriteUInt8(ms, (byte)wds.Definitions.Count);
        else
            throw new SegmentException("WDS defines too many window definitions.");

        foreach (var wd in wds.Definitions)
        {
            WriteUInt8(ms, wd.Id);
            WriteUInt16BE(ms, wd.X);
            WriteUInt16BE(ms, wd.Y);
            WriteUInt16BE(ms, wd.Width);
            WriteUInt16BE(ms, wd.Height);
        }

        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WritePDS(Stream stream, PaletteDefinitionSegment pds)
    {
        using var ms = new MemoryStream();

        WriteUInt8(ms, pds.Id);
        WriteUInt8(ms, pds.Version);

        foreach (var entry in pds.Entries)
        {
            WriteUInt8(ms, entry.Id);
            WriteUInt8(ms, entry.Y);
            WriteUInt8(ms, entry.Cr);
            WriteUInt8(ms, entry.Cb);
            WriteUInt8(ms, entry.Alpha);
        }

        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteSODS(Stream stream, SingleObjectDefinitionSegment sods)
    {
        var dataLength = (sods.Data.Length <= 65_524)
            ? (uint)sods.Data.Length + 4
            : throw new SegmentException("S-ODS data length exceeds 65,524.");

        using var ms = new MemoryStream();

        WriteUInt16BE(ms, sods.Id);
        WriteUInt8(ms, sods.Version);
        WriteUInt8(ms, 0xC0);
        WriteUInt24BE(ms, dataLength);
        WriteUInt16BE(ms, sods.Width);
        WriteUInt16BE(ms, sods.Height);
        ms.Write(sods.Data, 0, sods.Data.Length);
        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteIODS(Stream stream, InitialObjectDefinitionSegment iods)
    {
        if (iods.Length > 16_777_216)
            throw new SegmentException("I-ODS declared data length exceeds 16,777,216.");
        if (iods.Data.Length > 65_524)
            throw new SegmentException("I-ODS contained data length exceeds 65,524.");

        using var ms = new MemoryStream();

        WriteUInt16BE(ms, iods.Id);
        WriteUInt8(ms, iods.Version);
        WriteUInt8(ms, 0x80);
        WriteUInt24BE(ms, iods.Length);
        WriteUInt16BE(ms, iods.Width);
        WriteUInt16BE(ms, iods.Height);
        ms.Write(iods.Data, 0, iods.Data.Length);
        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteMODS(Stream stream, MiddleObjectDefinitionSegment mods)
    {
        if (mods.Data.Length > 65_531)
            throw new SegmentException("M-ODS contained data length exceeds 65,531.");

        using var ms = new MemoryStream();

        WriteUInt16BE(ms, mods.Id);
        WriteUInt8(ms, mods.Version);
        WriteUInt8(ms, 0x00);
        ms.Write(mods.Data, 0, mods.Data.Length);

        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteFODS(Stream stream, FinalObjectDefinitionSegment fods)
    {
        if (fods.Data.Length > 65_531)
            throw new SegmentException("F-ODS contained data length exceeds 65,531.");

        using var ms = new MemoryStream();

        WriteUInt16BE(ms, fods.Id);
        WriteUInt8(ms, fods.Version);
        WriteUInt8(ms, 0x40);
        ms.Write(fods.Data, 0, fods.Data.Length);

        WriteUInt16BE(stream, (ushort)ms.Length);
        ms.WriteTo(stream);
    }

    private static void WriteUInt8(Stream stream, byte value)
    {
        stream.WriteByte(value);
    }

    private static void WriteUInt16BE(Stream stream, ushort value)
    {
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }

    private static void WriteUInt24BE(Stream stream, uint value)
    {
        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }

    private static void WriteUInt32BE(Stream stream, uint value)
    {
        stream.WriteByte((byte)(value >> 24));
        stream.WriteByte((byte)(value >> 16));
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)value);
    }
}
