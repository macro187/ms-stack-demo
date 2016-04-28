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


using System.Linq;


namespace
Demo.Courses
{


public interface
ICourseRepository
{


IQueryable<Course> All { get; }


/// <exception cref="ArgumentOutOfRangeException"/>
/// The specified <paramref name="course"/> is already in another repository
/// </exception>
/// <exception cref="InvalidOperationException"/>
/// This repository already contains the specified <paramref name="course"/>
/// </exception>
///
void Add(Course course);


/// <exception cref="InvalidOperationException"/>
/// This repository does not contain the specified <paramref name="course"/>
/// </exception>
///
void Remove(Course course);


}
}

