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

/// <summary>
///     Represents the combination of a <see cref="DisplaySets.DisplayComposition"/> and
///     <see cref="DisplaySets.DisplayObject"/> that can be passed to a <see cref="Compositor"/>
///     instance in order to perform a draw operation.
/// </summary>
/// <remarks>
///     This is primarily an internal class used by the more accessible parts of this library.
///     It has been made <see langword="public"/> in the event any user needs to do compositing
///     of their own, for whatever reason.
/// </remarks>
public class CompositorComposition
{
    /// <summary>
    ///     The display set composition information.
    /// </summary>
    public DisplayComposition DisplayComposition { get; set; }

    /// <summary>
    ///     The display set object information.
    /// </summary>
    public DisplayObject DisplayObject { get; set; }

    /// <summary>
    ///     Initializes a new instance with the provided values.
    /// </summary>
    /// <param name="displayComposition">
    ///     The display set composition information.
    /// </param>
    /// <param name="displayObject">
    ///     The display set object information.
    /// </param>
    public CompositorComposition(DisplayComposition displayComposition
        , DisplayObject displayObject)
    {
        DisplayComposition = displayComposition;
        DisplayObject = displayObject;
    }
}
