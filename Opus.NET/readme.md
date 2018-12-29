# Open Interactive Audio Codec

## Overview

Opus is a totally open, royalty-free, highly versatile audio codec. Opus is unmatched for interactive speech and music transmission over the Internet, but also intended for storage and streaming applications. It is standardized by the Internet Engineering Task Force (IETF) as [RFC 6716](http://tools.ietf.org/html/rfc6716) which incorporated technology from Skype's SILK codec and Xiph.Org's CELT codec.

## Technology

Opus can handle a wide range of audio applications, including Voice over IP, videoconferencing, in-game chat, and even remote live music performances. It can scale from low bit-rate narrowband speech to very high quality stereo music. Supported features are:

* Bit-rates from 6 kb/s to 510 kb/s
* Sampling rates from 8 kHz (narrowband) to 48 kHz (fullband)
* Frame sizes from 2.5 ms to 60 ms
* Support for both constant bit-rate (CBR) and variable bit-rate (VBR)
* Audio bandwidth from narrowband to full-band
* Support for speech and music
* Support for mono and stereo
* Support for up to 255 channels (multistream frames)
* Dynamically adjustable bitrate, audio bandwidth, and frame size
* Good loss robustness and packet loss concealment (PLC)
* Floating point and fixed-point implementation

You can read the full specification, including the reference implementation, in [RFC 6716](http://tools.ietf.org/html/rfc6716).

# Opus.NET

Opus.NET is a managed wrapper around the native Opus library. Included in the repository is a basic Opus encoder and decoder plus an example WinForms application that demonstrates usage.

The WinForms demo app makes use of the excellent NAudio library.

Opus.NET is licensed under the MIT license.

# Licenses

A copy of all licenses are available in the repository.

* [Opus Distributed License](https://github.com/JohnACarruthers/Opus.NET/blob/master/opus license.txt)
* [NAudio Distributed License](https://github.com/JohnACarruthers/Opus.NET/blob/master/naudio license.txt)
* [Opus.NET License](https://github.com/JohnACarruthers/Opus.NET/blob/master/license.txt)