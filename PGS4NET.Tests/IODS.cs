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

namespace PGS4NET.Tests;

using PGS4NET;

internal static class IODS
{
    internal static byte[] Empty = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x15,                   // Type
        0x00, 0x0B,             // Size
        0xA0, 0xA1,             // ID
        0xA2,                   // Version
        0x80,                   // Sequence flags
        0xAB, 0xCD, 0xEF,       // Data length
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
    };

    internal static byte[] Small = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x15,                   // Type
        0x00, 0x0F,             // Size
        0xA0, 0xA1,             // ID
        0xA2,                   // Version
        0x80,                   // Sequence flags
        0xAB, 0xCD, 0xEF,       // Data length
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
        0xE0, 0xE1, 0xE2, 0xE3, // Data
    };
}
