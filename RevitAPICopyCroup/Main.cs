using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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
            try
            {

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                GroupPickFilter filter = new GroupPickFilter();
                Reference reference = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element,filter, "Выберете группу объектов");

                XYZ point = uidoc.Selection.PickPoint("Выберете точку");
                Element element = doc.GetElement(reference);
                Group group = element as Group;
                XYZ groupCenter = GetElementCenter(group);
                Room room1 = GetRoomByPoint(doc, point);
                XYZ room1Center = GetElementCenter(room1);
                XYZ offset1 = groupCenter - room1Center;

                XYZ point2 = uidoc.Selection.PickPoint("Выберете точку для вставки");
                Room room2 = GetRoomByPoint(doc, point2);
                XYZ room1Center2 = GetElementCenter(room2);
                XYZ pointPaste = room1Center2 + offset1;

                using (var ts = new Transaction(doc, "Cope Group"))
                {
                    ts.Start();

                   

                    doc.Create.PlaceGroup(pointPaste, group.GroupType);

                    ts.Commit();

                }

            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            { 
                message = ex.Message;
                return Result.Failed;
            }


            return Result.Succeeded;


        }
        public XYZ GetElementCenter(Element element) 
        {
            BoundingBoxXYZ bounding= element.get_BoundingBox(null);
            return (bounding.Max+bounding.Min)/2;
        
        }
        public Room GetRoomByPoint(Document doc, XYZ point)
        {
            FilteredElementCollector collector= new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms);
            foreach (Element element in collector) 
            { 
                Room room = element as Room;
                if (room != null) 
                {
                    if (room.IsPointInRoom(point))
                    {
                        return room;
                    }                                 
                }
            }
            return null;

        }
    }
    public class GroupPickFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if(elem.Category.Id.IntegerValue== (int) BuiltInCategory.OST_IOSModelGroups)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
