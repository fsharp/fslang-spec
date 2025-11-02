# Type Definitions

Type definitions define new named types. The grammar of type definitions is shown below.

```fsgrammar
type-defn :=
    abbrev-type-defn
    record-type-defn
    union-type-defn
    anon-type-defn
    class-type-defn
    struct-type-defn
    interface-type-defn
    enum-type-defn
    delegate-type-defn
    type-extension

type-name :=
    attributes~opt access~opt ident typar-defns~opt

abbrev-type-defn :=
    type-name = type

union-type-defn :=
    type-name '=' union-type-cases type-extension-elements~opt

union-type-cases :=
    '|'~opt union-type-case '|' ... '|' union-type-case

union-type-case :=
    attributes~opt union-type-case-data

union-type-case-data :=
    ident -- null union case
    ident of union-type-field * ... * union-type-field -- n-ary union case
    ident : uncurried-sig -- n-ary union case

union-type-field :=
    type -- unnamed union fiels
    ident : type -- named union field

record-type-defn :=
    type-name = '{' record-fields '}' type-extension-elements~opt

record-fields :=
    record-field ; ... ; record-field ;~opt

record-field :=
    attributes~opt mutable~opt access~opt ident : type

anon-type-defn :=
    type-name primary-constr-args~opt object-val~opt '=' begin class-type-body end

class-type-defn :=
    type-name primary-constr-args~opt object-val~opt '=' class class-type-body end

as-defn := as ident

class-type-body :=
    class-inherits-decl~opt class-function-or-value-defns~opt type-defn-elements~opt

class-inherits-decl := inherit type expr~opt

class-function-or-value-defn :=
    attributes~opt static~opt let rec~opt function-or-value-defns
    attributes~opt static~opt do expr

struct-type-defn :=
    type-name primary-constr-args~opt as-defn~opt '=' struct struct-type-body end

struct-type-body := type-defn-elements

interface-type-defn :=
    type-name '=' interface interface-type-body end

interface-type-body := type-defn-elements

exception-defn :=
    attributes~opt exception union-type-case-data -- exception definition
    attributes~opt exception ident = long-ident -- exception abbreviation

enum-type-defn :=
    type-name '=' enum-type-cases

enum-type-cases =
    '|'~opt enum-type-case '|' ... '|' enum-type-case

enum-type-case :=
    ident '=' const -- enum constant definition

delegate-type-defn :=
    type-name '=' delegate-sig

delegate-sig :=
    delegate of uncurried-sig -- CLI delegate definition

type-extension :=
    type-name type-extension-elements

type-extension-elements := with type-defn-elements end

type-defn-element :=
    member-defn
    interface-impl
    interface-spec

type-defn-elements := type-defn-element ... type-defn-element

primary-constr-args :=
    attributes~opt access~opt ( simple-pat, ... , simple-pat )

simple-pat :=
    | ident
    | simple-pat : type

additional-constr-defn :=
    attributes~opt access~opt new pat as-defn = additional-constr-expr

additional-constr-expr :=
    stmt ';' additional-constr-expr -- sequence construction (after)
    additional-constr-expr then expr -- sequence construction (before)
    if expr then additional-constr-expr else additional-constr-expr
    let function-or-value-defn in additional-constr-expr
    additional-constr-init-expr

additional-constr-init-expr :=
    '{' class-inherits-decl field-initializers '}' -- explicit construction
    new type expr -- delegated construction

member-defn :=
    attributes~opt static~opt member access~opt method-or-prop-defn -- concrete member
    attributes~opt abstract member~opt access~opt member-sig -- abstract member
    attributes~opt override access~opt method-or-prop-defn -- override member
    attributes~opt default access~opt method-or-prop-defn -- override member
    attributes~opt static~opt val mutable~opt access~opt ident : type -- value member
    additional-constr-defn -- additional constructor

method-or-prop-defn :=
    ident.~opt function-defn -- method definition
    ident.~opt value-defn -- property definition
    ident.~opt ident with function-or-value-defns -- property definition via get/set methods
    member ident = exp – - auto-implemented property definition
    member ident = exp with get – - auto-implemented property definition
    member ident = exp with set – - auto-implemented property definition
    member ident = exp with get,set – - auto-implemented property definition
    member ident = exp with set,get – - auto-implemented property definition

member-sig :=
    ident typar-defns~opt : curried-sig -- method or property signature
    ident typar-defns~opt : curried-sig with get -- property signature
    ident typar-defns~opt : curried-sig with set -- property signature
    ident typar-defns~opt : curried-sig with get,set -- property signature
    ident typar-defns~opt : curried-sig with set,get -- property signature

curried-sig :=
    args-spec - > ... - > args-spec - > type

uncurried-sig :=
    args-spec - > type

args-spec :=
    arg-spec * ... * arg-spec

arg-spec :=
    attributes~opt arg-name-spec~opt type

arg-name-spec :=
    ?~opt ident :

interface-spec :=
    interface type
```

For example:

```fsharp
type int = System.Int32
type Color = Red | Green | Blue
type Map<'T> = { entries: 'T[] }
```

Type definitions can be declared in:

- Module definitions
- Namespace declaration groups

F# supports the following kinds of type definitions:

