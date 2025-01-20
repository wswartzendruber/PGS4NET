﻿/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET;

internal class AreaState
{
    internal readonly Area Area;
    
    internal bool Forced;
    internal PgsTimeStamp? PendingTimeStamp;

    internal AreaState(Area area, bool forced)
    {
        Area = area;
        Forced = forced;
        PendingTimeStamp = null;
    }
}