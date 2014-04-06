using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core;
using DMT.Core.Interfaces;
using DMT.Core.Serialization;

namespace DMT.VIR.Data.Parser
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            var w = Stopwatch.StartNew();

            var dir = args[0];
            var users = new UserReader(dir).ReadAll();
            var groups = new GroupReader(dir).ReadAll();
            var memberships = new MembershipReader(dir).ReadAll();
            var scores = new CommunityScoreReader(dir).ReadAll();
            var valuations = new SemesterValuationReader(dir).ReadAllIntoList();

            new MembershipConnector(memberships, groups, users).Connect();
            new CommunitiyScoreConnector(scores, users).Connect();
            new SemesterValuationConnector(valuations, users, groups).Connect();

            List<INode> allNodes = new List<INode>();
            allNodes.AddRange(users.Values.Select(e => e.Entity));
            allNodes.AddRange(groups.Values.Select(e => e.Entity));
            allNodes.AddRange(memberships.Values.Select(e => e.Entity));
            allNodes.AddRange(scores.Values.Select(e => e.Entity));
            allNodes.AddRange(valuations.Select(e => e.Entity));

            ModelXmlSerializer s = new ModelXmlSerializer(new VirEntityFactory(), new ContextFactory());

            using (var modelWriter = XmlWriter.Create("model.xml"))
            {
                s.Serialize(modelWriter, new Model(allNodes));
            }

            w.Stop();
            Console.WriteLine("Elapsed: {0} sec", w.ElapsedMilliseconds / 1000.0);
            Console.ReadKey();
        }
    }
}
