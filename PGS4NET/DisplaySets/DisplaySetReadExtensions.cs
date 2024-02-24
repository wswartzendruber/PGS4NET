/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PGS4NET.Segments;

namespace PGS4NET.DisplaySets;

public static partial class DisplaySetExtensions
{
    public static IList<DisplaySet> ReadAllDisplaySets(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (stream.ReadDisplaySet() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    public static async Task<IList<DisplaySet>> ReadAllDisplaySetsAsync(this Stream stream)
    {
        var returnValue = new List<DisplaySet>();

        while (await stream.ReadDisplaySetAsync() is DisplaySet displaySet)
            returnValue.Add(displaySet);

        return returnValue;
    }

    public static DisplaySet? ReadDisplaySet(this Stream stream)
    {
        var read = false;
        var composer = new DisplaySetComposer();

        while (stream.ReadSegment() is Segment segment)
        {
            read = true;

            if (composer.Input(segment) is DisplaySet displaySet)
                return displaySet;
        }

        if (read)
            throw new DisplaySetException("EOF during display set composition.");

        return null;
    }

    public static async Task<DisplaySet?> ReadDisplaySetAsync(this Stream stream)
    {
        var read = false;
        var composer = new DisplaySetComposer();

        while (await stream.ReadSegmentAsync() is Segment segment)
        {
            read = true;

            if (composer.Input(segment) is DisplaySet displaySet)
                return displaySet;
        }

        if (read)
            throw new DisplaySetException("EOF during display set composition.");

        return null;
    }

    public static IList<DisplaySet> ToDisplaySetList(this IEnumerable<Segment> segments)
    {
        var pending = false;
        var returnValue = new List<DisplaySet>();
        var composer = new DisplaySetComposer();

        foreach (var segment in segments)
        {
            pending = true;

            if (composer.Input(segment) is DisplaySet displaySet)
            {
                pending = false;
                returnValue.Add(displaySet);
            }
        }

        if (pending)
            throw new DisplaySetException("Incomplete display set from trailing segments.");

        return returnValue;
    }
}
