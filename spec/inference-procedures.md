# Inference Procedures

## Name Resolution

The following sections describe how F# resolves names in various contexts.

### Name Environments

Each point in the interpretation of an F# program is subject to an environment. The environment
encompasses:

- All referenced external DLLs (assemblies).
- _ModulesAndNamespaces_ : a table that maps `long-ident`s to a list of signatures. Each signature is
    either a namespace declaration group signature or a module signature.

    For example, `System.Collections` may map to one namespace declaration group signature for
    each referenced assembly that contributes to the `System.Collections` namespace, and to a
    module signature, if a module called `System.Collections` is declared or in a referenced
    assembly.

    If the program references multiple assemblies, the assemblies are added to the name resolution
environment in the order in which the references appear on the command line. The order is
important only if ambiguities occur in referencing the contents of assemblies—for example, if
two assemblies define the type `MyNamespace.C`.

- _ExprItems_ : a table that maps names to the following items:
    - A value
    - A union case for use when constructing data
    - An active pattern result tag for use when returning results from active patterns
    - A type name for each class or struct type
- _FieldLabels_ : a table that maps names to sets of field references for record types
- _PatItems_ : a table that maps names to the following items:
    - A union case, for use when pattern matching on data
    - An active pattern case name, for use when specifying active patterns
    - A literal definition
- _Types_ : a table that maps names to type definitions. Two queries are supported on this table:
    - Find a type by name alone. This query may return multiple types. For example, in the default
       type-checking environment, the resolution of `System.Tuple` returns multiple tuple types.


    - Find a type by name and generic arity `n`. This query returns at most one type. For example, in
    the default type-checking environment, the resolution of `System.Tuple` with `n = 2` returns a
    single type.
- _ExtensionsInScope_ : a table that maps type names to one or more member definitions

The dot notation is resolved during type checking by consulting these tables.

### Name Resolution in Module and Namespace Paths

Given an input `long-ident` and environment `env`, _Name Resolution in Module and Namespace Paths_
computes the result of interpreting `long-ident` as a module or namespace. The procedure returns a
list of modules and namespace declaration groups.

_Name Resolution in Module and Namespace Paths_ proceeds through the following steps:

1. Consult the _ModulesAndNamespaces_ table to resolve the `long-ident` prefix to a list of modules
    and namespace declaration group signatures.
2. If any identifiers remain unresolved, recursively consult the declared modules and sub-modules
    of these namespace declaration groups.
3. Concatenate all the results.

If the `long-ident` starts with the special pseudo-identifier keyword `global`, the identifier is resolved
by consulting the _ModulesAndNamespaces_ table and ignoring all `open` directives, including those
implied by `AutoOpen` attributes.

For example, if the environment contains two referenced DLLs, and each DLL has namespace
declaration groups for the namespaces `System`, `System.Collections`, and
`System.Collections.Generic`, _Name Resolution in Module and Namespace Paths_ for
`System.Collections` returns the two namespace declaration groups named `System.Collections`, one
from each assembly.

### Opening Modules and Namespace Declaration Groups

When a module or namespace declaration group `F` is opened, the compiler adds items to the name
environment as follows:

