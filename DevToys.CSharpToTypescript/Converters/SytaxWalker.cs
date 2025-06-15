using CaseConverter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace DevToys.CSharpToTypescript.Converters;

public class SytaxWalker(DateType dateAsString, bool toCamelCase, bool publicOnly, bool addExport) : CSharpSyntaxWalker
{
    internal readonly StringBuilder _builder = new();
    internal readonly string[] arrayTypes = [
        "List",
        "IList",
        "IEnumerable",
        "ReadOnlyList",
        "IReadOnlyList",
        "ICollection",
        "Array",
        "IReadOnlyCollection",

    ];
    internal readonly string[] dictionaryTypes = [
        "Dictionary",
        "IDictionary",
        "IReadOnlyDictionary",
        "ReadOnlyDictionary",
    ];

    internal readonly string[] numberTypes = [
        "sbyte",
        "byte",
        "short",
        "ushort",
        "int",
        "uint",
        "long",
        "ulong",
        "nint",
        "nuint",
        "SByte",
        "Byte",
        "Int16",
        "UInt16",
        "Int32",
        "UInt32",
        "Int64",
        "UInt64",
        "IntPtr",
        "UIntPtr",
        "float",
        "double",
        "decimal",
        "Single",
        "Double",
        "Decimal"
    ];

    internal readonly string[] stringTypes = [
        "string",
        "String",
        "char",
        "Char"
    ];

    internal readonly string[] dateTypes = [
        "DateTime",
        "DateTimeOffset",
        "DateOnly",
        "TimeOnly"
    ];

    internal string GenericArgumentsTpTsType(GenericNameSyntax genericNameSyntax)
        => string.Join(", ", genericNameSyntax.TypeArgumentList.Arguments.Select(a => this.ToTsType(a)));

    internal string ToTsType(string typeName)
    {
        if (this.numberTypes.Contains(typeName))
        {
            return "number";
        }

        if (this.stringTypes.Contains(typeName))
        {
            return "string";
        }

        if (this.dateTypes.Contains(typeName))
        {
            return dateAsString switch
            {
                DateType.Date => "Date",
                DateType.String => "string",
                DateType.Union => "Date | string",
                _ => "Date"
            };
        }

        return typeName switch
        {
            "Boolean" => "boolean",
            "Array" => "any[]",
            "Object" => "object",
            _ => typeName
        };
    }

    internal string ToTsType(TypeSyntax? syntax)
    {
        var type = syntax switch
        {
            PredefinedTypeSyntax predefinedType => this.ToTsType(predefinedType.Keyword.Text),
            IdentifierNameSyntax identifierType => this.ToTsType(identifierType.Identifier.Text),
            NullableTypeSyntax nullableTypeSyntax => this.ToTsType(nullableTypeSyntax.ElementType) + " | null",
            TupleTypeSyntax tupleTypeSyntax => $"[{string.Join(", ", tupleTypeSyntax.Elements.Select(e => this.ToTsType(e.Type)))}]",
            ArrayTypeSyntax arrayTypeSyntax => $"{this.ToTsType(arrayTypeSyntax.ElementType)}[]",
            GenericNameSyntax genericNameSyntax => genericNameSyntax switch
            {
                { TypeArgumentList.Arguments: [var arg1] } when this.arrayTypes.Contains(genericNameSyntax.Identifier.Text) => $"{this.ToTsType(arg1)}[]",
                { TypeArgumentList.Arguments: [var arg1, var arg2] } when this.dictionaryTypes.Contains(genericNameSyntax.Identifier.Text) => $"{{[key: {this.ToTsType(arg1)}]: {this.ToTsType(arg2)}}}",
                _ => $"{genericNameSyntax.Identifier.Text}<{this.GenericArgumentsTpTsType(genericNameSyntax)}>"
            },
            null => "any",
            _ => syntax.ToString()
        };

        return type;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        this.VisitCore(
            node,
            base.VisitClassDeclaration,
            n => n.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => !publicOnly || p.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))),
            p => p.Identifier.Text,
            p => p.Type);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        this.VisitCore(
            node,
            base.VisitRecordDeclaration,
            n => n.ParameterList?.Parameters ?? [],
            p => p.Identifier.Text,
            p => p.Type);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        this.VisitCore(
            node,
            base.VisitInterfaceDeclaration,
            n => n.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => !publicOnly || p.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))),
            p => p.Identifier.Text,
            p => p.Type);
    }

    internal string AddConstrains(string typeName, List<TypeParameterConstraintClauseSyntax>? constraintList)
    {
        if (constraintList is null || constraintList.Count == 0)
        {
            return typeName;
        }

        var constraint = constraintList.FirstOrDefault(c => c.Name.Identifier.Text == typeName);
        if (constraint is null)
        {
            return typeName;
        }

        return $"{typeName} extends {string.Join(" & ", constraint.Constraints.Select(c => c))}";
    }

    public void VisitCore<TNodeSyntax, TMemberSyntax>(
        TNodeSyntax node,
        Action<TNodeSyntax> callBase,
        Func<TNodeSyntax, IEnumerable<TMemberSyntax>> enumMember,
        Func<TMemberSyntax, string> getText,
        Func<TMemberSyntax, TypeSyntax?> getTypeSyntax
        )
        where TNodeSyntax : TypeDeclarationSyntax
        where TMemberSyntax : CSharpSyntaxNode
    {
        var builder = new StringBuilder();

        if (publicOnly && !node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
        {
            callBase(node);
            return;
        }

        var typeParams = "";
        if (node.TypeParameterList?.Parameters.Any() == true)
        {
            var constraints = node.ConstraintClauses.ToList();

            typeParams = $"<{string.Join(", ",
                node.TypeParameterList.Parameters.Select(p => this.AddConstrains(p.Identifier.Text, constraints)))}>";
        }

        var baseTypes = "";
        if (node.BaseList is not null && node.BaseList.Types.Any())
        {
            baseTypes = $" extends {string.Join(", ", node.BaseList.Types.Select(t => this.ToTsType(t.Type)))}";
        }

        builder.AppendLine($"{(addExport ? "export " : "")}interface {node.Identifier.Text}{typeParams}{baseTypes} {{");

        foreach (var item in enumMember(node))
        {
            var propertyName = toCamelCase ? getText(item).ToCamelCase() : getText(item);

            var typeSyntax = getTypeSyntax(item);
            var type = typeSyntax is null ? "any" : this.ToTsType(typeSyntax);
            builder.AppendLine($"    {propertyName}: {type};");
        }

        builder.AppendLine("}");

        this._builder.AppendLine(builder.ToString());

        callBase(node);
    }

    public override string ToString() => this._builder.ToString();
}
