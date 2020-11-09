﻿using Analyses.Algorithms.Stack;
using Analyses.Analysis.BitVector.ReachingDefinitionsAnalysis;
using Analyses.Analysis.Monotone.DetectionOfSignsAnalysis;
using Analyses.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Analyses.Test.Analysis.Algorithms
{
	public class WorklistLIFOAlgorithmTests
	{

        private ReachingDefinitions _analysisRD;
        private DetectionOfSigns _analysisDoS;
        private readonly ProgramGraph _graphFibonacci;
        private readonly ProgramGraph _graphAddIntegers;

        public WorklistLIFOAlgorithmTests()
        {
            _graphFibonacci = new ProgramGraph(
                Parser.parse(
                    "int f2; int input; int current; f1 := 1; f2 := 1; read input; if (input == 0 | input == 1 ){ current := 1; } " +
                    "while (input > 1) { current := f1 + f2; f2 := f1; f1 := current; input := input - 1; } write current;"
                )
            );
            _graphAddIntegers = new ProgramGraph(
                Parser.parse(
                    "int a; int b; int c; a := 3; read b; c := a + b; write c;"
                )
            );
        }
        
        [Fact]
        public void TestSameInsertCountFibonacci()
        {

            _analysisRD = new ReachingDefinitions(_graphFibonacci, Analyses.Algorithms.WorklistImplementation.Lifo);
            _analysisRD.InitializeConstraints();
            _analysisRD.Analyse();

            var algo = _analysisRD._worklistAlgorithm as WorklistLIFOAlgorithm;
            var counter1 = algo.Counter;

            _analysisRD = new ReachingDefinitions(_graphFibonacci, Analyses.Algorithms.WorklistImplementation.Lifo);
            _analysisRD.InitializeConstraints();
            _analysisRD.Analyse();

            algo = _analysisRD._worklistAlgorithm as WorklistLIFOAlgorithm;
            var counter2 = algo.Counter;

            Assert.True(counter1 > 0, $"Expected Insert() count to be positive for counter1, as program is not empty.");
            Assert.True(counter2 > 0, $"Expected Insert() count to be positive for counter2, as program is not empty.");
            Assert.True(counter1 == counter2, $"Expected Insert() count to be identical for both executions of the program. Got ({counter1}, {counter2})");

            Assert.True(counter1 == 70, $"Expected Insert() count for counter1 to be 70. This is based on observing a previous run. Got {counter1}");
        }
        
        [Fact]
        public void TestSameInsertCountSumIntArray()
        {

            _analysisRD = new ReachingDefinitions(_graphAddIntegers, Analyses.Algorithms.WorklistImplementation.Lifo);
            _analysisRD.InitializeConstraints();
            _analysisRD.Analyse();

            var algo = _analysisRD._worklistAlgorithm as WorklistLIFOAlgorithm;
            var counter1 = algo.Counter;

            _analysisRD = new ReachingDefinitions(_graphAddIntegers, Analyses.Algorithms.WorklistImplementation.Lifo);
            _analysisRD.InitializeConstraints();
            _analysisRD.Analyse();

            algo = _analysisRD._worklistAlgorithm as WorklistLIFOAlgorithm;
            var counter2 = algo.Counter;

            Assert.True(counter1 > 0, $"Expected Insert() count to be positive for counter1, as program is not empty.");
            Assert.True(counter2 > 0, $"Expected Insert() count to be positive for counter2, as program is not empty.");
            Assert.True(counter1 == counter2, $"Expected Insert() count to be identical for both executions of the program. Got ({counter1}, {counter2})");

            Assert.True(counter1 == 22, $"Expected Insert() count for counter1 to be 22. This is based on observing a previous run. Got {counter1}");
        }


        [Fact]
        public void TestAnalysisCorrectness()
        {

            _analysisDoS = new DetectionOfSigns(_graphFibonacci, Analyses.Algorithms.WorklistImplementation.Lifo);
            _analysisDoS.InitializeConstraints();
            _analysisDoS.Analyse();

            var endNode = _graphFibonacci.Nodes.First(node => node.Name == ProgramGraph.EndNode);
            var constraint = _analysisDoS.FinalConstraintsForNodes[endNode] as DetectionOfSignsConstraint;

            // Simple variable count check
            Assert.True(constraint.VariableSigns.Count == 4, $"Expected 4 variables in resulting constraint, only given {constraint.VariableSigns.Count}.");

            // Check existence of variables in output
            Assert.True(constraint.VariableSigns.ContainsKey("f1"), "Variable 'f1' was not found in the resulting constraint.");
            Assert.True(constraint.VariableSigns.ContainsKey("f2"), "Variable 'f2' was not found in the resulting constraint.");
            Assert.True(constraint.VariableSigns.ContainsKey("input"), "Variable 'input' was not found in the resulting constraint.");
            Assert.True(constraint.VariableSigns.ContainsKey("current"), "Variable 'current' was not found in the resulting constraint.");

            // Check expected signs for each variable
            Assert.True(constraint.VariableSigns["f1"].signs.SetEquals(new HashSet<Sign>() { Sign.Positive }),
                $"Expected 'f1' to be {{ Positive }}, but got {{ {string.Join(", ", constraint.VariableSigns["f1"].signs)} }}.");
            Assert.True(constraint.VariableSigns["f2"].signs.SetEquals(new HashSet<Sign>() { Sign.Positive }),
                $"Expected 'f2' to be {{ Positive }}, but got {{ {string.Join(", ", constraint.VariableSigns["f2"].signs)} }}.");
            Assert.True(constraint.VariableSigns["input"].signs.SetEquals(new HashSet<Sign>() { Sign.Negative, Sign.Zero, Sign.Positive }),
                $"Expected 'input' to be {{ Negative, Zero, Positive }}, but got {{ {string.Join(", ", constraint.VariableSigns["input"].signs)} }}.");
            Assert.True(constraint.VariableSigns["current"].signs.SetEquals(new HashSet<Sign>() { Sign.Zero, Sign.Positive }),
                $"Expected 'current' to be {{ Zero, Positive }}, but got {{ {string.Join(", ", constraint.VariableSigns["current"].signs)} }}.");

            var algo = _analysisDoS._worklistAlgorithm as WorklistLIFOAlgorithm;
            var counter = algo.Counter;

            Assert.True(counter > 0, $"Expected Insert() count to be positive for counter, as program is not empty.");

            Assert.True(counter == 70, $"Expected Insert() count for counter to be 22. This is based on observing a previous run. Got {counter}");
        }


    }
}