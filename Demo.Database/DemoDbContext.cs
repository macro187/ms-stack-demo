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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using Demo.EntityFramework;
using Demo.Courses;


namespace
Demo.Database
{


public class
DemoDbContext
    : DbContext
{


const string
DEFAULT_CONNECTION_STRING = "Server=.;Trusted_Connection=true;Initial Catalog=Demo";


public
DemoDbContext()
    : this(DEFAULT_CONNECTION_STRING)
{
}


public
DemoDbContext(string nameOrConnectionString)
    : base(nameOrConnectionString)
{
    //
    // TODO
    // In real life you'd use migrations
    //
    global::System.Data.Entity.Database.SetInitializer<DemoDbContext>(
        new DropCreateDatabaseIfModelChanges<DemoDbContext>());
}


public DbSet<Course> Courses { get; set; }


public IQueryable<Course> CoursesQueryable
{
    get
    {
        return
            Courses
                .Include(c => c.Modules)
                .RewriteAliasedProperties();
    }
}


public override int
SaveChanges()
{
    //
    // Help EF delete orphans
    //
    var modules = Set<Module>();
    foreach(
        var module
        in modules.Local.Where(m => m.Course == null).ToList())
    {
        modules.Remove(module);
    }

    return base.SaveChanges();
}


protected override void
OnModelCreating(DbModelBuilder modelBuilder)
{
    modelBuilder
        .Entity<Course>()
        .HasKey(
            EFUtilities.MakeLambdaToMappableProperty<Course, Guid>("Id"));

    modelBuilder
        .Entity<Course>()
        .AliasedProperty(c => c.Name, "name")
            .HasColumnAnnotation(
                "Index",
                new IndexAnnotation(new IndexAttribute(){IsUnique = true}));

    modelBuilder.Entity<Course>()
        .AliasedProperty(c => c.Description, "description");

    modelBuilder.Entity<Course>()
        .AliasedProperty(c => c.Cost, "cost");

    modelBuilder
        .Entity<Course>()
        .Ignore(c => c.Duration);

    modelBuilder
        .Entity<Module>()
        .HasKey(
            EFUtilities.MakeLambdaToMappableProperty<Module, Guid>("Id"));

    modelBuilder
        .Entity<Module>()
        .HasRequired(m => m.Course)
        .WithMany(c => c.Modules)
        .Map(m => m.MapKey("CourseId"))
        .WillCascadeOnDelete();

    base.OnModelCreating(modelBuilder);
}


}
}

