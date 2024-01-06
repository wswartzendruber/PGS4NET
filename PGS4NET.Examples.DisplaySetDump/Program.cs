/*
 * Copyright 2024 William Swartzendruber
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
using PGS4NET.DisplaySets;

if (args.Length != 1)
    throw new ArgumentException("A single parameter with a PGS file must be passed.");

using var stream = new FileStream(args[0], FileMode.Open);

while (stream.ReadDisplaySet() is DisplaySet displaySet)
{
    Console.WriteLine("Display Set");
    Console.WriteLine($"├──PTS: {TsToTimeStamp(displaySet.Pts)}");
    Console.WriteLine($"├──DTS: {TsToTimeStamp(displaySet.Dts)}");
    Console.WriteLine($"├──Width: {displaySet.Width}");
    Console.WriteLine($"├──Height: {displaySet.Height}");
    Console.WriteLine($"├──Frame Rate: {displaySet.FrameRate}");
    Console.WriteLine($"├──Palette Update Only: {displaySet.PaletteUpdateOnly}");
    Console.WriteLine($"├──Palette ID: {displaySet.PaletteId}");
    Console.WriteLine($"├──Composition Number: {displaySet.CompositionNumber}");
    Console.WriteLine($"├──Composition State: {displaySet.CompositionState}");
    Console.WriteLine("├──Windows");

    foreach (var window in displaySet.Windows)
    {
        Console.WriteLine("├────Window");
        Console.WriteLine($"├──────ID: {(int)window.Key}");
        Console.WriteLine($"├──────X: {window.Value.X}");
        Console.WriteLine($"├──────Y: {window.Value.Y}");
        Console.WriteLine($"├──────Width: {window.Value.Width}");
        Console.WriteLine($"├──────Height: {window.Value.Height}");
    }

    Console.WriteLine("├──Palettes");

    foreach (var palette in displaySet.Palettes)
    {
        Console.WriteLine("├────Palette");
        Console.WriteLine($"├──────ID: {(int)palette.Key.Id}");
        Console.WriteLine($"├──────Version: {(int)palette.Key.Version}");
        Console.WriteLine($"├──────Entries: {palette.Value.Entries.Count}");
    }

    Console.WriteLine("├──Objects");

    foreach (var object_ in displaySet.Objects)
    {
        Console.WriteLine("├────Object");
        Console.WriteLine($"├──────ID: {object_.Key.Id}");
        Console.WriteLine($"├──────Version: {(int)object_.Key.Version}");
        Console.WriteLine($"├──────Width: {object_.Value.Width}");
        Console.WriteLine($"├──────Height: {object_.Value.Height}");
        Console.WriteLine($"├──────Data: [{object_.Value.Data.Length}]");
    }

    Console.WriteLine("├──Compositions");

    foreach (var composition in displaySet.Compositions)
    {
        Console.WriteLine("├────Composition");
        Console.WriteLine($"├──────Object ID: {composition.Key.ObjectId}");
        Console.WriteLine($"├──────Window ID: {(int)composition.Key.WindowId}");
        Console.WriteLine($"├──────X: {composition.Value.X}");
        Console.WriteLine($"├──────Y: {composition.Value.Y}");
        Console.WriteLine($"├──────Forced: {composition.Value.Forced}");

        if (composition.Value.Crop is CroppedArea crop)
        {
            Console.WriteLine("├──────Crop");
            Console.WriteLine($"├────────X: {crop.X}");
            Console.WriteLine($"├────────Y: {crop.Y}");
            Console.WriteLine($"├────────Width: {crop.Width}");
            Console.WriteLine($"├────────Height: {crop.Height}");
        }
    }

    Console.WriteLine();
}

string TsToTimeStamp(uint ts)
{
    var ms = ts / 90;
    var h = ms / 3_600_000;
    ms -= h * 3_600_000;
    var m = ms / 60_000;
    ms -= m * 60_000;
    var s = ms / 1_000;
    ms -= s * 1_000;

    return $"{h.ToString("D2")}:{m.ToString("D2")}:{s.ToString("D2")}.{ms.ToString("D3")}";
}
