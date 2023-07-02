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

if (args.Length != 1)
    throw new ArgumentException("A single parameter with a PGS file must be passed.");

using var pgsStream = new FileStream(args[0], FileMode.Open);

while (pgsStream.Position < pgsStream.Length)
{
    var segment = pgsStream.ReadSegment();

    switch (segment)
    {
        case PresentationCompositionSegment:
            Console.WriteLine("PCS");
            break;
        case WindowDefinitionSegment:
            Console.WriteLine("WDS");
            break;
        case PaletteDefinitionSegment:
            Console.WriteLine("PDS");
            break;
        case SingleObjectDefinitionSegment:
            Console.WriteLine("S-ODS");
            break;
        case InitialObjectDefinitionSegment:
            Console.WriteLine("I-ODS");
            break;
        case MiddleObjectDefinitionSegment:
            Console.WriteLine("M-ODS");
            break;
        case FinalObjectDefinitionSegment:
            Console.WriteLine("F-ODS");
            break;
        case EndSegment:
            Console.WriteLine("ES");
            break;
        default:
            throw new InvalidOperationException("Segment is of unhandled type.");
    }
}
