using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MaterailReport.Models.ExcelEditing
{
    public class ReportPackage
    {
        public List<ReportPackageField> PackageFields { get; set; }
        public int FieldCount { get => PackageFields.Count; }
        public Dictionary<int, ConstituentComponentType> ComponentTypes { get; set; }
        public SortedSet<string> SystemTypes { get; set; }

        public ReportPackage(List<int> inventoryIds)
        {
            SqlConnection connection = MRConnection.GetConnection();
            WarehouseProvider warehouseProvider = new WarehouseProvider(connection);
            MaterialsAndComponentsProvider macProvider = new MaterialsAndComponentsProvider(connection);

            PackageFields = new List<ReportPackageField>();
            ComponentTypes = new Dictionary<int, ConstituentComponentType>();
            SystemTypes = new SortedSet<string>();

            foreach (int id in inventoryIds)
            {
                DecommissionedContentOfWarehouse dcow = warehouseProvider.GetDecommissionedContentFromWarehouse(id);
                MaterialObject material = macProvider.GetMaterialObjectByName(dcow.Material_Object_Name);
                List<ConstituentComponent> components = macProvider.GetConstituentComponentsByMaterialObjectName(dcow.Material_Object_Name).ToList();
                List<ConstituentComponentType> componentTypes = macProvider.GetConstituentComponentTypesByMaterialObjectName(dcow.Material_Object_Name).ToList();

                PackageFields.Add(new ReportPackageField(dcow, material, components));
                foreach (var type in componentTypes)
                {
                    if (!ComponentTypes.ContainsKey(type.Id))
                        ComponentTypes.Add(type.Id, type);
                }

                if (!SystemTypes.Contains(material.System_Type))
                    SystemTypes.Add(material.System_Type);
            }
        }

        public List<ReportPackageField> GetPackageFieldsBySystemType(string systemType)
        {
            return PackageFields.Where(el => el.SystemType == systemType).ToList();
        }
    }

    public class ReportPackageField
    {
        public DecommissionedContentOfWarehouse DecommissionedContent { get; set; }
        public MaterialObject MaterialObject { get; set; }
        public string SystemType { get => MaterialObject.System_Type; }
        public List<ConstituentComponent> ConstituentComponents { get; set; }

        public ReportPackageField(DecommissionedContentOfWarehouse decommissionedContent, MaterialObject materialObject, List<ConstituentComponent> constituentComponents)
        {
            DecommissionedContent = decommissionedContent;
            MaterialObject = materialObject;
            ConstituentComponents = constituentComponents;
        }
    }
}