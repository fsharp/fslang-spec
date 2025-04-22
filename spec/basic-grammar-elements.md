# Basic Grammar Elements

This section defines grammar elements that are used repeatedly in later sections.

## Operator Names

Several places in the grammar refer to an `ident-or-op` rather than an `ident` :

```fsgrammar
ident-or-op :=
    | ident
    | ( op-name )
    | (*)
op-name :=
    | symbolic-op
    | range-op-name
    | active-pattern-op-name
range-op-name :=
    | ..
    | .. ..
active-pattern-op-name :=
    | | ident | ... | ident |
    | | ident | ... | ident | _ |
```

In operator definitions, the operator name is placed in parentheses. For example:

```fsgrammar
let (+++) x y = (x, y)
```

This example defines the binary operator `+++`. The text `(+++)` is an `ident-or-op` that acts as an
identifier with associated text `+++`. Likewise, for active pattern definitions (§ 7), the active pattern
case names are placed in parentheses, as in the following example:

```fsgrammar
let (|A|B|C|) x = if x < 0 then A elif x = 0 then B else C
```

Because an `ident-or-op` acts as an identifier, such names can be used in expressions. For example:

```fsgrammar
List.map ((+) 1) [ 1; 2; 3 ]
```

The three character token `(*)` defines the `*` operator:

```fsgrammar
let (*) x y = (x + y)
```

To define other operators that begin with `*`, whitespace must follow the opening parenthesis;
otherwise `(*` is interpreted as the start of a comment:

```fsgrammar
let ( *+* ) x y = (x + y)
```

Symbolic operators and some symbolic keywords have a compiled name that is visible in the
compiled form of F# programs. The compiled names are shown below.

```fsgrammar
[]    op_Nil
::    op_ColonColon
+     op_Addition
-     op_Subtraction
*     op_Multiply
/     op_Division
**    op_Exponentiation
@     op_Append
^     op_Concatenate
%     op_Modulus
&&&   op_BitwiseAnd
|||   op_BitwiseOr
^^^   op_ExclusiveOr
<<<   op_LeftShift
~~~   op_LogicalNot
>>>   op_RightShift
~+    op_UnaryPlus
~-    op_UnaryNegation
=     op_Equality
<>    op_Inequality
<=    op_LessThanOrEqual
>=    op_GreaterThanOrEqual
<     op_LessThan
>     op_GreaterThan
?     op_Dynamic
?<-   op_DynamicAssignment
|>    op_PipeRight
||>   op_PipeRight2
|||>  op_PipeRight3
<|    op_PipeLeft
<||   op_PipeLeft2
<|||  op_PipeLeft3
!     op_Dereference
>>    op_ComposeRight
<<    op_ComposeLeft
<@ @> op_Quotation
<@@ @@> op_QuotationUntyped
~%    op_Splice
~%%   op_SpliceUntyped
~&    op_AddressOf
~&&   op_IntegerAddressOf
||    op_BooleanOr
&&    op_BooleanAnd
+=    op_AdditionAssignment
- =   op_SubtractionAssignment
*=    op_MultiplyAssignment
/=    op_DivisionAssignment
..    op_Range
.. .. op_RangeStep
```

Compiled names for other symbolic operators are `op_N1` ... `Nn` where `N1` to `Nn` are the names for the
characters as shown in the table below. For example, the symbolic identifier `<*` has the compiled
name `op_LessMultiply`:

```fsgrammar
>   Greater
<   Less
+   Plus
-   Minus
*   Multiply
=   Equals
~   Twiddle
%   Percent
.   Dot
&   Amp
|   Bar
@   At
#   Hash
^   Hat
!   Bang
?   Qmark
/   Divide
.   Dot
:   Colon
(   LParen
,   Comma
)   RParen
[   LBrack
]   RBrack
```

## Long Identifiers

Long identifiers `long-ident` are sequences of identifiers that are separated by ‘`.`’ and optional
whitespace. Long identifiers `long-ident-or-op` are long identifiers that may terminate with an
operator name.

```fsgrammar
long-ident := ident '.' ... '.' ident
long-ident-or-op :=
    | long-ident '.' ident-or-op
    | ident-or-op
```

## Constants

