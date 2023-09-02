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

using PGS4NET.DisplaySets;

namespace PGS4NET.Tests.DisplaySets;

public class RoundTrip
{
    [Fact]
    public void CycleSegments()
    {
        using var stream = new MemoryStream();
        var inDisplaySet = DisplaySetInstances.Instances["defaults"];

        stream.WriteDisplaySet(inDisplaySet);
        stream.Position = 0;

        var outDisplaySet = stream.ReadDisplaySet()
            ?? throw new NullReferenceException("Display set could not be read.");

        AssertDisplaySetsEqual("defaults", inDisplaySet, outDisplaySet);
    }

    private static void AssertDisplaySetsEqual(string name, DisplaySet first, DisplaySet second)
    {
        if (first.Pts != second.Pts)
            throw new Exception($"PTS for '{name}' does not match.");
        if (first.Dts != second.Dts)
            throw new Exception($"DTS for '{name}' does not match.");
        if (first.Width != second.Width)
            throw new Exception($"Width for '{name}' does not match.");
        if (first.Height != second.Height)
            throw new Exception($"Height for '{name}' does not match.");
        if (first.FrameRate != second.FrameRate)
            throw new Exception($"FrameRate for '{name}' does not match.");
        if (first.PaletteUpdateOnly != second.PaletteUpdateOnly)
            throw new Exception($"PaletteUpdateOnly for '{name}' does not match.");
        if (first.PaletteUpdateId != second.PaletteUpdateId)
            throw new Exception($"PaletteUpdateId for '{name}' does not match.");

        if (first.Windows.Keys.Count != second.Windows.Keys.Count)
        {
            throw new Exception("DisplayWindow count mismatch.");
        }
        foreach (var window in first.Windows)
        {
            var windowId = window.Key;
            var firstWindow = window.Value;

            if (second.Windows.TryGetValue(windowId, out DisplayWindow secondWindow))
            {
                if (firstWindow.X != secondWindow.X)
                {
                    throw new Exception($"Window X for {windowId} in '{name}' does not match");
                }
                if (firstWindow.Y != secondWindow.Y)
                {
                    throw new Exception($"Window Y for {windowId} in '{name}' does not match");
                }
                if (firstWindow.Width != secondWindow.Width)
                {
                    throw new Exception($"Window Width for {windowId} in '{name}' does not "
                        + "match");
                }
                if (firstWindow.Height != secondWindow.Height)
                {
                    throw new Exception($"Window Height for {windowId} in '{name}' does not "
                        + "match");
                }
            }
            else
            {
                throw new Exception($"Window ID {windowId} not found in '{name}'.");
            }
        }

        if (first.Palettes.Keys.Count != second.Palettes.Keys.Count)
        {
            throw new Exception("DisplayPalette count mismatch.");
        }
        foreach (var palette in first.Palettes)
        {
            var paletteVid = palette.Key;
            var firstPalette = palette.Value;
            var pid = $"{paletteVid.Id}.{paletteVid.Version}";

            if (second.Palettes.TryGetValue(paletteVid, out DisplayPalette? secondPalette))
            {
                if (secondPalette is null)
                    throw new InvalidOperationException("Got a null DisplayPalette back.");

                foreach (var paletteEntry in firstPalette.Entries)
                {
                    var paletteEntryId = paletteEntry.Key;
                    var firstPaletteEntry = paletteEntry.Value;
                    var peid = $"{paletteVid.Id}.{paletteVid.Version}.{paletteEntryId}";

                    if (secondPalette.Entries.TryGetValue(paletteEntryId
                        , out DisplayPaletteEntry secondPaletteEntry))
                    {
                        if (firstPaletteEntry.Y != secondPaletteEntry.Y)
                        {
                            throw new Exception($"Palette entry Y for {peid} in {name} does "
                                + "not match.");
                        }
                        if (firstPaletteEntry.Cr != secondPaletteEntry.Cr)
                        {
                            throw new Exception($"Palette entry Cr for {peid} in {name} does "
                                + "not match.");
                        }
                        if (firstPaletteEntry.Cb != secondPaletteEntry.Cb)
                        {
                            throw new Exception($"Palette entry Cb for {peid} in {name} does "
                                + "not match.");
                        }
                        if (firstPaletteEntry.Alpha != secondPaletteEntry.Alpha)
                        {
                            throw new Exception($"Palette entry Alpha for {peid} in {name} "
                                + "does not match.");
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Palette VID {pid} not found in '{name}'.");
            }
        }

        if (first.Objects.Keys.Count != second.Objects.Keys.Count)
        {
            throw new Exception("DisplayObject count mismatch.");
        }
        foreach (var object_ in first.Objects)
        {
            var objectVid = object_.Key;
            var firstObject = object_.Value;
            var oid = $"{objectVid.Id}.{objectVid.Version}";

            if (second.Objects.TryGetValue(objectVid, out DisplayObject? secondObject))
            {
                if (secondObject is null)
                    throw new InvalidOperationException("Got a null DisplayObject back.");

                if (firstObject.Width != secondObject.Width)
                    throw new Exception($"Object Width for {oid} in {name} does not match.");
                if (firstObject.Height != secondObject.Height)
                    throw new Exception($"Object Height for {oid} in {name} does not match.");

                if (firstObject.Lines.Count != secondObject.Lines.Count)
                {
                    throw new Exception($"Object Line count for {oid} in {name} does not "
                        + "match.");
                }
                for (int i = 0; i < firstObject.Lines.Count; i++)
                {
                    if (firstObject.Lines[i].Count != secondObject.Lines[i].Count)
                    {
                        throw new Exception($"Object Line[{i}] count for {oid} in {name} does "
                            + "not match.");
                    }

                    for (int j = 0; j < firstObject.Lines[i].Count; j++)
                    {
                        if (firstObject.Lines[i][j] != secondObject.Lines[i][j])
                        {
                            throw new Exception($"Object Line[{i}][{j}] for {oid} in {name} "
                                + "does not match.");
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Object VID {oid} not found in '{name}'.");
            }
        }

        if (first.CompositionNumber != second.CompositionNumber)
            throw new Exception($"CompositionNumber for '{name}' does not match.");
        if (first.CompositionState != second.CompositionState)
            throw new Exception($"CompositionState for '{name}' does not match.");

        if (first.Compositions.Keys.Count != second.Compositions.Keys.Count)
        {
            throw new Exception("DisplayComposition count mismatch.");
        }
        foreach (var composition in first.Compositions)
        {
            var compositionId = composition.Key;
            var firstComposition = composition.Value;
            var cid = $"{compositionId.ObjectId}.{compositionId.WindowId}";

            if (second.Compositions.TryGetValue(compositionId
                , out DisplayComposition secondComposition))
            {
                if (firstComposition.X != secondComposition.X)
                    throw new Exception($"Composition X for '{name}' does not match.");
                if (firstComposition.Y != secondComposition.Y)
                    throw new Exception($"Composition Y for '{name}' does not match.");
                if (firstComposition.Forced != secondComposition.Forced)
                    throw new Exception($"Composition Forced for '{name}' does not match.");

                if (firstComposition.Crop?.X != secondComposition.Crop?.X)
                {
                    throw new Exception($"Composition Crop X for '{name}' does not match.");
                }
                if (firstComposition.Crop?.Y != secondComposition.Crop?.Y)
                {
                    throw new Exception($"Composition Crop Y for '{name}' does not match.");
                }
                if (firstComposition.Crop?.Width != secondComposition.Crop?.Width)
                {
                    throw new Exception($"Composition Crop Width for '{name}' does not match.");
                }
                if (firstComposition.Crop?.Height != secondComposition.Crop?.Height)
                {
                    throw new Exception($"Composition Crop Height for '{name}' does not "
                        + "match.");
                }
            }
            else
            {
                throw new Exception($"Composition ID {cid} not found in '{name}'.");
            }
        }
    }
}
