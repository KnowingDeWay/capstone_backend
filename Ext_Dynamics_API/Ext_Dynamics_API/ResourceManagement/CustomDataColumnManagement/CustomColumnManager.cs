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
                    DataColumn newKnownCol;
                    if(currDbCol.DataType == ColumnDataType.Number)
                    {
                        newKnownCol = new NumericDataColumn
                        {
                            Name = canvasCol.Title,
                            DataType = currDbCol.DataType,
                            ColumnType = currDbCol.ColumnType,
                            RelatedDataId = canvasCol.Id,
                            CalcRule = currDbCol.CalcRule,
                            ColMaxValue = currDbCol.ColMaxValue,
                            ColMinValue = currDbCol.ColMinValue,
                            ColumnId = Guid.NewGuid()
                        };
                        ((NumericDataColumn)newKnownCol).Rows = GetCustomDataRowsForNumberColumns(accessToken, courseId, newKnownCol);
                    }
                    else
                    {
                        newKnownCol = new StringDataColumn
                        {
                            Name = canvasCol.Title,
                            DataType = currDbCol.DataType,
                            ColumnType = currDbCol.ColumnType,
                            RelatedDataId = canvasCol.Id,
                            CalcRule = currDbCol.CalcRule,
                            ColMaxValue = currDbCol.ColMaxValue,
                            ColMinValue = currDbCol.ColMinValue,
                            ColumnId = Guid.NewGuid()
                        };
                        ((StringDataColumn)newKnownCol).Rows = GetCustomDataFromStringColumns(accessToken, courseId, newKnownCol);
                    }
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
                    if (newCol.DataType == ColumnDataType.Number)
                    {
                        var numCol = (NumericDataColumn)newCol;
                        numCol.Rows = new List<NumericDataRow>();
                        foreach(var row in newColRows)
                        {
                            numCol.Rows.Add(new NumericDataRow { 
                                ColumnId = Guid.NewGuid(),
                                AssociatedUser = row.AssociatedUser,
                                Value = double.Parse(row.Value),
                                ValueChanged = false
                            });
                        }
                        custDataColumns.Add(numCol);
                    }
                    else
                    {
                        var strCol = (StringDataColumn)newCol;
                        strCol.Rows = newColRows;
                        custDataColumns.Add(strCol);
                    }
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

        private List<NumericDataRow> GetCustomDataRowsForNumberColumns(string accessToken, int courseId, DataColumn column)
        {
            var data = new List<NumericDataRow>();
            var canvasDataEntries = _canvasDataAccess.GetCustomColumnEntries(accessToken, courseId, column.RelatedDataId);
            if(column.DataType == ColumnDataType.Number)
            {
                foreach(var entry in canvasDataEntries)
                {
                    data.Add(new NumericDataRow
                    {
                        AssociatedUser = _students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                        ColumnId = column.ColumnId,
                        ValueChanged = false,
                        Value = double.Parse(entry.Content)
                    });
                }
            }
            return data;
        }

        private List<StringDataRow> GetCustomDataFromStringColumns(string accessToken, int courseId, DataColumn column)
        {
            var data = new List<StringDataRow>();
            var canvasDataEntries = _canvasDataAccess.GetCustomColumnEntries(accessToken, courseId, column.RelatedDataId);
            foreach (var entry in canvasDataEntries)
            {
                data.Add(new StringDataRow
                {
                    AssociatedUser = _students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                    ColumnId = column.ColumnId,
                    ValueChanged = false,
                    Value = entry.Content
                });
            }
            return data;
        }

        private List<StringDataRow> GetCustomColumnDataRowsForUnknownTypes(string accessToken, int courseId, DataColumn column)
        {
            var data = new List<StringDataRow>();
            var canvasDataEntries = _canvasDataAccess.GetCustomColumnEntries(accessToken, courseId, column.RelatedDataId);
            foreach(var entry in canvasDataEntries)
            {
                data.Add(new StringDataRow { 
                    AssociatedUser = _students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                    ColumnId = column.ColumnId,
                    ValueChanged = false,
                    Value = entry.Content
                });
            }
            return data;
        }
    }
}
