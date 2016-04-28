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
using System.Collections;
using System.Text.RegularExpressions;
using Humanizer;


namespace
Demo.System
{


public static class
Check
{


//
// TODO
// Checker for lazy people that uses reflection to run checks based off of
// System.ComponentModel.DataAnnotations attributes
//


public static string
HumanizeParameterName(string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (paramName == "" || paramName == "value")
        return "The value";
    return paramName.Humanize();
}


public static void
Required(object arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        throw new ArgumentNullException(paramName);
}


public static void
NotEmpty(string arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        return;
    if (arg == "")
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "{0} is empty",
                HumanizeParameterName(paramName)));
}


public static void
NotWhitespaceOnly(string arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        return;
    if (arg.Trim() == "")
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "{0} is whitespace-only",
                HumanizeParameterName(paramName)));
}


public static void
NoLeadingOrTrailingWhitespace(string arg, string paramName)
{
    NoLeadingWhitespace(arg, paramName);
    NoTrailingWhitespace(arg, paramName);
}


public static void
NoLeadingWhitespace(string arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        return;
    if (arg == "")
        return;
    if (char.IsWhiteSpace(arg, 0))
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "{0} begins with whitespace",
                HumanizeParameterName(paramName)));
}


public static void
NoTrailingWhitespace(string arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        return;
    if (arg == "")
        return;
    if (char.IsWhiteSpace(arg, arg.Length-1))
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "{0} ends with whitespace",
                HumanizeParameterName(paramName)));
}


public static void
Length(int minLength, int maxLength, string arg, string paramName)
{
    if (minLength > maxLength)
        throw new ArgumentOutOfRangeException(
            "minLength",
            "minLength is greater than maxLength");
    MinLength(minLength, arg, paramName);
    MaxLength(maxLength, arg, paramName);
}


public static void
MinLength(int minLength, string arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        return;
    if (arg.Length < minLength)
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "Argument {0} was shorter than {1} characters",
                paramName,
                minLength));
}


public static void
MaxLength(int maxLength, string arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (arg == null)
        return;
    if (arg.Length > maxLength)
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "Argument {0} was longer than {1} characters",
                paramName,
                maxLength));
}


public static void
Range<T>(T min, T max, T arg, string paramName)
    where T : IComparable<T>
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (min == null)
        throw new ArgumentNullException("min");
    if (max == null)
        throw new ArgumentNullException("max");
    if (min.CompareTo(max) >= 0)
        throw new ArgumentOutOfRangeException(
            "min",
            "min was greater than or equal to max");
    Min(min, arg, paramName);
    Max(max, arg, paramName);
}


public static void
Min<T>(T min, T arg, string paramName)
    where T : IComparable<T>
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (min == null)
        throw new ArgumentNullException("min");
    if (arg == null)
        return;
    if (arg.CompareTo(min) < 0)
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "Argument {0} was less than {1}",
                paramName,
                min));
}


public static void
Max<T>(T max, T arg, string paramName)
    where T : IComparable<T>
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (max == null)
        throw new ArgumentNullException("max");
    if (arg == null)
        return;
    if (arg.CompareTo(max) > 0)
        throw new ArgumentOutOfRangeException(
            paramName,
            string.Format(
                "Argument {0} was greater than {1}",
                paramName,
                max));
}


public static void
MaxDecimalPlaces(int decimalPlaces, decimal arg, string paramName)
{
    if (paramName == null)
        throw new ArgumentNullException("paramName");
    if (decimalPlaces < 0)
        throw new ArgumentOutOfRangeException("decimalPlaces");
    if (decimalPlaces > 10)
        throw new ArgumentOutOfRangeException(
            "decimalPlaces",
            "This check method only handles up to 10 decimal places");
    decimal m = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
    if (arg != decimal.Truncate(arg * m) / m)
        throw new ArgumentOutOfRangeException(
            paramName,
            "Argument {0} had too many decimal places of precision");
}


}
}

