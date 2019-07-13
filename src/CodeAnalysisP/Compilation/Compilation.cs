// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysisP.Collections;
using Microsoft.CodeAnalysisP.Diagnostics;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysisP
{
    /// <summary>
    /// The compilation object is an immutable representation of a single invocation of the
    /// compiler. Although immutable, a compilation is also on-demand, and will realize and cache
    /// data as necessary. A compilation can produce a new compilation from existing compilation
    /// with the application of small deltas. In many cases, it is more efficient than creating a
    /// new compilation from scratch, as the new compilation can reuse information from the old
    /// compilation.
    /// </summary>
    public abstract class Compilation
    {
        /// <summary>
        /// Returns true if this is a case sensitive compilation, false otherwise.  Case sensitivity
        /// affects compilation features such as name lookup as well as choosing what names to emit
        /// when there are multiple different choices (for example between a virtual method and an
        /// override).
        /// </summary>
        public abstract bool IsCaseSensitive { get; }

        /// <summary>
        /// Used for test purposes only to emulate missing members.
        /// </summary>
        private SmallDictionary<int, bool> _lazyMakeWellKnownTypeMissingMap;

        /// <summary>
        /// Used for test purposes only to emulate missing members.
        /// </summary>
        private SmallDictionary<int, bool> _lazyMakeMemberMissingMap;

        // Protected for access in CSharpCompilation.WithAdditionalFeatures
        protected readonly IReadOnlyDictionary<string, string> _features;

        internal Compilation(
            string name,
            IReadOnlyDictionary<string, string> features,
            bool isSubmission,
            AsyncQueue<CompilationEvent> eventQueue)
        {
            Debug.Assert(features != null);

            this.AssemblyName = name;
            this.EventQueue = eventQueue;

            _features = features;
        }

        /// <summary>
        /// Creates a new compilation equivalent to this one with different symbol instances.
        /// </summary>
        public Compilation Clone()
        {
            return CommonClone();
        }

        protected abstract Compilation CommonClone();

        /// <summary>
        /// Returns a new compilation with a given event queue.
        /// </summary>
        internal abstract Compilation WithEventQueue(AsyncQueue<CompilationEvent> eventQueue);

        #region Name

        internal const string UnspecifiedModuleAssemblyName = "?";

        /// <summary>
        /// Simple assembly name, or null if not specified.
        /// </summary>
        /// <remarks>
        /// The name is used for determining internals-visible-to relationship with referenced assemblies.
        ///
        /// If the compilation represents an assembly the value of <see cref="AssemblyName"/> is its simple name.
        ///
        /// Unless <see cref="CompilationOptions.ModuleName"/> specifies otherwise the module name
        /// written to metadata is <see cref="AssemblyName"/> with an extension based upon <see cref="CompilationOptions.OutputKind"/>.
        /// </remarks>
        public string AssemblyName { get; }

        #endregion

        #region Syntax Trees

        /// <summary>
        /// Gets the syntax trees (parsed from source code) that this compilation was created with.
        /// </summary>
        public IEnumerable<SyntaxTree> SyntaxTrees { get { return CommonSyntaxTrees; } }
        protected abstract IEnumerable<SyntaxTree> CommonSyntaxTrees { get; }

        /// <summary>
        /// Creates a new compilation with additional syntax trees.
        /// </summary>
        /// <param name="trees">The new syntax trees.</param>
        /// <returns>A new compilation.</returns>
        public Compilation AddSyntaxTrees(params SyntaxTree[] trees)
        {
            return CommonAddSyntaxTrees(trees);
        }

        /// <summary>
        /// Creates a new compilation with additional syntax trees.
        /// </summary>
        /// <param name="trees">The new syntax trees.</param>
        /// <returns>A new compilation.</returns>
        public Compilation AddSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            return CommonAddSyntaxTrees(trees);
        }

        protected abstract Compilation CommonAddSyntaxTrees(IEnumerable<SyntaxTree> trees);

        /// <summary>
        /// Creates a new compilation without the specified syntax trees. Preserves metadata info for use with trees
        /// added later.
        /// </summary>
        /// <param name="trees">The new syntax trees.</param>
        /// <returns>A new compilation.</returns>
        public Compilation RemoveSyntaxTrees(params SyntaxTree[] trees)
        {
            return CommonRemoveSyntaxTrees(trees);
        }

        /// <summary>
        /// Creates a new compilation without the specified syntax trees. Preserves metadata info for use with trees
        /// added later.
        /// </summary>
        /// <param name="trees">The new syntax trees.</param>
        /// <returns>A new compilation.</returns>
        public Compilation RemoveSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            return CommonRemoveSyntaxTrees(trees);
        }

        protected abstract Compilation CommonRemoveSyntaxTrees(IEnumerable<SyntaxTree> trees);

        /// <summary>
        /// Creates a new compilation without any syntax trees. Preserves metadata info for use with
        /// trees added later.
        /// </summary>
        public Compilation RemoveAllSyntaxTrees()
        {
            return CommonRemoveAllSyntaxTrees();
        }

        protected abstract Compilation CommonRemoveAllSyntaxTrees();

        /// <summary>
        /// Creates a new compilation with an old syntax tree replaced with a new syntax tree.
        /// Reuses metadata from old compilation object.
        /// </summary>
        /// <param name="newTree">The new tree.</param>
        /// <param name="oldTree">The old tree.</param>
        /// <returns>A new compilation.</returns>
        public Compilation ReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree newTree)
        {
            return CommonReplaceSyntaxTree(oldTree, newTree);
        }

        protected abstract Compilation CommonReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree newTree);

        /// <summary>
        /// Returns true if this compilation contains the specified tree. False otherwise.
        /// </summary>
        /// <param name="syntaxTree">A syntax tree.</param>
        public bool ContainsSyntaxTree(SyntaxTree syntaxTree)
        {
            return CommonContainsSyntaxTree(syntaxTree);
        }

        protected abstract bool CommonContainsSyntaxTree(SyntaxTree syntaxTree);

        /// <summary>
        /// The event queue that this compilation was created with.
        /// </summary>
        internal readonly AsyncQueue<CompilationEvent> EventQueue;

        #endregion

        #region Diagnostics

        /// <summary>
        /// Gets the diagnostics produced during the parsing stage.
        /// </summary>
        public abstract ImmutableArray<Diagnostic> GetParseDiagnostics(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the diagnostics produced during symbol declaration.
        /// </summary>
        public abstract ImmutableArray<Diagnostic> GetDeclarationDiagnostics(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the diagnostics produced during the analysis of method bodies and field initializers.
        /// </summary>
        public abstract ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all the diagnostics for the compilation, including syntax, declaration, and
        /// binding. Does not include any diagnostics that might be produced during emit, see
        /// <see cref="EmitResult"/>.
        /// </summary>
        public abstract ImmutableArray<Diagnostic> GetDiagnostics(CancellationToken cancellationToken = default(CancellationToken));

        internal void EnsureCompilationEventQueueCompleted()
        {
            Debug.Assert(EventQueue != null);

            lock (EventQueue)
            {
                if (!EventQueue.IsCompleted)
                {
                    CompleteCompilationEventQueue_NoLock();
                }
            }
        }

        internal void CompleteCompilationEventQueue_NoLock()
        {
            Debug.Assert(EventQueue != null);

            // Signal the end of compilation.
            EventQueue.TryEnqueue(new CompilationCompletedEvent(this));
            EventQueue.PromiseNotToEnqueue();
            EventQueue.TryComplete();
        }

        internal abstract CommonMessageProvider MessageProvider { get; }

        #endregion

        private ConcurrentDictionary<SyntaxTree, SmallConcurrentSetOfInts> _lazyTreeToUsedImportDirectivesMap;
        private static readonly Func<SyntaxTree, SmallConcurrentSetOfInts> s_createSetCallback = t => new SmallConcurrentSetOfInts();

        private ConcurrentDictionary<SyntaxTree, SmallConcurrentSetOfInts> TreeToUsedImportDirectivesMap
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _lazyTreeToUsedImportDirectivesMap);
            }
        }

        internal abstract int GetSyntaxTreeOrdinal(SyntaxTree tree);

        /// <summary>
        /// Compare two source locations, using their containing trees, and then by Span.First within a tree.
        /// Can be used to get a total ordering on declarations, for example.
        /// </summary>
        internal abstract int CompareSourceLocations(Location loc1, Location loc2);

        /// <summary>
        /// Compare two source locations, using their containing trees, and then by Span.First within a tree.
        /// Can be used to get a total ordering on declarations, for example.
        /// </summary>
        internal abstract int CompareSourceLocations(SyntaxReference loc1, SyntaxReference loc2);

        /// <summary>
        /// Return the lexically first of two locations.
        /// </summary>
        internal TLocation FirstSourceLocation<TLocation>(TLocation first, TLocation second)
            where TLocation : Location
        {
            if (CompareSourceLocations(first, second) <= 0)
            {
                return first;
            }
            else
            {
                return second;
            }
        }

        /// <summary>
        /// Return the lexically first of multiple locations.
        /// </summary>
        internal TLocation FirstSourceLocation<TLocation>(ImmutableArray<TLocation> locations)
            where TLocation : Location
        {
            if (locations.IsEmpty)
            {
                return null;
            }

            var result = locations[0];

            for (int i = 1; i < locations.Length; i++)
            {
                result = FirstSourceLocation(result, locations[i]);
            }

            return result;
        }
    }
}
