# Introduction
F# is a scalable, succinct, type-safe, type-inferred, efficiently executing functional/imperative/object-oriented programming language. It aims to be the premier typed functional programming language for .NET and other implementations of the [ECMA-335 Common Language Infrastructure (CLI) specification](https://ecma-international.org/publications-and-standards/standards/ecma-335/). F# was partly inspired by the [OCaml language](https://ocaml.org/) and shares some common core constructs with it.

## A First Program
Over the next few sections, we will look at some small F# programs, describing some important aspects of F# along the way. As an introduction to F#, consider the following program:

```fsharp
let numbers = [ 1 .. 10 ]
let square x = x * x
let squares = List.map square numbers
printfn "N^2 = %A" squares
```

To explore this program, you can:

- Compile it as a project in a development environment such as Visual Studio.
- Manually invoke the F# command line compiler fsc.exe.
- Use F# Interactive, the dynamic compiler that is part of the F# distribution.

### Lightweight Syntax

The F# language uses simplified, indentation-aware syntactic constructs known as lightweight syntax. The lines of the sample program in the previous section form a sequence of declarations and are aligned on the same column. For example, the two lines in the following code are two separate declarations:

```fsharp
let squares = List.map square numbers
printfn "N^2 = %A" squares
```

Lightweight syntax applies to all the major constructs of the F# syntax. In the next example, the code is incorrectly aligned. The declaration starts in the first line and continues to the second and subsequent lines, so those lines must be indented to the same column under the first line:

```fsharp
    let computeDerivative f x =
        let p1 = f (x - 0.05)
      let p2 = f (x + 0.05)
           (p2 - p1) / 0.1
```

The following shows the correct alignment:

```fsharp
    let computeDerivative f x =
        let p1 = f (x - 0.05)
        let p2 = f (x + 0.05)
        (p2 - p1) / 0.1
```

The use of lightweight syntax is the default for all F# code in files with the extension .fs, .fsx, .fsi, or .fsscript.

### Making Data Simple

The first line in our sample simply declares a list of numbers from one through ten.

```fsharp
    let numbers = [1 .. 10]
```

An F# list is an immutable linked list, which is a type of data used extensively in functional programming. Some operators that are related to lists include `::` to add an item to the front of a list and `@` to concatenate two lists. If we try these operators in F# Interactive, we see the following results:

```fsharp
> let vowels = ['e'; 'i'; 'o'; 'u'];;
val vowels: char list = ['e'; 'i'; 'o'; 'u']

> ['a'] @ vowels;;
val it: char list = ['a'; 'e'; 'i'; 'o'; 'u']
    
> vowels @ ['y'];;
val it: char list = ['e'; 'i'; 'o'; 'u'; 'y']
```

Note that double semicolons delimit lines in F# Interactive, and that F# Interactive prefaces the result with val to indicate that the result is an immutable value, rather than a variable.

F# supports several other highly effective techniques to simplify the process of modeling and manipulating data such as tuples, options, records, unions, and sequence expressions. A tuple is an ordered collection of values that is treated as an atomic unit. In many languages, if you want to pass around a group of related values as a single entity, you need to create a named type, such as a class or record, to store these values. A tuple allows you to keep things organized by grouping related values together, without introducing a new type.

To define a tuple, you separate the individual components with commas:

    > let tuple = (1, false, "text");;
    val tuple : int * bool * string = (1, false, "text")

    > let getNumberInfo (x : int) = (x, x.ToString(), x * x);;
    val getNumberInfo : int -> int * string * int

    > getNumberInfo 42;;
    val it : int * string * int = (42, "42", 1764)

A key concept in F# is immutability. Tuples and lists are some of the many types in F# that are immutable, and indeed most things in F# are immutable by default. Immutability means that once a value is created and given a name, the value associated with the name cannot be changed. Immutability has several benefits. Most notably, it prevents many classes of bugs, and immutable data is inherently thread-safe, which makes the process of parallelizing code simpler.

### Making Types Simple

The next line of the sample program defines a function called `square`, which squares its input.

```fsharp
    let square x = x * x
```

Most statically-typed languages require that you specify type information for a function declaration. However, F# typically infers this type information for you. This process is referred to as *type inference*.

From the function signature, F# knows that `square` takes a single parameter named `x` and that the function returns `x * x`. The last thing evaluated in an F# function body is the return value; hence there is no "return" keyword here. Many primitive types support the multiplication (*) operator (such as `byte`, `uint64`, and `double`); however, for arithmetic operations, F# infers the type `int` (a signed 32-bit integer) by default.

Although F# can typically infer types on your behalf, occasionally you must provide explicit type annotations in F# code. For example, the following code uses a type annotation for one of the parameters to tell the compiler the type of the input.

    > let concat (x : string) y = x + y;;
    val concat : string -> string -> string

