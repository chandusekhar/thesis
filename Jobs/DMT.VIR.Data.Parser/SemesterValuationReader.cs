using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace DMT.VIR.Data.Parser
{
    class SemesterValuationReader : EntityReaderBase<SemesterValuationWrapper>
    {
        List<Point> points;

        public SemesterValuationReader(string dir)
            : base(dir)
        {

        }

        public List<SemesterValuationWrapper> ReadAllIntoList()
        {
            List<SemesterValuationWrapper> list = new List<SemesterValuationWrapper>();
            using (var reader = CreateReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetField<int>("id");
                    foreach (var point in points.Where(p => p.ValId == id))
                    {
                        var sv = new SemesterValuationWrapper
                        {
                            GroupId = reader.GetField<int>("grp_id"),
                            UserId = point.UserId,
                            Score = point.Value,
                            ValuationId = id,
                            Semester = new Semester(reader.GetField("semester")),
                            Status = reader.GetField("pontigeny_statusz"),
                            NextValuationId = GetNextId(reader),
                        };
                        list.Add(sv);
                    }
                }
            }

            return list;
        }

        protected override SemesterValuationWrapper CreateEntity(CsvReader reader)
        {
            throw new NotSupportedException();
        }

        protected override string GetFilename()
        {
            return "ertekelesek.csv";
        }

        protected override void Initialize()
        {
            this.points = new List<Point>();
            string path = Path.Combine(dir, "pontigenyles.csv");
            using (CsvReader reader = new CsvReader(new StreamReader(path), new CsvConfiguration { Delimiter = ";" }))
            {
                while (reader.Read())
                {
                    points.Add(new Point
                    {
                        UserId = reader.GetField<int>("usr_id"),
                        ValId = reader.GetField<int>("ertekeles_id"),
                        Value = reader.GetField<int>("pont")
                    });
                }
            }
        }

        private int GetNextId(CsvReader r)
        {
            var idstr = r.GetField("next_version");
            int id = -1;
            int.TryParse(idstr, out id);
            return id;
        }

        private class Point
        {
            public int UserId { get; set; }
            public int ValId { get; set; }
            public int Value { get; set; }
        }
    }
}
