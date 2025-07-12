/*
 * Copyright 2025 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

internal static class SegmentBuffers
{
    internal static readonly Dictionary<string, byte[]> Buffers = new()
    {
        {
            "pcs-es", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x0B,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x80,                   // Composition state
                0x00,                   // Palette update flag
                0xAB,                   // Palette ID
                0x00,                   // Composition object count
            }
        },
        {
            "pcs-ap", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x0B,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x40,                   // Composition state
                0x00,                   // Palette update flag
                0xAB,                   // Palette ID
                0x00,                   // Composition object count
            }
        },
        {
            "pcs-n-palette-update", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x0B,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x00,                   // Composition state
                0x00,                   // Palette update flag
                0xAB,                   // Palette ID
                0x00,                   // Composition object count
            }
        },
        {
            "pcs-n-no-palette-update", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x0B,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x00,                   // Composition state
                0x80,                   // Palette update flag
                0xAB,                   // Palette ID
                0x00,                   // Composition object count
            }
        },
        {
            "pcs-n-one", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x13,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x00,                   // Composition state
                0x00,                   // Palette update flag
                0xAB,                   // Palette ID
                0x01,                   // Composition object count
                0xAB, 0xCD,             // Object 1 object ID
                0xEF,                   // Object 1 window ID
                0x40,                   // Object 1 flags
                0x1A, 0x2B,             // Object 1 X offset
                0x3C, 0x4D,             // Object 1 Y offset
            }
        },
        {
            "pcs-n-one-cropped", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x1B,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x00,                   // Composition state
                0x00,                   // Palette update flag
                0xAB,                   // Palette ID
                0x01,                   // Composition object count
                0xAB, 0xCD,             // Object 1 object ID
                0xEF,                   // Object 1 window ID
                0x80,                   // Object 1 flags
                0x1A, 0x2B,             // Object 1 X offset
                0x3C, 0x4D,             // Object 1 Y offset
                0xA1, 0xA2,             // Object 1 cropped X offset
                0xA3, 0xA4,             // Object 1 cropped Y offset
                0xA5, 0xA6,             // Object 1 cropped width
                0xA7, 0xA8,             // Object 1 cropped height
            }
        },
        {
            "pcs-n-three-mixed", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x16,                   // Type
                0x00, 0x33,             // Size
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0x10,                   // Frame rate
                0x65, 0x43,             // Composition number
                0x00,                   // Composition state
                0x00,                   // Palette update flag
                0xAB,                   // Palette ID
                0x03,                   // Composition object count
                0xAB, 0xCD,             // Object 1 object ID
                0xEF,                   // Object 1 window ID
                0x80,                   // Object 1 flags
                0x1A, 0x2B,             // Object 1 X offset
                0x3C, 0x4D,             // Object 1 Y offset
                0xA1, 0xA2,             // Object 1 cropped X offset
                0xA3, 0xA4,             // Object 1 cropped Y offset
                0xA5, 0xA6,             // Object 1 cropped width
                0xA7, 0xA8,             // Object 1 cropped height
                0xAB, 0xCD,             // Object 2 object ID
                0xEF,                   // Object 2 window ID
                0x40,                   // Object 2 flags
                0x1A, 0x2B,             // Object 2 X offset
                0x3C, 0x4D,             // Object 2 Y offset
                0xAB, 0xCD,             // Object 3 object ID
                0xEF,                   // Object 3 window ID
                0xC0,                   // Object 3 flags
                0x1A, 0x2B,             // Object 3 X offset
                0x3C, 0x4D,             // Object 3 Y offset
                0xA1, 0xA2,             // Object 3 cropped X offset
                0xA3, 0xA4,             // Object 3 cropped Y offset
                0xA5, 0xA6,             // Object 3 cropped width
                0xA7, 0xA8,             // Object 3 cropped height
            }
        },
        {
            "es", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x80,                   // Type
                0x00, 0x00,             // Size
            }
        },
        {
            "iods-large-empty", new byte[]
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
            }
        },
        {
            "iods-large-small", new byte[]
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
            }
        },
        {
            "mods-empty", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x15,                   // Type
                0x00, 0x04,             // Size
                0xA0, 0xA1,             // ID
                0xA2,                   // Version
                0x00,                   // Sequence flags
            }
        },
        {
            "mods-small", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x15,                   // Type
                0x00, 0x08,             // Size
                0xA0, 0xA1,             // ID
                0xA2,                   // Version
                0x00,                   // Sequence flags
                0xE0, 0xE1, 0xE2, 0xE3, // Data
            }
        },
        {
            "fods-empty", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x15,                   // Type
                0x00, 0x04,             // Size
                0xA0, 0xA1,             // ID
                0xA2,                   // Version
                0x40,                   // Sequence flags
            }
        },
        {
            "fods-small", new byte[]
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
            }
        },
        {
            "pds-empty", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x14,                   // Type
                0x00, 0x02,             // Size
                0xA1,                   // ID
                0xA2,                   // Version
            }
        },
        {
            "pds-one", new byte[]
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
            }
        },
        {
            "pds-two", new byte[]
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
            }
        },
        {
            "sods-empty", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x15,                   // Type
                0x00, 0x0B,             // Size
                0xA0, 0xA1,             // ID
                0xA2,                   // Version
                0xC0,                   // Sequence flags
                0x00, 0x00, 0x04,       // Data length
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
            }
        },
        {
            "sods-small", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x15,                   // Type
                0x00, 0x0F,             // Size
                0xA0, 0xA1,             // ID
                0xA2,                   // Version
                0xC0,                   // Sequence flags
                0x00, 0x00, 0x08,       // Data length
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
                0xE0, 0xE1, 0xE2, 0xE3, // Data
            }
        },
        {
            "wds-empty", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x17,                   // Type
                0x00, 0x01,             // Size
                0x00,                   // Window count
            }
        },
        {
            "wds-one", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
                0x17,                   // Type
                0x00, 0x0A,             // Size
                0x01,                   // Window count
                0xEF,                   // ID
                0xA1, 0xB2,             // X offset
                0xC3, 0xD4,             // Y offset
                0x21, 0x43,             // Width
                0x65, 0x87,             // Height
            }
        },
        {
            "wds-two", new byte[]
            {
                0x50, 0x47,             // Magic number
                0x01, 0x23, 0x45, 0x67, // PTS
                0x12, 0x34, 0x56, 0x78, // DTS
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
            }
        },
    };
}
