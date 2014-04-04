using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TransformationCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = CreateTestCompilation();
            foreach (var sourceTree in test.SyntaxTrees)
            {
                var model = test.GetSemanticModel(sourceTree);
                var rewriter = new TypeInferenceRewriter(model);
                var newSource = rewriter.Visit(sourceTree.GetRoot());
                if (newSource != sourceTree.GetRoot())
                {
                    File.WriteAllText(sourceTree.FilePath, newSource.ToFullString());
                }
            }
        }

        private static Compilation CreateTestCompilation()
        {
            var programTree = CSharpSyntaxTree.ParseFile(@"..\..\Program.cs");
            var rewriterTree = CSharpSyntaxTree.ParseFile(@"..\..\TypeInferenceRewriter.cs");
            SyntaxTree[] sourceTrees = { programTree, rewriterTree };

            MetadataReference mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            MetadataReference codeAnalysis = new MetadataFileReference(typeof(SyntaxTree).Assembly.Location);
            MetadataReference csharpCodeAnalysis = new MetadataFileReference(typeof(CSharpSyntaxTree).Assembly.Location);
            MetadataReference[] references = { mscorlib, codeAnalysis, csharpCodeAnalysis }; return CSharpCompilation.Create("TransformationCS", sourceTrees, references,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }
    }
}
