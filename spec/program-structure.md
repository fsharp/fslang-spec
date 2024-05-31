# Program Structure

The inputs to the F# compiler or the F# Interactive dynamic compiler consist of:

- Source code files, with extensions `.fs`, `.fsi`, `.fsx`, or `.fsscript`.
    - Files with extension `.fs` must conform to grammar element `implementation-file` in [§12.1](program-structure-and-execution.md#implementation-files).
    - Files with extension `.fsi` must conform to grammar element `signature-file` in [§12.2](program-structure-and-execution.md#signature-files).
    - Files with extension `.fsx` or `.fsscript` must conform to grammar element `script-file` in
      [§12.3](program-structure-and-execution.md#script-files).
- Script fragments (for F# Interactive). These must conform to grammar element `script-fragment`.
  Script fragments can be separated by `;;` tokens.
- Assembly references that are specified by command line arguments or interactive directives.
- Compilation parameters that are specified by command line arguments or interactive directives.
- Compiler directives such as `#time`.

The `COMPILED` compilation symbol is defined for input that the F# compiler has processed. The
`INTERACTIVE` compilation symbol is defined for input that F# Interactive has processed.

Processing the source code portions of these inputs consists of the following steps:

1. **Decoding**. Each file and source code fragment is decoded into a stream of Unicode characters, as
   described in the C# specification, sections 2.3 and 2.4. The command-line options may specify a
   code page for this process.
2. **Tokenization**. The stream of Unicode characters is broken into a token stream by the lexical
   analysis described in [§3.](lexical-analysis.md#lexical-analysis)
3. **Lexical Filtering**. The token stream is filtered by a state machine that implements the rules
   described in [§15.](lexical-filtering.md#lexical-filtering) Those rules describe how additional (artificial) tokens are inserted into the
   token stream and how some existing tokens are replaced with others to create an augmented
   token stream.
4. **Parsing**. The augmented token stream is parsed according to the grammar specification in this
   document.
5. **Importing**. The imported assembly references are resolved to F# or CLI assembly specifications,
   which are then imported. From the F# perspective, this results in the pre-definition of numerous
   namespace declaration groups ([§12.1](program-structure-and-execution.md#implementation-files)), types and type provider instances. The namespace
   declaration groups are then combined to form an initial name resolution environment ([§14.1](inference-procedures.md#name-resolution)).
6. **Checking**. The results of parsing are checked one by one. Checking involves such procedures as
   Name Resolution (§14.1), Constraint Solving (§14.5), and Generalization ([§14.6.7](inference-procedures.md#generalization)), as well as the
   application of other rules described in this specification.
   
   Type inference uses variables to represent unknowns in the type inference problem. The various
   checking processes maintain tables of context information including a name resolution
   environment and a set of current inference constraints. After the processing of a file or program
   fragment is complete, all such variables have been either generalized or resolved and the type
   inference environment is discarded.
7. **Elaboration**. One result of checking is an elaborated program fragment that contains elaborated
   declarations, expressions, and types. For most constructs, such as constants, control flow, and
   data expressions, the elaborated form is simple. Elaborated forms are used for evaluation, CLI
   reflection, and the F# expression trees that are returned by quoted expressions ([§6.8](expressions.md#quoted-expressions)).
8. **Execution**. Elaborated program fragments that are successfully checked are added to a
   collection of available program fragments. Each fragment has a static initializer. Static initializers
   are executed as described in ([§12.5](program-structure-and-execution.md#program-execution)).
