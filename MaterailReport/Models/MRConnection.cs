using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MaterailReport.Models
{
    public class MRConnection
    {
        private static string connectionString = @"server=WIN-QSRMNQOT9SQ;database=material_report;user id=BAA;password=Artyom1";

        public static SqlConnection GetConnection() => new SqlConnection(connectionString);
    }
}