# fslang-spec

F# Language Specification

## Overview

This is an initiative to create a more complete and community-maintainable F# spec.

This will be no small task, but we believe it is worthwhile and we count on community contributions.

We foresee three phases:

1. Convert the [latest official spec](https://fsharp.org/specs/language-spec/4.1/FSharpSpec-4.1-latest.pdf) to markdown and create the structure and tools to make it community-maintainable. This is done.
2. Add the post-4.1 features as documented in the [RFCs](https://github.com/fsharp/fslang-design/) to the spec. Our goal: a complete F# 10 spec.
3. Make spec update part of new feature development so that an up-to-date spec can be released with every new major compiler release.

## Process

The spec is in the end closely coupled to the language design and therefore needs a) strong community contributions and b) a clearly defined final responsibility, which will be similar to the one of the [language design process](https://github.com/fsharp/fslang-design?tab=readme-ov-file#who-is-in-charge).

We foresee the following types of contributions:

- Issues and/or PRs for bug fixes.
- PRs for integration of an accepted and implemented RFC.
- Issues for proposing and discussing smaller or larger improvements to the spec
- PRs for such improvements, once the discussion converges and/or is decided by the team in charge

All PRs need to be accepted by two team reviewers for merging.

## F# Language Features RFCs

| Version | Feature ID | Feature Description | Link | Status |
|---------|------------|---------------------|------|-------|
| F# 4.0 | N/A | Auto Quotation | [AutoQuotationDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/AutoQuotationDesignAndSpec.md) |
| F# 4.0 | N/A | Class Names As Functions | [ClassNamesAsFunctionsDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ClassNamesAsFunctionsDesignAndSpec.md) |
| F# 4.0 | N/A | Core Library Functions | [CoreLibraryFunctions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/CoreLibraryFunctions.md) |
| F# 4.0 | N/A | Extended If Grammar | [ExtendedIfGrammarDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ExtendedIfGrammarDesignAndSpec.md) |
| F# 4.0 | N/A | Extension Property Initializers | [ExtensionPropertyInitializersDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ExtensionPropertyInitializersDesignAndSpec.md) |
| F# 4.0 | N/A | List, Seq, Array Additions | [ListSeqArrayAdditions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/ListSeqArrayAdditions.md) |
| F# 4.0 | N/A | Microsoft Optional | [MicrosoftOptionalDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/MicrosoftOptionalDesignAndSpec.md) |
| F# 4.0 | N/A | Multi-Interface Instantiation | [MultiInterfaceInstantiationDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/MultiInterfaceInstantiationDesignAndSpec.md) |
| F# 4.0 | N/A | Non-Null Provided Types | [NonNullProvidedTypesDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/NonNullProvidedTypesDesignAndSpec.md) |
| F# 4.0 | N/A | Printf Units of Measure | [PrintfUnitsOfMeasureDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/PrintfUnitsOfMeasureDesignAndSpec.md) |
| F# 4.0 | N/A | Static Method Arguments | [StaticMethodArgumentsDesignAndSpec.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.0/StaticMethodArgumentsDesignAndSpec.md) |
| F# 4.1 | FS-1002 | Cartesian Product for Collections | [FS-1002-cartesian-product-for-collections.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1002-cartesian-product-for-collections.md) |
| F# 4.1 | FS-1004 | Result Type | [FS-1004-result-type.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1004-result-type.md) |
| F# 4.1 | FS-1005 | Underscores in Numeric Literals | [FS-1005-underscores-in-numeric-literals.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1005-underscores-in-numeric-literals.md) | done |
| F# 4.1 | FS-1006 | Struct Tuples | [FS-1006-struct-tuples.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1006-struct-tuples.md) | done |
| F# 4.1 | FS-1007 | Additional Option Module Functions | [FS-1007-additional-Option-module-functions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1007-additional-Option-module-functions.md) |
| F# 4.1 | FS-1008 | Struct Records | [FS-1008-struct-records.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1008-struct-records.md) |
| F# 4.1 | FS-1009 | Mutually Referential Types and Modules in Single Scope | [FS-1009-mutually-referential-types-and-modules-single-scope.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1009-mutually-referential-types-and-modules-single-scope.md) |
| F# 4.1 | FS-1010 | Add Map.count | [FS-1010-add-map-count.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1010-add-map-count.md) |
| F# 4.1 | FS-1012 | Caller Info Attributes | [FS-1012-caller-info-attributes.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1012-caller-info-attributes.md) |
| F# 4.1 | FS-1013 | Enable Reflection Functionality on Portable Profiles | [FS-1013-enable-reflection-functionality-on-portable-profiles.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1013-enable-reflection-functionality-on-portable-profiles.md) |
| F# 4.1 | FS-1014 | Struct Discriminated Unions | [FS-1014-struct-discriminated-unions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1014-struct-discriminated-unions.md) |
| F# 4.1 | FS-1015 | Support for fixed | [FS-1015-support-for-fixed.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1015-support-for-fixed.md) |
| F# 4.1 | FS-1016 | Unreserve Keywords | [FS-1016-unreserve-keywords.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1016-unreserve-keywords.md) |
| F# 4.1 | FS-1017 | Fix SRTP Constraint Parsing | [FS-1017-fix-srtp-constraint-parsing.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1017-fix-srtp-constraint-parsing.md) |
| F# 4.1 | FS-1018 | Adjust Extensions Method Scope | [FS-1018-adjust-extensions-method-scope.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1018-adjust-extensions-method-scope.md) |
| F# 4.1 | FS-1019 | Implicitly Add the Module Suffix | [FS-1019-implicitly-add-the-module-suffix.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1019-implicitly-add-the-module-suffix.md) |
| F# 4.1 | FS-1020 | ByRef Returns | [FS-1020-byref-returns.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1020-byref-returns.md) |
| F# 4.1 | FS-1025 | Improve Record Type Inference | [FS-1025-improve-record-type-inference.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1025-improve-record-type-inference.md) |
| F# 4.1 | FS-1027 | Complete Optional DefaultParameterValue | [FS-1027-complete-optional-defaultparametervalue.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1027-complete-optional-defaultparametervalue.md) |
| F# 4.1 | FS-1029 | Implement IReadOnlyCollection in list | [FS-1029-Implement IReadOnlyCollection in list.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1029-Implement%20IReadOnlyCollection%20in%20list.md) |
| F# 4.1 | FS-1040 | Enum Match | [FS-1040-enum-match.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.1/FS-1040-enum-match.md) |
| F# 4.5 | FS-1047 | Match Bang | [FS-1047-match-bang.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1047-match-bang.md) |
| F# 4.5 | FS-1053 | Span | [FS-1053-span.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1053-span.md) |
| F# 4.5 | FS-1054 | Undent List Args | [FS-1054-undent-list-args.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1054-undent-list-args.md) |
| F# 4.5 | FS-1055 | Subsumption for Yield in Sequence Expression | [FS-1055-subsumption-for-yield-in-sequence-expression.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1055-subsumption-for-yield-in-sequence-expression.md) |
| F# 4.5 | FS-1058 | Make Enum Cases Public | [FS-1058-make-enum-cases-public.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.5/FS-1058-make-enum-cases-public.md) |
| F# 4.6 | FS-1030 | Anonymous Records | [FS-1030-anonymous-records.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.6/FS-1030-anonymous-records.md) |
| F# 4.7 | FS-1046 | Wildcard Self Identifiers | [FS-1046-wildcard-self-identifiers.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.7/FS-1046-wildcard-self-identifiers.md) |
| F# 4.7 | FS-1069 | Implicit Yields | [FS-1069-implicit-yields.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.7/FS-1069-implicit-yields.md) |
| F# 4.7 | FS-1070 | Offside Relaxations | [FS-1070-offside-relaxations.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-4.7/FS-1070-offside-relaxations.md) |
| F# 5.0 | FS-1001 | String Interpolation | [FS-1001-StringInterpolation.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1001-StringInterpolation.md) |
| F# 5.0 | FS-1003 | Nameof Operator | [FS-1003-nameof-operator.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1003-nameof-operator.md) |
| F# 5.0 | FS-1031 | Allow Implementing Same Interface at Different Generic Instantiations | [FS-1031-Allow implementing the same interface at different generic instantiations in the same type.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1031-Allow%20implementing%20the%20same%20interface%20at%20different%20generic%20instantiations%20in%20the%20same%20type.md) |
| F# 5.0 | FS-1063 | Support LetBang and AndBang for Applicative Functors | [FS-1063-support-letbang-andbang-for-applicative-functors.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1063-support-letbang-andbang-for-applicative-functors.md) |
| F# 5.0 | FS-1068 | Open Type Declaration | [FS-1068-open-type-declaration.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1068-open-type-declaration.md) |
| F# 5.0 | FS-1071 | Witness Passing Quotations | [FS-1071-witness-passing-quotations.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1071-witness-passing-quotations.md) |
| F# 5.0 | FS-1074 | Default Interface Member Consumption | [FS-1074-default-interface-member-consumption.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1074-default-interface-member-consumption.md) |
| F# 5.0 | FS-1075 | Nullable Interop | [FS-1075-nullable-interop.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1075-nullable-interop.md) |
| F# 5.0 | FS-1077 | 3D/4D Fixed Index Slicing | [FS-1077-3d-4d-fixed-index-slicing.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1077-3d-4d-fixed-index-slicing.md) |
| F# 5.0 | FS-1077 | Tolerant Slicing | [FS-1077-tolerant-slicing.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1077-tolerant-slicing.md) |
| F# 5.0 | FS-1080 | Float32 Without Dot | [FS-1080-float32-without-dot.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1080-float32-without-dot.md) |
| F# 5.0 | FS-1082 | Uint Type Abbreviation | [FS-1082-uint-type-abbreviation.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1082-uint-type-abbreviation.md) |
| F# 5.0 | FS-1085 | Nameof Pattern | [FS-1085-nameof-pattern.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1085-nameof-pattern.md) |
| F# 5.0 | FS-1089 | Allow String Everywhere | [FS-1089-allow-string-everywhere.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-5.0/FS-1089-allow-string-everywhere.md) |
| F# 6.0 | FS-1039 | Struct Representation for Active Patterns | [FS-1039-struct-representation-for-active-patterns.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1039-struct-representation-for-active-patterns.md) |
| F# 6.0 | FS-1056 | Allow Custom Operation Overloads | [FS-1056-allow-custom-operation-overloads.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1056-allow-custom-operation-overloads.md) |
| F# 6.0 | FS-1087 | Resumable Code | [FS-1087-resumable-code.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1087-resumable-code.md) |
| F# 6.0 | FS-1091 | Extend Units of Measure | [FS-1091-Extend-Units-of-Measure.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1091-Extend-Units-of-Measure.md) | done |
| F# 6.0 | FS-1093 | Additional Conversions | [FS-1093-additional-conversions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1093-additional-conversions.md) |
| F# 6.0 | FS-1097 | Task Builder | [FS-1097-task-builder.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1097-task-builder.md) |
| F# 6.0 | FS-1098 | Inline If Lambda | [FS-1098-inline-if-lambda.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1098-inline-if-lambda.md) |
| F# 6.0 | FS-1099 | List Collector | [FS-1099-list-collector.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1099-list-collector.md) |
| F# 6.0 | FS-1100 | Printf Binary | [FS-1100-Printf-binary.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1100-Printf-binary.md) |
| F# 6.0 | FS-1102 | Discards on Use Bindings | [FS-1102-discards-on-use-bindings.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1102-discards-on-use-bindings.md) |
| F# 6.0 | FS-1104 | Struct Representations | [FS-1104-struct-representations.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1104-struct-representations.md) |
| F# 6.0 | FS-1105 | Non-variable Patterns to the Right of As Patterns | [FS-1105-Non-variable-patterns-to-the-right-of-as-patterns.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1105-Non-variable-patterns-to-the-right-of-as-patterns.md) |
| F# 6.0 | FS-1107 | Allow Attributes After the Module Keyword | [FS-1107-Allow-attributes-after-the-module-keyword.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1107-Allow-attributes-after-the-module-keyword.md) |
| F# 6.0 | FS-1108 | Undentation Frenzy | [FS-1108-undentation-frenzy.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1108-undentation-frenzy.md) |
| F# 6.0 | FS-1109 | Additional Intrinsics for the NativePtr Module | [FS-1109-Additional-intrinsics-for-the-NativePtr-module.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1109-Additional-intrinsics-for-the-NativePtr-module.md) |
| F# 6.0 | FS-1110 | Index Syntax | [FS-1110-index-syntax.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1110-index-syntax.md) |
| F# 6.0 | FS-1111 | RefCell Op Information Messages | [FS-1111-refcell-op-information-messages.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1111-refcell-op-information-messages.md) |
| F# 6.0 | FS-1113 | Insert Remove Update Functions | [FS-1113-insert-remove-update-functions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1113-insert-remove-update-functions.md) |
| F# 6.0 | FS-1114 | ML Compat Revisions | [FS-1114-ml-compat-revisions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-6.0/FS-1114-ml-compat-revisions.md) |
| F# 7.0 | FS-1024 | Simplify Constrained Call Syntax | [FS-1024-simplify-constrained-call-syntax.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1024-simplify-constrained-call-syntax.md) |
| F# 7.0 | FS-1083 | SRTP Type No Whitespace | [FS-1083-srtp-type-no-whitespace.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1083-srtp-type-no-whitespace.md) |
| F# 7.0 | FS-1096 | Map Min Max KeyValue | [FS-1096-map-min-max-keyvalue.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1096-map-min-max-keyvalue.md) |
| F# 7.0 | FS-1123 | Result Module Parity with Option | [FS-1123-result-module-parity-with-option.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1123-result-module-parity-with-option.md) |
| F# 7.0 | FS-1124 | Interfaces with Static Abstract Members | [FS-1124-interfaces-with-static-abstract-members.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1124-interfaces-with-static-abstract-members.md) |
| F# 7.0 | FS-1126 | Allow Lower Case DU Cases when RequireQualifiedAccess is Specified | [FS-1126-allow-lower-case-du-cases when-require-qualified-access-is specified.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1126-allow-lower-case-du-cases%20when-require-qualified-access-is%20specified.md) |
| F# 7.0 | FS-1127 | Init Only and Required Properties | [FS-1127-init-only-and-required-properties.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1127-init-only-and-required-properties.md) |
| F# 7.0 | FS-1131 | NoCompilerInliningAttribute | [FS-1131-NoCompilerInliningAttribute.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1131-NoCompilerInliningAttribute.md) |
| F# 7.0 | FS-1133 | Arithmetic in Literals | [FS-1133-arithmetic-in-literals.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-7.0/FS-1133-arithmetic-in-literals.md) |
| F# 8.0 | FS-1011 | Warn on Recursive Without Tail Call | [FS-1011-warn-on-recursive-without-tail-call.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1011-warn-on-recursive-without-tail-call.md) |
| F# 8.0 | FS-1081 | Extended Fixed Bindings | [FS-1081-extended-fixed-bindings.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1081-extended-fixed-bindings.md) |
| F# 8.0 | FS-1128 | Allow Static Members in Interfaces | [FS-1128-allow-static-members-in-interfaces.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1128-allow-static-members-in-interfaces.md) |
| F# 8.0 | FS-1129 | Shorthand Anonymous Unary Functions | [FS-1129-shorthand-anonymous-unary-functions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1129-shorthand-anonymous-unary-functions.md) |
| F# 8.0 | FS-1132 | Better Interpolated Triple Quoted Strings | [FS-1132-better-interpolated-triple-quoted-strings.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1132-better-interpolated-triple-quoted-strings.md) |
| F# 8.0 | FS-1134 | Try With in Sequence Expressions | [FS-1134-try-with-in-sequence-expressions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1134-try-with-in-sequence-expressions.md) |
| F# 8.0 | FS-1136 | Constraint Intersection Syntax | [FS-1136-constraint-intersection-syntax.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-8.0/FS-1136-constraint-intersection-syntax.md) |
| F# 9.0 | FS-1060 | Nullable Reference Types | [FS-1060-nullable-reference-types.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1060-nullable-reference-types.md) |
| F# 9.0 | FS-1079 | Union Properties Visible | [FS-1079-union-properties-visible.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1079-union-properties-visible.md) |
| F# 9.0 | FS-1140 | Boolean Returning and Return Type Directed Partial Active Patterns | [FS-1140-boolean-returning-and-return-type-directed-partial-active-patterns.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1140-boolean-returning-and-return-type-directed-partial-active-patterns.md) |
| F# 9.0 | FS-1144 | Empty Bodied Computation Expressions | [FS-1144-empty-bodied-computation-expressions.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1144-empty-bodied-computation-expressions.md) |
| F# 9.0 | FS-1147 | Non-String Directive Args | [FS-1147-non-string-directive-args.md](https://github.com/fsharp/fslang-design/tree/main/FSharp-9.0/FS-1147-non-string-directive-args.md) |

## FSharp.Core RFCs

| Version | Feature ID | Feature Description | Link | Status |
|---------|------------|---------------------|------| ------|
| FSharp.Core 4.4.3.0 | FS-1028 | Implement Async.StartImmediateAsTask | [FS-1028-Implement Async.StartImmediateAsTask.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.4.3.0/FS-1028-Implement%20Async.StartImmediateAsTask.md) |
| FSharp.Core 4.4.3.0 | FS-1036 | Consistent Slicing | [FS-1036-consistent-slicing.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.4.3.0/FS-1036-consistent-slicing.md) |
| FSharp.Core 4.4.3.0 | FS-1041 | IReadOnly | [FS-1041-ireadonly.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.4.3.0/FS-1041-ireadonly.md) |
| FSharp.Core 4.4.3.0 | FS-1042 | Add Seq.transpose | [FS-1042-add-seq-transpose.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.4.3.0/FS-1042-add-seq-transpose.md) |
| FSharp.Core 4.4.3.0 | FS-1044 | Add NativePtr.toByRef | [FS-1044-add-nativeptr-tobyref.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.4.3.0/FS-1044-add-nativeptr-tobyref.md) |
| FSharp.Core 4.5.0.0 | FS-1045 | Func to FSharpFunc Overloads | [FS-1045-func-to-fsharpfunc-overloads.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.5.0.0/FS-1045-func-to-fsharpfunc-overloads.md) |
| FSharp.Core 4.5.0.0 | FS-1050 | TryGetValue Map | [FS-1050-trygetvalue-map.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.5.0.0/FS-1050-trygetvalue-map.md) |
| FSharp.Core 4.5.0.0 | FS-1057 | ValueOption | [FS-1057-valueoption.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.5.0.0/FS-1057-valueoption.md) |
| FSharp.Core 4.5.0.0 | FS-1059 | Improve Async Stack Traces | [FS-1059-improve-async-stack-traces.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.5.0.0/FS-1059-improve-async-stack-traces.md) |
| FSharp.Core 4.6.0 | FS-1065 | ValueOption Parity | [FS-1065-valueoption-parity.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.6.0/FS-1065-valueoption-parity.md) |
| FSharp.Core 4.6.0 | FS-1066 | tryExactlyOne | [FS-1066-tryExactlyOne.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.6.0/FS-1066-tryExactlyOne.md) |
| FSharp.Core 4.7.0 | FS-1067 | Async Sequential | [FS-1067-Async-Sequential.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-4.7.0/FS-1067-Async-Sequential.md) |
| FSharp.Core 8.0 | FS-1130 | Array Parallel Collection Functions More Regular | [FS-1130-array-parallel-collection-functions-more-regular.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-8.0/FS-1130-array-parallel-collection-functions-more-regular.md) |
| FSharp.Core 9.0 | FS-1135 | Random Functions for Collections | [FS-1135-random-functions-for-collections.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-9.0/FS-1135-random-functions-for-collections.md) |
| FSharp.Core 9.0 | FS-1145 | C# Collection Expression Support for Lists and Sets | [FS-1145-csharp-collection-expression-support-for-lists-and-sets.md](https://github.com/fsharp/fslang-design/tree/main/FSharp.Core-9.0/FS-1145-csharp-collection-expression-support-for-lists-and-sets.md) |

## Repository structure

The sources are the markdown files for the chapters (clauses) in the `spec` directory.
Run `build` to create a new complete spec (including ToC and updated reference links) in your `artifacts` directory.

At certain points, releases are created and published in the `releases` directory.
