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

namespace PGS4NET.Tests;

public class RleTests
{
    [Fact]
    public void SingleLineEmptyBuffer()
    {
        var input = new byte[] { };
        var expected = new byte[] { };
        var received = Rle.Decompress(input, 0, 0);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 0, 0));;
    }

    [Fact]
    public void SingleLineOneZeroPixels()
    {
        var input = new byte[] { 0x00, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x00 };
        var received = Rle.Decompress(input, 1, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 1, 1));;
    }

    [Fact]
    public void SingleLineSixtyThreeZeroPixels()
    {
        var input = new byte[] { 0x00, 0x3F, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x00, 63);
        var received = Rle.Decompress(input, 63, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 63, 1));
    }

    [Fact]
    public void SingleLineSixtyFourZeroPixels()
    {
        var input = new byte[] { 0x00, 0x40, 0x40, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x00, 64);
        var received = Rle.Decompress(input, 64, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 64, 1));
    }

    [Fact]
    public void SingleLineMaxZeroPixels()
    {
        var input = new byte[] { 0x00, 0x7F, 0xFF, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x00, 16_383);
        var received = Rle.Decompress(input, 16_383, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 16_383, 1));
    }

    [Fact]
    public void SingleLineOneValuePixel()
    {
        var input = new byte[] { 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x01 };
        var received = Rle.Decompress(input, 1, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 1, 1));
    }

    [Fact]
    public void SingleLineTwoValuePixels()
    {
        var input = new byte[] { 0x01, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x01, 0x01 };
        var received = Rle.Decompress(input, 2, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 2, 1));
    }

    [Fact]
    public void SingleLineThreeValuePixels()
    {
        var input = new byte[] { 0x00, 0x83, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x01, 0x01, 0x01 };
        var received = Rle.Decompress(input, 3, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 3, 1));
    }

    [Fact]
    public void SingleLineSixtyThreeValuePixels()
    {
        var input = new byte[] { 0x00, 0xBF, 0x01, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x01, 63);
        var received = Rle.Decompress(input, 63, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 63, 1));
    }

    [Fact]
    public void SingleLineSixtyFourValuePixels()
    {
        var input = new byte[] { 0x00, 0xC0, 0x40, 0x01, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x01, 64);
        var received = Rle.Decompress(input, 64, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 64, 1));
    }

    [Fact]
    public void SingleLineMaxValuePixels()
    {
        var input = new byte[] { 0x00, 0xFF, 0xFF, 0x01, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x01, 16_383);
        var received = Rle.Decompress(input, 16_383, 1);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 16_383, 1));
    }

    [Fact]
    public void TwoLinesOneMixedPixels()
    {
        var input = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x00, 0x01 };
        var received = Rle.Decompress(input, 1, 2);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 1, 2));
    }

    [Fact]
    public void TwoLinesSixtyThreeMixedPixels()
    {
        var input = new byte[] { 0x00, 0x3F, 0x00, 0x00, 0x00, 0xBF, 0x01, 0x00, 0x00 };
        var expected1 = NewPopulatedByteArray(0x00, 63);
        var expected2 = NewPopulatedByteArray(0x01, 63);
        var expected = expected1.Concat(expected2).ToArray();
        var received = Rle.Decompress(input, 63, 2);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 63, 2));
    }

    [Fact]
    public void TwoLinesSixtyFourMixedPixels()
    {
        var input = new byte[]
        {
            0x00, 0x40, 0x40, 0x00, 0x00, 0x00, 0xC0, 0x40, 0x01, 0x00, 0x00,
        };
        var expected1 = NewPopulatedByteArray(0x00, 64);
        var expected2 = NewPopulatedByteArray(0x01, 64);
        var expected = expected1.Concat(expected2).ToArray();
        var received = Rle.Decompress(input, 64, 2);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 64, 2));
    }

    [Fact]
    public void TwoLinesMaxMixedPixels()
    {
        var input = new byte[]
        {
            0x00, 0x7F, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x01, 0x00, 0x00,
        };
        var expected1 = NewPopulatedByteArray(0x00, 16_383);
        var expected2 = NewPopulatedByteArray(0x01, 16_383);
        var expected = expected1.Concat(expected2).ToArray();
        var received = Rle.Decompress(input, 16_383, 2);

        Assert.Equal(expected, received);
        Assert.Equal(input, Rle.Compress(received, 16_383, 2));
    }

    [Fact]
    public void SingleLineZeroWidth()
    {
        var input = new byte[] { };

        try
        {
            Rle.Decompress(input, 0, 1);

            Assert.Fail("Was able to compress a single line with zero width.");
        }
        catch (ArgumentException ae)
        {
            Assert.Equal(
                "The width and height parameters may not be zero unless both are zero."
                , ae.Message);
        }
    }

    [Fact]
    public void SingleLineZeroHeight()
    {
        var input = new byte[] { };

        try
        {
            Rle.Decompress(input, 1, 0);

            Assert.Fail("Was able to compress a single line with zero height.");
        }
        catch (ArgumentException ae)
        {
            Assert.Equal(
                "The width and height parameters may not be zero unless both are zero."
                , ae.Message);
        }
    }

    private byte[] NewPopulatedByteArray(byte value, int count)
    {
        var returnValue = new byte[count];

        for (int i = 0; i < returnValue.Length; i++)
            returnValue[i] = value;

        return returnValue;
    }
}
