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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using Demo.System;
using Demo.Courses;


namespace
Demo.Database
{


//
// TODO
// Can we avoid having to SaveChanges() in Add() and Remove()?  Maybe using
// DbSet<T>.Local somehow?
//


public class
DatabaseCourseRepository
    : CourseRepositoryBase
{


public DatabaseCourseRepository(DemoDbContext context)
{
    this.context = context;

    //
    // Set Course.Repository on the way out of the database
    //
    ((IObjectContextAdapter)context).ObjectContext.ObjectMaterialized += 
        OnObjectMaterialized;
}


public override IQueryable<Course>
All
{
    get
    {
        return context.CoursesQueryable;
    }
}


protected override void
DoAdd(Course course)
{
    context.Courses.Add(course);
    context.SaveChanges();
}


protected override void
DoRemove(Course course)
{
    context.Courses.Remove(course);
    context.SaveChanges();
}


private void
OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
{
    var course = e.Entity as Course;
    if (course == null) return;
    SetRepository(course);
}


private DemoDbContext context;


}
}

