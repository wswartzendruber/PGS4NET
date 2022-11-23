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

using System.IO;
using PGS4NET;

public class PCSWriteTests
{
    [Fact]
    public void EpochStart_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.EpochStart,
                PaletteUpdateOnly = false,
                PaletteUpdateID = 0xAB,
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.EpochStart_NoPaletteUpdateID_NoObjects,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void AcquisitionPoint_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.AcquisitionPoint,
                PaletteUpdateOnly = false,
                PaletteUpdateID = 0xAB,
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.AcquisitionPoint_NoPaletteUpdateID_NoObjects,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.Normal,
                PaletteUpdateOnly = false,
                PaletteUpdateID = 0xAB,
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.Normal_NoPaletteUpdateID_NoObjects,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void Normal_PaletteUpdateID_NoObjects()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.Normal,
                PaletteUpdateOnly = true,
                PaletteUpdateID = 0xAB,
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.Normal_PaletteUpdateID_NoObjects,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_OneObjectForced()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.Normal,
                PaletteUpdateOnly = false,
                PaletteUpdateID = 0xAB,
                Objects = new List<CompositionObject>
                {
                    new CompositionObject
                    {
                        ObjectID = 0xABCD,
                        WindowID = 0xEF,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Forced = true,
                        Crop = null,
                    },
                },
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.Normal_NoPaletteUpdateID_OneObjectForced,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_OneObjectCropped()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.Normal,
                PaletteUpdateOnly = false,
                PaletteUpdateID = 0xAB,
                Objects = new List<CompositionObject>
                {
                    new CompositionObject
                    {
                        ObjectID = 0xABCD,
                        WindowID = 0xEF,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Forced = false,
                        Crop = new CroppedArea
                        {
                            X = 0xA1A2,
                            Y = 0xA3A4,
                            Width = 0xA5A6,
                            Height = 0xA7A8,
                        },
                    },
                },
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.Normal_NoPaletteUpdateID_OneObjectCropped,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void Normal_NoPaletteUpdateID_ThreeObjectsMixed()
    {
        using (var stream = new MemoryStream())
        {
            var pcs = new PresentationCompositionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Width = 0x2143,
                Height = 0x6587,
                FrameRate = 0x10,
                Number = 0x6543,
                State = CompositionState.Normal,
                PaletteUpdateOnly = false,
                PaletteUpdateID = 0xAB,
                Objects = new List<CompositionObject>
                {
                    new CompositionObject
                    {
                        ObjectID = 0xABCD,
                        WindowID = 0xEF,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Forced = false,
                        Crop = new CroppedArea
                        {
                            X = 0xA1A2,
                            Y = 0xA3A4,
                            Width = 0xA5A6,
                            Height = 0xA7A8,
                        },
                    },
                    new CompositionObject
                    {
                        ObjectID = 0xABCD,
                        WindowID = 0xEF,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Forced = true,
                        Crop = null,
                    },
                    new CompositionObject
                    {
                        ObjectID = 0xABCD,
                        WindowID = 0xEF,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Forced = true,
                        Crop = new CroppedArea
                        {
                            X = 0xA1A2,
                            Y = 0xA3A4,
                            Width = 0xA5A6,
                            Height = 0xA7A8,
                        },
                    },
                },
            };

            stream.WriteSegment(pcs);

            Assert.True(Enumerable.SequenceEqual(
                PCS.Normal_NoPaletteUpdateID_ThreeObjectsMixed,
                stream.ToArray()
            ));
        }
    }
}
