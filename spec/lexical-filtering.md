# Lexical Filtering

## Lightweight Syntax

F# supports lightweight syntax, in which whitespace makes indentation significant.

The lightweight syntax option is a conservative extension of the explicit language syntax, in the sense
that it simply lets you leave out certain tokens such as `in` and `;;` because the parser takes
indentation into account. Indentation can make a surprising difference in the readability of code.
Compiling your code with the indentation-aware syntax option is useful even if you continue to use
explicit tokens, because the compiler reports many indentation problems with your code and
ensures a regular, clear formatting style.

In the processing of lightweight syntax, comments are considered pure whitespace. This means that
the compiler ignores the indentation position of comments. Comments act as if they were replaced
by whitespace characters. Tab characters cannot be used in F# files.

### Basic Lightweight Syntax Rules by Example

The basic rules that the F# compiler applies when it processes lightweight syntax are shown below,
illustrated by example.

#### ;; delimiter

When the lightweight syntax option is enabled, top level expressions do not require the `;;` delimiter
because every construct that starts in the first column is implicitly a new declaration. The `;;`
delimiter is still required to terminate interactive entries to fsi.exe, but not when using F# Interactive
from Visual Studio.

Lightweight Syntax

```fsharp
printf "Hello"
printf "World"
```

Normal Syntax

```fsharp
printf "Hello";;
printf "World";;
```

#### in keyword

When the lightweight syntax option is enabled, the `in` keyword is optional. The token after the `=` in
a `let` definition begins a new block, and the pre-parser inserts an implicit separating `in` token
between each definition that begins at the same column as that token.

Lightweight Syntax

```fsharp
let SimpleSample() =
    let x = 10 + 12 - 3
    let y = x * 2 + 1
    let r1,r2 = x/3, x%3
    (x,y,r1,r2)
```

```fsharp
Normal Syntax
#indent "off"
let SimpleSample() =
    let x = 10 + 12 - 3 in
    let y = x * 2 + 1 in
    let r1,r2 = x/3, x%3 in
    (x,y,r1,r2)
```

#### done keyword

When the lightweight syntax option is enabled, the `done` keyword is optional. Indentation establishes
the scope of structured constructs such as `match`, `for`, `while` and `if`/`then`/`else`.

Lightweight Syntax

```fsharp
let FunctionSample() =
    let tick x = printfn "tick %d" x
    let tock x = printfn "tock %d" x
    let choose f g h x =
        if f x then g x else h x
    for i = 0 to 10 do
        choose (fun n -> n%2 = 0) tick tock i
    printfn "done!" 
```

Normal Syntax

```fsharp
#indent "off"
let FunctionSample() =
    let tick x = printfn "tick %d" x in
    let tock x = printfn "tock %d" x in
    let choose f g h x =
        if f x then g x else h x in
    for i = 0 to 10 do
        choose (fun n -> n%2 = 0) tick tock i
    done;
    printfn "done!"
```

#### if / then / else Scope

When the lightweight syntax option is enabled, the scope of `if`/`then`/`else` is implicit from
indentation. Without the lightweight syntax option, `begin`/`end` or parentheses are often required to
delimit such constructs.

Lightweight Syntax

```fsharp
let ArraySample() =
    let numLetters = 26
    let results = Array.create numLetters 0
    let data = "The quick brown fox"
    for i = 0 to data.Length - 1 do
        let c = data.Chars(i)
        let c = Char.ToUpper(c)
        if c >= 'A' && c <= 'Z' then
            let i = Char.code c - Char.code 'A'
            results.[i] <- results.[i] + 1
    printfn "done!"
```

Normal Syntax

```fsharp
#indent "off"
let ArraySample() =
    let numLetters = 26 in
    let results = Array.create numLetters 0 in
    let data = "The quick brown fox" in
    for i = 0 to data.Length - 1 do
        let c = data.Chars(i) in
        let c = Char.ToUpper(c) in
        if c >= 'A' && c <= 'Z' then begin
            let i = Char.code c - Char.code 'A' in
            results.[i] <- results.[i] + 1
        end
    done;
    printfn "done!"
```

