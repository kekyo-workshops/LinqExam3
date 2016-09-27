using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqExam3
{
    #region SQLiteConfiguration
    public class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            SetProviderFactory(
                "System.Data.SQLite",
                SQLiteFactory.Instance);
            SetProviderFactory(
                "System.Data.SQLite.EF6",
                SQLiteProviderFactory.Instance);
            SetProviderServices(
                "System.Data.SQLite",
                (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
    #endregion

    public class x_ken_all
    {
        public string JISX0401 { get; set; }
        public string OldZipCode { get; set; }
        [Key]
        public string NewZipCode { get; set; }
        public string PrefectureKana { get; set; }
        public string CityWardKana { get; set; }
        public string AddressKana { get; set; }
        public string Prefecture { get; set; }
        public string CityWard { get; set; }
        public string Address { get; set; }
        public string Dup1 { get; set; }
        public string WithNumber { get; set; }
        public string StreetNumber { get; set; }
        public string Dup2 { get; set; }
        public string Changed { get; set; }
        public string Detail { get; set; }
    }

    public class x_ken_all_context : DbContext
    {
        public x_ken_all_context(DbConnection connection)
            : base(connection, false)
        {
        }

        public DbSet<x_ken_all> x_ken_alls { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = Path.GetFullPath("x_ken_all.sqlite");

            using (var connection = SQLiteProviderFactory.Instance.CreateConnection())
            {
                connection.ConnectionString = builder.ConnectionString;

                using (var context = new x_ken_all_context(connection))
                {
                    var q =
                        from record in context.x_ken_alls
                        group record by record.OldZipCode
                        into g
                        select new
                        {
                            ID = g.Key,
                            Infos = g
                        };

                    Console.WriteLine(string.Join(
                        "\r\n",
                        q.
                        AsEnumerable().
                        Select(entry =>
                            string.Format("ID={0},\r\n  {1}",
                                entry.ID,
                                string.Join("\r\n  ",
                                entry.Infos.Select(info =>
                                    $"{info.NewZipCode}, {info.Prefecture}{info.CityWard}{info.Address}"))))));
                }
            }
        }
    }
}
