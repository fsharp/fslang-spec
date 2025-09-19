# Program Structure and Execution

F# programs are made up of a collection of assemblies. F# assemblies are made up of static
references to existing assemblies, called the _referenced assemblies_ , and an interspersed sequence of
signature (`.fsi`) files, implementation (`.fs`) files, script (`.fsx` or `.fsscript`) files, and interactively
executed code fragments.

```fsgrammar
implementation-file :=
    namespace-decl-group ... namespace-decl-group
    named-module
    anonynmous-module

script-file := implementation-file  -- script file, additional directives allowed

signature-file :=
    namespace-decl-group-signature ... namespace-decl-group-signature
    anonynmous-module-signature
    named-module-signature

named-module :=
    module long-ident module-elems

anonymous-module :=
    module-elems

named-module-signature :=
    module long-ident module-signature-elements

anonymous-module-signature :=
    module-signature-elements

script-fragment :=
    module-elems                    -- interactively entered code fragment
```

A sequence of implementation and signature files is checked as follows.

1. Form an initial environment `sig-env0` and `impl-env0` by adding all assembly references to the
    environment in the order in which they are supplied to the compiler. This means that the
    following procedure is applied for each referenced assembly:
    - Add the top level types, modules, and namespaces to the environment.
    - For each `AutoOpen` attribute in the assembly, find the types, modules, and namespaces that
       the attribute references and add these to the environment.

    The resulting environment becomes the active environment for the first file to be processed.
2. For each file:
    - If the `i`th file is a signature file `file.fsi`:

        a. Check it against the current signature environment `sig-envi1`, which generates the
        signature `Sigfile` for the current file.

        b. Add `Sigfile` to `sig-envi-1` to produce `sig-envi` to make it available for use in later
        signature files.

        The processing of the signature file has no effect on the implementation environment, so
