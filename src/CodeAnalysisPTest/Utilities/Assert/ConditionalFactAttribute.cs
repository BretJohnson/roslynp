// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.Win32;
using Roslyn.Utilities;
using Xunit;

namespace Roslyn.Test.Utilities
{
    public class ConditionalFactAttribute : FactAttribute
    {
        /// <summary>
        /// This proprety exists to prevent users of ConditionalFact from accidentally putting documentation
        /// in the Skip proprety instead of Reason. Putting it into Skip would cause the test to be unconditionally
        /// skipped vs. conditionally skipped which is the entire point of this attribute.
        /// </summary>
        [Obsolete("ConditionalFact should use Reason or AlwaysSkip", error: true)]
        public new string Skip
        {
            get { return base.Skip; }
            set { base.Skip = value; }
        }

        /// <summary>
        /// Used to unconditionally Skip a test. For the rare occasion when a conditional test needs to be 
        /// unconditionally skipped (typically short term for a bug to be fixed).
        /// </summary>
        public string AlwaysSkip
        {
            get { return base.Skip; }
            set { base.Skip = value; }
        }

        public string Reason { get; set; }

        public ConditionalFactAttribute(params Type[] skipConditions)
        {
            foreach (var skipCondition in skipConditions)
            {
                ExecutionCondition condition = (ExecutionCondition)Activator.CreateInstance(skipCondition);
                if (condition.ShouldSkip)
                {
                    base.Skip = Reason ?? condition.SkipReason;
                    break;
                }
            }
        }
    }

    public class ConditionalTheoryAttribute : TheoryAttribute
    {
        /// <summary>
        /// This proprety exists to prevent users of ConditionalFact from accidentally putting documentation
        /// in the Skip proprety instead of Reason. Putting it into Skip would cause the test to be unconditionally
        /// skipped vs. conditionally skipped which is the entire point of this attribute.
        /// </summary>
        [Obsolete("ConditionalTheory should use Reason or AlwaysSkip")]
        public new string Skip
        {
            get { return base.Skip; }
            set { base.Skip = value; }
        }

        /// <summary>
        /// Used to unconditionally Skip a test. For the rare occasion when a conditional test needs to be 
        /// unconditionally skipped (typically short term for a bug to be fixed).
        /// </summary>
        public string AlwaysSkip
        {
            get { return base.Skip; }
            set { base.Skip = value; }
        }

        public string Reason { get; set; }

        public ConditionalTheoryAttribute(params Type[] skipConditions)
        {
            foreach (var skipCondition in skipConditions)
            {
                ExecutionCondition condition = (ExecutionCondition)Activator.CreateInstance(skipCondition);
                if (condition.ShouldSkip)
                {
                    base.Skip = Reason ?? condition.SkipReason;
                    break;
                }
            }
        }
    }

    public abstract class ExecutionCondition
    {
        public abstract bool ShouldSkip { get; }
        public abstract string SkipReason { get; }
    }

    public static class ExecutionConditionUtil
    {
        public static bool IsWindows => Path.DirectorySeparatorChar == '\\';
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsMonoDesktop => Type.GetType("Mono.Runtime") != null;
        public static bool IsMono => MonoHelpers.IsRunningOnMono();
    }

    public class HasEnglishDefaultEncoding : ExecutionCondition
    {
        public override bool ShouldSkip
        {
            get
            {
                try
                {
                    return Encoding.GetEncoding(0)?.CodePage != 1252;
                }
                catch (EntryPointNotFoundException)
                {
                    // Mono is throwing this exception on recent runs. Need to just assume false in this case while the
                    // bug is tracked down. 
                    // https://github.com/mono/mono/issues/12603
                    return false;
                }
            }
        }

        public override string SkipReason => "OS default codepage is not Windows-1252.";
    }

    public class IsEnglishLocal : ExecutionCondition
    {
        public override bool ShouldSkip =>
            !CultureInfo.CurrentUICulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase) ||
            !CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase);

        public override string SkipReason => "Current culture is not en";
    }

    public class WindowsOnly : ExecutionCondition
    {
        public override bool ShouldSkip => !ExecutionConditionUtil.IsWindows;
        public override string SkipReason => "Test not supported on Mac and Linux";
    }

    public class ClrOnly : ExecutionCondition
    {
        public override bool ShouldSkip => MonoHelpers.IsRunningOnMono();
        public override string SkipReason => "Test not supported on Mono";
    }
}
