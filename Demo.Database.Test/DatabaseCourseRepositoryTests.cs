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
using System.Data.Entity;
using EFDatabase = System.Data.Entity.Database;
using Com.Halfdecent.Testing;
using Demo.Courses;
using Demo.Database;


namespace
Demo.Database.Test
{


public class
DatabaseCourseRepositoryTests
    : TestBase
{


static void
Setup()
{
    EFDatabase.SetInitializer<DemoDbContext>(
        new DropCreateDatabaseIfModelChanges<DemoDbContext>());
}


//
// Assume behaviour provided by CourseRepositoryBase has been tested elsewhere.
//
// Here we only test behaviour specific to DatabaseCourseRepository.  This
// mainly means checking for correct behaviour immediately and after
// round-tripping through the database.
//


[Test]
public static void
Add()
{
    Setup();
    string name = Guid.NewGuid().ToString("N");
    DatabaseCourseRepository r;
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        var course = new Course(name);
        course.AddModule(new Module("m1"));
        course.AddModule(new Module("m2"));
        r.Add(course);
        Assert(r.All.Where(c => c.Name == name).Single().Modules.Count == 2);
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        Assert(r.All.Where(c => c.Name == name).Single().Modules.Count == 2);
    }
}


[Test]
public static void
LoadingFromDBDoesntUpdateModified()
{
    Setup();
    string name = Guid.NewGuid().ToString("N");
    DatabaseCourseRepository r;
    Course course;
    DateTimeOffset modified;
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        course = new Course(name);
        modified = course.Modified;
        r.Add(course);
        db.SaveChanges();
    }
    //
    // Only check down to the second because .NET and the RDBMS DateTime
    // resolutions probably aren't the same.  And even if they were the same
    // for some RDBMS', they might not be for others.
    //
    Thread.Sleep(1500);
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        course = r.All.Where(c => c.Name == name).Single();
        Assert(
            course.Modified.Minute == modified.Minute &&
            course.Modified.Second == modified.Second);
    }
}


[Test]
public static void
All()
{
    Setup();
    DatabaseCourseRepository r;
    string name1 = Guid.NewGuid().ToString("N");
    string name2 = Guid.NewGuid().ToString("N");
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        r.Add(new Course(name1));
        r.Add(new Course(name2));
        Assert(r.All.Any(c => c.Name == name1));
        Assert(r.All.Any(c => c.Name == name2));
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        Assert(r.All.Any(c => c.Name == name1));
        Assert(r.All.Any(c => c.Name == name2));
    }
}


[Test]
public static void
Remove()
{
    DatabaseCourseRepository r;
    Setup();
    string name = Guid.NewGuid().ToString("N");
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        var course = new Course(name);
        r.Add(course);
        r.Remove(course);
        Assert(!r.All.Any(c => c.Name == name));
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        var course = new Course(name);
        r.Add(course);
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        var course = r.All.Single(c => c.Name == name);
        r.Remove(course);
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        Assert(!r.All.Any(c => c.Name == name));
    }
}


[Test]
public static void
RemoveModule()
{
    Setup();
    string name = Guid.NewGuid().ToString("N");
    DatabaseCourseRepository r;
    Course course;
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        course = new Course(name);
        course.AddModule(new Module("m1"));
        course.AddModule(new Module("m2"));
        r.Add(course);
        Assert(r.All.Where(c => c.Name == name).Single().Modules.Count == 2);
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        course = r.All.Where(c => c.Name == name).Single();
        course.RemoveModule(course.Modules.First());
        db.SaveChanges();
    }
    using(var db = new DemoDbContext())
    {
        r = new DatabaseCourseRepository(db);
        course = r.All.Where(c => c.Name == name).Single();
        Assert(course.Modules.Count == 1);
    }
}


}
}

