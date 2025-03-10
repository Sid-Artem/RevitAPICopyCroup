using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICopyCroup
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference reference=uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выберете группу объектов");

            Element element = doc.GetElement(reference);
            Group group = element as Group;
            XYZ point = uidoc.Selection.PickPoint("Выберете точку");




            using (var ts = new Transaction(doc, "Cope Group"))
            {
                ts.Start();

                doc.Create.PlaceGroup(point, group.GroupType);

                ts.Commit();

            }


            return Result.Succeeded;
        }
    }
}
