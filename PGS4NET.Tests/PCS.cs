/*
 * Copyright 2022 William Swartzendruber
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

internal static class PCS
{
    internal static byte[] EpochStart_NoPaletteUpdateID_NoObjects = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x16,                   // Type
        0x00, 0x00,             // Size
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
        0x10,                   // Frame rate
        0x65, 0x43,             // Composition number
        0x80,                   // Composition state
        0x00,                   // Palette update flag
        0x00,                   // Palette update ID
        0x00,                   // Composition object count
    };

    internal static byte[] AcquisitionPoint_NoPaletteUpdateID_NoObjects = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x16,                   // Type
        0x00, 0x00,             // Size
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
        0x10,                   // Frame rate
        0x65, 0x43,             // Composition number
        0x40,                   // Composition state
        0x00,                   // Palette update flag
        0x00,                   // Palette update ID
        0x00,                   // Composition object count
    };

    internal static byte[] Normal_NoPaletteUpdateID_NoObjects = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x16,                   // Type
        0x00, 0x00,             // Size
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
        0x10,                   // Frame rate
        0x65, 0x43,             // Composition number
        0x00,                   // Composition state
        0x00,                   // Palette update flag
        0x00,                   // Palette update ID
        0x00,                   // Composition object count
    };

    internal static byte[] Normal_PaletteUpdateID_NoObjects = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x16,                   // Type
        0x00, 0x00,             // Size
        0x21, 0x43,             // Width
        0x65, 0x87,             // Height
        0x10,                   // Frame rate
        0x65, 0x43,             // Composition number
        0x00,                   // Composition state
        0x80,                   // Palette update flag
        0xAB,                   // Palette update ID
        0x00,                   // Composition object count
    };
}