### Inserted Tokens

Lexical filtering inserts the following hidden tokens :

```fsother
token $in       // Note: also called ODECLEND
token $done     // Note: also called ODECLEND
token $begin    // Note: also called OBLOCKBEGIN
token $end      // Note: also called OEND, OBLOCKEND and ORIGHT_BLOCK_END
token $sep      // Note: also called OBLOCKSEP
token $app      // Note: also called HIGH_PRECEDENCE_APP
token $tyapp    // Note: also called HIGH_PRECEDENCE_TYAPP
```

> Note: The following tokens are also used in the Microsoft F# implementation. They are
translations of the corresponding input tokens and help provide better error messages
for lightweight syntax code:
`tokens $let $use $let! $use! $do $do! $then $else $with $function $fun`

### Grammar Rules Including Inserted Tokens

Additional grammar rules take into account the token transformations performed by lexical filtering:

```fsgrammar
expr +:=
    | let function-defn $ in expr
    | let value-defn $ in expr
    | let rec function-or-value-defns $ in expr
    | while expr do expr $done
    | if expr then $begin expr $end
    | for pat in expr do expr $done
    | for expr to expr do expr $done
    | try expr $end with expr $done
    | try expr $end finally expr $done
    
    | expr $app expr            // equivalent to " expr ( expr )"
    | expr $sep expr            // equivalent to " expr ; expr "
    | expr $tyapp < types >     // equivalent to " expr < types >"
    | $begin expr $end          // equivalent to “ expr ”

elif-branch +:=
    | elif expr then $begin expr $end

else-branch +:=
    | else $begin expr $end

class-or-struct-type-body +:=
    | $begin class-or-struct-type-body $end
                                // equivalent to class-or-struct-type-body

module-elems +:=
    | $begin module-elem ... module-elem $end

module-abbrev +:=
    | module ident = $begin long-ident $end

module-defn +:=
    | module ident = $begin module-defn-body $end

module-signature-elements +:=
    | $begin module-signature-element ... module-signature-element $end

module-signature +:=
    | module ident = $begin module-signature-body $end
```

### Offside Lines

Lightweight syntax is sometimes called the “offside rule”. In F# code, offside lines occur at column
positions. For example, an `=` token associated with `let` introduces an offside line at the column of
the first non-whitespace token after the `=` token.

Other structured constructs also introduce offside lines at the following places:

- The column of the first token after `then` in an `if`/`then`/`else` construct.
- The column of the first token after `try`, `else`, `->`, `with` (in a `match`/`with` or `try`/`with`), or `with` (in a
    type extension).
- The column of the first token of a `(`, `{` or `begin` token.
- The start of a `let`, if or `module` token.

Here are some examples of how the offside rule applies to F# code. In the first example, `let` and
`type` declarations are not properly aligned, which causes F# to generate a warning.

```fsharp
// "let" and "type" declarations in
// modules must be precisely aligned.
let x = 1
 let y = 2   // unmatched 'let'
let z = 3    // warning FS0058: possible
             // incorrect indentation: this token is offside of
             // context at position (2:1)
```

In the second example, the `|` markers in the match patterns do not align properly:

```fsharp
// The "|" markers in patterns must align.
// The first "|" should always be inserted.
let f () =
    match 1+1 with
    | 2 -> printf "ok"
  | _ -> failwith "no!"     // syntax error
```

### The Pre-Parse Stack

