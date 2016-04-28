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
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Demo.System;


namespace
Demo.EntityFramework
{


public static class
EFUtilities
{


/// <summary>
/// Build a property access lambda expression to an EF-mappable property
/// </summary>
/// <remarks>
/// This includes protected or private properties.
/// </remarks>
/// <exception cref="ArgumentOutOfRangeException">
/// <paramref name="type"/> doesn't contain an EF-mappable property of the
/// specified <paramref name="name"/> and <paramref name="propertyType"/>
/// </exception>
///
public static Expression<Func<T, TProperty>>
MakeLambdaToMappableProperty<T, TProperty>(string name)
{
    return MakeLambdaToProperty<T, TProperty>(
        GetMappablePropertyInfo(typeof(T), typeof(TProperty), name),
        null);
}


/// <summary>
/// Build a property access lambda expression
/// </summary>
///
public static Expression<Func<T, TProperty>>
MakeLambdaToProperty<T, TProperty>(
    PropertyInfo    info)
{
    return MakeLambdaToProperty<T, TProperty>(info, null);
}


/// <summary>
/// Build a property access lambda expression, optionally including a unary
/// conversion operator to go from the property type to the lambda type
/// </summary>
///
public static Expression<Func<T, TProperty>>
MakeLambdaToProperty<T, TProperty>(
    PropertyInfo    info,
    MethodInfo      conversion)
{
    Check.Required(info, "info");

    var pe = Expression.Parameter(typeof(T), "p");

    Expression e = Expression.MakeMemberAccess(pe, info);

    if(conversion != null)
    {
        e = Expression.MakeUnary(
            ExpressionType.Convert,
            e,
            typeof(TProperty),
            conversion);
    }

    return Expression.Lambda<Func<T, TProperty>>(e, pe);
}


/// <summary>
/// Extract the PropertyInfo and (possibly) conversion operator from a property
/// access lambda
/// </summary>
///
public static void
GetPropertyInfoAndConversionFromPropertyLambda<T, TProperty>(
    Expression<Func<T, TProperty>>  e,
    out PropertyInfo                propertyInfo,
    out MethodInfo                  conversion)
{
    Check.Required(e, "e");
    conversion = null;

    MemberExpression me = e.Body as MemberExpression;
    UnaryExpression ue = e.Body as UnaryExpression;

    if(ue != null)
    {
        conversion = ue.Method;
        me = ((MemberExpression)ue.Operand) as MemberExpression;
    }

    if(me == null)
        throw new NotImplementedException(
            "Don't know how to find property info in this expression");

    propertyInfo = (PropertyInfo)me.Member;
}


/// <summary>
/// Get PropertyInfo for an EF-mappable property
/// </summary>
/// <remarks>
/// This includes protected or private properties.
/// </remarks>
/// <exception cref="ArgumentOutOfRangeException">
/// <paramref name="type"/> doesn't contain an EF-mappable property of the
/// specified <paramref name="name"/> and <paramref name="propertyType"/>
/// </exception>
///
public static PropertyInfo
GetMappablePropertyInfo(Type type, Type propertyType, string propertyName)
{
    Check.Required(type, "type");
    Check.Required(propertyType, "propertyType");
    Check.Required(propertyName, "propertyName");

    var info =
        type.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            propertyType,
            new Type[0],
            null);

    if(info == null)
        throw new ArgumentOutOfRangeException(
            "propertyName",
            String.Format(
                "Type {0} contains no unindexed property '{1}' of type {2}",
                type.FullName,
                propertyName,
                propertyType.FullName));

    return info;
}


}
}

