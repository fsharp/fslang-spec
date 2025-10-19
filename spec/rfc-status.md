# RFC status

| Version | Feature ID | Feature with RFC Link | Status |
|---------|------------|---------------------|------|
| F# 4.0 | N/A | [Auto Quotation](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/AutoQuotationDesignAndSpec.md) | This was already covered (see [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#81373-conversion-to-quotation-values) and the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-quotations-fsharpexpr.html#ValueWithName)) |
| F# 4.0 | N/A | [Class Names As Functions](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ClassNamesAsFunctionsDesignAndSpec.md) | [completed](https://github.com/fsharp/fslang-spec/pull/55) |
| F# 4.0 | N/A | [Core Library Functions](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/CoreLibraryFunctions.md) | This was replaced by the RFC "List, Seq, Array Additions", see below |
| F# 4.0 | N/A | [Extended If Grammar](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ExtendedIfGrammarDesignAndSpec.md) | This was already covered (see [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#33-conditional-compilation)) |
| F# 4.0 | N/A | [Extension Property Initializers](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ExtensionPropertyInitializersDesignAndSpec.md) | This was already covered (see [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#144-method-application-resolution), item 4, 5th bullet, second sub-bullet, second sentence) |
| F# 4.0 | N/A | [List, Seq, Array Additions](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ListSeqArrayAdditions.md) | This is documented in the [FSharp.Core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections.html). FSharp.Core functionality is not documented in detail in this spec, except as needed for reference (see [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#18-the-f-library-fsharpcoredll)) |
| F# 4.0 | N/A | [Microsoft Optional](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/MicrosoftOptionalDesignAndSpec.md) | Both [library documentation](https://fsharp.github.io/fsharp-core-docs) and [spec](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#18-the-f-library-fsharpcoredll) were adapted already. |
| F# 4.0 | N/A | [Multi-Interface Instantiation](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/MultiInterfaceInstantiationDesignAndSpec.md) | included in FS-1031 (see below) |
| F# 4.0 | N/A | [Non-Null Provided Types](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/NonNullProvidedTypesDesignAndSpec.md) | This was already covered (see [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#1634-Kind)) |
| F# 4.0 | N/A | [Printf Units of Measure](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/PrintfUnitsOfMeasureDesignAndSpec.md) | [completed](https://github.com/fsharp/fslang-spec/pull/58) |
| F# 4.0 | N/A | [Static Method Arguments](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/StaticMethodArgumentsDesignAndSpec.md) | This was already covered (see [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#161-Static-Parameters)) |
| F# 4.1 | FS-1002 | [Cartesian Product for Collections](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1002-cartesian-product-for-collections.md) | This specifies a detail of the `FSharp.Core` library, which is not part of this spec (except for the basic items listed in [§](https://github.com/fsharp/fslang-spec/blob/main/releases/FSharp-Spec-latest.md#18-the-f-library-fsharpcoredll)) |
| F# 4.1 | FS-1004 | [Result Type](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1004-result-type.md) | [completed](https://github.com/fsharp/fslang-spec/pull/50/commits/7ab45f76974000ae8a4c801a80eee81dd8e2928b) |
| F# 4.1 | FS-1005 | [Underscores in Numeric Literals](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1005-underscores-in-numeric-literals.md) | [completed](https://github.com/fsharp/fslang-spec/pull/29/commits/2bd80c8cdb2f89ccadc9dac038568d3089d8e1dc) |
| F# 4.1 | FS-1006 | [Struct Tuples](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1006-struct-tuples.md) | [completed](https://github.com/fsharp/fslang-spec/pull/30/commits) |
| F# 4.1 | FS-1007 | [Additional Option Module Functions](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1007-additional-Option-module-functions.md) | This specifies some details of the `FSharp.Core` library, which is not part of this spec (except for the basic items listed in [§18](https://fsharp.github.io/fslang-spec/the-f-library-fsharpcoredll/)) |
| F# 4.1 | FS-1008 | [Struct Records](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1008-struct-records.md) | |
| F# 4.1 | FS-1009 | [Mutually Referential Types and Modules in Single Scope](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1009-mutually-referential-types-and-modules-single-scope.md) | |
| F# 4.1 | FS-1010 | [Add Map.count](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1010-add-map-count.md) | This specifies some details of the `FSharp.Core` library, which is not part of this spec (except for the basic items listed in [§18](https://fsharp.github.io/fslang-spec/the-f-library-fsharpcoredll/)) |
| F# 4.1 | FS-1012 | [Caller Info Attributes](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1012-caller-info-attributes.md) | [completed](https://github.com/fsharp/fslang-spec/pull/74) |
| F# 4.1 | FS-1013 | [Enable Reflection Functionality on Portable Profiles](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1013-enable-reflection-functionality-on-portable-profiles.md) | |
| F# 4.1 | FS-1014 | [Struct Discriminated Unions](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1014-struct-discriminated-unions.md) | |
| F# 4.1 | FS-1015 | [Support for fixed](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1015-support-for-fixed.md) | [completed](https://github.com/fsharp/fslang-spec/pull/65) |
| F# 4.1 | FS-1016 | [Unreserve Keywords](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1016-unreserve-keywords.md) | [completed](https://github.com/fsharp/fslang-spec/pull/63) |
| F# 4.1 | FS-1017 | [Fix SRTP Constraint Parsing](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1017-fix-srtp-constraint-parsing.md) | |
| F# 4.1 | FS-1018 | [Adjust Extensions Method Scope](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1018-adjust-extensions-method-scope.md) | |
| F# 4.1 | FS-1019 | [Implicitly Add the Module Suffix](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1019-implicitly-add-the-module-suffix.md) | [completed](https://github.com/fsharp/fslang-spec/pull/77) |
| F# 4.1 | FS-1020 | [ByRef Returns](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1020-byref-returns.md) | |
| F# 4.1 | FS-1025 | [Improve Record Type Inference](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1025-improve-record-type-inference.md) | |
| F# 4.1 | FS-1027 | [Complete Optional DefaultParameterValue](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1027-complete-optional-defaultparametervalue.md) | [completed](https://github.com/fsharp/fslang-spec/pull/75) |
| F# 4.1 | FS-1029 | [Implement IReadOnlyCollection in list](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1029-Implement%20IReadOnlyCollection%20in%20list.md) | |
| F# 4.1 | FS-1040 | [Enum Match](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1040-enum-match.md) | |
| F# 4.5 | FS-1047 | [Match Bang](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1047-match-bang.md) | [completed](https://github.com/fsharp/fslang-spec/pull/67) |
| F# 4.5 | FS-1053 | [Span](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1053-span.md) | |
| F# 4.5 | FS-1054 | [Undent List Args](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1054-undent-list-args.md) | |
| F# 4.5 | FS-1055 | [Subsumption for Yield in Sequence Expression](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1055-subsumption-for-yield-in-sequence-expression.md) | |
| F# 4.5 | FS-1058 | [Make Enum Cases Public](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1058-make-enum-cases-public.md) | |
| F# 4.6 | FS-1030 | [Anonymous Records](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.6/FS-1030-anonymous-records.md) | |
| F# 4.7 | FS-1046 | [Wildcard Self Identifiers](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.7/FS-1046-wildcard-self-identifiers.md) | |
| F# 4.7 | FS-1069 | [Implicit Yields](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.7/FS-1069-implicit-yields.md) | |
| F# 4.7 | FS-1070 | [Offside Relaxations](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.7/FS-1070-offside-relaxations.md) | |
| F# 5.0 | FS-1001 | [String Interpolation](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1001-StringInterpolation.md) | |
| F# 5.0 | FS-1003 | [Nameof Operator](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1003-nameof-operator.md) | |
| F# 5.0 | FS-1031 | [Allow Implementing Same Interface at Different Generic Instantiations](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1031-Allow%20implementing%20the%20same%20interface%20at%20different%20generic%20instantiations%20in%20the%20same%20type.md) | [completed](https://github.com/fsharp/fslang-spec/pull/57) |
| F# 5.0 | FS-1063 | [Support LetBang and AndBang for Applicative Functors](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1063-support-letbang-andbang-for-applicative-functors.md) | |
| F# 5.0 | FS-1068 | [Open Type Declaration](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1068-open-type-declaration.md) | |
| F# 5.0 | FS-1071 | [Witness Passing Quotations](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1071-witness-passing-quotations.md) | |
| F# 5.0 | FS-1074 | [Default Interface Member Consumption](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1074-default-interface-member-consumption.md) | |
| F# 5.0 | FS-1075 | [Nullable Interop](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1075-nullable-interop.md) | |
| F# 5.0 | FS-1077 | [3D/4D Fixed Index Slicing](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1077-3d-4d-fixed-index-slicing.md) | |
| F# 5.0 | FS-1077 | [Tolerant Slicing](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1077-tolerant-slicing.md) | |
| F# 5.0 | FS-1080 | [Float32 Without Dot](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1080-float32-without-dot.md) | [completed](https://github.com/fsharp/fslang-spec/pull/73) |
| F# 5.0 | FS-1082 | [Uint Type Abbreviation](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1082-uint-type-abbreviation.md) | |
| F# 5.0 | FS-1085 | [Nameof Pattern](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1085-nameof-pattern.md) | |
| F# 5.0 | FS-1089 | [Allow String Everywhere](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1089-allow-string-everywhere.md) | |
| F# 6.0 | FS-1039 | [Struct Representation for Active Patterns](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1039-struct-representation-for-active-patterns.md) | |
| F# 6.0 | FS-1056 | [Allow Custom Operation Overloads](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1056-allow-custom-operation-overloads.md) | |
| F# 6.0 | FS-1087 | [Resumable Code](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1087-resumable-code.md) | |
| F# 6.0 | FS-1091 | [Extend Units of Measure](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1091-Extend-Units-of-Measure.md) | [completed](https://github.com/fsharp/fslang-spec/pull/37/commits) |
| F# 6.0 | FS-1093 | [Additional Conversions](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1093-additional-conversions.md) | |
| F# 6.0 | FS-1097 | [Task Builder](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1097-task-builder.md) | |
| F# 6.0 | FS-1098 | [Inline If Lambda](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1098-inline-if-lambda.md) | |
| F# 6.0 | FS-1099 | [List Collector](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1099-list-collector.md) | |
| F# 6.0 | FS-1100 | [Printf Binary](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1100-Printf-binary.md) | |
| F# 6.0 | FS-1102 | [Discards on Use Bindings](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1102-discards-on-use-bindings.md) | |
| F# 6.0 | FS-1104 | [Struct Representations](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1104-struct-representations.md) | |
| F# 6.0 | FS-1105 | [Non-variable Patterns to the Right of As Patterns](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1105-Non-variable-patterns-to-the-right-of-as-patterns.md) | |
| F# 6.0 | FS-1107 | [Allow Attributes After the Module Keyword](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1107-Allow-attributes-after-the-module-keyword.md) | |
| F# 6.0 | FS-1108 | [Undentation Frenzy](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1108-undentation-frenzy.md) | |
| F# 6.0 | FS-1109 | [Additional Intrinsics for the NativePtr Module](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1109-Additional-intrinsics-for-the-NativePtr-module.md) | |
| F# 6.0 | FS-1110 | [Index Syntax](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1110-index-syntax.md) | |
| F# 6.0 | FS-1111 | [RefCell Op Information Messages](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1111-refcell-op-information-messages.md) | |
| F# 6.0 | FS-1113 | [Insert Remove Update Functions](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1113-insert-remove-update-functions.md) | |
| F# 6.0 | FS-1114 | [ML Compat Revisions](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1114-ml-compat-revisions.md) | |
| F# 7.0 | FS-1024 | [Simplify Constrained Call Syntax](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1024-simplify-constrained-call-syntax.md) | |
| F# 7.0 | FS-1083 | [SRTP Type No Whitespace](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1083-srtp-type-no-whitespace.md) | |
| F# 7.0 | FS-1096 | [Map Min Max KeyValue](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1096-map-min-max-keyvalue.md) | |
| F# 7.0 | FS-1123 | [Result Module Parity with Option](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1123-result-module-parity-with-option.md) | |
| F# 7.0 | FS-1124 | [Interfaces with Static Abstract Members](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1124-interfaces-with-static-abstract-members.md) | |
| F# 7.0 | FS-1126 | [Allow Lower Case DU Cases when RequireQualifiedAccess is Specified](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1126-allow-lower-case-du-cases%20when-require-qualified-access-is%20specified.md) | |
| F# 7.0 | FS-1127 | [Init Only and Required Properties](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1127-init-only-and-required-properties.md) | |
| F# 7.0 | FS-1131 | [NoCompilerInliningAttribute](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1131-NoCompilerInliningAttribute.md) | |
| F# 7.0 | FS-1133 | [Arithmetic in Literals](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1133-arithmetic-in-literals.md) | |
| F# 8.0 | FS-1011 | [Warn on Recursive Without Tail Call](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1011-warn-on-recursive-without-tail-call.md) | |
| F# 8.0 | FS-1081 | [Extended Fixed Bindings](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1081-extended-fixed-bindings.md) | |
| F# 8.0 | FS-1128 | [Allow Static Members in Interfaces](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1128-allow-static-members-in-interfaces.md) | |
| F# 8.0 | FS-1129 | [Shorthand Anonymous Unary Functions](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1129-shorthand-anonymous-unary-functions.md) | |
| F# 8.0 | FS-1132 | [Better Interpolated Triple Quoted Strings](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1132-better-interpolated-triple-quoted-strings.md) | |
| F# 8.0 | FS-1134 | [Try With in Sequence Expressions](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1134-try-with-in-sequence-expressions.md) | |
| F# 8.0 | FS-1136 | [Constraint Intersection Syntax](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1136-constraint-intersection-syntax.md) | |
| F# 9.0 | FS-1060 | [Nullable Reference Types](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1060-nullable-reference-types.md) | |
| F# 9.0 | FS-1079 | [Union Properties Visible](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1079-union-properties-visible.md) | |
| F# 9.0 | FS-1140 | [Boolean Returning and Return Type Directed Partial Active Patterns](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1140-boolean-returning-and-return-type-directed-partial-active-patterns.md) | |
| F# 9.0 | FS-1144 | [Empty Bodied Computation Expressions](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1144-empty-bodied-computation-expressions.md) | |
| F# 9.0 | FS-1147 | [Non-String Directive Args](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1147-non-string-directive-args.md) | |