`impl-envi` is identical to `impl-envi-1`.

    - If the file is an implementation file `file.fs`, check it against the environment `impl-envi-1`,
    which gives elaborated namespace declaration groups `Implfile`.

       a. If a corresponding signature `Sigfile` exists, check `Implfile` against `Sigfile` during this
          process ([§](namespace-and-module-signatures.md#signature-conformance)). Then add `Sigfile` to `impl-envi-1` to produce `impl-envi`. This step makes
          the signature-constrained view of the implementation file available for use in later
          implementation files. The processing of the implementation file has no effect on the
          signature environment, so `sig-envi` is identical to `sig-envi-1`.

       b. If the implementation file has no signature file, add `Implfile` to both `sig-envi-1` and `impl-envi-1`,
          to produce `sig-envi` and `impl-envi`. This makes the contents of the
          implementation available for use in both later signature and implementation files.

The signature file for a particular implementation must occur before the implementation file in the
compilation order. For every signature file, a corresponding implementation file must occur after the
file in the compilation order. Script files may not have signatures.

## Implementation Files

Implementation files consist of one or more namespace declaration groups. For example:

```fsharp
namespace MyCompany.MyOtherLibrary

    type MyType() =
        let x = 1
        member v.P = x + 2

    module MyInnerModule =
        let myValue = 1

namespace MyCompany. MyOtherLibrary.Collections

    type MyCollection(x : int) =
        member v.P = x
```

An implementation file that begins with a `module` declaration defines a single namespace declaration
group with one module. For example:

```fsharp
module MyCompany.MyLibrary.MyModule

let x = 1
```

is equivalent to:

```fsharp
namespace MyCompany.MyLibrary

module MyModule =
    let x = 1
```

The final identifier in the `long-ident` that follows the `module` keyword is interpreted as the module
name, and the preceding identifiers are interpreted as the namespace.

_Anonymous implementation files_ do not have either a leading `module` or `namespace` declaration. Only
the scripts and the last file within an implementation group for an executable image (.exe) may be
anonymous. An anonymous implementation file contains module definitions that are implicitly
placed in a module. The name of the module is generated from the name of the source file by
capitalizing the first letter and removing the filename extensionIf the filename contains characters
that are not valid in an F# identifier, the resulting module name is unusable and a warning occurs.

Given an initial environment `env0`, an implementation file is checked as follows:

- Create a new constraint solving context.
- Check the namespace declaration groups in the file against the existing environment `envi-1` and
    incrementally add them to the environment ([§](namespaces-and-modules.md#namespace-declaration-groups)) to create a new environment `envi`.
- Apply default solutions to any remaining type inference variables that include `default`
    constraints. The defaults are applied in the order that the type variables appear in the type-
    annotated text of the checked namespace declaration groups.
- Check the inferred signature of the implementation file against any required signature by using
    _Signature Conformance_ ([§](namespace-and-module-signatures.md#signature-conformance)). The resulting signature of an implementation file is the required
    signature, if it is present; otherwise it is the inferred signature.
- Report a “value restriction” error if the resulting signature of any item that is not a member,
    constructor, function, or type function contains any free inference type variables.
- Choose solutions for any remaining type inference variables in the elaborated form of an
    expression. Process any remaining type variables in the elaborated form from left-to-right to
    find a minimal type solution that is consistent with constraints on the type variable. If no unique
    minimal solution exists for a type variable, report an error.

The result of checking an implementation file is a set of elaborated namespace declaration groups.

## Signature Files

Signature files specify the functionality that is implemented by a corresponding implementation file.
Each signature file contains a sequence of `namespace-decl-group-signature` elements. The inclusion
of a signature file in compilation implicitly applies that signature type to the contents of a
corresponding implementation file.

_Anonymous signature files_ do not have either a leading `module` or `namespace` declaration. Anonymous
signature files contain `module-elems` that are implicitly placed in a module. The name of the module
is generated from the name of the source file by capitalizing the first letter and removing the
filename extension. If the filename contains characters that are not valid in an F# identifier, the
resulting module name is unusable and a warning occurs.

Given an initial environment `env` , a signature file is checked as follows:

- Create a new constraint solving context.
- Check each `namespace-decl-group-signaturei` in `envi-1` and add the result to that environment to
    create a new environment `envi`.

The result of checking a signature file is a set of elaborated namespace declaration group types.

## Script Files

Script files have the `.fsx` or `.fsscript` filename extension. They are processed in the same way as
files that have the `.fs` extension, with the following exceptions:

- Side effects from all scripts are executed at program startup.
- For script files, the namespace `FSharp.Compiler.Interactive.Settings` is opened by default.
- F# Interactive references the assembly `FSharp.Compiler.Interactive.Settings.dll` by default,
    but the F# compiler does not. If the script uses the script helper `fsi` object, then the script
    should explicitly reference `FSharp.Compiler.Interactive.Settings.dll`.

Script files may add to the set of referenced assemblies by using the `#r` directive ([§](program-structure-and-execution.md#compiler-directives)).

Script files may add other signature, implementation, and script files to the list of sources by using
the `#load` directive. Files are compiled in the same order that was passed to the compiler, except
that each script is searched for `#load` directives and the loaded files are placed before the script, in
the order they appear in the script. If a filename appears in more than one `#load` directive, the file is
placed in the list only once, at the position it first appeared.

Script files may have `#nowarn` directives, which disable a warning for the entire compilation.

The F# compiler defines the `COMPILED` compilation symbol for input files that it has processed. F#
Interactive defines the `INTERACTIVE` symbol.

Script files may not have corresponding signature files.

## Compiler Directives

_Compiler directives_ are declarations in non-nested modules or namespace declaration groups in the
following form:

```fsgrammar
# id string ... string
```

The lexical preprocessor directives `#if`, `#else`, `#endif` and `#indent "off"` are similar to compiler
directives. For details on `#if`, `#else`, `#endif`, see [§](lexical-analysis.md#conditional-compilation). The `#indent "off"` directive is described in
[§](features-for-ml-compatibility.md#file-extensions-and-lexical-matters).

The following directives are valid in all files:

| Directive | Example | Short Description |
| --- | --- | --- |
| `#nowarn` | `#nowarn "54"` | For signature (`.fsi`) files and implementation (`.fs`) files, turns off warnings within this lexical scope.<br>For script (`.fsx` or `.fsscript`) files, turns off warnings globally. |

The following directives are valid in script files:

| Directive | Example | Short Description |
| --- | --- | --- |
| `#r`<br>`#reference` | `#r "System.Core"`<br>`#r @"Nunit.Core.dll"`<br`#r @"c:\NUnit\Nunit.Core.dll"`<br>`#r "nunit.core, Version=2.2.2.0, Culture=neutral,PublicKeyToken=96d09a1eb7f44a77"` | References a DLL within this entire script. |
| `#I`<br>`#Include` | `#I @"c:\Projects\Libraries\Bin"` | Adds a path to the search paths for DLLs that are referenced within this entire script.` |
| `#load #load "library.fs"` | `#load "core.fsi" "core.fs"` | Loads a set of signature and implementation files into the script execution engine. |
| `#time` | `#time`<br>`#time "on"`<br>`#time "off"` | Enables or disables the display of performance information, including elapsed real time, CPU time, and garbage collection information for each section of code that is interpreted and executed. |
| `#help` | `#help` | Asks the script execution environment for help. |
| `#q`<br>`#quit` | `#q`<br>`#quit` | Requests the script execution environment to halt execution and exit. |

## Program Execution

Execution of F# code occurs in the context of an executing CLI program into which one or more
compiled F# assemblies or script fragments is loaded. During execution, the CLI program can use the
functions, values, static members, and object constructors that the assemblies and script fragments
define.

### Execution of Static Initializers

Each implementation file, script file, and script fragment involves a _static initializer_. The execution of
the static initializer is triggered as follows:

- For executable (.exe) files that have an explicit entry point function, the static initializer for the
    last file that appears on the command line is forced immediately as the first action in the
    execution of the entry point function.
- For executable files that have an implicit entry point, the static initializer for the last file that
    appears on the command line is the body of the implicit entry point function.
- For scripts, F# Interactive executes the static initializer for each program fragment immediately.
- For all other implementation files, the static initializer for the file is executed on first access of a
    value that has observable initialization according to the rules that follow, or first access to any
    member of any type in the file that has at least one “static let” or “static do” declaration.

At runtime, the static initializer evaluates, in order, the definitions in the file that have observable
initialization according to the rules that follow. Definitions with observable initialization in nested
modules and types are included in the static initializer for the overall file.

All definitions have observable initialization except for the following definitions in modules:

- Function definitions
- Type function definitions
- Literal definitions
- Value definitions that are generalized to have one or more type variables
- Non-mutable, non-thread-local values that are bound to an _initialization constant expression_ ,
    which is an expression whose elaborated form is one of the following:
  - A simple constant expression.
  - A null expression.
  - A use of the `typeof<_>` or `sizeof<_>` operator from `FSharp.Core.Operators`, or the
       `defaultof<_>` operator from `FSharp.Core.Operators.Unchecked`.
  - A let expression where the constituent expressions are initialization constant expressions.
  - A match expression where the input is an initialization constant expression, each case is a
       test against a constant, and each target is an initialization constant expression.
  - A use of one of the unary or binary operators `=`, `<>`, `<`, `>`, `<=`, `>=`, `+`, `-`, `*` , `<<<`, `>>>`, `|||`, `&&&`, `^^^`,
       `~~~`, `enum<_>`, `not`, `compare`, prefix `–`, and prefix `+` from `FSharp.Core.Operators` on one or two
       arguments, respectively. The arguments themselves must be initialization constant
       expressions, but cannot be operations on decimals or strings. Note that the operators are
       unchecked for arithmetic operations, and that the operators `%` and `/` are not included
       because their use can raise division-by-zero exceptions.
  - A use of a `[<Literal>]` value.
  - A use of a case from an enumeration type.
  - A use of a null case from a union type.
  - A use of a value that is defined in the same assembly and does not have observable
       initialization, or the use of a value that is defined by a `let` or `match` expression within the
       expression itself.

If the execution environment supports the concurrent execution of multiple threads of F# code, each
static initializer runs as a mutual exclusion region. The use of a mutual exclusion region ensures that
if another thread attempts to access a value that has observable initialization, that thread pauses
until static initialization is complete. A static initializer runs only once, on the first thread that
acquires entry to the mutual exclusion region.

Values that have observable initialization have implied CLI fields that are private to the assembly. If
such a field is accessed by using CLI reflection before the execution of the corresponding
initialization code, then the default value for the type of the field will be returned.

Within implementation files, generic types that have static value definitions receive a static initializer
for each generic instantiation. These initializers are executed immediately before the first
dereference of the static fields for the generic type, subject to any limitations present in the specific
CLI implementation in used. If the static initializer for the enclosing file is first triggered during
execution of the static initializer for a generic instantiation, references to static values definition in
the generic class evaluate to the default value.

For example, if external code accesses `data` in this example, the static initializer runs and the
program prints “hello”:

```fsharp
module LibraryModule
printfn "hello"
let data = new Dictionary<int,int>()
```

That is, the side effect of printing “hello” is guaranteed to be triggered by an access to the value
`data`.

If external code calls `id` or accesses `size` in the following example, the execution of the static
initializer is not yet triggered. However if external code calls `f()`, the execution of the static initializer
is triggered because the body refers to the value `data`, which has observable initialization.

```fsharp
module LibraryModule
printfn "hello"
let data = new Dictionary<int,int>()
let size = 3
let id x = x
let f() = data
```

All of the following represent definitions that do not have observable initialization because they are
initialization constant expressions.

```fsharp
let x = System.DayOfWeek.Friday
let x = 1.0
let x = "two"
let x = enum<System.DayOfWeek>(0)
let x = 1 + 1
let x : int list = []
let x : int option = None
let x = compare 1 1
let x = match true with true -> 1 | false -> 2
let x = true && true
let x = 42 >>> 2
let x = typeof<int>
let x = Unchecked.defaultof<int>
let x = Unchecked.defaultof<string>
let x = sizeof<int>
```

### Explicit Entry Point

The last file that is specified in the compilation order for an executable file may contain an explicit
entry point. The entry point is indicated by annotating a function in a module with `EntryPoint`
attribute:

- The `EntryPoint` attribute applies only to a “let”-bound function in a module. The function cannot
    be a member.
- This attribute can apply to only one function, and the function must be the last declaration in the
    last file processed on the command line. The function may be in a nested module.
- The function is asserted to have type `string[] -> int` before type checking. If the assertion fails,
    an error occurs.
- At runtime, the entry point is passed one argument at startup: an array that contains the same
    entries as `System.Environment.GetCommandLineArgs()`, minus the first entry in that array.

The function becomes the entry point to the program. At startup, F# immediately forces execution
of the static initializer for the file in which the function is declared, and then evaluates the body of
the function.
