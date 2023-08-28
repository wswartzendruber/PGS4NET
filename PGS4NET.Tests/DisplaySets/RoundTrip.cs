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
            throw new Exception("PTS for '{name}' does not match.");
        if (first.Dts != second.Dts)
            throw new Exception("DTS for '{name}' does not match.");
        if (first.Width != second.Width)
            throw new Exception("Width for '{name}' does not match.");
        if (first.Height != second.Height)
            throw new Exception("Height for '{name}' does not match.");
        if (first.FrameRate != second.FrameRate)
            throw new Exception("FrameRate for '{name}' does not match.");
        if (first.PaletteUpdateOnly != second.PaletteUpdateOnly)
            throw new Exception("PaletteUpdateOnly for '{name}' does not match.");
        if (first.PaletteUpdateId != second.PaletteUpdateId)
            throw new Exception("PaletteUpdateId for '{name}' does not match.");
    }
}
