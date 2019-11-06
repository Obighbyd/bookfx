﻿namespace BookFx.Calculation
{
    using System;
    using BookFx.Cores;
    using BookFx.Functional;

    internal static class HeightCalc
    {
        public static Sc<Cache, int> Height(BoxCore box, Structure structure) => throw new NotImplementedException();

        private static Sc<Cache, int> OfValue(BoxCore box, Structure structure) =>
            box
                .RowSpan
                .Map(Sc<Cache>.Return)
                .OrElse(() => structure
                    .Parent(box)
                    .Map(
                        row: MinHeightCalc.MinHeight,
                        col: _ => Sc<Cache>.Return(1),
                        stack: MinHeightCalc.MinHeight,
                        value: _ => throw new InvalidOperationException(),
                        proto: _ => Sc<Cache>.Return(1)))
                .GetOrElse(Sc<Cache>.Return(1));
    }
}