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

using System.Collections.Generic;
using System.IO;
using PGS4NET.Segment;

namespace PGS4NET.Tests.Segment;

public class WindowDefinitionSegmentWriteTests
{
    [Fact]
    public void NoWindows()
    {
        using (var stream = new MemoryStream())
        {
            var wds = new WindowDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
            };

            stream.WriteSegment(wds);

            Assert.True(Enumerable.SequenceEqual(
                WindowDefinitionSegmentData.NoWindows,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void NoWindowsAsync()
    {
        using (var stream = new MemoryStream())
        {
            var wds = new WindowDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
            };

            stream.WriteSegmentAsync(wds).Wait();

            Assert.True(Enumerable.SequenceEqual(
                WindowDefinitionSegmentData.NoWindows,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void OneWindow()
    {
        using (var stream = new MemoryStream())
        {
            var wds = new WindowDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Definitions = new List<WindowDefinition>
                {
                    new WindowDefinition
                    {
                        ID = 0xEF,
                        X = 0xA1B2,
                        Y = 0xC3D4,
                        Width = 0x2143,
                        Height = 0x6587,
                    },
                },
            };

            stream.WriteSegment(wds);

            Assert.True(Enumerable.SequenceEqual(
                WindowDefinitionSegmentData.OneWindow,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void OneWindowAsync()
    {
        using (var stream = new MemoryStream())
        {
            var wds = new WindowDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Definitions = new List<WindowDefinition>
                {
                    new WindowDefinition
                    {
                        ID = 0xEF,
                        X = 0xA1B2,
                        Y = 0xC3D4,
                        Width = 0x2143,
                        Height = 0x6587,
                    },
                },
            };

            stream.WriteSegmentAsync(wds).Wait();

            Assert.True(Enumerable.SequenceEqual(
                WindowDefinitionSegmentData.OneWindow,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void TwoWindows()
    {
        using (var stream = new MemoryStream())
        {
            var wds = new WindowDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Definitions = new List<WindowDefinition>
                {
                    new WindowDefinition
                    {
                        ID = 0xEF,
                        X = 0xA1B2,
                        Y = 0xC3D4,
                        Width = 0x2143,
                        Height = 0x6587,
                    },
                    new WindowDefinition
                    {
                        ID = 0xFE,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Width = 0x1234,
                        Height = 0x5678,
                    },
                },
            };

            stream.WriteSegment(wds);

            Assert.True(Enumerable.SequenceEqual(
                WindowDefinitionSegmentData.TwoWindows,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void TwoWindowsAsync()
    {
        using (var stream = new MemoryStream())
        {
            var wds = new WindowDefinitionSegment
            {
                PTS = 0x01234567,
                DTS = 0x12345678,
                Definitions = new List<WindowDefinition>
                {
                    new WindowDefinition
                    {
                        ID = 0xEF,
                        X = 0xA1B2,
                        Y = 0xC3D4,
                        Width = 0x2143,
                        Height = 0x6587,
                    },
                    new WindowDefinition
                    {
                        ID = 0xFE,
                        X = 0x1A2B,
                        Y = 0x3C4D,
                        Width = 0x1234,
                        Height = 0x5678,
                    },
                },
            };

            stream.WriteSegmentAsync(wds).Wait();

            Assert.True(Enumerable.SequenceEqual(
                WindowDefinitionSegmentData.TwoWindows,
                stream.ToArray()
            ));
        }
    }
}
