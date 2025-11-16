# Expressions

The expression forms and related elements are as follows:

```fsgrammar
expr :=
    const                               -- a constant value
    ( expr )                            -- block expression
    begin expr end                      -- block expression
    long-ident-or-op                    -- lookup expression
    expr '.' long-ident-or-op           -- dot lookup expression
    expr expr                           -- application expression
    expr ( expr )                       -- high precedence application
    expr < types >                      -- type application expression
    expr infix-op expr                  -- infix application expression
    prefix-op expr                      -- prefix application expression
    expr .[ expr ]                      -- indexed lookup expression
    expr .[ slice-ranges ]              -- slice expression
    expr <- expr                        -- assignment expression
    expr , ... , expr                   -- tuple expression
    struct (expr , ... , expr)          -- struct tuple expression
    new type expr                       -- simple object expression
    { new base-call object-members interface-impls } -- object expression
    { field-initializers }              -- record expression
    { expr with field-initializers }    -- record cloning expression
    [ expr ; ... ; expr ]               -- list expression
    [| expr ; ... ; expr |]             -- array expression
    expr { comp-or-range-expr }         -- computation expression
    [ comp-or-range-expr ]              -- computed list expression
    [| comp-or-range-expr |]            -- computed array expression
    lazy expr                           -- delayed expression
    null                                -- the "null" value for a reference type
    expr : type                         -- type annotation
    expr :> type                        -- static upcast coercion
    expr :? type                        -- dynamic type test
    expr :?> type                       -- dynamic downcast coercion
    upcast expr                         -- static upcast expression
    downcast expr                       -- dynamic downcast expression
    let function-defn in expr           -- function definition expression
    let value-defn in expr              -- value definition expression
    let rec function-or-value-defns in expr -- recursive definition expression
    use ident = expr in expr            -- deterministic disposal expression
    use ident = fixed expr              -- pinned pointer expression
    fun argument-pats - > expr          -- function expression
    function rules                      -- matching function expression
    expr ; expr                         -- sequential execution expression
    match expr with rules               -- match expression
    try expr with rules                 -- try/with expression
    try expr finally expr               -- try/finally expression
    if expr then expr elif-branches? else-branch? -- conditional expression
    while expr do expr done             -- while loop
    for ident = expr to expr do expr done -- simple for loop
    for pat in expr - or-range-expr do expr done -- enumerable for loop
    assert expr                         -- assert expression
    <@ expr @>                          -- quoted expression
    <@@ expr @@>                        -- quoted expression

    %expr                              -- expression splice
    %%expr                              -- weakly typed expression splice

    (static-typars : (member-sig) expr) -– static member invocation
```

Expressions are defined in terms of patterns and other entities that are discussed later in this
specification. The following constructs are also used:

```fsgrammar
exprs := expr ',' ... ',' expr

expr-or-range-expr :=
    expr
    range-expr

elif-branches := elif-branch ... elif-branch

elif-branch := elif expr then expr

else-branch := else expr

function-or-value-defn :=
    function-defn
    value-defn

function-defn :=
    inline? access? ident-or-op typar-defns? argument-pats return-type? = expr

value-defn :=
    mutable? access? pat typar-defns? return-type? = expr

return-type :=
    : type

function-or-value-defns :=
    function-or-value-defn and ... and function-or-value-defn

argument-pats := atomic-pat ... atomic-pat

field-initializer :=
    long-ident = expr -- field initialization

field-initializers := field-initializer ; ... ; field-initializer

object-construction :=
    type expr -- construction expression
    type      -- interface construction expression

base-call :=
    object-construction          -- anonymous base construction
    object-construction as ident -- named base construction

interface-impls := interface-impl ... interface-impl

interface-impl :=
    interface type object-members? -- interface implementation

object-members := with member-defns end

member-defns := member-defn ... member-defn
```

Computation and range expressions are defined in terms of the following productions:

```fsgrammar
comp-or-range-expr :=
    comp-expr
    short-comp-expr
    range-expr

comp-expr :=
    let! pat = expr in comp-expr    -- binding computation
    let pat = expr in comp-expr
    do! expr in comp-expr           -- sequential computation
    do expr in comp-expr
    use! pat = expr in comp-expr    -- auto cleanup computation
    use pat = expr in comp-expr
    yield! expr                     -- yield computation
    yield expr                      -- yield result
    return! expr                    -- return computation
    return expr                     -- return result
    if expr then comp - expr        -- control flow or imperative action
    if expr then expr else comp-expr
    match! expr with pat -> comp-expr | ... | pat -> comp-expr
    match expr with pat -> comp-expr | ... | pat -> comp-expr
    try comp - expr with pat -> comp-expr | ... | pat -> comp-expr
    try comp - expr finally expr
    while expr do comp - expr done
    for ident = expr to expr do comp - expr done
    for pat in expr - or-range-expr do comp - expr done
    comp - expr ; comp - expr
    expr

short-comp-expr :=
    for pat in expr-or-range-expr -> expr -- yield result

range-expr :=
    expr .. expr                    -- range sequence
    expr .. expr .. expr            -- range sequence with skip

slice-ranges := slice-range , ... , slice-range

slice-range :=
    expr                            -- slice of one element of dimension
    expr ..                         -- slice from index to end
    .. expr                         -- slice from start to index
    expr .. expr                    -- slice from index to index
    '*'                             -- slice from start to end
```

## Some Checking and Inference Terminology

