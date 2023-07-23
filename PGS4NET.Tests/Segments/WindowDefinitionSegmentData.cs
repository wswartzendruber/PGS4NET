/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

namespace PGS4NET.Tests.Segments;

internal static class WindowDefinitionSegmentData
{
    internal static byte[] NoWindows = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // Pts
        0x12, 0x34, 0x56, 0x78, // Dts
        0x17,                   // Type
        0x00, 0x01,             // Size
        0x00,                   // Window count
    };

    internal static byte[] OneWindow = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // Pts
        0x12, 0x34, 0x56, 0x78, // Dts
        0x17,                   // Type
        0x00, 0x0A,             // Size
        0x01,                   // Window count
        0xEF,                   // ID
        0xA1, 0xB2,             // X offset
        0xC3, 0xD4,             // Y offset
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
    };

    internal static byte[] TwoWindows = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // Pts
        0x12, 0x34, 0x56, 0x78, // Dts
        0x17,                   // Type
        0x00, 0x13,             // Size
        0x02,                   // Window count
        0xEF,                   // ID
        0xA1, 0xB2,             // X offset
        0xC3, 0xD4,             // Y offset
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
        0xFE,                   // ID
        0x1A, 0x2B,             // X offset
        0x3C, 0x4D,             // Y offset
        0x12, 0x34,             // Width
        0x56, 0x78,             // Height
    };
}