- Type abbreviations ([§](type-definitions.md#type-abbreviations))
- Record type definitions ([§](type-definitions.md#record-type-definitions))
- Union type definitions ([§](type-definitions.md#union-type-definitions))
- Class type definitions ([§](type-definitions.md#class-type-definitions))
- Interface type definitions ([§](type-definitions.md#interface-type-definitions))
- Struct type definitions ([§](type-definitions.md#struct-type-definitions))
- Enum type definitions ([§](type-definitions.md#enum-type-definitions))
- Delegate type definitions ([§](type-definitions.md#delegate-type-definitions))
- Exception type definitions ([§](type-definitions.md#exception-definitions))
- Type extension definitions ([§](type-definitions.md#type-extensions))
- Measure type definitions ([§](units-of-measure.md#measure-definitions))

With the exception of type abbreviations and type extension definitions, type definitions define
fresh, named types that are distinct from other types.

A _type definition group_ defines several type definitions or extensions simultaneously:

```fsgrammar
type ... and ...
```

For example:

```fsharp
type RowVector(entries: seq<int>) =
    let entries = Seq.toArray entries
    member x.Length = entries.Length
    member x.Permute = ColumnVector(entries)
and ColumnVector(entries: seq<int>) =
    let entries = Seq.toArray entries
    member x.Length = entries.Length
    member x.Permute = RowVector(entries)
```

A type definition group can include any type definitions except for exception type definitions and
module definitions.

Most forms of type definitions may contain both _static_ elements and _instance_ elements. Static
elements are accessed by using the type definition. Within a `static` definition, only the `static`
elements are in scope. Most forms of type definitions may contain _members_ ([§](type-definitions.md#members)).

Custom attributes may be placed immediately before a type definition group, in which case they
apply to the first type definition, or immediately before the name of the type definition:

```fsharp
[<Obsolete>] type X1() = class end

type [<Obsolete>] X2() = class end
and [<Obsolete>] Y2() = class end
```

## Type Definition Group Checking and Elaboration

F# checks type definition groups by determining the basic shape of the definitions and then filling in
the details. In overview, a type definition group is checked as follows:

1. For each type definition:
    - Determine the generic arguments, accessibility and kind of the type definition
    - Determine whether the type definition supports equality and/or comparison
    - Elaborate the explicit constraints for the generic parameters.
2. For each type definition:
    - Establish type abbreviations
    - Determine the base types and implemented interfaces of each new type definition
    - Detect any cyclic abbreviations
    - Verify the consistency of types in fields, union cases, and base types.
3. For each type definition:
    - Determine the union cases, fields, and abstract members ([§](type-definitions.md#abstract-members-and-interface-implementations)) of each new type
definition.
    - Check the union cases, fields, and abstract members themselves, as described in the
corresponding sections of this chapter.
4. For each member, add items that represent the members to the environment as a recursive
group.
5. Check the members, function, and value definitions in order and apply incremental
generalization.

In the context in which type definitions are checked, the type definition itself is in scope, as are all
members and other accessible functionality of the type. This context enables recursive references to
the accessible static content of a type. It also enables recursive references to the accessible
properties of any object that has the same type as the type definition or a related type.

In more detail, given an initial environment `env`, a type definition group is checked as described in
the following paragraphs.

First, check the individual type definitions. For each type definition:

1. Determine the number, names, and sorts of generic arguments of the type definition.
    - For each generic argument, if a `Measure` attribute is present, mark the generic argument as a
       measure parameter. The generic arguments are initially inference parameters, and
       additional constraints may be inferred for these parameters.
    - For each type definition `T` , the subsequent steps use an environment `envT` that is produced
       by adding the type definitions themselves and the generic arguments for `T` to `env`.
2. Determine the accessibility of the type definition.
3. Determine and check the basic kind of the type definition, using _Type Kind Inference_ if necessary
    ([§](type-definitions.md#type-kind-inference)).
4. Mark the type definition as a measure type definition if a `Measure` attribute is present.

5. If the type definition is generic, infer whether the type definition supports equality and/or
comparison.

6. Elaborate and add the explicit constraints for the generic parameters of the type definition, and
    then generalize the generic parameters. Inference of additional constraints is not permitted.
7. If the type definition is a type abbreviation, elaborate and establish the type being abbreviated.
8. Check and elaborate any base types and implemented interfaces.
9. If the type definition is a type abbreviation, check that the type abbreviation is not cyclic.
10. Check whether the type definition has a single, zero-argument constructor, and hence forms a
    type that satisfies the default constructor constraint.
11. Recheck the following to ensure that constraints are consist:
    - The type being abbreviated, if any.
    - The explicit constraints for any generic parameters, if any.
    - The types and constraints occurring in the base types and implemented interfaces, if any.
12. Determine the union cases, fields, and abstract members, if any, of the type definition. Check
    and elaborate the types that the union cases, fields, and abstract members include.
13. Make additional checks as defined elsewhere in this chapter. For example, check that the
    `AbstractClass` attribute does not appear on a union type.
14. For each type definition that is a struct, class, or interface, check that the inheritance graph and
    the struct-inclusion graph are not cyclic. This check ensures that a struct does not contain itself
    and that a class or interface does not inherit from itself. This check includes the following steps:

    a) Create a graph with one node for each type definition.

    b) Close the graph under edges.

    - (T, base-type-definition)
    - (T, interface-type-definition)
    - (T 1 , T 2 ) where T 1 is a struct and T 2 is a type that would store a value of type T 1 <...> for
        some instantiation. Here “X storing Y” means that X is Y or is a struct type with an
        instance field that stores Y.

    c) Check for cycles.

    The special case of a struct `S<typars>` storing a static field of type `S<typars>` is allowed.
15. Collectively add the elaborated member items that represent the members for all new type
    definitions to the environment as a recursive group ([§](type-definitions.md#members)), excluding interface implementation
    members.

16. If the type definition has a primary constructor, create a member item to represent the primary
    constructor.

After these steps are complete for each type definition, check the members. For each member:

1. If the member is in a generic type, create a copy of the type parameters for the generic type and
    add the copy to the environment for that member.
2. If the member has explicit type parameters, elaborate these type parameters and any explicit
    constraints.
3. If the member is an override, default, or interface implementation member, apply dispatch-slot
    inference.
4. If the member has syntactic parameters, assign an initial type to the elaborated member item
    based on the patterns that specify arguments for the members.
5. If the member is an instance member, assign a type to the instance variable.

Finally, check the function, value, and member definitions of each new type definition in order as a
recursive group.

## Type Kind Inference

A type that is specified in one of the following ways has an anonymous type kind:

- By using `begin` and `end` on the right-hand side of the = token.
- In lightweight syntax, with an implicit `begin`/`end`.

F# infers the kind of an anonymous type by applying the following rules, in order:

1. If the type has a `Class` attribute, `Interface` attribute, or `Struct` attribute, this attribute identifies
    the kind of the type.
2. If the type has any concrete elements, the type is a class. Concrete elements are primary
    constructors, additional object constructors, function definitions, value definitions, non-abstract
    members, and any `inherit` declarations that have arguments.
3. Otherwise, the type is an interface type.

For example:

```fsharp
// This is implicitly an interface
type IName =
    abstract Name : string

// This is implicitly a class, because it has a constructor
type ConstantName(n:string) =
    member x.Name = n

// This is implicitly a class, because it has a constructor
type AbstractName(n:string) =
    abstract Name : string
    default x.Name = "<no-name>"
```

If a type is not an anonymous type, any use of the `Class` attribute, `Interface` attribute, or `Struct`
attribute must match the `class`/`end`, `interface`/`end`, and `struct`/`end` tokens, if such tokens are
present. These attributes cannot be used with other kinds of type definitions such as type
abbreviations, record, union, or enum types.

## Type Abbreviations

Type abbreviations define new names for other types. For example:

```fsharp
type PairOfInt = int * int
```

Type abbreviations are expanded and erased during compilation and do not appear in the
elaborated form of F# declarations, nor can they be referred to or accessed at runtime.

The process of repeatedly eliminating type abbreviations in favor of their equivalent types must not
result in an infinite type derivation. For example, the following are not valid type definitions:

```fsharp
type X = option<X>

type Identity<'T> = 'T
and Y = Identity<Y>
```

The constraints on a type abbreviation must satisfy any constraints that the abbreviated type
requires.

For example, assuming the following declarations:

```fsharp
type IA =
    abstract AbstractMember : int -> int

type IB =
    abstract AbstractMember : int -> int

type C<'T when 'T :> IB>() =
    static member StaticMember(x : 'a) = x.AbstractMember(1)
```

the following is permitted:

```fsharp
type D<'T when 'T :> IB> = C<'T>
```

whereas the following is not permitted:

```fsharp
type E<'T> = C<'T> // invalid: missing constraint
```

Type abbreviations can define additional constraints, so the following is permitted:

```fsharp
type F<'T when 'T :> IA and 'T :> IB> = C<'T>
```

The right side of a type abbreviation must use all the declared type variables that appear on the left
side. For this purpose, the order of type variables that are used on the right-hand side of a type
definition is determined by their left-to-right occurrence in the type.

For example, the following is not a valid type abbreviation.

```fsharp
type Drop<'T,'U> = 'T * 'T // invalid: dropped type variable
```

> Note : This restriction simplifies the process of guaranteeing a stable and consistent
compilation to generic CLI code.

Flexible type constraints # _type_ may not be used on the right side of a type abbreviation, because
they expand to a type variable that has not been named in the type arguments of the type
abbreviation. For example, the following type is disallowed:

```fsharp
type BadType = #Exception -> int // disallowed
```

Type abbreviations may be declared `internal` or `private`.

> Note: Private type abbreviations are still, for all purposes, considered equivalent to the
abbreviated types.

## Record Type Definitions

A _record type definition_ introduces a type in which all the inputs that are used to construct a value
are accessible as properties on values of the type. For example:

```fsharp
type R1 =
    { x : int;
    y : int }
    member this.Sum = this.x + this.y
```

In this example, the integers x and y can be accessed as properties on values of type R1.

Record fields may be marked mutable. For example:

```fsharp
type R2 =
    { mutable x : int;
      mutable y : int }
    member this.Move(dx,dy) =
        this.x <- this.x + dx
        this.y <- this.y + dy
```

The `mutable` attribute on `x` and `y` makes the assignments valid.

Record types are implicitly sealed and may not be given the `Sealed` attribute. Record types may not
be given the `AbstractClass` attribute.

Record types are implicitly marked serializable unless the `AutoSerializable(false)` attribute is used.

### Members in Record Types

Record types may declare members ([§](type-definitions.md#members)), overrides, and interface implementations. Like all types
with overrides and interface implementations, they are subject to _Dispatch Slot Checking_ ([§](inference-procedures.md#dispatch-slot-checking)).

### Name Resolution and Record Field Labels

For a record type, the record field labels `field1` ... `field`_ are added to the _FieldLabels_ table of the
current name resolution environment unless the record type has the `RequireQualifiedAccess`
attribute.

Record field labels in the _FieldLabels_ table play a special role in _Name Resolution for Members_
([§](inference-procedures.md#name-resolution)): an expression’s type may be inferred from a record label. For example:

```fsharp
type R = { dx : int; dy: int }
let f x = x.dx // x is inferred to have type R
```

In this example, the lookup `.dx` is resolved to be a field lookup.

### Structural Hashing, Equality, and Comparison for Record Types

Record types implicitly implement the following interfaces and dispatch slots unless they are
explicitly implemented as part of the definition of the record type:

```fsharp
interface System.Collections.IStructuralEquatable
interface System.Collections.IStructuralComparable
interface System.IComparable
override GetHashCode : unit -> int
override Equals : obj -> bool
```

The implicit implementations of these interfaces and overrides are described in [§](type-definitions.md#equality-hashing-and-comparison).

### With/End in Record Type Definitions

Record type definitions can include `with`/`end` tokens, as the following shows:

```fsharp
type R1 =
    { x : int;
      y : int }
    with
        member this.Sum = this.x + this.y
    end
```

The `with`/`end` tokens can be omitted if the type-defn-elements vertically align with the `{` in the
`record-fields`. The semicolon (`;`) tokens can be omitted if the next `record-field` vertically aligns
with the previous `record-field`.

### CLIMutable Attributes

Adding the `CLIMutable` attribute to a record type causes it to be compiled to a CLI representation as
a plain-old CLR object (POCO) with a default constructor along with property getters and setters.
Adding the default constructor and mutable properties makes objects of the record type usable with
.NET tools and frameworks such as database queries, serialization frameworks, and data models in
XAML programming.

For example, an F# immutable record cannot be serialized because it does not have a constructor.
However, if you attach the CLIMutable attribute as in the following example, the XmlSerializer is
enable to serialize or deserialize this record type:

```fsharp
[<CLIMutable>]
type R1 = { x : string; y : int }
```

## Union Type Definitions

A _union type definition_ is a type definition that includes one or more _union cases_. For example:

```fsharp
type Message =
    | Result of string
    | Request of int * string
    member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm
```

Union case names must begin with an uppercase letter, which is defined to mean any character for
which the CLI library function `System.Char.IsUpper` returns `true` and `System.Char.IsLower` returns
`false`.

The union cases `Case1` ... `CaseN` have module scope and are added to the _ExprItems_ and _PatItems_
tables in the name resolution environment. This means that their unqualified names can be used to
form both expressions and patterns, unless the record type has the `RequireQualifiedAccess`
attribute.

Parentheses are significant in union definitions. Thus, the following two definitions differ:

```fsharp
type CType = C of int * int
type CType = C of (int * int)
```

The lack of parentheses in the first example indicates that the union case takes two arguments. The
parentheses in the second example indicate that the union case takes one argument that is a first-
class tuple value.

Union fields may optionally be named within each case of a union type. For example:

```fsharp
type Shape =
    | Rectangle of width: float * length: float
    | Circle of radius: float
    | Prism of width: float * float * height: float
```

The names are referenced when pattern matching on union values of this type. When using pattern
matching with multiple fields, semicolons are used to delimit the named fields, e.g. `Prism(width=w; height=h).`

The following declaration defines a type abbreviation if the named type `A` exists in the name
resolution environment. Otherwise it defines a union type.

```fsharp
type OneChoice = A
```

To disambiguate this case and declare an explicit union type, use the following:

```fsharp
type OneChoice =
    | A
```

Union types are implicitly marked serializable unless the `AutoSerializable(false)` attribute is used.

### Members in Union Types

Union types may declare members ([§](type-definitions.md#members)), overrides, and interface implementations. As with all
types that declare overrides and interface implementations, they are subject to _Dispatch Slot
Checking_ ([§](inference-procedures.md#dispatch-slot-checking)).

### Structural Hashing, Equality, and Comparison for Union Types

Union types implicitly implement the following interfaces and dispatch slots unless they are explicitly
implemented as part of the definition of the union type:

```fsharp
interface System.Collections.IStructuralEquatable
interface System.Collections.IStructuralComparable
interface System.IComparable
override GetHashCode : unit -> int
override Equals : obj -> bool
```

The implicit implementations of these interfaces and overrides are described in [§](type-definitions.md#equality-hashing-and-comparison).

### With/End in Union Type Definitions

Union type definitions can include `with`/`end` tokens, as the following shows:

```fsharp
type R1 =
    { x : int;
      y : int }
    with
        member this.Sum = this.x + this.y
    end
```

The `with`/`end` tokens can be omitted if the type-defn-elements vertically align with the `{` in the
record-fields. The semicolon (`;`) tokens can be omitted if the next _record-field_ vertically aligns
with the previous _record-field_.

For union types, the `with`/`end` tokens can be omitted if the type-defn-elements vertically alignwith
the first `|` in the union-type-cases. However, `with`/`end` must be present if the `|` tokens align with the
`type` token. For example:

```fsharp
/// Note: this layout is permitted
type Message =
    | Result of string
    | Request of int * string
    member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm

/// Note: this layout is not permitted
type Message =
| Result of string
| Request of int * string
member x.Name = match x with Result(nm) -> nm | Request(_,nm) -> nm
```

### Compiled Form of Union Types for Use from Other CLI Languages

A compiled union type `U` has:

- One CLI static getter property `U.C` for each null union case `C`. This property gets a singleton
    object that represents each such case.
- One CLI nested type `U.C` for each non-null union case `C`. This type has instance properties `Item1`,
    `Item2` ... for each field of the union case, or a single instance property `Item` if there is only one
    field. However, a compiled union type that has only one case does not have a nested type.
    Instead, the union type itself plays the role of the case type.
- One CLI static method `U.NewC` for each non-null union case `C`. This method constructs an object
    for that case.
- One CLI instance property `U.IsC` for each case `C`. This property returns `true` or `false` for the case.
- One CLI instance property `U.Tag` for each case `C`. This property fetches or computes an integer
    tag corresponding to the case.
- If `U` has more than one case, it has one CLI nested type `U.Tags`. The `U.Tags` type contains one
    integer literal for each case, in increasing order starting from zero.
- A compiled union type has the methods that are required to implement its auto-generated
    interfaces, in addition to any user-defined properties or methods.

These methods and properties may not be used directly from F#. However, these types have user-
facing `List.Empty`, `List.Cons`, `Option.None`, and `Option.Some` properties and/or methods.

A compiled union type may not be used as a base type in another CLI language, because it has at
least one assembly-private constructor and no public constructors.

## Class Type Definitions

A _class type definition_ encapsulates values that are constructed by using one or more object
constructors. Class types have the form:

```fsgrammar
type type-name pat~opt as-defn~opt =
    class
        class-inherits-decl~opt
        class-function-or-value-defns~opt
        type-defn-elements
    end
```

The `class`/`end` tokens can be omitted, in which case _Type Kind Inference_ ([§](type-definitions.md#type-kind-inference)) is used to determine
the kind of the type.

In F#, class types are implicitly marked serializable unless the `AutoSerializable(false)` attribute is
present.

### Primary Constructors in Classes

An _object constructor_ represents a way of initializing an object. Object constructors can create values
of the type and can partially initialize an object from a subclass. A class can have an optional _primary
constructor_ and zero or more _additional object constructors_.

If a type definition has a pattern immediately after the `type-name` and any accessibility annotation,
then it has a _primary constructor_. For example, the following type has a primary constructor:

```fsharp
type Vector2D(dx : float, dy : float) =
    let length = sqrt(dx*x + dy*dy)
    member v.Length = length
    member v.DX = dx
    member v.DY = dy
```

Class definitions that have a primary constructor may contain function and value definitions,
including those that use `let rec`.

The pattern for a primary constructor must have zero or more patterns of the following form:

```fsgrammar
( simple-pat , ..., simple-pat )
```

Each `simple-pat` has this form:

```fsgrammar
simple-pat :=
    | ident
    | simple-pat : type
```

Specifically, nested patterns may not be used in the primary constructor arguments. For example,
the following is not permitted because the primary constructor arguments contain a nested tuple
pattern:

```fsharp
type TwoVectors((px, py), (qx, qy)) =
    member v.Length = sqrt((qx-px)*(qx-px) + (qy-py)*(qy-py))
```

Instead, one or more value definitions should be used to accomplish the same effect:

```fsharp
type TwoVectors(pv, qv) =
    let (px, py) = pv
    let (qx, qy) = qv
    member v.Length = sqrt((qx-px)*(qx-px) + (qy-py)*(qy-py))
```

When a primary constructor is evaluated, the inheritance and function and value definitions are
evaluated in order.

#### Object References in Primary Constructors

For types that have a primary constructor, the name of the object parameter can be bound and used
in the non-static function, value, and member definitions of the type definition as follows:

```fsharp
type X(a:int) as x =
    let mutable currentA = a
    let mutable currentB = 0
    do x.B <- x.A + 3
    member self.GetResult()= currentA + currentB
    member self.A with get() = currentA and set v = currentA <- v
    member self.B with get() = currentB and set v = currentB <- v
```

During construction, no member on the type may be called before the last value or function
definition in the type has completed; such a call results in an `InvalidOperationException`. For
example, the following code raises this exception:

```fsharp
type C() as self =
    let f = (fun (x:C) -> x.F())
    let y = f self
    do printfn "construct"
    member this.F() = printfn "hi, y = %A" y

let r = new C() // raises InvalidOperationException
```

The exception is raised because an attempt may be made to access the value of the field `y` before
initialization is complete.

#### Inheritance Declarations in Primary Constructors

An `inherit` declaration specifies that the type being defined is an extension of an existing type. Such
declarations have the following form:

```fsgrammar
class-inherits-decl := inherit type expr~opt
```

For example:

```fsharp
type MyDerived(...) =
    inherit MyBase(...)
```

If a class definition does not contain an `inherit` declaration, the class inherits `fromSystem.Object` by
default.

The `inherit` declaration for a type must have arguments if and only if the type has a primary
constructor.

Unlike [§](type-definitions.md#object-references-in-primary-constructors), members of a base type can be accessed during construction of the derived class.
For example, the following code does not raise an exception:

```fsharp
type B() =
    member this.G() = printfn "hello "

type C() as self =
    inherit B()
    let f = (fun (x:C) -> x.G())
    let y = f self
    do printfn "construct"
    member this.F() = printfn "hi, y = %A" y

let r = new C() // does not raise InvalidOperationException
```

#### Instance Function and Value Definitions in Primary Constructors

Classes that have primary constructors may include function definitions, value definitions, and “do”
statements. The following rules apply to these definitions:

- Each definition may be marked `static` (see [§](type-definitions.md#static-function-and-value-definitions-in-primary-constructors)). If the definition is not marked `static`, it is
    called an instance definition.

- The functions and values defined by instance definitions are lexically scoped (and thus implicitly
    private) to the object being defined.
- Each value definition may optionally be marked `mutable`.
- A group of function and value definitions may optionally be marked `rec`.
- Function and value definitions are generalized.
- Value definitions that declared in classes are represented in compiled code as follows:
  - If a value definition is not mutable, and is not used in any function or member, then the
       value is represented as a local value in the object constructor.
  - If a value definition is mutable, or used in any function or member, then the value is
       represented as an instance field in the corresponding CLI type.
- Function definitions are represented in compiled code as private members of the corresponding
    CLI type.
    For example, consider this type:

    ```fsharp
       type C(x:int,y:int) =
           let z = x + y
           let f w = x + w
           member this.Z = z
           member this.Add(w) = f w
    ```

The input `y` is used only during construction, and no field is stored for it. Likewise the function `f`
is represented as a member rather than a field that is a function value.

A value definition is considered a function definition if its immediate right-hand-side is an
anonymous function, as in this example:

```fsharp
let f = (fun w -> x + w)
```

Function and value definitions may have attributes as follows:

- Value definitions represented as fields may have attributes that target fields.
- Value definitions represented as locals may have attributes that target fields, but these
    attributes will not be attached to any construct in the resulting CLI assembly.
- Function definitions represented as methods may have attributes that target methods.

For example:

```fsharp
type C(x:int) =
    [<System.Obsolete>]
    let unused = x
    member __.P = 1
```

In this example, no field is generated for `unused`, and no corresponding compiled CLI attribute is
generated.

#### Static Function and Value Definitions in Primary Constructors

Classes that have primary constructors may have function definitions, value definitions, and “do”
statements that are marked as static:

- The values that are defined by static function and value definitions are lexically scoped (and thus
    implicitly private) to the type being defined.
- Each value definition may optionally be marked `mutable`.
- A group of function and value definitions may optionally be marked `rec`.
- Static function and value definitions are generalized.
- Static function and value definitions are computed once per generic instantiation.
- Static function and value definitions are elaborated to a _static initializer_ associated with each
    generic instantiation of the generated class. Static initializers are executed on demand in the
    same way as static initializers for implementation files [§](program-structure-and-execution.md#program-execution).
- The compiled representation for static value definitions is as follows:
  - If the value is not used in any function or member then the value is represented as a local
       value in the CLI class initializer of the type.
  - If the value is used in any function or member, then the value is represented as a static field
       of the CLI class for the type.
- The compiled representation for a static function definition is a private static member of the
    corresponding CLI type.

Static function and value definitions may have attributes as follows:

- Static function and value definitions represented as fields may have attributes that target fields.
- Static function and value definitions represented as methods may have attributes that target
    methods.

For example:

```fsharp
type C<'T>() =
    static let mutable v = 2 + 2
    static do v <- 3

    member x.P = v
    static member P2 = v+v

printfn "check: %d = 3" (new C<int>()).P
printfn "check: %d = 3" (new C<int>()).P
printfn "check: %d = 3" (new C<string>()).P
printfn "check: %d = 6" (C<int>.P2)
printfn "check: %d = 6" (C<string>.P2)
```

In this example, the value `v` is represented as a static field in the CLI type for `C`. One instance of this
field exists for each generic instantiation of `C`. The output of the program is

```fsother
check: 3 = 3
check: 3 = 3
check: 3 = 3
check: 6 = 6
check: 6 = 6
```

### Members in Classes

Class types may declare members ([§](type-definitions.md#members)), overrides, and interface implementations. As with all
types that have overrides and interface implementations, such class types are subject to _Dispatch
Slot Checking_ ([§](inference-procedures.md#dispatch-slot-checking)).

### Additional Object Constructors in Classes

Although the use of primary object constructors is generally preferable, additional object
constructors may also be specified. Additional object constructors are required in two situations:

- To define classes that have more than one constructor.
- To specify explicit `val` fields without the `DefaultValue` attribute.

For example, the following statement adds a second constructor to a class that has a primary
constructor:

```fsharp
type PairOfIntegers(x:int,y:int) =
    new (x) = PairOfIntegers(x,x)
```

The next example declares a class without a primary constructor:

```fsharp
type PairOfStrings =
    val s1 : string
    val s2 : string
    new (s) = { s1 = s; s2 = s }
    new (s1,s2) = { s1 = s1; s2 = s2 }
```

If a primary constructor is present, additional object constructors must call another object
constructor in the same type, which may be another additional constructor or the primary
constructor.

If no primary constructor is present, additional constructors must initialize any `val` fields of the
object that do not have the `DefaultValue` attribute. They must also specify a call to a base class
constructor for any inherited class type. A call to a base class constructor is not required if the base
class is `System.Object`.

The use of additional object constructors and `val` fields is required if a class has multiple object
constructors that must each call different base class constructors. For example:

```fsharp
type BaseClass =
    val s1 : string
    new (s) = { s1 = s }
    new () = { s 1 = "default" }

type SubClass =
    inherit BaseClass
    val s2 : string
    new (s1,s2) = { inherit BaseClass(s1); s2 = s2 }
    new (s2) = { inherit BaseClass(); s2 = s2 }
```

To implement additional object constructors, F# uses a restricted subset of expressions that ensure
that the code generated for the constructor is valid according to the rules of object construction for
CLI objects. Note that precisely one `additional-constr-init-expr` occurs for each branch of a
construction expression.

For classes without a primary constructor, side effects can be performed after the initialization of
the fields of the object by using the `additional-constr-expr then stmt` form. For example:

```fsharp
type PairOfIntegers(x:int,y:int) =
    // This additional constructor has a side effect after initialization.
    new(x) =
        PairOfIntegers(x, x)
        then
            printfn "Initialized with only one integer"
```

The name of the object parameter can be bound within additional constructors. For example:

```fsharp
type X =
    val a : (unit -> string)
    val mutable b : string
    new() as x = { a = (fun () -> x.b); b = "b" }
```

A warning is given if x occurs syntactically in or before the `additional-constr-init-expr` of the
construction expression. If any member is called before the completion of execution of the
`additional-constr-init-expr` within the `additional-constr-expr` then an `InvalidOperationException`
is thrown.

### Additional Fields in Classes

Additional field declarations indicate that a value is stored in an object. They are generally used only
for classes without a primary constructor, or for mutable fields that use default initialization, and
typically occur only in generated code. For example:

```fsharp
type PairOfIntegers =
    val x : int
    val y : int
    new(x, y) = {x = x; y = y}
```

The following shows an additional field declaration as a static field in an explicit class type:

```fsharp
type TypeWithADefaultMutableBooleanField =
    [<DefaultValue>]
    static val mutable ready : bool
```

At runtime, such a field is initially assigned the zero value for its type ([§](expressions.md#zero-values)). For example:

```fsharp
type MyClass(name:string) =
    // Keep a global count. It is initially zero.
    [<DefaultValue>]
    static val mutable count : int

    // Increment the count each time an object is created
    do MyClass.count <- MyClass.count + 1

    static member NumCreatedObjects = MyClass.count

    member x.Name = name
```

A `val` specification in a type that has a primary constructor must be marked mutable and must have
the `DefaultValue` attribute. For example:

```fsharp
type X() =
    [<DefaultValue>]
    val mutable x : int
```

The `DefaultValue` attribute takes a check parameter, which indicates whether to ensure that the `val`
specification does not create unexpected `null` values. The default value for `check` is `true`. If this
parameter is `true`, the type of the field must permit default initialization ([§](types-and-type-constraints.md#nullness)). For example, the
following type is rejected:

```fsharp
type MyClass<'T>() =
    [<DefaultValue>]
    static val mutable uninitialized : 'T
```

The reason is that the type `'T` does not admit default initialization. However, in compiler-generated
and hand-optimized code it is sometimes essential to be able to emit fields that are completely
uninitialized. In this case, `DefaultValue(false)` can be used. For example:

```fsharp
type MyNullable<'T>() =
    [<DefaultValue>]
    static val mutable ready : bool

    [<DefaultValue(false)>]
    static val mutable uninitialized : 'T
```

## Interface Type Definitions

An _interface type definition_ represents a contract that an object may implement. Such a type
definition containsonly abstract members. For example:

```fsharp
type IPair<'T,'U> =
    interface
        abstract First: 'T
        abstract Second: 'U
    end

type IThinker<'Thought> =
    abstract Think: ('Thought -> unit) -> unit
    abstract StopThinking: (unit -> unit)
```

>Note: The `interface`/`end` tokens can be omitted when lightweight syntax is used, in
which case Type Kind Inference ([§](type-definitions.md#type-kind-inference)) is used to determine the kind of the type. The
presence of any non-abstract members or constructors means a type is not an interface
type.
<br>
By convention, interface type names start with `I`, as in `IEvent`. However, this convention
is not followed as strictly in F# as in other CLI languages.

Interface types may be arranged hierarchically by specifying inherit declarations. For example:

```fsharp
type IA =
    abstract One: int -> int

type IB =
    abstract Two: int -> int
type IC =
    inherit IA
    inherit IB
    abstract Three: int -> int
```

Each `inherit` declaration must itself be an interface type. Circular references are not allowed among
`inherit` declarations. F# uses the named types of the inherited interface types to determine
whether references are circular.

## Struct Type Definitions

A _struct type definition_ is a type definition whose instances are stored inline inside the stack frame or
object of which they are a part. The type is represented as a CLI struct type, also called a _value type_.
For example:

```fsharp
type Complex =
    struct
        val real: float;
        val imaginary: float
        member x.R = x.real
        member x.I = x.imaginary
    end
```

> Note: The `struct`/`end` tokens can be omitted when lightweight syntax is used, in which
case Type Kind Inference ([§](type-definitions.md#type-kind-inference)) is used to determine the kind of the type.

Because structs undergo type kind inference ([§](type-definitions.md#type-kind-inference)), the following is valid:

```fsharp
[<Struct>]
type Complex(r:float, i:float) =
    member x.R = r
    member x.I = i
```

Structs may have primary constructors:

```fsharp
[<Struct>]
type Complex(r : float, I : float) =
    member x.R = r
    member x.I = i
```

Structs that have primary constructors must accept at least one argument.

Structs may have additional constructors. For example:

```fsharp
[<Struct>]
type Complex(r : float, I : float) =
    member x.R = r
    member x.I = i
    new(r : float) = new Complex(r, 0.0)
```

The fields in a struct may be mutable only if the struct does not have a primary constructor. For
example:

```fsharp
[<Struct>]
type MutableComplex =
    val mutable real : float;
    val mutable imaginary : float
    member x.R = x.real
    member x.I = x.imaginary
    member x.Change(r, i) = x.real <- r; x.imaginary <- i
    new (r, i) = { real = r; imaginary = i }
```

Struct types may declare members, overrides, and interface implementations. As for all types that
declare overrides and interface implementations, struct types are subject to _Dispatch Slot Checking_
([§](inference-procedures.md#dispatch-slot-checking)).

Structs may not have `inherit` declarations.

Structs may not have “let” or “do” statements unless they are static. For example, the following is
not valid:

```fsharp
[<Struct>]
type BadStruct1 (def : int) =
    do System.Console.WriteLine("Structs cannot use 'do'!")
```

Structs may have static “let” or “do” statements. For example, the following is valid:

```fsharp
[<Struct>]
type GoodStruct1 (def : int) =
    static do System.Console.WriteLine("Structs can use 'static do'")
```

A struct type must be valid according to the CLI rules for structs; in particular, recursively
constructed structs are not permitted. For example, the following type definition is not permitted,
because the size of `BadStruct2` would be infinite:

```fsharp
[<Struct>]
type BadStruct2 =
    val data : float;
    val rest : BadStruct2
    new (data, rest) = { data = data; rest = rest }
```

Likewise, the implied size of the following struct would be infinite:

```fsharp
[<Struct>]
type BadStruct3 (data : float, rest : BadStruct3 ) =
    member s.Data = data
    member s.Rest = rest
```

If the types of all the fields in a struct type permit default initialization, the struct type has an _implicit
default constructor_, which initializes all the fields to the default value. For example, the `Complex` type
defined earlier in this section permits default initialization.

```fsharp
[<Struct>]
type Complex(r : float, I : float) =
    member x.R = r
    member x.I = i

    new(r : float) = new Complex(r, 0.0)

let zero = Complex()
```

> Note : The existence of the implicit default constructor for structs is not recorded in CLI
metadata and is an artifact of the CLI specification and implementation itself. A CLI
implementation permits default constructors for all struct types, although F# does not
permit their direct use for F# struct types unless all field types admit default
initialization. This is similar to the way that F# considers some types to have null as an
abnormal value.
<br>
Public struct types for use from other CLI languages should be designed with the
existence of the default zero-initializing constructor in mind.

[Record Type Defintions](#record-type-definitions) may also use the `[<Struct>]` attribute to change their representation from a reference type to a value type:

```fsharp
[<Struct>]
type Vector3 = { X: float; Y: float; Z: float }
```

Record structs have the following limitations:

- Unlike normal F# structs you cannot call the default constructor
- When marked with `[<CLIMutable>]` attribute, a default constructor is not created because it already exists implicitly

## Enum Type Definitions

Occasionally the need arises to represent a type that compiles as a CLI enumeration type. An _enum
type definition_ has values that are represented by integer constants and has a CLI enumeration as its
compiled form. Enum type definitions are declared by specifying integer constants in a format that is
syntactically similar to a union type definition. For example:

```fsharp
type Color =
    | Red = 0
    | Green = 1
    | Blue = 2

let rgb = (Color.Red, Color.Green, Color.Blue)

let show(colorScheme) =
    match colorScheme with
    | (Color.Red, Color.Green, Color.Blue) -> printfn "RGB in use"
    | _ -> printfn "Unknown color scheme in use"
```

The example defines the enum type `Color`, which has the values `Red`, `Green`, and `Blue`, mapped to
the constants `0`, `1`, and `2` respectively. The values are accessed by their qualified names: `Color.Red`,
`Color.Green`, and `Color.Blue`.

Each case must be given a constant value of the same type. The constant values dictate the
_underlying type_ of the enum, and must be one of the following types:

- `sbyte`, `int16`, `int32`, `int64` , `byte`, `uint16`, `uint32`, `uint64`, `char`

The declaration of an enumeration type in an implementation file has the following effects on the
typing environment:

- Brings a named type into scope.
- Adds the named type to the inferred signature of the containing namespace or module.

Enum types coerce to `System.Enum` and satisfy the `enum<underlying-type>` constraint for their
underlying type.

Each enum type declaration is implicitly annotated with the `RequiresQualifiedAccess` attribute and
does not add the tags of the enumeration to the name environment.

```fsharp
type Color =
    | Red = 0
    | Green = 1
    | Blue = 2

let red = Red // not accepted, must use Color.Red
```

Unlike unions, enumeration types are fundamentally “incomplete,” because CLI enumerations can
be converted to and from their underlying primitive type representation. For example, a `Color` value
that is not in the above enumeration can be generated by using the `enum` function from the F#
library:

```fsharp
let unknownColor : Color = enum<Color>(7)
```

This statement adds the value named `unknownColor`, equal to the constant `7`, to the `Color`
enumeration.

## Delegate Type Definitions

Occasionally the need arises to represent a type that compiles as a CLI delegate type. A _delegate
type definition_ has as its values functions that are represented as CLI delegate values. A delegate
type definition is declared by using the `delegate` keyword with a member signature. For example:

```fsharp
type Handler<'T> = delegate of obj * 'T -> unit
```

Delegates are often used when using Platform Invoke (P/Invoke) to interface with CLI libraries, as in
the following example:

```fsharp
type ControlEventHandler = delegate of int -> bool

[<DllImport("kernel32.dll")>]
extern void SetConsoleCtrlHandler(ControlEventHandler callback, bool add)
```

## Exception Definitions

An _exception definition_ defines a new way of constructing values of type `exn` (a type abbreviation for
`System.Exception`). Exception definitions have the form:

```fsgrammar
exception ident of type1 * ... * typen
```

An exception definition has the following effect:

- The identifier `ident` can be used to generate values of type `exn`.
- The identifier `ident` can be used to pattern match on values of type `exn`.
- The definition generates a type with name `ident` that derives from `exn`.

For example:

```fsharp
exception Error of int * string
raise (Error (3, "well that didn't work did it"))

try
    raise (Error (3, "well that didn't work did it"))
with
    | Error(sev, msg) -> printfn "severity = %d, message = %s" sev msg
```

The type that corresponds to the exception definition can be used as a type in F# code. For example:

```fsharp
let exn = Error (3, "well that didn't work did it")
let checkException() =
    if (exn :? Error) then printfn "It is of type Error"
    if (exn.GetType() = typeof<Error>) then printfn "Yes, it really is of type Error"
```

Exception abbreviations may abbreviate existing exception constructors. For example:

```fsharp
exception ThatWentBadlyWrong of string * int
exception ThatWentWrongBadly = ThatWentBadlyWrong

let checkForBadDay() =
    if System.DateTime.Today.DayOfWeek = System.DayOfWeek.Monday then
        raise (ThatWentWrongBadly("yes indeed",123))
```

Exception values may also be generated by defining and using classes that extend `System.Exception`.

## Type Extensions

A _type extension_ associates additional members with an existing type. For example, the following
associates the additional member `IsLong` with the existing type `System.String`:

```fsharp
type System.String with
    member x.IsLong = (x.Length > 1000)
```

Type extensions may be applied to any accessible type definition except those defined by type
abbreviations. For example, to add an extension method to a list type, use `'a List` because `'a list`
is a type abbreviation of `'a List`. For example:

```fsharp
type 'a List with
    member x.GetOrDefault(n) =
        if x.Length > n then x.[n]
        else Unchecked.defaultof<'a>
let intlst = [1; 2; 3]
intlst.GetOrDefault(1) //2
intlst.GetOrDefault(4) //0
```

For an array type, backtick marks can be used to define an extension method to the array type:

```fsharp
type 'a ``[]`` with
    member x.GetOrDefault(n) =
        if x.Length > n then x.[n]
        else Unchecked.defaultof<'a>
let arrlist = [| 1; 2; 3 |]
arrlist.GetOrDefault(1) //2
arrlist.GetOrDefault(4) //0
```

A type can have any number of extensions.

If the type extension is in the same module or namespace declaration group as the original type
definition, it is called an _intrinsic extension_. Members that are defined in intrinsic extensions follow
the same name resolution and other language rules as members that are defined as part of the
original type definition.

If the type extension is not intrinsic, it must be in a module, and it is called an _extension member_.
Opening a module that contains an extension member extends the name resolution of the dot
syntax for the extended type. That is, extension members are accessible only if the module that
contains the extension is open.

Name resolution for members that are defined in type extensions behaves as follows:

- In method application resolution (see [§](inference-procedures.md#method-application-resolution)), regular members (that is, members that are part of
    the original definition of a type, plus intrinsic extensions) are preferred to extension members.
- Extension members that are in scope and have the correct name are included in the group of
    members considered for method application resolution (see [§](inference-procedures.md#method-application-resolution)).
- An intrinsic member is always preferred to an extension member. If an extension member has
    the same name and type signature as a member in the original type definition or an inherited
    member, then it will be inaccessible.

The following illustrates the definition of one intrinsic and one extension member for the same type:

```fsharp
namespace Numbers
type Complex(r : float, i : float) =
    member x.R = r
    member x.I = i

// intrinsic extension
type Complex with
    static member Create(a, b) = new Complex (a, b)
    member x.RealPart = x.R
    member x.ImaginaryPart = x.I

namespace Numbers

module ComplexExtensions =

    // extension member
    type Numbers.Complex with
        member x.Magnitude = ...
        member x.Phase = ...
```

Extensions may define both instance members and static members.

Extensions are checked as follows:

- Checking applies to the member definitions in an extension together with the members and
    other definitions in the group of type definitions of which the extension is a part.
- Two intrinsic extensions may not contain conflicting members because intrinsic extensions are
    considered part of the definition of the type.
- Extensions may not define fields, interfaces, abstract slots, inherit declarations, or dispatch slot
    (interface and override) implementations.
- Extension members must be in modules.
- Extension members are compiled as CLI static members with encoded names.
  - The elaborated form of an application of a static extension member `C.M(arg1, ..., argn)` is a call
       to this static member with arguments `arg1, ..., argn`.
  - The elaborated form of an application of an instance extension member `obj.M(arg1, ..., argn)`
       is an invocation of the static instance member where the object parameter is supplied as the
       first argument to the extension member followed by arguments `arg1 ... argn`.

### Imported CLI C# Extensions Members

The CLI C# language defines an “extension member,” which commonly occurs in CLI libraries, along
with some other CLI languages. C# limits extension members to instance methods.

C#-defined extension members are made available to F# code in environments where the C#-
authored assembly is referenced and an `open` declaration of the corresponding namespace is in
effect.

The encoding of compiled names for F# extension members is not compatible with C# encodings of
C# extension members. However, for instance extension methods, the naming can be made
compatible. For example:

```fsharp
open System.Runtime.CompilerServices

[<Extension>]
module EnumerableExtensions =
    [<CompiledName("OutputAll"); Extension>]
    type System.Collections.Generic.IEnumerable<'T> with
        member x.OutputAll (this:seq<'T>) =
            for x in this do
                System.Console.WriteLine (box x)
```

C#-style extension members may also be declared directly in F#. When combined with the “inline”
feature of F#, this allows the definition of generic, constrained extension members that are not
otherwise definable in C# or F#.

```fsharp
[<Extension>]
type ExtraCSharpStyleExtensionMethodsInFSharp () =
    [<Extension>]
    static member inline Sum(xs: seq<'T>) = Seq.sum xs
```

Such an extension member can be used as follows:

```fsharp
let listOfIntegers = [ 1 .. 100 ]
let listOfBigIntegers = [ 1I .. 100I ]
listOfIntegers.Sum()
listOfBigIntegers.Sum()
```

## Members

Member definitions describe functions that are associated with type definitions and/or values of
particular types. Member definitions can be used in type definitions. Members can be classified as
follows:

- Property members
- Method members

A _static member_ is prefixed by `static` and is associated with the type, rather than with any particular
object. Here are some examples of static members:

```fsharp
type MyClass() =
    static let mutable adjustableStaticValue = "3"
    static let staticArray = [| "A"; "B" |]
    static let staticArray2 = [|[| "A"; "B" |]; [| "A"; "B" |] |]

    static member StaticMethod(y:int) = 3 + 4 + y

    static member StaticProperty = 3 + staticArray.Length

    static member StaticProperty2
        with get() = 3 + staticArray.Length

    static member MutableStaticProperty
        with get() = adjustableStaticValue
        and set(v:string) = adjustableStaticValue <- v

    static member StaticIndexer
        with get(idx) = staticArray.[idx]

    static member StaticIndexer2
        with get(idx1,idx2) = staticArray2.[idx1].[idx2]

    static member MutableStaticIndexer
        with get (idx1) = staticArray.[idx1]
        and set (idx1) (v:string) = staticArray.[idx1] <- v
```

An _instance member_ is a member without `static`. Here are some examples of instance members:

```fsharp
type MyClass() =
    let mutable adjustableInstanceValue = "3"
    let instanceArray = [| "A"; "B" |]
    let instanceArray2 = [| [| "A"; "B" |]; [| "A"; "B" |] |]

    member x.InstanceMethod(y:int) = 3 + y + instanceArray.Length

    member x.InstanceProperty = 3 + instanceArray.Length

    member x.InstanceProperty2
        with get () = 3 + instanceArray.Length

    member x.InstanceIndexer
        with get (idx) = instanceArray.[idx]

    member x.InstanceIndexer2
        with get (idx1,idx2) = instanceArray2.[idx1].[idx2]

    member x.MutableInstanceProperty
        with get () = adjustableInstanceValue
        and set (v:string) = adjustableInstanceValue <- v

    member x.MutableInstanceIndexer
        with get (idx1) = instanceArray.[idx1]
        and set (idx1) (v:string) = instanceArray.[idx1] <- v
```

Members from a set of mutually recursive type definitions are checked as a single mutually recursive
group. As with collections of recursive functions, recursive calls to potentially-generic methods may
result in inconsistent type constraints:

```fsharp
type Test() =
    static member Id x = x
    member t.M1 (x: int) = Test.Id(x)
    member t.M2 (x: string) = Test.Id(x) // error, x has type 'string' not 'int'
```

A target method that has a full type annotation is eligible for early generalization ([§](inference-procedures.md#generalization)).

```fsharp
type Test() =
    static member Id<'T> (x:'T) : 'T = x
    member t.M1 (x: int) = Test.Id(x)
    member t.M2 (x: string) = Test.Id(x)
```

### Property Members

A _property member_ is a `method-or-prop-defn` in one of the following forms:

```fsgrammar
static~opt member ident.~opt ident = expr
static~opt member ident.~opt ident with get pat = expr
static~opt member ident.~opt ident with set pat~opt pat = expr
static~opt member ident.~opt ident with get pat = expr and set pat~opt pat = expr
static~opt member ident.~opt ident with set pat~opt pat = expr and get pat = expr
```

A property member in the form

```fsgrammar
static~opt member ident.~opt ident with get pat1 = expr1 and set pat2a pat2b~opt = expr2
```

is equivalent to two property members of the form:

```fsgrammar
static~opt member ident.~opt ident with get pat1 = expr1
static~opt member ident.~opt ident with set pat2a pat2b~opt = expr2
```

Furthermore, the following two members are equivalent:

```fsgrammar
static~opt member ident.~opt ident = expr
static~opt member ident.~opt ident with get() = expr
```

These two are also equivalent:

```fsgrammar
static~opt member ident.~opt ident with set pat = expr
static~opt member ident.~opt ident with set() pat = expr
```

Thus, property members may be reduced to the following two forms:

```fsgrammar
static~opt member ident.~opt ident with get patidx = expr
static~opt member ident.~opt ident with set patidx pat = expr
```

The `ident.~opt` must be present if and only if the property member is an instance member. When
evaluated, the identifier `ident` is bound to the “this” or “self” object parameter that is associated
with the object within the expression `expr`.

A property member is an _indexer property_ if `patidx` is not the unit pattern `()`. Indexer properties
called `Item` are special in the sense that they are accessible via the `.[]` notation. An `Item` property
that takes one argument is accessed by using `x.[i]`; with two arguments by `x.[i,j]`, and so on.
Setter properties must return type `unit`.

> Note : As of F# 3. 1 , the special `.[]` notation for `Item` properties is available only for
instance members. A static indexer property cannot be accessible by using the `.[]`
notation.

Property members may be declared `abstract`. If a property has both a getter and a setter, then both
must be abstract or neither must be abstract.

Each property member has an implied property type. The property type is the type of the value that
the getter property returns or the setter property accepts. If a property member has both a getter
and a setter, and neither is an indexer property, the signatures of both the getter and the setter
must imply the same property type.

Static and instance property members are evaluated every time the member is invoked. For
example, in the following, the body of the member is evaluated each time `C.Time` is evaluated:

```fsharp
type C () =
    static member Time = System.DateTime.Now
```

Note that a static property member may also be written with an explicit `get` method:

```fsharp
static member ComputerName
    with get() = System.Environment.GetEnvironmentVariable("COMPUTERNAME")
```

Property members that have the same name may not appear in the same type definition even if
their signatures are different. For example:

```fsharp
type C () =
    static member P = false // error: Duplicate property.
    member this.P = true
```

However, methods that have the same name can be overloaded when their signatures are different.

### Auto-implemented Properties

Properties can be declared in two ways: either explicitly specified with the underlying value or
automatically generated by the compiler. The compiler creates a backing field automatically if all of
the following are true for the declaration:

- The declaration uses the `member val` keywords.
- The declaration omits the self-identifier.
- The declaration includes an expression to initialize the property.

To create a mutable property, include `with get`, `with set`,or both:

```fsgrammar
static~opt member val access~opt ident : ty~opt = expr
static~opt member val access~opt ident : ty~opt = expr with get
static~opt member val access~opt ident : ty~opt = expr with set
static~opt member val access~opt ident : ty~opt = expr with get, set
```

Automatically implemented properties are part of the initialization of a type, so they must be
included before any other member definitions, in the same way as let bindings and do bindings in a
type definition. The expression that initializes an automatically implemented property is evaluated
only at initialization, and not every time the property is accessed. This behavior is different from the
behavior of an explicitly implemented property.

For example, the following class type includes two automatically implemented properties. `Property1`
is read-only and is initialized to the argument provided to the primary constructor and `Property2` is a
settable property that is initialized to an empty string:

```fsharp
type D (x:int) =
    member val Property1 = x
    member val Property2 = "" with get, set
```

Auto-implemented properties can also be used to implement default or override properties:

```fsharp
type MyBase () =
    abstract Property : string with get, set
    default val Property = “default” with get, set

type MyDerived() =
    inherit MyBase()
    override val Property = "derived" with get, set
```

The following example shows how to use an auto-implemented property to implement an interface:

```fsharp
type MyInterface () =
    abstract Property : string with get, set

type MyImplementation () =
    interface MyInterface with
        member val Property = "implemented" with get, set
```

### Method Members

A _method member_ is of the form:

```fsgrammar
static~opt member ident.~opt ident pat1 ... patn = expr
```

The `ident.~opt` can be present if and only if the property member is an instance member. In this case,
the identifier `ident` corresponds to the “this” (or “self”) variable associated with the object on which
the member is being invoked.

Arity analysis ([§](inference-procedures.md#arity-inference)) applies to method members. This is because F# members must compile to CLI
methods, which accept only a single fixed collection of arguments.

### Curried Method Members

Methods that take multiple arguments may be written in iterated (“curried”) form. For example:

```fsharp
static member StaticMethod2 s1 s2 =
    sprintf "In StaticMethod(%s,%s)" s1 s2
```

The rules of arity analysis ([§](inference-procedures.md#arity-inference)) determine the compiled form of these members.

The following limitations apply to curried method members:

- Additional argument groups may not include optional or byref parameters.
- When the member is called, additional argument groups may not use named
    arguments([§](type-definitions.md#named-arguments-to-method-members)).
- Curried members may not be overloaded.

The compiled representation of a curried method member is a .NET method in which the arguments
are concatenated into a single argument group.

> Note : It is recommended that curried argument members do not appear in the public
API of an F# assembly that is designed for use from other .NET languages. Information
about the currying order is not visible to these languages.

### Named Arguments to Method Members

Calls to methods—but not to let-bound functions or function values—may use named arguments.
For example:

```fsharp
System.Console.WriteLine(format = "Hello {0}", arg0 = "World")
System.Console.WriteLine("Hello {0}", arg0 = "World")
System.Console.WriteLine(arg0 = "World", format = "Hello {0}")
```

The argument names that are associated with a method declaration are derived from the names
that appear in the first pattern of a member definition, or from the names used in the signature for a
method member. For example:

```fsharp
type C() =
    member x.Swap(first, second) = (second, first)
let c = C()
c.Swap(first = 1,second = 2) // result is '(2,1)'
c.Swap(second = 1,first = 2) // result is '(1,2)'
```

Named arguments may be used only with the arguments that correspond to the arity of the
member. That is, because members have an arity only up to the first set of tupled arguments, named
arguments may not be used with subsequent curried arguments of the member.

The resolution of calls that use named arguments is specified in _Method Application Resolution_ (see
[§](inference-procedures.md#method-application-resolution)). The rules in that section describe how resolution matches a named argument with either a
formal parameter of the same name or a “settable” return property of the same name. For example,
the following code resolves the named argument to a settable property:

```fsharp
System.Windows.Forms.Form(Text = "Hello World")
```

If an ambiguity exists, assigning the named argument is assigned to a formal parameter rather than
to a settable return property.

The _Method Application Resolution_ ([§](inference-procedures.md#method-application-resolution)) rules ensure that:

- Named arguments must appear after all other arguments, including optional arguments that
    are matched by position.

After named arguments have been assigned, the remaining required arguments are called the
_required unnamed arguments_. The required unnamed arguments must precede the named
arguments in the argument list. The _n_ unnamed arguments are matched to the first _n_ formal
parameters; the subsequent named arguments must include only the remaining formal parameters.
In addition, the arguments must appear in the correct sequence.

For example, the following code is invalid:

```fsharp
// error: unnamed args after named
System.Console.WriteLine(arg0 = "World", "Hello {0}")
```

Similarly, the following code is invalid:

```fsharp
type Foo() =
    static member M (arg1, arg2, arg3) = 1
// error: arg1, arg3 not a prefix of the argument list
Foo.M(1, 2, arg2 = 3)
```

The following code is valid:

```fsharp
type Foo() =
    static member M (arg1, arg2, arg3) = 1

Foo.M (1, 2, arg 3 = 3)
```

The names of arguments to members may be listed in member signatures. For example, in a
signature file:

```fsharp
type C =
    static member ThreeArgs : arg1:int * arg2:int * arg3:int -> int
    abstract TwoArgs : arg1:int * arg2:int -> int
```

### Optional Arguments to Method Members

Method members—but not functions definitions—may have optional arguments. F# supports
two forms of optional arguments: F#-style optional arguments and CLI-compatible optional arguments.

CLI-compatible optional arguments are handled on the **caller side**. When a method call omits
an optional argument, the compiler reads the default value from the method's metadata and
explicitly passes that value. This contrasts with F#-style optional arguments, which are
handled by the **callee**. With F#-style optional arguments, if an argument is omitted, the
compiler passes `None`, and the callee determines the default value to use.

From the caller's perspective both styles appear as optional arguments. However, their
underlying mechanism and primary use cases differ.

The compiled representation of members varies as additional optional arguments are added. The
addition of optional arguments to a member signature results in a compiled form that is not binary-
compatible with the previous compiled form.

#### F\#-Style Optional Arguments

F#-style optional arguments must appear at the end of the argument list. An optional argument
is marked with a `?` before its name in the method declaration. Inside the member, the argument
has the type `option<argType>`. The `option` type is used to represent a value that may or may
not exist.

The following example declares a method member that has two optional arguments:

```fsharp
let defaultArg x y = match x with None -> y | Some v -> v

type T() =
    static member OneNormalTwoOptional (arg1, ?arg2, ?arg3) =
        let arg2 = defaultArg arg2 3
        let arg3 = defaultArg arg3 10
        arg1 + arg2 + arg3
```

Optional arguments may be used in interface and abstract members. In a signature, optional
arguments appear as follows:

```fsharp
static member OneNormalTwoOptional : arg1:int * ?arg2:int * ?arg3:int -> int
```

Callers may specify values for optional arguments in the following ways:

- By name, such as `arg2 = 1`.
- By propagating an existing optional value by name, such as `?arg2=None` or `?arg2=Some(3)` or
    `?arg2=arg2`. This can be useful when building a method that passes optional arguments on to
    another method.
- By using normal, unnamed arguments that are matched by position.

For example:

```fsharp
T.OneNormalTwoOptional(3)
T.OneNormalTwoOptional(3, 2)
T.OneNormalTwoOptional(arg1 = 3)
T.OneNormalTwoOptional(arg1 = 3, arg2 = 1)
T.OneNormalTwoOptional(arg2 = 3, arg1 = 0)
T.OneNormalTwoOptional(arg2 = 3, arg1 = 0, arg3 = 11)
T.OneNormalTwoOptional(0, 3, 11)
T.OneNormalTwoOptional(0, 3, arg3 = 11)
T.OneNormalTwoOptional(arg1 = 3, ?arg2 = Some 1)
T.OneNormalTwoOptional(arg2 = 3, arg1 = 0, arg3 = 11)
T.OneNormalTwoOptional(?arg2 = Some 3, arg1 = 0, arg3 = 11)
T.OneNormalTwoOptional(0, 3, ?arg3 = Some 11)
```

The resolution of calls that use optional arguments is specified in _Method Application Resolution_ (see
[§](inference-procedures.md#method-application-resolution)).

Optional arguments may not be used in member constraints.

Marking an argument as optional is equivalent to adding the `FSharp.Core.OptionalArgument`
attribute ([§](special-attributes-and-types.md#custom-attributes-recognized-by-f)) to a required argument. This attribute is added implicitly for optional arguments.
Adding the `[<OptionalArgument>]` attribute to a parameter of type `'a option` in a virtual method
signature is equivalent to using the `(?x:'a)` syntax in a method definition. If the attribute is applied
to an argument of a method, it should also be applied to all subsequent arguments of the method.
Otherwise, it has no effect and callers must provide all of the arguments.

#### CLI-Compatible Optional Arguments

For interoperability with C# and other CLI languages, F# supports optional arguments with default values using
the `Optional` and `DefaultParameterValue` attributes. This mechanism is equivalent to defining an optional
argument in C# with a default value, such as `MyMethod(int i = 3)`. In F#, this would be written as:

```fsharp
open System.Runtime.InteropServices

type C() =
    static member MyMethod([<Optional; DefaultParameterValue(3)>] i: int) =
        i + 1
```

These attributes are typically used for C# and VB interop so that callers in those languages see an argument as optional.
They can also be from F# code in the same assembly and from separate assemblies.

CLI-compatible optional arguments are not passed as values of type `Option<_>`. If the optional
argument is present, its value is passed. If the optional argument is omitted, the default
value from the CLI metadata is supplied instead. The value `System.Reflection.Missing.Value`
is supplied for any CLI optional arguments of type `System.Object` that do not have a
corresponding CLI default value, and the default (zero-bit pattern) value is supplied for
other CLI optional arguments of other types that have no default value.

##### Allowable Default Values

The `DefaultParameterValue` attribute accepts the following types of values:

- **Primitive Types**: Constant values for `sbyte`, `byte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `float32`, `float`, and `string`.
- **Reference Types**: The only allowed default value is `null`.
- **Value Types**: The only allowed default value is the default value of the struct.

##### Usage and Considerations

The value provided to `DefaultParameterValue` must match the parameter's type. A mismatch will generate a compiler warning, and both the `Optional` and `DefaultParameterValue` attributes will be ignored.

For example, the following is not allowed:

```fsharp
type Class() =
  static member Wrong([<Optional; DefaultParameterValue("string")>] i:int) = ()```

This will be compiled as if it were written:
```fsharp
type Class() =
  static member Wrong(i:int) = ()
```

Note that the `null` value for reference types must be type-annotated, for instance: `[<Optional; DefaultParameterValue(null:obj)>] o:obj`.

It is possible to use these attributes in the following ways, though it is not standard practice:

- Specifying `Optional` without `DefaultParameterValue`: Callers can omit the argument, and a default value will be chosen by convention (the default constructor for primitive types and structs).
- Specifying `DefaultParameterValue` without `Optional`.
- Specifying `Optional; DefaultParameterValue` on any parameter, not necessarily the last one.

> Note : Imported CLI metadata may specify arguments as optional and may additionally
specify a default value for the argument. CLI optional arguments can propagate an existing optional
value by name; for example, `?ValueTitle = Some (...)`.
<br>For example, here is a fragment of a call to a Microsoft Excel COM automation API that
uses named and optional arguments.

```fsharp
    chartobject.Chart.ChartWizard(Source = range5,
                                  Gallery = XlChartType.xl3DColumn,
                                  PlotBy = XlRowCol.xlRows,
                                  HasLegend = true,
                                  Title = "Sample Chart",
                                  CategoryTitle = "Sample Category Type",
                                  ValueTitle = "Sample Value Type")
```

### Type-directed Conversions at Member Invocations

As described in _Method Application Resolution_ (see [§](inference-procedures.md#method-application-resolution)), three type-directed conversions are
applied at method invocations.

#### Conversion to Delegates

The first type-directed conversion converts anonymous function expressions and other function-
valued arguments to delegate types. Given:

- A formal parameter of delegate type `D`
- An actual argument `farg` of known type `ty1 -> ... -> tyn -> rty`
- Precisely `n` arguments to the `Invoke` method of delegate type `D`

Then:

- The parameter is interpreted as if it were written:

    ```fsgrammar
    new D (fun arg1 ... argn -> farg arg1 ... argn)
    ```

If the type of the formal parameter is a variable type, then F# uses the known inferred type of the
argument including instantiations to determine whether a formal parameter has delegate type. For
example, if an explicit type instantiation is given that instantiates a generic type parameter to a
delegate type, the following conversion can apply:

```fsharp
type GenericClass<'T>() =
    static member M(arg: 'T) = ()

GenericClass<System.Action>.M(fun () -> ()) // allowed
```

#### Conversion to Reference Cells

The second type-directed conversion enables an F# reference cell to be passed where a `byref<ty>` is
expected. Given:

- A formal out parameter of type `byref<ty>`
- An actual argument that is not a byref type

Then:

- The actual parameter is interpreted as if it had type `ref<ty>`.

For example:

```fsharp
type C() =
    static member M1(arg: System.Action) = ()
    static member M2(arg: byref<int>) = ()

C.M1(fun () -> ()) // allowed
let f = (fun () -> ()) in C.M1(f) // not allowed

let result = ref 0
C.M2(result) // allowed
```

> Note: These type-directed conversions are primarily for interoperability with existing
member-based .NET libraries and do not apply at invocations of functions defined in
modules or bound locally in expressions.

A value of type `ref<ty>` may be passed to a function that accepts a byref parameter. The interior
address of the heap-allocated cell that is associated with such a parameter is passed as the pointer
argument.

For example, consider the following C# code:

```csharp
public class C
{
    static public void IntegerOutParam(out int x) { x = 3; }
}
public class D
{
    virtual public void IntegerOutParam(out int x) { x = 3; }
}
```

This C# code can be called by the following F# code:

```fsharp
let res1 = ref 0
C.IntegerOutParam(res 1 )
// res1.contents now equals 3
```

Likewise, the abstract signature can be implemented as follows:

```fsharp
let x = {new D() with IntegerOutParam(res : byref<int>) = res <- 4}
let res2 = ref 0
x.IntegerOutParam(res2);
// res2.contents now equals 4
```

#### Conversion to Quotation Values

The third type-directed conversion enables an F# expression to be implicitly quoted at a member
call.

Conversion to a quotation value is driven by the ReflectedDefinition attribute to a method argument
of type FSharp.Quotations.Expr<_>:

```fsharp
static member Plot([<ReflectedDefinition>] values:Expr<int>) = (...)
```

The intention is that this gives an implicit quotation from X --> <@ X @> at the callsite. So for

```fsharp
Chart.Plot(f x + f y)
```

the caller becomes:

```fsharp
Chart.Plot(<@ f x + f y @>)
```

Additionally, the method can declare that it wants both the quotation and the evaluation of the
expression, by giving `true` as the `includeValue` argument of the `ReflectedDefinitionAttribute`.

```fsharp
static member Plot([<ReflectedDefinition(true)>] values:Expr<X>) = (...)
```

So for

```fsharp
Chart.Plot(f x + f y)
```

the caller becomes:

```fsharp
Chart.Plot(Expr.WithValue(f x + f y, <@ f x + f y @>))
```

and the quotation value `Q` received by `Chart.Plot` matches:

```fsharp
match Q with
| Expr.WithValue(v, ty) --> // v = f x + f y
| ...
```

> Note: Methods with ReflectedDefinition arguments may be used as first class values
(including pipelined uses), but it will not normally be useful to use them in this way. This
is because, in the above example, a first-class use of the method `Chart.Plot` is
considered shorthand for `(fun x -> C.Plot(x))` for some compiler-generated local
name `x`, which will become `(fun x -> C.Plot( <@ x @> ))`, so the implicit quotation
will just be a local value substitution. This means a pipelines use `expr |> C.Plot` will not
capture a full quotation for `expr`, but rather just its value.
<br>
The same applies to auto conversions for LINQ expressions: if you pipeline a method
accepting Expression arguments. This is an intrinsic cost of having an auto-quotation
meta-programming facility. All uses of auto-quotation need careful use API designers.
<br>
Auto-quotation of arguments only applies at method calls, and not function calls.
<br>
The conversion only applies if the called-argument-type is type Expr for some type T, and
if the caller-argument type is not of the form Expr for any U.
<br>
The caller-argument-type is determined as normal, with the addition that a caller
argument of the form <@ ... @> is always considered to have a type of the form Expr<>,
in the same way that caller arguments of the form (fun x -> ...) are always assumed to
have type of the form ``-> _`` (i.e. a function type)

#### Conversion to LINQ Expressions

The third type-directed conversion enables an F# expression to be implicitly converted to a LINQ
expression at a method call. Conversion is driven by an argument of type
`System.Linq.Expressions.Expression`.

```fsharp
static member Plot(values:Expression<Func<int,int>>) = (...)
```

This attribute results in an implicit quotation from X --> <@ X @> at the callsite and a call for a
helper function. So for

```fsharp
Chart.Plot(f x + f y)
```

the caller becomes:

```fsharp
Chart.Plot(FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.
QuotationToLambdaExpression <@ f x + f y @>)
```

### Overloading of Methods

Multiple methods that have the same name may appear in the same type definition or extension.
For example:

```fsharp
type MyForm() =
    inherit System.Windows.Forms.Form()

    member x.ChangeText(text: string) =
        x.Text <- text

    member x.ChangeText(text: string, reason: string) =
        x.Text <- text
        System.Windows.Forms.MessageBox.Show ("changing text due to " + reason)
```

Methods must be distinct based on their name and fully inferred types, after erasure of type
abbreviations and unit-of-measure annotations.

Methods that take curried arguments may not be overloaded.

### Naming Restrictions for Members

A member in a record type may not have the same name as a record field in that type.

A member may not have the same name and signature as another method in the type. This check
ignores return types except for members that are named `op_Implicit` or `op_Explicit`.

### Members Represented as Events

_Events_ are the CLI notion of a “listening point”—that is, a configurable object that holds a set of
callbacks, which can be triggered, often by some external action such as a mouse click or timer tick.

In F#, events are first-class values; that is, they are objects that mediate the addition and removal of
listeners from a backing list of listeners. The F# library supports the type
`FSharp.Control.IEvent<_,_>` and the module `FSharp.Control.Event`, which contains operations to
map, fold, create, and compose events. The type is defined as follows:

```fsharp
type IDelegateEvent<'del when 'del :> System.Delegate > =
    abstract AddHandler : 'del -> unit
    abstract RemoveHandler : 'del -> unit

type IEvent<'Del,'T when 'Del : delegate<'T,unit> and 'del :> System.Delegate > =
    abstract Add : event : ('T -> unit) -> unit
    inherit IDelegateEvent<'del>

type Handler<'T> = delegate of sender : obj * 'T -> unit

type IEvent<'T> = IEvent<Handler<'T>, 'T>
```

The following shows a sample use of events:

```fsharp
open System.Windows.Forms

type MyCanvas() =
    inherit Form()
    let event = new Event<PaintEventArgs>()
    member x.Redraw = event.Publish
    override x.OnPaint(args) = event.Trigger(args)

let form = new MyCanvas()
form.Redraw.Add(fun args -> printfn "OnRedraw")
form.Activate()
Application.Run(form)
```

Events from CLI languages are revealed as object properties of type
`FSharp.Control.IEvent<tydelegate, tyargs>`. The F# compiler determines the type arguments, which
are derived from the CLI delegate type that is associated with the event.

Event declarations are not built into the F# language, and `event` is not a keyword. However, property
members that are marked with the `CLIEvent` attribute and whose type coerces to
`FSharp.Control.IDelegateEvent<tydelegate>` are compiled to include extra CLI metadata and methods
that mark the property name as a CLI event. For example, in the following code, the
`ChannelChanged` property is currently compiled as a CLI event:

```fsharp
type ChannelChangedHandler = delegate of obj * int -> unit

type C() =
    let channelChanged = new Event<ChannelChangedHandler,_>()
    [<CLIEvent>]
    member self.ChannelChanged = channelChanged.Publish
```

Similarly, the following shows the definition and implementation of an abstract event:

```fsharp
type I =
    [<CLIEvent>]
    abstract ChannelChanged : IEvent<ChannelChanged,int>

type ImplI() =
    let channelChanged = new Event<ChannelChanged,_>()
    interface I with
        [<CLIEvent>]
        member self.ChannelChanged = channelChanged.Publish
```

### Members Represented as Static Members

Most members are represented as their corresponding CLI method or property. However, in certain
situations an instance member may be compiled as a static method. This happens when either of the
following is true:

- The type definition uses `null` as a representation by placing the
    `CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)` attribute on
    the type that declares the member.

- The member is an extension member.

Compilation of an instance member as a static method can affect the view of the type when seen
from other languages or from `System.Reflection`. A member that might otherwise have a static
representation can be reverted to an instance member representation by placing the attribute
`CompilationRepresentation(CompilationRepresentationFlags.Instance)` on the member.

For example, consider the following type:

```fsharp
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type option<'T> =
    | None
    | Some of 'T

    member x.IsNone = match x with None -> true | _ -> false
    member x.IsSome = match x with Some _ -> true | _ -> false

    [<CompilationRepresentation(CompilationRepresentationFlags.Instance)>]
    member x.Item =
    match x with
        | Some x -> x
        | None -> failwith "Option.Item"
```

The `IsNone` and `IsSome` properties are represented as CLI static methods. The `Item` property is
represented as an instance property.

## Abstract Members and Interface Implementations

Abstract member definitions and interface declarations in a type definition represent promises that
an object will provide an implementation for a corresponding contract.

### Abstract Members

An _abstract member definition_ in a type definition represents a promise that an object will provide
an implementation for a dispatch slot. For example:

```fsharp
type IX =
    abstract M : int -> int
```

The abstract member `M` indicates that an object of type `IX` will implement a displatch slot for a
member that returns an `int`.

A class definition may contain abstract member definitions, but the definition must be labeled with
the `AbstractClass` attribute:

```fsharp
[<AbstractClass>]
type X() =
    abstract M : int -> int
```

An abstract member definition has the form

```fsgrammar
abstract access~opt member-sig
```

where a member signature has one of the following forms

```fsgrammar
ident typar-defns~opt : curried-sig
ident typar-defns~opt : curried-sig with get
ident typar-defns~opt : curried-sig with set
ident typar-defns~opt : curried-sig with get, set
ident typar-defns~opt : curried-sig with set, get
```

and the curried signature has the form

```fsgrammar
args-spec1 -> ... -> args-specn -> type
```

If `n` ≥ 2, then `args-spec2 ... args-specn` must all be patterns without attribute or optional argument
specifications.

If `get` or `set` is specified, the abstract member is a _property member_. If both `get` and `set` are
specified, the abstract member is equivalent to two abstract members, one with `get` and one with
`set`.

### Members that Implement Abstract Members

An _implementation member_ has the form:

```fsgrammar
override ident. ident pat 1 ... patn = expr
default ident. ident pat 1 ... patn = expr
```

Implementation members implement dispatch slots. For example:

```fsharp
[<AbstractClass>]
type BaseClass() =
    abstract AbstractMethod : int -> int

type SubClass(x: int) =
    inherit BaseClass()
    override obj.AbstractMethod n = n + x

let v1 = BaseClass() // not allowed – BaseClass is abstract
let v2 = (SubClass(7) :> BaseClass)

v2.AbstractMethod 6 // evaluates to 13
```

In this example, `BaseClass()` declares the abstract slot `AbstractMethod` and the `SubClass` type
supplies an implementation member `obj.AbstractMethod`, which takes an argument `n` and returns
the sum of `n` and the argument that was passed in the instantiation of `SubClass`. The `v2` object
instantiates `SubClass` with the value `7`, so `v2.AbstractMethod 6` evaluates to `13`.

The combination of an abstract slot declaration and a default implementation of that slot create the
F# equivalent of a “virtual” method in some other languages—that is, an abstract member that is
guaranteed to have an implementation. For example:

```fsharp
type BaseClass() =
    abstract AbstractMethodWithDefaultImplementation : int -> int
    default obj.AbstractMethodWithDefaultImplementation n = n

type SubClass1(x: int) =
    inherit BaseClass()
    override obj.AbstractMethodWithDefaultImplementation n = n + x

type SubClass2() =
    inherit BaseClass()

let v1 = BaseClass() // allowed -- BaseClass contains a default implementation
let v2 = (SubClass1(7) :> BaseClass)
let v3 = (SubClass2() :> BaseClass)

v1.AbstractMethodWithDefaultImplementation 6 // evaluates to 6
v2.AbstractMethodWithDefaultImplementation 6 // evaluates to 13
v3.AbstractMethodWithDefaultImplementation 6 // evaluates to 6
```

Here, the `BaseClass` type contains a default implementation, so F# allows the instantiation of `v1`. The
instantiation of `v2` is the same as in the previous example. The instantiation of `v3` is similar to that of
`v1`, because `SubClass2` inherits directly from `BaseClass` and does not override the `default` method.

> Note: The keywords `override` and `default` are synonyms. However, it is recommended
that `default` be used only when the implementation is in the same class as the
corresponding abstract definition; `override` should be used in other cases. This records
the intended role of the member implementation.

Implementations may override methods from System.Object:

```fsharp
type BaseClass() =
    override obj.ToString() = "I'm an instance of BaseClass"

type SubClass(x: int) =
    inherit BaseClass()
    override obj.ToString() = "I'm an instance of SubClass"
```

In this example, `BaseClass` inherits from `System.Object` and overrides the `ToString` method from
that class. The `SubClass`, in turn, inherits from `BaseClass` and overrides its version of the `ToString`
method.

Implementations may include abstract property members:

```fsharp
[<AbstractClass>]
type BaseClass() =
    let mutable data1 = 0
    let mutable data2 = 0
    abstract AbstractProperty : int
    abstract AbstractSettableProperty : int with get, set

    abstract AbstractPropertyWithDefaultImplementation : int
    default obj.AbstractPropertyWithDefaultImplementation = 3

    abstract AbstractSettablePropertyWithDefaultImplementation : int with get, set
    default obj.AbstractSettablePropertyWithDefaultImplementation
        with get() = data2
        and set v = data2 <- v

type SubClass(x: int) =
    inherit BaseClass()
    let mutable data1b = 0
    let mutable data2b = 0
    override obj.AbstractProperty = 3 + x
    override obj.AbstractSettableProperty
        with get() = data1b + x
        and set v = data1b <- v - x
    override obj.AbstractPropertyWithDefaultImplementation = 6 + x
    override obj.AbstractSettablePropertyWithDefaultImplementation
        with get() = data2b + x
        and set v = data2b <- v - x
```

The same rules apply to both property members and method members. In the preceding example,
`BaseClass` includes abstract properties named `AbstractProperty`, `AbstractSettableProperty`,
`AbstractPropertyWithDefaultImplementation`, and
`AbstractSettablePropertyWithDefaultImplementation` and provides default implementations for the
latter two. `SubClass` provides implementations for `AbstractProperty` and `AbstractSettableProperty`,
and overrides the default implementations for `AbstractPropertyWithDefaultImplementation` and
`AbstractSettablePropertyWithDefaultImplementation`.

Implementation members may also implement CLI events ([§](type-definitions.md#members-represented-as-events)). In this case, the member
should be marked with the `CLIEvent` attribute. For example:

```fsharp
type ChannelChangedHandler = delegate of obj * int -> unit

[<AbstractClass>]
type BaseClass() =
    [<CLIEvent>]
    abstract ChannelChanged : IEvent<ChannelChangedHandler, int>

type SubClass() =
    inherit BaseClass()
    let mutable channel = 7
    let channelChanged = new Event<ChannelChangedHandler, int>()

    [<CLIEvent>]
    override self.ChannelChanged = channelChanged.Publish
    member self.Channel
        with get () = channel
        and set v = channel <- v; channelChanged.Trigger(self, channel)
```

`BaseClass` implements the CLI event `IEvent`, so the abstract member `ChannelChanged` is marked with
`[<CLIEvent>]` as described earlier in §8.13.10. SubClass provides an implementation of the abstract
member, so the [<CLIEvent>] attribute must also precede the `override` declaration in `SubClass`.

### Interface Implementations

An _interface implementation_ specifies how objects of a given type support a particular interface. An
interface in a type definition indicates that objects of the defined type support the interface. For
example:

```fsharp
type IIncrement =
    abstract M : int -> int

type IDecrement =
    abstract M : int -> int

type C() =
    interface IIncrement with
        member x.M(n) = n + 1
    interface IDecrement with
        member x.M(n) = n - 1
```

The first two definitions in the example are implementations of the interfaces `IIncrement` and
`IDecrement`. In the last definition,the type `C` supports these two interfaces.

No type may implement multiple different instantiations of a generic interface, either directly or
through inheritance. For example, the following is not permitted:

```fsharp
// This type definition is not permitted because it implements two instantiations
// of the same generic interface
type ClassThatTriesToImplemenTwoInstantiations() =
    interface System.IComparable<int> with
        member x.CompareTo(n : int) = 0
    interface System.IComparable<string> with
        member x.CompareTo(n : string) = 1
```

Each member of an interface implementation is checked as follows:

- The member must be an instance member definition.
- _Dispatch Slot Inference_ ([§](inference-procedures.md#dispatch-slot-inference)) is applied.
- The member is checked under the assumption that the “this” variable has the enclosing type.

In the following example, the value `x` has type `C`.

```fsharp
type C() =
    interface IIncrement with
        member x.M(n) = n + 1
    interface IDecrement with
        member x.M(n) = n - 1
```

All interface implementations are made explicit. In its first implementation, every interface must be
completely implemented, even in an abstract class. However, interface implementations may be
inherited from a base class. In particular, if a class `C` implements interface `I`, and a base class of `C`
implements interface `I`, then `C` is not required to implement all the methods of `I`; it can implement
all, some, or none of the methods instead. For example:

```fsharp
type I1 =
    abstract V1 : string
    abstract V2 : string

type I2 =
    inherit I1
    abstract V3 : string

type C1() =
    interface I1 with
        member this.V1 = "C1"
        member this.V2 = "C2"
// This is OK
type C2() =
    inherit C1()

// This is also OK; C3 implements I2 but not I1.
type C3() =
    inherit C1()
    interface I2 with
        member this.V3 = "C3"

// This is also OK; C4 implements one method in I1.
type C4() =
    inherit C1()
    interface I1 with
        member this.V2 = "C2b"
```

## Equality, Hashing, and Comparison

Functional programming in F# frequently involves the use of structural equality, structural hashing,
and structural comparison. For example, the following expression evaluates to `true`, because tuple
types support structural equality:

```fsharp
(1, 1 + 1) = (1, 2)
```

Likewise, these two function calls return identical values:

```fsharp
hash (1, 1 +1 )
hash (1,2)
```

Similarly, an ordering on constituent parts of a tuple induces an ordering on tuples themselves, so all
the following evaluate to `true`:

```fsharp
(1, 2) < (1, 3)
(1, 2) < (2, 3)
(1, 2) < (2, 1)
(1, 2) > (1, 0)
```

The same applies to lists, options, arrays, and user-defined record, union, and struct types whose
constituent field types permit structural equality, hashing, and comparison. For example, given:

```fsharp
type R = R of int * int
```

then all of the following also evaluate to `true`:

```fsharp
R (1, 1 + 1) = R (1, 2)

R (1, 3) <> R (1, 2)

hash (R (1, 1 + 1)) = hash (R (1, 2))

R (1, 2) < R (1, 3)
R (1, 2) < R (2, 3)
R (1, 2) < R (2, 1)
R (1, 2) > R (1, 0)
```

To facilitate this, by default, record, union, and struct type definitions—called _structural types_ —
implicitly include compiler-generated declarations for structural equality, hashing, and comparison.
These implicit declarations consist of the following for structural equality and hashing:

```fsharp
override x.GetHashCode() = ...
override x.Equals(y:obj) = ...
    interface System.Collections.IStructuralEquatable with
    member x.Equals(yobj: obj, comparer: System.Collections.IEqualityComparer) = ...
    member x.GetHashCode(comparer: System.IEqualityComparer) = ...
```

The following declarations enable structural comparison:

```fsharp
interface System.IComparable with
    member x.CompareTo(y:obj) = ...
interface System.Collections.IStructuralComparable with
    member x.CompareTo(yobj: obj, comparer: System.Collections.IComparer) = ...
```

For exception types, implicit declarations for structural equality and hashings are generated, but
declarations for structural comparison are not generated. Implicit declarations are never generated
for interface, delegate, class, or enum types. Enum types implicitly derive support for equality,
hashing, and comparison through their underlying representation as integers.

### Equality Attributes

Several attributes affect the equality behavior of types:

```fsharp
FSharp.Core.NoEquality
FSharp.Core.ReferenceEquality
FSharp.Core.StructuralEquality
FSharp.Core.CustomEquality
```

The following table lists the effects of each attribute on a type:

| Attrribute | Effect |
| --- | --- |
| `NoEquality` | ▪ No equality or hashing is generated for the type.<br>▪ The type does not satisfy the `ty : equality` constraint. |
| `ReferenceEquality` | ▪ No equality or hashing is generated for the type.<br> ▪ The defaults for `System.Object` will implicitly be used. |
| `StructuralEquality` | ▪ The type must be a structural type.<br>▪ All structural field types `ty` must satisfy `ty : equality`. |
| `CustomEquality` | ▪ The type must have an explicit implementation of `override Equals(obj: obj)` |
| None |▪ For a non-structural type, the default is `ReferenceEquality`.<br>▪ For a structural type:<br>The default is `NoEquality` if any structural field type `F` fails `F : equality`.<br>The default is `StructuralEquality` if all structural field types `F` satisfy `F : equality`. |

Equality inference also determines the _constraint dependencies_ of a generic structural type. That is:

- If a structural type has a generic parameter `'T` and `T : equality` is necessary to make the type
    default to `StructuralEquality`, then the `EqualityConditionalOn` constraint dependency is
    inferred for `'T`.

### Comparison Attributes

The comparison behavior of types can be affected by the following attributes:

```fsharp
FSharp.Core.NoComparison
FSharp.Core.StructuralComparison
FSharp.Core.CustomComparison
```

The following table lists the effects of each attribute on a type.

| Attribute | Effect |
| --- | --- |
| `NoComparison` | ▪ No comparisons are generated for the type.<br>▪ The type does not satisfy the `ty : comparison` constraint. |
| `StructuralComparison` | ▪ The type must be a structural type other than an exception type.<br>▪ All structural field types `ty` must satisfy `ty : comparison`.<br>▪ An exception type may not have the `StructuralComparison` attribute. |
| `CustomComparison` | ▪ The type must have an explicit implementation of one or both of the following:<br>`interface System.IComparable`<br>`interface System.Collections.IStructuralComparable`<br>▪ A structural type that has an explicit implementation of one or both of these contracts must specify the `CustomComparison` attribute. |
| None | ▪ For a non-structural or exception type, the default is `NoComparison`.<br>▪ For any other structural type:<br>The default is `NoComparison` if any structural field type `F` fails `F : comparison`.<br>The default is `StructuralComparison` if all structural field types `F` satisfy `F : comparison`. |

This check also determines the _constraint dependencies_ of a generic structural type. That is:

- If a structural type has a generic parameter `'T` and `T` : comparison is necessary to make the type
    default to `StructuralComparison`, then the `ComparisonConditionalOn` constraint dependency is
    inferred for `'T`.

For example:

```fsharp
[<StructuralEquality; StructuralComparison>]
type X = X of (int -> int)
```

results in the following message:

```fsother
The struct, record or union type 'X' has the 'StructuralEquality' attribute
but the component type '(int -> int)' does not satisfy the 'equality' constraint
```

For example, given

```fsharp
type R1 =
    { myData : int }
    static member Create() = { myData = 0 }

[<ReferenceEquality>]
type R2 =
    { mutable myState : int }
    static member Fresh() = { myState = 0 }

[<StructuralEquality; NoComparison >]
type R3 =
    { someType : System.Type }
    static member Make() = { someType = typeof<int> }
```

then the following expressions all evaluate to `true`:

```fsharp
R1.Create() = R1.Create()
not (R2.Fresh() = R2.Fresh())
R3.Make() = R3.Make()
```

Combinations of `equality` and `comparion` attributes are restricted. If any of the following attributes
are present, they may be used only in the following combinations:

- No attributes
- `[<NoComparison>]` on any type
- `[<NoEquality; NoComparison>]` on any type
- `[<CustomEquality; NoComparison>]` on a structural type
- `[<ReferenceEquality>]` on a non-struct structural type
- `[<ReferenceEquality; NoComparison>]` on a non-struct structural type
- `[<StructuralEquality; NoComparison>]` on a structural type
- `[<CustomEquality; CustomComparison>]` on a structural type
- `[<StructuralEquality; CustomComparison>]` on a structural type
- `[<StructuralEquality; StructuralComparison>]` on a structural type

### Behavior of the Generated Object.Equals Implementation

For a type definition `T`, the behavior of the generated `override x.Equals(y:obj) = ...`
implementation is as follows.

1. If the interface `System.IComparable` has an explicit implementation, then just call
    `System.IComparable.CompareTo`:

    ```fsharp
    override x.Equals(y : obj) =
        ((x :> System.IComparable).CompareTo(y) = 0)
    ```

2. Otherwise:
    - Convert the `y` argument to type `T`. If the conversion fails, return `false`.
    - Return `false` if `T` is a reference type and `y` is null.
    - If `T` is a struct or record type, invoke `FSharp.Core.Operators.(=)` on each corresponding pair
       of fields of `x` and `y` in declaration order. This method stops at the first `false` result and
       returns `false`.
    - If `T` is a union type, invoke `FSharp.Core.Operators.(=)` first on the index of the union cases
       for the two values, then on each corresponding field pair of `x` and `y` for the data carried by
       the union case. This method stops at the first `false` result and returns `false`.
    - If `T` is an exception type, invoke `FSharp.Core.Operators.(=)` on the index of the tags for the
       two values, then on each corresponding field pair for the data carried by the exception. This
       method stops at the first `false` result and returns `false`.

### Behavior of the Generated CompareTo Implementations

For a type `T`, the behavior of the generated `System.IComparable.CompareTo` implementation is as
follows:

- Convert the `y` argument to type `T`. If the conversion fails, raise the `InvalidCastException`.
- If `T` is a reference type and `y` is `null`, return `1`.
- If `T` is a struct or record type, invoke `FSharp.Core.Operators.compare` on each corresponding pair
    of fields of `x` and `y` in declaration order, and return the first non-zero result.
- If `T` is a union type, invoke `FSharp.Core.Operators.compare` first on the index of the union cases
    for the two values, and then on each corresponding field pair of `x` and `y` for the data carried by
    the union case. Return the first non-zero result.

The first few lines of this code can be written:

```fsharp
interface System.IComparable with
    member x.CompareTo(y:obj) =
        let y = (obj :?> T) in
            match obj with
            | null -> 1
            | _ -> ...
```

### Behavior of the Generated GetHashCode Implementations

For a type `T`, the generated `System.Object.GetHashCode()` override implements a combination hash
of the structural elements of a structural type.

### Behavior of Hash, =, and Compare

The generated equality, hashing, and comparison declarations that are described in sections [§](type-definitions.md#behavior-of-the-generated-objectequals-implementation),
[§](type-definitions.md#behavior-of-the-generated-compareto-implementations), and [§](type-definitions.md#behavior-of-the-generated-gethashcode-implementations) use the `hash`, `=` and `compare` functions from the F# library. The behavior of these
library functions is defined by the pseudocode later in this section. This code ensures:

- Ordinal comparison for strings
- Structural comparison for arrays
- Natural ordering for native integers (which do not support `System.IComparable`)

#### Pseudocode for FSharp.Core.Operators.compare

> Note: In practice, fast (but semantically equivalent) code is emitted for direct calls to
(=), compare, and hash for all base types, and faster paths are used for comparing most
arrays.

```fsharp
open System

/// Pseudo code for code implementation of generic comparison.
let rec compare x y =
    let xobj = box x
    let yobj = box y
    match xobj, yobj with
    | null, null -> 0
    | null, _ -> - 1
    | _, null -> 1

    // Use Ordinal comparison for strings
    | (:? string as x),(:? string as y) ->
        String.CompareOrdinal(x, y)

    // Special types not supporting IComparable
    | (:? Array as arr1), (:? Array as arr2) ->
        ... compare the arrays by rank, lengths and elements ...
    | (:? nativeint as x),(:? nativeint as y) ->
        ... compare the native integers x and y....
    | (:? unativeint as x),(:? unativeint as y) ->
        ... compare the unsigned integers x and y....

    // Check for IComparable
    | (:? IComparable as x),_ -> x.CompareTo(yobj)
    | _,(:? IComparable as yc) -> -(sign(yc.CompareTo(xobj)))

    // Otherwise raise a runtime error
    | _ -> raise (new ArgumentException(...))
```

#### Pseudo code for FSharp.Core.Operators.(=)

> Note: In practice, fast (but semantically equivalent) code is emitted for direct calls to
(=), compare, and hash for all base types, and faster paths are used for comparing most
arrays

```fsharp
open System
/// Pseudo code for core implementation of generic equality.
let rec (=) x y =
    let xobj = box x
    let yobj = box y
    match xobj,yobj with
    | null,null -> true
    | null,_ -> false
    | _,null -> false

    // Special types not supporting IComparable
    | (:? Array as arr1), (:? Array as arr2) ->
        ... compare the arrays by rank, lengths and elements ...

    // Ensure NaN semantics on recursive calls
    | (:? float as f1), (:? float as f2) ->
        ... IEEE equality on f1 and f2...
    | (:? float32 as f1), (:? float32 as f2) ->
        ... IEEE equality on f1 and f2...

    // Otherwise use Object.Equals. This is reference equality
    // for reference types unless an override is provided (implicitly
    // or explicitly).
    | _ -> xobj.Equals(yobj)
```
