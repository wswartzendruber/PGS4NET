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

namespace PGS4NET.Tests.Segment;

internal static class FinalObjectDefinitionSegmentData
{
    internal static byte[] Empty = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x15,                   // Type
        0x00, 0x04,             // Size
        0xA0, 0xA1,             // ID
        0xA2,                   // Version
        0x40,                   // Sequence flags
    };

    internal static byte[] Small = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x15,                   // Type
        0x00, 0x08,             // Size
        0xA0, 0xA1,             // ID
        0xA2,                   // Version
        0x40,                   // Sequence flags
        0xE0, 0xE1, 0xE2, 0xE3, // Data
    };
}
