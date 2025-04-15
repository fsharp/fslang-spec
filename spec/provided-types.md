# Provided Types

Type providers are extensions provided to an F# compiler or interpreter which provide information
about types available in the environment for the F# code being analysed.

The compilation context is augmented with a set of `type provider instances`. A type provider instance
is interrogated for information through `type provider invocations` (TPI). Type provider invocations
are all executed at compile-time. The type provider instance is not required at runtime.

> Wherever an operation on a provided namespace, provided type definition or provided
member is mentioned in this section, it is assumed to be a compile-time type provider
invocation.<br><br>
The exact protocol used to implement type provider invocations and communicate
between an F# compiler/interpreter and type provider instances is implementation
dependent.<br><br>
As of this release of F#,<br><br>
    - a type provider is a .NET 4.x binary component referenced as an imported assembly<br>
      reference. The assembly should have a `TypeProviderAssemblyAttribute`, with at
      least one component marked with `TypeProviderAttribute`.<br>
    - a type provider instance is an object created for a component marked with
      `TypeProviderAttribute`.<br>
    - provided type definitions are `System.Type` objects returned by a type provider
      instance.<br>
    - provided methods are `System.Reflection.MethodInfo` objects returned by a type
      provider instance.<br>
    - provided constructors are `System.Reflection.ConstructorInfo` objects returned by
      a type provider instance.<br>
    - provided properties are `System.Reflection.PropertyInfo` objects returned by a type
      provider instance.<br>
    - provided events are `System.Reflection.EventInfo` objects returned by a type
      provider instance.<br>
    - provided literal fields are `System.Reflection.FieldInfo` objects returned by a type
      provider instance.<br>
    - provided parameters are `System.Reflection.ParameterInfo` objects returned by a
      type provider instance.<br>
    - provided static parameters are `System.Reflection.ParameterInfo` objects returned
      by a type provider instance.<br>
    - provided attributes are attribute value objects returned by a type provider instance.<br>

## Static Parameters

The syntax of types in F# is expanded to include static parameters, including named static
parameters:

```fsgrammar
type-arg =
    static-parameter
static-parameter =
    static-parameter-value
    id = static-parameter-value

static-parameter-value =
    const expr
    simple-constant-expression
```

References to provided types may include static parameters, e.g.

```fsharp
type SomeService = ODataService<"http://some.url.org/service">
```

Static parameters which are constant expressions, but not simple literal constants, may be specified
using the const keyword, e.g.

```fsharp
type SomeService = CsvFile<const (__SOURCE_DIRECTORY__ + "/a.csv")>
```

Parentheses are needed around any simple constants expressions after “const” that are not simple
literal constants, e.g.

```fsharp
type K = N.T< const (+1) >
```

During checking of a type `A<ype-args>`, where A is a provided type, the TPM `GetStaticParameters` is
invoked to determine the static parameters for the type A if any. If the static parameters exist and
are of the correct kinds, the TPM `ApplyStaticArguments` is invoked to apply the static arguments to
the provided type.

During checking of a method `M<type-args>`, where M is a provided method definition, the TPM
`GetStaticParametersForMethod` is invoked to determine the static parameters if any. If the static
parameters exist and are of the correct kinds, the TPM `ApplyStaticArgumentsForMethod` is invoked to
apply the static arguments to the provided method.

In both cases a static parameter value must be given for each non-optional static parameter.

### Mangling of Static Parameter Values

Static parameter values are encoded into the names used for types and methods within F#
metadata. The encoding scheme used is

```fsgrammar
encoding (A<arg1,...,argN>) =

  typeOrMethodName,ParamName1= encoding(arg1),..., ParamNameN=encoding(argN)

encoding(v) = "s"
```