Because `x` is stated to be of type `string`, and the only version of the `+` operator that accepts a left-hand argument of type `string` also takes a `string` as the right-hand argument, the F# compiler infers that the parameter `y` must also be a string. Thus, the result of `x + y` is the concatenation of the strings. Without the type annotation, the F# compiler would not have known which version of the `+` operator was intended and would have assumed `int` data by default.

The process of type inference also applies *automatic generalization* to declarations. This automatically makes code *generic* when possible, which means the code can be used on many types of data. For example, the following code defines a function that returns a new tuple in which the two values are swapped:

    > let swap (x, y) = (y, x);;
    val swap : 'a * 'b -> 'b * 'a

    > swap (1, 2);;
    val it : int * int = (2, 1)

    > swap ("you", true);;
    val it : bool * string = (true,"you")

Here the function `swap` is generic, and `'a` and `'b` represent type variables, which are placeholders for types in generic code. Type inference and automatic generalization greatly simplify the process of writing reusable code fragments.

### Functional Programming

Continuing with the sample, we have a list of integers named `numbers`, and the `square` function, and we want to create a new list in which each item is the result of a call to our function. This is called *mapping* our function over each item in the list. The F# library function `List.map` does just that:

```fsharp
    let squares = List.map square numbers
```

Consider another example:

    > List.map (fun x -> x % 2 = 0) [1 .. 5];;
    val it : bool list = [false; true; false; true; false]

The code `(fun x -> x % 2 = 0)` defines an anonymous function, called a *function expression*, that takes a single parameter `x` and returns the result `x % 2 = 0`, which is a Boolean value that indicates whether `x` is even. The `->` symbol separates the argument list (`x`) from the function body (`x % 2 = 0`).

Both of these examples pass a function as a parameter to another function — the first parameter to `List.map` is itself another function. Using functions as *function values* is a hallmark of functional programming.

Another tool for data transformation and analysis is *pattern matching*. This powerful switch construct allows you to branch control flow and to bind new values. For example, we can match an F# list against a sequence of list elements.

```fsharp
    let checkList alist =
        match alist with
        | [] -> 0
        | [a] -> 1
        | [a; b] -> 2
        | [a; b; c] -> 3
        | _ -> failwith "List is too big!"
```

In this example, `alist` is compared with each potentially matching pattern of elements. When `alist` matches a pattern, the result expression is evaluated and is returned as the value of the match expression. Here, the `->` operator separates a pattern from the result that a match returns.

Pattern matching can also be used as a control construct — for example, by using a pattern that performs a dynamic type test:

```fsharp
    let getType (x : obj) =
        match x with
        | :? string -> "x is a string"
        | :? int -> "x is an int"
        | :? System.Exception -> "x is an exception"
```

The `:?` operator returns true if the value matches the specified type, so if `x` is a string, `getType` returns `"x is a string"`.

Function values can also be combined with the *pipeline operator*, `|>`. For example, given these functions:

```fsharp
    let square x = x * x
    let toStr (x : int) = x.ToString()
    let reverse (x : string) = new System.String(Array.rev (x.ToCharArray()))
```

We can use the functions as values in a pipeline:

    > let result = 32 |> square |> toStr |> reverse;;
    val it : string = "4201"

Pipelining demonstrates one way in which F# supports *compositionality*, a key concept in functional programming. The pipeline operator simplifies the process of writing compositional code where the result of one function is passed into the next.

### Imperative Programming

The next line of the sample program prints text in the console window.

```fsharp
    printfn "N^2 = %A" squares
```

The F# library function `printfn` is a simple and type-safe way to print text in the console window. Consider this example, which prints an integer, a floating-point number, and a string:

    > printfn "%d * %f = %s" 5 0.75 ((5.0 * 0.75).ToString());;
    5 * 0.750000 = 3.75
    val it : unit = ()

The format specifiers `%d`, `%f`, and `%s` are placeholders for integers, floats, and strings. The `%A` format can be used to print arbitrary data types (including lists).

The `printfn` function is an example of *imperative programming*, which means calling functions for their side effects. Other commonly used imperative programming techniques include arrays and dictionaries (also called hash tables). F# programs typically use a mixture of functional and imperative techniques.

### .NET Interoperability and CLI Fidelity

The Common Language Infrastructure (CLI) function `System.Console.ReadKey` can be used to pause the program before the console window closes:

```fsharp
    System.Console.ReadKey(true)
```

Because F# is built on top of CLI implementations, you can call any CLI library from F#. Furthermore, other CLI languages can easily use any F# component.

### Parallel and Asynchronous Programming

