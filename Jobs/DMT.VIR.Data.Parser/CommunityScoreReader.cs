using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace DMT.VIR.Data.Parser
{
    class CommunityScoreReader : EntityReaderBase<CommunityScoreWrapper>
    {
        public CommunityScoreReader(string dir)
            : base(dir)
        {

        }

        protected override CommunityScoreWrapper CreateEntity(CsvReader reader)
        {
            return new CommunityScoreWrapper
            {
                UserId = reader.GetField<int>("usr_id"),
                Score = reader.GetField<int>("point"),
                Semester = new Semester(reader.GetField("semester"))
            };
        }

        protected override string GetFilename()
        {
            return "point_history.csv";
        }
    }
}
