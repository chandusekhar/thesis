﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Xml;
using DMT.Common.Rest;
using DMT.Common.Extensions;
using DMT.Core.Interfaces.Serialization;
using DMT.Matcher.Data.Interfaces;
using DMT.Common.Composition;

namespace DMT.Matcher.Module.Service
{
    class FindPartialHandler : IRouteHandler
    {
        [Import]
        IContextFactory contextFactory;

        public FindPartialHandler()
        {
            CompositionService.Default.InjectOnce(this);
        }

        public void Handle(Request request, Response response)
        {
            var job = MatcherModule.Instance.Job;
            Guid sessionId = request.Params["id"].ParseGuid();

            var session = SessionStore.Deafult.CreateSession(sessionId);
            session.Url = request.Params["callback"];

            using (XmlReader reader = XmlReader.Create(request.Body))
            {
                // xml structure in DMT.Matcher.Module.Service.MatcherServiceClient#FindPartialMatch
                reader.ReadToFollowing("Pattern");
                IPattern pattern = job.JobFactory.CreateEmptyPattern();
                pattern.Deserialize(reader, contextFactory.CreateContext());
                job.InnerJob.FindPartialMatch(sessionId, pattern);
            }
        }
    }
}
