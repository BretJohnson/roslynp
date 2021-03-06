## Welcome the RoslynP (Roslyn Portable) Compiler Platform

[Roslyn](https://github.com/dotnet/roslyn) brings the concept of the compiler as a service to the C# and VB languages. See [here](https://www.instinctools.com/blog/microsoft-roslyn-using-the-compiler-as-a-service) for the history of Roslyn and its motivations.

RoslynP (P for Portable) is a Roslyn subset allowing Roslyn core concepts to be used to build tooling for any language, not just C# and VB. RoslynP brings Roslyn's goodness everywhere.

This project is currently experimental.

### What does RoslynP include?

Currently RoslynP includes functionality for:

- Syntax trees (with immutability via red/green trees): SyntaxTree, SyntaxNode, SyntaxToken, SyntaxTrivia, etc.
- Source code text: SourceText, TextSpan, Location, etc.
- Diagnostics (errors/warnings/info): Diagnostic, DiagnosticSeverity, etc.
- Compilation driver: Compilation (much simplified from normal Roslyn)

### What does RoslynP exclude?

RoslynP is about 1/5th the size of Roslyn. Some of what's excluded:

- Everything C# or VB specific
- Semantic model (`I*Symbol` classes) - we might add this in the future, but it's less generally useful than syntax tree support
- Support for emitting code (IL) - RoslynP is intended to build the language front end; back end support needs are less general

RoslynP adopts the philosophy of starting small-ish and adding more Roslyn-style functionality as needs dictate. That makes the project more manageable. We will bring more from Roslyn to RoslynP as there's a need, if at least one client wants it

### Will the RoslynP and Roslyn projects ever converge?

Perhaps one day. But note that the projects serve different needs - Roslyn is about having a first class C# and VB experience, with the Roslyn team focused on that. RoslynP is for other languages, maintained by its users.

RoslynP was forked off the `release/dev16.1` Roslyn release. It will get occasional updates with Roslyn changes, though the forked code doesn't change often.

### Roslyn getting started, much of which also applies to RoslynP

* [Roslyn Overview](https://github.com/dotnet/roslyn/wiki/Roslyn%20Overview) 
* Tutorial articles by Alex Turner in MSDN Magazine
  - [Use Roslyn to Write a Live Code Analyzer for Your API](https://msdn.microsoft.com/en-us/magazine/dn879356)
  - [Adding a Code Fix to your Roslyn Analyzer](https://msdn.microsoft.com/en-us/magazine/dn904670.aspx)
* [Samples and Walkthroughs](https://github.com/dotnet/roslyn/wiki/Samples-and-Walkthroughs)
* [Documentation](https://github.com/dotnet/roslyn/tree/master/docs)
* [Analyzer documentation](https://github.com/dotnet/roslyn/tree/master/docs/analyzers)
* [Syntax Visualizer Tool](https://github.com/dotnet/roslyn/wiki/Syntax%20Visualizer)
* [Syntax Quoter Tool](http://roslynquoter.azurewebsites.net)
* [FAQ](https://github.com/dotnet/roslyn/wiki/FAQ)
* [Roslyn Wiki Home](https://github.com/dotnet/roslyn/wiki)
