using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Scripts are all loaded from external sources and have an inherent risk.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "License file holds required information.")]
