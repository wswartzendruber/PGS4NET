using System;
using PGS4NET.DisplaySets;

namespace PGS4NET.Captions;

internal class CompositionArea
{
    internal bool Pending = false;

    private readonly int Size;
    private readonly int Width;
    private readonly int Height;
    private readonly bool Forced;
    private readonly byte[] ClearPalettePlane;
    private readonly byte[] PalettePlane;
    private readonly YcbcraPixel[] CurrentYcbcraPlane;
    private readonly YcbcraPixel[] PreviousYcbcraPlane;

    private int X;
    private int Y;
    private PgsTime LastTimeStamp;

    internal CompositionArea(PgsTime timeStamp, int x, int y, int width, int height,
        bool forced)
    {
        Size = width * height;
        Width = width;
        Height = height;
        Forced = forced;
        ClearPalettePlane = new byte[Size];
        PalettePlane = new byte[Size];
        CurrentYcbcraPlane = new YcbcraPixel[Size];
        PreviousYcbcraPlane = new YcbcraPixel[Size];

        X = x;
        Y = y;
        LastTimeStamp = timeStamp;

        for (int i = 0; i < Size; i++)
            ClearPalettePlane[i] = 255;

        Array.Copy(ClearPalettePlane, PalettePlane, Size);
    }

    internal void Clear()
    {
        Array.Copy(ClearPalettePlane, PalettePlane, Size);
    }

    internal void DrawObject(DisplayObject displayObject, Area? crop)
    {
        if (crop is Area)
        {
            var destinationIndex = 0;
            var endY = crop.Y + crop.Height;

            for (int y = crop.Y; y < endY; y++)
            {
                Array.Copy(displayObject.Data, y * displayObject.Width + crop.X, PalettePlane,
                    destinationIndex, crop.Width);

                destinationIndex += crop.Width;
            }
        }
        else
        {
            Array.Copy(displayObject.Data, PalettePlane, Size);
        }
    }

    internal void Flush(PgsTime timeStamp)
    {
        //if (Pending)
        //{
        //    OutputCaption(timeStamp);
        //    Pending = false;
        //}

        //for (int i = 0; i < Size; i++)
        //{
        //    PalettePlane[i] = 255;
        //    CurrentYcbcraPlane[i] = default;
        //}

        //Array.Copy(CurrentYcbcraPlane, PreviousYcbcraPlane, Size);
        //LastTimeStamp = timeStamp;
    }

    internal void UpdatePalette(PgsTime timeStamp, YcbcraPixel[] palette)
    {
        var different = false;
        var hadSomething = Pending;
        var hasSomething = false;

        for (int i = 0; i < Size; i++)
        {
            CurrentYcbcraPlane[i] = palette[PalettePlane[i]];
            different |= CurrentYcbcraPlane[i] != PreviousYcbcraPlane[i];
            hasSomething |= CurrentYcbcraPlane[i] != default;
        }

        if (different)
        {
            if (hadSomething)
                OutputCaption(timeStamp);

            Array.Copy(CurrentYcbcraPlane, PreviousYcbcraPlane, Size);
            LastTimeStamp = timeStamp;
        }

        Pending = hasSomething;
    }

    private void OnReady(Caption caption)
    {
        Ready?.Invoke(this, caption);
    }

    private void OutputCaption(PgsTime timeStamp)
    {
        var duration = timeStamp - LastTimeStamp;
        var data = (YcbcraPixel[])PreviousYcbcraPlane.Clone();
        var caption = new Caption(LastTimeStamp, timeStamp - LastTimeStamp, X, Y, Width,
            Height, data, Forced);

        OnReady(caption);
    }

    internal event EventHandler<Caption>? Ready;
}
