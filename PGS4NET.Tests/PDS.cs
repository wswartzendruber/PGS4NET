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

internal static class PDS
{
    internal static byte[] NoEntries = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x14,                   // Type
        0x00, 0x02,             // Size
        0xA1,                   // ID
        0xA2,                   // Version
    };

    internal static byte[] OneEntry = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x14,                   // Type
        0x00, 0x07,             // Size
        0xA1,                   // ID
        0xA2,                   // Version
        0xB1,                   // Entry ID
        0xB2,                   // Entry Y value
        0xB3,                   // Entry Cr value
        0xB4,                   // Entry Cb value
        0xB5,                   // Entry alpha value
    };

    internal static byte[] TwoEntries = new byte[]
    {
        0x50, 0x47,             // Magic number
        0x01, 0x23, 0x45, 0x67, // PTS
        0x12, 0x34, 0x56, 0x78, // DTS
        0x14,                   // Type
        0x00, 0x0C,             // Size
        0xA1,                   // ID
        0xA2,                   // Version
        0xB1,                   // Entry ID
        0xB2,                   // Entry Y value
        0xB3,                   // Entry Cr value
        0xB4,                   // Entry Cb value
        0xB5,                   // Entry alpha value
        0xC1,                   // Entry ID
        0xC2,                   // Entry Y value
        0xC3,                   // Entry Cr value
        0xC4,                   // Entry Cb value
        0xC5,                   // Entry alpha value
    };
}