F# implements the lightweight syntax option by preparsing the token stream that results from a
lexical analysis of the input text according to the lexical rules in [§15.1.3](lexical-filtering.md#grammar-rules-including-inserted-tokens). Pre-parsing for lightweight
syntax uses a stack of _contexts_.

- When a column position becomes an offside line, a context is pushed.
- The closing bracketing tokens `)`, `}`, and `end` terminate offside contexts up to and including the
    context that the corresponding opening token introduced.

### Full List of Offside Contexts

This section describes the full list of offside contexts that is kept on the pre-parse stack.

The _SeqBlock_ context is the primary context of the analysis.It indicates a sequence of items that
must be column-aligned. Where necessary for parsing, the compiler automatically inserts a delimiter
that replaces the regular `in` and `;` tokens between the syntax elements. The _SeqBlock_ context is
pushed at the following times:

- Immediately after the start of a file, excluding lexical directives such as `#if`.
- Immediately after an = token is encountered in a _Let_ or _Member_ context.
- Immediately after a _Paren_ , _Then_ , _Else_ , _WithAugment_, _Try_ , _Finally_ , _Do_ context is pushed.
- Immediately after an infix token is encountered.
- Immediately after a -> token is encountered in a _MatchClauses_ context.
- Immediately after an `interface`, `class`, or `struct` token is encountered in a type declaration.
- Immediately after an `=` token is encountered in a record expression when the subsequent token
    either (a) occurs on the next line or (b) is one of _try_ , _match_ , _if_ , _let_ , _for_ , _while_ or _use_.
- Immediately after a `<-` token is encoutered when the subsequent token either (a) does not occur
    on the same line or (b) is one of _try_ , _match_ , _if_ , _let_ , _for_ , _while_ or _use_.

Here “immediately after” refers to the fact that the column position associated with the _SeqBlock_ is
the first token following the significant token.

In the last two rules, a new line is significant. For example, the following do not start a SeqBlock on
the right-hand side of the “<-“ token, so it does not parse correctly:

```fsharp
let mutable x = 1
// The subsequent token occurs on the same line.
X <- printfn "hello"
    2 + 2
```

To start a SeqBlock on the right, either parentheses or a new line should be used:

```fsharp
// The subsequent token does not occur on the same line, so a SeqBlock is pushed.
X <-
    printfn "hello"
    2 + 2
```

The following contexts are associated with nested constructs that are introduced by the specified
keywords:

| Context | Pushed when the token stream contains... |
| --- | --- |
| _Let_ | The `let` keyword |
| _If_ | The `if` or `elif` keyword |
| _Try_ | The `try` keyword |
| _Lazy_ | The `lazy` keyword |
| _Fun_ | The `fun` keyword |
| _Function_ | The `function` keyword |
| _WithLet_ | The `with` keyword as part of a record expression or an object expression whose members use the syntax `{ new Foo with M() = 1 and N() = 2 }` |
| _WithAugment_ | The `with` keyword as part of an extension, interface, or object expression whose members use the syntax `{ new Foo member x.M() = 1 member x. N() = 2 }` |
| _Match_ | the `match` keyword |
| _For_ | the `for` keyword |
| _While_ | The `while` keyword |
| _Then_ | The `then` keyword |
| _Else_ | The `else` keyword |
| _Do_ | The `do` keyword |
| _Type_ | The `type` keyword |
| _Namespace_ | The `namespace` keyword |
| _Module_ | The `module` keyword |
| _Member_ | The `member`, `abstract`, `default`, or `override` keyword, if the _Member_ context is not already active, because multiple tokens may be present.<br>- or -<br>`(` is the next token after the `new` keyword. This distinguishes the member declaration `new(x) = ... from the expression new x()` |
| _Paren(token)_ | `(`, `begin`, `struct`, `sig`, `{`, `[`, `[\|`, or `quote-op-left` |
| _MatchClauses_ | The `with` keyword in a _Try_ or _Match_ context immediately after a `function` keyword. |
| _Vanilla_ | An otherwise unprocessed keyword in a _SeqBlock_ context. |

### Balancing Rules

When the compiler processes certain tokens, it pops contexts off the offside stack until the stack
reaches a particular condition. When it pops a context, the compiler may insert extra tokens to
indicate the end of the construct. This procedure is called balancing the stack.

The following table lists the contexts that the compiler pops and the balancing condition for each:

| Token | Contexts Popped and Balancing Conditions: |
| --- | --- |
| `End` | Enclosing context is one of the following:<br> `WithAugment`<br> `Paren(interface)`<br> `Paren(class)`<br> `Paren(sig)`<br> `Paren(struct)`<br> `Paren(begin)` |
| `;;` | Pop all contexts from stack |
| `else` | `If` |
| `elif` | `If` |
| `done` | `Do` |
| `in` | `For` or `Let` |
| `with` | `Match`, `Member`, `Interface`, `Try`, `Type` |
| `finally` | `Try` |
| `)` | `Paren(()` |
| `}` | `Paren({)` |
| `]` | `Paren([)` |
| `\|]` | `Paren([\|)` |
| `quote-op-right` | `Paren(quote-op-left)` |

### Offside Tokens, Token Insertions, and Closing Contexts

The _offside limit_ for the current offside stack is the rightmost offside line for the offside contexts on
the context stack. The following example shows the offside limits:

```fsharp
let FunctionSample() =
    let tick x = printfn "tick %d" x
    let tock x = printfn "tock %d" x
    let choose f g h x =
        if f x then g x else h x
    for i = 0 to 10 do
        choose (fun n - > n% 2 = 0 ) tick tock i
    printfn "done!"
//      ^ Offside limit for inner let and for contexts
//  ^ Offside limit for outer let context
```

When a token occurs on or before the _offside limit_ for the current offside stack, and a _permitted
undentation_ does not apply, enclosing contexts are _closed_ until the token is no longer offside. This
may result in the insertion of extra delimiting tokens.

Contexts are closed as follows:

- When a _Fun_ context is closed, the `$end` token is inserted.
- When a _SeqBlock_ , _MatchClauses_ , _Let_ , or _Do_ context is closed, the `$end` token is inserted, with the
    exception of the first _SeqBlock_ pushed for the start of the file.
- When a _While_ or _For_ context is closed, and the offside token that forced the close is not `done`,
    the `$done` token is inserted.
- When a _Member_ context is closed, the `$end` token is inserted.
- When a _WithAugment_ context is closed, the `$end` token is inserted.

If a token is offside and a context cannot be closed, then an “undentation” warning or error is issued
to indicate that the construct is badly formatted.

Tokens are also inserted in the following situations:

- When a _SeqBlock_ context is pushed, the `$begin` token is inserted, with the exception of the first
    _SeqBlock_ pushed for the start of the file.
- When a token other than `and` appears directly on the offside line of _Let_ context, and the next
    surrounding context is a _SeqBlock_ , the `$in` token is inserted.
- When a token occurs directly on the offside line of a _SeqBlock_ on the second or subsequent lines
    of the block, the `$sep` token is inserted. This token plays the same role as `;` in the grammar rules.
    For example, consider this source text:

    ```fsharp
       let x = 1
       x
    ```

The raw token stream contains `let`, `x`, `=`, `1`, `x` and the end-of-file marker `eof`. An initial _SeqBlock_ is
pushed immediately after the start of the file, at the first token in the file, with an offside line on
column 0. The `let` token pushes a _Let_ context. The `=` token in a _Let_ context pushes a _SeqBlock_
context and inserts a `$begin` token. The `1` pushes a _Vanilla_ context. The final token, `x`, is offside
from the _Vanilla_ context, which pops that context. It is also offside from the _SeqBlock_ context,
which pops the context and inserts `$end`. It is also offside from the _Let_ context, which inserts
another `$end` token. It is directly aligned with the _SeqBlock_ context, so a `$seq` token is inserted.

### Exceptions to the Offside Rules

The compiler makes some exceptions to the offside rules when it analyzes a token to determine
whether it is offside from the current context.

- In a _SeqBlock_ context, an infix token may be offside by the
size of the token plus one.

    ```fsharp
    let x =
            expr + expr
          + expr + expr
    let x =
            expr
         |> f expr
         |> f expr
    ```

- In a _SeqBlock_ context, an infix token may align precisely with the offside line of the _SeqBlock_.

    In the following example, the infix `|>` token that begins the last
line is not considered as a new
element in the sequence block on the
right-hand side of the definition. The
same also applies to `end`, `and`, `with`,
`then`, and `right-parenthesis`
operators.

    ```fsharp
    let someFunction(someCollection) =
        someCollection
        |> List.map (fun x -> x + 1)
    ```

    In the example below, the first `)` token does
not indicate a new element in a
sequence of items, even though it
aligns precisely with the sequence
block that starts at the beginning of
the argument list.

    ```fsharp
    new MenuItem("&Open...",
      new EventHandler(fun _ _ ->
                               // ...
                 ))
    ```

- In a _Let_ context, the `and` token may align precisely with the `let` keyword.

    ```fsharp
    let rec x = 1
    and y = 2
    x + y
    ```

- In a _Type_ context, the `}`, `end`, `and`, and `|` tokens may align precisely with the `type` keyword.

    ```fsharp
    type X =
    | A
    | B
    with
        member x.Seven = 21 / 3
    end
    and Y = {
        x : int
    }
    and Z() = class
        member x.Eight = 4 + 4
    end
    ```

- In a _For_ context, the `done` token may align precisely with the `for` keyword.

    ```fsharp
    for i = 1 to 3 do
        expr
    done
    ```

- In a _Match_ context, on the right-hand side of an arrow for
a match expression, a token may align
precisely with the `match` keyword.
This exception allows the last
expression to align with the `match`, so
that a long series of matches does not
increase indentation.

    ```fsharp
    match x with
    | Some(_) -> 1
    | None ->
    match y with
    | Some(_) -> 2
    | None ->
    3
    ```

- In an _Interface_ context, the `end` token may align precisely with the `interface` keyword.

    ```fsharp
    interface IDisposable with
        member x.Dispose() = printfn disposing!"
    end
    ```

- In an _If_ context, the `then`, `elif`, and `else` tokens may align precisely with the `if` keyword.

    ```fsharp
    if big
    then callSomeFunction()
    elif small
    then callSomeOtherFunction()
    else doSomeCleanup()
    ```

- In a _Try_ context, the `finally` and `with` tokens may align precisely with the `try` keyword.

    Example 1:

    ```fsharp
    try
        callSomeFunction()
    finally
        doSomeCleanup()
    ```

    Example 2:

    ```fsharp
    try
        callSomeFunction()
    with Failure(s) ->
        doSomeCleanup()
    ```

- In a _Do_ context, the `done` token may align precisely with the `do` keyword.

    ```fsharp
    for i = 1 to 3
        do
            expr
        done
    ```

### Permitted Undentations

As a general rule, incremental indentation requires that nested expressions occur at increasing
column positions in indentation-aware code. Warnings or syntax errors are issued when this is not
the case. However, undentation is permitted for the following constructs:

- Bodies of function expressions
- Branches of if/then/else expressions
- Bodies of modules and module types

#### Undentation of Bodies of Function Expressions

The bodies of functions may be undented from the `fun` or `function` symbol. As a result, the compiler
ignores the symbol when determining whether the body of the function satisfies the incremental
indentation rule. For example, the `printf` expression in the following example is undented from the
fun symbol that delimits the function definition:

```fsharp
let HashSample(tab: Collections.HashTable<_,_>) =
    tab.Iterate (fun c v ->
        printfn "Entry (%O,%O)" c v)
```

However, the block must not undent past other offside lines. The following is not permitted because
the second line breaks the offside line established by the `=` in the first line:

```fsharp
let x = (function (s, n) ->
    (fun z ->
        s+n+z))
```

Constructs enclosed in brackets may be undented.

#### Undentation of Branches of If/Then/Else Expressions

The body of a `(` ... `)` or `begin` ... `end` block in an `if`/`then`/`else` expression may be undented when the
body of the block follows the `then` or `else` keyword but may not undent further than the `if`
keyword. In this example, the parenthesized block follows `then`, so the body can be undented to the
offside line established by `if`:

```fsharp
let IfSample(day: System.DayOfWeek) =
    if day = System.DayOfWeek.Monday then (
        printf "I don't like Mondays"
    )
```

#### Undentation of Bodies of Modules and Module Types

The bodies of modules and module types that are delimited by `begin` and `end` may be undented. For
example, in the following example the two let statements that comprise the module body are
undented from the `=`.

```fsharp
module MyNestedModule = begin
    let one = 1
    let two = 2
end
```

Similarly, the bodies of classes, interfaces, and structs delimited by `{` ... `}`, `class` ... `end`, `struct` ... `end`,
or `interface` ... `end` may be undented to the offside line established by the `type` keyword. For
example:

```fsharp
type MyNestedModule = interface
    abstract P : int
end
```

## High Precedence Application

The entry `f x` in the precedence table in [§4.4.2](basic-grammar-elements.md#precedence-of-symbolic-operators-and-patternexpression-constructs) refers to a function application in which the function
and argument are separated by spaces. The entry `"f(x)"` indicates that in expressions and patterns,
identifiers that are followed immediately by a left parenthesis without intervening whitespace form
a “high precedence” application. Such expressions are parsed with higher precedence than prefix
and dot-notation operators. Conceptually this means that

```fsharp
B(e)
```

is analyzed lexically as

```fsgrammar
B $app (e)
```

where `$app` is an internal symbol inserted by lexical analysis. We do not show this symbol in the
remainder of this specification and simply show the original source text.

This means that the following two statements

```fsharp
B(e).C
B (e).C
```

are parsed as

```fsgrammar
(B(e)).C
B ((e).C)
```

respectively.

Furthermore, arbitrary chains of method applications, property lookups, indexer lookups (`.[]`), field
lookups, and function applications can be used in sequence if the arguments of method applications
are parenthesized and immediately follow the method name, with no intervening spaces. For
example:

```fsharp
e.Meth1(arg1,arg2).Prop1.[3].Prop2.Meth2()
```

Although the grammar and these precedence rules technically allow the use of high-precedence
application expressions as direct arguments, an additional check prevents such use. Instead, such
expressions must be surrounded by parentheses. For example,

```fsharp
f e.Meth1(arg1,arg2) e.Meth2(arg1,arg2)
```

must be written

```fsharp
f (e.Meth1(arg1,arg2)) (e.Meth2(arg1,arg2))
```

However, indexer, field, and property dot-notation lookups may be used as arguments without
adding parentheses. For example:

```fsharp
f e.Prop1 e.Prop2.[3]
```

## Lexical Analysis of Type Applications

The entry `f<types> x` in the precedence table ([§4.4.2](basic-grammar-elements.md#precedence-of-symbolic-operators-and-patternexpression-constructs)) refers to any identifier that is followed
immediately by a `<` symbol and a sequence of all of the following:

- `_`, `,`, `*`, `'`, `[`, `]`, whitespace, or identifier tokens.
- A parentheses `(` or `<` token followed by any tokens until a matching parentheses `)` or `>` is
    encountered.
- A final `>` token.

During this analysis, any token that is composed only of the `>` character (such as `>`, `>>`, or `>>>`) is
treated as a series of individual `>` tokens. Likewise, any token composed only of `>` characters
followed by a period (such as `>.`, `>>.`, or `>>>.`) is treated as a series of individual > tokens followed by
a period.

If such a sequence of tokens follows an identifier, lexical analysis marks the construct as a _high
precedence type application_ and subsequent grammar rules ensure that the enclosed text is parsed
as a type. Conceptually this means that

```fsharp
B<int>.C<int>(e).C
```

is returned as the following stream of tokens:

```fsgrammar
B $app <int> .C $app <int>(e).C
```

where `$app` is an internal symbol inserted by lexical analysis. We do not show this symbol elsewhere
in this specification and simply show the original source text.

The lexical analysis of type applications does not apply to the character sequence “`<>`”. A character
sequence such as “`< >`” with intervening whitespace should be used to indicate an empty list of
generic arguments.

```fsharp
type Foo() =
    member this.Value = 1
let b = new Foo< >() // valid
let c = new Foo<>() // invalid
```
