﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Microsoft.Quantum.QsCompiler.DataTypes;



namespace Microsoft.Quantum.QsCompiler.BondSchemas
{
    public static class CompilerObjectTranslator
    {
        public static SyntaxTree.QsCompilation CreateQsCompilation(QsCompilation bondCompilation) =>
            new SyntaxTree.QsCompilation(
                namespaces: bondCompilation.Namespaces.Select(n => n.ToCompilerObject()).ToImmutableArray(),
                // TODO: Implement EntryPoints.
                entryPoints: Array.Empty<SyntaxTree.QsQualifiedName>().ToImmutableArray());

        private static BigInteger ToBigInteger(this ArraySegment<byte> blob) =>
            new BigInteger(blob);

        private static DataTypes.Position ToCompilerObject(this Position position) =>
            DataTypes.Position.Create(
                line: position.Line,
                column: position.Column);

        private static DataTypes.Range ToCompilerObject(this Range range) =>
            DataTypes.Range.Create(
                start: range.Start.ToCompilerObject(),
                end: range.End.ToCompilerObject());

        private static SyntaxTree.CallableInformation ToCompilerObject(CallableInformation bondCallableInformation) =>
            new SyntaxTree.CallableInformation(
                // TODO: Implement Characteristics.
                characteristics: default,
                // TODO: Implement InferredInformation.
                inferredInformation: default);

        private static SyntaxTree.Identifier ToCompilerObject(Identifier bondIdentifier)
        {
            string UnexpectedNullFieldMessage(string fieldName) =>
                $"Bond Identifier '{fieldName}' field is null when Kind is '{bondIdentifier.Kind}'";

            if (bondIdentifier.Kind == IdentifierKind.LocalVariable)
            {
                var localVariable =
                    bondIdentifier.LocalVariable ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("LocalVariable"));

                return SyntaxTree.Identifier.NewLocalVariable(item: localVariable.ToNonNullable());
            }
            else if (bondIdentifier.Kind == IdentifierKind.GlobalCallable)
            {
                var globalCallable =
                    bondIdentifier.GlobalCallable ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("GlobalCallable"));

