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
    private static RleException IncompleteRleLine = new("Incomplete RLE line.");
    private static RleException IncompleteRleSequence = new("Incomplete RLE sequence.");
    private static RleException InvalidRleSequence = new("Invalid RLE sequence.");

    /// <summary>
    ///     Decompresses the provided byte sequence into an ordered list of lines.
    /// </summary>
    /// <param name="input">
    ///     The RLE-compressed byte sequence to decompress.
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
}
