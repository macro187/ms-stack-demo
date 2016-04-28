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
using System.Threading;
using Com.Halfdecent.Testing;
using Demo.Courses;


namespace
Demo.Courses.Test
{


public class
ModuleTests
    : TestBase
{


[Test]
public static void
Constructor()
{
    new Module("AA");
}


[Test]
public static void
ConstructorSetsCreated()
{
    Assert(new Module("AA").Created != default(DateTimeOffset));
}


//
// Assume constructor name argument validated by Name property, which we test
// later
//


[Test]
public static void
NameGet()
{
    var module = new Module("AA");
    Assert(module.Name == "AA");
}


[Test]
public static void
NameSet()
{
    var module = new Module("AA");
    module.Name = "BB";
    Assert(module.Name == "BB");
}


[Test]
public static void
NameSetToDifferentUpdatesModified()
{
    var module = new Module("AA");
    var then = module.Modified;
    Thread.Sleep(100);
    module.Name = "BB";
    Assert(module.Modified != then);
}


[Test]
public static void
NameSetToSameDoesNotUpdateModified()
{
    var module = new Module("AA");
    var then = module.Modified;
    Thread.Sleep(100);
    module.Name = "AA";
    Assert(module.Modified == then);
}


[Test]
public static void
NameRequired()
{
    var module = new Module("AA");
    Expect<ArgumentNullException>(() =>
        module.Name = null);
    Expect<ArgumentOutOfRangeException>(() =>
        module.Name = "");
    Expect<ArgumentOutOfRangeException>(() =>
        module.Name = " ");
}


[Test]
public static void
NameLengthBetween2And50()
{
    var module = new Module("AA");

    Expect<ArgumentOutOfRangeException>(() =>
        module.Name = "1");
    module.Name = "12";

    module.Name = "12345678901234567890123456789012345678901234567890";
    Expect<ArgumentOutOfRangeException>(() =>
        module.Name = "123456789012345678901234567890123456789012345678901");
}


[Test]
public static void
DescriptionGet()
{
    var module = new Module("AA");
    Assert(module.Description == Module.DefaultDescription);
}


[Test]
public static void
DescriptionSet()
{
    var module = new Module("AA");
    module.Description = "AA";
    Assert(module.Description == "AA");
}


[Test]
public static void
DescriptionNormalisesNewlines()
{
    var module = new Module("AA");
    module.Description = "1\n2\r3\r\n4";
    Assert(
        module.Description ==
        string.Join(Environment.NewLine, "1", "2", "3", "4"));
}


[Test]
public static void
DescriptionSetToDifferentUpdatesModified()
{
    var module = new Module("AA");
    var then = module.Modified;
    Thread.Sleep(100);
    module.Description = "AA";
    Assert(module.Modified != then);
}


[Test]
public static void
DescriptionSetToSameDoesNotUpdateModified()
{
    var module = new Module("AA");
    var then = module.Modified;
    Thread.Sleep(100);
    module.Description = Module.DefaultDescription;
    Assert(module.Modified == then);
}


[Test]
public static void
DurationBetween1And365()
{
    var module = new Module("AA");
    Expect<ArgumentOutOfRangeException>(() =>
        module.Duration = 0);
    module.Duration = 1;
    module.Duration = 365;
    Expect<ArgumentOutOfRangeException>(() =>
        module.Duration = 366);
}


[Test]
public static void
CopyShallowTo()
{
    var m1 =
        new Module("m1") {
            Description = "description",
            Duration = 5 };
    var m2 = new Module("m2");
    var m3 = m1.CopyShallowTo(m2);
    Assert(m3 == m2);
    Assert(m2.Name == m1.Name);
    Assert(m2.Description == m1.Description);
    Assert(m2.Duration == m1.Duration);
}


}
}

