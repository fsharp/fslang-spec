# Namespace and Module Signatures

A signature file contains one or more namespace or module signatures, and specifies the
functionality that is implemented by its corresponding implementation file. It also can hide
functionality that the corresponding implementation file contains.

```fsgrammar
namespace-decl-group-signature :=
    namespace long-ident module-signature-elements

module-signature :=
    module ident = module-signature-body

module-signature-element :=
    val mutable~opt curried-sig     -- value signature
    val value-defn                  -- literal value signature
    type type-signatures            -- type(s) signature
    exception exception-signature   -- exception signature
    module-signature                -- submodule signature
    module-abbrev                   -- local alias for a module
    import-decl                     -- locally import contents of a module

module-signature-elements := module-signature-element ... module-signature-element

module-signature-body :=
    begin module-signature-elements end

type-signature :=
    abbrev-type-signature
    record-type-signature
    union-type-signature
    anon-type-signature
    class-type-signature
    struct-type-signature
    interface-type-signature
    enum-type-signature
    delegate-type-signature
    type-extension-signature

type-signatures := type-signature ... and ... type-signature

type-signature-element :=
    attributesopt access~opt new : uncurried-sig        -- constructor signature
    attributesopt member acces~opt member-sig           -- member signature
    attributesopt abstract access~opt member-sig        -- member signature
    attributesopt override member-sig                   -- member signature
    attributesopt default member-sig                    -- member signature
    attributes~opt static member access~opt member-sig  -- static member signature
    interface type                                      -- interface signature

abbrev-type-signature := type-name '=' type

union-type-signature := type-name '=' union-type-cases type-extension-elements-
    signature~opt

record-type-signature := type-name '=' '{' record-fields '}' type-extension-
    elements-signature~opt

anon-type-signature := type-name '=' begin type-elements-signature end

class-type-signature := type-name '=' class type-elements-signature end

struct-type-signature := type-name '=' struct type-elements-signature end

interface-type-signature := type-name '=' interface type-elements-signature end

enum-type-signature := type-name ' =' enum-type-cases

delegate-type-signature := type-name ' =' delegate-sig

type-extension-signature := type-name type-extension-elements-signature

type-extension-elements-signature := with type-elements-signature end
```

The `begin` and `end` tokens are optional when lightweight syntax is used.

Like module declarations, signature declarations are processed sequentially rather than
simultaneously, so that later signature declarations are not in scope when earlier ones are
processed.

```fsharp
namespace Utilities.Part1

    module Module1 =
        val x : Utilities.Part2.StorageCache // error (Part2 not yet declared)

namespace Utilities.Part2

    type StorageCache =
        new : unit -> unit
```

## Signature Elements

A namespace or module signature declares one or more _value signatures_ and one or more _type
definition signatures_. A type definition signature may include one or more _member signatures_ , in
addition to other elements of type definitions that are specified in the signature grammar at the
start of this chapter.

### Value Signatures

A value signature indicates that a value exists in the implementation. For example, in the signature
of a module, the following declares two value signatures:

```fsharp
module MyMap =
    val mapForward : index1: int * index2: int -> string
    val mapBackward : name: string -> (int * int)
```

The corresponding implementation file might contain the following implementation:

```fsharp
module MyMap =
    let mapForward (index1:int, index2:int) = string index1 + "," + string index2
    let mapBackward (name:string) = (0, 0)
```

### Type Definition and Member Signatures

A type definition signature indicates that a corresponding type definition appears in the
implementation. For example, in an interface type, the following declares a type definition signature
for `Forward` and `Backward`:

```fsharp
type IMap =
    interface
        abstract Forward : index1: int * index2: int -> string
        abstract Backward : name: string -> (int * int)
    end
```

A member signature indicates that a corresponding member appears on the corresponding type
definition in the implementation. Member specifications must specify argument and return types,
and can optionally specify names and attributes for parameters.

For example, the following declares a type definition signature for a type with one constructor
member, one property member `Kind` and one method member `Purr`:

```fsharp
type Cat =
    new : kind:string -> Cat
    member Kind : string
    member Purr : unit -> Cat
```

The corresponding implementation file might contain the following implementation:

```fsharp
type Cat(kind: string) =
    member x.Meow() = printfn "meow"
    member x.Purr() = printfn "purr"
    member x.Kind = kind
```

## Signature Conformance

