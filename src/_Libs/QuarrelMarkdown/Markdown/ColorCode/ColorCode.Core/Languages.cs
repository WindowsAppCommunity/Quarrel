// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core
{
    /// <summary>
    /// Provides easy access to ColorSyntax's built-in languages, as well as methods to load and find custom languages.
    /// </summary>
    public static class Languages
    {
        /// <summary>
        /// The <see cref="LanguageRepository"/>.
        /// </summary>
        internal static readonly LanguageRepository LanguageRepository;

        /// <summary>
        /// Set of <see cref="ILanguage"/>s by Id.
        /// </summary>
        internal static readonly Dictionary<string, ILanguage> LoadedLanguages;

        /// <summary>
        /// Set of <see cref="CompiledLanguage"/>s by Id.
        /// </summary>
        internal static readonly Dictionary<string, CompiledLanguage> CompiledLanguages;

        static Languages()
        {
            LoadedLanguages = new Dictionary<string, ILanguage>();
            CompiledLanguages = new Dictionary<string, CompiledLanguage>();
            LanguageRepository = new LanguageRepository(LoadedLanguages);

            Load<JavaScript>();
            Load<Html>();
            Load<CSharp>();
            Load<VbDotNet>();
            Load<Ashx>();
            Load<Asax>();
            Load<Aspx>();
            Load<AspxCs>();
            Load<AspxVb>();
            Load<Sql>();
            Load<Xml>();
            Load<Php>();
            Load<Css>();
            Load<Cpp>();
            Load<Java>();
            Load<PowerShell>();
            Load<Typescript>();
            Load<FSharp>();
            Load<Koka>();
            Load<Haskell>();
            Load<Compilation.Languages.Markdown>();
            Load<Fortran>();
            Load<Diff>();
            Load<Python>();
            Load<Arduino>();
        }

        /// <summary>
        /// Gets an enumerable list of all loaded languages.
        /// </summary>
        public static IEnumerable<ILanguage> All
        {
            get { return LanguageRepository.All; }
        }

        /// <summary>
        /// Gets language support for ASP.NET HTTP Handlers (.ashx files).
        /// </summary>
        /// <value>Language support for ASP.NET HTTP Handlers (.ashx files).</value>
        public static ILanguage Ashx
        {
            get { return LanguageRepository.FindById(LanguageId.Ashx); }
        }

        /// <summary>
        /// Gets language support for ASP.NET application files (.asax files).
        /// </summary>
        /// <value>Language support for ASP.NET application files (.asax files).</value>
        public static ILanguage Asax
        {
            get { return LanguageRepository.FindById(LanguageId.Asax); }
        }

        /// <summary>
        /// Gets language support for ASP.NET pages (.aspx files).
        /// </summary>
        /// <value>Language support for ASP.NET pages (.aspx files).</value>
        public static ILanguage Aspx
        {
            get { return LanguageRepository.FindById(LanguageId.Aspx); }
        }

        /// <summary>
        /// Gets language support for ASP.NET C# code-behind files (.aspx.cs files).
        /// </summary>
        /// <value>Language support for ASP.NET C# code-behind files (.aspx.cs files).</value>
        public static ILanguage AspxCs
        {
            get { return LanguageRepository.FindById(LanguageId.AspxCs); }
        }

        /// <summary>
        /// Gets language support for ASP.NET Visual Basic.NET code-behind files (.aspx.vb files).
        /// </summary>
        /// <value>Language support for ASP.NET Visual Basic.NET code-behind files (.aspx.vb files).</value>
        public static ILanguage AspxVb
        {
            get { return LanguageRepository.FindById(LanguageId.AspxVb); }
        }

        /// <summary>
        /// Gets language support for C#.
        /// </summary>
        /// <value>Language support for C#.</value>
        public static ILanguage CSharp
        {
            get { return LanguageRepository.FindById(LanguageId.CSharp); }
        }

        /// <summary>
        /// Gets language support for HTML.
        /// </summary>
        /// <value>Language support for HTML.</value>
        public static ILanguage Html
        {
            get { return LanguageRepository.FindById(LanguageId.Html); }
        }

        /// <summary>
        /// Gets language support for Java.
        /// </summary>
        /// <value>Language support for Java.</value>
        public static ILanguage Java
        {
            get { return LanguageRepository.FindById(LanguageId.Java); }
        }

        /// <summary>
        /// Gets language support for JavaScript.
        /// </summary>
        /// <value>Language support for JavaScript.</value>
        public static ILanguage JavaScript
        {
            get { return LanguageRepository.FindById(LanguageId.JavaScript); }
        }

        /// <summary>
        /// Gets language support for PowerShell.
        /// </summary>
        /// <value>Language support for PowerShell.</value>
        public static ILanguage PowerShell
        {
            get { return LanguageRepository.FindById(LanguageId.PowerShell); }
        }

        /// <summary>
        /// Gets language support for SQL.
        /// </summary>
        /// <value>Language support for SQL.</value>
        public static ILanguage Sql
        {
            get { return LanguageRepository.FindById(LanguageId.Sql); }
        }

        /// <summary>
        /// Gets language support for Visual Basic.NET.
        /// </summary>
        /// <value>Language support for Visual Basic.NET.</value>
        public static ILanguage VbDotNet
        {
            get { return LanguageRepository.FindById(LanguageId.VbDotNet); }
        }

        /// <summary>
        /// Gets language support for XML.
        /// </summary>
        /// <value>Language support for XML.</value>
        public static ILanguage Xml
        {
            get { return LanguageRepository.FindById(LanguageId.Xml); }
        }

        /// <summary>
        /// Gets language support for PHP.
        /// </summary>
        /// <value>Language support for PHP.</value>
        public static ILanguage Php
        {
            get { return LanguageRepository.FindById(LanguageId.Php); }
        }

        /// <summary>
        /// Gets language support for CSS.
        /// </summary>
        /// <value>Language support for CSS.</value>
        public static ILanguage Css
        {
            get { return LanguageRepository.FindById(LanguageId.Css); }
        }

        /// <summary>
        /// Gets language support for C++.
        /// </summary>
        /// <value>Language support for C++.</value>
        public static ILanguage Cpp
        {
            get { return LanguageRepository.FindById(LanguageId.Cpp); }
        }

        /// <summary>
        /// Gets language support for Diff.
        /// </summary>
        /// <value>Language support for Diff.<value>
        public static ILanguage Diff
        {
            get { return LanguageRepository.FindById(LanguageId.Diff); }
        }

        /// <summary>
        /// Gets language support for Typescript.
        /// </summary>
        /// <value>Language support for typescript.</value>
        public static ILanguage Typescript
        {
            get { return LanguageRepository.FindById(LanguageId.TypeScript); }
        }

        /// <summary>
        /// Gets language support for F#.
        /// </summary>
        /// <value>Language support for F#.</value>
        public static ILanguage FSharp
        {
            get { return LanguageRepository.FindById(LanguageId.FSharp); }
        }

        /// <summary>
        /// Gets language support for Koka.
        /// </summary>
        /// <value>Language support for Koka.</value>
        public static ILanguage Koka
        {
            get { return LanguageRepository.FindById(LanguageId.Koka); }
        }

        /// <summary>
        /// Gets language support for Haskell.
        /// </summary>
        /// <value>Language support for Haskell.</value>
        public static ILanguage Haskell
        {
            get { return LanguageRepository.FindById(LanguageId.Haskell); }
        }

        /// <summary>
        /// Gets language support for Markdown.
        /// </summary>
        /// <value>Language support for Markdown.</value>
        public static ILanguage Markdown
        {
            get { return LanguageRepository.FindById(LanguageId.Markdown); }
        }

        /// <summary>
        /// Gets language support for Fortran.
        /// </summary>
        /// <value>Language support for Fortran.</value>
        public static ILanguage Fortran
        {
            get { return LanguageRepository.FindById(LanguageId.Fortran); }
        }

        /// <summary>
        /// Gets language support for Python.
        /// </summary>
        /// <value>Language support for Python.</value>
        public static ILanguage Python
        {
            get { return LanguageRepository.FindById(LanguageId.Python); }
        }

        /// <summary>
        /// Gets language support for Arduino.
        /// </summary>
        /// <value>Language support for Arduino.</value>
        public static ILanguage Arduino
        {
            get { return LanguageRepository.FindById(LanguageId.Arduino); }
        }

        /// <summary>
        /// Finds a loaded language by the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the language to find.</param>
        /// <returns>An <see cref="ILanguage" /> instance if the specified identifier matches a loaded language; otherwise, null.</returns>
        public static ILanguage FindById(string id)
        {
            return LanguageRepository.FindById(id);
        }

        /// <summary>
        /// Loads the specified language.
        /// </summary>
        /// <param name="language">The language to load.</param>
        /// <remarks>
        /// If a language with the same identifier has already been loaded, the existing loaded language will be replaced by the new specified language.
        /// </remarks>
        public static void Load(ILanguage language)
        {
            LanguageRepository.Load(language);
        }

        private static void Load<T>()
            where T : ILanguage, new()
        {
            Load(new T());
        }
    }
}