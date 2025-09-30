# Lexical Analysis

Lexical analysis converts an input stream of Unicode characters into a stream of tokens by iteratively
processing the stream. If more than one token can match a sequence of characters in the source file,
lexical processing always forms the longest possible lexical element. Some tokens, such as `block-comment-start`, are discarded after processing as described later in this section.

## Whitespace

Whitespace consists of spaces and newline characters.

```fsgrammar
regexp whitespace = ' '+
regexp newline = '\n' | '\r' '\n'
token whitespace-or-newline = whitespace | newline
```

Whitespace tokens `whitespace-or-newline` are discarded from the returned token stream.

## Comments

Block comments are delimited by `(*` and `*)` and may be nested. Single-line comments begin with
two backslashes (`//`) and extend to the end of the line.

```fsgrammar
token block-comment-start = "(*"
token block-comment-end = "*)"
token end-of-line-comment = "//" [^'\n' '\r']*
```

When the input stream matches a `block-comment-start` token, the subsequent text is tokenized
recursively against the tokens that are described in §3 until a `block-comment-end` token is found. The
intermediate tokens are discarded.

For example, comments can be nested, and strings that are embedded within comments are
tokenized by the rules for `string`, `verbatim-string` , and `triple-quoted string`. In particular, strings
that are embedded in comments are tokenized in their entirety, without considering closing `*)`
marks. As a result of this rule, the following is a valid comment:

```fsharp
(* Here's a code snippet: let s = "*)" *)
```

However, the following construct, which was valid in F# 2.0, now produces a syntax error because a
closing comment token *) followed by a triple-quoted mark is parsed as part of a string:

```fsharp
(* """ *)
```

For the purposes of this specification, comment tokens are discarded from the returned lexical
stream. In practice, XML documentation tokens are `end-of-line-comments` that begin with ///. The
delimiters are retained and are associated with the remaining elements to generate XML
documentation.

## Conditional Compilation

The lexical preprocessing directives `#if ident /#else/#endif` delimit conditional compilation
sections. The following describes the grammar for such sections:

```fsgrammar
token if-directive = "#if" whitespace if-expression-text
token else-directive = "#else"
token endif-directive = "#endif"

if-expression-term =
    ident-text
    '(' if-expression ')'

if-expression-neg =
    if-expression-term
    '!' if-expression-term

if-expression-and =
    if-expression-neg
    if-expression-and && if-expression-and

if-expression-or =
    if-expression-and
    if-expression-or || if-expression-or

if-expression = if-expression-or
```

A preprocessing directive always occupies a separate line of source code and always begins with a #
character followed immediately by a preprocessing directive name, with no intervening whitespace.
However, whitespace can appear before the # character. A source line that contains the `#if`, `#else`,
or `#endif` directive can end with whitespace and a single-line comment. Multiple-line comments are
not permitted on source lines that contain preprocessing directives.

If an `if-directive` token is matched during tokenization, text is recursively tokenized until a
corresponding `else-directive` or `endif-directive`. If the evaluation of the associated
`if-expression-text` when parsed as an _if-expression_ is true in the compilation environment defines
(where each `ident-text` is evaluataed according to the values given by command line options such
as `–define`), the token stream includes the tokens between the `if-directive` and the corresponding
`else-directive` or `endif-directive`. Otherwise, the tokens are discarded. The converse applies to
the text between any corresponding `else-directive` and the `endif-directive`.

- In skipped text, `#if ident /#else/#endif` sections can be nested.
- Strings and comments are not treated as special

## Identifiers and Keywords

Identifiers follow the specification in this section.

```fsgrammar
regexp digit-char = [0-9]
regexp letter-char = '\Lu' | '\Ll' | '\Lt' | '\Lm' | '\Lo' | '\Nl'
regexp connecting-char = '\Pc'
regexp combining-char = '\Mn' | '\Mc'
regexp formatting-char = '\Cf'

regexp ident-start-char =
    | letter-char
    | _

regexp ident-char =
    | letter-char
    | digit-char
    | connecting-char
    | combining-char
    | formatting-char
    | '
    | _

regexp ident-text = ident-start-char ident-char *
token ident =
    | ident-text For example, myName1
    | `` ( [^'`' '\n' '\r' '\t'] | '`' [^ '`' '\n' '\r' '\t'] )+ ``
         example, ``value.with odd#name``
