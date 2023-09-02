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

if (args.Length != 1)
    throw new ArgumentException("A single parameter with a PGS file must be passed.");

using var stream = new FileStream(args[0], FileMode.Open);

while (stream.ReadDisplaySet() is DisplaySet displaySet)
{
    Console.WriteLine($"Read display set with PTS = {displaySet.Pts}");
}
