# Units of Measure

F# supports static checking of _units of measure_. Units of measure, or _measures_ for short, are like types in that they can appear as parameters to other types and values (as in `float<kg>`, `vector<m/s>`, `add<m>`), can be represented by variables (as in `float<'U>`), and are checked for consistency by the type-checker.

However, measures differ from types in several important ways:

- Measures play no role at runtime; in fact, they are erased.
- Measures obey special rules of _equivalence_ , so that `N m` can be interchanged with `m N`.
- Measures are supported by special syntax.

The syntax of constants ([§4.3](basic-grammar-elements.md#constants)) is extended to support numeric constants with units of measure. The syntax of types is extended with measure type annotations.

```fsgrammar
measure-literal-atom :=
    long-ident                                  -- named measure e.g. kg
    ( measure-literal-simp )                    -- parenthesized measure, such as (N m)

measure-literal-power :=
    measure-literal-atom
    measure-literal-atom ^ int32                -- power of measure, such as m^3

measure-literal-seq :=
    measure-literal-power
    measure-literal-power measure-literal-seq

measure-literal-simp :=
    measure-literal-seq                         -- implicit product, such as m s^- 2
    measure-literal-simp * measure-literal-simp -- product, such as m * s^3
    measure-literal-simp / measure-literal-simp -- quotient, such as m/s^2
    / measure-literal-simp                      -- reciprocal, such as /s
    1                                           -- dimensionless

measure-literal :=
    _                                           -- anonymous measure
    measure-literal-simp                        -- simple measure, such as N m

const :=
    ...
    byte < measure-literal >                    -- 8 - bit unsigned integer constant
    uint16 < measure-literal >                  -- 16 - bit unsigned integer constant
    uint32 < measure-literal >                  -- 32 - bit unsigned integer constant
    uint64 < measure-literal >                  -- 64 - bit unsigned integer constant
    sbyte < measure-literal >                   -- 8 - bit integer constant
    int16 < measure-literal >                   -- 16 - bit integer constant
    int32 < measure-literal >                   -- 32 - bit integer constant
    int64 < measure-literal >                   -- 64 - bit integer constant
    ieee32 < measure-literal >                  -- single-precision float32 constant
    ieee64 < measure-literal >                  -- double-precision float constant
    decimal < measure-literal >                 -- decimal constant

measure-atom :=
    typar                                       -- variable measure, such as 'U
    long-ident                                  -- named measure, such as kg
    ( measure-simp )                            -- parenthesized measure, such as (N m)

measure-power :=
    measure-atom
    measure-atom ^ int32                        -- power of measure, such as m^3

measure-seq :=
    measure-power
    measure-power measure-seq

measure-simp :=
    measure-seq                                 -- implicit product, such as 'U 'V^3
    measure-simp * measure-simp                 -- product, such as 'U * 'V
    measure-simp / measure-simp                 -- quotient, such as 'U / 'V
    / measure-simp                              -- reciprocal, such as /'U
    1                                           -- dimensionless measure (no units)

measure :=
    _                                           -- anonymous measure
measure-simp                                    -- simple measure, such as 'U 'V
```

Measure definitions use the special `Measure` attribute on type definitions. Measure parameters, meanwhile, use a variation on the syntax of generic parameters (see [§9.5](units-of-measure.md#measure-parameter-definitions)) to parameterize types and members by units of measure. The primitive types `byte`, `uint16`, `uint`, `uint64`, `sbyte`, `int16`, `int`, `int64`, `float`, `float32`, `decimal`, `unativeint`, and `nativeint` have non-parameterized (dimensionless) and parameterized versions.

Here is a simple example:

```fsharp
[<Measure>] type m          // base measure: meters
[<Measure>] type s          // base measure: seconds
[<Measure>] type sqm = m^2  // derived measure: square meters

let areaOfTriangle (baseLength:float<m>, height:float<m>) : float<sqm> =
    baseLength*height/2.0

let distanceTravelled (speed:float<m/s>, time:float<s>) : float<m> = speed*time
```

As with ordinary types, F# can infer that functions are generic in their units. For example, consider the following function definitions:

```fsharp
let sqr (x:float<_>) = x * x

let sumOfSquares x y = sqr x + sqr y
```

The inferred types are:

```fsother
val sqr : float<'u> -> float<'u ^ 2>

val sumOfSquares : float<'u> -> float<'u> -> float<'u ^ 2>
```

Measures are type-like annotations such as `kg` or `m/s` or `m^2`. Their special syntax includes the use of `*` and `/` for product and quotient of measures, juxtaposition as shorthand for product, and `^` for integer powers.

## Measures

Measures are built from:

- _Atomic measures_ from long identifiers such as `SI.kg` or `FreedomUnits.feet`.
- _Product measures_ , which are written `measure measure` (juxtaposition) or `measure * measure`.
- _Quotient measures_ , which are written `measure / measure`.
- _Integer powers of measures_ , which are written `measure ^ int`.
- _Dimensionless measures_ , which are written `1`.
- _Variable measures_, which are written `'u` or `'U`. Variable measures can include anonymous measures `_`, which indicates that the compiler can infer the measure from the context.

Dimensionless measures indicate “without units,” but are rarely needed, because non-parameterized types such as `float` are aliases for the parameterized type with `1` as parameter, that is, `float = float<1>`.

The precedence of operations involving measure is similar to that for floating-point expressions:

- Products and quotients (`*` and `/`) have the same precedence, and associate to the left, but juxtaposition has higher syntactic precedence than both `*` and `/`.
- Integer powers (`^`) have higher precedence than juxtaposition.
- The `/` symbol can also be used as a unary reciprocal operator.

## Constants Annotated by Measures

A floating-point constant can be annotated with its measure by specifying a literal measure in angle brackets following the constant.

Measure annotations on constants may not include measure variables.

Here are some examples of annotated constants:

```fsharp
let earthGravity = 9.81f<m/s^2>
let atmosphere = 101325.0<N m^-2>
let zero = 0.0f<_>
```

Constants that are annotated with units of measure are assigned a corresponding numeric type with the measure parameter that is specified in the annotation. In the example above, `earthGravity` is assigned the type `float32<m/s^2>`, `atmosphere` is assigned the type `float<N/m^2>` and `zero` is assigned the type `float<'U>`.

## Relations of Measures

After measures are parsed and checked, they are maintained in the following normalized form:

```fsgrammar
measure-int := 1 | long-ident | measure-par | measure-int measure-int | / measure-int
```

Powers of measures are expanded. For example, `kg^3` is equivalent to `kg` `kg` `kg`.

Two measures are indistinguishable if they can be made equivalent by repeated application of the following rules:

- _Commutativity_. `measure-int1 measure-int2` is equivalent to `measure-int2 measure-int1`.
- _Associativity_. It does not matter what grouping is used for juxtaposition (product) of measures, so parentheses are not required. For example, `kg m s` can be split as the product of `kg m` and `s`, or as the product of `kg` and `m s`.
- _Identity_. 1 `measure-int` is equivalent to `measure-int`.
- _Inverses_. `measure-int / measure-int` is equivalent to `1`.
- _Abbreviation_. `long-ident` is equivalent to `measure` if a measure abbreviation of the form `[<Measure>] type long-ident = measure` is currently in scope.

Note that these are the laws of Abelian groups together with expansion of abbreviations.

For example, `kg m / s^2` is the same as `m kg / s^2`.

For presentation purposes (for example, in error messages), measures are presented in the normalized form that appears at the beginning of this section, but with the following restrictions:

- Powers are positive and greater than 1. This splits the measure into positive powers and negative powers, separated by `/`.
- Atomic measures are ordered as follows: measure parameters first, ordered alphabetically, followed by measure identifiers, ordered alphabetically.

For example, the measure expression `m^1 kg s^-1` would be normalized to `kg m / s`.

This normalized form provides a convenient way to check the equality of measures: given two measure expressions `measure-int1` and `measure-int2` , reduce each to normalized form by using the rules of commutativity, associativity, identity, inverses, and abbreviation, and then compare the syntax.

To check the equality of two measures, abbreviations are expanded to compare their normalized forms. However, abbreviations are not expanded for presentation. For example, consider the following definitions:

```fsharp
[<Measure>] type a
[<Measure>] type b = a * a
let x = 1<b> / 1<a>
```

The inferred type is presented as `int<b/a>`, not `int<a>`. If a measure is equivalent to `1`, however, abbreviations are expanded to cancel each other and are presented without units:

```fsharp
let y = 1<b> / 1<a a> // val y : int = 1
```

### Constraint Solving

The mechanism described in [§14.5](inference-procedures.md#constraint-solving) is extended to support equational constraints between measure expressions. Such expressions arise from equations between parameterized types — that is, when `type<tyarg11 , ..., tyarg1n> = type<tyarg21, ..., tyarg2n>` is reduced to a series of constraints `tyarg1i = tyarg2i`. For the arguments that are measures, rather than types, the rules listed in [§9.3](units-of-measure.md#relations-on-measures) are applied to obtain primitive equations of the form `'U = measure-int` where `'U` is a measure variable and `measure-int` is a measure expression in internal form. The variable `'U` is then replaced by `measure-int` wherever else it occurs. For example, the equation `float<m^2/s^2> = float<'U^2>` would be reduced to the `constraint m^2/s^2 = 'U^2`, which would be further reduced to the primitive equation `'U = m/s`.

If constraints cannot be solved, a type error occurs. For example, the following expression

```fsharp
fun (x : float<m^2>, y : float<s>) -> x + y
```

would eventually result in the constraint `m^2 = s`, which cannot be solved, indicating a type error.

### Generalization of Measure Variables

Analogous to the process of generalization of type variables described in [§14.6.7](inference-procedures.md#generalization), a generalization procedure produces measure variables over which a value, function, or member can be generalized.

## Measure Definitions

Measure definitions define new named units of measure by using the same syntax as for type definitions, with the addition of the `Measure` attribute. For example:

```fsharp
[<Measure>] type kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = kg / m s^2
```

A primitive measure abbreviation defines a fresh, named measure that is distinct from other measures. Measure abbreviations, like type abbreviations, define new names for existing measures. Also like type abbreviations, repeatedly eliminating measure abbreviations in favor of their equivalent measures must not result in infinite measure expressions. For example, the following is not a valid measure definition because it results in the infinite squaring of `X`:

```fsharp
[<Measure>] type X = X^2
```

Measure definitions and abbreviations may not have type or measure parameters.

## Measure Parameter Definitions

Measure parameter definitions can appear wherever ordinary type parameter definitions can (see [§5.2.9](types-and-type-constraints.md#unmanaged-constraints)). If an explicit parameter definition is used, the parameter name is prefixed by the special `Measure` attribute. For example:

```fsharp
val sqr<[<Measure>] 'U> : float<'U> -> float<'U^2>

type Vector<[<Measure>] 'U> =
    { X: float<'U>;
      Y: float<'U>;
      Z: float<'U> }

type Sphere<[<Measure>] 'U> =
    { Center:Vector<'U>;
      Radius:float<'U> }

type Disc<[<Measure>] 'U> =
    { Center:Vector<'U>;
      Radius:float<'U>;
      Norm:Vector<1> }

type SceneObject<[<Measure>] 'U> =
    | Sphere of Sphere<'U>
    | Disc of Disc<'U>
```

Internally, the type checker distinguishes between type parameters and measure parameters by assigning one of two _sorts_ (Type or Measure) to each parameter. This technique is used to check the actual arguments to types and other parameterized definitions. The type checker rejects ill-formed types such as `float<int>` and `IEnumerable<m/s>`.

## Measure Parameter Erasure

In contrast to _type_ parameters on generic types, _measure_ parameters are not exposed in the metadata that the runtime interprets; instead, measures are _erased_. Erasure has several consequences:

- Casting is with respect to erased types.
- Method application resolution (see [§14.4](inference-procedures.md#method-application-resolution)) is with respect to erased types.
- Reflection is with respect to erased types.

## Type Definitions with Measures in the F# Core Library

The F# core library defines the following types:

```fsharp
type float<[<Measure>] 'U>
type float32<[<Measure>] 'U>
type decimal<[<Measure>] 'U>
type sbyte<[<Measure>] 'U>
type int16<[<Measure>] 'U>
type int<[<Measure>] 'U>
type int64<[<Measure>] 'U>
type nativeint<[<Measure>] 'U>
type uint<[<Measure>] 'U>
type byte<[<Measure>] 'U>
type uint16<[<Measure>] 'U>
type uint64<[<Measure>] 'U>
type unativeint<[<Measure>] 'U>
```

> Note: These definitions are called measure-annotated base types and are marked with the `MeasureAnnotatedAbbreviation` attribute in the implementation of the library. The `MeasureAnnotatedAbbreviation` attribute is not for use in user code and in future revisions of the language may result in a warning or error.

These type definitions have the following special properties:

- They extend `System.ValueType`.
- They explicitly implement `System.IFormattable`, `System.IComparable`, `System.IConvertible`, and corresponding generic interfaces, instantiated at the given type—for example, `System.IComparable<float<'u>>` and `System.IEquatable<float<'u>>` (so that you can invoke, for example, `CompareTo` after an explicit upcast).
- As a result of erasure, their compiled form is the corresponding primitive type.
- For the purposes of constraint solving and other logical operations on types, a type equivalence holds between the unparameterized primitive type and the corresponding measured type definition that is instantiated at `<1>`:

    ```fsother
    sbyte = sbyte<1>
    int16 = int16<1>
    int = int<1>
    int64 = int64<1>
    byte = byte<1>
    uint16 = uint16<1>
    uint = uint<1>
    uint64 = uint64<1>
    float = float<1>
    float32 = float32<1>
    decimal = decimal<1>
    ```

- The measured type definitions `byte`,  `uint16`, `uint`, `uint64`, `sbyte`, `int16`, `int`, `int64`, `float`, `float32`, `decimal`, `unativeint`, and `nativeint` are assumed to have additional static members that have the measure types that are listed in the table. Note that `N` is any of these types, and `F` is either `float32` or `float`.

| Member                                            | Measure Type                  |
| ------------------------------------------------- | ----------------------------- |
| `Sqrt`                                            | `F<'U^2> -> F<'U>`            |
| `Atan2`                                           | `F<'U> -> F<'U> -> F<1>`      |
| `op_Addition`<br>`op_Subtraction`<br>`op_Modulus` | `N<'U> -> N<'U> -> N<'U>`     |
| `op_Multiply`                                     | `N<'U> -> N<'V> -> N<'U 'V>`  |
| `op_Division`                                     | `N<'U> -> N<'V> -> N<'U/'V>`  |
| `Abs`<br>`op_UnaryNegation`<br>`op_UnaryPlus`     | `N<'U> -> N<'U>`              |
| `Sign`                                            | `N<'U> -> int`                |

This mechanism is used to support units of measure in the following math functions of the F# library:
`(+)`, `(-)`, `(*)`, `(/)`, `(%)`, `(~+)`, `(~-)`, `abs`, `sign`, `atan2` and `sqrt`.

Additionally, the F# core library provides the following measure-annotated aliases, which are functionally equivalent to the previously-listed measure-annotated types, and which are included for the sake of completeness:

```fsharp
type double<[<Measure>] 'U> // aliases float<'U>
type single<[<Measure>] 'U> // aliases float32<'U>
type int8<[<Measure>] 'U>   // aliases sbyte<'U>
type int32<[<Measure>] 'U>  // aliases int<'U>
type uint8<[<Measure>] 'U>  // aliases byte<'U>
type uint32<[<Measure>] 'U> // aliases uint<'U>
```

## Restrictions

Measures can be used in range expressions but a properly measured step is required. For example, these are not allowed:

```fsharp
[<Measure>] type s
[1<s> .. 5<s>] // error: The type 'int<s>' does not match the type 'int'
[1<s> .. 1 .. 5<s>] // error: The type 'int<s>' does not match the type 'int'
```

However, the following range expression is valid:

```fsharp
[1<s> .. 1<s> .. 5<s>] // int<s> list = [1; 2; 3; 4; 5]
```
