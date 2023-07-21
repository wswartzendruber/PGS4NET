/*
 * Copyright 2023 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;

namespace PGS4NET;

/// <summary>
///     Implements RLE compression and decompression as defined by PGS.
/// </summary>
public static class Rle
{
    // COMPRESSION
    private static RleException LineTooLong = new("Line too long.");

    // DECOMPRESSION
    private static RleException IncompleteRleLine = new("Incomplete RLE line.");
    private static RleException IncompleteRleSequence = new("Incomplete RLE sequence.");
    private static RleException InvalidRleSequence = new("Invalid RLE sequence.");

    /// <summary>
    ///     Compresses the provided ordered list of lines into a byte sequence.
    /// </summary>
    /// <param name="input">
    ///     An ordered list of lines, each one of which contains an ordered list of palette
    ///     entries defined elsewhere.
    /// </param>
    /// <returns>
    ///     An RLE-compressed byte sequence.
    /// </returns>
    public static IList<byte> Compress(IList<IList<byte>> input)
    {
        var output = new List<byte>();
        byte value = 0x00;
        uint count = 0;

        foreach (var line in input)
        {
            foreach (var nextValue in line)
            {
                if (nextValue == value)
                {
                    count += 1;
                }
                else
                {
                    if (count > 0)
                        OutputRleSequence(output, value, count);
                    value = nextValue;
                    count = 1;
                }
            }

            OutputRleSequence(output, value, count);
            value = 0;
            count = 0;

            output.Add(0x00);
            output.Add(0x00);
        }

        return output;
    }

    /// <summary>
    ///     Decompresses the provided byte sequence into an ordered list of lines.
    /// </summary>
    /// <param name="input">
    ///     An RLE-compressed byte sequence.
    /// </param>
    /// <returns>
    ///     An ordered list of lines, each one of which contains an ordered list of palette
    ///     entries defined elsewhere.
    /// </returns>
    /// <exception cref="PGS4NET.RleException">
    ///     <list type="bullet">
    ///         <item>An invalid RLE sequence is encountered.</item>
    ///         <item>An incomplete RLE sequence is encountered.</item>
    ///         <item>An incomplete RLE line is encountered.</item>
    ///     </list>
    /// </exception>
    public static IList<IList<byte>> Decompress(IEnumerable<byte> input)
    {
        var output = new List<IList<byte>>();
        var line = new List<byte>();
        var enumerator = input.GetEnumerator();

        while (true)
        {
            if (enumerator.MoveNext())
            {
                var byte1 = enumerator.Current;

                if (byte1 == 0x00)
                {
                    if (enumerator.MoveNext())
                    {
                        var byte2 = enumerator.Current;
                        var flag = byte2 >> 6;

                        if (byte2 == 0x00)
                        {
                            output.Add(line);
                            line = new List<byte>();
                        }
                        else if (flag == 0)
                        {
                            for (byte i = 0x00; i < (byte2 & 0x3F); i++)
                                line.Add(0);
                        }
                        else if (flag == 1)
                        {
                            if (enumerator.MoveNext())
                            {
                                var byte3 = enumerator.Current;
                                var limit = ((ushort)byte2 & 0x3F) << 8 | (ushort)byte3;

                                for (var i = 0; i < limit; i++)
                                    line.Add(0);
                            }
                            else
                            {
                                throw IncompleteRleSequence;
                            }
                        }
                        else if (flag == 2)
                        {
                            if (enumerator.MoveNext())
                            {
                                var byte3 = enumerator.Current;

                                for (byte i = 0x00; i < (byte2 & 0x3F); i++)
                                    line.Add(byte3);
                            }
                            else
                            {
                                throw IncompleteRleSequence;
                            }
                        }
                        else if (flag == 3)
                        {
                            if (enumerator.MoveNext())
                            {
                                var byte3 = enumerator.Current;
                                var limit = ((ushort)byte2 & 0x3F) << 8 | (ushort)byte3;

                                if (enumerator.MoveNext())
                                {
                                    var byte4 = enumerator.Current;

                                    for (var i = 0; i < limit; i++)
                                        line.Add(byte4);
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
                    line.Add(byte1);
                }
            }
            else
            {
                break;
            }
        }

        if (line.Count > 0)
            throw IncompleteRleLine;

        return output;
    }

    private static void OutputRleSequence(List<byte> output, byte value, uint count)
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
