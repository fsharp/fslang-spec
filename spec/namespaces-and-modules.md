# Namespaces and Modules

F# is primarily an expression-based language. However, F# source code units are made up of
_declarations_ , some of which can contain further declarations. Declarations are grouped using
_namespace declaration groups_ , _type definitions_ , and _module definitions_. These also have
corresponding forms in _signatures_. For example, a file may contain multiple namespace declaration
groups, each of which defines types and modules, and the types and modules may contain member,
function, and value definitions, which contain expressions.

Declaration elements are processed in the context of an _environment_. The definition of the elements
of an environment is found in [§14.1](inference-procedures.md#name-resolution).

```fsgrammar
namespace-decl-group :=
    namespace long-ident module-elems       -- elements within a namespace
    namespace global module-elems           -- elements within no namespace

module-defn :=
    attributesopt module accessopt ident = module-defn-body

module-defn-body :=
    begin module-elemsopt end

module-elem :=
    module-function-or-value-defn           -- function or value definitions
    type-defns                              -- type definitions
    exception-defn                          -- exception definitions
    module-defn                             -- module definitions
    module-abbrev                           -- module abbreviations
    import-decl                             -- import declarations
    compiler-directive-decl                 -- compiler directives

module-function-or-value-defn :=
    attributesopt let function-defn
    attributesopt let value-defn
    attributesopt let rec opt function-or-value-defns
    attributesopt do expr

import-decl := open long-ident

module-abbrev := module ident = long-ident

compiler-directive-decl := # ident string ... string

module-elems := module-elem ... module-elem

access :=
    private
    internal
    public
```

## Namespace Declaration Groups

Modules and types in an F# program are organized into _namespaces_, which encompass the
identifiers that are defined in the modules and types. New components may contribute entities to
existing namespaces. Each such contribution to a namespace is called a _namespace declaration
group_.

In the following example, the `MyCompany.MyLibrary` namespace contains `Values` and `x`:

```fsharp
namespace MyCompany.MyLibrary

    module Values1 =
        let x = 1
```

A namespace declaration group is the basic declaration unit within an F# implementation file and is
of the form

```fsgrammar
namespace long-ident

module-elems
```

The `long-ident` must be fully qualified. Each such group contains a series of module and type
definitions that contribute to the indicated namespace. An implementation file may contain multiple
namespace declaration groups, as in this example:

```fsharp
namespace MyCompany.MyOtherLibrary

    type MyType() =
        let x = 1
            member v.P = x + 2

    module MyInnerModule =
        let myValue = 1

namespace MyCompany.MyOtherLibrary.Collections

    type MyCollection(x : int) =
        member v.P = x
```

Namespace declaration groups may not be nested.

A namespace declaration group can contain type and module definitions, but not function or value
definitions. For example:

```fsharp
namespace MyCompany.MyLibrary

// A type definition in a namespace
type MyType() =
    let x = 1
    member v.P = x+2

// A module definition in a namespace
    module MyInnerModule =
        let myValue = 1

// The following is not allowed: value definitions are not allowed in namespaces
let addOne x = x + 1
```

When a namespace declaration group `N` is checked in an environment `env` , the individual
declarations are checked in order and an overall _namespace declaration group signature_ `Nsig` is
inferred for the module. An entry for `N` is then added to the _ModulesAndNamespaces_ table in the
environment `env` (see [§14.1.3](inference-procedures.md#opening-modules-and-namespace-declaration-groups)).

Like module declarations, namespace declaration groups are processed sequentially rather than
simultaneously, so that later namespace declaration groups are not in scope when earlier ones are
processed. This prevents invalid recursive definitions.

In the following example, the declaration of `x` in `Module1` generates an error because the
`Utilities.Part2` namespace is not in scope:

```fsharp
namespace Utilities.Part1

module Module1 =
    let x = Utilities.Part2.Module2.x + 1 // error (Part2 not yet declared)

namespace Utilities.Part2

    module Module2 =
        let x = Utilities.Part1.Module1.x + 2
```

Within a namespace declaration group, the namespace itself is implicitly opened if any preceding
namespace declaration groups or referenced assemblies contribute to it. For example:

```fsharp
namespace MyCompany.MyLibrary

    module Values1 =
        let x = 1

namespace MyCompany.MyLibrary

    // Here, the implicit open of MyCompany.MyLibrary brings Values1 into scope
    module Values2 =
        let x = Values1.x
```

## Module Definitions

A module definition is a named collection of declarations such as values, types, and function values.
Grouping code in modules helps keep related code together and helps avoid name conflicts in your
program. For example:

```fsharp
module MyModule =
    let x = 1
    type Foo = A | B
    module MyNestedModule =
        let f y = y + 1
        type Bar = C | D
```

When a module definition `M` is checked in an environment `env0`, the individual declarations are
checked in order and an overall _module signature_ `Msig` is inferred for the module. An entry for `M` is
then added to the _ModulesAndNamespaces_ table to environment `env0` to form the new environment
used for checking subsequent modules.

Like namespace declaration groups, module definitions are processed sequentially rather than
simultaneously, so that later modules are not in scope when earlier ones are processed.

```fsharp
module Part1 =

    let x = Part2.StorageCache() // error (Part2 not yet declared)

module Part2 =

    type StorageCache() =
        member cache.Clear() = ()
```

No two types or modules may have identical names in the same namespace. The
`[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]` attribute adds the
suffix `Module` to the name of a module to distinguish the module name from a type of a similar name.

For example, this is frequently used when defining a type and a set of functions and values to
manipulate values of this type.

```fsharp
type Cat(kind: string) =
    member x.Meow() = printfn "meow"
    member x.Purr() = printfn "purr"
    member x.Kind = kind

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Cat =

    let tabby = Cat "Tabby"
    let purr (c:Cat) = c.Purr()
    let purrTwice (c:Cat) = purr(); purr()

Cat.tabby |> Cat.purr |> Cat.purrTwice
```

### Function and Value Definitions in Modules

Function and value definitionsin modules introduce named values and functions.

```fsgrammar
let rec~opt function-or-value-defn1 and ... and function-or-value-defnn
```

The following example defines value `x` and functions `id` and `fib`:

```fsharp
module M =
    let x = 1
    let id x = x
    let rec fib x = if x <= 2 then 1 else fib (n - 1) + fib (n - 2)
```

Function and value definitions in modules may declare explicit type variables and type constraints:

```fsharp
let pair<'T>(x : 'T) = (x, x)
let dispose<'T when 'T :> System.IDisposable>(x : 'T) = x.Dispose()
let convert<'T, 'U>(x) = unbox<'U>(box<'T>(x))
```

A value definition that has explicit type variables is called a type function ([§10.2.3](namespaces-and-modules.md#type-function-definitions-in-modules)).

Function and value definitions may specify attributes:

```fsharp
// A value definition with the System.Obsolete attribute
[<System.Obsolete("Don't use this")>]
let oneTwoPair = ( 1 , 2 )

// A function definition with an attribute
[<System.Obsolete("Don't use this either")>]
let pear v = (v, v)
```

By the use of pattern matching, a value definition can define more than one value. In such cases,
the attributes apply to each value.

```fsharp
// A value definition that defines two values, each with an attribute
[<System.Obsolete("Don't use this")>]
let (a, b) = (1, 2)
```

Values may be declared mutable:

```fsharp
// A value definition that defines a mutable value
let mutable count = 1
let freshName() = (count <- count + 1; count)
```

Function and value definitions in modules are processed in the same way as function and value
definitions in expressions ([§14.6](inference-procedures.md#checking-and-elaborating-function-value-and-member-definitions)), with the following adjustments:

- Each defined value may have an accessibility annotation ([§10.5](namespaces-and-modules.md#accessibility-annotations)). By default, the accessibility
    annotation of a function or value definition in a module is `public`.
- Each defined value is _externally accessible_ if its accessibility annotation is `public` and it is not
    hidden by an explicit signature. Externally accessible values are guaranteed to have compiled CLI
    representations in compiled CLI binaries.
- Each defined value can be used to satisfy the requirements of any signature for the module
    ([§11.2](namespace-and-module-signatures.md#signature-conformance)).
- Each defined value is subject to arity analysis ([§14.10](inference-procedures.md#arity-inference)).
- Values may have attributes, including the `ThreadStatic` or `ContextStatic` attribute.

### Literal Definitions in Modules

Value definitions in modules may have the `Literal` attribute. This attribute causes the value to be
compiled as a constant. For example:

```fsharp
[<Literal>]
let PI = 3.141592654
```

Literal values may be used in custom attributes and pattern matching. For example:

```fsharp
[<Literal>]
let StartOfWeek = System.DayOfWeek.Monday

[<MyAttribute(StartOfWeek)>]
let feeling(day) =
    match day with
    | StartOfWeek -> "rough"
    | _ -> "great"
```

A value that has the `Literal` attribute is subject to the following restrictions:

- It may not be marked `mutable` or `inline`.
- It may not also have the `ThreadStatic` or `ContextStatic` attributes.
- The right-hand side expression must be a _literal constant expression_ that is both a valid
    expression after checking, and is made up of either:
  - A simple constant expression, with the exception of `()`, native integer literals, unsigned native
       integer literals, byte array literals, BigInteger literals, and user-defined numeric literals.

— OR —

- A reference to another literal

— OR —

- A bitwise combination of literal constant expressions

— OR —

- A `+` concatenation of two literal constant expressions which are strings

— OR —

- `enum x` or `LanguagePrimitives.EnumOfValue x` where `x` is a literal constant expression.

### Type Function Definitions in Modules

Value definitions within modules may have explicit generic parameters. For example, `‘T` is a generic
parameter to the value `empty`:

```fsharp
let empty<'T> : (list<'T> * Set<'T>) = ([], Set.empty)
```

A value that has explicit generic parameters but has arity `[]` (that is, no explicit function parameters)
is called a _type function_. The following are some example type functions from the F# library:

```fsharp
val typeof<'T> : System.Type
val sizeof<'T> : int
module Set =
    val empty<'T> : Set<'T>
module Map =
    val empty<'Key,'Value> : Map<'Key,'Value>
```

Type functions are rarely used in F# programming, although they are convenient in certain
situations. Type functions are typically used for:

- Pure functions that compute type-specific information based on the supplied type arguments.
- Pure functions whose result is independent of inferred type arguments, such as empty sets and
    maps.

Type functions receive special treatment during generalization ([§14.6.7](inference-procedures.md#generalization)) and signature conformance
([§11.2](namespace-and-module-signatures.md#signature-conformance)). They typically have either the `RequiresExplicitTypeArguments` attribute or the
`GeneralizableValue` attribute. Type functions may not be defined inside types, expressions, or
computation expressions.

In general, type functions should be used only for computations that do not have observable side
effects. However, type functions may still perform computations. In this example, `r` is a type function
that calculates the number of times it has been called

```fsharp
let mutable count = 1
let r<'T> = (count <- count + 1); ref ([] : 'T list);;
// count = 1
let x1 = r<int>
// count = 2
let x2 = r<int>
// count = 3
let z0 = x1
// count = 3
```

The elaborated form of a type function is that of a function definition that takes one argument of
type `unit`. That is, the elaborated form of

```fsgrammar
let ident typar-defns = expr
```

is the same as the compiled form for the following declaration:

```fsgrammar
let ident typar-defns () = expr
```

References to type functions are elaborated to invocations of such a function.

### Active Pattern Definitions in Modules

A value definition within a module that has an `active-pattern-op-name` introduces pattern-matching
tags into the environment when the module is accessed or opened. For example,

```fsharp
let (|A|B|C|) x = if x < 0 then A elif x = 0 then B else C
```

introduces pattern tags `A`, `B`, and `C` into the _PatItems_ table in the name resolution environment.

### “do” statements in Modules

A `do` statement within a module has the following form:

```fsgrammar
do expr
```

The expression `expr` is checked with an arbitrary initial type `ty`. After checking `expr`, `ty` is asserted to
be equal to `unit`. If the assertion fails, a warning rather than an error is reported. This warning is
suppressed for plain expressions without do in script files (that is, .fsx and .fsscript files).

A `do` statement may have attributes. In this example, the `STAThread` attribute specifies that main
uses the single-threaded apartment (STA) threading model of COM:

```fsharp
let main() =
    let form = new System.Windows.Forms.Form()
    System.Windows.Forms.Application.Run(form)

[<STAThread>]
do main()
```

## Import Declarations

Namespace declaration groups and module definitions can include _import declarations_ in the
following form:

```fsgrammar
open long-ident
```

Import declarations make elements of other namespace declaration groups and modules accessible
by the use of unqualified names. For example:

```fsharp
open FSharp.Collections
open System
```

Import declarations can be used in:

- Module definitions and their signatures.
- Namespace declaration groups and their signatures.

An import declaration is processed by first resolving the `long-ident` to one or more namespace
declaration groups and/or modules [ `F1`, ..., `Fn` ] by _Name Resolution in Module and Namespace Paths_
([§14.1.2](inference-procedures.md#name-resolution-in-module-and-namespace-paths)). For example, `System.Collections.Generic` may resolve to one or more namespace
declaration groups—one for each assembly that contributes a namespace declaration group in the
current environment. Next, each `Fi` is added to the environment successively by using the technique
specified in [§14.1.3](inference-procedures.md#opening-modules-and-namespace-declaration-groups). An error occurs if any `Fi` is a module that has the `RequireQualifiedAccess`
attribute.

## Module Abbreviations

A module abbreviation defines a local name for a module long identifier, as follows:

```fsgrammar
module ident = long-ident
```

For example:

```fsharp
module Ops = FSharp.Core.Operators
```

Module abbreviations can be used in:

- Module definitions and their signatures.
- Namespace declaration groups and their signatures.

Module abbreviations are implicitly private to the module or namespace declaration group in which
they appear.

A module abbreviation is processed by first resolving the `long-ident` to a list of modules by _Name
Resolution in Module and Namespace Paths_ (see [§14.1](inference-procedures.md#name-resolution)). The list is then appended to the set of
names that are associated with `ident` in the _ModulesAndNamespaces_ table.

Module abbreviations may not be used to abbreviate namespaces.

## Accessibility Annotations

Accessibilities may be specified on declaration elements in namespace declaration groups and
modules, and on members in types. The table lists the accessibilities that can appear in user code:

| Accessibility | Description |
| --- | --- |
| `public` | No restrictions on access. |
| `private` | Access is permitted only from the enclosing type, module, or namespace declaration group. |
| `internal` | Access is permitted only from within the enclosing assembly, or from assemblies whose name is listed using the `InternalsVisibleTo` attribute in the current assembly. |

The default accessibilities are `public`. Specifically:

- Function definitions, value definitions, type definitions, and exception definitions in modules are
    public.
- Modules, type definitions, and exception definitions in namespaces are public.
- Members in type definitions are public.

Some function and value definitions may not be given an accessibility and, by their nature, have
restricted lexical scope. In particular:

- Function and value definitions in classes are lexically available only within the class being
    defined, and only from the point of their definition onward.
- Module type abbreviations are lexically available only within the module or namespace
    declaration group being defined, and only from their point of their definition onward.

Note that:

- `private` on a member means “private to the enclosing type or module.”
- `private` on a function or value definition in a module means “private to the module or
    namespace declaration group.”
- `private` on a type, module, or type representation in a module means “private to the module.”

The CLI compiled form of all non-public entities is `internal`.

> Note: The `family` and `protected` specifications are not supported in this version of the F#
language.

Accessibility modifiers can appear only in the locations summarized in the following table.

| Component | Location | Example |
| --- | --- | --- |
| Function or value <br>definition in module |Precedes identifier |  `let private x = 1`<br>`let inline private f x = 1`<br>`let mutable private x = 1` |
| Module definition | Precedes identifier | `module private M =`<br>`let x = 1` |
| Type definition | Precedes identifier | `type private C = A \| B`<br>`type private C<'T> = A \| B`
| `val` definition in a class | Precedes identifier | `val private x : int` |
| Explicit constructor | Precedes identifier | `private new () = { inherit Base }`
| Implicit constructor | Precedes identifier | `type C private() = ...`
| Member definition | Precedes identifier, but cannot appear on <br>`inherit` definitions, <br>`interface` definitions, <br>`abstract` definitions, <br>individual union cases. <br>Accessibility for <br>`inherit`, `interface`, <br>and `abstract` definitions is <br>always the same as that of <br>the enclosing class. | `member private x.X = 1` |
| Explicit property get <br>or set in a class | Precedes identifier | `member __.Item`<br> `with private get i = 1` <br>`and private set i v = ()` |
| Type representation | Precedes identifier | `type Cases = private \| A \| B` |
