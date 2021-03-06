﻿using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using StoryTeller.Model;
using StoryTeller.Results;

namespace StoryTeller.Grammars.Tables
{
    public class TableGrammar : IGrammar
    {
        private readonly IGrammar _inner;
        private Action<ISpecContext> _after;
        private Action<ISpecContext, Section> _before;
        private string _leafName = "rows";
        private string _key;

        public TableGrammar(IGrammar inner)
        {
            _inner = inner;
        }

        public string Title { get; private set; }

        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                _inner.Key = value + ":Row";
            }
        }

        public IExecutionStep CreatePlan(Step step, FixtureLibrary library, bool inTable = false)
        {
            return new CompositeExecution(toExecutionSteps(library, step));
        }

        public GrammarModel Compile(Fixture fixture, CellHandling cells)
        {
            if (_inner is MissingGrammar)
            {
                return _inner.Compile(fixture, cells);
            }

            var innerModel = _inner.Compile(fixture, cells).As<IModelWithCells>();

            return new Table
            {
                cells = innerModel.cells.ToArray(),
                collection = _leafName,
                title = Title,
                hasBeforeStep = _before != null,
                hasAfterStep = _after != null
            };
        }

        public TableGrammar Before(Action<ISpecContext> before)
        {
            return Before((c, s) => before(c));
        }
        
        public TableGrammar Before(Action<ISpecContext, Section> before)
        {
            _before = before;
            return this;
        }

        public TableGrammar Before(Action before)
        {
            return Before(c => before());
        }

        public TableGrammar After(Action<ISpecContext> after)
        {
            _after = after;
            return this;
        }

        public TableGrammar After(Action after)
        {
            return After(c => after());
        }

        public string LeafName()
        {
            return _leafName;
        }

        public TableGrammar LeafName(string leafName)
        {
            _leafName = leafName;
            return this;
        }

        public TableGrammar Titled(string title)
        {
            Title = title;
            return this;
        }

        private IEnumerable<IExecutionStep> toExecutionSteps(FixtureLibrary library, Step parentStep)
        {
            var section = parentStep.Collections[_leafName];
            if (section.id.IsEmpty()) section.id = Guid.NewGuid().ToString();
            
            if (_before != null) yield return new SilentAction("Grammar", Stage.before, c =>
            {
                c.State.Store(section);
                _before(c, section);
            }, section)
            {
                Subject = Key + "Before"
            };


            var rowNumber = 0;
            foreach (Step row in section.Children.OfType<Step>())
            {
                var line = _inner.CreatePlan(row, library, true);

                if (line is IWithValues l) l.Values.Order = ++rowNumber;
                
                yield return line;
            }

            if (_after != null) yield return new SilentAction("Grammar", Stage.after, _after, section)
            {
                Subject = Key + ":After"
            };
        }

        public bool IsHidden { get; set; }

        public long MaximumRuntimeInMilliseconds { get; set; }
    }
}