The constants in the following table may be used in patterns and expressions. The individual lexical
formats for the different constants are defined in [§3.](lexical-analysis.md#lexical-analysis)

```fsgrammar
const :=
    | sbyte
    | int16
    | int32
    | int64                 -- 8, 16, 32 and 64-bit signed integers
    | byte
    | uint16
    | uint32
    | int                   -- 32 - bit signed integer
    | uint64                -- 8, 16, 32 and 64-bit unsigned integers
    | ieee32                -- 32 - bit number of type "float32"
    | ieee64                -- 64 - bit number of type "float"
    | bignum                -- User or library-defined integral literal type
    | char                  -- Unicode character of type "char"
    | string v              -- String of type "string" (System.String)
    | verbatim-string       -- String of type "string" (System.String)
    | triple-quoted-string  -- String of type "string" (System.String)
    | bytestring            -- String of type "byte[]"
    | verbatim-bytearray    -- String of type "byte[]"
    | bytechar              -- Char of type "byte"
    | false | true          -- Boolean constant of type "bool"
    | '(' ')'               -- unit constant of type "unit"
```

## Operators and Precedence

### Categorization of Symbolic Operators

The following `symbolic-op` tokens can be used to form prefix and infix expressions. The marker `OP`
represents all `symbolic-op` tokens that begin with the indicated prefix, except for tokens that appear
elsewhere in the table.

```fsgrammar
infix-or-prefix-op :=
    +, -, +., -., %, &, &&
prefix-op :=
    infix-or-prefix-op
    ~ ~~ ~~~    (and any repetitions of ~)
    !OP         (except !=)
infix-op :=
    infix-or-prefix-op
    - OP +OP || <OP >OP = |OP &OP ^OP *OP /OP %OP !=
                (or any of these preceded by one or more ‘.’)
    :=
    ::
    $
    or
    ?
```

The operators `+`, `-`, `+.`, `-.`, `%`, `%%`, `&`, `&&` can be used as both prefix and infix operators. When these
operators are used as prefix operators, the tilde character is prepended internally to generate the
operator name so that the parser can distinguish such usage from an infix use of the operator. For
example, `-x` is parsed as an application of the operator `~-` to the identifier `x`. This generated name is
also used in definitions for these prefix operators. Consequently, the definitions of the following
prefix operators include the `~` character:

```fsgrammar
// To completely redefine the prefix + operator:
let (~+) x = x
```

```fsgrammar
// To completely redefine the infix + operator to be addition modulo- 7
let (+) a b = (a + b) % 7
```

```fsgrammar
// To define the operator on a type:
type C(n:int) =
let n = n % 7
    member x.N = n
    static member (~+) (x:C) = x
    static member (~-) (x:C) = C(-n)
    static member (+) (x1:C,x2:C) = C(x1.N+x2.N)
    static member (-) (x1:C,x2:C) = C(x1.N-x2.N)
```

The `::` operator is special. It represents the union case for the addition of an element to the head of
an immutable linked list, and cannot be redefined, although it may be used to form infix expressions.
It always accepts arguments in tupled form — as do all union cases — rather than in curried form.

### Precedence of Symbolic Operators and Pattern/Expression Constructs

Rules of precedence control the order of evaluation for ambiguous expression and pattern
constructs. Higher precedence items are evaluated before lower precedence items.

The following table shows the order of precedence, from highest to lowest, and indicates whether
the operator or expression is associated with the token to its left or right. The `OP` marker represents
the `symbolic-op` tokens that begin with the specified prefix, except those listed elsewhere in the
table. For example, `+OP` represents any token that begins with a plus sign, unless the token appears
elsewhere in the table.

| Operator or expression | Associativity | Comments |
| --- | --- | --- |
| f<types> | Left | High-precedence type application; see [§15.3](lexical-filtering.md#lexical-analysis-of-type-applications) |
| f(x) | Left | High-precedence application; see [§15.2](lexical-filtering.md#high-precedence-application) |
| . | Left |  |
| _prefix-op_ | Left | Applies to prefix uses of these symbols |
| "| rule" | Right Pattern matching rules |
| "f x" <br> "lazy x" <br> "assert x" | Left | |
| **OP | Right | |
| *OP /OP %OP | Left |  |
| - OP +OP | Left | Applies to infix uses of these symbols |
| :? | Not associative |  |
| :: | Right | |
| ^OP | Right | |
| !=OP \<OP \>OP = \|OP &OP $ | Left |  |
| :> :?> | Right |  |
| & && | Left |  |
| or \|\| | Left |  |
| , | Not associative |  |
| := | Right |  |
| -> | Right |  |
| if | Not associative |  |
| function, fun, match, try | Not associative |  |
| let | Not associative |  |
| ; | Right |  |
| \| | Left |  |
| when | Right |  |
| as | Right |  |

If ambiguous grammar rules (such as the rules from §6) involve tokens in the table, a construct that
appears earlier in the table has higher precedence than a construct that appears later in the table.

The associativity indicates whether the operator or construct applies to the item to the left or the
right of the operator.

For example, consider the following token stream:

```fsharp
a + b * c
```

In this expression, the `expr infix-op expr` rule for `b * c` takes precedence over the
`expr infix-op expr` rule for `a + b`, because the `*` operator has higher precedence than the `+` operator. Thus, this
expression can be pictured as follows:

```fsharp
   a + b * c
// _________
//     _____
```

rather than

```fsharp
   a + b * c
// _________
// _____
```

Likewise, given the tokens

```fsharp
a * b * c
```

the left associativity of `*` means we can picture the resolution of the ambiguity as:

```fsharp
   a * b * c
// _____
```

In the preceding table, leading `.` characters are ignored when determining precedence for infix
operators. For example, `.*` has the same precedence as `*.` This rule ensures that operators such as
`.*`, which is frequently used for pointwise-operation on matrices, have the expected precedence.

The table entries marked as “High-precedence application” and “High-precedence type application”
are the result of the augmentation of the lexical token stream, as described in §15.2 and [§15.3](lexical-filtering.md#lexical-analysis-of-type-applications).
