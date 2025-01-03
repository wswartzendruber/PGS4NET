/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System;
using System.Collections.Generic;

namespace PGS4NET;

/// <summary>
///     Implements RLE compression and decompression as defined by PGS.
/// </summary>
public static class Rle
{
    // COMPRESSION
    private static readonly RleException LineTooLong
        = new("RLE line is too long.");

    // DECOMPRESSION
    private static readonly RleException IncompleteRleLine
        = new("Incomplete RLE line.");
    private static readonly RleException InvalidRleLineLength
        = new("Invalid RLE line length.");
    private static readonly RleException IncompleteRleSequence
        = new("Incomplete RLE sequence.");
    private static readonly RleException InvalidRleSequence
        = new("Invalid RLE sequence.");

    /// <summary>
    ///     Compresses the provided object data into a buffer using the PGS RLE algorithm.
    /// </summary>
    /// <param name="input">
    ///     An ordered list of object pixel data where each byte addresses a palette entry
    ///     during playback. The length of the input must be the product of the
    ///     <paramref name="width"/> and the <paramref name="height"/>.
    /// </param>
    /// <param name="width">
    ///     The width of the object in pixels.
    /// </param>
    /// <param name="height">
    ///     The height of the object in pixels.
    /// </param>
    /// <returns>
    ///     An RLE-compressed byte sequence.
    /// </returns>
    public static byte[] Compress(byte[] input, int width, int height)
    {
        if ((width == 0) ^ (height == 0))
        {
            throw new ArgumentException(
                "The width and height parameters may not be zero unless both are zero."
            );
        }
        if (input.Length != width * height)
        {
            throw new ArgumentException(
                "Input length is not the product of the width and the height."
            );
        }

        var output = new List<byte>();
        byte value = 0x00;
        long count = 0;

        for (long x = 0; x < input.Length; x += width)
        {
            var end = x + width;

            for (long i = x; i < end; i++)
            {
                var current = input[i];

                if (current == value)
                {
                    count += 1;
                }
                else
                {
                    if (count > 0)
                        OutputRleSequence(output, value, count);
                    value = current;
                    count = 1;
                }
            }

            OutputRleSequence(output, value, count);
            value = 0;
            count = 0;

            output.Add(0x00);
            output.Add(0x00);
        }

        return output.ToArray();
    }

    /// <summary>
    ///     Decompresses the provided buffer into object data using the PGS RLE algorithm.
    /// </summary>
    /// <param name="input">
    ///     An RLE-compressed byte sequence.
    /// </param>
    /// <param name="width">
    ///     The width of the object in pixels.
    /// </param>
    /// <param name="height">
    ///     The height of the object in pixels.
    /// </param>
    /// <returns>
    ///     An ordered list of object pixel data where each byte addresses a palette entry
    ///     during playback. The length of the input will be the product of the
    ///     <see name="width"/> and the <paramref name="height"/>.
    /// </returns>
    /// <exception cref="RleException">
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 An invalid RLE sequence is encountered.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 An incomplete RLE sequence is encountered.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 An incomplete RLE line is encountered.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 An RLE line has a length other than <paramref name="width"/>.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 The number of RLE lines contradicts <paramref name="height"/>.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </exception>
    public static byte[] Decompress(byte[] input, int width, int height)
    {
        if ((width == 0) ^ (height == 0))
        {
            throw new ArgumentException(
                "The width and height parameters may not be zero unless both are zero."
            );
        }

        var inIndex = 0;
        var outIndex = 0;
        var output = new byte[width * height];
        var lineOpen = false;

        while (true)
        {
            if (inIndex < input.Length)
            {
                var byte1 = input[inIndex++];

                lineOpen = true;

                if (byte1 == 0x00)
                {
                    if (inIndex < input.Length)
                    {
                        var byte2 = input[inIndex++];
                        var flag = byte2 >> 6;

                        if (byte2 == 0x00)
                        {
                            if (outIndex % width != 0)
                                throw InvalidRleLineLength;

                            lineOpen = false;
                        }
                        else if (flag == 0)
                        {
                            for (byte i = 0x00; i < (byte2 & 0x3F); i++)
                                output[outIndex++] = 0x00;
                        }
                        else if (flag == 1)
                        {
                            if (inIndex < input.Length)
                            {
                                var byte3 = input[inIndex++];
                                var limit = (byte2 & 0x3F) << 8 | byte3;

                                for (var i = 0; i < limit; i++)
                                    output[outIndex++] = 0x00;
                            }
                            else
                            {
                                throw IncompleteRleSequence;
                            }
                        }
                        else if (flag == 2)
                        {
                            if (inIndex < input.Length)
                            {
                                var byte3 = input[inIndex++];

                                for (byte i = 0x00; i < (byte2 & 0x3F); i++)
                                    output[outIndex++] = byte3;
                            }
                            else
                            {
                                throw IncompleteRleSequence;
                            }
                        }
                        else if (flag == 3)
                        {
                            if (inIndex < input.Length)
                            {
                                var byte3 = input[inIndex++];
                                var limit = (byte2 & 0x3F) << 8 | byte3;

                                if (inIndex < input.Length)
                                {
                                    var byte4 = input[inIndex++];

                                    for (var i = 0; i < limit; i++)
                                        output[outIndex++] = byte4;
                                }
                                else
                                {
                                    throw IncompleteRleSequence;
                                }
                            }
                            else
                            {
                                throw IncompleteRleSequence;
                            }
                        }
                        else
                        {
                            throw InvalidRleSequence;
                        }
                    }
                    else
                    {
                        throw IncompleteRleSequence;
                    }
                }
                else
                {
                    output[outIndex++] = byte1;
                }
            }
            else
            {
                break;
            }
        }

        if (lineOpen || outIndex != output.Length)
            throw IncompleteRleLine;

        return output;
    }

    private static void OutputRleSequence(List<byte> output, byte value, long count)
    {
        if (value == 0x00)
        {
            switch (count)
            {
                case 0:
                    break;
                case < 64:
                    output.Add(0x00);
                    output.Add((byte)count);
                    break;
                case < 16_384:
                    output.Add(0x00);
                    output.Add((byte)(0x40 | count >> 8));
                    output.Add((byte)(count & 0xFF));
                    break;
                default:
                    throw LineTooLong;
            }
        }
        else
        {
            switch (count)
            {
                case 0:
                    break;
                case 1:
                    output.Add(value);
                    break;
                case 2:
                    output.Add(value);
                    output.Add(value);
                    break;
                case < 64:
                    output.Add(0x00);
                    output.Add((byte)(0x80 | count));
                    output.Add(value);
                    break;
                case < 16_384:
                    output.Add(0x00);
                    output.Add((byte)(0xC0 | count >> 8));
                    output.Add((byte)(count & 0xFF));
                    output.Add(value);
                    break;
                default:
                    throw LineTooLong;
            }
        }
    }
}