Values, types, and members that are present in implementations can be omitted in signatures, with
the following exceptions:

- Type abbreviations may not be hidden by signatures. That is, a type abbreviation `type T = ty` in
    an implementation does not match type `T` (without an abbreviation) in a signature.
- Any type that is represented as a record or union must reveal either all or none of its fields or
    cases, in the same order as that specified in the implementation. Types that are represented as
    classes may reveal some, all, or none of their fields in a signature.
- Any type that is revealed to be an interface, or a type that is a class or struct with one or more
    constructors may not hide its `inherit` declaration, abstract dispatch slot declarations, or abstract
    interface declarations.

> Note: This section does not yet document all checks made by the F# 3.1 language
implementation.

### Signature Conformance for Functions and Values

If both a signature and an implementation contain a function or value definition with a given name,
the signature and implementation must conform as follows:

- The declared accessibilities, `inline`, and `mutable` modifiers must be identical in both the
    signature and the implementation.
- If either the signature or the implementation has the `[<Literal>]` attribute, both must have this
    attribute. Furthermore, the declared literal values must be identical.
- The number of generic parameters—both inferred and explicit—must be identical.
- The types and type constraints must be identical up to renaming of inferred and/or explicit
    generic parameters. For example, assume a signature is written `val head : seq<'T> -> 'T` and
    the compiler could infer the type `val head : seq<'a> -> 'a` from the implementation. These
    are considered identical up to renaming the generic parameters.
- The arities must match, as described in the next section.

#### Arity Conformance for Functions and Values

Arities of functions and values must conform between implementation and signature. Arities of
values are implicit in module signatures. A signature that contains the following results in the arity
`[A1 ... An]` for `F`:

```fsgrammar
val F : ty11 * ... * ty1A1 - > ... -> tyn1 * ... * tynAn - > rty
```

Arities in a signature must be equal to or shorter than the corresponding arities in an
implementation, and the prefix must match. This means that F# makes a deliberate distinction
between the following two signatures:

```fsharp
val F: int -> int
```

and

```fsharp
val F: (int -> int)
```

The parentheses indicate a top-level function, which might be a first-class computed expression that
computes to a function value, rather than a compile-time function value.

The first signature can be satisfied only by a true function; that is, the implementation must be a
lambda value as in the following:

```fsharp
let F x = x + 1
```

> Note: Because arity inference also permits right-hand-side function expressions, the
implementation may currently also be:
<br>`let F = fun x -> x + 1`

The second signature

```fsharp
val F: (int -> int)
```

can be satisfied by any value of the appropriate type. For example:

```fsharp
let f =
    let myTable = new System.Collections.Generic.Dictionary<int,int>(4)
    fun x ->
        if myTable.ContainsKey x then
            myTable.[x]
        else
            let res = x * x
            myTable.[x] <- res
            res
```

—or—

```fsharp
let f = fun x -> x + 1
```

—or—

```fsharp
// throw an exception as soon as the module initialization is triggered
let f : int -> int = failwith "failure"
```

For both the first and second signatures, you can still use the functions as first-class function values
from client code—the parentheses simply act as a constraint on the implementation of the value.

The reason for this interpretation of types in value and member signatures is that CLI
interoperability requires that F# functions compile to methods, rather than to fields that are
function values. Thus, signatures must contain enough information to reveal the desired arity of a
method as it is revealed to other CLI programming languages.

#### Signature Conformance for Type Functions

If a value is a type function, then its corresponding value signature must have explicit type
arguments. For example, the implementation

```fsharp
let empty<'T> : list<'T> = printfn "hello"; []
```

conforms to this signature:

```fsharp
val empty<'T> : list<'T>
```

but not to this signature:

```fsharp
val empty : list<'T>
```

The reason for this rule is that the second signature indicates that the value is, by default,
generalizable ([§14.6.7](inference-procedures.md#generalization)).

### Signature Conformance for Members

If both a signature and an implementation contain a member with a given name, the signature and
implementation must conform as follows:

- If one is an extension member, both must be extension members.
- If one is a constructor, then both must be constructors.
- If one is a property, then both must be properties.

- The types must be identical up to renaming of inferred or explicit type parameters (as for
    functions and values).
- The `static`, `abstract`, and `override` qualifiers must match precisely.
- Abstract members must be present in the signature if a representation is given for a type.

> Note: This section does not yet document all checks made by the F# 3 .1 language
implementation.
