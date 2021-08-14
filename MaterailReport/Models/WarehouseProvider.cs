using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MaterailReport.Models
{
    public class ContentOfWarehouse
    {
        public int Inventory_Id { get; set; }
        public string Material_Object_Name { get; set; }
        public DateTime Commissioning_Date { get; set; }
    }

    public class BrokenContentOfWarehouse
    {
        public int Inventory_Id { get; set; }
        public string Material_Object_Name { get; set; }
        public DateTime Commissioning_Date { get; set; }
        public double Restoration_Coast { get; set; }
        public double Deprecation { get; set; }
    }

    public class DecommissionedContentOfWarehouse
    {
        public int Inventory_Id { get; set; }
        public string Material_Object_Name { get; set; }
        public DateTime Commissioning_Date { get; set; }
        public double Restoration_Coast { get; set; }
        public double Deprecation { get; set; }
        public DateTime Decommissioning_Date { get; set; }
    }

    public class WarehouseProvider : IDisposable
    {
        SqlConnection _connection;

        public WarehouseProvider(SqlConnection connection)
        {
            _connection = connection;
        }

        public WarehouseProvider()
        {
            _connection = MRConnection.GetConnection();
        }


        public int AddContentToWarehouse(string materialObjectName, DateTime commissioningDate)
        {
            return _connection.Execute($@"execute AddContentToWarehouse N'{materialObjectName}', '{commissioningDate.ToString("yyyyMMdd hh:mm:ss tt")}'");
        }
        
        public ContentOfWarehouse GetContentFromWarehouse(int inventoryId)
        {
            return _connection.QueryFirstOrDefault<ContentOfWarehouse>($@"execute GetContentFromWarehouseById {inventoryId}");
        }

        public BrokenContentOfWarehouse GetBrokenContentFromWarehouse(int inventoryId)
        {
            return _connection.QueryFirstOrDefault<BrokenContentOfWarehouse>($@"execute GetBrokenContentFromWarehouseById {inventoryId}");
        }

        public DecommissionedContentOfWarehouse GetDecommissionedContentFromWarehouse(int inventoryId)
        {
            return _connection.QueryFirstOrDefault<DecommissionedContentOfWarehouse>($@"execute GetDecommissionedContentFromWarehouseById {inventoryId}");
        }


        public IEnumerable<ContentOfWarehouse> GetContentOfWarehouseByIdRange(int part)
        {
            return _connection.Query<ContentOfWarehouse>($@"execute GetContentOfWarehouseByIdRange {part}");
        }
        
        public int GetContentPartsCount()
        {
            return _connection.QueryFirstOrDefault<int>($@"execute GetContentPartsCount");
        }

        public IEnumerable<BrokenContentOfWarehouse> GetBrokenContentOfWarehouseByIdRange(int part)
        {
            return _connection.Query<BrokenContentOfWarehouse>($@"execute GetBrokenContentOfWarehouseByIdRange {part}");
        }

        public int GetBrokenContentPartsCount()
        {
            return _connection.QueryFirstOrDefault<int>($@"execute GetBrokenContentPartsCount");
        }

        public IEnumerable<DecommissionedContentOfWarehouse> GetDecommissionedContentOfWarehouseByIdRange(int part)
        {
            return _connection.Query<DecommissionedContentOfWarehouse>($@"execute GetDecommissionedContentOfWarehouseByIdRange {part}");
        }

        public int GetDecommissionedContentPartsCount()
        {
            return _connection.QueryFirstOrDefault<int>($@"execute GetDecommissionedContentPartsCount");
        }



        public int FromSimpleToBroken(int inventoryId, double restorationCoast, double deprecation)
        {
            string restorFormatted = restorationCoast.ToString().Replace(",", ".");
            string deprFormatted = deprecation.ToString().Replace(",", ".");
            return _connection.Execute($@"execute FromSimpleToBroken {inventoryId}, {restorFormatted}, {deprFormatted}");
        }

        public int FromBrokenToSimple(int inventoryId)
        {
            return _connection.Execute($@"execute FromBrokenToSimple {inventoryId}");
        }

        public int FromBrokenToDecommissioned(int inventoryId, DateTime decommissioningDate)
        {
            return _connection.Execute($@"execute FromBrokenToDecommissioned {inventoryId}, '{decommissioningDate.ToString("yyyyMMdd hh:mm:ss tt")}'");
        }



        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}