1. Add each exception label for each exception type definition ([§8.11](type-definitions.md#exception-definitions)) in `F` to the _ExprItems_ and
    _PatItems_ tables in the original order of declaration in `F`.
2. Add each type definition in the original order of declaration in `F`. Adding a type definition
    involves the following procedure:
    - If the type is a class or struct type (or an abbreviation of such a type), add the type name to
    the _ExprItems_ table.
    - If the type definition is a record, add the record field labels to the _FieldLabels_ table, unless
    the type has the `RequireQualifiedAccess` attribute.
    - If the type is a union, add the union cases to the _ExprItems_ and _PatItems_ tables, unless the
    type has the `RequireQualifiedAccess` attribute.
    - Add the type to the _TypeNames_ table. If the type has a CLI-encoded generic name such as
    ``List`1``, add an entry under both `List` and ``List`1``.

3. Add each value in the original order of declaration in `F` , as follows:

    - Add the value to the _ExprItems_ table.
    - If any value is an active pattern, add the tags of that active pattern to the _PatItems_ table
    according to the original order of declaration.
    - If the value is a literal, add it to the _PatItems_ table.
4. Add the member contents of each type extension in `Fi` to the _ExtensionsInScope_ table according
    to the original order of declaration in `Fi`.
5. Add each sub-module or sub-namespace declaration group in `Fi` to the _ModulesAndNamespaces_
    table according to the original order of declaration in `Fi`.
6. Open any sub-modules that are marked with the `FSharp.Core.AutoOpen` attribute.

### Name Resolution in Expressions

Given an input `long-ident` , environment `env` , and an optional count `n` of the number of subsequent
type arguments `<_, ..., _>`, _Name Resolution in Expressions_ computes a result that contains the
interpretation of the `long-ident` `<_, ..., _>` prefix as a value or other expression item, and a residue
path `rest`.

How Name Resolution in Expressions proceeds depends on whether `long-ident` is a single identifier
or is composed of more than one identifier.

If `long-ident` is a single identifier `ident`:

1. Look up `ident` in the _ExprItems_ table. Return the result and empty `rest`.
2. If `ident` does not appear in the _ExprItems_ table, look it up in the _Types_ table, with generic arity
    that matches `n` if available. Return this type and empty `rest`.
3. If `ident` does not appear in either the _ExprItems_ table or the _Types_ table, fail.

If `long-ident` is composed of more than one identifier `ident.rest`, _Name Resolution in Expressions_
proceeds as follows:

1. If `ident` exists as a value in the _ExprItems_ table, return the result, with `rest` as the residue.
2. If `ident` does not exist as a value in the _ExprItems_ table, perform a backtracking search as
    follows:

    - Consider each division of `long-ident` into `[namespace-or-module-path].ident[.rest]`, in
    which the `namespace-or-module-path` becomes successively longer.
    - For each such division, consider each module signature or namespace declaration group
    signature `F` in the list that is produced by resolving `namespace-or-module-path` by using Name
    _Resolution in Module and Namespace Paths_.
    - For each such `F` , attempt to resolve `ident[.rest]` in the following order. If any resolution
    succeeds, then terminate the search:
        - A value in `F`. Return this item and `rest`.
        - A union case in `F`. Return this item and `rest`.
        - An exception constructor in `F`. Return this item and `rest`.
        - A type in `F`. If `rest` is empty, then return this type; if not, resolve using _Name
        Resolution for Members_.
        - A [sub-]module in `F`. Recursively resolve `rest` against the contents of this module.
3. If steps 1 and 2 do not resolve `long-ident`, look up `ident` in the _Types_ table.

    - If the generic arity `n` is available, then look for a type that matches both `ident` and `n`.
    - If no generic arity `n` is available, and `rest` is not empty:
        - If the _Types_ table contains a type `ident` that does not have generic arguments,
        resolve to this type.
        - If the _Types_ table contains a unique type `ident` that has generic arguments, resolve
        to this type. However, if the overall result of the _Name Resolution in Expressions_
        operation is a member, and the generic arguments do not appear in either the
        return or argument types of the item, warn that the generic arguments cannot be
        inferred from the type of the item.
        - If neither of the preceding steps resolves the type, give an error.
    - If rest is empty, return the type, otherwise resolve using _Name Resolution for Members_.
4. If steps 1-3 do not resolve `long-ident`, look up `ident` in the _ExprItems_ table and return the result
    and residue `rest`.
5. Otherwise, if `ident` is a symbolic operator name, resolve to an item that indicates an implicitly
    resolved symbolic operator.
6. Otherwise, fail.

If the expression contains ambiguities, _Name Resolution in Expressions_ returns the first result that
the process generates. For example, consider the following cases:

```fsharp
module M =
    type C =
        | C of string
        | D of string
        member x.Prop1 = 3
    type Data =
        | C of string
        | E
        member x.Prop1 = 3
        member x.Prop2 = 3
    let C = 5
    open M
    let C = 4
    let D = 6

    let test1 = C               // resolves to the value C
    let test2 = C.ToString()    // resolves to the value C with residue ToString
    let test3 = M.C             // resolves to the value M.C
    let test4 = M.Data.C        // resolves to the union case M.Data.C
    let test5 = M.C.C           // error: first part resolves to the value M.C,
                                // and this contains no field or property "C"
    let test6 = C.Prop1         // error: the value C does not have a property Prop
    let test7 = M.E.Prop2       // resolves to M.E, and then a property lookup
```
The following example shows the resolution behavior for type lookups that are ambiguous by
generic arity:

```fsharp
module M =
    type C<'T>() =
        static member P = 1

    type C<'T,'U>() =
        static member P = 1

    let _ = new M.C() // gives an error
    let _ = new M.C<int>() // no error, resolves to C<'T>
    let _ = M.C() // gives an error
    let _ = M.C<int>() // no error, resolves to C<'T>
    let _ = M.C<int,int>() // no error, resolves to C<'T,'U>
    let _ = M.C<_>() // no error, resolves to C<'T>
    let _ = M.C<_,_>() // no error, resolves to C<'T,'U>
    let _ = M.C.P // gives an error
    let _ = M.C<_>.P // no error, resolves to C<'T>
    let _ = M.C<_,_>.P // no error, resolves to C<'T,'U>
```

The following example shows how the resolution behavior differs slightly if one of the types has no generic arguments.

```fsharp
module M =
    type C() =
        static member P = 1

    type C<'T>() =
        static member P = 1

    let _ = new M.C()       // no error, resolves to C
    let _ = new M.C<int>()  // no error, resolves to C<'T>
    let _ = M.C()           // no error, resolves to C
    let _ = M.C< >()        // no error, resolves to C
    let _ = M.C<int>()      // no error, resolves to C<'T>
    let _ = M.C< >()        // no error, resolves to C
    let _ = M.C<_>()        // no error, resolves to C<'T>
    let _ = M.C.P           // no error, resolves to C
let _ = M.C< >.P            // no error, resolves to C
    let _ = M.C<_>.P        // no error, resolves to C<'T>
```
In the following example, the procedure issues a warning for an incomplete type. In this case, the
type parameter `'T` cannot be inferred from the use `M.C.P`, because `'T` does not appear at all in the
type of the resolved element `M.C<'T>.P`.

```fsharp
module M =
    type C<'T>() =
        static member P = 1

    let _ = M.C.P // no error, resolves to C<'T>.P, warning given
```
The effect of these rules is to prefer value names over module names for single identifiers. For
example, consider this case:

```fsharp
let Foo = 1

module Foo =
    let ABC = 2
let x1 = Foo // evaluates to 1
```
The rules, however, prefer type names over value names for single identifiers, because type names
appear in the _ExprItems_ table. For example, consider this case:

```fsharp
let Foo = 1
type Foo() =
    static member ABC = 2
let x1 = Foo.ABC // evaluates to 2
let x2 = Foo() // evaluates to a new Foo()
```
### Name Resolution for Members

_Name Resolution for Members_ is a sub-procedure used to resolve `.member-ident[.rest]` to a
member, in the context of a particular type `type`.

_Name Resolution for Members_ proceeds through the following steps:

1. Search the hierarchy of the type from `System.Object` to `type`.

2. At each type, try to resolve `member-ident` to one of the following, in order:

    - A union case of `type`.
    - A property group of `type`.
    - A method group of `type`.
    - A field of `type`.
    - An event of `type`.
    - A property group of extension members of `type`, by consulting the _ExtensionsInScope_ table.
    - A method group of extension members of `type`, by consulting the _ExtensionsInScope_ table.
    - A nested type `type-nested` of `type`. Recursively resolve `.rest` if it is present, otherwise return
    `type-nested`.

3. At any type, the existence of a property, event, field, or union case named `member-ident` causes
    any methods or other entities of that same name from base types to be hidden.
4. Combine method groups with method groups from base types. For example:

```fsharp
type A() =
    member this.Foo(i : int) = 0

type B() =
    inherit A()
    member this.Foo(s : string) = 1

let b = new B()
b.Foo(1)        // resolves to method in A
b.Foo("abc")    // resolves to method in B
```
### Name Resolution in Patterns

_Name Resolution for Patterns_ is used to resolve `long-ident` in the context of pattern expressions.
The `long-ident` must resolve to a union case, exception label, literal value, or active pattern case
name. If it does not, the `long-ident` may represent a new variable definition in the pattern.

_Name Resolution for Patterns_ follows the same steps to resolve the `member-ident` as _Name
Resolution in Expressions_ ([§14.1.4](inference-procedures.md#name-resolution-in-expressions)) except that it consults the _PatItems_ table instead of the _ExprItems_
table. As a result, values are not present in the namespace that is used to resolve identifiers in
patterns. For example:

```fsharp
let C = 3
match 4 with
| C -> sprintf "matched, C = %d" C
| _ -> sprintf "no match, C = %d" C
```
results in `"matched, C = 4"`, because `C` is _not_ present in the _PatItems_ table, and hence becomes a
value pattern. In contrast,


```fsharp
[<Literal>]
let C = 3

match 4 with
| C -> sprintf "matched, C = %d" C
| _ -> sprintf "no match, C = %d" C
```
results in `"no match, C = 3"`, because `C` is a literal and therefore _is_ present in the _PatItems_ table.

### Name Resolution for Types

_Name Resolution for Types_ is used to resolve `long-ident` in the context of a syntactic type. A generic
arity that matches `n` is always available. The result is a type definition and a possible residue `rest`.

_Name Resolution for Types_ proceeds through the following steps:

1. Given `ident[.rest]`, look up `ident` in the _Types_ table, with generic arity `n`. Return the result and
    residue `rest`.
2. If `ident` is not present in the _Types_ table:

    - Divide `long-ident` into `[namespace-or-module-path].ident[.rest]`, in which the `namespace-
    or-module-path` becomes successively longer.
    - For each such division, consider each module and namespace declaration group `F` in the list
    that results from resolving `namespace-or-module-path` by using _Name Resolution in Module
    and Namespace Paths_ ([§14.1.2](inference-procedures.md#name-resolution-in-module-and-namespace-paths)).
    - For each such `F` , attempt to resolve `ident[.rest]` in the following order. Terminate the
    search when the expression is successfully resolved.
        1) A type in `F`. Return this type and residue `rest`.
        2) A [sub-]module in `F`. Recursively resolve `rest` against the contents of this module.

In the following example, the name `C` on the last line resolves to the named type `M.C<_,_>` because `C`
is applied to two type arguments:

```fsharp
module M =
    type C<'T, 'U> = 'T * 'T * 'U

module N =
    type C<'T> = 'T * 'T

open M
open N

let x : C<int, string> = (1, 1, "abc")
```

### Name Resolution for Type Variables

Whenever the F# compiler processes syntactic types and expressions, it assumes a context that
maps identifiers to inference type variables. This mapping ensures that multiple uses of the same
type variable name map to the same type inference variable. For example, consider the following
function:

```fsharp
let f x y = (x:'T), (y:'T)
```
In this case, the compiler assigns the identifiers `x` and `y` the same static type - that is, the same type
inference variable is associated with the name `'T`. The full inferred type of the function is:

```fsharp
val f<'T> : 'T -> 'T -> 'T * 'T
```
The map is used throughout the processing of expressions and types in a left-to-right order. It is
initially empty for any member or any other top-level construct that contains expressions and types.
Entries are eliminated from the map after they are generalized. As a result, the following code
checks correctly:

```fsharp
let f () =
    let g1 (x:'T) = x
    let g2 (y:'T) = (y:string)
    g1 3, g1 "3", g2 "4"
```
The compiler generalizes `g1`, which is applied to both integer and string types. The type variable `'T` in
`(y:'T)` on the third line refers to a different type inference variable, which is eventually constrained to
be type `string`.

### Field Label Resolution

_Field Label Resolution_ specifies how to resolve identifiers such as `field1` in `{field1 = expr; ... fieldN = expr}`.

_Field Label Resolution_ proceeds through the following steps:

1. Look up all fields in all available types in the _Types_ table and the _FieldLabels_ table ([§8.4](type-definitions.md#record-type-definitions)).
2. Return the set of field declarations.

## Resolving Application Expressions

Application expressions that use dot notation - such as `x.Y<int>.Z(g).H.I.j` - are resolved
according to a set of rules that take into account the many possible shapes and forms of these
expressions and the ambiguities that may occur during their resolution. This section specifies the
exact algorithmic process that is used to resolve these expressions.

Resolution of application expressions proceeds as follows:

1. Repeatedly decompose the application expression into a leading expression `expr` and a list of
    projections `projs`. Each projection has the following form:
    - `.long-ident-or-op` is a dot lookup projection.
    - `expr` is an application projection.
    - `<types>` is a type application projection.

   For example:
    - `x.y.Z(g).H.I.j` decomposes into `x.y.Z` and projections `(g)`, `.H.I.j`.
    - `x.M<int>(g)` decomposes into `x.M` and projections `<int>`, `(g)`.
    - `f x` decomposes into `f` and projection `x`.

    > Note: In this specification we write sequences of projections by juxtaposition; for
    example, `(expr).long-ident<types>(expr)`. We also write ( `.rest` + `projs` ) to refer to
    adding a residue long identifier to the front of a list of projections, which results in `projs`
    if `rest` is empty and `.rest projs` otherwise.

2. After decomposition:
    - If `expr` is a long identifier expression `long-ident`, apply _Unqualified Lookup_ ([§14.2.1](inference-procedures.md#unqualified-lookup)) on `long-ident` with projections `projs`.
    - If `expr` is not such an expression, check the expression against an arbitrary initial type `ty`, to
       generate an elaborated expression `expr`. Then process `expr`, `ty`, and `projs` by using
       _Expression-Qualified Lookup_ ([§14.2.3](inference-procedures.md#expression-qualified-lookup))

### Unqualified Lookup

Given an input `long-ident` and projections `projs`, _Unqualified Lookup_ computes the result of
“looking up” `long-ident.projs` in an environment `env`. The first part of this process resolves a prefix
of the information in `long-ident.projs`, and recursive resolutions typically use _Expression-Qualified
Resolution_ to resolve the remainder.

For example, _Unqualified Lookup_ is used to resolve the vast majority of identifier references in F#
code, from simple identifiers such as `sin`, to complex accesses such as
`System.Environment.GetCommandLineArgs().Length`.

_Unqualified Lookup_ proceeds through the following steps:

1. Resolve `long-ident` by using _Name Resolution in Expressions_ ([§14.1](inference-procedures.md#name-resolution)). This returns a _name resolution item_ `item` and a _residue long identifier_ `rest`.

For example, the result of _Name Resolution in Expressions_ for `v.X.Y` may be a value reference `v`
along with a residue long identifier `X.Y`. Likewise, `N.X(args).Y` may resolve to an overloaded
method `N.X` and a residue long identifier `Y`.

_Name Resolution in Expressions_ also takes as input the presence and count of subsequent type
arguments in the first projection. If the first projection in `projs` is `<tyargs>`, _Unqualified Lookup_
invokes _Name Resolution in Expressions_ with a known number of type arguments. Otherwise, it
is invoked with an unknown number of type arguments.

2. Apply _Item-Qualified Lookup_ for `item` and (`rest` + `projs`).


### Item-Qualified Lookup

Given an input item `item` and projections `projs`, _Item-Qualified Lookup_ computes the projection
`item.projs`. This computation is often a recursive process: the first resolution uses a prefix of the
information in `item.projs`, and recursive resolutions resolve any remaining projections.

_Item-Qualified Lookup_ proceeds as follows:

1. If `item` is not one of the following, return an error:
    - A named value
    - A union case
    - A group of named types
    - A group of methods
    - A group of indexer getter properties
    - A single non-indexer getter property
    - A static F# field
    - A static CLI field
    - An implicitly resolved symbolic operator name
2. If the first projection is `<types>`, then we say the resolution has a type application `<types>` with
    remaining projections.
3. Otherwise, checking proceeds as follows.
    - If `item` is a value reference `v` then first
    Instantiate the type scheme of `v`, which results in a type `ty`. Apply these rules:

        - If the first projection is `<types>`, process the types and use the results as the
        arguments to instantiate the type scheme.
        - If the first projection is not `<types>`, the type scheme is _freshly instantiated_.
        - If the value has the `RequiresExplicitTypeArguments` attribute, the first
        projection must be `<types>`.
        - If the value has type `byref<ty2>`, add a byref dereference to the elaborated
        expression.
        - Insert implicit flexibility for the use of the value ([§14.4.3](inference-procedures.md#implicit-insertion-of-flexibility-for-uses-of-functions-and-members)).
        
        Then Apply _Expression-Qualified Lookup_ for type `ty` and any remaining projections.

    - If `item` is a type name, where `projs` begins with `<types>.long-ident`

        - Process the types and use the results as the arguments to instantiate the named
        type reference, thus generating a type `ty`.
        - Apply _Name Resolution for Members_ to `ty` and `long-ident`, which generates a new
        `item`.
        - Apply _Item-Qualified Lookup_ to the new `item` and any remaining projections.

    - If `item` is a group of type names where `projs` begins with `<types>` or `expr` or `projs` is empty
        
        - Process the types and use the results as the arguments to instantiate the named
        type reference, thus generating a type `ty`.
        - Process the object construction `ty(expr)` as an object constructor call in the same
        way as `new ty(expr)`. If `projs` is empty then process the object construction `ty` as
        an object constructor call in the same way as `(fun arg -> new ty(arg))`, i.e.
        resolve the object constructor call with no arguments.
        - Apply _Expression-Qualified Lookup_ to `item` and any remaining projections.

    - If `item` is a group of method references

        - Apply _Method Application Resolution_ for the method group. _Method Application Resolution_ 
          accepts an optional set of type arguments and a syntactic expression
            argument. Determine the arguments based on what `projs` begins with:
            - `<types> expr`, then use `<types>` as the type arguments and `expr` as the
               expression argument.
            - `expr`, then use `expr` as the expression argument.
            - anything else, use no expression argument or type arguments.
        - If the result of _Method Application Resolution_ is labeled with the
            `RequiresExplicitTypeArguments` attribute, then explicit type arguments are
            required.
        - Let `fty` be the actual return type that results from _Method Application Resolution_.
            Apply _Expression-Qualified Lookup_ to `fty` and any remaining projections.

    - If `item` is a group of property indexer references

        - Apply _Method Application Resolution_, and use the underlying getter indexer
        methods for the method group.
        2. Determine the arguments to _Method Application Resolution_ as described for a
        group of methods.

    - If `item` is a static field reference
        - Check the field for accessibility and attributes.
        - Let `fty` be the actual type of the field, taking into account the type `ty` via which the
          field was accessed in the case where this is a field in a generic type.
        - Apply _Expression-Qualified Lookup_ to `fty` and `projs`.

    - If `item` is a union case tag, exception tag, or active pattern result element tag
        - Check the tag for accessibility and attributes.
        - If `projs` begins with `expr`, use `expr` as the expression argument.
        - Otherwise, use no expression argument or type arguments. In this case, build a
          function expression for the union case.
        - Let `fty` be the actual type of the union case.
        - Apply _Expression-Qualified Lookup_ to `fty` and remaining `projs`.

    - If `item` is a CLI event reference

        - Check the event for accessibility and attributes.
        - Let `fty` be the actual type of the event.
        - Apply _Expression-Qualified Lookup_ to `fty` and `projs`.

    - If `item` is an implicitly resolved symbolic operator name `op`
        
        - If `op` is a unary, binary or the ternary operator ?<-, resolve to the following
        expressions, respectively:

            ```fsharp
            (fun (x:^a) -> (^a : static member (op) : ^a -> ^b) x)
            (fun (x:^a) (y:^b) ->
                ((^a or ^b) : static member (op) : ^a * ^b -> ^c) (x,y))
            (fun (x:^a) (y:^b) (z:^c)
                -> ((^a or ^b or ^c) : static member (op) : ^a * ^b * ^c -> ^d) (x,y,z))
            ```
        - The resulting expressions are static member constraint invocation expressions ([§6.4.8](expressions.md#member-constraint-invocation-expressions)),
        which enable the default interpretation of operators by using type-directed
        member resolution.
        - Recheck the entire expression with additional subsequent projections `.projs`.

### Expression-Qualified Lookup

Given an elaborated expression `expr` of type `ty`, and projections `projs`, _Expression-Qualified Lookup_
computes the “lookups or applications” for `expr.projs`.

Expression-Qualified Lookup proceeds through the following steps:

1. Inspect `projs` and process according to the following table.

    | `projs` | Action | Comments |
    | --- | --- | --- |
    | Empty | Assert that the type of the overall, original application expression is `ty`. | Checking is complete.|
    | Starts with `(expr2)` | Apply _Function Application Resolution_ ([§14.3](inference-procedures.md#function-application-resolution)). | Checking is complete when _Function Application Resolution_ returns. |
    | Starts with `<types>` | Fail. | Type instantiations may not be applied to arbitrary expressions; they can apply only to generic types, generic methods, and generic values. |
    | Starts with `.long-ident` | Resolve `long-ident` using _Name Resolution for Members_ ([§14.1.4](inference-procedures.md#name-resolution-in-expressions))_. Return a name resolution item `item` and a residue long identifier `rest`. Continue processing at step 2. | For example, for `ty = string` and `long-ident = Length`, _Name Resolution for Members_ returns a property reference to the CLI instance property `System.String.Length`. |

2. If Step 1 returned an `item` and `rest`, report an error if `item` is not one of the following:
    - A group of methods.
    - A group of instance getter property indexers.
    - A single instance, non-indexer getter property.
    - A single instance F# field.
    - A single instance CLI field.
3. Proceed based on `item` as follows:

    - If `item` is a group of methods
        - Apply _Method Application Resolution_ for the method group. _Method Application Resolution_ accepts an optional set of type arguments and a syntactic expression argument. If `projs` begins with:
            - `<types>(arg)`, then use `<types>` as the type arguments and `arg` as
            the expression argument.
            - `(arg)`, then use `arg` as the expression argument.
            - otherwise, use no expression argument or type arguments.

        - Let `fty` be the actual return type resulting from _Method Application Resolution_. Apply _Expression-Qualified Lookup_ to `fty` and any remaining projections.

    - If `item` is a group of indexer properties
        - Apply _Method Application Resolution_ and use the underlying getter indexer methods for the method group.
        - Determine the arguments to _Method Application Resolution_ as described for a group of methods.

    - If `item` is a non-indexer getter property
        - Apply _Method Application Resolution_ for the method group that contains
            only the getter method for the property, with no type arguments and one `()` argument.

    - If `item` is an instance intermediate language (IL) or F# field `F`

        - Check the field for accessibility and attributes.
        - Let `fty` be the actual type of the field (taking into account the type `ty`
          by which the field was accessed).
        - Assert that `ty` is a subtype of the actual containing type of the field.
        - Produce an elaborated form for `expr.F`. If `F` is a field in a value type
          then take the address of `expr` by using the _AddressOf_(`expr, NeverMutates`) operation [§6.9.4](expressions.md#taking-the-address-of-an-elaborated-expression).
        - Apply _Expression-Qualified Lookup_ to `fty` and `projs`.

## Function Application Resolution

Given expressions `f` and `expr` where `f` has type `ty`, and given subsequent projections `projs`, _Function
Application Resolution_ does the following:

1. Asserts that `f` has type `ty1 -> ty2` for new inference variables `ty1` and `ty2`.
2. If the assertion succeeds:

    - Check `expr` with the initial type `ty1`.
    - Process `projs` using _Expression-Qualified Lookup_ against `ty2`.
3. If the assertion fails, and `expr` has the form `{ computation-expr }`:

    - Check the expression as the computation expression form `f { computation-expr }`, giving result type `ty1`.
    - Process `projs` using Expression-Qualified Lookup against `ty1`.

## Method Application Resolution

Given a method group `M`, optional type arguments `<ActualTypeArgs>`, an optional syntactic argument
`obj`, an optional syntactic argument `arg`, and overall initial type `ty`, _Method Application Resolution_
resolves the overloading based on the partial type information that is available. It also:

- Resolves optional and named arguments.
- Resolves “out” arguments.
- Resolves post-hoc property assignments.
- Applies method application resolution.
- Inserts _ad hoc_ conversions that are only applied for method calls.

If no syntactic argument is supplied, _Method Application Resolution_ tries to resolve the use of the
method as a first class value, such as the method call in the following example:

```fsharp
List.map System.Environment.GetEnvironmentVariable ["PATH"; "USERNAME"]
```
_Method Application Resolution_ proceeds through the following steps:

1. Restrict the candidate method group `M` to those methods that are _accessible_ from the point of
    resolution.
2. If an argument `arg` is present, determine the sets of _unnamed_ and _named actual arguments_,
    `UnnamedActualArgs` and `NamedActualArgs`:

    - Decompose `arg` into a list of arguments:
        - If `arg` is a syntactic tuple `arg1, ..., argN`, use these arguments.
        - If `arg` is a syntactic unit value `()`, use a zero-length list of arguments.

    - For each argument:
        - If `arg` is a binary expression of the form `name=expr`, it is a named actual argument.
        - Otherwise, `arg` is an unnamed actual argument.

    If there are no named actual arguments, and `M` has only one candidate method, which accepts
    only one required argument, ignore the decomposition of `arg` to tuple form. Instead, `arg` itself is
    the only named actual argument.

    All named arguments must appear after all unnamed arguments.

    Examples:
    - `x.M(1, 2)` has two unnamed actual arguments.
    - `x.M(1, y = 2)` has one unnamed actual argument and one named actual argument.
    - `x.M(1, (y = 2))` has two unnamed actual arguments.
    - `x.M( printfn "hello"; ())` has one unnamed actual argument.
    - `x.M((a, b))` has one unnamed actual argument.
    - `x.M(())` has one unnamed actual argument.

3. Determine the named and unnamed _prospective actual argument types_, called `ActualArgTypes`.
    - If an argument `arg` is present, the prospective actual argument types are fresh type
       inference variables for each unnamed and named actual argument.
       - If the argument has the syntactic form of an address-of expression `&expr` after ignoring
          parentheses around the argument, equate this type with a type `byref<ty>` for a fresh
          type `ty`.
       - If the argument has the syntactic form of a function expression `fun pat1 ... patn -> expr`
          after ignoring parentheses around the argument, equate this type with a 
          type `ty1 -> ... tyn -> rty` for fresh types `ty1 ... tyn`.
    - If no argument `arg` is present:

        - If the method group contains a single method, the prospective unnamed argument
        types are one fresh type inference variable for each required, non-“out” parameter that
        the method accepts.
        - If the method group contains more than one method, the expected overall type of the
        expression is asserted to be a function type `dty -> rty`.
            - If `dty` is a tuple type `(dty1 * ... * dtyN)`, the prospective argument types are `(dty1,
            ..., dtyN)`.

            - If `dty` is `unit`, then the prospective argument types are an empty list.

            - If `dty` is any other type, the prospective argument types are `dty` alone.

        - Subsequently:
            - The method application is considered to have one unnamed actual argument for
            each prospective unnamed actual argument type.

            -    The method application is considered to have no named actual arguments.

4. For each candidate method in `M`, attempt to produce zero, one, or two _prospective method calls_
    `M~possible` as follows:

    - If the candidate method is generic and has been generalized, generate fresh type inference
    variables for its generic parameters. This results in the `FormalTypeArgs` for `M~possible`.
    - Determine the named and unnamed formal parameters , called `NamedFormalArgs` and
    `UnnamedFormalArgs` respectively, by splitting the formal parameters for `M` into parameters
    that have a matching argument in `NamedActualArgs` and parameters that do not.
    - If the number of `UnnamedFormalArgs` exceeds the number of `UnnamedActualArgs`, then modify
    `UnnamedFormalArgs` as follows:

        - Determine the suffix of `UnnamedFormalArgs` beyond the number of `UnnamedActualArgs`.
        - If all formal parameters in the suffix are `out` arguments with `byref` type, remove the
            suffix from `UnnamedFormalArgs` and call it `ImplicitlyReturnedFormalArgs`.
        - If all formal parameters in the suffix are optional arguments, remove the suffix from
            `UnnamedFormalArgs` and call it `ImplicitlySuppliedFormalArgs`.

    - If the last element of `UnnamedFormalArgs` has the `ParamArray` attribute and type `pty[]` for
    some `pty`, then modify `UnnamedActualArgs` as follows:

        - If the number of `UnnamedActualArgs` exceeds the number of `UnnamedFormalArgs - 1`,
            produce a prospective method call named `ParamArrayActualArgs` that has the excess of
            `UnnamedActualArgs` removed.
        - If the number of `UnnamedActualArgs` equals the number of `UnnamedFormalArgs - 1`, produce
            two prospective method calls:

            - One has an empty `ParamArrayActualArgs`.
            - One has no `ParamArrayActualArgs`.

        - If `ParamArrayActualArgs` has been produced, then `M~possible` is said to use _ParamArray conversion_ with type `pty`.

    - Associate each `name = arg` in `NamedActualArgs` with a target. A target is a _named formal
      parameter_, a _settable return property_, or a _settable return field_ as follows:
    
        - If one of the arguments in `NamedFormalArgs` has name `name`, that argument is the target.
        - If the return type of `M`, before the application of any type arguments `ActualTypeArgs`,
            contains a settable property `name`, then `name` is the target. The available properties
            include any property extension members of type, found by consulting the
            _ExtensionsInScope_ table.
        - If the return type of `M`, before the application of any type arguments `ActualTypeArgs`,
            contains a settable field `name`, then `name` is the target.

    - No prospective method call is generated if any of the following are true:

        - A named argument cannot be associated with a target.
        - The number of `UnnamedActualArgs` is less than the number of `UnnamedFormalArgs` after
            steps 4 a-e.
        - The number of `ActualTypeArgs`, if any actual type arguments are present, does not
            precisely equal the number of `FormalTypeArgs` for `M`.
        - The candidate method is static and the optional syntactic argument `obj` is present, or
            the candidate method is an instance method and `obj` is not present.

5. Attempt to apply initial types before argument checking. If only one prospective method call
`M~possible` exists, _assert_ `M~possible` by performing the following steps:

    - Verify that each `ActualTypeArgi` is equal to its corresponding `FormalTypeArgi`.
    - Verify that the type of `obj` is a subtype of the containing type of the method `M`.
    - For each `UnnamedActualArgi` and `UnnamedFormalArgi`, verify that the corresponding
    `ActualArgType` coerces to the type of the corresponding argument of `M`.
    - If `M~possible` uses ParamArray conversion with type `pty`, then for each `ParamArrayActualArgi`,
    verify that the corresponding `ActualArgType` coerces to `pty`.
    - For each `NamedActualArgi` that has an associated formal parameter target, verify that the
    corresponding `ActualArgType` coerces to the type of the corresponding argument of `M`.
    - For each `NamedActualArgi` that has an associated property or field setter target, verify that
    the corresponding `ActualArgType` coerces to the type of the property or field.
    - Verify that the prospective formal return type coerces to the expected actual return type. If
    the method `M` has return type `rty`, the formal return type is defined as follows:

        - If the prospective method call contains `ImplicitlyReturnedFormalArgs` with type `ty1, ..., tyN`,
          the formal return type is `rty * ty1 * ... * tyN`. If `rty` is `unit` then the formal
            return type is `ty1 * ... * tyN`.
        - Otherwise the formal return type is `rty`.

6. Check and elaborate argument expressions. If `arg` is present:
    - Check and elaborate each unnamed actual argument expression `argi`. Use the
    corresponding type in `ActualArgTypes` as the initial type.
    - Check and elaborate each named actual argument expression `argi`. Use the corresponding
    type in `ActualArgTypes` as the initial type.
7. Choose a unique `M~possible` according to the following rules:
    - For each `M~possible`, determine whether the method is _applicable_ by attempting to assert
    `M~possible` as described in step 4a). If the actions in step 4a detect an inconsistent constraint set
    ([§14.5](inference-procedures.md#constraint-solving)), the method is not applicable. Regardless, the overall constraint set is left unchanged
    as a result of determining the applicability of each `M~possible`.
    - If a unique applicable `M~possible` exists, choose that method. Otherwise, choose the unique _best_
    `M~possible` by applying the following criteria, in order:
        1) Prefer candidates whose use does not constrain the use of a user-introduced generic
        type annotation to be equal to another type.
        2) Prefer candidates that do not use ParamArray conversion. If two candidates both use
        ParamArray conversion with types `pty1` and `pty2`, and `pty1` feasibly subsumes `pty2`, prefer
        the second; that is, use the candidate that has the more precise type.
        3) Prefer candidates that do not have `ImplicitlyReturnedFormalArgs`.
        4) Prefer candidates that do not have `ImplicitlySuppliedFormalArgs`.
        5) If two candidates have unnamed actual argument types `ty11 ... ty1n` and `ty21 ... ty2n`, and
           each `ty1i` either
            - feasibly subsumes `ty2i`, or
            - `ty2i` is a `System.Func` type and `ty1i` is some other delegate type,

           then prefer the second candidate. That is, prefer any candidate that has the more
           specific actual argument types, and consider any `System.Func` type to be more specific
           than any other delegate type.
        6) Prefer candidates that are not extension members over candidates that are.
        7) To choose between two extension members, prefer the one that results from the most
        recent use of open.
        8) Prefer candidates that are not generic over candidates that are generic - that is, prefer
        candidates that have empty `ActualArgTypes`.

    Report an error if steps 1) through 8) do not result in the selection of a unique better method.

8. Once a unique best `M~possible` is chosen, commit that method.
9. Apply attribute checks.
10. Build the resulting elaborated expression by following these steps:
    - If the type of `obj` is a variable type or a value type, take the address of `obj` by using the
    _AddressOf_`(obj , PossiblyMutates)` operation ([§6.9.4](expressions.md#taking-the-address-of-an-elaborated-expression)).
    - Build the argument list by:
        - Passing each argument corresponding to an `UnamedFormalArgs` where the argument is an
            optional argument as a `Some` value.
        - Passing a `None` value for each argument that corresponds to an `ImplicitlySuppliedFormalArgs`.
        - Applying coercion to arguments.

    - Bind `ImplicitlyReturnedFormalArgs` arguments by introducing mutable temporaries for each
    argument, passing them as `byref` parameters, and building a tuple from these mutable
    temporaries and any method return value as the overall result.
    - For each `NamedActualArgs` whose target is a settable property or field, assign the value into
    the property.
    - If `arg` is not present, return a function expression that represents a first class function value.


Two additional rules apply when checking arguments (see [§8.13.7](type-definitions.md#type-directed-conversions-at-member-invocations) for examples):

  - If a formal parameter has delegate type `D`, an actual argument `farg` has known type
      `ty1 -> ... -> tyn -> rty`, and the number of arguments of the Invoke method of delegate type
      `D` is precisely `n`, interpret the formal parameter in the same way as the following:
         `new D (fun arg1 ... argn -> farg arg1 ... argn)`.

    For more information on the conversions that are automatically applied to arguments, see
  [§8.13.6](type-definitions.md#optional-arguments-to-method-members).
  
  - If a formal parameter is an `out` parameter of type `byref<ty>`, and an actual argument type is
      not a byref type, interpret the actual parameter in the same way as type `ref<ty>`. That is, an F#
      reference cell can be passed where a `byref<ty>` is expected.

One effect of these additional rules is that a method that is used as a first class function value can
resolve even if a method is overloaded and no further information is available. For example:

```fsharp
let r = new Random()
let roll = r.Next;;
```

_Method Application Resolution_ results in the following, despite the fact that in the standard CLI
library, `System.Random.Next` is overloaded:

```fsharp
val roll : int -> int
```
The reason is that if the initial type contains no information about the expected number of
arguments, the F# compiler assumes that the method has one argument.

### Additional Propagation of Known Type Information in F# 3.1

In the above descreiption of F# overload resolution, the argument expressions of a call to an
overloaded set of methods

```fsgrammar
callerObjArgTy.Method(callerArgExpr1 , ... callerArgExprN)
```
calling

```fsgrammar
calledObjArgTy.Method(calledArgTy1, ... calledArgTyN)
```
In F# 3.1 and subsequently, immediately prior to checking argument expressions, each argument
position of the unnamed caller arguments for the method call is analysed to propagate type
information extracted from method overloads to the expected types of lambda expressions. The
new rule is applied when

- the candidates are overloaded
- the caller argument at the given unnamed argument position is a syntactic lambda, possible
    parenthesized
- all the corresponding formal called arguments have `calledArgTy` either of

    - function type `calledArgDomainTy1 -> ... -> calledArgDomainTyN -> calledArgRangeTy`
        (after taking into account “function to delegate” adjustments), or
    - some other type which would cause an overload to be discarded

- at least one overload has enough curried lambda arguments for it corresponding expected
    function type

In this case, for each unnamed argument position, then for each overload:

- Attempt to solve `callerObjArgTy = calledObjArgTy` for the overload, if the overload is for an
    instance member. When making this application, only solve type inference variables present in
    the `calledObjArgTy`. If any of these conversions fail, then skip the overload for the purposes of
    this rule
- Attempt to solve `callerArgTy = (calledArgDomainTy1_ -> ... -> calledArgDomainTyN_ -> ?)`. If
    this fails, then skip the overload for the purposes of this rule

### Conditional Compilation of Member Calls

If a member definition has the `System.Diagnostics.Conditional` attribute, then any application of
the member is adjusted as follows:

- The `Conditional("symbol")` attribute may apply to methods only.
- Methods that have the `Conditional` attribute must have return type `unit`. The return type may
    be checked either on use of the method or definition of the method.
- If `symbol` is not in the current set of conditional compilation symbols, the compiler eliminates
    application expressions that resolve to calls to members that have the `Conditional` attribute and
    ensures that arguments are not evaluated. Elimination of such expressions proceeds first with
    static members and then with instance members, as follows:
    - Static members: `Type.M(args)` => `()`
    - Instance members: `expr.M(args)` => `()`

### Implicit Insertion of Flexibility for Uses of Functions and Members

At each use of a data constructor, named function, or member that forms an expression, flexibility is
implicitly added to the expression. This flexibility is associated with the use of the function or
member, according to the inferred type of the expression. The added flexibility allows the item to
accept arguments that are statically known to be subtypes of argument types to a function without
requiring explicit upcasts

The flexibility is added by adjusting each expression `expr` which represents a use of a function or
member as follows:

- The type of the function or member is decomposed to the following form:

    ```fsgrammar
    ty11 * ... * ty1n -> ... -> tym1 * ... * tymn -> rty
    ```
- If the type does not decompose to this form, no flexibility is added.
- The positions `tyij` are called the “parameter positions” for the type. For each parameter position
    where `tyij` is not a sealed type, and is not a variable type, the type is replaced by a fresh type
    variable `ty'ij` with a coercion constraint `ty'ij :> tyij`.
- After the addition of flexibility, the expression elaborates to an expression of type

    ```fsgrammar
    ty'11 * ... * ty'1n -> ... -> ty'm1 * ... * ty'mn -> rty
    ```

    but otherwise is semantically equivalent to `expr` by creating an anonymous function expression
    and inserting appropariate coercions on arguments where necessary.

This means that F# functions whose inferred type includes an unsealed type in argument position
may be passed subtypes when called, without the need for explicit upcasts. For example:

```fsharp
type Base() =
    member b.X = 1

type Derived(i : int) =
    inherit Base()
    member d.Y = i

let d = new Derived(7)

let f (b : Base) = b.X

// Call f: Base -> int with an instance of type Derived
let res = f d

// Use f as a first-class function value of type : Derived -> int
let res2 = (f : Derived -> int)
```
The F# compiler determines whether to insert flexibility after explicit instantiation, but before any
arguments are checked. For example, given the following:

```fsharp
let M<'b>(c :'b, d :'b) = 1
let obj = new obj()
let str = ""
```
these expressions pass type-checking:

```fsharp
M<obj>(obj, str)
M<obj>(str, obj)
M<obj>(obj, obj)
M<obj>(str, str)
M(obj, obj)
M(str, str)
```
These expressions do not, because the target type is a variable type:

```fsharp
M(obj, str)
M(str, obj)
```
## Constraint Solving

Constraint solving involves processing (“solving”) non-primitive constraints to reduce them to
primitive, normalized constraints on type variables. The F# compiler invokes constraint solving every
time it adds a constraint to the set of current inference constraints at any point during type
checking.

Given a type inference environment, the _normalized form_ of constraints is a list of the following
primitive constraints where `typar` is a type inference variable:

```fsother
typar :> type
typar : null
( type or ... or type ) : ( member-sig )
typar : (new : unit -> 'T)
typar : struct
typar : unmanaged
typar : comparison
typar : equality
typar : not struct
typar : enum< type >
typar : delegate< type, type >
```
Each newly introduced constraint is solved as described in the following sections.


### Solving Equational Constraints

New equational constraints in the form `typar = type` or `type = typar` , where `typar` is a type
inference variable, cause `type` to replace `typar` in the constraint problem; `typar` is eliminated. Other
constraints that are associated with `typar` are then no longer primitive and are solved again.

New equational constraints of the form `type<tyarg11,..., tyarg1n> = type<tyarg21, ..., tyarg2n>`
are reduced to a series of constraints `tyarg1i = tyarg2i` on identical named types and solved again.

### Solving Subtype Constraints

Primitive constraints in the form `typar :> obj` are discarded.

New constraints in the form `type1 :> type2`, where `type2` is a sealed type, are reduced to the
constraint `type1` = `type2` and solved again.

New constraints in either of these two forms are reduced to the constraints 
`tyarg11 = tyarg 21 ... tyarg1n = tyarg2n` and solved again:

```fsother
type<tyarg11, ..., tyarg1n> :> type<tyarg21, ..., tyarg2n>
type<tyarg11, ..., tyarg1n> = type<tyarg21, ..., tyarg2n>
```

> Note: F# generic types do not support covariance or contravariance. That is, although
single-dimensional array types in the CLI are effectively covariant, F# treats these types
as invariant during constraint solving. Likewise, F# considers CLI delegate types as
invariant and ignores any CLI variance type annotations on generic interface types and
generic delegate types.

New constraints of the form `type1<tyarg11, ..., tyarg1n> :> type2<tyarg21, ..., tyarg2n>` where
`type1` and `type2` are hierarchically related, are reduced to an equational constraint on two
instantiations of `type2` according to the subtype relation between `type1` and `type2`, and solved again.

For example, if `MySubClass<'T>` is derived from `MyBaseClass<list<'T>>`, then the constraint

```fsother
MySubClass<'T> :> MyBaseClass<int>
```
is reduced to the constraint

```fsother
MyBaseClass<list<'T>> :> MyBaseClass<list<int>>
```
and solved again, so that the constraint `'T = int` will eventually be derived.

> Note : Subtype constraints on single-dimensional array types `ty[] :> ty` are reduced to
residual constraints, because these types are considered to be subtypes of `System.Array`,
`System.Collections.Generic.IList<'T>`, `System.Collections.Generic.ICollection<'T>`,
and `System.Collections.Generic.IEnumerable<'T>`. Multidimensional array types
`ty[,...,]` are also subtypes of `System.Array`.
<br>Types from other CLI languages may, in theory, support multiple instantiations of the
same interface type, such as `C : I<int>, I<string>`. Consequently, it is more difficult to
solve a constraint such as `C :> I<'T>`. Such constraints are rarely used in practice in F#
coding. To solve this constraint, the F# compiler reduces it to a constraint `C :> I<'T>`,
where `I<'T>` is the first interface type that occurs in the tree of supported interface
types, when the tree is ordered from most derived to least derived, and iterated left-to-
right in the order of the declarations in the CLI metadata.
<br>The F# compiler ignores CLI variance type annotations on interfaces.

New constraints of the form `type :> 'b` are solved again as `type = 'b`.

> Note : Such constraints typically occur only in calls to generic code from other CLI
languages where a method accepts a parameter of a “naked” variable type—for
example, a C# 2.0 function with a signature such as `T Choose<'T>(T x, T y)`.

### Solving Nullness, Struct, and Other Simple Constraints

New constraints in any of the following forms, where `type` is not a variable type, are reduced to
further constraints:

```fsother
type : null
type : (new : unit -> 'T)
type : struct
type : not struct
type : enum< type >
type : delegate< type, type >
type : unmanaged
```
The compiler then resolves them according to the requirements for each kind of constraint listed in
[§5.2](types-and-type-constraints.md#type-constraints) and [§5.4.8](types-and-type-constraints.md#nullness).

### Solving Member Constraints

New constraints in the following form are solved as _member constraints_ ([§5.2.3](types-and-type-constraints.md#member-constraints)):

```fsother
(type1 or ... or typen) : (member-sig)
```
A member constraint is satisfied if one of the types in the _support set_ `type1 ... typen` satisfies the
member constraint. A static type `type` satisfies a member constraint in the form
`(static~opt member ident : arg-type1 * ... * arg-typen -> ret-type)`
if all of the following are true:

- `type` is a named type whose type definition contains the following member, which takes `n`
    arguments:
    `static~opt member ident : formal-arg-type1 * ... * formal-arg-typen -> ret-type`
- The `type` and the constraint are both marked `static` or neither is marked `static`.
- The assertion of type inference constraints on the arguments and return types does not result in
    a type inference error.

As mentioned in [§5.2.3](types-and-type-constraints.md#member-constraints), a type variable may not be involved in the support set of more than one
member constraint that has the same name, staticness, argument arity, and support set. If a type
variable is in the support set of more than one such constraint, the argument and return types are
themselves constrained to be equal.

#### Simulation of Solutions for Member Constraints
Certain types are assumed to implicitly define static members even though the actual CLI metadata
for types does not define these operators. This mechanism is used to implement the extensible
conversion and math functions of the F# library including `sin`, `cos`, `int`, `float`, `(+)`, and `(-)`. The
following table shows the static members that are implicitly defined for various types.

| Type | Implicitly defined static members |
| --- | --- |
| Integral types:<br> `byte`, `sbyte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `nativeint`, `unativeint` | `op_BitwiseAnd`, `op_BitwiseOr`, `op_ExclusiveOr`, `op_LeftShift`, `op_RightShift`, `op_UnaryPlus`, `op_UnaryNegation`, `op_Increment`, `op_Decrement`, `op_LogicalNot`, `op_OnesComplement`, `op_Addition`, `op_Subtraction`, `op_Multiply`, `op_Division`, `op_Modulus`, `op_UnaryPlus`<br>`op_Explicit`: takes the type as an argument and returns `byte`, `sbyte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `float32`, `float`, `decimal`, `nativeint`, or `unativeint` |
| Signed integral CLI types:<br> `sbyte`, `int16`, `int32`, `int64` and `nativeint` | `op_UnaryNegation`, `Sign`, `Abs` |
| Floating-point CLI types:<br>`float32` and `float` | `Sin`, `Cos`, `Tan`, `Sinh`, `Cosh`, `Tanh`, `Atan`, `Acos`, `Asin`, `Exp`, `Ceiling`, `Floor`, `Round`, `Log10`, `Log`, `Sqrt`, `Atan2`, `Pow`, `op_Addition`, `op_Subtraction`, `op_Multiply`, `op_Division`, `op_Modulus`, `op_UnaryPlus`, `op_UnaryNegation`, `Sign`, `Abs` <br> `op_Explicit`: takes the type as an argument and returns `byte`, `sbyte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `float32`, `float`, `decimal`, `nativeint`, or `unativeint` |
| decimal type <br>**Note** : The decimal type is included only for the Sign static member. This is deliberate: in the CLI, `System.Decimal` includes the definition of static members such as `op_Addition` and the F# compiler does not need to simulate the existence of these methods. | `Sign` |
| String type `string` | `op_Addition` <br> `op_Explicit`: takes the type as an argument and return `byte`, `sbyte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `float32`, `float` or `decimal`. |

### Over-constrained User Type Annotations

An implementation of F# must give a warning if a type inference variable that results from a user
type annotation is constrained to be a type other than another type inference variable. For example,
the following results in a warning because `'T` has been constrained to be precisely `string`:

```fsharp
let f (x:'T) = (x:string)
```
During the resolution of overloaded methods, resolutions that do not give such a warning are
preferred over resolutions that do give such a warning.

## Checking and Elaborating Function, Value, and Member Definitions

This section describes how function, value, and member definitions are checked, generalized, and
elaborated. These definitions occur in the following contexts:

- Module declarations
- Class type declarations
- Expressions
- Computation expressions

Recursive definitions can also occur in each of these locations. In addition, member definitions in a
mutually recursive group of type declarations are implicitly recursive.

Each definition is one of the following:

- A function definition :

    ```fsgrammar
    inline~opt ident1 pat1 ... patn :~opt return-type~opt = rhs-expr
    ```
- A value definition, which defines one or more values by matching a pattern against an expression:
    ```fsgrammar
    mutable~opt pat :~opt type~opt = rhs-expr
    ```
- A member definition:

    ```fsgrammar
    static~opt member ident~opt ident pat1 ... patn = expr
    ```
For a function, value, or member definition in a class:

1. If the definition is an instance function, value or member, checking uses an environment to
    which both of the following have been added:
    - The instance variable for the class, if one is present.
    - All previous function and value definitions for the type, whether static or instance.
2. If the definition is static (that is, a static function, value or member defeinition), checking uses an
    environment to which all previous static function, value, and member definitions for the type
    have been added.

### Ambiguities in Function and Value Definitions

In one case, an ambiguity exists between the syntax for function and value definitions. In particular,
`ident pat = expr` can be interpreted as either a function or value definition. For example, consider
the following:

```fsharp
type OneInteger = Id of int

let Id x = x
```
In this case, the ambiguity is whether `Id x` is a pattern that matches values of type `OneInteger` or is
the function name and argument list of a function called `Id`. In F# this ambiguity is always resolved
as a function definition. In this case, to make a value definition, use the following syntax in which the
ambiguous pattern is enclosed in parentheses:

```fsharp
let v = if 3 = 4 then Id "yes" else Id "no"
let (Id answer) = v
```
### Mutable Value Definitions

Value definitions may be marked as mutable. For example:

```fsharp
let mutable v = 0
while v < 10 do
    v <- v + 1
    printfn "v = %d" v
```
These variables are implicitly dereferenced when used.

### Processing Value Definitions

A value definition `pat = rhs-expr` with optional pattern type `type` is processed as follows:

1. The pattern `pat` is checked against a fresh initial type `ty` (or `type` if such a type is present). This
    check results in zero or more identifiers `ident1 ... identm`, each of type `ty1` ... `tym`.
2. The expression `rhs-expr` is checked against initial type `ty`, resulting in an elaborated form `expr`.
3. Each `identi` (of type `tyi`) is then generalized ([§14.6.7](inference-procedures.md#generalization)) and yields generic parameters `<typarsj>`.
4. The following rules are checked:
    - All `identj` must be distinct.
    - Value definitions may not be `inline`.
5. If `pat` is a single value pattern, the resulting elaborated definition is:

    ```fsgrammar
    ident<typars1> = expr
    body-expr
    ```
6. Otherwise, the resulting elaborated definitions are the following, where `tmp` is a fresh identifier
    and each `expri` results from the compilation of the pattern `pat` ([§7](patterns.md#patterns)) against input `tmp`.

    ```fsgrammar
    tmp<typars1 ... typarsn> = expr
    ident1<typars1> = expr1
    ...
    identn<typarsn> = exprn
    ```
### Processing Function Definitions

A function definition `ident1 pat1 ... patn = rhs-expr` is processed as follows:

1. If `ident1` is an active pattern identifier then active pattern result tags are added to the
    environment ([§10.2.4](namespaces-and-modules.md#active-pattern-definitions-in-modules)).
2. The expression `(fun pat1 ... patn : return-type -> rhs-expr)` is checked against a fresh initial
    type `ty1` and reduced to an elaborated form `expr1`. The return type is omitted if the definition
    does not specify it.
3. The `ident1` (of type `ty1`) is then generalized ([§14.6.7](inference-procedures.md#generalization)) and yields generic parameters `<typars1>`.
4. The following rules are checked:
    - Function definitions may not be `mutable`. Mutable function values should be written as follows:
    ```fsother
    let mutable f = (fun args -> ...)`
    ```
    - The patterns of functions may not include optional arguments ([§8.13.6](type-definitions.md#optional-arguments-to-method-members)).
5. The resulting elaborated definition is:

    ```fsgrammar
    ident1<typars1> = expr1
    ```
### Processing Recursive Groups of Definitions

A group of functions and values may be declared recursive through the use of `let rec`. Groups of
members in a recursive set of type definitions are also implicitly recursive. In this case, the defined
values are available for use within their own definitions—that is, within all the expressions on the
right-hand side of the definitions.

For example:

```fsharp
let rec twoForward count =
    printfn "at %d, taking two steps forward" count
    if count = 1000 then "got there!"
    else oneBack (count + 2)
and oneBack count =
    printfn "at %d, taking one step back " count
    twoForward (count – 1)
```
When one or more definitions specifies a value, the recursive expressions are analyzed for safety
([§14.6.6](inference-procedures.md#recursive-safety-analysis)). This analysis may result in warnings—including some reported at compile time—and
runtime checks.

Within recursive groups, each definition in the group is checked ([§14.6.7](inference-procedures.md#generalization)) and then the definitions
are generalized incrementally. In addition, any use of an ungeneralized recursive definition results in
immediate constraints on the recursively defined construct. For example, consider the following
declaration:

```fsharp
let rec countDown count x =
    if count > 0 then
        let a = countDown (count - 1) 1 // constrains "x" to be of type int
        let b = countDown (count – 1) "Hello" // constrains "x" to be of type string
        a + b
    else
        1
```
In this example, the definition is not valid because the recursive uses of `f` result in inconsistent
constraints on `x`.


If a definition has a full signature, early generalization applies and recursive calls at different types
are permitted ([§14.6.7](inference-procedures.md#generalization)). For example:

```fsharp
module M =
    let rec f<'T> (x:'T) : 'T =
        let a = f 1
        let b = f "Hello"
        x
```
In this example, the definition is valid because `f` is subject to early generalization, and so the
recursive uses of `f` do not result in inconsistent constraints on `x`.

### Recursive Safety Analysis

A set of recursive definitions may include value definitions. For example:

```fsharp
type Reactor = React of (int -> React) * int

let rec zero = React((fun c -> zero), 0)

let const n =
    let rec r = React((fun c -> r), n)
    r
```
Recursive value definitions may result in invalid recursive cycles, such as the following:

```fsharp
let rec x = x + 1
```
The _Recursive Safety Analysis_ process partially checks the safety of these definitions and convert
thems to a form that uses lazy initialization, where runtime checks are inserted to check
initialization.

A right-hand side expression is _safe_ if it is any of the following:

- A function expression, including those whose bodies include references to variables that are
    defined recursively.
- An object expression that implements an interface, including interfaces whose member bodies
    include references to variables that are being defined recursively.
- A `lazy` delayed expression.
- A record, tuple, list, or data construction expression whose field initialization expressions are all
    safe.
- A value that is not being recursively bound.
- A value that is being recursively bound and appears in one of the following positions:
    - As a field initializer for a field of a record type where the field is marked `mutable`.
    - As a field initializer for an immutable field of a record type that is defined in the current
       assembly.
       If record fields contain recursive references to values being bound, the record fields must be
       initialized in the same order as their declared type, as described later in this section.


- Any expression that refers only to earlier variables defined by the sequence of recursive
    definitions.

Other right-hand side expressions are elaborated by adding a new definition. If the original definition
is

```fsharp
u = expr
```
then a fresh value (say v) is generated with the definition:

```fsharp
v = lazy expr
```
and occurrences of the original variable `u` on the right-hand side are replaced by `Lazy.force v`. The
following definition is then added at the end of the definition list:

```fsharp
u = v .Force()
```

> Note: This specification implies that recursive value definitions are executed as an
initialization graph of delayed computations. Some recursive references may be checked
at runtime because the computations that are involved in evaluating the definitions
might actually execute the delayed computations. The F# compiler gives a warning for
recursive value definitions that might involve a runtime check. If runtime self-reference
does occur then an exception will be raised.
<br>Recursive value definitions that involve computation are useful when defining objects
such as forms, controls, and services that respond to various inputs. For example, GUI
elements that store and retrieve the state of the GUI elements as part of their
specification typically involve recursive value definitions. A simple example is the
following menu item, which prints out part of its state when invoked:
```fsharp
open System.Windows.Form
let rec menuItem : MenuItem =
    new MenuItem("&Say Hello",
                 new EventHandler(fun sender e ->
                     printfn "Text = %s" menuItem.Text),
                 Shortcut.CtrlH)
```
> This code results in a compiler warning because, in theory, the
`new MenuItem(...)` constructor might evaluate the callback as part of the construction
process. However, because the `System.Windows.Forms` library is well designed, in this
example this does not happen in practice, and so the warning can be suppressed or
ignored by using compiler options.

The F# compiler performs a simple approximate static analysis to determine whether immediate
cyclic dependencies are certain to occur during the evaluation of a set of recursive value definitions.
The compiler creates a graph of definite references and reports an error if such a dependency cycle
exists. All references within function expressions, object expressions, or delayed expressions are
assumed to be indefinite, which makes the analysis an under-approximation. As a result, this check
catches naive and direct immediate recursion dependencies, such as the following:

```fsharp
let rec A = B + 1
and B = A + 1
```

Here, a compile-time error is reported. This check is necessarily approximate because dependencies
under function expressions are assumed to be delayed, and in this case the use of a lazy initialization
means that runtime checks and forces are inserted.

> Note: In F# 3 .1 this check does not apply to value definitions that are generic through
generalization because a generic value definition is not executed immediately, but is
instead represented as a generic method. For example, the following value definitions
are generic because each right-hand-side is generalizable:
```fsharp
let rec a = b
and b = a
```
> In compiled code they are represented as a pair of generic methods, as if the code had
been written as follows:
```fsharp
let rec a<'T>() = b<'T>()
and b<'T>() = a<'T>()
```
> As a result, the definitions are not executed immediately unless the functions are called.
Such definitions indicate a programmer error, because executing such generic,
immediately recursive definitions results in an infinite loop or an exception. In practice
these definitions only occur in pathological examples, because value definitions are
generalizable only when the right-hand-side is very simple, such as a single value. Where
this issue is a concern, type annotations can be added to existing value definitions to
ensure they are not generic. For example:
```fsharp
let rec a : int = b
and b : int = a
```
> In this case, the definitions are not generic. The compiler performs immediate
dependency analysis and reports an error. In addition, record fields in recursive data
expressions must be initialized in the order they are declared. For example:
```fsharp
type Foo = {
    x: int
    y: int
    parent: Foo option
    children: Foo list
}

let rec parent = { x = 0; y = 0; parent = None; children = children }
and children = [{ x = 1; y = 1; parent = Some parent; children = [] }]

printf "%A" parent
```
> Here, if the order of the fields x and y is swapped, a type-checking error occurs.

### Generalization

Generalization is the process of inferring a generic type for a definition where possible, thereby
making the construct reusable with multiple different types. Generalization is applied by default at
all function, value, and member definitions, except where listed later in this section. Generalization
also applies to member definitions that implement generic virtual methods in object expressions.

Generalization is applied incrementally to items in a recursive group after each item is checked.


Generalization takes a set of ungeneralized but type-checked definitions _checked-defns_ that form
part of a recursive group, plus a set of unchecked definitions _unchecked-defns_ that have not yet been
checked in the recursive group, and an environment _env_. Generalization involves the following steps:

1. Choose a subset `generalizable-defns` of `checked-defns` to generalize.

    A definition can be generalized if its inferred type is closed with respect to any inference
    variables that are present in the types of the `unchecked-defns` that are in the recursive group and
    that are not yet checked or which, in turn, cannot be generalized. A greatest-fixed-point
    computation repeatedly removes definitions from the set of `checked-defns` until a stable set of
    generalizable definitions remains.
2. Generalize all type inference variables that are not otherwise ungeneralizable and for which any
    of the following is true:
    - The variable is present in the inferred types of one or more of `generalizable-defns`.
    - The variable is a type parameter copied from the enclosing type definition (for members and
       “let” definitions in classes).
    - The variable is explicitly declared as a generic parameter on an item.

The following type inference variables cannot be generalized:

- A type inference variable `^typar` that is part of the inferred or declared type of a definition,
    unless the definition is marked `inline`.
- A type inference variable in an inferred type in the _ExprItems_ or _PatItems_ tables of `env` , or in
    an inferred type of a module in the _ModulesAndNamespaces_ table in `env`.
- A type inference variable that is part of the inferred or declared type of a definition in which
    the elaborated right-hand side of the definition is not a generalizable expression, as
    described later in this section.
- A type inference variable that appears in a constraint that itself refers to an ungeneralizable
    type variable.

Generalizable type variables are computed by a greatest-fixed-point computation, as follows:

1. Start with all variables that are candidates for generalization.

2. Determine a set of variables `U` that cannot be generalized because they are free in the
environment or present in ungeneralizable definitions.

3. Remove the variables in _U_ from consideration.

4. Add to `U` any inference variables that have a constraint that involves a variable in `U`.

5. Repeat steps 2 through 4.


Informally, generalizable expressions represent a subset of expressions that can be freely copied and
instantiated at multiple types without affecting the typical semantics of an F# program. The
following expressions are generalizable:

- A function expression
- An object expression that implements an interface
- A delegate expression
- A “let” definition expression in which both the right-hand side of the definition and the body of
    the expression are generalizable
- A “let rec” definition expression in which the right-hand sides of all the definitions and the body
    of the expression are generalizable
- A tuple expression, all of whose elements are generalizable
- A record expression, all of whose elements are generalizable, where the record contains no
    mutable fields
- A union case expression, all of whose arguments are generalizable
- An exception expression, all of whose arguments are generalizable
- An empty array expression
- A simple constant expression
- An application of a type function that has the `GeneralizableValue` attribute.

Explicit type parameter definitions on value and member definitions can affect the process of type
inference and generalization. In particular, a declaration that includes explicit generic parameters
will not be generalized beyond those generic parameters. For example, consider this function:

```fsharp
let f<'T> (x : 'T) y = x
```
During type inference, this will result in a function of the following type, where `'_b` is a type
inference variable that is yet to be resolved.

```fsharp
f<'T> : 'T -> '_b -> '_b
```
To permit generalization at these definitions, either remove the explicit generic parameters (if they
can be inferred), or use the required number of parameters, as the following example shows:

```fsharp
let throw<'T,'U> (x:'T) (y:'U) = x
```
### Condensation of Generalized Types

After a function or member definition is generalized, its type is condensed by removing generic type
parameters that apply subtype constraints to argument positions. (The removed flexibility is
implicitly reintroduced at each use of the defined function; see [§14.4.3](inference-procedures.md#implicit-insertion-of-flexibility-for-uses-of-functions-and-members)).

Condensation decomposes the type of a value or member to the following form:

```fsgrammar
ty11 * ... * ty1n -> ... -> tym1 * ... * tymn -> rty
```
The positions `tyij` are called the parameter positions for the type.

Condensation applies to a type parameter `'a` if all of the following are true:

- `'a` is not an explicit type parameter.
- `'a` occurs at exactly one `tyij` parameter position.
- `'a` has a single coercion constraint `'a :> ty` and no other constraints. However, one additional
    nullness constraint is permitted if `ty` satisfies the nullness constraint.
- `'a` does not occur in any other `tyij`, nor in `rty`.
- `'a` does not occur in the constraints of any condensed `typar`.

Condensation is a greatest-fixed-point computation that initially assumes all generalized type
parameters are condensed, and then progressively removes type parameters until a minimal set
remains that satisfies the above rules.

The compiler removes all condensed type parameters and replaces them with their subtype
constraint `ty`. For example:

```fsharp
let F x = (x :> System.IComparable).CompareTo(x)
```
After generalization, the function is inferred to have the following type:

```fsharp
F : 'a -> int when 'a :> System.IComparable
```
In this case, the actual inferred, generalized type for `F` is condensed to:

```fsharp
F : System.IComparable -> R
```
Condensation does not apply to arguments of unconstrained variable type. For example:

```fsharp
let ignore x = ()
```
with type

```fsharp
ignore: 'a -> unit
```
In particular, this is not condensed to

```fsharp
ignore: obj -> unit
```
In rare cases, condensation affects the points at which value types are boxed. In the following
example, the value `3` is now boxed at uses of the function:

```fsharp
F 3
```
If a function is not generalized, condensation is not applied. For example, consider the following:

```fsharp
let test1 =
    let ff = Seq.map id >> Seq.length
    (ff [1], ff [| 1 |]) // error here
```
In this example, `ff` is not generalized, because it is not defined by using a generalizable expression—
computed functions such as `Seq.map id >> Seq.length` are not generalizable. This means that its
inferred type, after processing the definition, is

```fsharp
F : '_a -> int when '_a :> seq<'_b>
```
where the type variables are not generalized and are unsolved inference variables. The application
of `ff` to `[1]` equates `'a` with `int list`, making the following the type of `F`:

```fsharp
F : int list -> int
```
The application of `ff` to an array type then causes an error. This is similar to the error returned by
the following:

```fsharp
let test1 =
    let ff = Seq.map id >> Seq.length
    (ff [1], ff ["one"]) // error here
```
Again, `ff` is not generalized, and its use with arguments of type `int list` and `string list` is not
permitted.

## Dispatch Slot Inference

The F# compiler applies _Dispatch Slot Inference_ to object expressions and type definitions before it
processes their members. For both object expressions and type definitions, the following are input
to _Dispatch Slot Inference_:

- A type `ty0` that is being implemented.
- A set of members `override x.M(arg1 ... argN)`.
- A set of additional interface types `ty1 ... tyn`.
- A further set of members `override x.M(arg1 ... argN)` for each `tyi`.

Dispatch slot inference associates each member with a unique abstract member or interface
member that the collected types `tyi` define or inherit.

The types `ty0 ... tyn` together imply a collection of _required types_ R, each of which has a set of
_required dispatch slots_ SlotsR of the form `abstract M : aty1 ... atyN -> atyrty`. Each dispatch slot is
placed under the _most-specific_ `tyi` relevant to that dispatch slot. If there is no most-specific type for
a dispatch slot, an error occurs.

For example, assume the following definitions:

```fsharp
type IA = interface abstract P : int end
type IB = interface inherit IA end
type ID = interface inherit IB end
```
With these definitions, the following object expression is legal. Type `IB` is the most-specific
implemented type that encompasses `IA`, and therefore the implementation mapping for `P` must be
listed under `IB`:

```fsharp
let x = { new ID
          interface IB with
            member x.P = 2 }
```
But given:


```fsharp
type IA = interface abstract P : int end
type IB = interface inherit IA end
type IC = interface inherit IB end
type ID = interface inherit IB inherit IC end
```
then the following object expression causes an error, because both `IB` and `IC` include the interface
`IA`, and consequently the implementation mapping for `P` is ambiguous.

```fsharp
let x = { new ID
          interface IB with
            member x.P = 2
          interface IC with
            member x.P = 2 }
```
The ambiguity can be resolved by explicitly implementing interface `IA`.

After dispatch slots are assigned to types, the compiler tries to associate each member with a
dispatch slot based on name and number of arguments. This is called _dispatch slot inference,_ and it
proceeds as follows:

- For each `member x.M(arg1 ... argN)` in type `tyi`, attempt to find a single dispatch slot in the form

    ```fsgrammar
    abstract M : aty1 ... atyN -> rty
    ```
    with name `M`, argument count `N`, and most-specific implementing type `tyi`.

- To determine the argument counts, analyze the syntax of patterns and look specifically for
    tuple and unit patterns. Thus, the following members have argument count 1, even though
    the argument type is unit:
    ```fsgrammar
    member obj.ToString(() | ()) = ...
    member obj.ToString(():unit) = ...
    member obj.ToString(_:unit) = ...
    ```
- A member may have a return type, which is ignored when determining argument counts:
    ```fsother
    member obj.ToString() : string = ...
    ```

For example, given

```fsharp
let obj1 =
    { new System.Collections.Generic.IComparer<int> with
        member x.Compare(a,b) = compare (a % 7) (b % 7) }
```
the types of `a` and `b` are inferred by looking at the signature of the implemented dispatch slot, and
are hence both inferred to be `int`.

## Dispatch Slot Checking

_Dispatch Slot Checking_ is applied to object expressions and type definitions to check consistency
properties, such as ensuring that all abstract members are implemented.

After the compiler checks all bodies of all methods, it checks that a one-to-one mapping exists
between dispatch slots and implementing members based on exact signature matching.


The interface methods and abstract method slots of a type are collectively known as _dispatch slots_.
Each object expression and type definition results in an elaborated _dispatch map_. This map is keyed
by dispatch slots, which are qualified by the declaring type of the slot. This means that a type that
supports two interfaces `I` and `I2`, both of which contain the method m, may supply different
implementations for `I.m()` and `I2.m()`.

The construction of the dispatch map for any particular type is as follows:

- If the type definition or extension has an implementation of an interface, mappings are added
    for each member of the interface,
- If the type definition or extension has a `default` or `override` member, a mapping is added for the
    associated abstract member slot.

## Byref Safety Analysis

Byref arguments are pointers that can be stack-bound and are used to pass values by reference to
procedures in CLI languages, often to simulate multiple return values. Byref pointers are not often
used in F#; more typically, tuple values are used for multiple return values. However, a byref value
can result from calling or overriding a CLI method that has a signature that involves one or more
byref values.

To ensure the safety of byref arguments, the following checks are made:

- Byref types may not be used as generic arguments.
- Byref values may not be used in any of the following:
    - The argument types or body of function expressions `(fun ... -> ...)`.
    - The member implementations of object expressions.
    - The signature or body of let-bound functions in classes.
    - The signature or body of let-bound functions in expressions.

Note that function expressions occur in:

- The elaborated form of sequence expressions.
- The elaborated form of computation expressions.
- The elaborated form of partial applications of module-bound functions and members.

In addition:

- A generic type cannot be instantiated by a byref type.
- An object field cannot have a byref type.
- A static field or module-bound value cannot have a byref type.

As a result, a byref-typed expression can occur only in these situations:

- As an argument to a call to a module-defined function or class-defined function.


- On the right-hand-side of a value definition for a byref-typed local.

These restrictions also apply to uses of the prefix && operator for generating native pointer values.

## Promotion of Escaping Mutable Locals to Objects

Value definitions whose byref address would be subject to the restrictions on `byref<_>` listed in
[§14.9](inference-procedures.md#byref-safety-analysis) are treated as implicit declarations of reference cells. For example

```fsharp
let sumSquares n =
    let mutable total = 0
    [ 1 .. n ] |> Seq.iter (fun x -> total <- total + x*x)
    total
```
is considered equivalent to the following definition:

```fsharp
let sumSquares n =
    let total = ref 0
    [ 1 .. n ] |> Seq.iter
                    (fun x -> total.contents <- total.contents + x*x)
    total.contents
```
because the following would be subject to byref safety analysis:

```fsharp
let sumSquares n =
    let mutable total = 0
    &total
```
## Arity Inference

During checking, members within types and function definitions within modules are inferred to have
an _arity_. An arity includes both of the following:

- The number of iterated (curried) arguments `n`
- A tuple length for these arguments `[A1; ...; An]`. A tuple length of zero indicates that the
    corresponding argument is of type `unit`.

Arities are inferred as follows. A function definition of the following form is given arity `[A1; ...; An]`,
where each `Ai` is derived from the tuple length for the final inferred types of the patterns:

```fsother
let ident pat 1 ... patn = ...
```
For example, the following is given arity [1; 2]:

```fsharp
let f x (y,z) = x + y + z
```
Arities are also inferred from function expressions that appear on the immediate right of a value
definition. For example, the following has an arity of [1]:

```fsharp
let f = fun x -> x + 1
```
Similarly, the following has an arity of [1;1]:


```fsharp
let f x = fun y -> x + y
```
Arity inference is applied partly to help define the elaborated form of a function definition. This is
the form that other CLI languages see. In particular:

- A function value `F` in a module that has arity `[A1 ; ...; An]` and the type
    `ty1,1 * ... * ty1,A1 -> ... -> tyn,1 * ... * tyn,An - > rty`
    elaborates to a CLI static method definition with signature
    `rty F(ty1,1, ..., ty1,A1, ..., tyn,1 , ..., tyn,An)`.
- F# instance (respectively static) methods that have arity `[A1 ; ...; An]` and type
    `ty1,1 * ... * ty1,A1 -> ... -> tyn,1 * ... * tynAn -> rty`
    elaborate to a CLI instance (respectively static) method definition with signature
    `rty F(ty1,1, ..., ty1,A1)`, subject to the syntactic restrictions that result from the patterns that
    define the member, as described later in this section.

For example, consider a function in a module with the following definition:

```fsharp
let AddThemUp x (y, z) = x + y + z
```
This function compiles to a CLI static method with the following C# signature:

```fsharp
int AddThemUp(int x, int y, int z);
```
Arity inference applies differently to function and member definitions. Arity inference on function
definitions is fully type-directed. Arity inference on members is limited if parentheses or other
patterns are used to specify the member arguments. For example:

```fsharp
module Foo =
    // compiles as a static method taking 3 arguments
    let test1 (a1: int, a2: float, a3: string) = ()

    // compiles as a static method taking 3 arguments
    let test2 (aTuple : int * float * string) = ()

    // compiles as a static method taking 3 arguments
    let test3 ( (aTuple : int * float * string) ) = ()

    // compiles as a static method taking 3 arguments
    let test4 ( (a1: int, a2: float, a3: string) ) = ()

    // compiles as a static method taking 3 arguments
    let test5 (a1, a2, a3 : int * float * string) = ()

type Bar() =
    // compiles as a static method taking 3 arguments
    static member Test1 (a1: int, a2: float, a3: string) = ()
    
    // compiles as a static method taking 1 tupled argument
    static member Test2 (aTuple : int * float * string) = ()
    
    // compiles as a static method taking 1 tupled argument
    static member Test3 ( (aTuple : int * float * string) ) = ()
    
    // compiles as a static method taking 1 tupled argument
    static member Test4 ( (a1: int, a2: float, a3: string) ) = ()
    
    // compiles as a static method taking 1 tupled argument
    static member Test5 (a1, a2, a3 : int * float * string) = ()
```
## Additional Constraints on CLI Methods

F# treats some CLI methods and types specially, because they are common in F# programming and
cause extremely difficult-to-find bugs. For each use of the following constructs, the F# compiler
imposes additional _ad hoc_ constraints:

`x.Equals(yobj)` requires type `ty : equality` for the static type of `x`

`x.GetHashCode()` requires type `ty : equality` for the static type of `x`

`new Dictionary<A,B>()` requires `A : equality`, for any overload that does not take an
`IEqualityComparer<T>`

No constraints are added for the following operations. Consider writing wrappers around these
functions to improve the type safety of the operations.

`System.Array.BinarySearch<T>(array,value)` requiring `C : comparison`, for any overload that
does not take an `IComparer<T>`

`System.Array.IndexOf` requiring `C : equality`

`System.Array.LastIndexOf(array,T)` requiring `C : equality`

`System.Array.Sort<'T>(array)` requiring `C : comparison`, for any overload that does not take an
`IEqualityComparer<T>`

`new SortedList<A,B>()` requiring `A : comparison`, for any overload that does not take an
`IEqualityComparer<T>`

`new SortedDictionary<A,B>()` requiring `C : comparison`, for any overload that does not take an
`IEqualityComparer<_>`
