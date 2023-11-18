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

using PGS4NET;

namespace PGS4NET.Tests;

public class RleTests
{
    private static readonly Exception ExceptionFailedException
        = new("Expected exception was not thrown.");

    [Fact]
    public void SingleLineEmptyBuffer()
    {
        var input = new byte[] { };
        var expected = new byte[] { };
        var received = Rle.Decompress(input, 0, 0);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 0, 0).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineOneZeroPixels()
    {
        var input = new byte[] { 0x00, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x00 };
        var received = Rle.Decompress(input, 1, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 1, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineSixtyThreeZeroPixels()
    {
        var input = new byte[] { 0x00, 0x3F, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x00, 63);
        var received = Rle.Decompress(input, 63, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 63, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineSixtyFourZeroPixels()
    {
        var input = new byte[] { 0x00, 0x40, 0x40, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x00, 64);
        var received = Rle.Decompress(input, 64, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 64, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineMaxZeroPixels()
    {
        var input = new byte[] { 0x00, 0x7F, 0xFF, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x00, 16_383);
        var received = Rle.Decompress(input, 16_383, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 16_383, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineOneValuePixel()
    {
        var input = new byte[] { 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x01 };
        var received = Rle.Decompress(input, 1, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 1, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineTwoValuePixels()
    {
        var input = new byte[] { 0x01, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x01, 0x01 };
        var received = Rle.Decompress(input, 2, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 2, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineThreeValuePixels()
    {
        var input = new byte[] { 0x00, 0x83, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x01, 0x01, 0x01 };
        var received = Rle.Decompress(input, 3, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 3, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineSixtyThreeValuePixels()
    {
        var input = new byte[] { 0x00, 0xBF, 0x01, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x01, 63);
        var received = Rle.Decompress(input, 63, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 63, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineSixtyFourValuePixels()
    {
        var input = new byte[] { 0x00, 0xC0, 0x40, 0x01, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x01, 64);
        var received = Rle.Decompress(input, 64, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 64, 1).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineMaxValuePixels()
    {
        var input = new byte[] { 0x00, 0xFF, 0xFF, 0x01, 0x00, 0x00 };
        var expected = NewPopulatedByteArray(0x01, 16_383);
        var received = Rle.Decompress(input, 16_383, 1);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 16_383, 1).SequenceEqual(input));
    }

    [Fact]
    public void TwoLinesOneMixedPixels()
    {
        var input = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00 };
        var expected = new byte[] { 0x00, 0x01 };
        var received = Rle.Decompress(input, 1, 2);

        Assert.True(received.SequenceEqual(expected));
        Assert.True(Rle.Compress(received, 1, 2).SequenceEqual(input));
    }

    [Fact]
    public void SingleLineZeroWidth()
    {
        var input = new byte[] { };

        try
        {
            Rle.Decompress(input, 0, 1);

            throw ExceptionFailedException;
        }
        catch (ArgumentException ae)
        {
            Assert.True(ae.Message
                == "The width and height parameters may not be zero unless both are zero.");
        }
    }

    [Fact]
    public void SingleLineZeroHeight()
    {
        var input = new byte[] { };

        try
        {
            Rle.Decompress(input, 1, 0);

            throw ExceptionFailedException;
        }
        catch (ArgumentException ae)
        {
            Assert.True(ae.Message
                == "The width and height parameters may not be zero unless both are zero.");
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
