/*
 * Copyright 2024 William Swartzendruber
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
                Objects = new Dictionary<VersionedId<int>, DisplayObject>(),
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
                        new DisplayWindow(RandomUInt16(), RandomUInt16(), RandomUInt16()
                            , RandomUInt16())
                    },
                    {
                        (byte)(RandomByte(64) + 64),
                        new DisplayWindow(RandomUInt16(), RandomUInt16(), RandomUInt16()
                            , RandomUInt16())
                    },
                    {
                        (byte)(RandomByte(64) + 128),
                        new DisplayWindow(RandomUInt16(), RandomUInt16(), RandomUInt16()
                            , RandomUInt16())
                    },
                },
                Palettes = new Dictionary<VersionedId<byte>, DisplayPalette>
                {
                    {
                        new VersionedId<byte>(RandomByte(64), RandomByte()),
                        new DisplayPalette
                        {
                            Entries = new Dictionary<byte, YcbcraPixel>
                            {
                            },
                        }
                    },
                    {
                        new VersionedId<byte>((byte)(RandomByte(64) + 64), RandomByte()),
                        new DisplayPalette
                        {
                            Entries = new Dictionary<byte, YcbcraPixel>
                            {
                                {
                                    RandomByte(),
                                    new YcbcraPixel(RandomByte(), RandomByte(), RandomByte()
                                        , RandomByte())
                                },
                            },
                        }
                    },
                    {
                        new VersionedId<byte>((byte)(RandomByte(64) + 128), RandomByte()),
                        new DisplayPalette
                        {
                            Entries = new Dictionary<byte, YcbcraPixel>
                            {
                                {
                                    RandomByte(64),
                                    new YcbcraPixel(RandomByte(), RandomByte(), RandomByte()
                                        , RandomByte())
                                },
                                {
                                    (byte)(RandomByte(64) + 64),
                                    new YcbcraPixel(RandomByte(), RandomByte(), RandomByte()
                                        , RandomByte())
                                },
                                {
                                    (byte)(RandomByte(64) + 128),
                                    new YcbcraPixel(RandomByte(), RandomByte(), RandomByte()
                                        , RandomByte())
                                },
                            },
                        }
                    },
                },
                Objects = new Dictionary<VersionedId<int>, DisplayObject>
                {
                    {
                        new VersionedId<int>(RandomUInt16(128), RandomByte()),
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<int>(RandomUInt16(128) + 128
                            , RandomByte()),
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<int>(RandomUInt16(128) + 256
                            , RandomByte()),
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<int>(RandomUInt16(128) + 384
                            , RandomByte()),
                        RandomDisplayObject()
                    },
                    {
                        new VersionedId<int>(RandomUInt16(128) + 512
                            , RandomByte()),
                        RandomDisplayObject()
                    },
                },
                CompositionNumber = RandomUInt16(),
                CompositionState = RandomCompositionState(),
                Compositions = new Dictionary<CompositionId, DisplayComposition>
                {
                    {
                        new CompositionId(RandomUInt16(128), RandomByte()),
                        new DisplayComposition
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Forced = false,
                            Crop = null,
                        }
                    },
                    {
                        new CompositionId(RandomUInt16(128) + 128, RandomByte()),
                        new DisplayComposition
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Forced = false,
                            Crop = new Crop(RandomUInt16(), RandomUInt16(), RandomUInt16()
                                , RandomUInt16()),
                        }
                    },
                    {
                        new CompositionId((RandomUInt16(128) + 256), RandomByte()),
                        new DisplayComposition
                        {
                            X = RandomUInt16(),
                            Y = RandomUInt16(),
                            Forced = true,
                            Crop = new Crop(RandomUInt16(), RandomUInt16(), RandomUInt16()
                                , RandomUInt16()),
                        }
                    },
                },
            }
        },
    };

    private static byte RandomByte(byte max = byte.MaxValue) => (byte)Rng.Next(max);

    private static int RandomUInt16(int max = int.MaxValue) => Rng.Next(max);

    private static long RandomUInt32() => Rng.Next();

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
        var width = (int)(RandomUInt16(99) + 1);
        var height = (int)(RandomUInt16(99) + 1);
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
