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
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Demo.System;


namespace
Demo.Courses
{


public class
Module
{


#region EF Compromises
private Guid Id { get; set; }
#endregion


public static readonly string
DefaultName = "A New Module";

public static readonly string
DefaultDescription = "";

public static readonly int
DefaultDuration = 1;


public
Module()
    : this(DefaultName)
{
}


public
Module(string name)
{
    this.Id = Guid.NewGuid();
    this.Course = null;
    this.Name = name;
    this.Description = DefaultDescription;
    this.Duration = DefaultDuration;
    this.Created = DateTimeOffset.Now;
}


/// <summary>
/// The course this module belongs to
/// </summary>
/// <remarks>
/// <c>null</c> if this module doesn't belong to a course
/// </remarks>
///
public Course
Course
{
    get;
    internal set;
}


[Required]
[StringLength(50, MinimumLength=2)]
public string
Name
{
    get { return name; }
    set
    {
        Check.Required(value, "value");
        Check.NotWhitespaceOnly(value, "value");
        Check.NoLeadingOrTrailingWhitespace(value, "value");
        Check.Length(2, 50, value, "value");
        if (value == name) return;
        if (Course != null && Course.Modules.Any(m => m.Name == value))
            throw new ArgumentOutOfRangeException(
                "value",
                "Another module with the specified name already exists in the course");
        Modified = DateTimeOffset.Now;
        name = value;
    }
}

private string name { get; set; }


[Required(AllowEmptyStrings=true)]
[DisplayFormat(ConvertEmptyStringToNull=false)]
public string
Description
{
    get { return description; }
    set
    {
        Check.Required(value, "value");
        value = StringUtilities.NormaliseNewlines(value);
        if(value == description) return;
        Modified = DateTimeOffset.Now;
        description = value;
    }
}

private string description { get; set; }


/// <summary>
/// Duration of the module in days
/// </summary>
///
[Range(1, 365)]
public int
Duration
{
    get { return duration; }
    set
    {
        Check.Range(1, 365, value, "value");
        if(value == duration) return;
        Modified = DateTimeOffset.Now;
        duration = value;
    }
}

private int duration { get; set; }


public DateTimeOffset
Created
{
    get;
    private set;
}


public DateTimeOffset
Modified
{
    get;
    private set;
}


public Module
CopyShallowTo(Module module)
{
    Check.Required(module, "module");
    module.Name = Name;
    module.Description = Description;
    module.Duration = Duration;
    return module;
}


}
}

