/*
 * Copyright 2024 William Swartzendruber
 *
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a
 * copy of the MPL was not distributed with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * SPDX-License-Identifier: MPL-2.0
 */

using PGS4NET.DisplaySets;

namespace PGS4NET;

/// <summary>
///     Represents the combination of a <see cref="DisplaySets.DisplayComposition"/>,
///     <see cref="DisplaySets.DisplayObject"/>, and <see cref="DisplaySets.DisplayPalette"/>
///     that can be passed to a <see cref="Compositor"/> instance in order to perform a draw
///     operation.
/// </summary>
public class CompositorComposition
{
    /// <summary>
    ///     The PGS composition information.
    /// </summary>
    public DisplayComposition DisplayComposition { get; set; }

    /// <summary>
    ///     The PGS object information.
    /// </summary>
    public DisplayObject DisplayObject { get; set; }

    /// <summary>
    ///     The PGS palette information.
    /// </summary>
    public DisplayPalette DisplayPalette { get; set; }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="displayComposition">
    ///     The PGS composition information.
    /// </param>
    /// <param name="displayObject">
    ///     The PGS object information.
    /// </param>
    /// <param name="displayPalette">
    ///     The PGS palette information.
    /// </param>
    public CompositorComposition(DisplayComposition displayComposition
        , DisplayObject displayObject, DisplayPalette displayPalette)
    {
        DisplayComposition = displayComposition;
        DisplayObject = displayObject;
        DisplayPalette = displayPalette;
    }
}
