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

namespace PGS4NET;

internal class GraphicsOperation
{
    internal DisplayComposition DisplayComposition;
    internal DisplayObject DisplayObject;

    internal GraphicsOperation(DisplayComposition displayComposition
        , DisplayObject displayObject)
    {
        DisplayComposition = displayComposition;
        DisplayObject = displayObject;
    }
}
