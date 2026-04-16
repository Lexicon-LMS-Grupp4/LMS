using Domain.Models.Entities;
using System.Linq.Expressions;

namespace LMS.Services.Common;
/// <summary>
/// Helper methods for validating required navigation properties.
/// </summary>
public static class NavProp
{
    /// <summary>
    /// Ensures that a required navigation property is loaded.
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type.</typeparam>
    /// <typeparam name="TProp">The navigation property type.</typeparam>
    /// <param name="entity">The entity that owns the navigation property.</param>
    /// <param name="selector">
    /// An expression selecting the required navigation property.
    /// Only simple member-access expressions are supported.
    /// </param>
    /// <returns>The loaded navigation property.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the selected navigation property is not loaded.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the selector is not a simple member-access expression.
    /// </exception>
    /// <remarks>
    /// This method is intended for validating loaded navigation properties in already materialized entities.
    /// The selector must be a simple member-access expression, for example <c>x => x.Module</c>
    /// or <c>x => x.Activity.Module</c>.
    /// More complex expressions, such as method calls, filtering, indexing, or projections, are not supported.
    /// </remarks>
    /// <example>
    /// <code>
    /// var module = NavProp.RequireLoaded(activity, a => a.Module);
    /// var teachers = NavProp.RequireLoaded(course, c => c.CourseTeachers);
    /// </code>
    /// </example>
    public static TProp RequireLoadedFrom<TEntity, TProp>(
        TEntity entity,
        Expression<Func<TEntity, TProp?>> selector)
        where TEntity : EntityBase
        where TProp : class
    {
        var value = selector.Compile()(entity);
        var path = GetPath(selector.Body);

        return value ?? throw new InvalidOperationException(
            $"{typeof(TEntity).Name} with id {entity.Id} is missing required navigation property '{path}'.");
    }

    /// <summary>
    /// Ensures that a navigation property is loaded when the corresponding foreign key
    /// on the current entity has a value.
    /// </summary>
    /// <typeparam name="TEntity">The owning entity type.</typeparam>
    /// <typeparam name="TKey">The foreign key value type.</typeparam>
    /// <typeparam name="TProp">The navigation property type.</typeparam>
    /// <param name="entity">The entity that owns both the foreign key and the navigation property.</param>
    /// <param name="foreignKeySelector">
    /// A simple member-access expression selecting the foreign key property.
    /// </param>
    /// <param name="navigationSelector">
    /// A simple member-access expression selecting the navigation property.
    /// </param>
    /// <returns>
    /// The loaded navigation property if the foreign key has a value; otherwise <see langword="null" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the foreign key has a value but the selected navigation property is not loaded.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when either selector is not a simple member-access expression.
    /// </exception>
    /// <remarks>
    /// Use this when the current entity contains the foreign key property itself.
    /// If the foreign key is <see langword="null" />, the relation is treated as not applicable
    /// and this method returns <see langword="null" />.
    /// If the foreign key has a value, the corresponding navigation property must be loaded.
    /// Only simple member-access expressions are supported, for example
    /// <c>x => x.ModuleId</c> and <c>x => x.Module</c>.
    /// Method calls, LINQ operators, indexing, and other computed expressions are not supported.
    /// </remarks>
    /// <example>
    /// <code>
    /// var module = NavProp.RequireLoadedIfForeignKeySet(
    ///     document,
    ///     d => d.ModuleId,
    ///     d => d.Module);
    /// </code>
    /// </example>
    public static TProp? RequireLoadedIfForeignKeySet<TEntity, TKey, TProp>(
        TEntity entity,
        Expression<Func<TEntity, TKey?>> foreignKeySelector,
        Expression<Func<TEntity, TProp?>> navigationSelector)
        where TEntity : EntityBase
        where TProp : class
        where TKey : struct
    {
        var fkValue = foreignKeySelector.Compile()(entity);

        if (fkValue is null)
            return null;

        var navValue = navigationSelector.Compile()(entity);

        var fkPath = GetPath(foreignKeySelector.Body);
        var navPath = GetPath(navigationSelector.Body);

        return navValue ?? throw new InvalidOperationException(
            $"{typeof(TEntity).Name} with id {entity.Id} has {fkPath} set but navigation property '{navPath}' is not loaded.");
    }

    /// <summary>
    /// Builds a property path from a simple member-access expression.
    /// </summary>
    /// <param name="expression">The selector expression body.</param>
    /// <returns>The property path.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the expression is not a simple member-access expression.
    /// </exception>
    /// <remarks>
    /// Supports chained member access such as <c>x => x.Module</c> or
    /// <c>x => x.Activity.Module</c>.
    /// Does not support method calls, LINQ operators, indexing, or other computed expressions.
    /// </remarks>
    private static string GetPath(Expression expression)
    {
        var parts = new Stack<string>();

        while (expression is MemberExpression memberExpression)
        {
            parts.Push(memberExpression.Member.Name);
            expression = memberExpression.Expression!;
        }

        if (expression is not ParameterExpression || parts.Count == 0)
        {
            throw new ArgumentException(
                "Selector must be a simple member access expression, for example x => x.Module or x => x.Activity.Module.");
        }

        return string.Join(".", parts);
    }


    ///// <summary>
    ///// Builds an error message for a missing required navigation property.
    ///// </summary>
    ///// <param name="entityName">The entity type name.</param>
    ///// <param name="entityId">The entity id.</param>
    ///// <param name="navPropPath">The navigation property path.</param>
    ///// <returns>The formatted error message.</returns>
    //public static string MissingMessage(string entityName, int entityId, string navPropPath)
    //    => $"{entityName} with id {entityId} is missing required navigation property '{navPropPath}'.";

    ///// <summary>
    ///// Builds an error message for a missing navigation property when its foreign key is set.
    ///// </summary>
    ///// <param name="entityName">The entity type name.</param>
    ///// <param name="entityId">The entity id.</param>
    ///// <param name="foreignKeyName">The foreign key property name.</param>
    ///// <param name="navPropName">The navigation property name.</param>
    ///// <returns>The formatted error message.</returns>
    //public static string MissingForFkMessage(
    //    string entityName,
    //    int entityId,
    //    string foreignKeyName,
    //    string navPropName)
    //    => $"{entityName} with id {entityId} has {foreignKeyName} set but navigation property '{navPropName}' is not loaded.";
}