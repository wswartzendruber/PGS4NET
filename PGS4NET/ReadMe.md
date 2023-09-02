<!--
    Copyright 2023 William Swartzendruber

    This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
    copy of the MPL was not distributed with this file, You can obtain one at
    https://mozilla.org/MPL/2.0/.

    SPDX-License-Identifier: MPL-2.0
-->

# PGS4NET

## Introduction

The purpose of this library is to someday facilitate intuitive interaction with Presentation
Graphics Stream (PGS) subtitles, often times referred to as SUP subtitles due to their file
extension.

In its current, early state, it supports reading and writing both display sets and segments.
Epoch composition is not yet implemented.

## Examples

### Display Sets

Reading display sets is illustrated below as seen in the `PGS4NET.Examples.DisplaySetDump`
project:

```csharp
using PGS4NET.DisplaySets;

if (args.Length != 1)
    throw new ArgumentException("A single parameter with a PGS file must be passed.");

using var stream = new FileStream(args[0], FileMode.Open);

while (stream.ReadDisplaySet() is DisplaySet displaySet)
{
    Console.WriteLine($"Read display set with PTS = {displaySet.Pts}");
}
```

Display sets can be written out to streams via the `WriteDisplaySet` extension method on the
`System.IO.Stream` class.

### Segments

Reading segments is illustrated below as seen in the `PGS4NET.Examples.SegmentDump` project:

```csharp
using PGS4NET.Segments;

if (args.Length != 1)
    throw new ArgumentException("A single parameter with a PGS file must be passed.");

using var stream = new FileStream(args[0], FileMode.Open);

while (stream.ReadSegment() is Segment segment)
{
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
```

Segments can be written out to streams via the `WriteSegment` extension method on the
`System.IO.Stream` class.
