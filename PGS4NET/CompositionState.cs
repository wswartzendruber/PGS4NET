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

/// <summary>
///     Defines the role of a presentation composition segment (PCS), and thereby the associated
///     display set (DS), within an epoch.
/// </summary>
public enum CompositionState
{
    /// <summary>
    ///     Indicates that the associated PCS (and the DS it belongs to) defines the start of a
    ///     new epoch. This signals a reset of all previously buffered compositions, windows,
    ///     palettes, and objects.
    /// </summary>
    EpochStart,

    /// <summary>
    ///     Similar to <see cref="EpochStart"/>, except used to refresh the screen with the
    ///     current composition. That is, the associated DS should redefine the same windows,
    ///     objects, and palettes as the <see cref="EpochStart"/> DS. This allows, for example,
    ///     a player to seek past an <see cref="EpochStart"/> and land in the middle of an
    ///     epoch, while still being able to show the relevant composition once the
    ///     <see cref="AcquisitionPoint"/> is encountered. While it is technically possible to
    ///     use this to alter the composition from what the <see cref="EpochStart"/> DS has
    ///     defined, this practice is less common.
    /// </summary>
    AcquisitionPoint,

    /// <summary>
    ///     This updates the composition that is on the screen. This is typically used to clear
    ///     the current composition from the screen by defining a PCS with no composition
    ///     objects, thereby effectively closing out the current epoch. But other things like
    ///     palette updates and object substitution within a window can also be done. As an
    ///     epoch must compose to fixed areas of the screen, redefining windows here is
    ///     forbidden.
    /// </summary>
    Normal,
}