The rules applied to check individual expressions are described in the following subsections. Where
necessary, these sections reference specific inference procedures such as _Name Resolution_ ([§](inference-procedures.md#name-resolution))
and _Constraint Solving_ ([§](inference-procedures.md#constraint-solving)).

All expressions are assigned a static type through type checking and inference. During type checking,
each expression is checked with respect to an _initial type_. The initial type establishes some of the
information available to resolve method overloading and other language constructs. We also use the
following terminology:

- The phrase “the type `ty1` is asserted to be equal to the type `ty2`” or simply “`ty1 = ty2` is asserted”
    indicates that the constraint “`ty1 = ty2`” is added to the current inference constraints.

- The phrase “`ty1` is asserted to be a subtype of `ty2`” or simply “`ty1 :> ty2` is asserted” indicates
    that the constraint `ty1 :> ty2` is added to the current inference constraints.
- The phrase “type `ty` is known to ...” indicates that the initial type satisfies the given property
    given the current inference constraints.
- The phrase “the expression `expr` has type `ty` ” means the initial type of the expression is asserted
    to be equal to `ty`.

Additionally:

- The addition of constraints to the type inference constraint set fails if it causes an inconsistent
    set of constraints ([§](inference-procedures.md#constraint-solving)). In this case either an error is reported or, if we are only attempting to
    _assert_ the condition, the state of the inference procedure is left unchanged and the test fails.

## Elaboration and Elaborated Expressions

Checking an expression generates an _elaborated expression_ in a simpler, reduced language that
effectively contains a fully resolved and annotated form of the expression. The elaborated
expression provides more explicit information than the source form. For example, the elaborated
form of `System.Console.WriteLine("Hello")` indicates exactly which overloaded method definition
the call has resolved to.
<!-- Elaborated forms are underlined in this specification, for example, <u>let x = 1 in x + x</u>. -->

Except for this extra resolution information, elaborated forms are syntactically a subset of syntactic
expressions, and in some cases (such as constants) the elaborated form is the same as the source
form. This specification uses the following elaborated forms:

- Constants
- Resolved value references: `path`
- Lambda expressions: `(fun ident -> expr)`
- Primitive object expressions
- Data expressions (tuples, union cases, array creation, record creation)
- Default initialization expressions
- Local definitions of values: `let ident = expr in expr`
- Local definitions of functions:
    `let rec ident = expr and ... and ident = expr in expr`
- Applications of methods and functions (with static overloading resolved)
- Dynamic type coercions: `expr :?> type`
- Dynamic type tests: `expr :? type`
- For-loops: `for ident in ident to ident do expr done`
- While-loops: `while expr do expr done`
- Sequencing: `expr; expr`
- Try-with: `try expr with expr`
- Try-finally: `try expr finally expr`
- The constructs required for the elaboration of pattern matching ([§](patterns.md#patterns)).
  - Null tests
  - Switches on integers and other types
  - Switches on union cases
  - Switches on the runtime types of objects

The following constructs are used in the elaborated forms of expressions that make direct
assignments to local variables and arrays and generate “byref” pointer values. The operations are
loosely named after their corresponding primitive constructs in the CLI.

- Assigning to a byref-pointer: `expr <-stobj expr`
- Generating a byref-pointer by taking the address of a mutable value: `&path`.
- Generating a byref-pointer by taking the address of a record field: `&(expr.field)`
- Generating a byref-pointer by taking the address of an array element: `&(expr.[expr])`

Elaborated expressions form the basis for evaluation (see [§](expressions.md#evaluation-of-elaborated-forms)) and for the expression trees that
_quoted expressions_ return (see [§](expressions.md#quoted-expressions)).

By convention, when describing the process of elaborating compound expressions, we omit the
process of recursively elaborating sub-expressions.

## Data Expressions

This section describes the following data expressions:

- Simple constant expressions
- Tuple expressions
- List expressions
- Array expressions
- Record expressions
- Copy-and-update record expressions
- Function expressions
- Object expressions
- Delayed expressions
- Computation expressions
- Sequence expressions
- Range expressions
- Lists via sequence expressions
- Arrays via sequence expressions
- Null expressions
- 'printf' formats

### Simple Constant Expressions

Simple constant expressions are numeric, string, Boolean and unit constants. For example:

```fsgrammar
3y              // sbyte
32uy            // byte
17s             // int16
18us            // uint16
86              // int/int32
99u             // uint32
99999999L       // int64
10328273UL      // uint64
1.              // float/double
1.01            // float/double
1.01e10         // float/double
1.0f            // float32/single
1.01f           // float32/single
1.01e10f        // float32/single
99999999n       // nativeint    (System.IntPtr)
10328273un      // unativeint   (System.UIntPtr)
99999999I       // bigint       (System.Numerics.BigInteger or user-specified)
'a'             // char         (System.Char)
"3"             // string       (String)
"c:\\home"      // string       (System.String)
@"c:\home"      // string       (Verbatim Unicode, System.String)
"ASCII"B        // byte[]
()              // unit         (FSharp.Core.Unit)
false           // bool         (System.Boolean)
true            // bool         (System.Boolean)
```

Simple constant expressions have the corresponding simple type and elaborate to the corresponding
simple constant value.

Integer literals with the suffixes `Q`, `R`, `Z`, `I`, `N`, `G` are processed using the following syntactic translation:

```fsgrammar
xxxx<suffix>
    For xxxx = 0                → NumericLiteral<suffix>.FromZero()
    For xxxx = 1                → NumericLiteral<suffix>.FromOne()
    For xxxx in the Int32 range → NumericLiteral<suffix>.FromInt32(xxxx)
    For xxxx in the Int64 range → NumericLiteral<suffix>.FromInt64(xxxx)
    For other numbers           → NumericLiteral<suffix>.FromString("xxxx")
```

For example, defining a module `NumericLiteralZ` as below enables the use of the literal form `32Z` to
generate a sequence of 32 ‘Z’ characters. No literal syntax is available for numbers outside the range
of 32-bit integers.

```fsharp
module NumericLiteralZ =
    let FromZero() = ""
    let FromOne() = "Z"
    let FromInt32 n = String.replicate n "Z"
```

F# compilers may optimize on the assumption that calls to numeric literal functions always
terminate, are idempotent, and do not have observable side effects.

### Tuple Expressions

An expression of the form `expr1 , ..., exprn` is a _tuple expression_. For example:

```fsharp
let three = (1,2,"3")
let blastoff = (10,9,8,7,6,5,4,3,2,1,0)
```

The expression has the type `S<ty1 * ... * tyn>` for fresh types `ty1 ... tyn` and fresh pseudo-type `S` that indicates the "structness" (i.e. reference tuple or struct tuple) of the tuple. Each individual
expression `expri` is checked using initial type `tyi`. The pseudo-type `S` participates in type checking similar to normal types until it is resolved to either reference or struct tuple, with a default of reference tuple.

An expression of the form `struct (expr1 , ..., exprn)` is a _struct tuple expression_. For example:

```fsharp
let pair = struct (1,2)
```

A _struct tuple expression_ is checked in the same way as a _tuple expression_, but the pseudo-type `S` is resolved to struct tuple.

Tuple types and expressions that have `S` resolved to reference tuple are translated into applications of a family of .NET types named
[`System.Tuple`](https://learn.microsoft.com/dotnet/api/system.tuple). Tuple types `ty1 * ... * tyn` are translated as follows:

- For `n <= 7` the elaborated form is `Tuple<ty1 ,... , tyn>`.
- For larger `n` , tuple types are shorthand for applications of the additional F# library type
    System.Tuple<_> as follows:
  - For `n = 8` the elaborated form is `Tuple<ty1, ..., ty7, Tuple<ty8>>`.
  - For `9 <= n` the elaborated form is `Tuple<ty1, ..., ty7, tyB>` where `tyB` is the converted form of
       the type `(ty8 * ... * tyn)`.

Tuple expressions `(expr1, ..., exprn)` are translated as follows:

- For `n <= 7` the elaborated form `new Tuple<ty1, ..., tyn>(expr1, ..., exprn)`.
- For `n = 8` the elaborated form `new Tuple<ty1, ..., ty7, Tuple<ty8>>(expr1, ..., expr7, new Tuple<ty8>(expr8)`.
- For `9 <= n` the elaborated form `new Tuple<ty1, ... ty7, ty8n>(expr1, ..., expr7, new ty8n(e8n)`
    where `ty8n` is the type `(ty8 * ... * tyn)` and `expr8n` is the elaborated form of the expression
    `expr8, ..., exprn`.

When considered as static types, tuple types are distinct from their encoded form. However, the
encoded form of tuple values and types is visible in the F# type system through runtime types. For
example, `typeof<int * int>` is equivalent to `typeof<System.Tuple<int,int>>`, and `(1 ,2)` has the
runtime type `System.Tuple<int,int>`. Likewise, `(1,2,3,4,5,6,7,8,9)` has the runtime type
`Tuple<int,int,int,int,int,int,int,Tuple<int,int>>`.

Tuple types and expressions that have `S` resolved to struct tuple are translated in the same way to [`System.ValueTuple`](https://learn.microsoft.com/dotnet/api/system.valuetuple) .

> Note: The above encoding is invertible and the substitution of types for type variables
preserves this inversion. This means, among other things, that the F# reflection library
can correctly report tuple types based on runtime System.Type and System.ValueTuple values. The inversion is
defined by:
<br>- For the runtime type `Tuple<ty1, ..., tyN>` when `n <= 7`, the corresponding F# tuple
    type is `ty1 * ... * tyN`
<br>- For the runtime type `Tuple<ty1, ..., Tuple<tyN>>` when `n = 8`, the corresponding F#
    tuple type is `ty1 * ... * ty8`
<br>- For the runtime type `Tuple<ty1, ..., ty7, ty8n>` , if `ty8n` corresponds to the F# tuple
    type `ty8 * ... * tyN`, then the corresponding runtime type is `ty1 * ... * tyN`.<br>Runtime types of other forms do not have a corresponding tuple type. In particular,
runtime types that are instantiations of the eight-tuple type `Tuple<_, _, _, _, _, _, _, _ >`
must always have `Tuple<_>` in the final position. Syntactic types that have some other
form of type in this position are not permitted, and if such an instantiation occurs in F#
code or CLI library metadata that is referenced by F# code, an F# implementation may
report an error.

### List Expressions

An expression of the form `[expr1 ; ...; exprn]` is a _list expression_. The initial type of the expression is
asserted to be `FSharp.Collections.List<ty>` for a fresh type `ty`.

If `ty` is a named type, each expression `expri` is checked using a fresh type `ty'` as its initial type, with
the constraint `ty' :> ty`. Otherwise, each expression `expri` is checked using `ty` as its initial type.

List expressions elaborate to uses of `FSharp.Collections.List<_>` as
`op_Cons(expr1 ,(op_Cons(_expr2 ... op_Cons(exprn, op_Nil) ...)` where `op_Cons` and `op_Nil` are the
union cases with symbolic names `::` and `[]` respectively.

### Array Expressions

An expression of the form `[|expr1; ...; exprn|]` is an _array expression_. The initial type of the
expression is asserted to be `ty[]` for a fresh type `ty`.

If this assertion determines that `ty` is a named type, each expression `expri` is checked using a fresh
type `ty'` as its initial type, with the constraint `ty' :> ty`. Otherwise, each expression `expri` is
checked using `ty` as its initial type.

Array expressions are a primitive elaborated form.

> Note: The F# implementation ensures that large arrays of constants of type `bool`, `char`,
`byte`, `sbyte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, and `uint64` are compiled to an efficient
binary representation based on a call to
`System.Runtime.CompilerServices.RuntimeHelpers.InitializeArray`.

### Record Expressions

An expression of the form `{field-initializer1; ... ; field-initializern}` is a _record
construction expression_. For example:

```fsharp
type Data = { Count : int; Name : string }
let data1 = { Count = 3; Name = "Hello"; }
let data2 = { Name = "Hello"; Count= 3 }
```

In the following example, `data4` uses a long identifier to indicate the relevant field:

```fsharp
module M =
    type Data = { Age : int; Name : string; Height : float }

let data3 = { M.Age = 17; M.Name = "John"; M.Height = 186.0 }
let data4 = { data3 with M.Name = "Bill"; M.Height = 176.0 }
```

Fields may also be referenced by using the name of the containing type:

```fsharp
module M2 =
    type Data = { Age : int; Name : string; Height : float }

let data5 = { M2.Data.Age = 17; M2.Data.Name = "John"; M2.Data.Height = 186.0 }
let data6 = { data5 with M2.Data.Name = "Bill"; M2.Data.Height=176.0 }

open M2
let data7 = { Data.Age = 17; Data.Name = "John"; Data.Height = 186.0 }
let data8 = { data5 with Data.Name = "Bill"; Data.Height=176.0 }
```

Each `field-initializeri` has the form `field-labeli = expri`. Each `field-labeli` is a `long-ident`,
which must resolve to a field `F` i in a unique record type `R` as follows:

- If `field-labeli` is a single identifier `fld` and the initial type is known to be a record type
    `R<_, ..., _>` that has field `Fi` with name `fld`, then the field label resolves to `Fi`.
- If `field-labeli` is not a single identifier or if the initial type is a variable type, then the field label
    is resolved by performing _Field Label Resolution_ (see [§](inference-procedures.md#name-resolution)) on `field-labeli`. This procedure
    results in a set of fields `FSeti`. Each element of this set has a corresponding record type, thus
    resulting in a set of record types `RSeti`. The intersection of all `RSeti` must yield a single record
    type `R`, and each field then resolves to the corresponding field in `R`.
    The set of fields must be complete. That is, each field in record type `R` must have exactly one
    field definition. Each referenced field must be accessible (see [§](namespaces-and-modules.md#accessibility-annotations)), as must the type `R`.

After all field labels are resolved, the overall record expression is asserted to be of type
`R<ty1, ..., tyN>` for fresh types `ty1, ..., tyN`. Each `expri` is then checked in turn. The initial type is
determined as follows:

1. Assume the type of the corresponding field `Fi` in `R<ty1, ..., tyN>` is `ftyi`
2. If the type of `Fi` prior to taking into account the instantiation `<ty1, ..., tyN>` is a named type, then
    the initial type is a fresh type inference variable `fty'i` with a constraint `fty'i :> ftyi`.
3. Otherwise the initial type is `ftyi`.

Primitive record constructions are an elaborated form in which the fields appear in the same order
as in the record type definition. Record expressions themselves elaborate to a form that may
introduce local value definitions to ensure that expressions are evaluated in the same order that the
field definitions appear in the original expression. For example:

```fsharp
type R = {b : int; a : int }
{ a = 1 + 1; b = 2 }
```

The expression on the last line elaborates to `let v = 1 + 1 in { b = 2; a = v }`.

Records expressions are also used for object initializations in additional object constructor
definitions ([§](type-definitions.md#additional-object-constructors-in-classes)). For example:

```fsharp
type C =
    val x : int
    val y : int
    new() = { x = 1; y = 2 }
```

> Note: The following record initialization form is deprecated:
<br>`{ new type with Field1 = expr1 and ... and Fieldn = exprn }`
<br>The F# implementation allows the use of this form only with uppercase identifiers.
<br>F# code should not use this expression form. A future version of the F# language will
issue a deprecation warning.

### Copy-and-update Record Expressions

A _copy-and-update record expression_ has the following form:

```fsgrammar
{ expr with field-initializers }
```

where `field-initializers` is of the following form:

```fsgrammar
field-label1 = expr1; ...; field-labeln = exprn
```

Each `field-labeli` is a `long-ident`. In the following example, `data2` is defined by using such an
expression:

```fsharp
type Data = { Age : int; Name : string; Height : float }
let data1 = { Age = 17; Name = "John"; Height = 186.0 }
let data2 = { data1 with Name = "Bill"; Height = 176.0 }
```

The expression `expr` is first checked with the same initial type as the overall expression. Next, the
field definitions are resolved by using the same technique as for record expressions. Each field label
must resolve to a field `Fi` in a single record type `R` , all of whose fields are accessible. After all field
labels are resolved, the overall record expression is asserted to be of type `R<ty1, ..., tyN>` for fresh
types `ty1, ..., tyN`. Each `expri` is then checked in turn with initial type that results from the following
procedure:

1. Assume the type of the corresponding field `Fi` in `R<ty1, ..., tyN>` is `ftyi`.
2. If the type of `Fi` before considering the instantiation `<ty1, ..., tyN>` is a named type, then the
    initial type is a fresh type inference variable `fty'i` with a constraint `fty'i :> ftyi`.
3. Otherwise, the initial type is `ftyi`.

A copy-and-update record expression elaborates as if it were a record expression written as follows:

`let v = expr in { field-label1 = expr1; ...; field-labeln = exprn; F1 = v.F1; ...; FM = v.FM }`
where `F1 ... FM` are the fields of `R` that are not defined in `field-initializers` and `v` is a fresh
variable.

### Function Expressions

An expression of the form `fun pat1 ... patn -> expr` is a _function expression_. For example:

```fsharp
(fun x -> x + 1)
(fun x y -> x + y)
(fun [x] -> x) // note, incomplete match
(fun (x,y) (z,w) -> x + y + z + w)
```

Function expressions that involve only variable patterns are a primitive elaborated form. Function
expressions that involve non-variable patterns elaborate as if they had been written as follows:

```fsharp
fun v1 ... vn ->
    let pat1 = v 1
    ...
    let patn = vn
    expr
```

No pattern matching is performed until all arguments have been received. For example, the
following does not raise a `MatchFailureException` exception:

```fsharp
let f = fun [x] y -> y
let g = f [] // ok
```

However, if a third line is added, a `MatchFailureException` exception is raised:

```fsharp
let z = g 3 // MatchFailureException is raised
```

### Object Expressions

An expression of the following form is an _object expression_ :

```fsharp
{ new ty0 args-expr? object-members
  interface ty1 object-members1
  ...
  interface tyn object-membersn }
```

In the case of the interface declarations, the `object-members` are optional and are considered empty
if absent. Each set of `object-members` has the form:

```fsgrammar
with member-defns end?
```

Lexical filtering inserts simulated `$end` tokens when lightweight syntax is used.

Each member of an object expression members can use the keyword `member`, `override`, or `default`.
The keyword `member` can be used even when overriding a member or implementing an interface.

For example:

```fsharp
let obj1 =
    { new System.Collections.Generic.IComparer<int> with
        member x.Compare(a,b) = compare (a % 7) (b % 7) }

let obj2 =
    { new System.Object() with
        member x.ToString () = "Hello" }

let obj3 =
    { new System.Object() with
        member x.ToString () = "Hello, base.ToString() = " + base.ToString() }

let obj4 =
    { new System.Object() with
        member x.Finalize() = printfn "Finalize";
    interface System.IDisposable with
        member x.Dispose() = printfn "Dispose"; }
```

An object expression can specify additional interfaces beyond those required to fulfill the abstract
slots of the type being implemented. For example, `obj4` in the preceding examples has static type
`System.Object` but the object additionally implements the interface `System.IDisposable`. The
additional interfaces are not part of the static type of the overall expression, but can be revealed
through type tests.

Object expressions are statically checked as follows.

1. First, `ty0` to `tyn` are checked to verify that they are named types. The overall type of the
expression is `ty0` and is asserted to be equal to the initial type of the expression. However, if `ty0`
is type equivalent to `System.Object` and `ty1` exists, then the overall type is instead `ty1`.

2. The type `ty0` must be a class or interface type. The base construction argument `args-expr` must
    appear if and only if `ty0` is a class type. The type must have one or more accessible constructors;
    the call to these constructors is resolved and elaborated using _Method Application Resolution_
    (see [§](inference-procedures.md#method-application-resolution)). Except for `ty0`, each `tyi` must be an interface type.
3. The F# compiler attempts to associate each member with a unique _dispatch slot_ by using
    _dispatch slot inference_ ([§](inference-procedures.md#dispatch-slot-inference)). If a unique matching dispatch slot is found, then the argument
    types and return type of the member are constrained to be precisely those of the dispatch slot.
4. The arguments, patterns, and expressions that constitute the bodies of all implementing
    members are next checked one by one to verify the following:
    - For each member, the “this” value for the member is in scope and has type `ty0`.
    - Each member of an object expression can initially access the protected members of `ty0`.
    - If the variable `base-ident` appears, it must be named `base`, and in each member a base
       variable with this name is in scope. Base variables can be used only in the member
       implementations of an object expression, and are subject to the same limitations as byref
       values described in [§](inference-procedures.md#byref-safety-analysis).

The object must satisfy _dispatch slot checking_ ([§](inference-procedures.md#dispatch-slot-checking)) which ensures that a one-to-one mapping
exists between dispatch slots and their implementations.

Object expressions elaborate to a primitive form. At execution, each object expression creates an
object whose runtime type is compatible with all of the `tyi` that have a dispatch map that is the
result of _dispatch slot checking_ ([§](inference-procedures.md#dispatch-slot-checking)).

The following example shows how to both implement an interface and override a method from
`System.Object`. The overall type of the expression is `INewIdentity`.

```fsharp
type public INewIdentity =
    abstract IsAnonymous : bool

let anon =
{ new System.Object() with
    member i.ToString() = "anonymous"
  interface INewIdentity with
    member i.IsAnonymous = true }
```

### Delayed Expressions

An expression of the form `lazy expr` is a _delayed expression_. For example:

```fsharp
lazy (printfn "hello world")
```

is syntactic sugar for

```fsharp
new System.Lazy (fun () -> expr )
```

The behavior of the `System.Lazy` library type ensures that expression `expr` is evaluated on demand in
response to a `.Value` operation on the lazy value.

### Computation Expressions

The following expression forms are all _computation expressions_ :

```fsgrammar
expr { for ... }
expr { let ... }
expr { let! ... }
expr { use ... }
expr { while ... }
expr { yield ... }
expr { yield! ... }
expr { try ... }
expr { return ... }
expr { return! ... }
expr { match! ... }
```

More specifically, computation expressions have the following form:

```fsgrammar
builder-expr { cexpr }
```

where `cexpr` is, syntactically, the grammar of expressions with the additional constructs that are
defined in `comp-expr`. Computation expressions are used for sequences and other non-standard
interpretations of the F# expression syntax. For a fresh variable `b`, the expression

```fsgrammar
builder-expr { cexpr }
```

translates to

```fsgrammar
let b = builder-expr in {| cexpr |}C
```

The type of `b` must be a named type after the checking of builder-expr. The subscript indicates that
custom operations (`C`) are acceptable but are not required.

If the inferred type of `b` has one or more of the `Run`, `Delay`, or `Quote` methods when `builder-expr` is
checked, the translation involves those methods. For example, when all three methods exist, the
same expression translates to:

```fsgrammar
let b = builder-expr in b.Run (<@ b.Delay(fun () -> {| cexpr |}C) >@)
```

If a `Run` method does not exist on the inferred type of b, the call to `Run` is omitted. Likewise, if no
`Delay` method exists on the type of `b`, that call and the inner lambda are omitted, so the expression
translates to the following:

```fsgrammar
let b = builder-expr in b.Run (<@ {| cexpr |}C >@)
```

Similarly, if a `Quote` method exists on the inferred type of `b`, at-signs `<@ @>` are placed around `{| cexpr |}C`
or `b.Delay(fun () -> {| cexpr |}C)` if a `Delay` method also exists.

The translation `{| cexpr |}C` , which rewrites computation expressions to core language expressions,
is defined recursively according to the following rules:

`{| cexpr |}C = T (cexpr, [], fun v -> v, true)`

During the translation, we use the helper function {| cexpr |}0 to denote a translation that does not
involve custom operations:

`{| cexpr |}0 = T (cexpr, [], fun v -> v, false)`

```fsgrammar
T (e, V , C , q) where e : the computation expression being translated
                       V : a set of scoped variables
                       C : continuation (or context where “e” occurs,
                           up to a hole to be filled by the result of translating “e”)
                       q : Boolean that indicates whether a custom operator is allowed
```

<!-- start of weird section -->

Then, T is defined for each computation expression e:

**T** (let p = e in ce, **V** , **C** , q) = **T** (ce, **V**  `var` (p), v. **C** (let p = e in v), q)

**T** (let! p = e in ce, **V** , **C** , q) = **T** (ce, **V**  `var` (p), v. **C** (b.Bind( `src` (e),fun p -> v), q)

**T** (yield e, **V** , **C** , q) = **C** (b.Yield(e))

**T** (yield! e, **V** , **C** , q) = **C** (b.YieldFrom( `src` (e)))

**T** (return e, **V** , **C** , q) = **C** (b.Return(e))

**T** (return! e, **V** , **C** , q) = **C** (b.ReturnFrom( `src` (e)))

**T** (use p = e in ce, **V** , **C** , q) = **C** (b.Using(e, fun p -> {| `ce` |} 0 ))

**T** (use! p = e in ce, **V** , **C** , q) = **C** (b.Bind( `src` (e), fun p -> b.Using(p, fun p -> {| `ce` |} 0 ))

**T** (match e with pi - > cei, **V** , **C** , q) = **C** (match e with pi - > {| `ce` i |} 0 )

**T** (match! e with pi - > cei, **V** , **C** , q) = **C** (let! p = e in match p with pi - > {| `ce` i |} 0 )

**T** (while e do ce, **V** , **C** , q) = **T** (ce, **V** , v. **C** (b.While(fun () -> e, b.Delay(fun () -> v))), q)

**T** (try ce with pi - > cei, **V** , **C** , q) =
Assert(not q); **C** (b.TryWith(b.Delay(fun () -> {| `ce` |} 0 ), fun pi - > {| `ce` i |} 0 ))

**T** (try ce finally e, **V** , **C** , q) =
Assert(not q); **C** (b.TryFinally(b.Delay(fun () -> {| `ce` |} 0 ), fun () -> e))

**T** (if e then ce, **V** , **C** , q) = **T** (ce, **V** , v. **C** (if e then v else b.Zero()), q)

**T** (if e then ce1 else ce2 , **V** , **C** , q) = Assert(not q); **C** (if e then {| `ce` 1 |} 0 ) else {| `ce` 2 |} 0 )

**T** (for x = e1 to e2 do ce, **V** , **C** , q) = **T** (for x in e1 .. e2 do ce, **V** , **C** , q)

**T** (for p1 in e1 do joinOp p2 in e2 onWord (e3 `eop` e4 ) ce, **V** , **C** , q) =
Assert(q); **T** (for `pat` ( **V** ) in b.Join( `src` (e1 ), `src` (e2 ), p1 .e3 , p2 .e4 ,
p1. p2 .(p1 ,p2 )) do ce, **V** , **C** , q)

**T** (for p1 in e1 do groupJoinOp p2 in e2 onWord (e3 `eop` e4) into p3 ce, **V** , **C** , q) =
Assert(q); **T** (for `pat` ( **V** ) in b.GroupJoin( `src` (e1),
`src` (e2), p1.e3, p2.e4, p1. p3.(p1,p3)) do ce, **V** , **C** , q)

**T** (for x in e do ce, **V** , **C** , q) = **T** (ce, **V**  {x}, v. **C** (b.For( `src` (e), fun x -> v)), q)

**T** (do e in ce, **V** , **C** , q) = **T** (ce, **V** , v. **C** (e; v), q)

**T** (do! e in ce, **V** , **C** , q) = **T** (let! () = e in ce, **V** , **C** , q)

**T** (joinOp p2 in e2 on (e3 `eop` e4) ce, **V** , **C** , q) =
**T** (for `pat` ( **V** ) in **C** ({| yield `exp` ( **V** ) |}0) do join p2 in e2 onWord (e3 `eop` e4) ce, **V** , v.v, q)

**T** (groupJoinOp p2 in e2 onWord (e3 eop e4) into p3 ce, **V** , **C** , q) =
**T** (for `pat` ( **V** ) in **C** ({| yield `exp` ( **V** ) |}0) do groupJoin p2 in e2 on (e3 `eop` e4) into p3 ce,
**V** , v.v, q)

**T** ([<CustomOperator("Cop")>]cop arg, **V** , **C** , q) = Assert (q); [| cop arg, **C** (b.Yield `exp` ( **V** )) |] **V**

**T** ([<CustomOperator("Cop", MaintainsVarSpaceUsingBind=true)>]cop arg; e, **V** , **C** , q) =
Assert (q); **CL** (cop arg; e, **V** , **C** (b.Return `exp` ( **V** )), false)

**T** ([<CustomOperator("Cop")>]cop arg; e, **V** , **C** , q) =
Assert (q); **CL** (cop arg; e, **V** , **C** (b.Yield `exp` ( **V** )), false)

**T** (ce1; ce2, **V** , **C** , q) = **C** (b.Combine({| ce1 |}0, b.Delay(fun () -> {| ce2 |}0)))

**T** (do! e;, **V** , **C** , q) = **T** (let! () = `src` (e) in b.Return(), **V** , **C** , q)

**T** (e;, **V** , **C** , q) = **C** (e;b.Zero())

The following notes apply to the translations:

- The lambda expression (fun f x -> b) is represented by x.b.
- The auxiliary function var (p) denotes a set of variables that are introduced by a pattern p. For
    example:
    var(x) = {x}, var((x,y)) = {x,y} or var(S (x,y)) = {x,y}
    where S is a type constructor.
-  is an update operator for a set V to denote extended variable spaces. It updates the existing
    variables. For example, {x,y}  var((x,z)) becomes {x,y,z} where the second x replaces the
    first x.
- The auxiliary function pat ( **V** ) denotes a pattern tuple that represents a set of variables in **V**. For
    example, pat({x,y}) becomes (x,y), where x and y represent pattern expressions.
- The auxiliary function exp ( **V** ) denotes a tuple expression that represents a set of variables in **V**.
    For example, exp ({x,y}) becomes (x,y), where x and y represent variable expressions.

- The auxiliary function src (e) denotes b.Source(e) if the innermost ForEach is from the user
    code instead of generated by the translation, and a builder b contains a Source method.
    Otherwise, src (e) denotes e.
- Assert() checks whether a custom operator is allowed. If not, an error message is reported.
    Custom operators may not be used within try/with, try/finally, if/then/else, use, match, or
    sequential execution expressions such as (e1;e2). For example, you cannot use if/then/else in
    any computation expressions for which a builder defines any custom operators, even if the
    custom operators are not used.
- The operator eop denotes one of =, ?=, =? or ?=?.
- joinOp and onWord represent keywords for join-like operations that are declared in
    CustomOperationAttribute. For example, [<CustomOperator("join", IsLikeJoin=true,
    JoinConditionWord="on")>] declares “join” and “on”.
- Similarly, groupJoinOp represents a keyword for groupJoin-like operations, declared in
    CustomOperationAttribute. For example, [<CustomOperator("groupJoin",
    IsLikeGroupJoin=true, JoinConditionWord="on")>] declares “groupJoin” and “on”.
- The auxiliary translation **CL** is defined as follows:

```fsgrammar
CL (e1, V, e2, bind) where e1: the computation expression being translated
V : a set of scoped variables
e2 : the expression that will be translated after e1 is done
bind: indicator if it is for Bind (true) or iterator (false).
```

The following shows translations for the uses of CL in the preceding computation expressions:

```fsgrammar
CL (cop arg, V , e’, bind) = [| cop arg, e’ |] V
CL ([<MaintainsVariableSpaceUsingBind=true>]cop arg into p; e, V , e’, bind) =
T (let! p = e’ in e, [], v.v, true)
CL (cop arg into p; e, V , e’, bind) = T (for p in e’ do e, [], v.v, true)
CL ([<MaintainsVariableSpace=true>]cop arg; e, V , e’, bind) =
CL (e, V , [| cop arg, e’ |] V , true)
CL ([<MaintainsVariableSpaceUsingBind=true>]cop arg; e, V , e’, bind) =
CL (e, V , [| cop arg, e’ |] V , true)
CL (cop arg; e, V , e’, bind) = CL (e, [], [| cop arg, e’ |] V , false)
CL (e, V , e’, true) = T (let! pat ( V ) = e’ in e, V , v.v, true)
CL (e, V , e’, false) = T (for pat ( V ) in e’ do e, V , v.v, true)
```

- The auxiliary translation [| e1, e2 |]V is defined as follows:

[|[ e1, e2 |] **V** where e1: the custom operator available in a build
e2 : the context argument that will be passed to a custom operator
**V** : a list of bound variables

```fsgrammar
[|[<CustomOperator(" Cop")>] cop [<ProjectionParameter>] arg, e |] V =
b.Cop (e, fun pat ( V) - > arg)
[|[<CustomOperator("Cop")>] cop arg, e |] V = b.Cop (e, arg)
```

- The final two translation rules (for do! e; and do! e;) apply only for the final expression in the
    computation expression. The semicolon (;) can be omitted.

The following attributes specify custom operations:

- `CustomOperationAttribute` indicates that a member of a builder type implements a custom
    operation in a computation expression. The attribute has one parameter: the name of the
    custom operation. The operation can have the following properties:
  - `MaintainsVariableSpace` indicates that the custom operation maintains the variable space of
       a computation expression.
  - `MaintainsVariableSpaceUsingBind` indicates that the custom operation maintains the
       variable space of a computation expression through the use of a bind operation.
  - `AllowIntoPattern` indicates that the custom operation supports the use of ‘into’ immediately
       following the operation in a computation expression to consume the result of the operation.
  - `IsLikeJoin` indicates that the custom operation is similar to a join in a sequence
       computation, which supports two inputs and a correlation constraint.
  - `IsLikeGroupJoin` indicates that the custom operation is similar to a group join in a sequence
       computation, which support two inputs and a correlation constraint, and generates a group.
  - `JoinConditionWord` indicates the names used for the ‘on’ part of the custom operator for
       join-like operators.
- `ProjectionParameterAttribute` indicates that, when a custom operation is used in a
    computation expression, a parameter is automatically parameterized by the variable space of
    the computation expression.

<!-- end of weird section -->

The following examples show how the translation works. Assume the following simple sequence
builder:

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Yield (item:'a) : seq<'a> = seq { yield item }

let myseq = SimpleSequenceBuilder()
```

Then, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    yield i*i
    }
```

translates to

```fsharp
let b = myseq
b.For([1..10], fun i ->
    b.Yield(i*i))
```

`CustomOperationAttribute` allows us to define custom operations. For example, the simple sequence
builder can have a custom operator, “where”:

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Yield (item:'a) : seq<'a> = seq { yield item }
    [<CustomOperation("where")>]
    member __.Where (source : seq<'a>, f: 'a -> bool) : seq<'a> = Seq.filter f source

let myseq = SimpleSequenceBuilder()
```

Then, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    where (fun x -> x > 5)
    }
```

translates to

```fsharp
let b = myseq
    b.Where(
        b.For([1..10], fun i ->
            b.Yield (i)),
        fun x -> x > 5)
```

`ProjectionParameterAttribute` automatically adds a parameter from the variable space of the
computation expression. For example, `ProjectionParameterAttribute` can be attached to the second
argument of the `where` operator:

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Yield (item:'a) : seq<'a> = seq { yield item }
    [<CustomOperation("where")>]
    member __.Where (source: seq<'a>, [<ProjectionParameter>]f: 'a -> bool) : seq<'a> =
        Seq.filter f source

let myseq = SimpleSequenceBuilder()
```

Then, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    where (i > 5)
    }
```

translates to

```fsharp
let b = myseq
b.Where(
    b.For([1..10], fun i ->
        b.Yield (i)),
    fun i -> i > 5)
```

`ProjectionParameterAttribute` is useful when a let binding appears between `ForEach` and the
custom operators. For example, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    let j = i * i
    where (i > 5 && j < 49)
    }
```

translates to

```fsharp
let b = myseq
b.Where(
    b.For([1..10], fun i ->
        let j = i * i
        b.Yield (i,j)),
    fun (i,j) -> i > 5 && j < 49)
```

Without `ProjectionParameterAttribute`, a user would be required to write “`fun (i,j) ->`” explicitly.

Now, assume that we want to write the condition “`where (i > 5 && j < 49)`” in the following
syntax:

```fsharp
where (i > 5)
where (j < 49)
```

To support this style, the `where` custom operator should produce a computation that has the same
variable space as the input computation. That is, `j` should be available in the second `where`. The
following example uses the `MaintainsVariableSpace` property on the custom operator to specify this
behavior:

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Yield (item:'a) : seq<'a> = seq { yield item }
    [<CustomOperation("where", MaintainsVariableSpace=true)>]
    member __.Where (source: seq<'a>, [<ProjectionParameter>]f: 'a -> bool) : seq<'a> =
        Seq.filter f source

let myseq = SimpleSequenceBuilder()
```

Then, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    let j = i * i
    where (i > 5)
    where (j < 49)
    }
```

translates to

```fsharp
let b = myseq
b.Where(
    b.Where(
        b.For([1..10], fun i ->
            let j = i * i
            b.Yield (i,j)),
        fun (i,j) -> i > 5),
    fun (i,j) -> j < 49)
```

When we may not want to produce the variable space but rather want to explicitly express the chain
of the `where` operator, we can design this simple sequence builder in a slightly different way. For
example, we can express the same expression in the following way:

```fsharp
myseq {
    for i in 1 .. 10 do
    where (i > 5) into j
    where (j*j < 49)
    }
```

In this example, instead of having a let-binding (for `j` in the previous example) and passing variable
space (including `j`) down to the chain, we can introduce a special syntax that captures a value into a
pattern variable and passes only this variable down to the chain, which is arguably more readable.
For this case, `AllowIntoPattern` allows the custom operation to have an `into` syntax:

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Yield (item:'a) : seq<'a> = seq { yield item }

    [<CustomOperation("where", AllowIntoPattern=true)>]
    member __.Where (source: seq<'a>, [<ProjectionParameter>]f: 'a -> bool) : seq<'a> =
        Seq.filter f source

let myseq = SimpleSequenceBuilder()
```

Then, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    where (i > 5) into j
    where (j*j < 49)
    }
```

translates to

```fsharp
let b = myseq
b.Where(
    b.For(
        b.Where(
            b.For([1..10], fun i -> b.Yield (i))
            fun i -> i>5),
        fun j -> b.Yield (j)),
    fun j -> j*j < 49)
```

Note that the `into` keyword is not customizable, unlike `join` and `on`.

In addition to `MaintainsVariableSpace`, `MaintainsVariableSpaceUsingBind` is provided to pass
variable space down to the chain in a different way. For example:

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Return (item:'a) : seq<'a> = seq { yield item }
    member __.Bind (value , cont) = cont value
    [<CustomOperation("where", MaintainsVariableSpaceUsingBind=true, AllowIntoPattern=true)>]
    member __.Where (source: seq<'a>, [<ProjectionParameter>]f: 'a -> bool) : seq<'a> =
        Seq.filter f source

let myseq = SimpleSequenceBuilder()
```

The presence of `MaintainsVariableSpaceUsingBindAttribute` requires `Return` and `Bind` methods
during the translation.

Then, the expression

```fsharp
myseq {
    for i in 1 .. 10 do
    where (i > 5 && i*i < 49) into j
    return j
    }
```

translates to

```fsharp
let b = myseq
b.Bind(
    b.Where(B.For([1..10], fun i -> b.Return (i)),
        fun i -> i > 5 && i*i < 49),
    fun j -> b.Return (j))
```

where `Bind` is called to capture the pattern variable `j`. Note that `For` and `Yield` are called to capture
the pattern variable when `MaintainsVariableSpace` is used.

Certain properties on the `CustomOperationAttribute` introduce join-like operators. The following
example shows how to use the `IsLikeJoin` property.

```fsharp
type SimpleSequenceBuilder() =
    member __.For (source : seq<'a>, body : 'a -> seq<'b>) =
        seq { for v in source do yield! body v }
    member __.Yield (item:'a) : seq<'a> = seq { yield item }
    [<CustomOperation("merge", IsLikeJoin=true, JoinConditionWord="whenever")>]
    member __.Merge (src1:seq<'a>, src2:seq<'a>, ks1, ks2, ret) =
        seq { for a in src1 do
            for b in src2 do
            if ks1 a = ks2 b then yield((ret a ) b)
        }

let myseq = SimpleSequenceBuilder()
```

`IsLikeJoin` indicates that the custom operation is similar to a join in a sequence computation; that
is, it supports two inputs and a correlation constraint.

The expression

```fsharp
myseq {
    for i in 1 .. 10 do
    merge j in [5 .. 15] whenever (i = j)
    yield j
    }
```

translates to

```fsharp
let b = myseq
b.For(
    b.Merge([1..10], [5..15],
            fun i -> i, fun j -> j,
            fun i -> fun j -> (i,j)),
    fun j -> b.Yield (j))
```

This translation implicitly places type constraints on the expected form of the builder methods. For
example, for the `async` builder found in the `FSharp.Control` library, the translation phase
corresponds to implementing a builder of a type that has the following member signatures:

```fsharp
type AsyncBuilder with
    member For: seq<'T> * ('T -> Async<unit>) -> Async<unit>
    member Zero : unit -> Async<unit>
    member Combine : Async<unit> * Async<'T> -> Async<'T>
    member While : (unit -> bool) * Async<unit> -> Async<unit>
    member Return : 'T -> Async<'T>
    member Delay : (unit -> Async<'T>) -> Async<'T>
    member Using: 'T * ('T -> Async<'U>) -> Async<'U>
        when 'U :> System.IDisposable
    member Bind: Async<'T> * ('T -> Async<'U>) -> Async<'U>
    member TryFinally: Async<'T> * (unit -> unit) -> Async<'T>
    member TryWith: Async<'T> * (exn -> Async<'T>) -> Async<'T>
```

The following example shows a common approach to implementing a new computation expression
builder for a monad. The example uses computation expressions to define computations that can be
partially run by executing them step-by-step, for example, up to a time limit.

```fsharp
/// Computations that can cooperatively yield by returning a continuation
type Eventually<'T> =
    | Done of 'T
    | NotYetDone of (unit -> Eventually<'T>)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Eventually =

    /// The bind for the computations. Stitch 'k' on to the end of the computation.
    /// Note combinators like this are usually written in the reverse way,
    /// for example,
    /// e |> bind k
    let rec bind k e =
        match e with
        | Done x -> NotYetDone (fun () -> k x)
        | NotYetDone work -> NotYetDone (fun () -> bind k (work()))

    /// The return for the computations.
    let result x = Done x

    type OkOrException<'T> =
        | Ok of 'T
        | Exception of System.Exception

    /// The catch for the computations. Stitch try/with throughout
    /// the computation and return the overall result as an OkOrException.
    let rec catch e =
        match e with
        | Done x -> result (Ok x)
        | NotYetDone work ->
            NotYetDone (fun () ->
                let res = try Ok(work()) with | e -> Exception e
                match res with
                | Ok cont -> catch cont // note, a tailcall
                | Exception e -> result (Exception e))

    /// The delay operator.
    let delay f = NotYetDone (fun () -> f())

    /// The stepping action for the computations.
    let step c =
        match c with
        | Done _ -> c
        | NotYetDone f -> f ()

    // The rest of the operations are boilerplate.

    /// The tryFinally operator.
    /// This is boilerplate in terms of "result", "catch" and "bind".
    let tryFinally e compensation =
        catch (e)
        |> bind (fun res ->
            compensation();
            match res with
            | Ok v -> result v
            | Exception e -> raise e)

    /// The tryWith operator.
    /// This is boilerplate in terms of "result", "catch" and "bind".
    let tryWith e handler =
        catch e
        |> bind (function Ok v -> result v | Exception e -> handler e)

    /// The whileLoop operator.
    /// This is boilerplate in terms of "result" and "bind".
    let rec whileLoop gd body =
        if gd() then body |> bind (fun v -> whileLoop gd body)
        else result ()

    /// The sequential composition operator
    /// This is boilerplate in terms of "result" and "bind".
    let combine e1 e2 =
        e1 |> bind (fun () -> e2)

    /// The using operator.
    let using (resource: #System.IDisposable) f =
        tryFinally (f resource) (fun () -> resource.Dispose())

    /// The forLoop operator.
    /// This is boilerplate in terms of "catch", "result" and "bind".
    let forLoop (e:seq<_>) f =
        let ie = e.GetEnumerator()
        tryFinally (whileLoop (fun () -> ie.MoveNext())
                              (delay (fun () -> let v = ie.Current in f v)))
                   (fun () -> ie.Dispose())

// Give the mapping for F# computation expressions.
type EventuallyBuilder() =
    member x.Bind(e,k) = Eventually.bind k e
    member x.Return(v) = Eventually.result v
    member x.ReturnFrom(v) = v
    member x.Combine(e1,e2) = Eventually.combine e1 e2
    member x.Delay(f) = Eventually.delay f
    member x.Zero() = Eventually.result ()
    member x.TryWith(e,handler) = Eventually.tryWith e handler
    member x.TryFinally(e,compensation) = Eventually.tryFinally e compensation
    member x.For(e:seq<_>,f) = Eventually.forLoop e f
    member x.Using(resource,e) = Eventually.using resource e

let eventually = new EventuallyBuilder()
```

After the computations are defined, they can be built by using eventually { ... }:

```fsharp
let comp =
    eventually {
        for x in 1 .. 2 do
            printfn " x = %d" x
        return 3 + 4 }
```

These computations can now be stepped. For example:

```fsharp
let step x = Eventually.step x
    comp |> step
// returns "NotYetDone <closure>"

comp |> step |> step
// prints "x = 1"
// returns "NotYetDone <closure>"

comp |> step |> step |> step |> step |> step |> step
// prints "x = 1"
// prints "x = 2"
// returns “NotYetDone <closure>”

comp |> step |> step |> step |> step |> step |> step |> step |> step
// prints "x = 1"
// prints "x = 2"
// returns "Done 7"
```

### Sequence Expressions

An expression in one of the following forms is a _sequence expression_ :

```fsgrammar
seq { comp-expr }
seq { short-comp-expr }
```

For example:

```fsharp
seq { for x in [ 1; 2; 3 ] do for y in [5; 6] do yield x + y }
seq { for x in [ 1; 2; 3 ] do yield x + x }
seq { for x in [ 1; 2; 3 ] -> x + x }
```

Logically speaking, sequence expressions can be thought of as computation expressions with a
builder of type `FSharp.Collections.SeqBuilder`. This type can be considered to be defined as
follows:

```fsharp
type SeqBuilder() =
    member x.Yield (v) = Seq.singleton v
    member x.YieldFrom (s:seq<_>) = s
    member x.Return (():unit) = Seq.empty
    member x.Combine (xs1,xs2) = Seq.append xs1 xs2
    member x.For (xs,g) = Seq.collect f xs
    member x.While (guard,body) = SequenceExpressionHelpers.EnumerateWhile guard body
    member x.TryFinally (xs,compensation) =
        SequenceExpressionHelpers.EnumerateThenFinally xs compensation
    member x.Using (resource,xs) = SequenceExpressionHelpers.EnumerateUsing resource xs
```

> Note that this builder type is not actually defined in the F# library. Instead, sequence expressions are
elaborated directly. For details, see page 79 of the old pdf spec.

<!-- Text skipped during conversion -->

### Range Expressions

Expressions of the following forms are _range expressions_.

```fsgrammar
{ e1 .. e2 }
{ e1 .. e2 .. e3 }
seq { e1 .. e2 }
seq { e1 .. e2 .. e3 }
```

Range expressions generate sequences over a specified range. For example:

```fsgrammar
seq { 1 .. 10 } // 1; 2; 3; 4; 5; 6; 7; 8; 9; 10
seq { 1 .. 2 .. 10 } // 1; 3; 5; 7; 9
```

Range expressions involving `expr1 .. expr2` are translated to uses of the `(..)` operator, and those
involving `expr1 .. expr1 .. expr3` are translated to uses of the `(.. ..)` operator:

```fsgrammar
seq { e1 .. e2 } → ( .. ) e1 e2
seq { e1 .. e2 .. e3 } → ( .. .. ) e1 e2 e3
```

The default definition of these operators is in `FSharp.Core.Operators`. The ( `..` ) operator generates
an `IEnumerable<_>` for the range of values between the start (`expr1`) and finish (`expr2`) values, using
an increment of 1 (as defined by `FSharp.Core.LanguagePrimitives.GenericOne`). The `(.. ..)`
operator generates an `IEnumerable<_>` for the range of values between the start (`expr1`) and finish
(`expr3`) values, using an increment of `expr2`.

The `seq` keyword, which denotes the type of computation expression, can be omitted for simple
range expressions, but this is not recommended and might be deprecated in a future release. It is
always preferable to explicitly mark the type of a computation expression.

Range expressions also occur as part of the translated form of expressions, including the following:

- `[ expr1 .. expr2 ]`
- `[| expr1 .. expr2 |]`
- `for var in expr1 .. expr2 do expr3`

A sequence iteration expression of the form `for var in expr1 .. expr2 do expr3 done` is sometimes
elaborated as a simple for loop-expression ([§](expressions.md#simple-for-loop-expressions)).

### Lists via Sequence Expressions

A _list sequence expression_ is an expression in one of the following forms

```fsgrammar
[ comp-expr ]
[ short-comp-expr ]
[ range-expr ]
```

In all cases `[ cexpr ]` elaborates to `FSharp.Collections.Seq.toList(seq { cexpr })`.

For example:

```fsharp
let x2 = [ yield 1; yield 2 ]
```

```fsharp
let x3 = [ yield 1
           if System.DateTime.Now.DayOfWeek = System.DayOfWeek.Monday then
               yield 2]
```

### Arrays Sequence Expressions

An expression in one of the following forms is an _array sequence expression_ :

```fsgrammar
[| comp-expr |]
[| short-comp-expr |]
[| range-expr |]
```

In all cases `[| cexpr |]` elaborates to `FSharp.Collections.Seq.toArray(seq { cexpr })`.

For example:

```fsharp
let x2 = [| yield 1; yield 2 |]
let x3 = [| yield 1
    if System.DateTime.Now.DayOfWeek = System.DayOfWeek.Monday then
        yield 2 |]
```

### Null Expressions

An expression in the form `null` is a _null expression_. A null expression imposes a nullness constraint
([§](types-and-type-constraints.md#nullness-constraints), [§](types-and-type-constraints.md#nullness)) on the initial type of the expression. The constraint ensures that the type directly
supports the value `null`.

Null expressions are a primitive elaborated form.

### 'printf' Formats

Format strings are strings with `%` markers as format placeholders. Format strings are analyzed at
compile time and annotated with static and runtime type information as a result of that analysis.
They are typically used with one of the functions `printf`, `fprintf`, `sprintf`, or `bprintf` in the
`FSharp.Core.Printf` module. Format strings receive special treatment in order to type check uses of
these functions more precisely.

More concretely, a constant string is interpreted as a printf-style format string if it is expected to
have the type `FSharp.Core.PrintfFormat<'Printer,'State,'Residue,'Result,'Tuple>`. The string is
statically analyzed to resolve the generic parameters of the `PrintfFormat type`, of which `'Printer`
and `'Tuple` are the most interesting:

- `'Printer` is the function type that is generated by applying a printf-like function to the format
    string.
- `'Tuple` is the type of the tuple of values that are generated by treating the string as a generator
    (for example, when the format string is used with a function similar to `scanf` in other
    languages).

A format placeholder has the following shape:

`%[flags][width][.precision][type]`

where:

`flags` are 0 , -, +, and the space character. The # flag is invalid and results in a compile-time error.

`width` is an integer that specifies the minimum number of characters in the result.

`precision` is the number of digits to the right of the decimal point for a floating-point type..

`type` is as shown in the following table.

| Placeholder string | Type |
| --- | --- |
| `%b` | `bool` |
| `%s` | `string` |
| `%c` | `char` |
| `%d, %i` | One of the basic integer types.</br>A basic integer type is `byte`, `sbyte`, `int16`, `uint16`, `int32`, `uint32`, `int64`, `uint64`, `nativeint`, `unativeint`, or one of these types with a unit of measure |
| `%u` | Basic integer type formatted as an unsigned integer |
| `%x` | Basic integer type formatted as an unsigned hexadecimal integer with lowercase letters a through f. |
| `%X` | Basic integer type formatted as an unsigned hexadecimal integer with uppercase letters A through F. |
| `%o` | Basic integer type formatted as an unsigned octal integer. |
| `%e, %E, %f, %F, %g, %G` | `float` or `float32`, possibly with a unit of measure|
| `%M` | `System.Decimal`, possibly with a unit of measure |
| `%O` | `System.Object`, possibly with a unit of measure |
| `%A` | Fresh variable type `'T` |
| `%a` | Formatter of type `'State -> 'T -> 'Residue` for a fresh variable type `'T` |
| `%t` | Formatter of type `'State -> 'Residue` |

For example, the format string "`%s %d %s`" is given the type `PrintfFormat<(string -> int -> string -> 'd), 'b, 'c, 'd, (string * int * string)>` for fresh variable types `'b`, `'c`, `'d`. Applying `printf`
to it yields a function of type `string -> int -> string -> unit`.

## Application Expressions

### Basic Application Expressions

Application expressions involve variable names, dot-notation lookups, function applications, method
applications, type applications, and item lookups, as shown in the following table.

| Expression | Description |
| --- | --- |
| `long-ident-or-op` | Long-ident lookup expression |
| `expr '.' long-ident-or-op` | Dot lookup expression |
| `expr expr` | Function or member application expression |
| `expr(expr)` | High precedence function or member application expression |
| `expr<types>` | Type application expression |
| `expr< >` | Type application expression with an empty type list |
| `type expr` | Simple object expression |

The following are examples of application expressions:

```fsharp
System.Math.PI
System.Math.PI.ToString()
(3 + 4).ToString()
System.Environment.GetEnvironmentVariable("PATH").Length
System.Console.WriteLine("Hello World")
```

Application expressions may start with object construction expressions that do not include the `new`
keyword:

```fsharp
System.Object()
System.Collections.Generic.List<int>(10)
System.Collections.Generic.KeyValuePair(3,"Three")
System.Object().GetType()
System.Collections.Generic.Dictionary<int,int>(10).[1]
```

If the `long-ident-or-op` starts with the special pseudo-identifier keyword `global`, F# resolves the
identifier with respect to the global namespace — that is, ignoring all `open` directives (see [§](inference-procedures.md#resolving-application-expressions)). For example:

```fsharp
global.System.Math.PI
```

is resolved to `System.Math.PI` ignoring all `open` directives.

The checking of application expressions is described in detail as an algorithm in [§](inference-procedures.md#resolving-application-expressions). To check an
application expression, the expression form is repeatedly decomposed into a _lead_ expression `expr`
and a list of projections `projs` through the use of _Unqualified Lookup_ ([§](inference-procedures.md#unqualified-lookup)). This in turn uses
procedures such as _Expression-Qualified Lookup_ and _Method Application Resolution_.

As described in [§](inference-procedures.md#resolving-application-expressions), checking an application expression results in an elaborated expression that
contains a series of lookups and method calls. The elaborated expression may include:

- Uses of named values
- Uses of union cases
- Record constructions
- Applications of functions
- Applications of methods (including methods that access properties)
- Applications of object constructors
- Uses of fields, both static and instance
- Uses of active pattern result elements

Additional constructs may be inserted when resolving method calls into simpler primitives:

- The use of a method or value as a first-class function may result in a function expression.

    For example, `System.Environment.GetEnvironmentVariable` elaborates to:
    `(fun v -> System.Environment.GetEnvironmentVariable(v))`
    for some fresh variable `v`.

- The use of post-hoc property setters results in the insertion of additional assignment and
    sequential execution expressions in the elaborated expression.

    For example, `new System.Windows.Forms.Form(Text="Text")` elaborates to
    `let v = new System.Windows.Forms.Form() in v.set_Text("Text"); v`
    for some fresh variable `v`.

- The use of optional arguments results in the insertion of `Some(_)` and `None` data constructions in
    the elaborated expression.

For uses of active pattern results (see [§](namespaces-and-modules.md#active-pattern-definitions-in-modules)), for result `i` in an active pattern that has `N` possible
results of types `types` , the elaborated expression form is a union case `ChoiceNOfi` of type
`FSharp.Core.Choice<types>`.

### Object Construction Expressions

An expression of the following form is an _object construction expression_:

```fsgrammar
new ty ( e1 ... en )
```

An object construction expression constructs a new instance of a type, usually by calling a
constructor method on the type. For example:

```fsharp
new System.Object()
new System.Collections.Generic.List<int>()
new System.Windows.Forms.Form (Text="Hello World")
new 'T()
```

The initial type of the expression is first asserted to be equal to `ty`. The type `ty` must not be an array,
record, union or tuple type. If `ty` is a named class or struct type:

- `ty` must not be abstract.
- If `ty` is a struct type, `n` is 0 , and `ty` does not have a constructor method that takes zero
    arguments, the expression elaborates to the default “zero-bit pattern” value for `ty`.
- Otherwise, the type must have one or more accessible constructors. The overloading between
    these potential constructors is resolved and elaborated by using _Method Application Resolution_
    (see [§](inference-procedures.md#method-application-resolution)).

If `ty` is a delegate type the expression is a _delegate implementation expression_.

- If the delegate type has an `Invoke` method that has the following signature

    `Invoke(ty1, ..., tyn) -> rtyA` ,

    then the overall expression must be in this form:

    `new ty(expr)` where `expr` has type `ty1 -> ... -> tyn -> rtyB`

    If type `rtyA` is a CLI void type, then `rtyB` is unit, otherwise it is `rtyA`.

- If any of the types `tyi` is a byref-type then an explicit function expression must be specified. That
    is, the overall expression must be of the form `new ty(fun pat1 ... patn -> exprbody)`.

If `ty` is a type variable:

- There must be no arguments (that is, `n = 0`).
- The type variable is constrained as follows:

    `ty : (new : unit -> ty )` -- CLI default constructor constraint

- The expression elaborates to a call to
    `FSharp.Core.LanguagePrimitives.IntrinsicFunctions.CreateInstance<ty>()`, which in turn calls
    `System.Activator.CreateInstance<ty>()`, which in turn uses CLI reflection to find and call the
    null object constructor method for type `ty`. On return from this function, any exceptions are
    wrapped by using `System.TargetInvocationException`.

### Operator Expressions

Operator expressions are specified in terms of their shallow syntactic translation to other constructs.
The following translations are applied in order:

```fsgrammar
infix-or-prefix-op e1 → (~infix-or-prefix-op) e1
prefix-op e1 → (prefix-op) e1
e1 infix-op e2 → (infix-op) e1 e2
```

> Note: When an operator that may be used as either an infix or prefix operator is used in
prefix position, a tilde character ~ is added to the name of the operator during the
translation process.

These rules are applied after applying the rules for dynamic operators ([§](expressions.md#dynamic-operator-expressions)).

The parenthesized operator name is then treated as an identifier and the standard rules for
unqualified name resolution ([§](inference-procedures.md#name-resolution)) in expressions are applied. The expression may resolve to a
specific definition of a user-defined or library-defined operator. For example:

```fsharp
let (+++) a b = (a,b)
3 +++ 4
```

In some cases, the operator name resolves to a standard definition of an operator from the F#
library. For example, in the absence of an explicit definition of (+),

```fsharp
3 + 4
```

resolves to a use of the infix operator FSharp.Core.Operators.(+).

Some operators that are defined in the F# library receive special treatment in this specification. In
particular:

- The `&expr` and `&&expr` address-of operators ([§](expressions.md#the-addressof-operators))
- The `expr && expr` and `expr || expr` shortcut control flow operators ([§](expressions.md#shortcut-operator-expressions))
- The `%expr` and `%%expr` expression splice operators in quotations ([§](expressions.md#expression-splices))
- The library-defined operators, such as `+`, `-`, `*`, `/`, `%`, `**`, `<<<`, `>>>`, `&&&`, `|||`, and `^^^` ([§](the-f-library-fsharpcoredll.md#basic-operators-and-functions-fsharpcoreoperators)).

If the operator does not resolve to a user-defined or library-defined operator, the name resolution
rules ([§](inference-procedures.md#name-resolution)) ensure that the operator resolves to an expression that implicitly uses a static member
invocation expression (§ ?) that involves the types of the operands. This means that the effective
behavior of an operator that is not defined in the F# library is to require a static member that has the
same name as the operator, on the type of one of the operands of the operator. In the following
code, the otherwise undefined operator `-->` resolves to the static member on the `Receiver` type,
based on a type-directed resolution:

```fsharp
type Receiver(latestMessage:string) =
    static member (<--) (receiver:Receiver,message:string) =
        Receiver(message)

    static member (-->) (message,receiver:Receiver) =
        Receiver(message)

let r = Receiver "no message"

r <-- "Message One"

"Message Two" --> r
```

### Dynamic Operator Expressions

Expressions of the following forms are _dynamic operator expressions:_

```fsgrammar
expr1 ? expr2
expr1 ? expr2 <- expr3
```

These expressions are defined by their syntactic translation:

`expr ? ident` → `(?) expr "ident"`

`expr1 ? (expr2)` → `(?) expr1 expr2`

`expr1 ? ident <- expr2` → `(?<-) expr1 "ident" expr2`

`expr1 ? (expr2) <- expr3` → `(?<-) expr1 expr2 expr3`

Here `"ident"` is a string literal that contains the text of `ident`.

> Note: The F# core library `FSharp.Core.dll` does not define the `(?)` and `(?<-)` operators.
However, user code may define these operators. For example, it is common to define
the operators to perform a dynamic lookup on the properties of an object by using
reflection.

This syntactic translation applies regardless of the definition of the `(?)` and `(?<-)` operators.
However, it does not apply to uses of the parenthesized operator names, as in the following:

```fsharp
(?) x y
```

### The AddressOf Operators

Under default definitions, expressions of the following forms are _address-of expressions,_ called
_byref-address-of expression_ and _nativeptr-address-of expression,_ respectively:

```fsgrammar
& expr
&& expr
```

Such expressions take the address of a mutable local variable, byref-valued argument, field, array
element, or static mutable global variable.

For `&expr` and `&&expr`, the initial type of the overall expression must be of the form `byref<ty>` and
`nativeptr<ty>` respectively, and the expression `expr` is checked with initial type `ty`.

The overall expression is elaborated recursively by taking the address of the elaborated form of `expr`,
written `AddressOf(expr, DefinitelyMutates)`, defined in [§](expressions.md#taking-the-address-of-an-elaborated-expression).

Use of these operators may result in unverifiable or invalid common intermediate language (CIL)
code; when possible, a warning or error is generated. In general, their use is recommended only:

- To pass addresses where `byref` or `nativeptr` parameters are expected.
- To pass a `byref` parameter on to a subsequent function.
- When required to interoperate with native code.

Addresses that are generated by the `&&` operator must not be passed to functions that are in tail call
position. The F# compiler does not check for this.

Direct uses of `byref` types, `nativeptr` types, or values in the `FSharp.NativeInterop` module may
result in invalid or unverifiable CIL code. In particular, `byref` and `nativeptr` types may NOT be used
within named types such as tuples or function types.

When calling an existing CLI signature that uses a CLI pointer type `ty*`, use a value of type
`nativeptr<ty>`.

> Note: The rules in this section apply to the following prefix operators, which are defined
in the F# core library for use with one argument.
<br>`FSharp.Core.LanguagePrimitives.IntrinsicOperators.(~&)`
<br>`FSharp.Core.LanguagePrimitives.IntrinsicOperators.(~&&)`
<br>Other uses of these operators are not permitted.

### Lookup Expressions

Lookup expressions are specified by syntactic translation:

`e1.[eargs]` → `e1.get_Item(eargs)`

`e1.[eargs] <- e3` → `e .set_Item(eargs, e3)`

In addition, for the purposes of resolving expressions of this form, array types of rank 1, 2, 3, and 4
are assumed to support a type extension that defines an `Item` property that has the following
signatures:

```fsharp
type 'T[] with
    member arr.Item : int -> 'T

type 'T[,] with
    member arr.Item : int * int -> 'T

type 'T[,,] with
    member arr.Item : int * int * int -> 'T

type 'T[,,,] with
    member arr.Item : int * int * int * int -> 'T
```

In addition, if type checking determines that the type of `e1` is a named type that supports the
`DefaultMember` attribute, then the member name identified by the `DefaultMember` attribute is used
instead of Item.

### Slice Expressions

Slice expressions are defined by syntactic translation:

`e1.[sliceArg1, ,,, sliceArgN]` → `e1.GetSlice(args1, ..., argsN)`

`e1.[sliceArg1, ,,, sliceArgN] <- expr` → `e1.SetSlice(args1, ...,argsN, expr)`

where each `sliceArgN` is one of the following and translated to `argsN` (giving one or two args) as
indicated

`*` → `None, None`

`e1..` → `Some e1, None`

`..e2` → `None, Some e2`

`e1..e2` → `Some e1, Some e2`

`idx` → `idx`

Because this is a shallow syntactic translation, the `GetSlice` and `SetSlice` name may be resolved by
any of the relevant _Name Resolution_ ([§](inference-procedures.md#name-resolution)) techniques, including defining the method as a type
extension for an existing type.

For example, if a matrix type has the appropriate overloads of the GetSlice method (see below), it is
possible to do the following:

```fsharp
matrix.[1..,*] // get rows 1.. from a matrix (returning a matrix)
matrix.[1..3,*] // get rows 1..3 from a matrix (returning a matrix)
matrix.[*,1..3] // get columns 1..3from a matrix (returning a matrix)
matrix.[1..3,1,.3] // get a 3x3 sub-matrix (returning a matrix)
matrix.[3,*] // get row 3 from a matrix as a vector
matrix.[*,3] // get column 3 from a matrix as a vector
```

In addition, CIL array types of rank 1 to 4 are assumed to support a type extension that defines a
method `GetSlice` that has the following signature:

```fsharp
type 'T[] with
    member arr.GetSlice : ?start1:int * ?end1:int -> 'T[]
type 'T[,] with
    member arr.GetSlice : ?start1:int * ?end1:int * ?start2:int * ?end2:int -> 'T[,]
    member arr.GetSlice : idx1:int * ?start2:int * ?end2:int -> 'T[]
    member arr.GetSlice : ?start1:int * ?end1:int * idx2:int - > 'T[]
type 'T[,,] with
    member arr.GetSlice : ?start1:int * ?end1:int * ?start2:int * ?end2:int *
                          ?start3:int * ?end3:int
                            -> 'T[,,]
type 'T[,,,] with
    member arr.GetSlice : ?start1:int * ?end1:int * ?start2:int * ?end2:int *
                          ?start3:int * ?end3:int * ?start4:int * ?end4:int
                            -> 'T[,,,]
```

In addition, CIL array types of rank 1 to 4 are assumed to support a type extension that defines a
method `SetSlice` that has the following signature:

```fsharp
type 'T[] with
    member arr.SetSlice : ?start1:int * ?end1:int * values:T[] -> unit

type 'T[,] with
    member arr.SetSlice : ?start1:int * ?end1:int * ?start2:int * ?end2:int *
                          values:T[,] -> unit
    member arr.SetSlice : idx1:int * ?start2:int * ?end2:int * values:T[] -> unit
    member arr.SetSlice : ?start1:int * ?end1:int * idx2:int * values:T[] -> unit

type 'T[,,] with
    member arr.SetSlice : ?start1:int * ?end1:int * ?start2:int * ?end2:int *
                          ?start3:int * ?end3:int *
                          values:T[,,] -> unit

type 'T[,,,] with
    member arr.SetSlice : ?start1:int * ?end1:int * ?start2:int * ?end2:int *
                          ?start3:int * ?end3:int * ?start4:int * ?end4:int *
                          values:T[,,,] -> unit
```

### Member Constraint Invocation Expressions

An expression of the following form is a member constraint invocation expression:

```fsgrammar
(static-typars : (member-sig) expr)
```

Type checking proceeds as follows:

1. The expression is checked with initial type `ty`.
2. A statically resolved member constraint is applied ([§](types-and-type-constraints.md#member-constraints)):
    <br>`static-typars: (member-sig)`
3. `ty` is asserted to be equal to the return type of the constraint.
4. `expr` is checked with an initial type that corresponds to the argument types of the constraint.

The elaborated form of the expression is a member invocation. For example:

```fsharp
let inline speak (a: ^a) =
    let x = (^a : (member Speak: unit -> string) (a))
    printfn "It said: %s" x
    let y = (^a : (member MakeNoise: unit -> string) (a))
    printfn "Then it went: %s" y

type Duck() =
    member x.Speak() = "I'm a duck"
    member x.MakeNoise() = "quack"
type Dog() =
    member x.Speak() = "I'm a dog"
    member x.MakeNoise() = "grrrr"

let x = new Duck()
let y = new Dog()
speak x
speak y
```

Outputs:

```fsother
It said: I'm a duck
Then it went: quack
It said: I'm a dog
Then it went: grrrr
```

### Assignment Expressions

An expression of the following form is an _assignment expression_ :

```fsharp
expr1 <- expr2
```

A modified version of _Unqualified Lookup_ ([§](inference-procedures.md#unqualified-lookup)) is applied to the expression `expr1` using a fresh
expected result type `ty` , thus producing an elaborate expression `expr1`. The last qualification for `expr1`
must resolve to one of the following constructs:

- An invocation of a property with a setter method. The property may be an indexer.

    Type checking incorporates `expr2` as the last argument in the method application resolution for
    the setter method. The overall elaborated expression is a method call to this setter property and
    includes the last argument.

- A mutable value `path` of type `ty`.

    Type checking of `expr2` uses the expected result type `ty` and generates an elaborated expression
    `expr2`. The overall elaborated expression is an assignment to a value reference `&path <-stobj expr2`.

- A reference to a value `path` of type `byref<ty>`.

    Type checking of `expr2` uses the expected result type `ty` and generates an elaborated expression
    `expr2`. The overall elaborated expression is an assignment to a value reference `path <-stobj expr2`.

- A reference to a mutable field `expr1a.field` with the actual result type `ty`.

    Type checking of `expr2` uses the expected result type `ty` and generates an elaborated expression
    `expr2`. The overall elaborated expression is an assignment to a field (see [§](expressions.md#taking-the-address-of-an-elaborated-expression)):

    `AddressOf(expr1a.field, DefinitelyMutates) <-stobj expr2`

- A array lookup `expr1a.[expr1b]` where `expr1a` has type `ty[]`.

    Type checking of expr2 uses the expected result type ty and generates thean elaborated
    expression expr2. The overall elaborated expression is an assignment to a field (see [§](expressions.md#taking-the-address-of-an-elaborated-expression)):

    `AddressOf(expr1a.[expr1b], DefinitelyMutates) <-stobj expr2`

    > Note: Because assignments have the preceding interpretations, local values must be
    mutable so that primitive field assignments and array lookups can mutate their
    immediate contents. In this context, “immediate” contents means the contents of a
    mutable value type. For example, given

    ```fsharp
    [<Struct>]
    type SA =
        new(v) = { x = v }
        val mutable x : int

    [<Struct>]
    type SB =
        new(v) = { sa = v }
        val mutable sa : SA

    let s1 = SA(0)
    let mutable s2 = SA(0)
    let s3 = SB(0)
    let mutable s4 = SB(0)
    ```

    > Then these are not permitted:

    ```fsharp
    s1.x <- 3
    s3.sa.x <- 3
    ```

    and these are:

    ```fsharp
    s2.x <- 3
    s4.sa.x <- 3
    s4.sa <- SA(2)
    ```

## Control Flow Expressions

### Parenthesized and Block Expressions

A _parenthesized expression_ has the following form:

```fsgrammar
(expr)
```

A _block expression_ has the following form:

```fsgrammar
begin expr end
```

The expression `expr` is checked with the same initial type as the overall expression.

The elaborated form of the expression is simply the elaborated form of `expr`.

### Sequential Execution Expressions

A _sequential execution expression_ has the following form:

```fsgrammar
expr1 ; expr2
```

For example:

```fsharp
printfn "Hello"; printfn "World"; 3
```

The `;` token is optional when both of the following are true:

- The expression `expr2` occurs on a subsequent line that starts in the same column as `expr1`.

- The current pre-parse context that results from the syntax analysis of the program text is a
    `SeqBlock` ([§](lexical-filtering.md#lexical-filtering)).

When the semicolon is optional, parsing inserts a `$sep` token automatically and applies an additional
syntax rule for lightweight syntax ([§](lexical-filtering.md#basic-lightweight-syntax-rules-by-example)). In practice, this means that code can omit the `;` token
for sequential execution expressions that implement functions or immediately follow tokens such as
`begin` and `(`.

The expression `expr1` is checked with an arbitrary initial type `ty`. After checking `expr1`, `ty` is asserted
to be equal to `unit`. If the assertion fails, a warning rather than an error is reported. The expression
`expr2` is then checked with the same initial type as the overall expression.

Sequential execution expressions are a primitive elaborated form.

### Conditional Expressions

A _conditional expression_ has the following forms

```fsgrammar
if expr1a then expr1b
elif expr3a then expr2b
...
elif exprna then exprnb
else exprlast
```

The `elif` and `else` branches may be omitted. For example:

```fsharp
if (1 + 1 = 2) then "ok" else "not ok"
if (1 + 1 = 2) then printfn "ok"
```

Conditional expressions are equivalent to pattern matching on Boolean values. For example, the
following expression forms are equivalent:

```fsgrammar
if expr1 then expr2 else expr3
match (expr1: bool) with true -> expr2 | false -> expr3
```

If the `else` branch is omitted, the expression is a _sequential conditional expression_ and is equivalent
to:

```fsgrammar
match (expr1: bool) with true -> expr2 | false -> ()
```

with the exception that the initial type of the overall expression is first asserted to be `unit`.

### Shortcut Operator Expressions

Under default definitions, expressions of the following form are respectively an _shortcut and expression_ and a _shortcut or expression_ :

```fsgrammar
expr && expr
expr || expr
```

These expressions are defined by their syntactic translation:

```fsgrammar
expr1 && expr2 → if expr1 then expr2 else false
expr1 || expr2 → if expr1 then true else expr2
```

> Note: The rules in this section apply when the following operators, as defined in the F#
    core library, are applied to two arguments.
    <br>`FSharp.Core.LanguagePrimitives.IntrinsicOperators.(&&)`
    <br>`FSharp.Core.LanguagePrimitives.IntrinsicOperators.(||)`
    <br>
    If the operator is not immediately applied to two arguments, it is interpreted as a strict
    function that evaluates both its arguments before use.

### Pattern-Matching Expressions and Functions

A _pattern-matching expression_ has the following form:

```fsgrammar
match expr with rules
```

Pattern matching is used to evaluate the given expression and select a rule ([§](patterns.md#patterns)). For example:

```fsharp
match (3, 2) with
| 1, j -> printfn "j = %d" j
| i, 2 - > printfn "i = %d" i
| _ - > printfn "no match"
```

A _pattern-matching function_ is an expression of the following form:

```fsgrammar
function rules
```

A pattern-matching function is syntactic sugar for a single-argument function expression that is
followed by immediate matches on the argument. For example:

```fsharp
function
| 1, j -> printfn "j = %d" j
| _ - > printfn "no match"
```

is syntactic sugar for the following, where x is a fresh variable:

```fsharp
fun x ->
    match x with
    | 1, j -> printfn "j = %d" j
    | _ - > printfn "no match"
```

### Sequence Iteration Expressions

An expression of the following form is a _sequence iteration expression_ :

```fsgrammar
for pat in expr1 do expr2 done
```

The done token is optional if `expr2` appears on a later line and is indented from the column position
of the for token. In this case, parsing inserts a `$done` token automatically and applies an additional
syntax rule for lightweight syntax ([§](lexical-filtering.md#basic-lightweight-syntax-rules-by-example)).

For example:

```fsharp
for x, y in [(1, 2); (3, 4)] do
    printfn "x = %d, y = %d" x y
```

The expression `expr1` is checked with a fresh initial type `tyexpr`, which is then asserted to be a subtype
of type `IEnumerable<ty>`, for a fresh type `ty`. If the assertion succeeds, the expression elaborates to
the following, where `v` is of type `IEnumerator<ty>` and `pat` is a pattern of type `ty` :

```fsharp
let v = expr1.GetEnumerator()
try
    while (v.MoveNext()) do
        match v.Current with
        | pat - > expr2
        | _ -> ()
finally
    match box(v) with
    | :? System.IDisposable as d - > d .Dispose()
    | _ -> ()
```

If the assertion fails, the type `tyexpr` may also be of any static type that satisfies the “collection
pattern” of CLI libraries. If so, the _enumerable extraction_ process is used to enumerate the type. In
particular, `tyexpr` may be any type that has an accessible GetEnumerator method that accepts zero
arguments and returns a value that has accessible MoveNext and Current properties. The type of `pat`
is the same as the return type of the Current property on the enumerator value. However, if the
Current property has return type obj and the collection type `ty` has an Item property with a more
specific (non-object) return type `ty2` , type `ty2` is used instead, and a dynamic cast is inserted to
convert v.Current to `ty2`.

A sequence iteration of the form

```fsgrammar
for var in expr1 .. expr2 do expr3 done
```

where the type of `expr1` or `expr2` is equivalent to `int`, is elaborated as a simple for-loop expression
([§](expressions.md#simple-for-loop-expressions))

### Simple for-Loop Expressions

An expression of the following form is a _simple for loop expression_ :

```fsgrammar
for var = expr1 to expr2 do expr3 done
```

The `done` token is optional when `e2` appears on a later line and is indented from the column position
of the `for` token. In this case, a `$done` token is automatically inserted, and an additional syntax rule
for lightweight syntax applies ([§](lexical-filtering.md#basic-lightweight-syntax-rules-by-example)). For example:

```fsharp
for x = 1 to 30 do
    printfn "x = %d, x^2 = %d" x (x*x)
```

The bounds `expr1` and `expr2` are checked with initial type `int`. The overall type of the expression is
`unit`. A warning is reported if the body `expr3` of the `for` loop does not have static type `unit`.

The following shows the elaborated form of a simple for-loop expression for fresh variables `start`
and `finish`:

```fsharp
let start = expr1 in
let finish = expr2 in
for var = start to finish do expr3 done
```

For-loops over ranges that are specified by variables are a primitive elaborated form. When
executed, the iterated range includes both the starting and ending values in the range, with an
increment of 1.

An expression of the form

```fsgrammar
for var in expr1 .. expr2 do expr3 done
```

is always elaborated as a simple for-loop expression whenever the type of `expr1` or `expr2` is
equivalent to `int`.

### While Expressions

A _while loop expression_ has the following form:

```fsgrammar
while expr1 do expr2 done
```

The `done` token is optional when `expr2` appears on a subsequent line and is indented from the
column position of the `while`. In this case, a `$done` token is automatically inserted, and an additional
syntax rule for lightweight syntax applies ([§](lexical-filtering.md#basic-lightweight-syntax-rules-by-example)).

For example:

```fsharp
while System.DateTime.Today.DayOfWeek = System.DayOfWeek.Monday do
    printfn "I don't like Mondays"
```

The overall type of the expression is `unit`. The expression `expr1` is checked with initial type `bool`. A
warning is reported if the body `expr2` of the while loop cannot be asserted to have type `unit`.

### Try-with Expressions

A _try-with expression_ has the following form:

```fsgrammar
try expr with rules
```

For example:

```fsharp
try "1" with _ -> "2"

try
    failwith "fail"
with
    | Failure msg -> "caught"
    | :? System.InvalidOperationException -> "unexpected"
```

Expression `expr` is checked with the same initial type as the overall expression. The pattern matching
clauses are then checked with the same initial type and with input type `System.Exception`.

Try-with expressions are a primitive elaborated form.

### Reraise Expressions

A _reraise expression_ is an application of the `reraise` F# library function. This function must be
applied to an argument and can be used only on the immediate right-hand side of `rules` in a try-with
expression.

```fsharp
try
    failwith "fail"
with e -> printfn "Failing"; reraise()
```

> Note: The rules in this section apply to any use of the function
  `FSharp.Core.Operators.reraise`, which is defined in the F# core library.

When executed, `reraise()` continues exception processing with the original exception information.

### Try-finally Expressions

A _try-finally expression_ has the following form:

```fsgrammar
try expr1 finally expr2
```

For example:

```fsharp
try "1" finally printfn "Finally!"

try
    failwith "fail"
finally
    printfn "Finally block"
```

Expression `expr1` is checked with the initial type of the overall expression. Expression `expr2` is
checked with arbitrary initial type, and a warning occurs if this type cannot then be asserted to be
equal to `unit`.

Try-finally expressions are a primitive elaborated form.

### Assertion Expressions

An _assertion expression_ has the following form:

```fsgrammar
assert expr
```

The expression `assert expr` is syntactic sugar for `System.Diagnostics.Debug.Assert(expr)`

> Note: `System.Diagnostics.Debug.Assert` is a conditional method call. This means that
assertions are triggered only if the DEBUG conditional compilation symbol is defined.

## Definition Expressions

A _definition expression_ has one of the following forms:

```fsgrammar
let function-defn in expr
let value-defn in expr
let rec function-or-value-defns in expr
use ident = expr1 in expr
```

Such an expression establishes a local function or value definition within the lexical scope of `expr`
and has the same overall type as `expr`.

In each case, the `in` token is optional if `expr` appears on a subsequent line and is aligned with the
token `let`. In this case, a `$in` token is automatically inserted, and an additional syntax rule for
lightweight syntax applies ([§](lexical-filtering.md#basic-lightweight-syntax-rules-by-example))

For example:

```fsharp
let x = 1
x + x
```

and

```fsharp
let x, y = ("One", 1)
x.Length + y
```

and

```fsharp
let id x = x in (id 3, id "Three")
```

and

```fsharp
let swap (x, y) = (y,x)
List.map swap [ (1, 2); (3, 4) ]
```

and

```fsharp
let K x y = x in List.map (K 3) [ 1; 2; 3; 4 ]
```

Function and value definitions in expressions are similar to function and value definitions in class
definitions ([§](type-definitions.md#class-type-definitions)), modules ([§](namespaces-and-modules.md#function-and-value-definitions-in-modules)), and computation expressions ([§](expressions.md#computation-expressions)), with the following
exceptions:

- Function and value definitions in expressions may not define explicit generic parameters ([§](types-and-type-constraints.md#type-parameter-definitions)).
    For example, the following expression is rejected:
       <br>`let f<'T> (x:'T) = x in f 3`
- Function and value definitions in expressions are not public and are not subject to arity analysis
    ([§](inference-procedures.md#arity-inference)).
- Any custom attributes that are specified on the declaration, parameters, and/or return
    arguments are ignored and result in a warning. As a result, function and value definitions in
    expressions may not have the `ThreadStatic` or `ContextStatic` attribute.

### Value Definition Expressions

A value definition expression has the following form:

```fsgrammar
let value-defn in expr
```

where _value-defn_ has the form:

```fsgrammar
mutable? access? pat typar-defns? return-type? = rhs-expr
```

Checking proceeds as follows:

1. Check the _value-defn_ ([§](inference-procedures.md#checking-and-elaborating-function-value-and-member-definitions)), which defines a group of identifiers `identj` with inferred types `tyj`

2. Add the identifiers `identj` to the name resolution environment, each with corresponding type
    `tyj`.
3. Check the body `expr` against the initial type of the overall expression.

In this case, the following rules apply:

- If `pat` is a single value pattern `ident`, the resulting elaborated form of the entire expression is

    ```fsgrammar
    let ident1 <typars1> = expr1 in
    body-expr
    ```

    where ident1 , typars1 and expr1 are defined in [§](inference-procedures.md#checking-and-elaborating-function-value-and-member-definitions).

- Otherwise, the resulting elaborated form of the entire expression is

    ```fsgrammar
    let tmp <typars1 ... typars n> = expr in
    let ident1 <typars1> = expr1 in
    ...
    let identn <typarsn> = exprn in
    body-expr
    ```

    where `tmp` is a fresh identifier and `identi`, `typarsi`, and `expri` all result from the compilation of
    the pattern `pat` ([§](patterns.md#patterns)) against the input `tmp`.

Value definitions in expressions may be marked as `mutable`. For example:

```fsharp
let mutable v = 0
while v < 10 do
    v <- v + 1
    printfn "v = %d" v
```

Such variables are implicitly dereferenced each time they are used.

### Function Definition Expressions

A function definition expression has the form:

```fsgrammar
let function-defn in expr
```

where `function-defn` has the form:

```fsgrammar
inline? access? ident-or-op typar-defns? pat1 ... patn return-type? = rhs-expr
```

Checking proceeds as follows:

1. Check the `function-defn` ([§](inference-procedures.md#checking-and-elaborating-function-value-and-member-definitions)), which defines `ident1`, `ty1`, `typars1` and `expr1`
2. Add the identifier `ident1` to the name resolution environment, each with corresponding type `ty1`.
3. Check the body `expr` against the initial type of the overall expression.

The resulting elaborated form of the entire expression is

```fsgrammar
let ident1 < typars1 > = expr1 in
expr
```

where `ident1` , `typars1` and `expr1` are as defined in [§](inference-procedures.md#checking-and-elaborating-function-value-and-member-definitions).

### Recursive Definition Expressions

An expression of the following form is a _recursive definition expression_:

```fsgrammar
let rec function-or-value-defns in expr
```

The defined functions and values are available for use within their own definitions—that is can be
used within any of the expressions on the right-hand side of `function-or-value-defns`. Multiple
functions or values may be defined by using `let rec ... and ...`. For example:

```fsharp
let test() =
    let rec twoForward count =
        printfn "at %d, taking two steps forward" count
        if count = 1000 then "got there!"
        else oneBack (count + 2)
    and oneBack count =
        printfn "at %d, taking one step back " count
        twoForward (count - 1)

    twoForward 1

test()
```

In the example, the expression defines a set of recursive functions. If one or more recursive values
are defined, the recursive expressions are analyzed for safety ([§](inference-procedures.md#recursive-safety-analysis)). This may result in warnings
(including some reported as compile-time errors) and runtime checks.

### Deterministic Disposal Expressions

A _deterministic disposal expression_ has the form:

```fsgrammar
use ident = expr1 in expr2
```

For example:

```fsharp
use inStream = System.IO.File.OpenText "input.txt"
let line1 = inStream.ReadLine()
let line2 = inStream.ReadLine()
(line1,line2)
```

The expression is first checked as an expression of form `let ident = expr1 in expr2` ([§](expressions.md#value-definition-expressions)), which results in an elaborated expression of the following form:

```fsgrammar
let ident1 : ty1 = expr1 in expr2.
```

Only one value may be defined by a deterministic disposal expression, and the definition is not
generalized ([§](inference-procedures.md#generalization)). The type `ty1` , is then asserted to be a subtype of `System.IDisposable`. If the
dynamic value of the expression after coercion to type `obj` is non-null, the `Dispose` method is called
on the value when the value goes out of scope. Thus the overall expression elaborates to this:

```fsgrammar
let ident1 : ty1 = expr1
try expr2
finally (match ( ident :> obj) with
         | null -> ()
         | _ -> (ident :> System.IDisposable).Dispose())
```

### Pinned Pointer Expressions

A _pinned pointer expression_ allows a pointer to be extracted from an expression and bound to a name, preventing the value from being collected or moved by the garbage collector for the scope of the binding. This feature is intended for low-level programming scenarios.

A pinned pointer expression has the following form:

```fsgrammar
use ident = fixed expr
```

For example, pinning a field within an object:

```fsharp
type Point = { mutable x : int; mutable y : int }

let pinObject() =
    let point = { x = 1; y = 2 }
    use p1 = fixed &point.x
    // code that uses p1 as a nativeptr<int>
```

Pinning an array:

```fsharp
let pinArray() =
    let arr = [| 0.0; 1.5; 2.3 |]
    use p = fixed arr
    // code that uses p as a nativeptr<float>
```

Pinning a string:

```fsharp
let pinString() =
    let str = "Hello"
    use pChar = fixed str
    // code that uses pChar as a nativeptr<char>
```

The `fixed` keyword is used to pin the expression and can only appear immediately to the right of a `use` binding. The pointer is fixed for the duration of the `use` binding's scope; once it goes out of scope, it is no longer pinned. This construct is not a try/finally `IDisposable` pattern but is instead used to define the scope of the pinning.

A _pinned pointer expression_ is more efficient and convenient than creating a `GCHandle`.

Like all pointer-related code, the use of `fixed` is considered an unsafe feature and will result in a compiler warning. The use of `fixed` is restricted to expressions within functions or methods and cannot be used at the script or module level.

## Type-related Expressions

### Type-Annotated Expressions

A _type-annotated expression_ has the following form, where `ty` indicates the static type of `expr`:

```fsgrammar
expr : ty
```

For example:

```fsharp
(1 : int)
let f x = (x : string) + x
```

When checked, the initial type of the overall expression is asserted to be equal to `ty`. Expression `expr`
is then checked with initial type `ty`. The expression elaborates to the elaborated form of `expr`. This
ensures that information from the annotation is used during the analysis of `expr` itself.

### Static Coercion Expressions

A _static coercion expression_ — also called a flexible type constraint — has the following form:

```fsgrammar
expr :> ty
```

The expression `upcast expr` is equivalent to `expr :> _`, so the target type is the same as the initial
type of the overall expression. For example:

```fsharp
(1 :> obj)
("Hello" :> obj)
([1;2;3] :> seq<int>).GetEnumerator()
(upcast 1 : obj)
```

The initial type of the overall expression is `ty`. Expression `expr` is checked using a fresh initial type
`tye`, with constraint `tye :> ty`. Static coercions are a primitive elaborated form.

### Dynamic Type-Test Expressions

A dynamic type-test expression has the following form:

```fsgrammar
expr :? ty
```

For example:

```fsharp
((1 :> obj) :? int)
((1 :> obj) :? string)
```

The initial type of the overall expression is `bool`. Expression `expr` is checked using a fresh initial type
`tye`. After checking:

- The type `tye` must not be a variable type.
- A warning is given if the type test will always be true and therefore is unnecessary.
- The type `tye` must not be sealed.
- If type `ty` is sealed, or if `ty` is a variable type, or if type `tye` is not an interface type, then `ty :> tye`
    is asserted.

Dynamic type tests are a primitive elaborated form.

### Dynamic Coercion Expressions

A dynamic coercion expression has the following form:

```fsgrammar
expr :?> ty
```

The expression downcast `e1` is equivalent to `expr :?> _` , so the target type is the same as the initial
type of the overall expression. For example:

```fsharp
let obj1 = (1 :> obj)
(obj1 :?> int)
(obj1 :?> string)
(downcast obj1 : int)
```

The initial type of the overall expression is `ty`. Expression `expr` is checked using a fresh initial type
`tye`. After these checks:

- The type `tye` must not be a variable type.
- A warning is given if the type test will always be true and therefore is unnecessary.
- The type `tye` must not be sealed.
- If type `ty` is sealed, or if `ty` is a variable type, or if type `tye` is not an interface type, then `ty :> tye`
    is asserted.

Dynamic coercions are a primitive elaborated form.

## Quoted Expressions

An expression in one of these forms is a quoted expression:

```fsgrammar
<@ expr @>

<@@ expr @@>
```

The former is a _strongly typed quoted expression_ , and the latter is a _weakly typed quoted expression_.
In both cases, the expression forms capture the enclosed expression in the form of a typed abstract
syntax tree.

The exact nodes that appear in the expression tree are determined by the elaborated form of `expr`
that type checking produces.

For details about the nodes that may be encountered, see the documentation for the
`FSharp.Quotations.Expr` type in the F# core library. In particular, quotations may contain:

- References to module-bound functions and values, and to type-bound members. For example:

    ```fsharp
    let id x = x
    let f (x : int) = <@ id 1 @>
    ```

    In this case the value appears in the expression tree as a node of kind
    `FSharp.Quotations.Expr.Call`.

- A type, module, function, value, or member that is annotated with the `ReflectedDefinition`
    attribute. If so, the expression tree that forms its definition may be retrieved dynamically using
    the `FSharp.Quotations.Expr.TryGetReflectedDefinition`.

    If the `ReflectedDefinition` attribute is applied to a type or module, it will be recursively applied
    to all members, too.

- References to defined values, such as the following:

    ```fsharp
    let f (x : int) = <@ x + 1 @>
    ```

    Such a value appears in the expression tree as a node of kind FSharp.Quotations.Expr.Value.

- References to generic type parameters or uses of constructs whose type involves a generic
    parameter, such as the following:

    ```fsharp
    let f (x:'T) = <@ (x, x) : 'T * 'T @>
    ```

    In this case, the actual value of the type parameter is implicitly substituted throughout the type
    annotations and types in the generated expression tree.

As of F# 3. 1 , the following limitations apply to quoted expressions:

- Quotations may not use object expressions.
- Quotations may not define expression-bound functions that are themselves inferred to be
    generic. Instead, expression-bound functions should either include type annotations to refer to a
    specific type or should be written by using module-bound functions or class-bound members.

### Strongly Typed Quoted Expressions

A strongly typed quoted expression has the following form:

```fsgrammar
<@ expr @>
```

For example:

```fsharp
<@ 1 + 1 @>

<@ (fun x -> x + 1) @>
```

In the first example, the type of the expression is `FSharp.Quotations.Expr<int>`. In the second
example, the type of the expression is `FSharp.Quotations.Expr<int -> int>`.

When checked, the initial type of a strongly typed quoted expression `<@ expr @>` is asserted to be of
the form `FSharp.Quotations.Expr<ty>` for a fresh type `ty`. The expression `expr` is checked with initial
type `ty`.

### Weakly Typed Quoted Expressions

A _weakly typed quoted expression_ has the following form:

```fsgrammar
<@@ expr @@>
```

Weakly typed quoted expressions are similar to strongly quoted expressions but omit any type
annotation. For example:

```fsharp
<@@ 1 + 1 @@>

<@@ (fun x -> x + 1) @@>
```

In both these examples, the type of the expression is `FSharp.Quotations.Expr`.

When checked, the initial type of a weakly typed quoted expression `<@@ expr @@>` is asserted to be
of the form `FSharp.Quotations.Expr`. The expression `expr` is checked with fresh initial type `ty`.

### Expression Splices

Both strongly typed and weakly typed quotations may contain expression splices in the following
forms:

```fsgrammar
%expr
%%expr
```

These are respectively strongly typed and weakly typed splicing operators.

#### Strongly Typed Expression Splices

An expression of the following form is a _strongly typed expression splice_ :

```fsgrammar
%expr
```

For example, given

```fsharp
open FSharp.Quotations
let f1 (v:Expr<int>) = <@ %v + 1 @>
let expr = f1 <@ 3 @>
```

the identifier `expr` evaluates to the same expression tree as `<@ 3 + 1 @>`. The expression tree
for `<@ 3 @>` replaces the splice in the corresponding expression tree node.

A strongly typed expression splice may appear only in a quotation. Assuming that the splice
expression `%expr` is checked with initial type `ty` , the expression `expr` is checked with initial type
`FSharp.Quotations.Expr<ty>`.

> Note: The rules in this section apply to any use of the prefix operator
`FSharp.Core.ExtraTopLevelOperators.(~%)`. Uses of this operator must be applied to an
argument and may only appear in quoted expressions.

**6.8.3.2 Weakly Typed Expression Splices**
An expression of the following form is a _weakly typed expression splice_ :

```fsgrammar
%%expr
```

For example, given

```fsharp
open FSharp.Quotations
let f1 (v:Expr) = <@ %%v + 1 @>
let tree = f1 <@@ 3 @@>
```

the identifier `tree` evaluates to the same expression tree as `<@ 3 + 1 @>`. The expression tree
replaces the splice in the corresponding expression tree node.

A weakly typed expression splice may appear only in a quotation. Assuming that the splice
expression `%%expr` is checked with initial type `ty`, then the expression `expr` is checked with initial type
`FSharp.Quotations.Expr`. No additional constraint is placed on `ty`.

Additional type annotations are often required for successful use of this operator.

> Note: The rules in this section apply to any use of the prefix operator
`FSharp.Core.ExtraTopLevelOperators.(~%%)`, which is defined in the F# core library. Uses
of this operator must be applied to an argument and may only occur in quoted
expressions.

## Evaluation of Elaborated Forms

At runtime, execution evaluates expressions to values. The evaluation semantics of each expression
form are specified in the subsections that follow.

### Values and Execution Context

The execution of elaborated F# expressions results in values. Values include:

- Primitive constant values
- The special value `null`
- References to object values in the global heap of object values
- Values for value types, containing a value for each field in the value type
- Pointers to mutable locations (including static mutable locations, mutable fields and array
    elements)

Evaluation assumes the following evaluation context:

- A global heap of object values. Each object value contains:
  - A runtime type and dispatch map
  - A set of fields with associated values
  - For array objects, an array of values in index order
  - For function objects, an expression which is the body of the function
  - An optional _union case label_ , which is an identifier
  - A closure environment that assigns values to all variables that are referenced in the method
       bodies that are associated with the object
- A global environment that maps runtime-type/name pairs to values.Each name identifies a static
    field in a type definition or a value in a module.
- A local environment mapping names of variables to values.
- A local stack of active exception handlers, made up of a stack of try/with and try/finally handlers.

Evaluation may also raise an exception. In this case, the stack of active exception handlers is
processed until the exception is handled, in which case additional expressions may be executed (for

try/finally handlers), or an alternative expression may be evaluated (for try/with handlers), as
described below.

### Parallel Execution and Memory Model

In a concurrent environment, evaluation may involve both multiple active computations (multiple
concurrent and parallel threads of execution) and multiple pending computations (pending
callbacks, such as those activated in response to an I/O event).

If multiple active computations concurrently access mutable locations in the global environment or
heap, the atomicity, read, and write guarantees of the underlying CLI implementation apply. The
guarantees are related to the logical sizes and characteristics of values, which in turn depend on
their type:

- F# reference types are guaranteed to map to CLI reference types. In the CLI memory model,
    reference types have atomic reads and writes.
- F# value types map to a corresponding CLI value type that has corresponding fields. Reads and
    writes of sizes less than or equal to one machine word are atomic.

The `VolatileField` attribute marks a mutable location as volatile in the compiled form of the code.

Ordering of reads and writes from mutable locations may be adjusted according to the limitations
specified by the CLI memory model. The following example shows situations in which changes to
read and write order can occur, with annotations about the order of reads:

```fsharp
type ClassContainingMutableData() =
    let value = (1, 2)
    let mutable mutableValue = (1, 2)

    [<VolatileField>]
    let mutable volatileMutableValue = (1, 2)

    member x.ReadValues() =
        // Two reads on an immutable value
        let (a1, b1) = value

        // One read on mutableValue, which may be duplicated according
        // to ECMA CLI spec.
        let (a2, b2) = mutableValue

        // One read on volatileMutableValue, which may not be duplicated.
        let (a3, b3) = volatileMutableValue

        a1, b1, a2, b2, a3, b3

    member x.WriteValues() =
        // One read on mutableValue, which may be duplicated according
        // to ECMA CLI spec.
        let (a2, b2) = mutableValue

        // One write on mutableValue.
        mutableValue <- (a2 + 1, b2 + 1)

        // One read on volatileMutableValue, which may not be duplicated.
        let (a3, b3) = volatileMutableValue

        // One write on volatileMutableValue.
        volatileMutableValue <- (a3 + 1, b3 + 1)

let obj = ClassContainingMutableData()
Async.Parallel [ async { return obj.WriteValues() };
                 async { return obj.WriteValues() };
                 async { return obj.ReadValues() };
                 async { return obj.ReadValues() } ]
```

### Zero Values

Some types have a _zero value_. The zero value is the “default” value for the type in the CLI execution
environment. The following types have the following zero values:

- For reference types, the `null` value.
- For value types, the value with all fields set to the zero value for the type of the field. The zero
    value is also computed by the F# library function `Unchecked.defaultof<ty>`.

### Taking the Address of an Elaborated Expression

When the F# compiler determines the elaborated forms of certain expressions, it must compute a
“reference” to an elaborated expression `expr` , written `AddressOf(expr, mutation)`. The `AddressOf`
operation is used internally within this specification to indicate the elaborated forms of address-of
expressions, assignment expressions, and method and property calls on objects of variable and value
types.

The `AddressOf` operation is computed as follows:

- If `expr` has form `path` where `path` is a reference to a value with type `byref<ty>`, the elaborated
    form is `&path`.
- If `expr` has form `expra.field` where `field` is a mutable, non-readonly CLI field, the elaborated
    form is `&(AddressOf(expra).field)`.
- If `expr` has form expra.[exprb] where the operation is an array lookup, the elaborated form is
    `&(AddressOf(expra).[exprb])`.
- If `expr` has any other form, the elaborated form is `&v` ,where `v` is a fresh mutable local value that
    is initialized by adding `let v = expr` to the overall elaborated form for the entire assignment
    expression. This initialization is known as a _defensive copy_ of an immutable value. If `expr` is a
    struct, `expr` is copied each time the `AddressOf` operation is applied, which results in a different
    address each time. To keep the struct in place, the field that contains it should be marked as
    mutable.

The `AddressOf` operation is computed with respect to `mutation`, which indicates whether the
relevant elaborated form uses the resulting pointer to change the contents of memory. This
assumption changes the errors and warnings reported.

- If `mutation` is `DefinitelyMutates`, then an error is given if a defensive copy must be created.
- If `mutation` is `PossiblyMutates`, then a warning is given if a defensive copy arises.

An F# compiler can optionally upgrade `PossiblyMutates` to `DefinitelyMutates` for calls to property
setters and methods named `MoveNext` and `GetNextArg`, which are the most common cases of struct-
mutators in CLI library design. This is done by the F# compiler.

> Note:In F#, the warning “copy due to possible mutation of value type” is a level 4
  warning and is not reported when using the default settings of the F# compiler. This is
  because the majority of value types in CLI libraries are immutable. This is warning
  number 52 in the F# implementation.
  <br> CLI libraries do not include metadata to indicate whether a particular value type is
  immutable. Unless a value is held in arrays or locations marked mutable, or a value type
  is known to be immutable to the F# compiler, F# inserts copies to ensure that
  inadvertent mutation does not occur.

### Evaluating Value References

At runtime, an elaborated value reference `v` is evaluated by looking up the value of `v` in the local
environment.

### Evaluating Function Applications

At runtime, an elaborated application of a function `f e1 ... en` is evaluated as follows:

- The expressions `f` and `e1 ... en`, are evaluated.
- If `f` evaluates to a function value with closure environment `E`, arguments `v1 ... vm`, and body `expr`,
    where `m <= n` , then `E` is extended by mapping `v1 ... vm` to the argument values for `e1 ... em`. The
    expression `expr` is then evaluated in this extended environment and any remaining arguments
    applied.
- If `f` evaluates to a function value with more than `n` arguments, then a new function value is
    returned with an extended closure mapping `n` additional formal argument names to the
    argument values for `e1 ... em`.

The result of calling the `obj.GetType()` method on the resulting object is under-specified (see
[§](expressions.md#values-with-underspecified-object-identity-and-type-identity)).

### Evaluating Method Applications

At runtime an elaborated application of a method is evaluated as follows:

- The elaborated form is `e0.M(e1 , ..., en)` for an instance method or `M(e, ..., en)` for a static method.
- The (optional) `e0` and `e1` ,..., _en_ are evaluated in order.
- If `e0` evaluates to `null`, a `NullReferenceException` is raised.
- If the method is declared `abstract` — that is, if it is a virtual dispatch slot — then the body of the
    member is chosen according to the dispatch maps of the value of `e0` ([§](inference-procedures.md#dispatch-slot-checking)).
- The formal parameters of the method are mapped to corresponding argument values. The body
    of the method member is evaluated in the resulting environment.

### Evaluating Union Cases

At runtime, an elaborated use of a union case `Case(e1 , ..., en)` for a union type `ty` is evaluated as
follows:

- The expressions `e1, ..., en` are evaluated in order.
- The result of evaluation is an object value with union case label `Case` and fields given by the
    values of `e1 , ..., en`.
- If the type `ty` uses null as a representation ([§](types-and-type-constraints.md#nullness)) and `Case` is the single union case without
    arguments, the generated value is `null`.
- The runtime type of the object is either `ty` or an internally generated type that is compatible
    with `ty`.

### Evaluating Field Lookups

At runtime, an elaborated lookup of a CLI or F# fields is evaluated as follows:

- The elaborated form is `expr.F` for an instance field or `F` for a static field.
- The (optional) `expr` is evaluated.
- If `expr` evaluates to `null`, a `NullReferenceException` is raised.
- The value of the field is read from either the global field table or the local field table associated
    with the object.

### Evaluating Array Expressions

At runtime, an elaborated array expression `[| e1; ...; en |]ty` is evaluated as follows:

- Each expression `e1 ... en` is evaluated in order.
- The result of evaluation is a new array of runtime type `ty[]` that contains the resulting values in
    order.

### Evaluating Record Expressions

At runtime, an elaborated record construction `{ field1 = e1; ... ; fieldn = en }ty` is evaluated as
follows:

- Each expression `e1 ... en` is evaluated in order.
- The result of evaluation is an object of type `ty` with the given field values

### Evaluating Function Expressions

At runtime, an elaborated function expression `(fun v1 ... vn -> expr)` is evaluated as follows:

- The expression evaluates to a function object with a closure that assigns values to all variables
    that are referenced in `expr` and a function body that is `expr`.
- The values in the closure are the current values of those variables in the execution environment.
- The result of calling the `obj.GetType()` method on the resulting object is under-specified (see
    [§](expressions.md#values-with-underspecified-object-identity-and-type-identity)).

### Evaluating Object Expressions

At runtime, elaborated object expressions

```fsgrammar
{ new ty0 args-expr? object-members
      interface ty1 object-members1
      interface tyn object-membersn }
```

is evaluated as follows:

- The expression evaluates to an object whose runtime type is compatible with all of the `tyi` and
    which has the corresponding dispatch map ([§](inference-procedures.md#dispatch-slot-checking)). If present, the base construction expression
    `ty0 (args-expr)` is executed as the first step in the construction of the object.
- The object is given a closure that assigns values to all variables that are referenced in `expr`.
- The values in the closure are the current values of those variables in the execution environment.

The result of calling the `obj.GetType()` method on the resulting object is under-specified (see
[§](expressions.md#values-with-underspecified-object-identity-and-type-identity)).

### Evaluating Definition Expressions

At runtime, each elaborated definition `pat = expr` is evaluated as follows:

- The expression `expr` is evaluated.
- The expression is then matched against `pat` to produce a value for each variable pattern ([§](patterns.md#named-patterns))
    in `pat`.
- These mappings are added to the local environment.

### Evaluating Integer For Loops

At runtime, an integer for loop `for var = expr1 to expr2 do expr3 done` is evaluated as follows:

- Expressions `expr1` and `expr2` are evaluated once to values `v1` and `v2`.
- The expression `expr3` is evaluated repeatedly with the variable `var` assigned successive values in
    the range of `v1` up to `v2`.
- If `v1` is greater than `v2` , then `expr3` is never evaluated.

### Evaluating While Loops

As runtime, while-loops `while expr1 do expr2 done` are evaluated as follows:

- Expression `expr1` is evaluated to a value `v1`.
- If `v1` is true, expression `expr2` is evaluated, and the expression `while expr1 do expr2 done` is
    evaluated again.
- If `v1` is `false`, the loop terminates and the resulting value is `null` (the representation of the only
    value of type `unit`)

### Evaluating Static Coercion Expressions

At runtime, elaborated static coercion expressions of the form `expr :> ty` are evaluated as follows:

- Expression `expr` is evaluated to a value `v`.
- If the static type of `e` is a value type, and `ty` is a reference type, `v` is _boxed_ ; that is, `v` is converted
    to an object on the heap with the same field assignments as the original value. The expression
    evaluates to a reference to this object.
- Otherwise, the expression evaluates to `v`.

### Evaluating Dynamic Type-Test Expressions

At runtime, elaborated dynamic type test expressions `expr :? ty` are evaluated as follows:

1. Expression `expr` is evaluated to a value `v`.
2. If `v` is `null`, then:
    - If `tye` uses `null` as a representation ([§](types-and-type-constraints.md#nullness)), the result is `true`.
    - Otherwise the expression evaluates to `false`.
3. If `v` is not `null` and has runtime type `vty` which dynamically converts to `ty` ([§](types-and-type-constraints.md#dynamic-conversion-between-types)), the
    expression evaluates to `true`. However, if `ty` is an enumeration type, the expression evaluates to
    `true` if and only if `ty` is precisely `vty`.

### Evaluating Dynamic Coercion Expressions

At runtime, elaborated dynamic coercion expressions `expr :?> ty` are evaluated as follows:

1. Expression `expr` is evaluated to a value `v`.
2. If `v` is `null`:
    - If `tye` uses `null` as a representation ([§](types-and-type-constraints.md#nullness)), the result is the `null` value.
    - Otherwise a `NullReferenceException` is raised.
3. If `v` is not `null`:
    - If `v` has dynamic type `vty` which _dynamically converts_ to `ty` ([§](types-and-type-constraints.md#dynamic-conversion-between-types)), the expression evaluates to the dynamic conversion of `v` to `ty`.
        - If `vty` is a reference type and `ty` is a value type, then `v` is _unboxed_ ; that is, `v` is
             converted from an object on the heap to a struct value with the same field
             assignments as the object. The expression evaluates to this value.
        - Otherwise, the expression evaluates to `v`.
    - Otherwise an `InvalidCastException` is raised.

Expressions of the form `expr :?> ty` evaluate in the same way as the F# library function
`unbox<ty>(expr)`.

> Note: Some F# types — most notably the `option<_>` type — use `null` as a representation
    for efficiency reasons ([§](types-and-type-constraints.md#nullness)). For these  types, boxing and unboxing can lose type
    distinctions. For example, contrast the following two examples:

    ```fsother
    > (box([]:string list) :?> int list);;
    System.InvalidCastException...
    > (box(None:string option) :?> int option);;
    val it : int option = None
    ```

> In the first case, the conversion from an empty list of strings to an empty list of integers
    (after first boxing) fails. In the second case, the conversion from a string option to an
    integer option (after first boxing) succeeds.

### Evaluating Sequential Execution Expressions

At runtime, elaborated sequential expressions `expr1 ; expr2` are evaluated as follows:

- The expression `expr1` is evaluated for its side effects and the result is discarded.
- The expression `expr2` is evaluated to a value `v2` and the result of the overall expression is `v2`.

### Evaluating Try-with Expressions

At runtime, elaborated try-with expressions try `expr1 with rules` are evaluated as follows:

- The expression `expr1` is evaluated to a value `v1`.
- If no exception occurs, the result is the value `v1`.
- If an exception occurs, the pattern rules are executed against the resulting exception value.
  - If no rule matches, the exception is reraised.
  - If a rule `pat -> expr2` matches, the mapping `pat = v1` is added to the local environment,
       and `expr2` is evaluated.

### Evaluating Try-finally Expressions

At runtime, elaborated try-finally expressions try `expr1 finally expr2` are evaluated as follows:

- The expression `expr1` is evaluated.
  - If the result of this evaluation is a value `v` , then `expr2` is evaluated.
       1) If this evaluation results in an exception, then the overall result is that exception.
       2) If this evaluation does not result in an exception, then the overall result is `v`.
  - If the result of this evaluation is an exception, then `expr2` is evaluated.
       3) If this evaluation results in an exception, then the overall result is that exception.
       4) If this evaluation does not result in an exception, then the original exception is re-
          raised.

### Evaluating AddressOf Expressions

At runtime, an elaborated address-of expression is evaluated as follows. First, the expression has
one of the following forms:

- `&path` where `path` is a static field.
- `&(expr.field)`
- `&(expra.[exprb])`
- `&v` where `v` is a local mutable value.

The expression evaluates to the address of the referenced local mutable value, mutable field, or
mutable static field.

> Note: The underlying CIL execution machinery that F# uses supports covariant arrays, as
evidenced by the fact that the type `string[]` dynamically converts to `obj[]` (§5.4.10).
Although this feature is rarely used in F#, its existence means that array assignments and
taking the address of array elements may fail at runtime with a
`System.ArrayTypeMismatchException` if the runtime type of the target array does not
match the runtime type of the element being assigned. For example, the following code
fails at runtime:

```fsharp
let f (x: byref<obj>) = ()

let a = Array.zeroCreate<obj> 10
let b = Array.zeroCreate<string> 10
f (&a.[0])
let bb = ((b :> obj) :?> obj[])
// The next line raises a System.ArrayTypeMismatchException exception.
F (&bb.[1])
```

### Values with Underspecified Object Identity and Type Identity

The CLI and F# support operations that detect object identity—that is, whether two object
references refer to the same “physical” object. For example, `System.Object.ReferenceEquals(obj1, obj2)`
returns true if the two object references refer to the same object. Similarly,
`System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode()` returns a hash code that is partly
based on physical object identity, and the `AddHandler` and `RemoveHandler` operations (which register
and unregister event handlers) are based on the object identity of delegate values.

The results of these operations are underspecified when used with values of the following F# types:

- Function types
- Tuple types
- Immutable record types
- Union types
- Boxed immutable value types

For two values of such types, the results of `System.Object.ReferenceEquals` and
`System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode` are underspecified; however, the
operations terminate and do not raise exceptions. An implementation of F# is not required to define
the results of these operations for values of these types.

For function values and objects that are returned by object expressions, the results of the following
operations are underspecified in the same way:

- `Object.GetHashCode()`
- `Object.GetType()`

For union types the results of the following operations are underspecified in the same way:

- `Object.GetType()`
