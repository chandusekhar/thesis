using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace DMT.VIR.Data.Parser
{
    class MembershipReader : EntityReaderBase<MembershipWrapper>
    {
        private Dictionary<int, List<string>> posts = new Dictionary<int, List<string>>();

        public MembershipReader(string dir)
            : base(dir)
        {
        }

        protected override MembershipWrapper CreateEntity(CsvReader reader)
        {
            return new MembershipWrapper
            {
                Start = DateTime.ParseExact(reader.GetField("membership_start"), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                End = ParseEndDate(reader),
                UserId = reader.GetField<int>("usr_id"),
                GroupId = reader.GetField<int>("grp_id"),
                Posts = GetPosts(reader),
            };
        }

        protected override string GetFilename()
        {
            return "grp_membership.csv";
        }

        protected override void Initialize()
        {
            var path = Path.Combine(dir, "poszttipus.csv");

            var postTypes = new Dictionary<int, string>();
            using (var reader = new CsvReader(new StreamReader(path), new CsvConfiguration { Delimiter = ";" }))
            {
                while (reader.Read())
                {
                    postTypes.Add(reader.GetField<int>("pttip_id"), reader.GetField("pttip_name"));
                }
            }

            using (var reader = new CsvReader(new StreamReader(Path.Combine(dir, "poszt.csv")), new CsvConfiguration { Delimiter = ";" }))
            {
                while (reader.Read())
                {
                    var msId = reader.GetField<int>("grp_member_id");
                    if (!posts.ContainsKey(msId))
                    {
                        posts.Add(msId, new List<string>());
                    }
                    posts[msId].Add(postTypes[reader.GetField<int>("pttip_id")]);
                }
            }
        }

        private DateTime ParseEndDate(CsvReader reader)
        {
            string raw = reader.GetField("membership_end");
            return string.IsNullOrEmpty(raw) ? DateTime.MinValue : DateTime.ParseExact(raw, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private List<string> GetPosts(CsvReader reader)
        {
            var id = reader.GetField<int>("id");
            if (this.posts.ContainsKey(id))
            {
                return new List<string>(this.posts[id]);
            }
            return new List<string>();
        }
    }
}
