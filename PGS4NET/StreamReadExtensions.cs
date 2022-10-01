/*
 * Copyright 2022 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET;

using System.Collections.Generic;
using System.IO;

public static partial class StreamExtensions
{
    public static Segment ReadSegment(this Stream stream)
    {
        var magicNumber = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading magic number.");

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized magic number.");

        var pts = ReadUInt32BE(stream)
            ?? throw new IOException("EOF while reading segment PTS.");
        var dts = ReadUInt32BE(stream)
            ?? throw new IOException("EOF while reading segment DTS.");
        var kind = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading segment kind.");
        var size = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading segment size.");

        return kind switch
        {
            0x14 => ReadPDS(stream, pts, dts, size),
            0x15 => ReadODS(stream, pts, dts, size),
            0x16 => ReadPCS(stream, pts, dts),
            0x17 => ReadWDS(stream, pts, dts),
            0x80 => new EndSegment { PTS = pts, DTS = dts },
            _ => throw new SegmentException("Unrecognized kind."),
        };
    }

    private static PresentationCompositionSegment ReadPCS(Stream stream, uint pts, uint dts)
    {
        var width = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading PCS width.");
        var height = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading PCS height.");
        var frameRate = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PCS frame rate.");
        var compositionNumber = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading PCS composition number.");
        var parsedCompositionState = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PCS composition state.");
        var compositionState = parsedCompositionState switch
        {
            0x00 => CompositionState.Normal,
            0x40 => CompositionState.AcquisitionPoint,
            0x80 => CompositionState.EpochStart,
            _ => throw new SegmentException("PCS has unrecognized composition state."),
        };
        var parsedPaletteUpdateFlag = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PCS palette update flag.");
        var parsedPaletteUpdateID = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PCS palette update ID.");
        byte? paletteUpdateID = parsedPaletteUpdateFlag switch
        {
            0x00 => null,
            0x80 => parsedPaletteUpdateID,
            _ => throw new SegmentException("PCS has unrecognized palette update flag."),
        };
        var compositionObjectCount = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PCS composition count.");
        var compositionObjects = new List<CompositionObject>();

        for (int i = 0; i < compositionObjectCount; i++)
        {
            var objectID = ReadUInt16BE(stream)
                ?? throw new IOException("EOF while reading PCS composition object ID.");
            var windowID = ReadUInt8(stream)
                ?? throw new IOException("EOF while reading PCS composition window ID.");
            var flags = ReadUInt8(stream)
                ?? throw new IOException("EOF while reading PCS composition flags.");
            var x = ReadUInt16BE(stream)
                ?? throw new IOException("EOF while reading PCS composition X value.");
            var y = ReadUInt16BE(stream)
                ?? throw new IOException("EOF while reading PCS composition Y value.");
            var forced = (flags & 0x40) != 0;
            CroppedArea? croppedArea = ((flags & 0x80) != 0) ? new CroppedArea {
                X = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading PCS composition crop X value."),
                Y = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading PCS composition crop Y value."),
                Width = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading PCS composition crop width."),
                Height = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading PCS composition crop height."),
            } : null;

            compositionObjects.Add(
                new CompositionObject {
                    ObjectID = objectID,
                    WindowID = windowID,
                    X = x,
                    Y = y,
                    Forced = forced,
                    Crop = croppedArea,
                }
            );
        }

        return new PresentationCompositionSegment {
            PTS = pts,
            DTS = dts,
            Width = width,
            Height = height,
            FrameRate = frameRate,
            Number = compositionNumber,
            State = compositionState,
            PaletteUpdateID = paletteUpdateID,
            Objects = compositionObjects,
        };
    }

    private static WindowDefinitionSegment ReadWDS(Stream stream, uint pts, uint dts)
    {
        var definitions = new List<WindowDefinition>();
        var count = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading WDS definition count.");

        for (int i = 0; i < count; i++)
        {
            definitions.Add(new WindowDefinition
            {
                ID = ReadUInt8(stream)
                    ?? throw new IOException("EOF while reading WDS definition ID."),
                X = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading WDS definition X value."),
                Y = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading WDS definition Y value."),
                Width = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading WDS definition width."),
                Height = ReadUInt16BE(stream)
                    ?? throw new IOException("EOF while reading WDS definition height."),
            }
            );
        }

        return new WindowDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            Definitions = definitions,
        };
    }

    private static PaletteDefinitionSegment ReadPDS(Stream stream, uint pts, uint dts
        , ushort size)
    {
        var count = (size - 2) / 5;
        var id = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PDS ID.");
        var version = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading PDS version.");
        var entries = new List<PaletteEntry>();

        for (int i = 0; i < count; i++)
        {
            entries.Add(new PaletteEntry
            {
                ID = ReadUInt8(stream)
                    ?? throw new IOException("EOF while reading PDS entry ID."),
                Y = ReadUInt8(stream)
                    ?? throw new IOException("EOF while reading PDS entry Y value."),
                Cr = ReadUInt8(stream)
                    ?? throw new IOException("EOF while reading PDS entry Cr value."),
                Cb = ReadUInt8(stream)
                    ?? throw new IOException("EOF while reading PDS entry Cb value."),
                Alpha = ReadUInt8(stream)
                    ?? throw new IOException("EOF while reading PDS entry alpha value."),
            }
            );
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

    private static ObjectDefinitionSegment ReadODS(Stream stream, uint pts, uint dts
        , ushort size)
    {
        var id = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading ODS ID.");
        var version = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading ODS version.");
        var sequenceFlags = ReadUInt8(stream)
            ?? throw new IOException("EOF while reading ODS sequence flags.");

        return sequenceFlags switch
        {
            0xC0 => ReadSODS(stream, pts, dts, id, version, size),
            0x80 => ReadIODS(stream, pts, dts, id, version, size),
            0x00 => ReadMODS(stream, pts, dts, id, version, size),
            0x40 => ReadFODS(stream, pts, dts, id, version, size),
            _ => throw new SegmentException("Unrecognized ODS sequence flags."),
        };
    }

    private static SingleObjectDefinitionSegment ReadSODS(Stream stream, uint pts, uint dts
        , ushort id, byte version, ushort size)
    {
        var dataLength = ReadUInt24BE(stream)
            ?? throw new IOException("EOF while reading S-ODS data length.");

        if (dataLength != size - 7)
            throw new SegmentException("Unexpected S-ODS data length.");

        var width = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading S-ODS width.");
        var height = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading S-ODS height.");
        var data = new byte[size - 11];

        if (stream.Read(data, 0, data.Length) != data.Length)
            throw new IOException("EOF while reading S-ODS image data.");

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

    private static InitialObjectDefinitionSegment ReadIODS(Stream stream, uint pts, uint dts
        , ushort id, byte version, ushort size)
    {
        var dataLength = ReadUInt24BE(stream)
            ?? throw new IOException("EOF while reading I-ODS data length.");
        var width = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading I-ODS width.");
        var height = ReadUInt16BE(stream)
            ?? throw new IOException("EOF while reading I-ODS height.");
        var data = new byte[size - 11];

        if (stream.Read(data, 0, data.Length) != data.Length)
            throw new IOException("EOF while reading I-ODS image data.");

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

    private static MiddleObjectDefinitionSegment ReadMODS(Stream stream, uint pts, uint dts
        , ushort id, byte version, ushort size)
    {
        var data = new byte[size - 4];

        if (stream.Read(data, 0, data.Length) != data.Length)
            throw new IOException("EOF while reading M-ODS image data.");

        return new MiddleObjectDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Data = data,
        };
    }

    private static FinalObjectDefinitionSegment ReadFODS(Stream stream, uint pts, uint dts
        , ushort id, byte version, ushort size)
    {
        var data = new byte[size - 4];

        if (stream.Read(data, 0, data.Length) != data.Length)
            throw new IOException("EOF while reading M-ODS image data.");

        return new FinalObjectDefinitionSegment
        {
            PTS = pts,
            DTS = dts,
            ID = id,
            Version = version,
            Data = data,
        };
    }

    private static byte? ReadUInt8(Stream stream)
    {
        var byteRead = stream.ReadByte();

        return (byteRead >= 0) ? (byte)byteRead : null;
    }

    private static ushort? ReadUInt16BE(this Stream stream)
    {
        var buffer = new byte[2];
        var bytesRead = stream.Read(buffer, 0, 2);

        return (bytesRead == 2) ? (ushort)
        (
            buffer[0] << 8
            | buffer[1]
        )
        : null;
    }

    private static uint? ReadUInt24BE(Stream stream)
    {
        var buffer = new byte[3];
        var bytesRead = stream.Read(buffer, 0, 3);

        return (bytesRead == 3) ? (uint)
        (
            buffer[0] << 16
            | buffer[1] << 8
            | buffer[2]
        )
        : null;
    }

    private static uint? ReadUInt32BE(Stream stream)
    {
        var buffer = new byte[4];
        var bytesRead = stream.Read(buffer, 0, 4);

        return (bytesRead == 4) ? (uint)
        (
            buffer[0] << 24
            | buffer[1] << 16
            | buffer[2] << 8
            | buffer[3]
        )
        : null;
    }
}
