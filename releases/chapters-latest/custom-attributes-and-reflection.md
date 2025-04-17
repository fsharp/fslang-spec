# 13. Custom Attributes and Reflection

CLI languages use metadata inspection and the `System.Reflection` libraries to make guarantees
about how compiled entities appear at runtime. They also allow entities to be attributed by static
data, and these attributes may be accessed and read by tools and running programs. This chapter
describes these mechanisms for F#.

Attributes are given by the following grammar:

```fsgrammar
attribute := attribute-target : opt object-construction

attribute-set := [< attribute ; ... ; attribute >]

attributes := attribute-set ... attribute-set

attribute-target :=
    assembly
    module
    return
    field
    property
    param
    type
    constructor
    event
```

## 13.1 Custom Attributes

CLI languages support the notion of _custom attributes_ which can be added to most declarations.
These are added to the corresponding elaborated and compiled forms of the constructs to which
they apply.

Custom attributes can be applied only to certain target language constructs according to the
`AttributeUsage` attribute, which is found on the attribute class itself. An error occurs if an attribute is
attached to a language construct that does not allow that attribute.

Custom attributes are not permitted on function or value definitions in expressions or computation
expressions. Attributes on parameters are given as follows:

```fsharp
let foo([<SomeAttribute>] a) = a + 5
```

If present, the arguments to a custom attribute must be _literal constant expressions_ , or arrays of the
same.

Custom attributes on return values are given as follows:

```fsharp
let foo a : [<SomeAttribute>] = a + 5
```

Custom attributes on primary constructors are given before the arguments and before any
accessibility annotation:

```fsharp
type Foo1 [<System.Obsolete("don't use me")>] () =
    member x.Bar() = 1

type Foo2 [<System.Obsolete("don't use me")>] private () =
    member x.Bar() = 1
```

Custom attributes are mapped to compiled CLI metadata as follows:

- Custom attributes map to the element that is specified by their target, if a target is given.
- A custom attribute on a type `type` is compiled to a custom attribute on the corresponding CLI
    type definition, whose `System.Type` object is returned by `typeof<type>`.
- By default, a custom attribute on a record field `F` for a type `T` is compiled to a custom attribute
    on the CLI property for the field that is named `F`, unless the target of the attribute is `field`, in
    which case it becomes a custom attribute on the underlying backing field for the CLI property
    that is named `_F`.
- A custom attribute on a union case `ABC` for a type `T` is compiled to a custom attribute on a static
    method on the CLI type definition `T`. This method is called:
  - `get_ABC` if the union case takes no arguments
  - `ABC` otherwise
- Custom attributes on arguments are propagated only for arguments of member definitions, and
    not for “let”-bound function definitions.
- Custom attributes on generic parameters are not propagated.

Custom attributes that appear immediately preceding “do” statements in modules anywhere in an
assembly are attached to one of the following:

- The `main` entry point of the program.
- The compiled module.
- The compiled assembly.

Custom attributes are attached to the main entry point if it is valid for them to be attached to a
method according to the `AttributeUsage` attribute that is found on the attribute class itself, and
likewise for the assembly. If it is valid for the attribute to be attached to either the main method or
the assembly. the main method takes precedence.

For example, the `STAThread` attribute should be placed immediately before a top-level `do`
statement.

```fsharp
let main() =
    let form = new System.Windows.Forms.Form()
    System.Windows.Forms.Application.Run(form)

[<STAThread>]
do main()
```

### 13.1.1 Custom Attributes and Signatures

During signature checking, custom attributes attached to items in F# signature files (`.fsi` files) are
combined with custom attributes on the corresponding element from the implementation file
according to the following algorithm:

- Start with lists `AImpl` and `ASig` containing the attributes in the implementation and signature, in
    declaration order.
- Check each attribute in `AImpl` against the available attributes in `ASig`.
- If `ASig` contains an attribute that is an exact match after evaluating attribute arguments, then
    ignore the attribute in the implementation, remove the attribute from `ASig` , and continue
    checking;
- If `ASig` contains an attribute that has the same attribute type but is not an exact match, then
    give a warning and ignore the attribute in the implementation;
- Otherwise, keep the attribute in the implementation.

The compiled element contains the compiled forms of the attributes from the signature and the
retained attributes from the implementation.

This means:

- When an implementation has an attribute `X("abc")` and the signature is missing the
    attribute, then no warning is given and the attribute appears in the compiled assembly.
- When a signature has an attribute `X("abc")` and the implementation is missing the attribute,
    then no warning is given, and the attribute appears in the compiled assembly.
- When an implementation has an attribute `X("abc")` and the signature has attribute
    `X("def")`, then a warning is given, and only `X("def")` appears in the compiled assembly.

## 13.2 Reflected Forms of Declaration Elements

The `typeof` and `typedefof` F# library operators return a `System.Type` object for an F# type definition.
According to typical implementations of the CLI execution environment, the `System.Type` object in
turn can be used to access further information about the compiled form of F# member declarations.
If this operation is supported in a particular implementation of F#, then the following rules describe
which declaration elements have corresponding `System.Reflection` objects:

- All member declarations are present as corresponding methods, properties or events.
- Private and internal members and types are included.
- Type abbreviations are not given corresponding `System.Type` definitions.

In addition:

- F# modules are compiled to provide a corresponding compiled CLI type declaration and
    `System.Type` object, although the `System.Type` object is not accessible by using the `typeof`
    operator.

However:

- Internal and private function and value definitions are not guaranteed to be given corresponding
    compiled CLI metadata definitions. They may be removed by optimization.
- Additional internal and private compiled type and member definitions may be present in the
    compiled CLI assembly as necessary for the correct implementation of F# programs.
- The `System.Reflection` operations return results that are consistent with the erasure of F# type
    abbreviations and F# unit-of-measure annotations.
- The definition of new units of measure results in corresponding compiled CLI type declarations
    with an associated `System.Type`.
