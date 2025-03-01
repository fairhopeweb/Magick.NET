﻿// Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;

namespace ImageMagick;

/// <summary>
/// Contains the he perceptual hash of one or more image channels.
/// </summary>
public sealed partial class PerceptualHash : IPerceptualHash
{
    private readonly Dictionary<PixelChannel, ChannelPerceptualHash> _channels;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerceptualHash"/> class.
    /// </summary>
    /// <param name="hash">The hash.</param>
    public PerceptualHash(string hash)
      : this()
    {
        Throw.IfNullOrEmpty(nameof(hash), hash);
        Throw.IfFalse(nameof(hash), hash.Length == 210, "Invalid hash size.");

        _channels[PixelChannel.Red] = new ChannelPerceptualHash(PixelChannel.Red, hash.Substring(0, 70));
        _channels[PixelChannel.Green] = new ChannelPerceptualHash(PixelChannel.Green, hash.Substring(70, 70));
        _channels[PixelChannel.Blue] = new ChannelPerceptualHash(PixelChannel.Blue, hash.Substring(140, 70));
    }

    internal PerceptualHash(MagickImage image, IntPtr list)
      : this()
    {
        if (list == IntPtr.Zero)
            return;

        AddChannel(image, list, PixelChannel.Red);
        AddChannel(image, list, PixelChannel.Green);
        AddChannel(image, list, PixelChannel.Blue);
    }

    private PerceptualHash()
    {
        _channels = new Dictionary<PixelChannel, ChannelPerceptualHash>();
    }

    internal bool Isvalid
    {
        get
        {
            return _channels.ContainsKey(PixelChannel.Red) &&
              _channels.ContainsKey(PixelChannel.Green) &&
              _channels.ContainsKey(PixelChannel.Blue);
        }
    }

    /// <summary>
    /// Returns the perceptual hash for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to get the has for.</param>
    /// <returns>The perceptual hash for the specified channel.</returns>
    public IChannelPerceptualHash GetChannel(PixelChannel channel)
    {
        _channels.TryGetValue(channel, out var perceptualHash);
        return perceptualHash;
    }

    /// <summary>
    /// Returns the sum squared difference between this hash and the other hash.
    /// </summary>
    /// <param name="other">The <see cref="PerceptualHash"/> to get the distance of.</param>
    /// <returns>The sum squared difference between this hash and the other hash.</returns>
    public double SumSquaredDistance(IPerceptualHash other)
    {
        Throw.IfNull(nameof(other), other);

        return
          _channels[PixelChannel.Red].SumSquaredDistance(other.GetChannel(PixelChannel.Red)) +
          _channels[PixelChannel.Green].SumSquaredDistance(other.GetChannel(PixelChannel.Green)) +
          _channels[PixelChannel.Blue].SumSquaredDistance(other.GetChannel(PixelChannel.Blue));
    }

    /// <summary>
    /// Returns a string representation of this hash.
    /// </summary>
    /// <returns>A <see cref="string"/>.</returns>
    public override string ToString()
    {
        return
          _channels[PixelChannel.Red].ToString() +
          _channels[PixelChannel.Green].ToString() +
          _channels[PixelChannel.Blue].ToString();
    }

    internal static void DisposeList(IntPtr list)
    {
        if (list != IntPtr.Zero)
            NativePerceptualHash.DisposeList(list);
    }

    private static ChannelPerceptualHash? CreateChannelPerceptualHash(MagickImage image, IntPtr list, PixelChannel channel)
    {
        var instance = NativePerceptualHash.GetInstance(image, list, channel);
        if (instance == IntPtr.Zero)
            return null;

        return new ChannelPerceptualHash(channel, instance);
    }

    private void AddChannel(MagickImage image, IntPtr list, PixelChannel channel)
    {
        var instance = CreateChannelPerceptualHash(image, list, channel);
        if (instance is not null)
            _channels.Add(instance.Channel, instance);
    }
}
