# The F# Library FSharp.Core.dll

All compilations reference the following two base libraries:

- The CLI base library `mscorlib.dll`.
- The F# base library `FSharp.Core.dll`.

The API documentation of these libraries can be found at [https://fsharp.github.io/fsharp-core-docs](https://fsharp.github.io/fsharp-core-docs) and [https://learn.microsoft.com/dotnet/standard/runtime-libraries-overview](https://learn.microsoft.com/dotnet/standard/runtime-libraries-overview), resp.


The following namespaces are automatically opened for all F# code:

```fsharp
open FSharp
open FSharp.Core
open FSharp.Core.LanguagePrimitives
open FSharp.Core.Operators
open FSharp.Text
open FSharp.Collections
open FSharp.Core.ExtraTopLevelOperators
```
A compilation may open additional namespaces may be opened if the referenced F# DLLs contain
`AutoOpenAttribute` declarations.


## Basic Types (FSharp.Core)

This section provides details about the basic types that are defined in `FSharp.Core`.

### Basic Type Abbreviations

| Type Name | Description |
| --- | --- |
| `obj` | `System.Object` |
| `exn` | `System.Exception` |
| `nativeint` | `System.IntPtr` |
| `unativeint` | `System.UIntPtr` |
| `string` | `System.String` |
| `float32`, `single` | `System.Single` |
| `float`, `double` | `System.Double` |
| `sbyte`, `int8` | `System.SByte` |
| `byte`, `uint8` | `System.Byte` |
| `int16` | `System.Int16` |
| `uint16` | `System.UInt16` |
| `int32`, `int` | `System.Int32` |
| `uint32` | `System.UInt32` |
| `int64` | `System.Int64` |
| `uint64` | `System.UInt64` |
| `char` | `System.Char` |
| `bool` | `System.Boolean` |
| `decimal` | `System.Decimal` |

### Basic Types that Accept Unit of Measure Annotations

| Type Name | Description |
| --- | --- |
| `sbyte<_>` | Underlying representation `System.SByte`, but accepts a unit of measure. |
| `int16<_>` | Underlying representation `System.Int16`, but accepts a unit of measure. |
| `int32<_>` | Underlying representation `System.Int32`, but accepts a unit of measure. |
| `int64<_>` | Underlying representation `System.Int64`, but accepts a unit of measure. |
| `float32<_>` | Underlying representation `System.Single`, but accepts a unit of measure. |
| `float<_>` | Underlying representation `System.Double`, but accepts a unit of measure. |
| `decimal<_>` | Underlying representation `System.Decimal`, but accepts a unit of measure. |

### The nativeptr<_> Type

When the `nativeptr<type>` is used in method argument or return position, it is represented in
compiled CIL code as either:

- A CLI pointer type `type*`, if `type` does not contain any generic type parameters.
- T CLI type `System.IntPtr` otherwise.

> Note : CLI pointer types are rarely used. In CLI metadata, pointer types sometimes
appear in CLI metadata unsafe object constructors for the CLI type `System.String`.
<br>
You can convert between `System.UIntPtr` and `nativeptr<'T>` by using the inlined
unverifiable functions in `FSharp.NativeInterop.NativePtr`.
<br>
`nativeptr<_>` compiles in different ways because CLI restricts where pointer types can
appear.

## Basic Operators and Functions (FSharp.Core.Operators)

### Basic Arithmetic Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `(+)` | `x + y` | Overloaded addition. |
| `(-)` | `x - y` | Overloaded subtraction. |
| `(*)` | `x * y` | Overloaded multiplication. |
| `(/)` | `x / y` | Overloaded division.<br>For negative numbers, the behavior of this operator follows the definition of the corresponding operator in the C# specification. |
| `(%)` | `x % y` | Overloaded remainder.<br>For integer types, the result of x % y is the value produced by x – (x / y) * y. If y is zero, `System.DivideByZeroException` is thrown. The remainder operator never causes an overflow. This follows the definition of the remainder operator in the C# specification.<br>For floating-point types, the behavior of this operator also follows the definition of the remainder operator in the C# specification. |
| `(~-)` | `-x` | Overloaded unary negation.
| `not` | `not x` | Boolean negation.


### Generic Equality and Comparison Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `(<)` | `x < y` | Generic less-than |
| `(<=)` | `x <= y` | Generic less-than-or-equal |
| `(>)` | `x > y` | Generic greater-than |
| `(>=)` | `x >= y` | Generic greater-than-or-equal |
| `(=)` | `x = y` | Generic equality |
| `(<>)` | `x <> y` | Generic disequality |
| `max` | `max x y` | Generic maximum |
| `min` | `min x y` | Generic minimum |

### Bitwise Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `(<<<)` | `x <<< y` | Overloaded bitwise left-shift |
| `(>>>)` | `x >>> y` | Overloaded bitwise arithmetic right-shift |
| `(^^^)` | `x ^^^ y` | Overloaded bitwise exclusive or (XOR) |
| `(&&&)` | `x &&& y` | Overloaded bitwise and |
| `(\|\|\|)` | `x \|\|\| y` | Overloaded bitwise or |
| `(~~~)` | `~~~x` | Overloaded bitwise negation |

### Math Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `abs` | `abs x ` | Overloaded absolute value |
| `acos` | `acos x ` | Overloaded inverse cosine |
| `asin` | `asin x ` | Overloaded inverse sine |
| `atan` | `atan x ` | Overloaded inverse tangent |
| `atan2` | `atan2 x y ` | Overloaded inverse tangent of x/y |
| `ceil` | `ceil x ` | Overloaded floating-point ceiling |
| `cos` | `cos x ` | Overloaded cosine |
| `cosh` | `cosh x ` | Overloaded hyperbolic cosine |
| `exp` | `exp x ` | Overloaded exponent |
| `floor` | `floor x ` | Overloaded floating-point floor |
| `log` | `log x ` | Overloaded natural logarithm |
| `log10` | `log10 x ` | Overloaded base-10 logarithm |
| `(**)` | `x ** y ` | Overloaded exponential |
| `pown` | `pown x y ` | Overloaded integer exponential |
| `round` | `round x ` | Overloaded rounding |
| `sign` | `sign x ` | Overloaded sign function |
| `sin` | `sin x ` | Overloaded sine function |
| `sinh` | `sinh x ` | Overloaded hyperbolic sine function |
| `sqrt` | `sqrt x ` | Overloaded square root function |
| `tan` | `tan x ` | Overloaded tangent function |
| `tanh` | `tanh x ` | Overloaded hyperbolic tangent function |


### Function Pipelining and Composition Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `(\|>)` | `x \|> f ` | Pipelines the value `x` to the function `f` (forward pipelining) |
| `(>>)` | `f >> g ` | Composes two functions, so that they are applied in order from left to right |
| `(<\|)` | `f <\| x ` | Pipelines the value `x` to the function `f` (backward pipelining) |
| `(<<)` | `g << f ` | Composes two functions, so that they are applied in order from right to left (backward function composition) |
| `ignore` | `ignore x ` | Computes and discards a value |

### Object Transformation Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `box` | `box x ` | Converts to object representation. |
| `hash` | `hash x ` | Generates a hash value. |
| `sizeof` | `sizeof<type> ` | Computes the size of a value of the given type. |
| `typeof` | `typeof<type> ` | Computes the `System.Type` representation of the giventype. |
| `typedefof` | `typedefof<type> ` | Computes the `System.Type` representation of `type` and calls `GetGenericTypeDefinition` if it is a generic type. |
| `unbox` | `unbox x ` | Converts from object representation. |
| `ref` | `ref x ` | Allocates a mutable reference cell. |
| `(!)` | `!x` | Reads a mutable reference cell. |

### Pair Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `fst` | `fst p ` | Returns the first element of a pair. |
| `snd` | `snd p ` | Returns the second element of a pair |

### Exception Operators

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `failwith` | `failwith x ` | Raises a `FailureException` exception. |
| `invalidArg` | `invalidArg x ` | Raises an `ArgumentException` exception. |
| `raise` | `raise x ` | Raises an `exception. |
| reraise | reraise() | Rethrows the current exception. |

### Input/Output Handles

The following operators are defined in `FSharp.Core.Operators`:


| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `stdin` | `Stdin ` | Computes `System.Console.In`. |
| `stdout` | `Stdout ` | Computes `System.Console.Out`. |
| `stderr` | `Stderr ` | Computes `System.Console.Error`. |

### Overloaded Conversion Functions

The following operators are defined in `FSharp.Core.Operators`:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `byte` | `byte x ` | Overloaded conversion to a byte |
| `sbyte` | `sbyte x ` | Overloaded conversion to a signed byte |
| `int16` | `int16 x ` | Overloaded conversion to a 16-bit integer |
| `uint16` | `uint16 x ` | Overloaded conversion to an unsigned 16-bit integer |
| `int32`, `int` | `int32 x`, `int x` | Overloaded conversion to a 32-bit integer |
| `uint32` | `uint32 x ` | Overloaded conversion to an unsigned 32-bit integer |
| `int64` | `int64 x ` | Overloaded conversion to a 64-bit integer |
| `uint64` | `uint64 x ` | Overloaded conversion to an unsigned 64-bit integer |
| `nativeint` | `nativeint x ` | Overloaded conversion to an native integer |
| `unativeint` | `unativeint x ` | Overloaded conversion to an unsigned native integer |
| `float`, `double` | `float x`, `double x` | Overloaded conversion to a 64-bit IEEE floating-point number |
| `float32`, `single` | `float32 x`, `single x` | Overloaded conversion to a 32-bit IEEE floating-point number |
| `decimal` | `decimal x ` | Overloaded conversion to a System.Decimal number |
| `char` | `char x ` | Overloaded conversion to a System.Char value |
| `enum` | `enum x ` | Overloaded conversion to a typed enumeration value |

## Checked Arithmetic Operators

The module `FSharp.Core.Operators.Checked` defines runtime-overflow-checked versions of the
following operators:

| Operator or Function Name | Expression Form | Description |
| --- | --- | --- |
| `(+)` | `x + y` | Checked overloaded addition |
| `(-)` | `x – y` | Checked overloaded subtraction |
| `(*)` | `x * y` | Checked overloaded multiplication |
| (~-) | `-x` | Checked overloaded unary negation |
| `byte` | `byte x ` | Checked overloaded conversion to a byte |
| `sbyte` | `sbyte x ` | Checked overloaded conversion to a signed byte |
| `int16` | `int16 x ` | Checked overloaded conversion to a 16-bit integer |
| `uint16` | `uint16 x ` | Checked overloaded conversion to an unsigned 16-bit integer |
| `int32`, `int` | `int32 x`, `int x` | Checked overloaded conversion to a 32-bit integer |
| `uint32` | `uint32 x ` | Checked overloaded conversion to an unsigned 32-bit integer |
| `int64` | `int64 x ` | Checked overloaded conversion to a 64 - bit integer |
| `uint64` | `uint64 x ` | Checked overloaded conversion to an unsigned 64-bit integer |
| `nativeint` | `nativeint x ` | Checked overloaded conversion to an native integer |
| `unativeint` | `unativeint x ` | Checked overloaded conversion to an unsigned native integer |
| `char` | `char x ` | Checked overloaded conversion to a `System.Char` value |

## List and Option Types

### The List Type

The following shows the elements of the F# type `FSharp.Collections.list` referred to in this
specification:

```fsharp
type 'T list =
    | ([])
    | (::) of 'T * 'T list
    static member Empty : 'T list
    member Length : int
    member IsEmpty : bool
    member Head : 'T
    member Tail : 'T list
    member Item :int -> 'T with get
    static member Cons : 'T * 'T list -> 'T list
    interface System.Collections.Generic.IEnumerable<'T>
    interface System.Collections.IEnumerable
```
See also the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/lists) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-list-1.html).

### The Option Type

The following shows the elements of the F# type `FSharp.Core.option` referred to in this specification:

```fsharp
[<DefaultAugmentation(false)>]
[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type 'T option =
    | None
    | Some of 'T
    static member None : 'T option
    static member Some : 'T -> 'T option
    [<CompilationRepresentation(CompilationRepresentationFlags.Instance)>]
    member Value : 'T
    member IsSome : bool
    member IsNone : bool
```
See also the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/options) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-option-1.html).

## Lazy Computations (Lazy)

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/lazy-expressions) and the [FSharp.Core documentation]().

## Asynchronous Computations (Async)

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/async-expressions) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-control-fsharpasync.html).


## Query Expressions

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/query-expressions) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-linq-querybuilder.html).


## Agents (MailboxProcessor)

Check the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-control-fsharpmailboxprocessor-1.html).

## Event Types

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/members/events) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-control-eventmodule.html).

## Immutable Collection Types (Map, Set)

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/fsharp-collection-types) and the [FSharp.Core documentation for Map](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-fsharpmap-2.html) and for [Set](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-fsharpset-1.html).

## Text Formatting (printf)

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/plaintext-formatting) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-printfmodule.html)

## Reflection

Check the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-reflection.html).

## Quotations

Check the [Language Guide](https://learn.microsoft.com/dotnet/fsharp/language-reference/code-quotations) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-quotations.html).

## Native Pointer Operations

The [FSharp.Core.NativeInterop namespace](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-nativeinterop.html) contains functionality for interoperating with native code.

Use of these functions is unsafe, and incorrect use may generate invalid IL code.

| Operator or Function Name | Description |
| --- | --- |
| `NativePtr.ofNativeInt` | Returns a typed native pointer for a machine address. |
| `NativePtr.toNativeInt` | Returns a machine address for a typed native pointer. |
| `NativePtr.add` | Computes an indexed offset from the input pointer. |
| `NativePtr.read` | Reads the memory that the input pointer references. |
| `NativePtr.write` | Writes to the memory that the input pointer references. |
| `NativePtr.get` | Reads the memory at an indexed offset from the input pointer. |
| `NativePtr.set` | Writes the memory at an indexed offset from the input pointer. |
| `NativePtr.stackalloc` | Allocates a region of memory on the stack. |

### Stack Allocation

The `NativePtr.stackalloc` function works as follows. Given

```fsharp
stackalloc<ty> n
```
the unmanaged type `ty` specifies the type of the items that will be stored in the newly allocated
location, and `n` indicates the number of these items. Taken together, these establish the required
allocation size.

The `stackalloc` function allocates `n * sizeof<ty>` bytes from the call stack and returns a pointer of
type `nativeptr<ty>` to the newly allocated block. The content of the newly allocated memory is
undefined. If `n` is a negative value, the behavior of the function is undefined. If `n` is zero, no allocation
is made, and the returned pointer is implementation-defined. If insufficient memory is available to
allocate a block of the requested size, the `System.StackOverflowException` is thrown.

Use of this function is unsafe, and incorrect use might generate invalid IL code. For example, the
function should not be used in `with` or `finally` blocks in `try`/`with` or `try`/`finally` expressions. These
conditions are not checked by the F# compiler, because this primitive is rarely used from F# code.

There is no way to explicitly free memory that is allocated using stackalloc. All stack-allocated
memory blocks that are created during the execution of a function or member are automatically
discarded when that function or member returns. This behavior is similar to that of the `alloca`
function, an extension commonly found in C and C++ implementations.

