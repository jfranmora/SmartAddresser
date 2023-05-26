using System;
using System.Text.RegularExpressions;

namespace SmartAddresser.Editor.Foundation.SemanticVersioning
{
    /// <summary>
    ///     <see cref="IVersionExpressionParser" /> implementation like Unity's Version Define expressions
    ///     (https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html).
    /// </summary>
    public sealed class UnityVersionExpressionParser : IVersionExpressionParser
    {
        public bool TryCreateComparator(string expression, out CompositeVersionComparator result)
        {
            try
            {
                result = CreateComparator(expression);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public CompositeVersionComparator CreateComparator(string expression)
        {
            var split = expression.Split(',');
            switch (split.Length)
            {
                case 1:
                    return CreateSingleVersionComparer(expression);
                case 2:
                    return CreateVersionRangeComparer(expression);
                default:
                    throw new ArgumentException($"Invalid format expression: {expression}");
            }
        }

        private static CompositeVersionComparator CreateSingleVersionComparer(string expression)
        {
            var result = new CompositeVersionComparator();
            var regex = new Regex(@"^\[(.+)\]");
            var match = regex.Match(expression);
            var versionStr = match.Success ? match.Groups[1].Value : expression;
            var comparatorOperator = match.Success
                ? VersionComparator.Operator.Equal
                : VersionComparator.Operator.GreaterThanOrEqual;

            if (Version.TryCreate(versionStr, out var minVersion))
                result.Add(new VersionComparator(minVersion, comparatorOperator));
            else
                throw new ArgumentException($"Invalid format expression: {expression}");

            return result;
        }

        private static CompositeVersionComparator CreateVersionRangeComparer(string expression)
        {
            var result = new CompositeVersionComparator();
            var split = expression.Split(',');

            // Create minimum version comparer.
            var firstChar = expression[0];
            var minVersionStr = split[0].Substring(1, split[0].Length - 1);

            VersionComparator.Operator minVersionOperator;
            switch (firstChar)
            {
                case '[':
                    minVersionOperator = VersionComparator.Operator.GreaterThanOrEqual;
                    break;
                case '(':
                    minVersionOperator = VersionComparator.Operator.GreaterThan;
                    break;
                default:
                    throw new ArgumentException($"Invalid format expression: {expression}");
            }

            if (Version.TryCreate(minVersionStr, out var minVersion))
                result.Add(new VersionComparator(minVersion, minVersionOperator));
            else
                throw new ArgumentException($"Invalid format expression: {expression}");

            // Create max version comparer.
            var lastChar = expression[expression.Length - 1];
            var maxVersionStr = split[1].Substring(0, split[1].Length - 1);

            VersionComparator.Operator maxVersionOperator;
            switch (lastChar)
            {
                case ']':
                    maxVersionOperator = VersionComparator.Operator.LessThanOrEqual;
                    break;
                case ')':
                    maxVersionOperator = VersionComparator.Operator.LessThan;
                    break;
                default:
                    throw new ArgumentException($"Invalid format expression: {expression}");
            }

            if (Version.TryCreate(maxVersionStr, out var maxVersion))
                result.Add(new VersionComparator(maxVersion, maxVersionOperator));
            else
                throw new ArgumentException($"Invalid format expression: {expression}");

            return result;
        }
    }
}
