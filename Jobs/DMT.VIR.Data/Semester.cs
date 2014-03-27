using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Core.Interfaces.Serialization;

namespace DMT.VIR.Data
{
    public class Semester : ISerializable
    {
        const string FirstYearTag = "FirstYear";
        const string SecondYearTag = "SecondYear";
        const string PeriodTag = "Period";

        public int FirstYear  { get; set; }
        public int SecondYear { get; set; }
        public SemesterPeriod Period { get; set; }

        public Semester()
        {

        }

        public Semester(string semester)
        {
            // 201320142 -> 2013/2014 tavasz
            this.FirstYear = int.Parse(semester.Substring(0, 4));
            this.SecondYear = int.Parse(semester.Substring(4, 4));
            this.Period = (SemesterPeriod)int.Parse(semester.Substring(8, 1));
        }

        public enum SemesterPeriod
        {
            Autumn = 1,
            Spring = 2
        }

        public void Serialize(XmlWriter writer)
        {
            writer.WriteElementString(FirstYearTag, this.FirstYear.ToString());
            writer.WriteElementString(SecondYearTag, this.SecondYear.ToString());
            writer.WriteElementString(PeriodTag, this.Period.ToString());
        }

        public void Deserialize(XmlReader reader, IContext context)
        {
            reader.ReadToFollowing(FirstYearTag);
            this.FirstYear = reader.ReadElementContentAsInt();
            this.SecondYear = reader.ReadElementContentAsInt();
            this.Period = (SemesterPeriod)Enum.Parse(typeof(SemesterPeriod), reader.ReadElementContentAsString());
        }
    }
}
