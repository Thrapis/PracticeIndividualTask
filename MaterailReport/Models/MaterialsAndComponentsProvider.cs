using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace MaterailReport.Models
{
    public class MaterialObject
    {
        public string Name { get; set; }
        public string System_Type { get; set; }
        public string Comments { get; set; }
    }

    public class ConstituentComponentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Recyclable { get; set; }
        public string Description { get; set; }
    }

    public class ConstituentComponent
    {
        public int Id { get; set; }
        public int Constituent_Component_Type_Id { get; set; }
        public double Count { get; set; }
        public string Count_Unit { get; set; }
        public string Material_Object_Name { get; set; }
    }

    public class MaterialsAndComponentsProvider : IDisposable
    {
        SqlConnection _connection;

        public MaterialsAndComponentsProvider(SqlConnection connection)
        {
            _connection = connection;
        }

        public MaterialsAndComponentsProvider()
        {
            _connection = MRConnection.GetConnection();
        }

        public MaterialObject GetMaterialObjectByName(string name)
        {
            return _connection.QueryFirstOrDefault<MaterialObject>($@"execute GetMaterialObjectByName N'{name}'");
        }

        public IEnumerable<ConstituentComponent> GetConstituentComponentsByMaterialObjectName(string materialObjectName)
        {
            return _connection.Query<ConstituentComponent>($@"execute GetConstituentComponentsByMaterialObjectName N'{materialObjectName}'");
        }
        
        public IEnumerable<ConstituentComponentType> GetConstituentComponentTypesByMaterialObjectName(string materialObjectName)
        {
            return _connection.Query<ConstituentComponentType>($@"execute GetConstituentComponentTypesByMaterialObjectName N'{materialObjectName}'");
        }

        public ConstituentComponentType GetConstituentComponentTypeById(int id)
        {
            return _connection.QueryFirstOrDefault<ConstituentComponentType>($@"execute GetConstituentComponentTypeById {id}");
        }


        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}