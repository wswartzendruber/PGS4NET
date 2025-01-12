/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

namespace PGS4NET;

internal class WindowState
{
    internal readonly bool PrimaryPlaneDirty;
    internal readonly bool SecondaryPlaneDirty;
    internal readonly bool PlanesDiffer;

    public WindowState(bool primaryPlaneDirty, bool secondaryPlaneDirty
        , bool planesDiffer)
    {
        PrimaryPlaneDirty = primaryPlaneDirty;
        SecondaryPlaneDirty = secondaryPlaneDirty;
        PlanesDiffer = planesDiffer;
    }
}