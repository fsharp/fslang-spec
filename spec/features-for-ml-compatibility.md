# Features for ML Compatibility

F# has its roots in the Caml family of programming languages and its core constructs are similar to
some other ML-family languages. As a result, F# supports some constructs for compatibility with
other implementations of ML-family languages.

## Conditional Compilation for ML Compatibility

F# supports the following constructs for conditional compilation:

```fsgrammar
token start-fsharp-token = "(*IF-FSHARP" | "(*F#"
token end-fsharp-token = "ENDIF-FSHARP*)" | "F#*)"
token start-ml-token = "(*IF-OCAML*)"
token end-ml-token = "(*ENDIF-OCAML*)"
```

F# ignores the `start-fsharp-token` and `end-fsharp-token` tokens. This means that sections marked

```fsgrammar
(*IF-FSHARP ... ENDIF-FSHARP*)
```

**or**

```fsgrammar
(*F# ... F#*)
```

are included during tokenization when compiling with the F# compiler. The intervening text is
tokenized and returned in the token stream as normal.

In addition, the `start-ml-token` token is discarded and the following text is tokenized as `string` , `_`
(any character), and `end-ml-token` until an `end-ml-token` is reached. Comments are not treated as
special during this process and are simply processed as “other text”. This means that text
surrounded by the following is excluded when compiling with the F# compiler:

```fsgrammar
(*IF-CAML*) ... (*ENDIF-CAML*)
or (*IF-OCAML*) ... (*ENDIF-OCAML*)
```

The intervening text is tokenized as “strings and other text” and the tokens are discarded until the
corresponding end token is reached. Comments are not treated as special during this process and
are simply processed as “other text.”

The converse holds when programs are compiled using a typical ML compiler.

## Extra Syntactic Forms for ML Compatibility

The following identifiers are also keywords primarily because they are keywords in OCaml. Although
F# reserves several OCaml keywords for future use, the `/mlcompatibility` option enables the use of
these keywords as identifiers.

```fsgrammar
token ocaml-ident-keyword =
    asr land lor lsl lsr lxor mod
```

> Note: In F# the following alternatives are available. The precedence of these operators
differs from the precedence that OCaml uses.<br><br>
    `asr >>> (on signed type)`<br>
    `land &&&`<br>
    `lor |||`<br>
    `lsl <<<`<br>
    `lsr >>> (on unsigned type)`<br>
    `lxor ^^^`<br>
    `mod %`<br>
    `sig begin (that is, begin/end may be used instead of sig/end)`<br>

F# includes the following additional syntactic forms for ML compatibility:

```fsgrammar
expr :=
    | ...
    | expr .( expr ) // array lookup
    | expr .( expr ) <- expr // array assignment
```

```fsgrammar
type :=
    | ...
    | ( type ,..., type ) long-ident // generic type instantiation

module-implementation :=
    | ...
    | module ident = struct ... end

module-signature :=
    | ...
    | module ident : sig ... end
```

An ML compatibility warning occurs when these constructs are used.

Note that the for-expression form `for var = expr1 downto expr2 do expr3` is also permitted for ML compatibility.

The following expression forms

```fsgrammar
expr :=
    | ...
    | expr.(expr) // array lookup
    | expr.(expr) <- expr // array assignment
```

Are equivalent to the following uses of library-defined operators:

```fsgrammar
e1.(e2)               → (.()) e1 e2
e1.(e2) <- e3         → (.()<-) e1 e2 e3
```

## Extra Operators

F# defines the following two additional shortcut operators:

```fsgrammar
e1 or e2                  → (or) e1 e2
e1 & e2                   → (&) e1 e2
```

## File Extensions and Lexical Matters

F# supports the use of the `.ml` and `.mli` extensions on the command line. The “indentation
awareness off” syntax option is implicitly enabled when using either of these filename extensions.

Lightweight syntax can be explicitly disabled in `.fs`, `.fsi`, `.fsx`, and `.fsscript` files by specifying
`#indent "off"` as the first declaration in a file:

```fsharp
#indent "off"
```

When lightweight syntax is disabled, whitespace can include tab characters:

```fsgrammar
regexp whitespace = [ ' ' '\t' ]+
```