where s is the result applying the F# `string` operator to v (using invariant numeric
formatting), and in the result `"` is replaced by `\"` and `\` by `\\`

## Provided Namespaces

Each type provider instance in the assembly context reports a collection of provided namespaces
though the `GetNamespaces` type provider method. Each provided namespace can in turn report
further namespaces through the `GetNestedNamespaces` type provider method.

## Provided Type Definitions

Each provided namespace reports provided type definitions though the `GetTypes` and
`ResolveTypeName` type provider methods. The type provider is obliged to ensure that these two
methods return consistent results.

Name resolution for unqualified identifiers may return provided type definitions if no other
resolution is available.

### Generated v. Erased Types

Each provided type definition may be `generated` or `erased`. In this case, the types and method calls
are removed entirely during compilation and replaced with other representations. When an erased
type is used, the compiler will replace it with the first concrete type in its inheritance chain as
returned by the TPM `type.BaseType`. The erasure of an erased interface type is `object`.

- If it has a type definition under a path `D.E.F` , and the .Assembly of that type is in a different
assembly A to the provider’s assembly, then that type definition is a `generated` type
definition. Otherwise, it is an erased type definition.

- Erased type definitions must return `TypeAttributes` with the `IsErased` flag set, value
`0x40000000` and given by the F# literal `TypeProviderTypeAttributes.IsErased`.

- When a provided type definition is generated, its reported assembly `A` is treated as an
injected assembly which is statically linked into the resulting assembly.

- Concrete type definitions (both provided and F#-authored) and object expressions may not
inherit from erased types

- Concrete type definitions (both provided and F#-authored) and object expressions may not
implement erased interfaces

- If an erased type definition reports an interface, its erasure must implement the erasure of
that interface. The interfaces reported by an erased type definition must be unique up to
erasure.

- Erased types may not be used as the target type of a runtime type test of runtime coercion.

- When determining uniqueness for F#-declared methods, uniqueness is determined after
erasure of both provided types and units of measure.

- The elaborated form of F# expressions is after erasure of provided types.

- Two generated type definitions are equivalent if and only if they have the same F# path and
name in the same assembly, once they are rooted according to their corresponding
generative type definition.

- Two erased type definitions are only equivalent if they are provided by the same provider,
using the same type name, with the same static arguments.

### Type References

The elements of provided type definitions may reference other provided type definitions, and types
from imported assemblies referenced in the compilation context. They may not reference type
defined in the F# code currently being compiled.

### Static Parameters

A provided type definition may report a set of static parameters. For such a definition, all other
provided contents are ignored.

A provided method definition may also report a set of static parameters. For such a definition, all
other provided contents are ignored.

Static parameters may be optional and/or named, indicated by the `Attributes` property of the static
parameter. For a given set of static parameters, no two static parameters may have the same name
and named static arguments must come after all other arguments.

### Kind

- Provided type definitions may be classes. This includes both erased and concrete types.
This corresponds to the `type.IsClass` property returning true for the provided type definition.

- Provided type definitions may be interfaces. This includes both erased and concrete types.
This corresponds to the `type.IsInterface` property returning true. Only one of `IsInterface`, `IsClass`, `IsStruct`, `IsEnum`, `IsDelegate`, `IsArray` may return true.

- Provided type definitions may be static classes. This includes both erased and concrete types.

- Provided type definitions may be sealed.

- Provided type definitions may not be arrays. This means the `type.IsArray` property must
always return false. Provided types used in return types and argument positions may be
array `symbol` types, see below.

- By default, provided type definitions which are reference types are considered to support
`null` literals.

A provided type definition may have the `AllowNullLiteralAttribute` with value `false` in
which case the type is considered to have null as an abnormal value.

### Inheritance

- Provided type definitions may report base types.
- Provided type definition may report interfaces.

### Members

- Provided type definitions may report methods.

    This corresponds to non-null results from the `type.GetMethod` and `type.GetMethods` of the
    provided type definition. The results returned by these methods must be consistent.

  - Provided methods may be static, instance and abstract
  - Provided methods may not be class constructors (.cctor). By .NET rules these would have to be private anyway.
  - Provided methods may be operators such as op_Addition.

- Provided type definitions may report properties.

    This corresponds to non-null results from the `type.GetProperty` and `type.GetProperties` of the
    provided type definition. The results returned by these methods must be consistent.

  - Provided properties may be static or instance
  - Provided properties may be indexers. This corresponds to reporting methods with
    name Item , or as identified by `DefaultMemberAttribute` non-null results from the
    `type.GetEvent` and `type.GetEvents` of the provided type definition. The results
    returned by these methods must be consistent. This includes 1D, 2D, 3D and 4D
    indexer access notation in F# (corresponding to different numbers of parameters to
    the indexer property).

- Provided type definitions may report constructors.

    This corresponds to non-null results from the type.GetConstructor and type.GetConstructors of
    the provided type definition. The results returned by these methods must be consistent.

- Provided type definitions may report events.

    This corresponds to non-null results from the type.GetEvent and type.GetEvents of the
    provided type definition. The results returned by these methods must be consistent.

- Provided type definitions may report nested types.

    This corresponds to non-null results from the type.GetNestedType and type.GetNestedTypes of
    the provided type definition. The results returned by these methods must be consistent.

  - The nested types of an erased type may be generated types in a generated
    assembly. The `type.DeclaringType` property of the nested type need not report the
    erased type.

- Provided type definitions may report literal (constant) fields.

    This corresponds to non-null results from the `type.GetField` and `type.GetFields` of the
    provided type definition, and is related to the fact that provided types may be
    enumerations. The results returned by these methods must be consistent.

- Provided type definitions may not report non-literal (i.e. non-const) fields

    This is a deliberate feature limitation, because in .NET, non-literal fields should not appear in
    public API surface area.

### Attributes

- Provided type definitions, properties, constructors, events and methods may report
attributes.

    This includes `ObsoleteAttribute` and `ParamArrayAttribute` attributes

### Accessibility

- All erased provided type definitions must be public

    However, concrete provided types are each in an assembly A that gets statically linked into
    the resulting F# component. These assemblies may contain private types and methods.
    These types are not directly “provided” types, since they are not returned to the compiler by
    the API, but they are part of the closure of the types that are being embedded.

### Elaborated Code

Elaborated uses of provided methods are erased to elaborated expressions returned by the TPM
`GetInvokerExpression`. In the current release of F#, replacement elaborated expressions are
specified via F# quotation values composed of quotations constructed with respect to the
referenced assemblies in the compilation context according to the following quotation library calls:

- Expr.NewArray
- Expr.NewObject
- Expr.WhileLoop
- Expr.NewDelegate
- Expr.ForIntegerRangeLoop
- Expr.Sequential
- Expr.TryWith
- Expr.TryFinally
- Expr.Lambda
- Expr.Call
- Expr.Constant
- Expr.DefaultValue
- Expr.NewTuple
- Expr.TupleGet
- Expr.TypeAs
- Expr.TypeTest
- Expr.Let
- Expr.VarSet
- Expr.IfThenElse
- Expr.Var

The type of the quotation expression returned by `GetInvokerExpression` must be an erased type. The
type provider is obliged to ensure that this type is equivalent to the erased type of the expression it
is replacing.

### Further Restrictions

- If a provided type definition reports a member with `ExtensionAttribute`, it is not treated as
an extension member

- Provided type and method definitions may not be generic

    This corresponds to
  - `GetGenericArguments` returning length 0
  - For type definitions, `IsGenericType` and `IsGenericTypeDefinition` returning false
  - For method definitions, `IsGenericMethod` and `IsGenericMethodDefinition` returning false
