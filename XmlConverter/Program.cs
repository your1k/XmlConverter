using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml.Linq;

namespace XmlConverter
{
    class Program
    {
        const string ConnectionString = @"Server=localhost\SQLEXPRESS;Database=AccountingExpensesIncomeDataBase;Trusted_Connection=True;";
        static void Main(string[] args)
        {
            Console.WriteLine(GetXml("SELECT * FROM Data"));
            Console.Read();
        }

        static List<dynamic> GetData(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var dt = new DataTable();
                new SqlDataAdapter(new SqlCommand(query, connection)).Fill(dt);
                return dt.AsEnumerable().Select(c => c).AsQueryable().Select("new(" + string.Join(", ", dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName)) + ")").ToDynamicList();
            }
        }

        static XDocument GetXml(string query) 
        {
            var elements = new XElement("Data");
            foreach (var item in GetData(query))
            {
                XElement element = new XElement("Item");
                foreach (var prop in TypeDescriptor.GetProperties(item))
                    element.Add(new XElement(prop.Name, item.GetType().GetProperty(prop.Name).GetValue(item)));
                elements.Add(element);
            }
            return new XDocument(elements);
        }
    }
}
