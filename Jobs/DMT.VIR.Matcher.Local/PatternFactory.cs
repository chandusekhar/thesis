﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Composition;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Matcher.Local
{
    class PatternFactory
    {
        [Import]
        private IMatcherEntityFactory entityFactory;
        private IPattern pattern;

        public PatternFactory()
        {
            CompositionService.Default.InjectOnce(this);
            this.pattern = this.entityFactory.CreatePattern();
        }

        public static IPattern CreateUnmatched()
        {
            return new PatternFactory().CreateUnmatchedPattern();
        }

        public IPattern CreateUnmatchedPattern()
        {
            // Create person as the center of the pattern.
            var person = this.entityFactory.CreatePatternNode(PatternNodes.Person);
            this.pattern.AddNodes(person);

            AddActiveMembershipSubpattern(person);
            AddCommunityScoreSubpattern(person);
            AddGroupLeaderSubpattern(person);
            AddSemesterValuationWithActiveMemebershipSuppattern(person);

            return this.pattern;
        }

        private void AddSemesterValuationWithActiveMemebershipSuppattern(IPatternNode person)
        {
            var activeMembership1 = this.entityFactory.CreatePatternNode(PatternNodes.ActiveMembership1);
            var group1 = this.entityFactory.CreatePatternNode(PatternNodes.Group1);
            var semesterValuation = this.entityFactory.CreatePatternNode(PatternNodes.SemesterValuation);
            var semesterValuationNext = this.entityFactory.CreatePatternNode(PatternNodes.SemesterValuationNext);

            person.ConnectTo(activeMembership1, EdgeDirection.Both);
            activeMembership1.ConnectTo(group1, EdgeDirection.Both);
            group1.ConnectTo(semesterValuation, EdgeDirection.Both);
            group1.ConnectTo(semesterValuationNext, EdgeDirection.Both);
            semesterValuation.ConnectTo(person, EdgeDirection.Both);
            semesterValuationNext.ConnectTo(person, EdgeDirection.Both);
            semesterValuation.ConnectTo(semesterValuation, EdgeDirection.Both);

            this.pattern.AddNodes(activeMembership1, group1, semesterValuation, semesterValuationNext);
        }

        private void AddCommunityScoreSubpattern(IPatternNode person)
        {
            var communityPoint = this.entityFactory.CreatePatternNode(PatternNodes.CommunityScore);
            person.ConnectTo(communityPoint, EdgeDirection.Both);
            this.pattern.AddNodes(communityPoint);
        }

        private void AddGroupLeaderSubpattern(IPatternNode person)
        {
            var groupLeader = this.entityFactory.CreatePatternNode(PatternNodes.GroupLeader);
            person.ConnectTo(groupLeader, EdgeDirection.Both);

            this.pattern.AddNodes(groupLeader);
        }

        private void AddActiveMembershipSubpattern(IPatternNode person)
        {
            var activeMembership2 = this.entityFactory.CreatePatternNode(PatternNodes.ActiveMembership2);
            var group2 = this.entityFactory.CreatePatternNode(PatternNodes.Group2);

            person.ConnectTo(activeMembership2, EdgeDirection.Both);
            activeMembership2.ConnectTo(group2, EdgeDirection.Both);

            this.pattern.AddNodes(activeMembership2, group2);
        }
    }

    class PatternNodes
    {
        public const string Person = "person";
        public const string GroupLeader = "group leader";
        public const string CommunityScore = "community point";
        public const string ActiveMembership1 = "active membership1";
        public const string Group1 = "group1";
        public const string SemesterValuation = "semester valuation";
        public const string SemesterValuationNext = "semester valuation next";
        public const string ActiveMembership2 = "active membership2";
        public const string Group2 = "group2";
    }
}
