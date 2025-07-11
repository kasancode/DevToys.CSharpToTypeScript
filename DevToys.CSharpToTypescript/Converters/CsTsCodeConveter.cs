﻿using Microsoft.CodeAnalysis.CSharp;

namespace DevToys.CSharpToTypescript.Converters;

public class CsTsCodeConveter(string cs, DateType dateType, bool toCamelCase, bool publicOnly, bool addExport)
{
    public string Convert()
    {
        var tree = CSharpSyntaxTree.ParseText(cs);
        var root = tree.GetRoot();
        var walker = new SytaxWalker(dateType, toCamelCase, publicOnly, addExport);

        walker.Visit(root);

        return walker.ToString().TrimEnd();
    }
}
