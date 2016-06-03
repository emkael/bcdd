[Polish README | Plik pomocy po polsku](/doc/README.pl.md)

BCDD - BCalc Double Dummy
=========================

BCalc-based double dummy analysis provider for PBN files.

Overview
========

This application provides an alternative to the Deep Finesse double-dummy analysis suggested by BigDeal while attempting print-out of a PBN file that hasn't yet been analyzed.

Its main features, and advantages, over BigDeal's solution (MiniMaxDOS.exe's, to be precise) are:

 * better performance and stability
 * more consistency
 * ability to batch analyze multiple files
 * export support for both JFR custom DD analysis PBN tags and [standard PBN set of tags](http://www.tistis.nl/pbn/pbn_v21.txt)
 * additional import support for Double Dummy Solver custom `OptimumResult` PBN tag
 * automatic conversion between these formats (note that this results in a larger PBN output file)

Double dummy analysis is provided by the [BCalc's](http://bcalc.w8.pl) `libbcalcdds` library (which is a prerequisite not distributed with the software).

Installation
============

 1. Download the application.
 2. Unpack it to your destination directory.
 3. [Download `libbcalcdds.dll` library](http://bcalc.w8.pl/download/API_C/) suitable for your CPU architecture (application supports both win32 and win64) and put it in the application directory or somewhere in your OS' library search path.
 4. You should be good to go.

BCDD is a .NET 3.5 application and requires [suitable runtime environment](https://www.microsoft.com/download/details.aspx?id=21) to function properly.

Usage
=====

You can provide path(s) to PBN file(s) as command line argument(s):

    BCDD.exe [FILE_1 [FILE_2...]]

When no file paths are provided (or none of the provided paths exist), the application prompts (via a standard file open dialog) for input files.

All files are overwritten in place (but through a temporary file, so in case of irrecoverable file-wide errors, the file is not overwritten).

Note: If errors concern single boards from a file, invalid boards are skipped in the output.

Implementation details
======================

There are some quirks to double dummy analysis results, which are purely conventional. In case of this application, wherever there might be something ambiguous, it's generally safe to assume that compatibility with BigDeal's results was main priority.

This includes:

 * boards with the same par contract for both sides: par contract for the dealing side is assumed (contrary to [Richard Pavlicek's](http://www.rpbridge.net/7a23.htm) convention of assigning "Pass out" par contract in such cases)
 * from all equal par contracts (i.e. yielding the same score), the *lowest* one is assumed the par contract (but: see below)
 * when par contract is the same for both partners of a specific side, the declarer is unspecified (may, and usually will, differ from BigDeal's)

In some cases, when par contract differs between two players of one side, BigDeal deviates from the lowest equal par contract. This inconsistency is not maintained in BCDD.

The priority of existing PBN tags treated as input sources for double dummy trick table and par contract/score is:

 * JFR's custom `Ability` and `Minimax` tags
 * Double Dummy Solver's custom `OptimumResult` tag and standard PBN `OptimumScore` tags (for par contract and score)
 * standard PBN v2.1 `OptimumResultTable` section (for double dummy trick table)
 * only when both `Ability` and `OptimumResultTable` sections are missing, double dummy analysis is conducted

DDS' custom tag is used only to determine par score, it is not written back to the file if it's absent in original file.

Credits and author
==================

 * `libbcalcdds.dll` is part of the [BCalc project](http://bcalc.w8.pl) by Piotr Beling
 * application icon is a custom-modified version of an icon made by [Freepik](http://www.freepik.com), acquired from [www.flaticon.com](http://www.flaticon.com/free-icon/playing-cards_82783) and is licensed under [Creative Commons BY 3.0](http://creativecommons.org/licenses/by/3.0/)

This software was made by [Michał Klichowicz](https://emkael.info).

If you use it, you probably know how to reach me.

If you don't (know how to reach me), you can find it on my website.

License
=======

This software is licensed under the [Simplified BSD License](https://opensource.org/licenses/BSD-2-Clause), because why the hell not.

Here's the license text:

```
Copyright (C) 2016, Michał Klichowicz
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
   2. Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
         and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
```

---

`Then make them bring the curtain down.`
