/*
 * Copyright 2025 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

internal class StoredWindow
{
    internal DisplayWindow DisplayWindow;
    internal WindowState WindowState;
    internal bool Forced;

    internal StoredWindow(DisplayWindow displayWindow, WindowState windowState, bool forced)
    {
        DisplayWindow = displayWindow;
        WindowState = windowState;
        Forced = forced;
    }
}