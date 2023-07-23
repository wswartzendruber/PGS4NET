﻿/*
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
using PGS4NET.Segments;

namespace PGS4NET.Tests.Segments;

public class EndSegmentWriteTests
{
    [Fact]
    public void Single()
    {
        using (var stream = new MemoryStream())
        {
            var es = new EndSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
            };

            stream.WriteSegment(es);

            Assert.True(Enumerable.SequenceEqual(
                EndSegmentData.Single,
                stream.ToArray()
            ));
        }
    }

    [Fact]
    public void SingleAsync()
    {
        using (var stream = new MemoryStream())
        {
            var es = new EndSegment
            {
                Pts = 0x01234567,
                Dts = 0x12345678,
            };

            stream.WriteSegmentAsync(es).Wait();

            Assert.True(Enumerable.SequenceEqual(
                EndSegmentData.Single,
                stream.ToArray()
            ));
        }
    }
}
