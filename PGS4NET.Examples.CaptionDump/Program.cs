/*
 * Copyright 2024 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using PGS4NET;
using PGS4NET.Captions;
using PGS4NET.DisplaySets;

if (args.Length != 1)
    throw new ArgumentException("A single parameter with a PGS file must be passed.");

using var stream = new FileStream(args[0], FileMode.Open);
var composer = new CaptionComposer();
var count = 0;

composer.NewCaption += (_, caption) =>
{
    var startTimeStamp = TsToTimeStamp(caption.TimeStamp);
    var endTimeStamp = TsToTimeStamp(caption.TimeStamp + caption.Duration);
    using var file = new FileStream($"caption-{count.ToString("D4")}-"
        + $"{caption.Width}x{caption.Height}.raw", FileMode.Create);

    Console.WriteLine($"Caption[{count.ToString("D4")}]: {startTimeStamp} -> {endTimeStamp}");

    foreach (var pixel in caption.Data)
    {
        file.WriteByte(pixel.Y);
        // file.WriteByte(pixel.Cr);
        // file.WriteByte(pixel.Cb);
        // file.WriteByte(pixel.Alpha);
    }

    count++;
};

while (stream.ReadDisplaySet() is DisplaySet displaySet)
{
    composer.Input(displaySet);
}

string TsToTimeStamp(uint ts)
{
    var ms = ts / 90;
    var h = ms / 3_600_000;
    ms -= h * 3_600_000;
    var m = ms / 60_000;
    ms -= m * 60_000;
    var s = ms / 1_000;
    ms -= s * 1_000;

    return $"{h.ToString("D2")}:{m.ToString("D2")}:{s.ToString("D2")}.{ms.ToString("D3")}";
}
