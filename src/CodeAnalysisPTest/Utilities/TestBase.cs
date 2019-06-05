// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.CodeAnalysis.Text;

namespace Roslyn.Test.Utilities
{
    /// <summary>
    /// Base class for all unit test classes.
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        private TempRoot _temp;

        protected TestBase()
        {
        }

        public static string GetUniqueName()
        {
            return Guid.NewGuid().ToString("D");
        }

        public TempRoot Temp
        {
            get
            {
                if (_temp == null)
                {
                    _temp = new TempRoot();
                }

                return _temp;
            }
        }

        public virtual void Dispose()
        {
            if (_temp != null)
            {
                _temp.Dispose();
            }
        }

        #region Diagnostics

        internal static DiagnosticDescription Diagnostic(
            object code,
            string squiggledText = null,
            object[] arguments = null,
            LinePosition? startLocation = null,
            Func<SyntaxNode, bool> syntaxNodePredicate = null,
            bool argumentOrderDoesNotMatter = false)
        {
            return TestHelpers.Diagnostic(
                code,
                squiggledText,
                arguments,
                startLocation,
                syntaxNodePredicate,
                argumentOrderDoesNotMatter);
        }

        internal static DiagnosticDescription Diagnostic(
           object code,
           XCData squiggledText,
           object[] arguments = null,
           LinePosition? startLocation = null,
           Func<SyntaxNode, bool> syntaxNodePredicate = null,
           bool argumentOrderDoesNotMatter = false)
        {
            return TestHelpers.Diagnostic(
                code,
                squiggledText,
                arguments,
                startLocation,
                syntaxNodePredicate,
                argumentOrderDoesNotMatter);
        }

        #endregion
    }
}
