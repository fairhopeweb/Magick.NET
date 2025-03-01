﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using ImageMagick;
using Xunit;

#if Q8
using QuantumType = System.Byte;
#elif Q16
using QuantumType = System.UInt16;
#elif Q16HDRI
using QuantumType = System.Single;
#else
#error Not implemented!
#endif

namespace Magick.NET.Tests;

public partial class ResourceLimitsTests
{
    [Collection(nameof(RunTestsSeparately))]
    public class TheHeightProperty
    {
        [Fact]
        public void ShouldHaveTheCorrectValue()
        {
            if (Runtime.Is64Bit)
            {
                Assert.Equal(1844674407370955161U / sizeof(QuantumType), ResourceLimits.Height);
            }
            else
            {
                Assert.Equal(429496729U / sizeof(QuantumType), ResourceLimits.Height);
            }
        }

        [Fact]
        public void ShouldReturnTheCorrectValueWhenChanged()
        {
            var height = ResourceLimits.Height;

            ResourceLimits.Height = 100000U;
            Assert.Equal(100000U, ResourceLimits.Height);
            ResourceLimits.Height = height;
        }
    }
}