```

Any sequence of characters that is enclosed in double-backtick marks (` `` `` `), excluding newlines,
tabs, and double-backtick pairs themselves, is treated as an identifier. Note that when an identifier is
used for the name of a types, union type case, module, or namespace, the following characters are
not allowed even inside double-backtick marks:

```fsgrammar
‘.', '+', '$', '&', '[', ']', '/', '\\', '*', '\"', '`'
```

All input files are currently assumed to be encoded as UTF-8. See the C# specification for a list of the
Unicode characters that are accepted for the Unicode character classes \Lu, \Li, \Lt, \Lm, \Lo, \Nl,
\Pc, \Mn, \Mc, and \Cf.

The following identifiers are treated as keywords of the F# language:

```fsgrammar
token ident-keyword =
    abstract and as assert base begin class default delegate do done
    downcast downto elif else end exception extern false finally fixed for
    fun function global if in inherit inline interface internal lazy let
    match member mod module mutable namespace new null of open or
    override private public rec return sig static struct then to
    true try type upcast use val void when while with yield
```

The `mod` keyword is treated as an infix operator for compatibility with OCaml syntax. Other infix keywords
used in OCaml have been deprecated ([§](features-for-ml-compatibility.md#extra-operators)). A future FSharp.Core
version may add a definition for the `mod` operator.

The following identifiers are reserved for future use:

```fsgrammar
token reserved-ident-keyword =
    break checked component const constraint
    continue fori include
    mixin parallel params process protected pure
    sealed tailcall trait virtual
```

A future revision of the F# language may promote any of these identifiers to be full keywords.

The following identifiers were previously reserved but can now be used:

```fsgrammar
token ident =
    atomic constructor eager functor measure method
    object recursive volatile
```

The following token forms are reserved, except when they are part of a symbolic keyword ([§](lexical-analysis.md#symbolic-keywords)).

```fsgrammar
token reserved-ident-formats =
    | ident-text ( '!' | '#')
