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
using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

public class CaptionComposer
{
    private readonly Dictionary<byte, PgsPixel[]> Planes = new();
    private readonly Dictionary<byte, PgsTimeStamp> PlaneTimeStamps = new();
    private readonly Dictionary<byte, DisplayWindow> Windows = new();
    private readonly Dictionary<ushort, DisplayObject> Objects = new();
    private readonly Dictionary<byte, DisplayPalette> Palettes = new();

    public IList<Caption> Input(DisplaySet displaySet)
    {
        var captions = new List<Caption>();

        if (displaySet.CompositionState == CompositionState.EpochStart)
            Reset();

        return captions;
    }

    public void Reset()
    {
        Planes.Clear();
        PlaneTimeStamps.Clear();
        Windows.Clear();
        Objects.Clear();
        Palettes.Clear();
    }

    private Caption? RenderCaption(byte id, PgsTimeStamp endTimeStamp)
    {
        var startTimeStamp = PlaneTimeStamps[id];
        var window = Windows[id];
        var forced = false; // TODO

        return new Caption(startTimeStamp, endTimeStamp - startTimeStamp, window.X, window.Y
            , window.Width, window.Height, Planes[id], forced);
    }

    private bool SetWindow(byte id, DisplayWindow newWindow)
    {
        if (Windows.TryGetValue(id, out DisplayWindow existingWindow))
        {
            if (WindowsMatch(newWindow, existingWindow))
            {
                return true;
            }
            else
            {
                Windows[id] = newWindow;

                return false;
            }
        }
        else
        {
            Windows[id] = newWindow;

            return true;
        }
    }

    private bool WindowsMatch(DisplayWindow first, DisplayWindow second)
    {
        return first.X == second.X
            && first.Y == second.Y
            && first.Width == second.Width
            && first.Height == second.Height;
    }
}
