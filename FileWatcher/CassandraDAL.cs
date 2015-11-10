using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cassandra;
using System.Data;

namespace FileWatcher
{
    class CassandraDAL
    {
        Cluster cluster;
        ISession session;

        public CassandraDAL()
        {
            
        }

        public void Open()
        {
            cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            session = cluster.Connect("filewatch");
        }

        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable("results");
            dt.Columns.Add("job", typeof(int));
            dt.Columns.Add("path", typeof(string));
            dt.Columns.Add("filename", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("datecreated", typeof(DateTime));

            RowSet results = session.Execute(query);
            foreach (Row row in results.GetRows())
            {
                /*
                Console.WriteLine(String.Format("{0, -30}\t{1, -20}\t{2, -20}\t{3}",
                row.GetValue<String>("title"), row.GetValue<String>("album"),
                row.GetValue<String>("artist"),
                row.GetValue<List<String>>("tags").ToString()));
                 */
                dt.Rows.Add(row.GetValue<Double>("job"), row.GetValue<String>("path"), row.GetValue<String>("filename"),
                    row.GetValue<String>("status"), row.GetValue<DateTime>("datecreated"));

            }            return dt;
        }

        public void Close()
        {
            cluster.Shutdown();
            session.Dispose();
        }
    }
}