                return SyntaxTree.Identifier.NewGlobalCallable(item: globalCallable.ToCompilerObject());
            }
            else
            {
                throw new ArgumentException($"Unsupported Bond IdentifierKind '{bondIdentifier.Kind}'");
            }
        }

        private static SyntaxTree.QsCallable ToCompilerObject(this QsCallable bondQsCallable) =>
            new SyntaxTree.QsCallable(
                kind: bondQsCallable.Kind.ToCompilerObject(),
                fullName: bondQsCallable.FullName.ToCompilerObject(),
                attributes: bondQsCallable.Attributes.Select(a => a.ToCompilerObject()).ToImmutableArray(),
                modifiers: bondQsCallable.Modifiers.ToCompilerObject(),
                sourceFile: bondQsCallable.SourceFile.ToNonNullable(),
                location: bondQsCallable.Location != null ?
                    bondQsCallable.Location.ToCompilerObject().ToQsNullableGeneric() :
                    QsNullable<SyntaxTree.QsLocation>.Null,
                signature: bondQsCallable.Signature.ToCompilerObject(),
                // TODO: Implement ArgumentTuple.
                argumentTuple: default,
                specializations: bondQsCallable.Specializations.Select(s => s.ToCompilerObject()).ToImmutableArray(),
                documentation: bondQsCallable.Documentation.ToImmutableArray(),
                comments: bondQsCallable.Comments.ToCompilerObject());

        private static SyntaxTree.QsCallableKind ToCompilerObject(this QsCallableKind bondQsCallableKind) =>
            bondQsCallableKind switch
            {
                QsCallableKind.Operation => SyntaxTree.QsCallableKind.Operation,
                QsCallableKind.Function => SyntaxTree.QsCallableKind.Function,
                QsCallableKind.TypeConstructor => SyntaxTree.QsCallableKind.TypeConstructor,
                _ => throw new ArgumentException($"Unsupported Bond QsCallableKind '{bondQsCallableKind}'")
            };

        private static SyntaxTree.QsComments ToCompilerObject(this QsComments bondQsComments) =>
            new SyntaxTree.QsComments(
                openingComments: bondQsComments.OpeningComments.ToImmutableArray(),
                closingComments: bondQsComments.ClosingComments.ToImmutableArray());

        private static SyntaxTree.QsCustomType ToCompilerObject(this QsCustomType bondQsCustomType) =>
            new SyntaxTree.QsCustomType(
                fullName: bondQsCustomType.FullName.ToCompilerObject(),
                // TODO: Implement Attributes.
                attributes: Array.Empty<SyntaxTree.QsDeclarationAttribute>().ToImmutableArray(),
                modifiers: bondQsCustomType.Modifiers.ToCompilerObject(),
                sourceFile: bondQsCustomType.SourceFile.ToNonNullable(),
                location: bondQsCustomType.Location != null ?
                    bondQsCustomType.Location.ToCompilerObject().ToQsNullableGeneric() :
                    QsNullable<SyntaxTree.QsLocation>.Null,
                // TODO: Implement Type.
                type: default,
                // TODO: Implement TypeItems.
                typeItems: default,
                documentation: bondQsCustomType.Documentation.ToImmutableArray(),
                comments: bondQsCustomType.Comments.ToCompilerObject());

        private static SyntaxTree.QsDeclarationAttribute ToCompilerObject(this QsDeclarationAttribute bondQsDeclarationAttribute) =>
            new SyntaxTree.QsDeclarationAttribute(
                typeId: bondQsDeclarationAttribute.TypeId != null ?
                    bondQsDeclarationAttribute.TypeId.ToCompilerObject().ToQsNullableGeneric() :
                    QsNullable<SyntaxTree.UserDefinedType>.Null,
                // TODO: Implement Argument.
                argument: default,
                offset: bondQsDeclarationAttribute.Offset.ToCompilerObject(),
                comments: bondQsDeclarationAttribute.Comments.ToCompilerObject());

        private static SyntaxTree.QsLocalSymbol ToCompilerObject(this QsLocalSymbol bondQsLocalSymbol)
        {
            if (bondQsLocalSymbol.Kind == QsLocalSymbolKind.ValidName)
            {
                var validName =
                    bondQsLocalSymbol.Name ??
                    throw new ArgumentNullException($"Bond QsLocalSymbol 'Name' field is null when Kind is '{bondQsLocalSymbol.Kind}'");

                return SyntaxTree.QsLocalSymbol.NewValidName(item: validName.ToNonNullable());
            }
            else
            {
                return bondQsLocalSymbol.Kind switch
                {
                    QsLocalSymbolKind.InvalidName => SyntaxTree.QsLocalSymbol.InvalidName,
                    _ => throw new ArgumentException($"Unsupported Bond QsLocalSymbolKind '{bondQsLocalSymbol.Kind}'")
                };
            }
        }

        private static SyntaxTree.QsLocation ToCompilerObject(this QsLocation bondQsLocation) =>
            new SyntaxTree.QsLocation(
                offset: bondQsLocation.Offset.ToCompilerObject(),
                range: bondQsLocation.Range.ToCompilerObject());

        private static SyntaxTree.QsNamespace ToCompilerObject(this QsNamespace bondQsNamespace) =>
            new SyntaxTree.QsNamespace(
                name: bondQsNamespace.Name.ToNonNullable(),
                elements: bondQsNamespace.Elements.Select(e => e.ToCompilerObject()).ToImmutableArray(),
                documentation: bondQsNamespace.Documentation.ToLookup(
                    p => p.FileName.ToNonNullable(),
                    p => p.DocumentationItems.ToImmutableArray()));

        private static SyntaxTree.QsNamespaceElement ToCompilerObject(this QsNamespaceElement bondQsNamespaceElement)
        {
            string UnexpectedNullFieldMessage(string fieldName) =>
                $"Bond QsNamespaceElement '{fieldName}' field is null when Kind is '{bondQsNamespaceElement.Kind}'";

            if (bondQsNamespaceElement.Kind == QsNamespaceElementKind.QsCallable)
            {
                var callable =
                    bondQsNamespaceElement.Callable ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("Callable"));

                return SyntaxTree.QsNamespaceElement.NewQsCallable(item: callable.ToCompilerObject());
            }
            else if (bondQsNamespaceElement.Kind == QsNamespaceElementKind.QsCustomType)
            {
                var customType =
                    bondQsNamespaceElement.CustomType ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("CustomType"));

                return SyntaxTree.QsNamespaceElement.NewQsCustomType(item: customType.ToCompilerObject());
            }
            else
            {
                throw new ArgumentException($"Unsupported Bond QsNamespaceElementKind '{bondQsNamespaceElement.Kind}'");
            }
        }

        private static SyntaxTree.QsQualifiedName ToCompilerObject(this QsQualifiedName bondQsQualifiedName) =>
            new SyntaxTree.QsQualifiedName(
                @namespace: bondQsQualifiedName.Namespace.ToNonNullable(),
                name: bondQsQualifiedName.Name.ToNonNullable());

        private static SyntaxTree.QsSpecialization ToCompilerObject(this QsSpecialization bondQsSpecialization) =>
            new SyntaxTree.QsSpecialization(
                kind: bondQsSpecialization.Kind.ToCompilerObject(),
                parent: bondQsSpecialization.Parent.ToCompilerObject(),
                attributes: bondQsSpecialization.Attributes.Select(a => a.ToCompilerObject()).ToImmutableArray(),
                sourceFile: bondQsSpecialization.SourceFile.ToNonNullable(),
                location: bondQsSpecialization.Location != null ?
                    bondQsSpecialization.Location.ToCompilerObject().ToQsNullableGeneric() :
                    QsNullable<SyntaxTree.QsLocation>.Null,
                typeArguments: bondQsSpecialization.TypeArguments != null ?
                    bondQsSpecialization.TypeArguments.Select(t => t.ToCompilerObject()).ToImmutableArray().ToQsNullableGeneric() :
                    QsNullable<ImmutableArray<SyntaxTree.ResolvedType>>.Null,
                // TODO: Implement Signature.
                signature: default,
                // TODO: Implement Implementation.
                implementation: default,
                // TODO: Implement Documentation.
                documentation: Array.Empty<string>().ToImmutableArray(),
                // TODO: Implement Comments.
                comments: default);

        private static SyntaxTree.QsSpecializationKind ToCompilerObject(this QsSpecializationKind bondQsSpecializationKind) =>
            bondQsSpecializationKind switch
            {
                QsSpecializationKind.QsAdjoint => SyntaxTree.QsSpecializationKind.QsAdjoint,
                QsSpecializationKind.QsBody => SyntaxTree.QsSpecializationKind.QsBody,
                QsSpecializationKind.QsControlled => SyntaxTree.QsSpecializationKind.QsControlled,
                QsSpecializationKind.QsControlledAdjoint => SyntaxTree.QsSpecializationKind.QsControlledAdjoint,
                _ => throw new ArgumentException($"Unsupported Bond QsSpecializationKind '{bondQsSpecializationKind}'")
            };

        private static SyntaxTree.QsTypeParameter ToCompilerObject(this QsTypeParameter bondQsTypeParameter) =>
            new SyntaxTree.QsTypeParameter(
                origin: bondQsTypeParameter.Origin.ToCompilerObject(),
                typeName: bondQsTypeParameter.TypeName.ToNonNullable(),
                range: bondQsTypeParameter.Range != null ?
                    bondQsTypeParameter.Range.ToCompilerObject().ToQsNullableGeneric() :
                    QsNullable<DataTypes.Range>.Null);

        private static SyntaxTree.ResolvedSignature ToCompilerObject(this ResolvedSignature bondResolvedSignature) =>
            new SyntaxTree.ResolvedSignature(
                typeParameters: bondResolvedSignature.TypeParameters.Select(tp => tp.ToCompilerObject()).ToImmutableArray(),
                // Implement ArgumentType
                argumentType: default,
                // Implement ReturnType
                returnType: default,
                // Implement Information
                information: default);

        private static SyntaxTree.ResolvedType ToCompilerObject(this ResolvedType bondResolvedType) =>
            SyntaxTree.ResolvedType.New(bondResolvedType.TypeKind.ToCompilerObject());

        private static SyntaxTree.UserDefinedType ToCompilerObject(this UserDefinedType bondUserDefinedType) =>
            new SyntaxTree.UserDefinedType(
                @namespace: bondUserDefinedType.Namespace.ToNonNullable(),
                name: bondUserDefinedType.Name.ToNonNullable(),
                range: bondUserDefinedType.Range != null ?
                    bondUserDefinedType.Range.ToCompilerObject().ToQsNullableGeneric() :
                    QsNullable<DataTypes.Range>.Null);

        private static SyntaxTokens.AccessModifier ToCompilerObject(this AccessModifier accessModifier) =>
            accessModifier switch
            {
                AccessModifier.DefaultAccess => SyntaxTokens.AccessModifier.DefaultAccess,
                AccessModifier.Internal => SyntaxTokens.AccessModifier.Internal,
                _ => throw new ArgumentException($"Unsupported Bond AccessModifier '{accessModifier}'")
            };

        private static SyntaxTokens.Modifiers ToCompilerObject(this Modifiers modifiers) =>
            new SyntaxTokens.Modifiers(
                access: modifiers.Access.ToCompilerObject());

        private static SyntaxTokens.QsTypeKind<SyntaxTree.ResolvedType, SyntaxTree.UserDefinedType, SyntaxTree.QsTypeParameter, SyntaxTree.CallableInformation> ToCompilerObject(
            this QsTypeKindComposition<ResolvedType, UserDefinedType, QsTypeParameter, CallableInformation> bondQsTypeKindComposition) =>
            bondQsTypeKindComposition.ToCompilerObjectGeneric(
                typeTranslator: ToCompilerObject,
                udtTranslator: ToCompilerObject,
                paramTranslator: ToCompilerObject,
                characteristicsTranslator: ToCompilerObject);

        private static SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType> ToCompilerObjectGeneric<
            TCompilerExpression,
            TCompilerSymbol,
            TCompilerType,
            TBondExpression,
            TBondSymbol,
            TBondType>(
                this QsExpressionKindComposition<TBondExpression, TBondSymbol, TBondType> bondQsExpressionKindComposition,
                Func<TBondExpression, TCompilerExpression> expressionTranslator,
                Func<TBondSymbol, TCompilerSymbol> symbolTranslator,
                Func<TBondType, TCompilerType> typeTranslator)
        {
            string UnexpectedNullFieldMessage(string fieldName) =>
                $"Bond QsExpressionKindComposition '{fieldName}' field is null when Kind is '{bondQsExpressionKindComposition.Kind}'";

            if (bondQsExpressionKindComposition.Kind == QsExpressionKind.Identifier)
            {
                var identifier =
                    bondQsExpressionKindComposition.Identifier ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("Identifier"));

                return SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                    NewIdentifier(
                        item1: symbolTranslator(identifier.Symbol),
                        item2: identifier.Types.Select(t => typeTranslator(t)).ToImmutableArray().ToQsNullableGeneric());
            }
            else if (bondQsExpressionKindComposition.Kind == QsExpressionKind.ValueTuple)
            {
                var valueTuple =
                    bondQsExpressionKindComposition.ExpressionArray ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("ExpressionArray"));

                return SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                    NewValueTuple(item: valueTuple.Select(e => expressionTranslator(e)).ToImmutableArray());
            }
            else if (bondQsExpressionKindComposition.Kind == QsExpressionKind.IntLiteral)
            {
                var intLiteral =
                    bondQsExpressionKindComposition.IntLiteral ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("IntLiteral"));

                return SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                    NewIntLiteral(item: intLiteral);
            }
            else if (bondQsExpressionKindComposition.Kind == QsExpressionKind.BigIntLiteral)
            {
                var bigIntLiteral =
                    bondQsExpressionKindComposition.BigIntLiteral != null ?
                        bondQsExpressionKindComposition.BigIntLiteral :
                        throw new ArgumentNullException(UnexpectedNullFieldMessage("BigIntLiteral"));

                return SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                    NewBigIntLiteral(item: bigIntLiteral.ToBigInteger());
            }
            else if (bondQsExpressionKindComposition.Kind == QsExpressionKind.DoubleLiteral)
            {
                var doubleLiteral =
                    bondQsExpressionKindComposition.DoubleLiteral ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("DoubleLiteral"));

                return SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                    NewDoubleLiteral(item: doubleLiteral);
            }
            else if (bondQsExpressionKindComposition.Kind == QsExpressionKind.BoolLiteral)
            {
                var boolLiteral =
                    bondQsExpressionKindComposition.BoolLiteral ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("BoolLiteral"));

                return SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                    NewBoolLiteral(item: boolLiteral);
            }
            // TODO: Implement the remaining cases.
            else
            {
                return bondQsExpressionKindComposition.Kind switch
                {
                    QsExpressionKind.UnitValue => SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                        CreateUnitValue<TCompilerExpression, TCompilerSymbol, TCompilerType>(),
                    QsExpressionKind.MissingExpr => SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                        CreateMissingExpr<TCompilerExpression, TCompilerSymbol, TCompilerType>(),
                    QsExpressionKind.InvalidExpr => SyntaxTokens.QsExpressionKind<TCompilerExpression, TCompilerSymbol, TCompilerType>.
                        CreateInvalidExpr<TCompilerExpression, TCompilerSymbol, TCompilerType>(),
                    _ => throw new ArgumentException($"Unsupported Bond QsExpressionKind '{bondQsExpressionKindComposition.Kind}'")
                };
            }
        }

        private static SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics> ToCompilerObjectGeneric<
            TCompilerType,
            TCompilerUdt,
            TCompilerParam,
            TCompilerCharacteristics,
            TBondType,
            TBondUdt,
            TBondParam,
            TBondCharacteristics>(
                this QsTypeKindComposition<TBondType, TBondUdt, TBondParam, TBondCharacteristics> bondQsTypeKindComposition,
                Func<TBondType, TCompilerType> typeTranslator,
                Func<TBondUdt, TCompilerUdt> udtTranslator,
                Func<TBondParam, TCompilerParam> paramTranslator,
                Func<TBondCharacteristics, TCompilerCharacteristics> characteristicsTranslator)
        {
            string UnexpectedNullFieldMessage(string fieldName) =>
                $"Bond QsTypeKindComposition '{fieldName}' field is null when Kind is '{bondQsTypeKindComposition.Kind}'";

            if (bondQsTypeKindComposition.Kind == QsTypeKind.ArrayType)
            {
                var arrayType =
                    bondQsTypeKindComposition.ArrayType ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("ArrayType"));

                return SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                    NewArrayType(item: typeTranslator(arrayType));
            }
            else if (bondQsTypeKindComposition.Kind == QsTypeKind.TupleType)
            {
                var tupleType =
                    bondQsTypeKindComposition.TupleType ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("TupleType"));

                return SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                    NewTupleType(item: tupleType.Select(t => typeTranslator(t)).ToImmutableArray());
            }
            else if (bondQsTypeKindComposition.Kind == QsTypeKind.UserDefinedType)
            {
                var userDefinedType =
                    bondQsTypeKindComposition.UserDefinedType ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("UserDefinedType"));

                return SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                    NewUserDefinedType(item: udtTranslator(userDefinedType));
            }
            else if (bondQsTypeKindComposition.Kind == QsTypeKind.TypeParameter)
            {
                var typeParameter =
                    bondQsTypeKindComposition.TypeParameter ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("TypeParameter"));

                return SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                    NewTypeParameter(item: paramTranslator(typeParameter));
            }
            else if (bondQsTypeKindComposition.Kind == QsTypeKind.Operation)
            {
                var operation =
                    bondQsTypeKindComposition.Operation ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("Operation"));

                return SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                    NewOperation(
                        item1: Tuple.Create(typeTranslator(operation.Type1), typeTranslator(operation.Type2)),
                        item2: characteristicsTranslator(operation.Characteristics));
            }
            else if (bondQsTypeKindComposition.Kind == QsTypeKind.Function)
            {
                var function =
                    bondQsTypeKindComposition.Function ??
                    throw new ArgumentNullException(UnexpectedNullFieldMessage("Function"));

                return SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                    NewFunction(
                        item1: typeTranslator(function.Type1),
                        item2: typeTranslator(function.Type2));
            }
            else
            {
                var simpleQsTypeKind = bondQsTypeKindComposition.Kind switch
                {
                    QsTypeKind.UnitType => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateUnitType<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Int => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateInt<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.BigInt => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateBigInt<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Double => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateDouble<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Bool => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateBool<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.String => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateString<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Qubit => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateQubit<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Result => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateResult<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Pauli => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreatePauli<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.Range => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateRange<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.MissingType => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateMissingType<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    QsTypeKind.InvalidType => SyntaxTokens.QsTypeKind<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>.
                        CreateInvalidType<TCompilerType, TCompilerUdt, TCompilerParam, TCompilerCharacteristics>(),
                    _ => throw new ArgumentException($"Unsupported Bond QsTypeKind {bondQsTypeKindComposition.Kind}")
                };

                return simpleQsTypeKind;
            }
        }

        private static NonNullable<string> ToNonNullable(this string str) =>
            NonNullable<string>.New(str);

        private static QsNullable<T> ToQsNullableGeneric<T>(this T obj) =>
                QsNullable<T>.NewValue(obj);
    }
}