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
using Demo.System;


namespace
Demo.Courses
{


public abstract class
CourseRepositoryBase
    : ICourseRepository
{


public abstract IQueryable<Course>
All
{
    get;
}


public void
Add(Course course)
{
    Check.Required(course, "course");
    if (course.Repository != null && course.Repository != this)
        throw new ArgumentOutOfRangeException(
            "course",
            "The specified course is already in another repository");
    if (course.Repository == this)
        throw new InvalidOperationException(
            "This repository already contains the specified course");
    if (All.Any(c => c.Name == course.Name))
        throw new InvalidOperationException(
            "This repository already contains a course with the same name");
    SetRepository(course);
    DoAdd(course);
}


public void
Remove(Course course)
{
    Check.Required(course, "course");
    if (course.Repository != this)
        throw new InvalidOperationException(
            "This repository does not contain the specified Course");
    DoRemove(course);
    ClearRepository(course);
}


protected void
SetRepository(Course course)
{
    Check.Required(course, "course");
    course.Repository = this;
}


protected void
ClearRepository(Course course)
{
    Check.Required(course, "course");
    course.Repository = null;
}


protected abstract void
DoAdd(Course course);


protected abstract void
DoRemove(Course course);


}
}

