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

public static class StreamExtensions
{
    public static Segment ReadSegment(this Stream stream)
    {
        var magicNumber = stream.ReadUInt16BE()
            ?? throw new IOException("EOF while reading magic number.");

        if (magicNumber != 0x5047)
            throw new SegmentException("Unrecognized magic number.");

        var pts = stream.ReadUInt32BE()
            ?? throw new IOException("EOF while reading segment PTS.");
        var dts = stream.ReadUInt32BE()
            ?? throw new IOException("EOF while reading segment DTS.");
        var kind = stream.ReadUInt8()
            ?? throw new IOException("EOF while reading segment kind.");
        var size = stream.ReadUInt16BE()
            ?? throw new IOException("EOF while reading segment size.");

        return kind switch
        {
            0x14 => null,
            0x15 => null,
            0x16 => ParsePCS(stream, pts, dts),
            0x17 => null,
            0x80 => null,
            _ => throw new SegmentException("Unrecognized kind."),
        };
    }

    private static PresentationCompositionSegment ParsePCS(Stream stream, uint pts, uint dts)
    {
        var width = stream.ReadUInt16BE()
            ?? throw new IOException("EOF while reading PCS width.");
        var height = stream.ReadUInt16BE()
            ?? throw new IOException("EOF while reading PCS height.");
        var frameRate = stream.ReadUInt8()
            ?? throw new IOException("EOF while reading PCS frame rate.");
        var compositionNumber = stream.ReadUInt16BE()
            ?? throw new IOException("EOF while reading PCS composition number.");
        var parsedCompositionState = stream.ReadUInt8()
            ?? throw new IOException("EOF while reading PCS composition state.");
        var compositionState = parsedCompositionState switch
        {
            0x00 => CompositionState.Normal,
            0x40 => CompositionState.AcquisitionPoint,
            0x80 => CompositionState.EpochStart,
            _ => throw new SegmentException("PCS has unrecognized composition state."),
        };
        var parsedPaletteUpdateFlag = stream.ReadUInt8()
            ?? throw new IOException("EOF while reading PCS palette update flag.");
        var parsedPaletteUpdateID = stream.ReadUInt8()
            ?? throw new IOException("EOF while reading PCS palette update ID.");
        byte? paletteUpdateID = parsedPaletteUpdateID switch
        {
            0x00 => null,
            0x80 => parsedPaletteUpdateID,
            _ => throw new SegmentException("PCS has unrecognized palette update flag."),
        };
        var compositionObjectCount = stream.ReadUInt8()
            ?? throw new IOException("EOF while reading PCS composition count.");
        var compositionObjects = new List<CompositionObject>();

        for (int i = 0; i < compositionObjectCount; i++)
        {
            var objectID = stream.ReadUInt16BE()
                ?? throw new IOException("EOF while reading PCS composition object ID.");
            var windowID = stream.ReadUInt8()
                ?? throw new IOException("EOF while reading PCS composition window ID.");
            var flags = stream.ReadUInt8()
                ?? throw new IOException("EOF while reading PCS composition flags.");
            var x = stream.ReadUInt16BE()
                ?? throw new IOException("EOF while reading PCS composition X value.");
            var y = stream.ReadUInt16BE()
                ?? throw new IOException("EOF while reading PCS composition Y value.");
            var forced = (flags & 0x40) != 0;
            Crop? croppedDimensions = ((flags & 0x80) != 0) ? new Crop {
                X = stream.ReadUInt16BE()
                    ?? throw new IOException("EOF while reading PCS composition crop X value."),
                Y = stream.ReadUInt16BE()
                    ?? throw new IOException("EOF while reading PCS composition crop Y value."),
                Width = stream.ReadUInt16BE()
                    ?? throw new IOException("EOF while reading PCS composition crop width."),
                Height = stream.ReadUInt16BE()
                    ?? throw new IOException("EOF while reading PCS composition crop height."),
            } : null;

            compositionObjects.Add(
                new CompositionObject {
                    ObjectID = objectID,
                    WindowID = windowID,
                    X = x,
                    Y = y,
                    Forced = forced,
                    CroppedDimensions = croppedDimensions,
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

    private static byte? ReadUInt8(this Stream stream)
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

    private static uint? ReadUInt24BE(this Stream stream)
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

    private static uint? ReadUInt32BE(this Stream stream)
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
