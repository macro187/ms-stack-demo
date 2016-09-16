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
using System.Threading;
using Halfdecent.Testing;
using Demo.Courses;


namespace
Demo.Courses.Test
{


public class
CourseTests
    : TestBase
{


[Test]
public static void
Constructor()
{
    new Course("AA");
}


[Test]
public static void
ConstructorSetsCreated()
{
    Assert(new Course("AA").Created != default(DateTimeOffset));
}


//
// Assume constructor name argument validated by Name property, which we test
// later
//


[Test]
public static void
NameGet()
{
    var course = new Course("AA");
    Assert(course.Name == "AA");
}


[Test]
public static void
NameSet()
{
    var course = new Course("AA");
    course.Name = "BB";
    Assert(course.Name == "BB");
}


[Test]
public static void
NameSetToDifferentUpdatesModified()
{
    var course = new Course("AA");
    var then = course.Modified;
    Thread.Sleep(100);
    course.Name = "BB";
    Assert(course.Modified != then);
}


[Test]
public static void
NameSetToSameDoesNotUpdateModified()
{
    var course = new Course("AA");
    var then = course.Modified;
    Thread.Sleep(100);
    course.Name = "AA";
    Assert(course.Modified == then);
}


[Test]
public static void
NameRequired()
{
    var course = new Course("AA");
    Expect<ArgumentNullException>(() =>
        course.Name = null);
    Expect<ArgumentOutOfRangeException>(() =>
        course.Name = "");
    Expect<ArgumentOutOfRangeException>(() =>
        course.Name = " ");
}


[Test]
public static void
NameLengthBetween2And50()
{
    var course = new Course("AA");

    Expect<ArgumentOutOfRangeException>(() =>
        course.Name = "1");
    course.Name = "12";

    course.Name = "12345678901234567890123456789012345678901234567890";
    Expect<ArgumentOutOfRangeException>(() =>
        course.Name = "123456789012345678901234567890123456789012345678901");
}


[Test]
public static void
DescriptionGet()
{
    var course = new Course("AA");
    Assert(course.Description == Course.DefaultDescription);
}


[Test]
public static void
DescriptionSet()
{
    var course = new Course("AA");
    course.Description = "AA";
    Assert(course.Description == "AA");
}


[Test]
public static void
DescriptionRequiredButEmptyAllowed()
{
    var course = new Course("AA");
    Expect<ArgumentNullException>(() =>
        course.Description = null);
    course.Description = "";
}


[Test]
public static void
DescriptionNormalisesNewlines()
{
    var course = new Course("AA");
    course.Description = "1\n2\r3\r\n4";
    Assert(
        course.Description ==
        string.Join(Environment.NewLine, "1", "2", "3", "4"));
}


[Test]
public static void
DescriptionSetToDifferentUpdatesModified()
{
    var course = new Course("AA");
    var then = course.Modified;
    Thread.Sleep(100);
    course.Description = "AA";
    Assert(course.Modified != then);
}


[Test]
public static void
DescriptionSetToSameDoesNotUpdateModified()
{
    var course = new Course("AA");
    var then = course.Modified;
    Thread.Sleep(100);
    course.Description = Course.DefaultDescription;
    Assert(course.Modified == then);
}


[Test]
public static void
CostBetween0And100000()
{
    var course = new Course("AA");
    Expect<ArgumentOutOfRangeException>(() =>
        course.Cost = -1);
    course.Cost = 0;
    course.Cost = 100000;
    Expect<ArgumentOutOfRangeException>(() =>
        course.Cost = 100001);
}


[Test]
public static void
CostDecimalPlaces()
{
    var course = new Course("AA");
    course.Cost = 1.0M;
    course.Cost = 1.1M;
    course.Cost = 1.12M;
    Expect<ArgumentOutOfRangeException>(() =>
        course.Cost = 1.123M);
}


[Test]
public static void
Duration()
{
    var course = new Course("c1");
    Assert(course.Duration == 0);
    course.AddModule(new Module("m1"){Duration = 1});
    Assert(course.Duration == 1);
    course.AddModule(new Module("m2"){Duration = 1});
    Assert(course.Duration == 2);
}


[Test]
public static void
AddModule()
{
    var course = new Course("c1");
    Assert(course.Modules.Count == 0);
    course.AddModule(new Module("m1"));
    Assert(course.Modules.Count == 1);
}


[Test]
public static void
AddModuleSetsModuleCourse()
{
    var course = new Course("c1");
    var module = new Module("m1");
    course.AddModule(module);
    Assert(module.Course == course);
}


[Test]
public static void
RemoveModule()
{
    var course = new Course("c1");
    var module = new Module("m1");
    course.AddModule(module);
    Assert(course.Modules.Count == 1);
    course.RemoveModule(module);
    Assert(course.Modules.Count == 0);
}


[Test]
public static void
RemoveModuleUnsetsModuleCourse()
{
    var course = new Course("c1");
    var module = new Module("m1");
    course.AddModule(module);
    course.RemoveModule(module);
    Assert(module.Course == null);
}


[Test]
public static void
Modules()
{
    var course = new Course("c1");
    course.AddModule(new Module("m1"));
    course.AddModule(new Module("m2"));
    Assert(
        course.Modules
            .Select(m => m.Name)
            .SequenceEqual(new string[]{"m1", "m2"}));
}


[Test]
public static void
CantAddModuleWithDuplicateName()
{
    var course = new Course("c1");
    course.AddModule(new Module("m1"));
    Expect<InvalidOperationException>(() =>
        course.AddModule(new Module("m1")));
}


[Test]
public static void
CantSetAddedModuleNameToDuplicate()
{
    var course = new Course("c1");
    course.AddModule(new Module("m1"));
    var module = new Module("m2");
    course.AddModule(module);
    Expect<ArgumentOutOfRangeException>(() =>
        module.Name = "m1");
}


[Test]
public static void
CopyShallowTo()
{
    var c1 =
        new Course("c1") {
            Description = "description",
            Cost = 100 };
    var c2 = new Course("c2");
    var c3 = c1.CopyShallowTo(c2);
    Assert(c3 == c2);
    Assert(c2.Name == c1.Name);
    Assert(c2.Description == c1.Description);
    Assert(c2.Cost == c1.Cost);
}


}
}

