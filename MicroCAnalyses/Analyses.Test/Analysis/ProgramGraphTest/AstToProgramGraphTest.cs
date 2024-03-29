using System.Linq;
using Analyses.Analysis.Actions;
using Xunit;

namespace Analyses.Test.Analysis.ProgramGraphTest
{
    public class AstToProgramGraphTest
    {
        public AstToProgramGraphTest() {}

        [Fact]
        public void TestSimpleGraph()
        {
            Graph.ProgramGraph graph = new Graph.ProgramGraph(
                Parser.parse(
                    "int x; x := 2;"
                )
            );

            Assert.True(graph.Nodes.Count == 3, "The number of nodes in the graph should be 3, got " + graph.Nodes.Count);
            Assert.True(graph.Edges.Any(edge => edge.Action is IntDeclaration), "Expected an edge with a variable declaration, but none was found.");
        }

        [Fact]
        public void TestGenerateFibonnachiGraph()
        {
            Graph.ProgramGraph graph = new Graph.ProgramGraph(
                Parser.parse(
                    "int f2; int input; int current; f1 := 1; f2 := 1; read input; if (input == 0 | input == 1 ){ current := 1; } while (input > 1) { current := f1 + f2; f2 := f1; f1 := current; input := input - 1; } write current;"
                )
            );

            Assert.True(graph.Nodes.Count == 15, "The number of nodes in the graph should be 15, got " + graph.Nodes.Count);
            for (int i = 1; i <= 13; i++)
            {
                Assert.True(
                    graph.Nodes.Any(node => node.Name == Graph.ProgramGraph.NodePrefix + i),
                    $"Expected Node '{Graph.ProgramGraph.NodePrefix + i}' was not found."
                );
            }
            Assert.True(graph.Nodes.Any(node => node.Name == Graph.ProgramGraph.EndNode), $"Expected Node '{Graph.ProgramGraph.EndNode}' was not found.");
            Assert.True(graph.Edges.Count == 16, "The number of edges in the graph should be 16, got " + graph.Edges.Count);
            Assert.True(graph.Edges.Any(edge => edge.Action is Condition), "Expected an edge with a condition, but none was found.");
        }

        [Fact]
        public void TestGenerateSumIntegerGraph()
        {
            Graph.ProgramGraph graph = new Graph.ProgramGraph(
                Parser.parse(
                    "int[6] n; int x; int r; n[0] := 2; n[1] := 7; n[2] := 1; n[3] := 9; n[4] := 2; n[5] := 5; x := 0; r := 0; while (x < 6) { r := r + n[x]; x := x + 1; }"
                )
            );

            Assert.True(graph.Nodes.Count == 15, "The number of nodes in the graph should be 15, got " + graph.Nodes.Count);
            for (int i = 1; i <= 13; i++)
            {
                Assert.True(
                    graph.Nodes.Any(node => node.Name == Graph.ProgramGraph.NodePrefix + i),
                    $"Expected Node '{Graph.ProgramGraph.NodePrefix + i}' was not found."
                );
            }
            Assert.True(graph.Nodes.Any(node => node.Name == Graph.ProgramGraph.EndNode), $"Expected Node '{Graph.ProgramGraph.EndNode}' was not found.");
            Assert.True(graph.Edges.Count == 15, "The number of edges in the graph should be 15, got " + graph.Edges.Count);
            Assert.True(graph.Edges.Any(edge => edge.Action is ArrayAssignment), "Expected an edge with an array assignment, but none was found.");
        }

        [Fact]
        public void TestRecordSorting()
        {
            Graph.ProgramGraph graph = new Graph.ProgramGraph(
                Parser.parse(
                    "{ int fst; int snd } r; int isUnchanged; r := (3, 1); if (r.fst > r.snd) { tmp := r.fst; r.fst := r.snd; r.snd := tmp; isUnchanged := 0; } else { isUnchanged := 1; } write isUnchanged;"
                )
            );

            Assert.True(graph.Edges.Any(edge => edge.Action is RecordAssignment assignment && assignment.ToSyntax() == "r := (3, 1);"), "Expected an edge with a record assignment r of (3, 1), but none was found.");
            Assert.True(graph.Edges.Count(edge => edge.Action is Condition) == 2, $"Expected two edges with a condition, but had {graph.Edges.Count(edge => edge.Action is Condition)}.");
            Assert.True(graph.Nodes.Any(node => node.InGoingEdges.Count == 2), "Expected a node with two ingoing edges, which is true for the after-node of an if-else statement.");
        }
    }
}