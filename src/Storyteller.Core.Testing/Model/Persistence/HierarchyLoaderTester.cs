﻿using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Storyteller.Core.Model.Persistence;
using Storyteller.Core.Remotes.Messaging;

namespace Storyteller.Core.Testing.Model.Persistence
{
    [TestFixture]
    public class HierarchyLoaderTester
    {
        [Test]
        public void read_a_spec_node()
        {
            var path = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                .AppendPath("Storyteller.Samples", "Specs", "General", "Check properties.xml");

            var spec = HierarchyLoader.ReadSpecNode(path);

            spec.name.ShouldEqual("Check properties");
            spec.lifecycle.ShouldEqual("Acceptance");
            spec.id.ShouldEqual("general1"); 
            spec.filename.ShouldEqual(path);
        }

        [Test]
        public void read_an_entire_suite()
        {
            var path = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                .AppendPath("Storyteller.Samples", "Specs");


            var hierarchy = HierarchyLoader.ReadHierarchy(path);

            hierarchy.suites.Select(x => x.name)
                .ShouldHaveTheSameElementsAs("Embedded", "General", "Paragraphs", "Sentences", "Sets", "Tables");

            /*
            var serializer = new Newtonsoft.Json.JsonSerializer();
            var writer = new StringWriter();
            serializer.Serialize(writer, hierarchy);

            var json = writer.ToString();

            Debug.WriteLine(json);
             * */
        }

        [Test, Explicit]
        public void pretty_print_for_sample_data()
        {
            var path = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                .AppendPath("Storyteller.Samples", "Specs");


            var hierarchy = HierarchyLoader.ReadHierarchy(path);

            var json = JsonSerialization.ToJson(hierarchy).FormatJson();
            Debug.WriteLine(json);
        }
    }
}