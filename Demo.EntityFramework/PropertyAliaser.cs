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
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using QueryInterceptor;
using Demo.System;


namespace
Demo.EntityFramework
{


/// <summary>
/// Mechanism for redirecting EF read/writes to alternate properties
/// </summary>
/// <remarks>
/// Hack to get EF to "redirect" read/write to alternate properties.  Can be
/// used e.g. to get it to read/write to private backing properties.
///
/// 1) In your DbContext's OnModelCreating(), configure aliased properties
///    using AliasedProperty() where you'd normally use Property().  You can
///    continue to configure the aliased property fluently as usual.
///
/// 2) When you expose your DbSet<T>'s for querying, .RewriteAliasedProperties()
///    them on the way out.  This intercepts and rewrites aliased property
///    in query expressions.
///
/// 3) ???
///
/// 4) Profit!
/// </remarks>
///
public static class
PropertyAliaser
{


public static StringPropertyConfiguration
AliasedProperty<
    TEntity>(
    this EntityTypeConfiguration<TEntity>   entityConfig,
    Expression<Func<TEntity, string>>       realExpression,
    string                                  aliasName)
where TEntity
    : class
{
    PropertyInfo realInfo;
    StringPropertyConfiguration aliasConfig;

    Alias<
        TEntity,
        string,
        StringPropertyConfiguration>(
        entityConfig,
        realExpression,
        aliasName,
        pe => entityConfig.Property(pe),
        out realInfo,
        out aliasConfig);

    var stringLength = realInfo.GetCustomAttribute<StringLengthAttribute>();
    if(stringLength != null)
    {
        aliasConfig.HasMaxLength(stringLength.MaximumLength);
    }

    //
    // TODO Any other string-specific validation attributes?
    //

    return aliasConfig;
}


public static DecimalPropertyConfiguration
AliasedProperty<
    TEntity>(
    this EntityTypeConfiguration<TEntity>   entityConfig,
    Expression<Func<TEntity, decimal>>      realExpression,
    string                                  aliasName)
where TEntity
    : class
{
    PropertyInfo realInfo;
    DecimalPropertyConfiguration aliasConfig;

    Alias<
        TEntity,
        decimal,
        DecimalPropertyConfiguration>(
        entityConfig,
        realExpression,
        aliasName,
        pe => entityConfig.Property(pe),
        out realInfo,
        out aliasConfig);

    //
    // TODO Any numeric validation attributes?
    //

    return aliasConfig;
}


/// <summary>
/// IQueryable<T> wrapper which rewrites aliased property accesses in query
/// expressions
/// <summary>
///
public static IQueryable<T>
RewriteAliasedProperties<T>(this IQueryable<T> queryable)
{
    Check.Required(queryable, "queryable");
    return queryable.InterceptWith(new QueryVisitor());
}


//
// TODO
// Alias() overloads to match the rest of the Property() overloads in
// EntityTypeConfiguration<T> and StructuralTypeConfiguration<T>
//


private static void
Alias<
    TEntity,
    T,
    TPropertyConfiguration>(
    EntityTypeConfiguration<TEntity>        entityConfig,
    Expression<Func<TEntity, T>>            realExpression,
    string                                  aliasName,
    Func<Expression<Func<TEntity, T>>,TPropertyConfiguration>
                                            propertyConfigFunc,
    out PropertyInfo                        realInfo,
    out TPropertyConfiguration              aliasConfig)
where TEntity
    : class
where TPropertyConfiguration
    : PrimitivePropertyConfiguration
{
    Check.Required(entityConfig, "entityConfig");
    Check.Required(realExpression, "realExpression");
    Check.Required(aliasName, "aliasName");
    Check.Required(propertyConfigFunc, "propertyConfigFunc");

    //
    // First, tell EF not to map the real property.  This has the side-effect
    // of getting it to sanity-check the real property lambda for us.
    //
    entityConfig.Ignore(realExpression);

    //
    // Dig PropertyInfo and (possibly) conversion operator out of the real
    // property lambda
    //
    MethodInfo realConversion;

    EFUtilities.GetPropertyInfoAndConversionFromPropertyLambda(
        realExpression,
        out realInfo,
        out realConversion);

    string realName = realInfo.Name;
    Type realType = realInfo.PropertyType;

    //
    // Get the alias PropertyInfo
    //
    var aliasInfo = EFUtilities.GetMappablePropertyInfo(
        typeof(TEntity),
        realType,
        aliasName);

    //
    // Build a lambda to the alias property, going through the same conversion
    // operator as the real one (if present)
    //
    var aliasExpression = EFUtilities.MakeLambdaToProperty<TEntity,T>(
        aliasInfo,
        realConversion);

    //
    // Tell EF to map the alias property to the real property's DB column
    //
    aliasConfig = propertyConfigFunc(aliasExpression);
    aliasConfig.HasColumnName(realName);

    //
    // Apply alias column settings from attributes on the real property
    //
    if(realInfo.GetCustomAttributes<RequiredAttribute>().Any())
    {
        aliasConfig.IsRequired();
    }

    //
    // Remember the property mapping so we can duplicate it when querying
    //
    lock(realToAlias)
        realToAlias[realInfo] = aliasInfo;
}


/// <summary>
/// Rewrite aliased property accesses in query expressions
/// <summary>
///
class
QueryVisitor
    : ExpressionVisitor
{
    protected override Expression
    VisitMember(MemberExpression e)
    {
        var real = e.Member as PropertyInfo;
        if(real == null) goto done;
        PropertyInfo alias;
        lock(realToAlias)
            if(!realToAlias.TryGetValue(real, out alias)) goto done;
        e = Expression.MakeMemberAccess(e.Expression, alias);
    done:
        return base.VisitMember(e);
    }
}


static IDictionary<PropertyInfo, PropertyInfo>
realToAlias = new Dictionary<PropertyInfo, PropertyInfo>();


}
}

