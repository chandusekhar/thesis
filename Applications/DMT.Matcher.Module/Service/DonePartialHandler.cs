using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DMT.Common.Rest;
using DMT.Common.Extensions;
using System.Xml;
using DMT.Matcher.Data.Interfaces;
using DMT.Core.Interfaces.Serialization;
using System.ComponentModel.Composition;
using DMT.Common.Composition;

namespace DMT.Matcher.Module.Service
{
    class DonePartialHandler : IRouteHandler
    {
        [Import]
        IContextFactory contextFactory;

        public DonePartialHandler()
        {
            CompositionService.Default.InjectOnce(this);
        }

        public void Handle(Request request, Response response)
        {
            Guid id = request.Params["id"].ParseGuid();

            IPattern pattern = MatcherModule.Instance.Job.JobFactory.CreateEmptyPattern();
            using (XmlReader reader = XmlReader.Create(request.Body))
            {
                reader.ReadToFollowing("Pattern");
                if (!reader.IsEmptyElement)
                {
                    pattern.Deserialize(reader, contextFactory.CreateContext());
                }
            }

            MatcherModule.Instance.Job.MatcherFramwork.ReleasePartialMatchNode(id, pattern);
        }
    }
}