F# is both a *parallel* and a *reactive* language. During execution, F# programs can have multiple parallel active evaluations and multiple pending reactions, such as callbacks and agents that wait to react to events and messages.

One way to write parallel and reactive F# programs is to use F# *async* expressions. For example, the code below is similar to the original program in [§1.1](introduction.md#a-first-program) except that it computes the Fibonacci function (using a technique that will take some time) and schedules the computation of the numbers in parallel:

```fsharp
    let rec fib x = if x < 2 then 1 else fib(x-1) + fib(x-2)

    let fibs =
        Async.Parallel [ for i in 0..40 -> async { return fib(i) } ]
        |> Async.RunSynchronously

    printfn "The Fibonacci numbers are %A" fibs

    System.Console.ReadKey(true)
```

The preceding code sample shows multiple, parallel, CPU-bound computations.

F# is also a reactive language. The following example requests multiple web pages in parallel, reacts to the responses for each request, and finally returns the collected results.

```fsharp
open System
open System.IO
open System.Net

let http url =
    async {
        let req = WebRequest.Create(Uri url)
        use! resp = req.AsyncGetResponse()
        use stream = resp.GetResponseStream()
        use reader = new StreamReader(stream)
        let contents = reader.ReadToEnd()
        return contents
    }

let sites =
    [ "http://www.bing.com"
      "http://www.google.com"
      "http://www.yahoo.com"
      "http://www.search.com" ]

let htmlOfSites =
    Async.Parallel [ for site in sites -> http site ]
    |> Async.RunSynchronously
```

By using asynchronous workflows together with other CLI libraries, F# programs can implement parallel tasks, parallel I/O operations, and message-receiving agents.

### Strong Typing for Floating-Point Code

F# applies type checking and type inference to floating-point-intensive domains through *units of measure inference and checking*. This feature allows you to type-check programs that manipulate floating-point numbers that represent physical and abstract quantities in a stronger way than other typed languages, without losing any performance in your compiled code. You can think of this feature as providing a type system for floating-point code.

Consider the following example:

```fsharp
    [<Measure>] type kg
    [<Measure>] type m
    [<Measure>] type s

    let gravityOnEarth = 9.81<m/s^2>
    let heightOfTowerOfPisa = 55.86<m>
    let speedOfImpact = sqrt(2.0 * gravityOnEarth * heightOfTowerOfPisa)
```

The `Measure` attribute tells F# that `kg`, `s`, and `m` are not really types in the usual sense of the word, but are used to build units of measure. Here `speedOfImpact` is inferred to have type `float<m/s>`.

### Object-Oriented Programming and Code Organization

The sample program shown at the start of this chapter is a *script*. Although scripts are excellent for rapid prototyping, they are not suitable for larger software components. F# supports the transition from scripting to structured code through several techniques.

The most important of these is *object-oriented programming* through the use of *class type definitions*, *interface type definitions*, and *object expressions*. Object-oriented programming is a primary application programming interface (API) design technique for controlling the complexity of large software projects. For example, here is a class definition for an encoder/decoder object.

```fsharp
    open System

    /// Build an encoder/decoder object that maps characters to an
    /// encoding and back. The encoding is specified by a sequence
    /// of character pairs, for example, [('a','Z'); ('Z','a')]
    type CharMapEncoder(symbols: seq<char*char>) =
        let swap (x, y) = (y, x)

        /// An immutable tree map for the encoding
        let fwd = symbols |> Map.ofSeq

        /// An immutable tree map for the decoding
        let bwd = symbols |> Seq.map swap |> Map.ofSeq
        let encode (s:string) =
            String [| for c in s -> if fwd.ContainsKey(c) then fwd.[c] else c |]

        let decode (s:string) =
            String [| for c in s -> if bwd.ContainsKey(c) then bwd.[c] else c |]

        /// Encode the input string
        member x.Encode(s) = encode s

        /// Decode the given string
        member x.Decode(s) = decode s
```

You can instantiate an object of this type as follows:

```fsharp
    let rot13 (c:char) =
        char(int 'a' + ((int c - int 'a' + 13) % 26))
    let encoder =
        CharMapEncoder( [for c in 'a'..'z' -> (c, rot13 c)])
```

And use the object as follows:

    > "F# is fun!" |> encoder.Encode ;;
    val it : string = "F# vf sha!"

    > "F# is fun!" |> encoder.Encode |> encoder.Decode ;;
    val it : String = "F# is fun!"

An interface type can encapsulate a family of object types:

```fsharp
    open System

    type IEncoding =
        abstract Encode : string -> string
        abstract Decode : string -> string
```

In this example, `IEncoding` is an interface type that includes both `Encode` and `Decode` object types.

Both object expressions and type definitions can implement interface types. For example, here is an object expression that implements the `IEncoding` interface type:

