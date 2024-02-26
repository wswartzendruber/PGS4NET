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
    public PgsTimeStamp TimeStamp;

    public PgsTimeStamp Duration;

    public ushort X;

    public ushort Y;

    public ushort Width;

    public ushort Height;

    public PgsPixel[] Data = new PgsPixel[0];

    public bool Forced;
}