```

In the remainder of this specification, we refer to the token that is generated for a keyword simply
by using the text of the keyword itself.

## Strings and Characters

String literals may be specified for two types:

- Unicode strings, type `string = System.String`
- Unsigned byte arrays, type `byte[] = bytearray`

Literals may also be specified by using C#-like verbatim forms that interpret `\` as a literal character
rather than an escape sequence. In a UTF-8-encoded file, you can directly embed the following in a
string in the same way as in C#:

- Unicode characters, such as “`\u0041bc`”
- Identifiers, as described in the previous section, such as “`abc`”
- Trigraph specifications of Unicode characters, such as “`\067`” which represents “`C`”

```fsgrammar
regexp escape-char = '\' ["\'ntbrafv]
regexp non-escape-chars = '\' [^"\'ntbrafv]
regexp simple-char-char =
    | (any char except '\n' '\t' '\r' '\b' '\a' '\f' '\v' ' \ ")

regexp unicodegraph-short = '\' 'u' hexdigit hexdigit hexdigit hexdigit
regexp unicodegraph-long =  '\' 'U' hexdigit hexdigit hexdigit hexdigit
                                    hexdigit hexdigit hexdigit hexdigit

regexp trigraph = '\' digit-char digit-char digit-char

regexp char-char =
    | simple-char-char
    | escape-char
    | trigraph
    | unicodegraph-short

regexp string-char =
    | simple-string-char
    | escape-char
    | non-escape-chars
    | trigraph
    | unicodegraph-short
    | unicodegraph-long
    | newline

regexp string-elem =
    | string-char
    | '\' newline whitespace * string-elem

token char = ' char-char '
token string = " string-char * "

regexp verbatim-string-char =
    | simple-string-char
    | non-escape-chars
    | newline
    | \
    | ""

token verbatim-string = @" verbatim-string-char * "

token bytechar = ' simple-or-escape-char 'B
token bytearray = " string-char * "B
token verbatim-bytearray = @" verbatim-string-char * "B
token simple-or-escape-char = escape-char | simple-char
token simple-char = any char except newline,return,tab,backspace,',\,"

token triple-quoted-string = """ simple-or-escape-char* """
```

To translate a string token to a string value, the F# parser concatenates all the Unicode characters
for the `string-char` elements within the string. Strings may include `\n` as a newline character.
However, if a line ends with `\`, the newline character and any leading whitespace elements on the
subsequent line are ignored. Thus, the following gives `s` the value `"abcdef"`:

```fsharp
let s = "abc\
    def"
```

Without the backslash, the resulting string includes the newline and whitespace characters. For
example:

```fsharp
let s = "abc
    def"
```

In this case, s has the value `abc\010    def` where `\010` is the embedded control character for `\n`,
which has Unicode UTF-16 value 10.

Verbatim strings may be specified by using the `@` symbol preceding the string as in C#. For example,
the following assigns the value `"abc\def"` to `s`.

```fsharp
let s = @"abc\def"
```

String-like and character-like literals can also be specified for unsigned byte arrays (type `byte[]`).
These tokens cannot contain Unicode characters that have surrogate-pair UTF-16 encodings or UTF-
16 encodings greater than 127.

A triple-quoted string is specified by using three quotation marks (`"""`) to ensure that a string that
includes one or more escaped strings is interpreted verbatim. For example, a triple-quoted string can
be used to embed XML blobs:

```fsharp
let catalog = """
<?xml version="1.0"?>
<catalog>
    <book id="book">
        <author>Author</author>
        <title>F#</title>
        <genre>Computer</genre>
        <price>44.95</price>
        <publish_date>2012- 10 - 01</publish_date>
        <description>An in-depth look at creating applications in F#</description>
    </book>
</catalog>
"""
```

## Symbolic Keywords

The following symbolic or partially symbolic character sequences are treated as keywords:

```fsgrammar
token symbolic-keyword =
    let! use! do! yield! return! match!
    | -> <-. : ( ) [ ] [< >] [| |] { }
    ' # :?> :? :> .. :: := ;; ; =
    _? ?? (*) <@ @> <@@ @@>
```

The following symbols are reserved for future use:

```fsgrammar
token reserved-symbolic-sequence =
    ~ `
```

## Symbolic Operators

User-defined and library-defined symbolic operators are sequences of characters as shown below,
except where the sequence of characters is a symbolic keyword ([§](lexical-analysis.md#symbolic-keywords)).

```fsgrammar
regexp first-op-char = !%&*+-./<=>@^|~
regexp op-char = first-op-char |?

token quote-op-left =
    | <@ <@@

token quote-op-right =
    | @> @@>

token symbolic-op =
    |?
    | ?<-
    | first-op-char op-char *
    | quote-op-left
    | quote-op-right
```

For example, `&&&` and `|||` are valid symbolic operators. Only the operators `?` and `?<-` may start with
`?`.

The `quote-op-left` and `quote-op-right` operators are used in quoted expressions ([§](expressions.md#quoted-expressions)).

For details about the associativity and precedence of symbolic operators in expression forms, see
§4.4.

## Numeric Literals

The lexical specification of numeric literals is as follows:

```fsgrammar
regexp digit = [0-9]
regexp hexdigit = digit | [A-F] | [a-f]
regexp octaldigit = [0-7]
regexp bitdigit = [0-1]

regexp int =
    | digit+                                     For example, 34
    | int _ int                                  For example, 123_456_789

regexp hexint =
    | hexdigit+                                  For example, 1f
    | hexint _ hexint                            For example, abc_456_78

regexp octalint =
    | octaldigit+                                For example, 34
    | octalint _ octalint                        For example, 123_4_711

regexp bitint =
    | bitdigit+                                  For example, 11
    | bitint _ bitint                            For example, 1110_0101_0100

regexp xint =
    | 0 (x|X) hexint                             For example, 0x22_1f
    | 0 (o|O) octalint                           For example, 0o42
    | 0 (b|B) bitint                             For example, 0b10010

token sbyte = ( int | xint ) 'y'                 For example, 34y
token byte = ( int | xint ) ' uy'                For example, 34uy
token int16 = ( int | xint ) 's'                 For example, 34s
token uint16 = ( int | xint ) 'us'               For example, 34us
token int32 = ( int | xint ) 'l'                 For example, 34l
token uint32 = ( int | xint ) 'ul'               For example, 34ul
             | ( int | xint ) 'u'                For example, 34u
token nativeint = ( int | xint ) 'n'             For example, 34n
token unativeint = ( int | xint ) 'un'           For example, 34un
token int64 = ( int | xint ) 'L'                 For example, 34L
token uint64 = ( int | xint ) 'UL'               For example, 34UL
             | ( int | xint ) 'uL'               For example, 34uL

token float =
    int . int?
    int (. int?)? (e|E) (+|-)? int

token ieee32 =
    | float [Ff]                                 For example, 3.0F or 3.0f
    | xint 'lf'                                  For example, 0x00000000lf
token ieee64 =
    | float                                      For example, 3.0
    | xint 'LF'                                  For example, 0x0000000000000000LF

token bignum = int ('Q' | ' R' | 'Z' | 'I' | 'N' | 'G')
                                                 For example, 34742626263193832612536171N

token decimal = ( float | int ) [Mm]
```

### Post-filtering of Adjacent Prefix Tokens

Negative integers are specified using the `–` token; for example, `-3`. The token steam is post-filtered
according to the following rules:

- If the token stream contains the adjacent tokens `– token`:

  If `token` is a constant numeric literal, the pair of tokens is merged. For example, adjacent tokens
  `-` and `3` becomes the single token `-3`. Otherwise, the tokens remain separate. However the `-`
  token is marked as an `ADJACENT_PREFIX_OP` token.

  This rule does not apply to the sequence `token1 - token2`, if all three tokens are adjacent and
  `token1` is a terminating token from expression forms that have lower precedence than the
  grammar production `expr = MINUS expr`.

  For example, the `–` and `b` tokens in the following sequence are not merged if all three tokens
  are adjacent:

```fsharp
    a-b
```

- Otherwise, the usual grammar rules apply to the uses of `–` and `+`, with an addition for
  `ADJACENT_PREFIX_OP`:

```fsgrammar
  expr = expr MINUS expr
      | MINUS expr
      | ADJACENT_PREFIX_OP expr
```

### Post-filtering of Integers Followed by Adjacent “..”

Tokens of the form

```fsgrammar
token intdotdot = int..
```

such as `34..` are post-filtered to two tokens: one `int` and one `symbolic-keyword` , “`..`”.

This rule allows “`..`” to immediately follow an integer. This construction is used in expressions of the
form `[for x in 1..2 -> x + x ]`. Without this rule, the longest-match rule would consider this
sequence to be a floating-point number followed by a “`.`”.

### Reserved Numeric Literal Forms

The following token forms are reserved for future numeric literal formats:

```fsgrammar
token reserved-literal-formats =
    | (xint | ieee32 | ieee64) ident-char+
```

### Shebang

A shebang (#!) directive may exist at the beginning of F# source files. Such a line is treated as a
comment. This allows F# scripts to be compatible with the Unix convention whereby a script
indicates the interpreter to use by providing the path to that interpreter on the first line, following
the #! directive.

```fsharp
#!/bin/usr/env fsharpi --exec
```

## Line Directives

Line directives adjust the source code filenames and line numbers that are reported in error
messages, recorded in debugging symbols, and propagated to quoted expressions. F# supports the
following line directives:

```fsgrammar
token line-directive =
    # int
    # int string
    # int verbatim-string
    #line int
    #line int string
    #line int verbatim-string
```

A line directive applies to the line that immediately follows the directive. If no line directive is
present, the first line of a file is numbered 1.

## Hidden Tokens

Some hidden tokens are inserted by lexical filtering (§ 15 ) or are used to replace existing tokens. See
§ 15 for a full specification and for the augmented grammar rules that take these into account.

## Identifier Replacements

The following table lists identifiers that are automatically replaced by expressions.

| Identifier | Replacement |
| --- | --- |
| `__SOURCE_DIRECTORY__` | A literal verbatim string that specifies the name of the directory that contains the <br> current file. For example:<br>`C:\source`<br>The name of the current file is derived from the most recent line directive in the file. If no line directive has appeared, the name is derived from the name that was specificed to the command-line compiler in combination with<br> `System.IO.Path.GetFullPath`.<br> In F# Interactive, the name `stdin` is used. When F# Interactive is used from tools such as Visual Studio, a line directive is implicitly added before the interactive execution of each script fragment. |
| `__SOURCE_FILE__` | A literal verbatim string that contains the name of the current file. For example:<br>`file.fs` |
| `__LINE__`| A literal string that specifies the line number in the source file, after taking into account adjustments from line directives. |
