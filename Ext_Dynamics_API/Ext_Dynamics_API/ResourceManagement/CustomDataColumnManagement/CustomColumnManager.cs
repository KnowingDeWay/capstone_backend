using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models.CustomTabModels;
using Ext_Dynamics_API.Canvas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResourceManagement.CustomDataColumnManagement
{
    public class CustomColumnManager
    {
        private ExtensibleDbContext _dbCtx;
        private CanvasDataAccess _canvasDataAccess;
        private SystemConfig _config;
        private List<DataTableStudent> _students;

        public CustomColumnManager(ExtensibleDbContext dbCtx, List<DataTableStudent> students)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _canvasDataAccess = new CanvasDataAccess(_config);
            _students = students;
        }

        public List<DataColumn> GetCustomDataColumns(string accessToken, int courseId)
        {
            var custDataColumns = new List<DataColumn>();
            var canvasCustCols = _canvasDataAccess.GetCustomColumns(accessToken, courseId);
            var custColsDb = _dbCtx.CustomDataColumns.Where(x => x.CourseId == courseId).ToList();
            foreach(var canvasCol in canvasCustCols)
            {
                var currDbCol = custColsDb.Where(x => x.RelatedDataId == canvasCol.Id).FirstOrDefault();
                if(currDbCol != null)
                {
                    var newKnownCol = new DataColumn
                    {
                        Name = canvasCol.Title,
                        DataType = currDbCol.DataType,
                        ColumnType = currDbCol.ColumnType,
                        RelatedDataId = canvasCol.Id,
                        CalcRule = currDbCol.CalcRule,
                        ColMaxValue = currDbCol.ColMaxValue,
                        ColMinValue = currDbCol.ColMinValue,
                    };
                    newKnownCol.Rows = GetCustomDataRowsForKnownColumnTypes(accessToken, courseId, newKnownCol);
                    custDataColumns.Add(newKnownCol);
                }
                else
                {
                    var newCol = new DataColumn
                    {
                        Name = canvasCol.Title,
                        RelatedDataId = canvasCol.Id,
                        ColumnType = ColumnType.Custom_Canvas_Column,
                    };
                    var newColRows = GetCustomColumnDataRowsForUnknownTypes(accessToken, courseId, newCol);
                    newCol.DataType = GetColumnDataType(newColRows[0].Value);
                    newCol.Rows = new List<DataRowBase>();
                    if (newCol.DataType == ColumnDataType.Number)
                    {
                        foreach(var row in newColRows)
                        {
                            newCol.Rows.Add(new NumericDataRow { 
                                DataColumn = newCol,
                                AssociatedUser = row.AssociatedUser,
                                Value = double.Parse(row.Value),
                                ValueChanged = false
                            });
                        }
                    }
                    else
                    {
                        newCol.Rows = newColRows;
                    }
                    custDataColumns.Add(newCol);
                }
            }
            return custDataColumns;
        }

        public ColumnDataType GetColumnDataType(string content)
        {
            try
            {
                double.Parse(content);
                return ColumnDataType.Number;
            }
            catch(FormatException)
            {
                return ColumnDataType.String;
            }
        }

        private List<DataRowBase> GetCustomDataRowsForKnownColumnTypes(string accessToken, int courseId, DataColumn column)
        {
            var data = new List<DataRowBase>();
            var canvasDataEntries = _canvasDataAccess.GetCustomColumnEntries(accessToken, courseId, column.RelatedDataId);
            if(column.DataType == ColumnDataType.Number)
            {
                foreach(var entry in canvasDataEntries)
                {
                    data.Add(new NumericDataRow
                    {
                        AssociatedUser = _students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                        DataColumn = column,
                        ValueChanged = false,
                        Value = double.Parse(entry.Content)
                    });
                }
            }
            else
            {
                foreach (var entry in canvasDataEntries)
                {
                    data.Add(new DataRowBase
                    {
                        AssociatedUser = _students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                        DataColumn = column,
                        ValueChanged = false,
                        Value = entry.Content
                    });
                }
            }
            return data;
        }

        private List<DataRowBase> GetCustomColumnDataRowsForUnknownTypes(string accessToken, int courseId, DataColumn column)
        {
            var data = new List<DataRowBase>();
            var canvasDataEntries = _canvasDataAccess.GetCustomColumnEntries(accessToken, courseId, column.RelatedDataId);
            foreach(var entry in canvasDataEntries)
            {
                data.Add(new DataRowBase { 
                    AssociatedUser = _students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                    DataColumn = column,
                    ValueChanged = false,
                    Value = entry.Content
                });
            }
            return data;
        }
    }
}
