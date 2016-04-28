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
using System.Web.Mvc;
using Demo.System;
using Demo.Courses;
using Demo.Database;


namespace
Demo.Web.Controllers
{


public class
CoursesController
    : Controller
{


public ActionResult Index()
{
    using(var db = new DemoDbContext())
    {
        return View(
            new DatabaseCourseRepository(db)
                .All
                .OrderBy(c => c.Name)
                .ToList());
    }
}


public ActionResult Course(string courseName)
{
    courseName = (courseName ?? "").Trim();
    bool creating = (courseName == "");
    bool updating = !creating;
    ViewBag.CourseName = courseName;
    ViewBag.Creating = creating;
    ViewBag.Updating = false;

    if (creating)
        return View(new Course());

    using(var db = new DemoDbContext())
    {
        var course =
            new DatabaseCourseRepository(db)
                .All
                .Where(c => c.Name == courseName)
                .SingleOrDefault();
        if (course == null)
            return HttpNotFound();
        return View(course);
    }
}


[HttpPost]
public ActionResult Course(Course course)
{
    string courseName = (RouteData.Values["courseName"] ?? "").ToString().Trim();
    string action = (Request.Form["action"] ?? "").Trim().ToLowerInvariant();

    bool deleting = (action == "delete" && courseName != "");
    bool creating = (!deleting && courseName == "");
    bool updating = (!deleting && !creating);
    bool cancelling = (action == "cancel");

    ViewBag.CourseName = courseName;
    ViewBag.Creating = creating;
    ViewBag.Updating = updating;

    if (cancelling)
    {
        if (creating)
            return RedirectToRoute("Courses");
        else
            return RedirectToRoute("Course", new { courseName = courseName });
    }

    if (!ModelState.IsValid)
        return View(course);

    using(var db = new DemoDbContext())
    {
        ICourseRepository courses = new DatabaseCourseRepository(db);
        Course existing =
            !creating
                ? courses.All.Single(c => c.Name == courseName)
                : null;

        if (deleting)
        {
            courses.Remove(existing);
            db.SaveChanges();
            return RedirectToRoute("Courses");
        }

        try
        {
            if (creating)
            {
                courses.Add(course);
            }
            else
            {
                course.CopyShallowTo(existing);
            }
        }
        catch (InvalidOperationException ioe)
        {
            ModelState.AddModelError("", ioe.Message);
        }
        catch (ArgumentException ae)
        {
            ModelState.AddModelError("", ae.Message);
        }

        if (!ModelState.IsValid)
            return View(course);

        db.SaveChanges();
        return RedirectToRoute("Course", new { courseName = course.Name });
    }
}


public ActionResult Module(string courseName, string moduleName)
{
    courseName = (courseName ?? "").Trim();
    moduleName = (moduleName ?? "").Trim();

    if (courseName == "")
        return HttpNotFound();

    bool creating = (moduleName == "");
    bool updating = !creating;

    ViewBag.CourseName = courseName;
    ViewBag.ModuleName = moduleName;
    ViewBag.Creating = creating;
    ViewBag.Updating = false;

    if (creating)
        return View(new Module());

    using(var db = new DemoDbContext())
    {
        var course =
            new DatabaseCourseRepository(db)
                .All
                .Where(c => c.Name == courseName)
                .SingleOrDefault();
        if (course == null)
            return HttpNotFound();
        var module =
            course
                .Modules
                .Where(m => m.Name == moduleName)
                .SingleOrDefault();
        if (module == null)
            return HttpNotFound();
        return View(module);
    }

}


[HttpPost]
public ActionResult Module(Module module)
{
    string courseName = (RouteData.Values["courseName"] ?? "").ToString().Trim();
    string moduleName = (RouteData.Values["moduleName"] ?? "").ToString().Trim();
    string action = (Request.Form["action"] ?? "").Trim().ToLowerInvariant();

    if (courseName == "")
        return HttpNotFound();

    bool deleting = (action == "delete" && moduleName != "");
    bool creating = (!deleting && moduleName == "");
    bool updating = (!deleting && !creating);
    bool cancelling = (action == "cancel");

    ViewBag.CourseName = courseName;
    ViewBag.ModuleName = moduleName;
    ViewBag.Creating = creating;
    ViewBag.Updating = updating;

    if (cancelling)
    {
        if (creating)
            return RedirectToRoute("Course", new { courseName = courseName });
        else
            return RedirectToRoute("Module", new { courseName = courseName, moduleName = moduleName });
    }

    if (!ModelState.IsValid)
        return View(module);

    using(var db = new DemoDbContext())
    {
        var course =
            new DatabaseCourseRepository(db)
                .All
                .Where(c => c.Name == courseName)
                .SingleOrDefault();
        if (course == null)
            return HttpNotFound();

        Module existing =
            !creating
                ? course.Modules.Single(c => c.Name == moduleName)
                : null;

        if (deleting)
        {
            course.RemoveModule(existing);
            db.SaveChanges();
            return RedirectToRoute("Course", new { courseName = courseName });
        }

        try
        {
            if (creating)
            {
                course.AddModule(module);
            }
            else
            {
                module.CopyShallowTo(existing);
            }
        }
        catch (InvalidOperationException ioe)
        {
            ModelState.AddModelError("", ioe.Message);
        }
        catch (ArgumentException ae)
        {
            ModelState.AddModelError("", ae.Message);
        }

        if (!ModelState.IsValid)
            return View(module);

        db.SaveChanges();
        return RedirectToRoute("Module", new { courseName = courseName, moduleName = module.Name });
    }
}


}
}

