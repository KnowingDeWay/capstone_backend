using Ext_Dynamics_API.Canvas;
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
using System.Data;
using Ext_Dynamics_API.Canvas.Models;

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

        /// <summary>
        /// Returns the list of custom data columns related with the Canvas Gradebook of a particular course
        /// </summary>
        /// <param name="accessToken">The Canvas Personal Access Token being used</param>
        /// <param name="courseId">The id of the course</param>
        /// <returns>List&lt;CourseDataColumn>: Custom Columns of Canvas Gradebook</returns>
        public List<CourseDataColumn> GetCustomDataColumns(string accessToken, int courseId)
        {
            var custDataColumns = new List<CourseDataColumn>();
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
                        CourseDataColumn newKnownCol;
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
                    var newCol = new CourseDataColumn
                    {
                        Name = canvasCol.Title,
                        RelatedDataId = canvasCol.Id,
                        ColumnType = ColumnType.Custom_Canvas_Column,
                    };
                    var newColRows = GetCustomColumnDataRowsForUnknownTypes(accessToken, courseId, newCol);
                    newCol.DataType = GetColumnDataType(newColRows[0].Value);
                    if (newColRows != null)
                    {
                        if(newColRows.Count > 0)
                        {

                        }
                    }

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

        /// <summary>
        /// Loads Derived Custom Columns for a particular course
        /// </summary>
        /// <param name="courseId">The id of the course</param>
        /// <param name="table">The table to load the columns into</param>
        /// <returns>List&lt;NumericDataColumn>: List of Derived columns in the Canvas gradebook</returns>
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
                    RelatedDataId = entry.RelatedDataId,
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

        /// <summary>
        /// Gets the datatype for a column that has been retreived from the Canvas Gradebook.
        /// </summary>
        /// <param name="content">Any value from any row in that particular column</param>
        /// <returns>ColumnDataType: An enumerable which specifies whether the column is a numeric or string column</returns>
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

        /// <summary>
        /// Adds a new custom column to the Canvas gradebook
        /// </summary>
        /// <param name="accessToken">The Canvas access token of the user</param>
        /// <param name="column">The column to add to the Canvas Gradebook</param>
        /// <param name="courseId">The id of the course to add the column to</param>
        /// <param name="userId">The id of the user in <b>this</b> system</param>
        /// <param name="table">The table to add the custom column to</param>
        /// <param name="csvFileContent">The contents of the csv file (if applicable) as a string</param>
        /// <returns>bool: Whether or not this operation succeeded</returns>
        public bool AddNewColumn(string accessToken, CourseDataColumn column, int courseId, 
            int userId, CourseDataTable table, string csvFileContent)
        {
            return column.ColumnType switch
            {
                ColumnType.Custom_Canvas_Column => AddCustomColumn(accessToken, column, courseId, userId),
                ColumnType.Derived_Data => AddDerivedColumn(accessToken, column, courseId, userId, table),
                ColumnType.File_Import => AddFileColumn(accessToken, column, courseId, userId, csvFileContent),
                _ => false,
            };
        }

        /// <summary>
        /// Determines whether or not a column with the specified name exists
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="updatedColId">
        /// An optional parameter indicating the related id of a column to ignore if a match is made with it
        /// </param>
        /// <returns>bool: Whether or not a column with this name exists</returns>
        public bool IsColumnExists(string columnName, int updatedColId = -1)
        {
            return _dbCtx.CustomDataColumns.Where(x => x.Name.Equals(columnName) && x.RelatedDataId != updatedColId)
                .FirstOrDefault() != null;
        }

        /// <summary>
        /// Updates the custom column in the database and pushes any appropriate changes (such as column name) to Canvas
        /// </summary>
        /// <param name="accessToken">The API Key used to access Canvas APIs</param>
        /// <param name="column">The updated details for a column</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="userId">The id of the user in the system</param>
        /// <param name="table">The table to add the custom column</param>
        /// <param name="csvFileContent">The content of the uploaded csv file as a string (if applicable)</param>
        /// <returns>bool: Whether or not the update operation was successful</returns>
        public bool EditCustomColumn(string accessToken, CourseDataColumn column, int courseId, int userId, 
            CourseDataTable table, string csvFileContent)
        {
            return column.ColumnType switch
            {
                ColumnType.Custom_Canvas_Column => EditToCustomColumn(accessToken, column, courseId, userId),
                ColumnType.File_Import => EditToFileColumn(accessToken, column, courseId, userId, csvFileContent),
                ColumnType.Derived_Data => EditToDerivedColumn(accessToken, column, courseId, userId, table),
                _ => false,
            };
        }

        /// <summary>
        /// Deletes a custom column in the Canvas Gradebook for a specified course
        /// </summary>
        /// <param name="accessToken">The access token of a particular user</param>
        /// <param name="courseId">The id of the course in Canvas</param>
        /// <param name="relatedDataId">The id of the custom column (this is found in column.RelatedDataId)</param>
        /// <returns>bool: Whether or not the deletion was a success</returns>
        public bool DeleteCustomColumn(string accessToken, int courseId, int relatedDataId)
        {
            var column = _dbCtx.CustomDataColumns.Where(x => x.RelatedDataId == relatedDataId).FirstOrDefault();
            if(column != null)
            {
                try
                {
                    _canvasDataAccess.DeleteCustomColumn(accessToken, courseId, relatedDataId);
                }
                catch(Exception)
                {
                    return false;
                }

                _dbCtx.CustomDataColumns.Remove(column);
                _dbCtx.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Resets the values in a custom column to their default values given the datatype of the column
        /// </summary>
        /// <param name="accessToken">The API Key to use with Canvas APIs</param>
        /// <param name="courseId">The id of the course</param>
        /// <param name="column">The column to reset</param>
        /// <returns>bool: If the operation succeeded or not</returns>
        public bool ResetCustomColumn(string accessToken, int courseId, CourseDataColumn column)
        {
            var request = new CustomColumnsUpdateRequest()
            {
                ColumnData = new List<CustomColumnDataEntry>()
            };

            if(column.DataType == ColumnDataType.Number)
            {
                foreach (var student in Students)
                {
                    request.ColumnData.Add(new CustomColumnDataEntry
                    {
                        ColumnId = column.RelatedDataId,
                        UserId = student.Id,
                        Content = "0"
                    });
                }
            }
            else
            {
                foreach (var student in Students)
                {
                    request.ColumnData.Add(new CustomColumnDataEntry
                    {
                        ColumnId = column.RelatedDataId,
                        UserId = student.Id,
                        Content = ""
                    });
                }
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, request);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private bool AddCustomColumn(string accessToken, CourseDataColumn column, int courseId, int userId)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name,
                Hidden = false,
                TeacherNotes = false,
                ReadOnly = false
            };

            CustomColumn newCol;
            
            try
            {
                newCol = _canvasDataAccess.AddNewCustomColumn(accessToken, request, courseId);
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
                RelatedDataId = newCol.Id,
                UserId = userId,
                ColumnType = column.ColumnType,
            };

            _dbCtx.CustomDataColumns.Add(newEntry);

            var success = SaveColumnEntriesChangesConcurently(newEntry);

            if(!success)
            {
                try
                {
                    _canvasDataAccess.DeleteCustomColumn(accessToken, courseId, newCol.Id);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return success;
        }

        private bool AddDerivedColumn(string accessToken, CourseDataColumn column, int courseId, int userId, CourseDataTable table)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name,
                Hidden = false,
                TeacherNotes = false,
                ReadOnly = true // Derived columns are not to be edited manually
            };

            CustomColumn newCol;

            try
            {
                newCol = _canvasDataAccess.AddNewCustomColumn(accessToken, request, courseId);
            }
            catch (Exception)
            {
                return false;
            }

            var newEntry = new DataColumnEntry
            {
                Name = column.Name,
                CourseId = courseId,
                CalcRule = column.CalcRule,
                ColMaxValue = column.ColMaxValue,
                ColMinValue = column.ColMinValue,
                DataType = column.DataType,
                RelatedDataId = newCol.Id,
                UserId = userId,
                ColumnType = column.ColumnType,
            };

            _dbCtx.CustomDataColumns.Add(newEntry);

            var success = SaveColumnEntriesChangesConcurently(newEntry);

            if(!success)
            {
                try
                {
                    _canvasDataAccess.DeleteCustomColumn(accessToken, courseId, newCol.Id);
                }
                catch (Exception)
                {

                }
                return false;
            }

            // Be mindful that Derived Data Columns MUST be numeric (double datatype)
            var numCol = (NumericDataColumn)column;

            var derivedColManager = new DerivedColumnManager(table);
            derivedColManager.LoadDerivedDataColumn(ref numCol);

            var custColsRequest = new CustomColumnsUpdateRequest();
            custColsRequest.ColumnData = new List<CustomColumnDataEntry>();
            
            foreach(var row in numCol.Rows)
            {
                custColsRequest.ColumnData.Add(new CustomColumnDataEntry { 
                    ColumnId = newCol.Id,
                    Content = $"{row.Value}",
                    UserId = row.AssociatedUser.Id
                });
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, custColsRequest);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        private bool AddFileColumn(string accessToken, CourseDataColumn column, int courseId, int userId, string csvFileContent)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name,
                Hidden = false,
                TeacherNotes = false,
                ReadOnly = false
            };

            CustomColumn newCol;

            try
            {
                newCol = _canvasDataAccess.AddNewCustomColumn(accessToken, request, courseId);
            }
            catch (Exception)
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
                RelatedDataId = newCol.Id,
                UserId = userId,
                ColumnType = column.ColumnType,
            };

            _dbCtx.CustomDataColumns.Add(newEntry);

            var saveSuccessful = SaveColumnEntriesChangesConcurently(newEntry);

            if(!saveSuccessful)
            {
                try
                {
                    _canvasDataAccess.DeleteCustomColumn(accessToken, courseId, newCol.Id);
                }
                catch(Exception)
                {

                }
                return false;
            }

            CustomColumnsUpdateRequest updateRequest;

            try
            {
                var stringReader = new CsvStringReader();
                updateRequest = stringReader.CreateCanvasColumnRequestFromCsvString(csvFileContent, Students, newCol.Id);
            }
            catch(Exception)
            {
                return false;
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, updateRequest);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        private bool EditToCustomColumn(string accessToken, CourseDataColumn column, int courseId, int userId)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name,
                Hidden = false,
                TeacherNotes = false,
                ReadOnly = false
            };

            CustomColumn updatedCol;

            try
            {
                updatedCol = _canvasDataAccess.EditCustomColumn(accessToken, request, courseId, column.RelatedDataId);
            }
            catch (Exception)
            {
                return false;
            }

            var entryToUpdate = _dbCtx.CustomDataColumns.Where(x => x.RelatedDataId == column.RelatedDataId).FirstOrDefault();

            if(entryToUpdate != null)
            {
                entryToUpdate.Name = column.Name;
                entryToUpdate.CalcRule = "";
                entryToUpdate.ColMaxValue = column.ColMaxValue;
                entryToUpdate.ColMinValue = column.ColMinValue;
                entryToUpdate.ColumnType = column.ColumnType;
                entryToUpdate.DataType = column.DataType;

                var success = SaveColumnEntriesChangesConcurently(entryToUpdate);

                return true;
            }
            else
            {
                var newEntry = new DataColumnEntry
                {
                    Name = column.Name,
                    CourseId = courseId,
                    CalcRule = "",
                    ColMaxValue = column.ColMaxValue,
                    ColMinValue = column.ColMinValue,
                    DataType = column.DataType,
                    RelatedDataId = updatedCol.Id,
                    UserId = userId,
                    ColumnType = column.ColumnType,
                };

                _dbCtx.CustomDataColumns.Add(newEntry);

                var success = SaveColumnEntriesChangesConcurently(newEntry);

                return success;
            }
        }

        private bool EditToDerivedColumn(string accessToken, CourseDataColumn column, int courseId, int userId, CourseDataTable table)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name,
                Hidden = false,
                TeacherNotes = false,
                ReadOnly = false
            };

            CustomColumn updatedCol;

            try
            {
                updatedCol = _canvasDataAccess.EditCustomColumn(accessToken, request, courseId, column.RelatedDataId);
            }
            catch (Exception)
            {
                return false;
            }

            var entryToUpdate = _dbCtx.CustomDataColumns.Where(x => x.RelatedDataId == column.RelatedDataId).FirstOrDefault();

            if (entryToUpdate != null)
            {
                entryToUpdate.Name = column.Name;
                entryToUpdate.CalcRule = column.CalcRule;
                entryToUpdate.ColMaxValue = column.ColMaxValue;
                entryToUpdate.ColMinValue = column.ColMinValue;
                entryToUpdate.ColumnType = column.ColumnType;
                entryToUpdate.DataType = column.DataType;

                var success = SaveColumnEntriesChangesConcurently(entryToUpdate);

                if(!success)
                {
                    return false;
                }
            }
            else
            {
                var newEntry = new DataColumnEntry
                {
                    Name = column.Name,
                    CourseId = courseId,
                    CalcRule = column.CalcRule,
                    ColMaxValue = column.ColMaxValue,
                    ColMinValue = column.ColMinValue,
                    DataType = column.DataType,
                    RelatedDataId = updatedCol.Id,
                    UserId = userId,
                    ColumnType = column.ColumnType,
                };

                _dbCtx.CustomDataColumns.Add(newEntry);

                var success = SaveColumnEntriesChangesConcurently(newEntry);

                if(!success)
                {
                    return false;
                }
            }

            var resetResult = ResetCustomColumn(accessToken, courseId, column);

            // If the values failed to reset, then there is no point in continuing, it is best not to have muddled values in the column
            // from the old version to the new version
            if (!resetResult)
            {
                return false;
            }

            // Be mindful that Derived Data Columns MUST be numeric (double datatype)
            var numCol = (NumericDataColumn)column;

            var derivedColManager = new DerivedColumnManager(table);
            derivedColManager.LoadDerivedDataColumn(ref numCol);

            var custColsRequest = new CustomColumnsUpdateRequest
            {
                ColumnData = new List<CustomColumnDataEntry>()
            };

            foreach (var row in numCol.Rows)
            {
                custColsRequest.ColumnData.Add(new CustomColumnDataEntry
                {
                    ColumnId = updatedCol.Id,
                    Content = $"{row.Value}",
                    UserId = row.AssociatedUser.Id
                });
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, custColsRequest);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool EditToFileColumn(string accessToken, CourseDataColumn column, int courseId, int userId, string csvFileContent)
        {
            var request = new CustomColumnCreationRequest
            {
                Title = column.Name,
                Hidden = false,
                TeacherNotes = false,
                ReadOnly = false
            };

            CustomColumn updatedCol;

            try
            {
                updatedCol = _canvasDataAccess.EditCustomColumn(accessToken, request, courseId, column.RelatedDataId);
            }
            catch (Exception)
            {
                return false;
            }

            // If there is an error obtaining the updated column, cease this operation
            if(updatedCol.Id == 0)
            {
                return false;
            }

            var entryToUpdate = _dbCtx.CustomDataColumns.Where(x => x.RelatedDataId == column.RelatedDataId).FirstOrDefault();

            if (entryToUpdate != null)
            {
                entryToUpdate.Name = column.Name;
                entryToUpdate.CalcRule = "";
                entryToUpdate.ColMaxValue = column.ColMaxValue;
                entryToUpdate.ColMinValue = column.ColMinValue;
                entryToUpdate.ColumnType = column.ColumnType;
                entryToUpdate.DataType = column.DataType;

                var success = SaveColumnEntriesChangesConcurently(entryToUpdate);

                if (!success)
                {
                    return false;
                }
            }
            else
            {
                var newEntry = new DataColumnEntry
                {
                    Name = column.Name,
                    CourseId = courseId,
                    CalcRule = "",
                    ColMaxValue = column.ColMaxValue,
                    ColMinValue = column.ColMinValue,
                    DataType = column.DataType,
                    RelatedDataId = updatedCol.Id,
                    UserId = userId,
                    ColumnType = column.ColumnType,
                };

                _dbCtx.CustomDataColumns.Add(newEntry);

                var success = SaveColumnEntriesChangesConcurently(newEntry);

                if (!success)
                {
                    return false;
                }
            }

            var resetResult = ResetCustomColumn(accessToken, courseId, column);

            // If the values failed to reset, then there is no point in continuing, it is best not to have muddled values in the column
            // from the old version to the new version
            if (!resetResult)
            {
                return false;
            }

            CustomColumnsUpdateRequest updateRequest;

            try
            {
                var stringReader = new CsvStringReader();
                updateRequest = stringReader.CreateCanvasColumnRequestFromCsvString(csvFileContent, Students, updatedCol.Id);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, updateRequest);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves changes to the database in a concurrently safe way using Optimistic thread conflict resolution
        /// </summary>
        /// <param name="newEntry">The new entry to add or the updated column entry</param>
        /// <returns>bool: The result of the database save operation</returns>
        private bool SaveColumnEntriesChangesConcurently(DataColumnEntry newEntry) 
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

        private List<NumericDataRow> GetCustomDataRowsForNumberColumns(string accessToken, int courseId, CourseDataColumn column)
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
            return data;
        }

        private List<StringDataRow> GetCustomDataFromStringColumns(string accessToken, int courseId, CourseDataColumn column)
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

        private List<StringDataRow> GetCustomColumnDataRowsForUnknownTypes(string accessToken, int courseId, CourseDataColumn column)
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
                editSuccessful = EditCustomValues(courseId, accessToken, table);
                if(editSuccessful)
                {
                    editSuccessful = UpdateDerivedColumnValues(courseId, accessToken, table);
                }
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
                // Derived data columns cannot have their values changed manually, this is strictly managed by the system
                // If a user wishes to change the values, they may either change the formula (DataColumn.CalcRule) or
                // change the column type to a standard custom column
                if(column.ColumnType != ColumnType.Derived_Data)
                {
                    if (column.DataType == ColumnDataType.Number)
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
            }

            // This is important, if there is nothing to update, best to stop processing here!
            // If this update request gets through to Canvas it will act as a signal to reset all values for that particular
            // custom gradebook column on Canvas! Be wary of this!
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

        private bool UpdateDerivedColumnValues(int courseId, string accessToken, CourseDataTable table)
        {
            var updateReq = new CustomColumnsUpdateRequest();
            updateReq.ColumnData = new List<CustomColumnDataEntry>();

            foreach(var col in table.CustomDataColumns)
            {
                if(col.ColumnType == ColumnType.Derived_Data)
                {
                    var numCol = (NumericDataColumn)col;
                    foreach(var row in numCol.Rows)
                    {
                        updateReq.ColumnData.Add(new CustomColumnDataEntry { 
                            ColumnId = col.RelatedDataId,
                            Content = $"{row.NewValue}",
                            UserId = row.AssociatedUser.Id
                        });
                    }
                }
            }

            if(updateReq.ColumnData.Count == 0)
            {
                return true;
            }

            try
            {
                _canvasDataAccess.SetCustomColumnEntries(accessToken, courseId, updateReq);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        private List<CustomColumnDataEntry> GetUpdatedNumericRows(int courseId, string accessToken, List<NumericDataRow> rows,
            CourseDataColumn column)
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
            CourseDataColumn column)
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
