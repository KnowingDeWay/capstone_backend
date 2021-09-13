﻿using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Canvas.DataAccessModels;
using Ext_Dynamics_API.Canvas.RequestModels;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.Models.CustomTabModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResourceManagement
{
    public class CourseDataTableManager
    {
        private ExtensibleDbContext _dbCtx;
        private CanvasDataAccess _canvasDataAccess;
        private SystemConfig _config;
        public List<DataTableStudent> Students { get; set; }

        public CourseDataTableManager(ExtensibleDbContext dbCtx, List<DataTableStudent> students = null)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _canvasDataAccess = new CanvasDataAccess(_config);
            Students = students;
        }

        public List<DataColumn> GetCustomDataColumns(string accessToken, int courseId)
        {
            var custDataColumns = new List<DataColumn>();
            var canvasCustCols = _canvasDataAccess.GetCustomColumns(accessToken, courseId);
            var custColsDb = _dbCtx.CustomDataColumns.Where(x => x.CourseId == courseId).ToList();
            foreach (var canvasCol in canvasCustCols)
            {
                var currDbCol = custColsDb.Where(x => x.RelatedDataId == canvasCol.Id).FirstOrDefault();
                if (currDbCol != null)
                {
                    // The derived data columns should be loaded last since they derive data from other columns
                    // (hence it being called a "Dervied" column)
                    if(currDbCol.ColumnType != ColumnType.Derived_Data)
                    {
                        DataColumn newKnownCol;
                        if (currDbCol.DataType == ColumnDataType.Number)
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
                        foreach (var row in newColRows)
                        {
                            numCol.Rows.Add(new NumericDataRow
                            {
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

        public List<NumericDataColumn> GetDerivedColumns(int courseId, CourseDataTable table)
        {
            var derivedColumns = new List<NumericDataColumn>();
            var custColsDb = _dbCtx.CustomDataColumns.Where(x => x.CourseId == courseId 
            && x.ColumnType == ColumnType.Derived_Data).ToList();

            foreach(var entry in custColsDb)
            {
                var newCol = new NumericDataColumn
                {
                    Name = entry.Name,
                    ColumnType = entry.ColumnType,
                    RelatedDataId = (entry.RelatedDataId != null) ? (int)entry.RelatedDataId : -1,
                    DataType = entry.DataType,
                    CalcRule = entry.CalcRule,
                    ColMaxValue = entry.ColMaxValue,
                    ColMinValue = entry.ColMinValue,
                    ColumnId = Guid.NewGuid(),
                    Rows = new List<NumericDataRow>()
                };
                var derivedColManager = new DerivedColumnManager(table);
                derivedColManager.LoadDerivedDataColumn(ref newCol);
                derivedColumns.Add(newCol);
            }

            return derivedColumns;
        }

        public ColumnDataType GetColumnDataType(string content)
        {
            try
            {
                double.Parse(content);
                return ColumnDataType.Number;
            }
            catch (FormatException)
            {
                return ColumnDataType.String;
            }
        }

        public bool AddNewColumn(string accessToken, DataColumn column, int courseId, int userId)
        {
            switch(column.ColumnType)
            {
                case ColumnType.Custom_Canvas_Column: return AddCustomColumn(accessToken, column, courseId, userId);
                case ColumnType.Derived_Data: return AddDerivedColumn(accessToken, column, courseId, userId);
                case ColumnType.File_Import: return AddFileColumn(accessToken, column, courseId, userId);
                default: return false;
            }
        }

        public bool AddCustomColumn(string accessToken, DataColumn column, int courseId, int userId)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name
            };
            
            try
            {
                _canvasDataAccess.AddNewCustomColumn(accessToken, request, courseId);
            }
            catch(Exception)
            {
                return false;
            }

            var newEntry = new DataColumnEntry
            {
                Name = column.Name,
                CourseId = courseId,
                ColMaxValue = column.ColMaxValue,
                ColMinValue = column.ColMinValue,
                DataType = column.DataType,
                RelatedDataId = column.RelatedDataId,
                UserId = userId,
                ColumnType = column.ColumnType,
            };

            _dbCtx.CustomDataColumns.Add(newEntry);

            var success = SaveColumnEntriesChangesConccurently(newEntry);

            return success;
        }

        public bool AddDerivedColumn(string accessToken, DataColumn column, int courseId, int userId)
        {
            return false;
        }

        public bool AddFileColumn(string accessToken, DataColumn column, int courseId, int userId)
        {
            return false;
        }

        private bool SaveColumnEntriesChangesConccurently(DataColumnEntry newEntry) 
        {
            try
            {
                _dbCtx.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var item in e.Entries)
                {
                    if (item.Entity is CustomColumnDataEntry)
                    {
                        var currValues = item.CurrentValues;
                        var dbValues = item.GetDatabaseValues();

                        foreach (var property in currValues.Properties)
                        {
                            var currentValue = currValues[property];
                            var dbValue = dbValues[property];
                        }

                        // Refresh the original values to bypass next concurrency check
                        item.OriginalValues.SetValues(dbValues);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                // Rollback token entry
                _dbCtx.CustomDataColumns.Remove(newEntry);
                return false;
            }
        }

        private List<NumericDataRow> GetCustomDataRowsForNumberColumns(string accessToken, int courseId, DataColumn column)
        {
            var data = new List<NumericDataRow>();
            if(column.ColumnType != ColumnType.Derived_Data)
            {
                var canvasDataEntries = _canvasDataAccess.GetCustomColumnEntries(accessToken, courseId, column.RelatedDataId);
                if (column.DataType == ColumnDataType.Number)
                {
                    foreach (var entry in canvasDataEntries)
                    {
                        data.Add(new NumericDataRow
                        {
                            AssociatedUser = Students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                            ColumnId = column.ColumnId,
                            ValueChanged = false,
                            Value = double.Parse(entry.Content)
                        });
                    }
                }
            }
            else
            {

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
                    AssociatedUser = Students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
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
            foreach (var entry in canvasDataEntries)
            {
                data.Add(new StringDataRow
                {
                    AssociatedUser = Students.Where(x => x.Id == entry.UserId).FirstOrDefault(),
                    ColumnId = column.ColumnId,
                    ValueChanged = false,
                    Value = entry.Content
                });
            }
            return data;
        }

        public bool EditTable(int courseId, string accessToken, CourseDataTable table)
        {
            var editSuccessful = EditAssignmentValues(courseId, accessToken, table);
            if(editSuccessful)
            {
                EditCustomValues(courseId, accessToken, table);
            }
            return editSuccessful;
        }

        private bool EditAssignmentValues(int courseId, string accessToken, CourseDataTable table)
        {
            foreach (var column in table.AssignmentGradeColumns)
            {
                if(column.ColumnType != ColumnType.Derived_Data)
                {
                    var numericCol = (NumericDataColumn)column;
                    var alteredRows = new List<NumericDataRow>();
                    var newRowEntries = new List<AssignmentGradeChangeEntry>();
                    alteredRows.AddRange(numericCol.Rows.Where(x => x.ValueChanged).ToList());
                    foreach (var row in alteredRows)
                    {
                        newRowEntries.Add(new AssignmentGradeChangeEntry
                        {
                            StudentId = row.AssociatedUser.Id,
                            NewGrade = row.NewValue
                        });
                    }

                    try
                    {
                        _canvasDataAccess.SetAssignmentColumnEntries(accessToken, courseId, column.RelatedDataId, newRowEntries);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool EditCustomValues(int courseId, string accessToken, CourseDataTable table)
        {
            var updateReq = new CustomColumnsUpdateRequest();
            updateReq.ColumnData = new List<CustomColumnDataEntry>();
            foreach(var column in table.CustomDataColumns)
            {
                if(column.DataType == ColumnDataType.Number)
                {
                    var numCol = (NumericDataColumn)column;
                    var editedRows = numCol.Rows.Where(x => x.ValueChanged).ToList();
                    updateReq.ColumnData.AddRange(GetUpdatedNumericRows(courseId, accessToken, numCol.Rows, column));
                }
                else
                {
                    var numCol = (StringDataColumn)column;
                    var editedRows = numCol.Rows.Where(x => x.ValueChanged).ToList();
                    updateReq.ColumnData.AddRange(GetUpdatedStringRows(courseId, accessToken, numCol.Rows, column));
                }
            }

            if(updateReq.ColumnData.Count == 0)
            {
                return true;
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, updateReq);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private List<CustomColumnDataEntry> GetUpdatedNumericRows(int courseId, string accessToken, List<NumericDataRow> rows,
            DataColumn column)
        {
            var updateReq = new List<CustomColumnDataEntry>();
            foreach(var row in rows)
            {
                updateReq.Add(new CustomColumnDataEntry { 
                    ColumnId = column.RelatedDataId,
                    UserId = row.AssociatedUser.Id,
                    Content = $"{row.NewValue}"
                });
            }
            return updateReq;
        }

        private List<CustomColumnDataEntry> GetUpdatedStringRows(int courseId, string accessToken, List<StringDataRow> rows,
            DataColumn column)
        {
            var updateReq = new List<CustomColumnDataEntry>();
            foreach (var row in rows)
            {
                updateReq.Add(new CustomColumnDataEntry
                {
                    ColumnId = column.RelatedDataId,
                    UserId = row.AssociatedUser.Id,
                    Content = $"{row.NewValue}"
                });
            }
            return updateReq;
        }
    }
}