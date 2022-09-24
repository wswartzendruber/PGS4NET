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

using System;
using System.IO;

public static partial class StreamExtensions
{
    public static void WriteSegment(this Stream stream, Segment segment)
    {
        WriteUInt16BE(stream, 0x5047);

        switch (segment)
        {
            case PresentationCompositionSegment pcs:
                WriteTS(stream, segment);
                WriteUInt8(stream, 0x16);
                WritePCS(stream, pcs);
                break;
            default:
                throw new ArgumentException("Segment type is not recognized.");
        }
    }

    private static void WriteTS(Stream stream, Segment segment)
    {
        WriteUInt32BE(stream, segment.PTS);
        WriteUInt32BE(stream, segment.DTS);
    }

    private static void WritePCS(Stream stream, PresentationCompositionSegment pcs)
    {
        using (var ms = new MemoryStream())
        {
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
                }
            );

            if (pcs.PaletteUpdateID is byte paletteUpdateID)
            {
                WriteUInt8(ms, 0x80);
                WriteUInt8(ms, paletteUpdateID);
            }
            else
            {
                WriteUInt16BE(ms, 0x0000);
            }

            if (pcs.Objects.Count < 256)
                WriteUInt8(ms, (byte)pcs.Objects.Count);
            else
                throw new SegmentException("PCS defines too many composition objects.");

            foreach (var co in pcs.Objects)
            {
                WriteUInt16BE(ms, co.ObjectID);
                WriteUInt8(ms, co.WindowID);
                WriteUInt8(ms, (byte)(
                    (co.Crop.HasValue ? 0x80 : 0x00) | (co.Forced ? 0x40 : 0x00)
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
            ms.CopyTo(stream);
        }
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
