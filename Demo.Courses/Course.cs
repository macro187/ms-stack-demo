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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Demo.System;


namespace
Demo.Courses
{


public class
Course
{


#region EF Compromises
private Guid Id { get; set; }
#endregion


public static readonly string
DefaultName = "A New Course";

public static readonly string
DefaultDescription = "";

public static readonly decimal
DefaultCost = 0;


public
Course()
    : this(DefaultName)
{
}


public
Course(string name)
{
    this.Id = Guid.NewGuid();
    this.Repository = null;
    this.Name = name;
    this.Description = DefaultDescription;
    this.Cost = DefaultCost;
    this.Created = DateTimeOffset.Now;
    this.Modules = new List<Module>();
}


/// <summary>
/// The repository this course belongs to
/// </summary>
/// <remarks>
/// <c>null</c> if this course doesn't belong to a repository
/// </remarks>
///
public ICourseRepository
Repository
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
        if (Repository != null && Repository.All.Any(c => c.Name == value))
            throw new ArgumentOutOfRangeException(
                "value",
                "Another course with the specified name already exists in the repository");
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


[Range(0, 100000)]
public decimal
Cost
{
    get { return cost; }
    set
    {
        Check.Range(0, 100000, value, "value");
        Check.MaxDecimalPlaces(2, value, "value");
        if(value == cost) return;
        Modified = DateTimeOffset.Now;
        cost = value;
    }
}

private decimal cost { get; set; }


/// <summary>
/// Total duration of the course in days
/// </summary>
///
public int
Duration
{
    get
    {
        return Convert.ToInt32(Modules.Sum(m => m.Duration));
    }
}


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


//
// TODO
// Avoid exposing a mutable collection
//
public ICollection<Module>
Modules
{
    get;
    private set;
}


public void
AddModule(Module module)
{
    Check.Required(module, "module");
    if (module.Course != null && module.Course != this)
        throw new ArgumentOutOfRangeException(
            "module",
            "The specified module already in another course");
    if (module.Course == this)
        throw new InvalidOperationException(
            "This course already contains the specified module");
    if (Modules.Any(m => m.Name == module.Name))
        throw new InvalidOperationException(
            "This course already contains a module with the same name");
    Modules.Add(module);
    module.Course = this;
}


public void
RemoveModule(Module module)
{
    Check.Required(module, "module");
    if (!Modules.Contains(module))
        throw new InvalidOperationException(
            "This course doesn't contain the specified module");
    Modules.Remove(module);
    module.Course = null;
}


public Course
CopyShallowTo(Course course)
{
    Check.Required(course, "course");
    course.Name = Name;
    course.Description = Description;
    course.Cost = Cost;
    return course;
}


}
}

