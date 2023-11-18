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

using PGS4NET.DisplaySets;

namespace PGS4NET.Tests.DisplaySets;

internal static class DisplaySetInstances
{
    private const int UInt24Max = 16_777_216;

    private static readonly Random Rng = new(1_024);

    internal static readonly Dictionary<string, DisplaySet> Instances = new()
    {
        {
            "defaults", new DisplaySet
            {
            }
        },
        {
            "empty", new DisplaySet
            {
                Pts = RandomUInt32(),
                Dts = RandomUInt32(),
                Width = RandomUInt16(),
                Height = RandomUInt16(),
                FrameRate = RandomByte(),
                PaletteUpdateOnly = false,
                PaletteId = RandomByte(),
                Windows = new Dictionary<byte, DisplayWindow>(),
                Palettes = new Dictionary<VersionedId<byte>, DisplayPalette>(),
                Objects = new Dictionary<VersionedId<ushort>, DisplayObject>(),
                CompositionNumber = RandomUInt16(),
                CompositionState = RandomCompositionState(),
                Compositions = new Dictionary<CompositionId, DisplayComposition>(),
            }
        },
        {
            "not-empty", new DisplaySet
            {
                Pts = RandomUInt32(),
                Dts = RandomUInt32(),
                Width = RandomUInt16(),
                Height = RandomUInt16(),
                FrameRate = RandomByte(),
                PaletteUpdateOnly = true,
                PaletteId = RandomByte(),
                Windows = new Dictionary<byte, DisplayWindow>
                {
                    {
                        RandomByte(64),
                        new DisplayWindow
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Width = RandomUInt16(),
                            Height = RandomUInt16(),
                        }
                    },
                    {
                        (byte)(RandomByte(64) + 64),
                        new DisplayWindow
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Width = RandomUInt16(),
                            Height = RandomUInt16(),
                        }
                    },
                    {
                        (byte)(RandomByte(64) + 128),
                        new DisplayWindow
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Width = RandomUInt16(),
                            Height = RandomUInt16(),
                        }
                    },
                },
                Palettes = new Dictionary<VersionedId<byte>, DisplayPalette>
                {
                    {
                        new VersionedId<byte>
                        {
                            Id = RandomByte(64),
                            Version = RandomByte(),
                        },
                        new DisplayPalette
                        {
                            Entries = new Dictionary<byte, DisplayPaletteEntry>
                            {
                            },
                        }
                    },
                    {
                        new VersionedId<byte>
                        {
                            Id = (byte)(RandomByte(64) + 64),
                            Version = RandomByte(),
                        },
                        new DisplayPalette
                        {
                            Entries = new Dictionary<byte, DisplayPaletteEntry>
                            {
                                {
                                    RandomByte(),
                                    new DisplayPaletteEntry
                                    {
                                        Y = RandomByte(),
                                        Cr = RandomByte(),
                                        Cb = RandomByte(),
                                        Alpha = RandomByte(),
                                    }
                                },
                            },
                        }
                    },
                    {
                        new VersionedId<byte>
                        {
                            Id = (byte)(RandomByte(64) + 128),
                            Version = RandomByte(),
                        },
                        new DisplayPalette
                        {
                            Entries = new Dictionary<byte, DisplayPaletteEntry>
                            {
                                {
                                    RandomByte(64),
                                    new DisplayPaletteEntry
                                    {
                                        Y = RandomByte(),
                                        Cr = RandomByte(),
                                        Cb = RandomByte(),
                                        Alpha = RandomByte(),
                                    }
                                },
                                {
                                    (byte)(RandomByte(64) + 64),
                                    new DisplayPaletteEntry
                                    {
                                        Y = RandomByte(),
                                        Cr = RandomByte(),
                                        Cb = RandomByte(),
                                        Alpha = RandomByte(),
                                    }
                                },
                                {
                                    (byte)(RandomByte(64) + 128),
                                    new DisplayPaletteEntry
                                    {
                                        Y = RandomByte(),
                                        Cr = RandomByte(),
                                        Cb = RandomByte(),
                                        Alpha = RandomByte(),
                                    }
                                },
                            },
                        }
                    },
                },
                Objects = new Dictionary<VersionedId<ushort>, DisplayObject>
                {
                    {
                        new VersionedId<ushort>
                        {
                            Id = RandomUInt16(128),
                            Version = RandomByte(),
                        },
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<ushort>
                        {
                            Id = (ushort)(RandomUInt16(128) + 128),
                            Version = RandomByte(),
                        },
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<ushort>
                        {
                            Id = (ushort)(RandomUInt16(128) + 256),
                            Version = RandomByte(),
                        },
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<ushort>
                        {
                            Id = (ushort)(RandomUInt16(128) + 384),
                            Version = RandomByte(),
                        },
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<ushort>
                        {
                            Id = (ushort)(RandomUInt16(128) + 512),
                            Version = RandomByte(),
                        },
                        RandomDisplayObject()
                    },
                },
                CompositionNumber = RandomUInt16(),
                CompositionState = RandomCompositionState(),
                Compositions = new Dictionary<CompositionId, DisplayComposition>
                {
                    {
                        new CompositionId
                        {
                            ObjectId = RandomUInt16(128),
                            WindowId = RandomByte(),
                        },
                        new DisplayComposition
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Forced = false,
                            Crop = null,
                        }
                    },
                    {
                        new CompositionId
                        {
                            ObjectId = (ushort)(RandomUInt16(128) + 128),
                            WindowId = RandomByte(),
                        },
                        new DisplayComposition
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Forced = false,
                            Crop = new CroppedArea
                            {
                                X = RandomUInt16(),
                                Y = RandomUInt16(),
                                Width = RandomUInt16(),
                                Height = RandomUInt16(),
                            },
                        }
                    },
                    {
                        new CompositionId
                        {
                            ObjectId = (ushort)(RandomUInt16(128) + 256),
                            WindowId = RandomByte(),
                        },
                        new DisplayComposition
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Forced = true,
                            Crop = new CroppedArea
                            {
                                X = RandomUInt16(),
                                Y = RandomUInt16(),
                                Width = RandomUInt16(),
                                Height = RandomUInt16(),
                            },
                        }
                    },
                },
            }
        },
    };

    private static byte RandomByte(byte max = byte.MaxValue) => (byte)Rng.Next(max);

    private static ushort RandomUInt16(ushort max = ushort.MaxValue) => (ushort)Rng.Next(max);

    private static uint RandomUInt24(uint max = UInt24Max) =>
        max > UInt24Max
            ? throw new ArgumentOutOfRangeException("RandomUInt24(max) is limited to 24 bits.")
            : (uint)Rng.NextInt64(max);

    private static uint RandomUInt32(uint max = uint.MaxValue) => (uint)Rng.NextInt64(max);

    private static CompositionState RandomCompositionState()
    {
        switch (Rng.Next() % 3)
        {
            case 0:
                return CompositionState.EpochStart;
            case 1:
                return CompositionState.AcquisitionPoint;
            case 2:
                return CompositionState.Normal;
            default:
                throw new InvalidOperationException();
        }
    }

    private static DisplayObject RandomDisplayObject()
    {
        var width = (ushort)(RandomUInt16(99) + 1);
        var height = (ushort)(RandomUInt16(99) + 1);
        var max = UInt24Max / 2;
        var count = 32;
        var eachMax = max / count;
        var data = new byte[width * height];

        Rng.NextBytes(data);

        return new DisplayObject
        {
            Width = width,
            Height = height,
            Data = data,
        };
    }
}
