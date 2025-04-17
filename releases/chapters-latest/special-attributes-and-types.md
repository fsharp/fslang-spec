# 17. Special Attributes and Types

This chapter describes attributes and types that have special significance to the F# compiler.

## 17.1 Custom Attributes Recognized by F#

The following custom attributes have special meanings recognized by the F# compiler. Except where
indicated, the attributes may be used in F# code, in referenced assemblies authored in F#, or in
assemblies that are authored in other CLI languages.

| Attribute | Description |
| --- | --- |
| System.ObsoleteAttribute <br> [\<Obsolete(...)>] | Indicates that the construct is obsolete and gives a warning or error depending on the settings in the attribute. <br>This attribute may be used in both F# and imported assemblies. |
| System.ParamArrayAttribute <br> [\<ParamArray(...)>] | When applied to an argument of a method, indicates that the method can accept a variable number of arguments. <br>This attribute may be used in both F# and imported assemblies.
| System.ThreadStaticAttribute <br> [\<ThreadStatic(...)>] | Marks a mutable static value in a class as thread static. <br>This attribute may be used in both F# and imported assemblies.
| System.ContextStaticAttribute <br> [\<ContextStatic(...)>] | Marks a mutable static value in a class as context static. <br>This attribute may be used in both F# and imported assemblies.
| System.AttributeUsageAttribute <br> [\<AttributeUsage(...)>] | Specifies the attribute usage targets for an attribute. <br>This attribute may be used in both F# and imported assemblies.
| System.Diagnostics.ConditionalAttribute <br> [\<Conditional(...)>] | Emits code to call the method only if the corresponding conditional compilation symbol is defined. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyInformationalVersionAttribute <br> [\<AssemblyInformationalVersion(...)>] | Attaches additional version metadata to the compiled form of the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyFileVersionAttribute <br> [\<AssemblyFileVersion(...)>] | Attaches file version metadata to the compiled form of the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyDescriptionAttribute <br> [\<AssemblyDescription(...)>] | Attaches descriptive metadata to the compiled form of the assembly, such as the “Comments” attribute in the Win32 version resource for the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyTitleAttribute <br> [\<AssemblyTitle(...)>] | Attaches title metadata to the compiled form of the assembly, such as the “ProductName” attribute in the Win32 version resource for the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyCopyrightAttribute <br> [\<AssemblyCopyright(...)>] | Attaches copyright metadata to the compiled form of the assembly, such as the “LegalCopyright” attribute in the Win32 version resource for the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyTrademarkAttribute <br> [\<AssemblyTrademark(...)>] | Attaches trademark metadata to the compiled form of the assembly, such as the “LegalTrademarks” attribute in the Win32 version resource for the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyCompanyAttribute <br> [\<AssemblyCompany(...)>] | Attaches company name metadata to the compiled form of the assembly, such as the “CompanyName” attribute in the Win32 version resource for the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyProductAttribute <br> [\<AssemblyProduct(...)>] | Attaches product name metadata to the compiled form of the assembly, such as the “ProductName” attribute in the Win32 version resource for the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.AssemblyKeyFileAttribute <br> [\<AssemblyKeyFile(...)>] | Indicates to the F# compiler how to sign an assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Reflection.DefaultMemberAttribute <br> [\<DefaultMember(...)>] | When applied to a type, specifies the name of the indexer property for that type. <br>This attribute may be used in both F# and imported assemblies.
| System.Runtime.CompilerServices.InternalsVisibleToAttribute [\<InternalsVisibleTo(...)>] | Directs the F# compiler to permit access to the internals of the assembly. <br>This attribute may be used in both F# and imported assemblies.
| System.Runtime.CompilerServices.TypeForwardedToAttribute [\<TypeForwardedTo(...)>] | Indicates a type redirection. <br>This attribute may be used only in imported non-F# assemblies. It is not permitted in F# code.
| System.Runtime.CompilerServices.ExtensionAttribute <br> [\<Extension(...)>] | Indicates the compiled form of a C# extension member. <br>This attribute may be used only in imported non-F# assemblies. It is not permitted in F# code.
| System.Runtime.InteropServices.DllImportAttribute <br> [\<DllImport(...)>] | When applied to a function definition in a module, causes the F# compiler to ignore the implementation of the definition, and instead compile it as a CLI P/Invoke stub declaration. <br>This attribute may be used in both F# and imported assemblies.
| System.Runtime.InteropServices.MarshalAsAttribute <br> [\<MarshalAs(...)>] | When applied to a parameter or return type, specifies the marshalling attribute for a CLI P/Invoke stub declaration. <br>This attribute may be used in both F# and imported assemblies. However, F# does not support the specification of "custom" marshallers.
| System.Runtime.InteropServices.InAttribute <br> [\<In>] | When applied to a parameter, specifies the CLI In attribute. <br>This attribute may be used in both F# and imported assemblies. However, in F# its only effect is to change the corresponding attribute in the CLI compiled form.
| System.Runtime.InteropServices.OutAttribute <br> [\<Out>] | When applied to a parameter, specifies the CLI Out attribute. <br>This attribute may be used in both F# and imported assemblies. However, in F# its only effect is to change the corresponding attribute in the CLI compiled form.
| System.Runtime.InteropServices.OptionalAttribute <br> [\<Optional(...)>] | When applied to a parameter, specifies the CLI Optional attribute. <br>This attribute may be used in both F# and imported assemblies. However, in F# its only effect is to change the corresponding attribute in the CLI compiled form.
| System.Runtime.InteropServices.FieldOffsetAttribute <br> [\<FieldOffset(...)>] | When applied to a field, specifies the field offset of the underlying CLI field. <br>This attribute may be used in both F# and imported assemblies.
| System.NonSerializedAttribute <br> [\<NonSerialized>] | When applied to a field, sets the "not serialized" bit for the underlying CLI field. <br>This attribute may be used in both F# and imported assemblies.
| System.Runtime.InteropServices.StructLayoutAttribute <br> [\<StructLayout(...)>] | Specifies the layout of a CLI type. <br>This attribute may be used in both F# and imported assemblies.
| FSharp.Core.AutoSerializableAttribute <br> [\<AutoSerializable(false)>] | When added to a type with value false , disables default serialization, so that F# does not make the type serializable. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.CLIMutableAttribute <br> [\<CLIMutable>] | When specified, a record type is compiled to a CLI representation with a default constructor with property getters and setters. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.AutoOpenAttribute <br> [\<AutoOpen>] | When applied to an assembly and given a string argument, causes the namespace or module to be opened automatically when the assembly is referenced. When applied to a module without a string argument, causes the module to be opened automatically when the enclosing namespace or module is opened. <br>This attribute should be used only in F# assemblies. FSharp.Core.
| CompilationRepresentationAttribute <br> [\<CompilationRepresentation(...)>] | Adjusts the runtime representation of a type. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.CompiledNameAttribute <br> [\<CompiledName(...)>] | Changes the compiled name of an F# language construct. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.CustomComparisonAttribute <br> [\<CustomComparison>] | When applied to an F# structural type, indicates that the type has a user-specified comparison implementation. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.CustomEqualityAttribute <br> [\<CustomEquality>] | When applied to an F# structural type, indicates that the type has a user-defined equality implementation. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.DefaultAugmentationAttribute <br> [\<DefaultAugmentation(...)>] | When applied to an F# discriminated union type with value false, turns off the generation of standard helper member tester, constructor and accessor members for the generated CLI class for that type. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.DefaultValueAttribute <br> [\<DefaultValue(...)>] | When added to a field declaration, specifies that the field is not initialized. During type checking, a constraint is asserted that the field type supports null. If the argument to the attribute is `false`, the constraint is not asserted. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.GeneralizableValueAttribute <br> [\<GeneralizableValue>] | When applied to an F# value, indicates that uses of the attribute can result in generic code through the process of type inference. For example, `Set.empty`. The value must typically be a type function whose implementation has no observable side effects. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.LiteralAttribute <br> [\<Literal>] | When applied to a value, compiles the value as a CLI literal. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.NoDynamicInvocationAttribute <br> [\<NoDynamicInvocation>] | When applied to an inline function or member definition, replaces the generated code with a stub that throws an exception at runtime. This attribute is used to replace the default generated implementation of unverifiable inline members with a verifiable stub. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.CompilerMessageAttribute <br> [\<CompilerMessage(...)>] | When applied to an F# construct, indicates that the F# compiler should report a message when the construct is used. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.StructAttribute <br> [\<Struct>] | Indicates that a type is a struct type. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.ClassAttribute <br> [\<Class>] | Indicates that a type is a class type. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.InterfaceAttribute <br> [\<Interface>] | Indicates that a type is an interface type. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.MeasureAttribute <br> [\<Measure>] | Indicates that a type or generic parameter is a unit of measure definition or annotation. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.ReferenceEqualityAttribute <br> [\<ReferenceEquality>] | When applied to an F# record or union type, indicates that the type should use reference equality for its default equality implementation. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.ReflectedDefinitionAttribute <br> [\<ReflectedDefinition>] | Makes the quotation form of a definition available at runtime through the FSharp.Quotations. Expr.GetReflectedDefinition method. <br>This attribute should be used only in F# assemblies. FSharp.Core.
| RequireQualifiedAccessAttribute <br> [\<RequireQualifiedAccess>] | When applied to an F# module, warns if an attempt is made to open the module name. When applied to an F# union or record type, indicates that the field labels or union cases must be referenced by using a qualified path that includes the type name. <br>This attribute should be used only in F# assemblies.
| RequiresExplicitTypeArgumentsAttribute <br> [\<RequiresExplicitTypeArguments>] | When applied to an F# function or method, indicates that the function or method must be invoked with explicit type arguments, such as typeof<int>. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.StructuralComparisonAttribute <br> [\<StructuralComparison>] | When added to a record, union, exception, or structure type, confirms the automatic generation of implementations for `IComparable` for the type. <br>This attribute should only be used in F# assemblies.
| FSharp.Core.StructuralEqualityAttribute <br> [\<StructuralEquality>] | When added to a record, union, or struct type, confirms the automatic generation of overrides for `Equals` and `GetHashCode` for the type. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.VolatileFieldAttribute <br> [\<VolatileField>] | When applied to an F# field or mutable value definition, controls whether the CLI volatile prefix is emitted before accesses to the field. <br>This attribute should be used only in F# assemblies.
| FSharp.Core.TypeProviderXmlDocAttribute | Specifies documentation for provided type definitions and provided members |
| FSharp.Core.TypeProviderDefinitionLocationAttribute | Specifies location information for provided type definitions and provided members |

