## Welcome the RoslynP (Roslyn Portable) Compiler Platform

[Roslyn](https://github.com/dotnet/roslyn) brings the concept of the compiler as a service to the C# and VB languages. See [here](https://www.instinctools.com/blog/microsoft-roslyn-using-the-compiler-as-a-service) for more about the history of Roslyn and its motivations.

Roslyn is a very powerful - but it's only for C# and VB. 
RoslynP (P stands for Portable) is a Roslyn subset, allowing Roslyn core concepts to be used to build tooling for any language. RoslynP brings Roslyn's goodness everywhere.

This project is currently experimental.

### What does RoslynP include?

Currently RoslynP includes functionality for:

- Syntax trees (with immutability via red/green trees): SyntaxTree, SyntaxNode, SyntaxToken, SyntaxTrivia, etc.
- Source code text: SourceText, TextSpan, Location, etc.
- Diagnostics (errors/warnings/info): Diagnostic, DiagnosticSeverity, etc.
- Compilation driver: Compilation (much simplified from normal Roslyn)

RoslynP adopts the philosophy of starting small-ish and adding more Roslyn-style functionality as needs dictate. That makes the project more manageable. Syntax tree support is the core, needed by most all languages.

### What does RoslynP not include?

RoslynP removes a lot from traditional Roslyn - it's about 1/5th the size. Some of what's excluded is:

- Everything C# or VB specific
- Semantic model (I*Symbol classes) - we might add this in the future, but it's less generally useful than syntax tree support
- Support for emitting code (IL) - RoslynP is intended to build the language front end; back end support needs are less general

And again, we can add more from Roslyn to RoslynP as there's a need, if at least one client wants it.

### Roslyn getting started, much of which applies also RoslynP

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
* Also take a look at our [Wiki](https://github.com/dotnet/roslyn/wiki) for more information on how to contribute, what the labels on issue mean, etc.

### Contribute!

Some of the best ways to contribute are to try things out, file bugs, and join in design conversations.

* [Pull requests](https://github.com/dotnet/roslyn/pulls): [Open](https://github.com/dotnet/roslyn/pulls?q=is%3Aopen+is%3Apr)/[Closed](https://github.com/dotnet/roslyn/pulls?q=is%3Apr+is%3Aclosed)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).  For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### .NET Foundation

This project is part of the [.NET Foundation](http://www.dotnetfoundation.org/projects) along with other
projects like [the class libraries for .NET Core](https://github.com/dotnet/corefx/).