```fsharp
    let nullEncoder =
        { new IEncoding with
            member x.Encode(s) = s
            member x.Decode(s) = s }
```

*Modules* are a simple way to encapsulate code during rapid prototyping when you do not want to spend the time to design a strict object-oriented type hierarchy. In the following example, we place a portion of our original script in a module.

```fsharp
    module ApplicationLogic =
        let numbers n = [1 .. n]
        let square x = x * x
        let squares n = numbers n |> List.map square

    printfn "Squares up to 5 = %A" (ApplicationLogic.squares 5)
    printfn "Squares up to 10 = %A" (ApplicationLogic.squares 10)
    System.Console.ReadKey(true)
```

Modules are also used in the F# library design to associate extra functionality with types. For example, `List.map` is a function in a module.

Other mechanisms aimed at supporting software engineering include *signatures*, which can be used to give explicit types to components, and namespaces, which serve as a way of organizing the name hierarchies for larger APIs.

### Information-rich Programming

F# Information-rich programming addresses the trend toward greater availability of data, services, and information. The key to information-rich programming is to eliminate barriers to working with diverse information sources that are available on the Internet and in modern enterprise environments. Type providers and query expressions are a significant part of F# support for information-rich programming.

The F# Type Provider mechanism allows you to seamlessly incorporate, in a strongly typed manner, data and services from external sources. A *type provider* presents your program with new types and methods that are typically based on the schemas of external information sources. For example, an F# type provider for Structured Query Language (SQL) supplies types and methods that allow programmers to work directly with the tables of any SQL database:

```fsharp
    // Add References to FSharp.Data.TypeProviders, System.Data, and System.Data.   Linq
    type schema = SqlDataConnection<"Data Source=localhost;Integrated   Security=SSPI;">

    let db = schema.GetDataContext()
```

The type provider connects to the database automatically and uses this for IntelliSense and type information.

Query expressions (added in F# 3.0) add the established power of query-based programming against SQL, Open Data Protocol (OData), and other structured or relational data sources. Query expressions provide support for language-Integrated Query (LINQ) in F#, and several query operators enable you to construct more complex queries. For example, we can create a query to filter the customers in the data source:

```fsharp
    let countOfCustomers =
        query {
            for customer in db.Customers do
                where (customer.LastName.StartsWith("N"))
                select (customer.FirstName, customer.LastName)
            }
```

Now it is easier than ever to access many important data sources—including enterprise, web, and cloud—by using a set of built-in type providers for SQL databases and web data protocols. Where necessary, you can create your own custom type providers or reference type providers that others have created. For example, assume your organization has a data service that provides a large and growing number of named data sets, each with its own stable data schema. You may choose to create a type provider that reads the schemas and presents the latest available data sets to the programmer in a strongly typed way.

## Notational Conventions in This Specification

This specification describes the F# language by using a mixture of informal and semiformal techniques. All examples in this specification use lightweight syntax, unless otherwise specified.

Regular expressions are given in the usual notation, as shown in the table:

| Notation          | Meaning                                  |
| ----------------- | ---------------------------------------- |
| regexp+           | One or more occurrences                  |
| regexp*           | Zero or more occurrences                 |
| regexp?           | Zero or one occurrences                  |
| [ char - char ]   | Range of ASCII characters                |
| [ ^ char - char ] | Any characters except those in the range |

Unicode character classes are referred to by their abbreviation as used in CLI libraries for regular expressions—for example, `\Lu` refers to any uppercase letter. The following characters are referred to using the indicated notation:

| Character | Name      | Notation                          |
| --------- | --------- | --------------------------------- |
| \b        | backspace | ASCII/UTF-8/UTF-16/UTF-32 code 08 |
| \n        | newline   | ASCII/UTF-8/UTF-16/UTF-32 code 10 |
| \r        | return    | ASCII/UTF-8/UTF-16/UTF-32 code 13 |
| \t        | tab       | ASCII/UTF-8/UTF-16/UTF-32 code 09 |

Strings of characters that are clearly not a regular expression are written verbatim. Therefore, the following string

    abstract

matches precisely the characters `abstract`.

Where appropriate, apostrophes and quotation marks enclose symbols that are used in the specification of the grammar itself, such as `'<'` and `'|'`. For example, the following regular expression matches `(+)` or `(-)`:

    '(' (+|-) ')'

This regular expression matches precisely the characters `#if`:

    "#if"

Regular expressions are typically used to specify tokens.

    token token-name = regexp

In the grammar rules, the notation `element-name~opt` indicates an optional element. The notation `...` indicates repetition of the preceding non-terminal construct and the separator token. For example, `expr ',' ... ',' expr` means a sequence of one or more `expr` elements separated by commas.