## 17.2 Custom Attributes Emitted by F#

The F# compiler can emit the following custom attributes:

| Attribute | Description |
| --- | --- |
| System.Diagnostics.DebuggableAttribute | Improves debuggability of F# code. |
| System.Diagnostics.DebuggerHiddenAttribute | Improves debuggability of F# code. |
| System.Diagnostics.DebuggerDisplayAttribute | Improves debuggability of F# code. |
| System.Diagnostics.DebuggerBrowsableAttribute | Improves debuggability of F# code. |
| System.Runtime.CompilerServices.CompilationRelaxationsAttribute | Enables extra JIT optimizations. |
| System.Runtime.CompilerServices.CompilerGeneratedAttribute | Indicates that a method, type, or property is generated by the F# compiler, and does not correspond directly  |to | user source code. |
| System.Reflection.DefaultMemberAttribute | Specifies the name of the indexer property for a class. |
| FSharp.Core.CompilationMappingAttribute | Indicates how a CLI construct corresponds to an F# source language construct. |
| FSharp.Core.FSharpInterfaceDataVersionAttribute | Defines the schema number for the embedded binary resource for F#-specific interface and optimization data. |
| FSharp.Core.OptionalArgumentAttribute | Indicates optional arguments to F# members. |

## 17.3 Custom Attributes not Recognized by F#

