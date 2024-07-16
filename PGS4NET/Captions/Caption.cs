/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET.Captions;

public class Caption
{
    public PgsTimeStamp TimeStamp { get; set; }

    public PgsTimeStamp Duration { get; set; }

    public ushort X { get; set; }

    public ushort Y { get; set; }

    public ushort Width { get; set; }

    public ushort Height { get; set; }

    public PgsPixel[] Data { get; set; }

    public bool Forced { get; set; }

    public Caption()
    {
        Data = new PgsPixel[0];
    }

    public Caption(PgsTimeStamp timeStamp, PgsTimeStamp duration, ushort x, ushort y
        , ushort width, ushort height, PgsPixel[] data, bool forced)
    {
        TimeStamp = timeStamp;
        Duration = duration;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Data = data;
        Forced = forced;
    }
}
