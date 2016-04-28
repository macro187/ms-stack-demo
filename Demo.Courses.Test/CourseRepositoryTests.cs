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
using Com.Halfdecent.Testing;
using Demo.Courses;


namespace
Demo.Courses.Test
{


public class
CourseRepositoryTests
    : TestBase
{


[Test]
public static void
NewRepositoryContainsNoCourses()
{
    Assert(new CourseRepository().All.Count() == 0);
}


[Test]
public static void
Add()
{
    var r = new CourseRepository();
    r.Add(new Course("AA"));
    Assert(r.All.Count() == 1);
}


[Test]
public static void
AddSetsCourseRepository()
{
    var r = new CourseRepository();
    var course = new Course("AA");
    r.Add(course);
    Assert(r.All.Count() == 1);
    Assert(course.Repository == r);
}


[Test]
public static void
CantReAddCourse()
{
    var r = new CourseRepository();
    var c = new Course("AA");
    r.Add(c);
    Expect<InvalidOperationException>(() =>
        r.Add(c));
}


[Test]
public static void
CantAddCourseWithDuplicateName()
{
    var r = new CourseRepository();
    var c = new Course("AA");
    r.Add(c);
    c = new Course("AA");
    Expect<InvalidOperationException>(() =>
        r.Add(c));
}


[Test]
public static void
CantSetCourseNameToDuplicate()
{
    var r = new CourseRepository();
    var c = new Course("AA");
    r.Add(c);
    c = new Course("BB");
    r.Add(c);
    Expect<ArgumentOutOfRangeException>(() =>
        c.Name = "AA");
}


[Test]
public static void
All()
{
    var r = new CourseRepository();
    r.Add(new Course("AA"));
    r.Add(new Course("BB"));
    Assert(
        r.All
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .SequenceEqual(new string[]{"AA", "BB"}));
}


[Test]
public static void
Remove()
{
    var r = new CourseRepository();
    var c1 = new Course("AA");
    var c2 = new Course("BB");
    r.Add(c1);
    r.Add(c2);
    r.Remove(c1);
    Assert(r.All.Single().Name == "BB");
}


[Test]
public static void
CantRemoveCourseNotInRepository()
{
    Expect<InvalidOperationException>(() =>
        new CourseRepository().Remove(new Course("AA")));
}


[Test]
public static void
RemovingCourseSetsRespositoryToNull()
{
    var r = new CourseRepository();
    var c = new Course("AA");
    r.Add(c);
    r.Remove(c);
    Assert(c.Repository == null);
}


}
}