The following custom attributes are defined in some CLI implementations and may appear to be
relevant to F#. However, they either do not affect the behavior of the F# compiler, or result in an
error when used in in F# code.

| Attribute | Description |
| --- | --- |
| System.Runtime.CompilerServices.DecimalConstantAttribute | The F# compiler ignores this attribute.<br>However, if used in F# code, it can cause some other CLI languages to interpret a decimal constant as a compile-time literal. |
| System.Runtime.CompilerServices.RequiredAttributeAttribute | Do not use this attribute in F# code.<br> The F# compiler ignores it or returns an error. |
| System.Runtime.InteropServices.DefaultParameterValueAttribute | Do not use this attribute in F# code.<br>The F# compiler ignores it or returns an error. |
| System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute | Do not use this attribute in F# code.<br>The F# compiler ignores it or returns an error. |
| System.Runtime.CompilerServices.FixedBufferAttribute | Do not use this attribute in F# code.<br>The F# compiler ignores it or returns an error. |
| System.Runtime.CompilerServices.UnsafeValueTypeAttribute | Do not use this attribute in F# code.<br>The F# compiler ignores it or returns an error. |
| System.Runtime.CompilerServices.SpecialNameAttribute | Do not use this attribute in F# code.<br>The F# compiler ignores it or returns an error.|

## 17.4 Exceptions Thrown by F# Language Primitives

Certain F# language and primitive library operations throw the following exceptions.

| Attribute | Description |
| --- | --- |
| System.ArithmeticException | An arithmetic operation failed. This is the base class for exceptions such as System.DivideByZeroException and System.OverflowException. |
| System.ArrayTypeMismatchException | An attempt to store an element in an array failed because the runtime type of the stored element is incompatible with the runtime type of the array. |
| System.DivideByZeroException | An attempt to divide an integral value by zero occurred. |
| System.IndexOutOfRangeException | An attempt to index an array failed because the index is less than zero or outside the bounds of the array. |
| System.InvalidCastException | An explicit conversion from a base type or interface to a derived type failed at run time. |
| System.NullReferenceException | A null reference was used in a way that caused the referenced object to be required. |
| System.OutOfMemoryException | An attempt to use new to allocate memory failed. |
| System.OverflowException | An arithmetic operation in a checked context overflowed. |
| System.StackOverflowException | The execution stack was exhausted because of too many pending method calls, which typically indicates deep or unbounded recursion. |
| System.TypeInitializationException | F# initialization code for a type threw an exception that was not caught. |
