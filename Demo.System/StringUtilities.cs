// -----------------------------------------------------------------------------
// Copyright (c) 2014 Ron MacNeil <macro@hotmail.com>
//
// Permission to use, copy, modify, and distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
// -----------------------------------------------------------------------------


using System;
using System.Text.RegularExpressions;


namespace
Demo.System
{


public static class
StringUtilities
{


public static string
NormaliseNewlines(string s)
{
    if(s == null) return null;
    return s
        .Replace("\r\n", "\n")
        .Replace("\r", "\n")
        .Replace("\n", Environment.NewLine);
}


public static string
ToUrlSlug(string s)
{
    Check.Required(s, "s");
    Check.NotEmpty(s, "s");
    Check.NotWhitespaceOnly(s, "s");
    Check.NoLeadingOrTrailingWhitespace(s, "s");
    s = s.ToLowerInvariant();
    s = Regex.Replace(s, @"[^a-z0-9-]", "");
    s = Regex.Replace(s, @"[\s]+", "-");
    return s;
}


}
}

