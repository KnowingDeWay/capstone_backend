using Ext_Dynamics_API.Models.CustomTabModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.RequestModels
{
    public class NewColumnRequest
    {
        public CourseDataColumn NewColumn { get; set; }
        public string CsvFileContent { get; set; }

        public static NewColumnRequest GetRequestFromDynamicObject(dynamic request)
        {
            var newRequest = new NewColumnRequest();

            if(request.newColumn.dataType == ColumnDataType.Number)
            {
                var newCol = new NumericDataColumn
                {
                    Name = request.newColumn.name,
                    ColumnId = request.newColumn.columnId,
                    ColumnType = request.newColumn.columnType,
                    DataType = request.newColumn.dataType,
                    RelatedDataId = request.newColumn.relatedDataId,
                    CalcRule = request.newColumn.calcRule,
                    ColMaxValue = request.newColumn.colMaxValue,
                    ColMinValue = request.newColumn.colMinValue,
                    Rows = new List<NumericDataRow>()
                };
                newRequest.NewColumn = newCol;
            }
            else
            {
                var newCol = new StringDataColumn
                {
                    Name = request.newColumn.name,
                    ColumnId = request.newColumn.columnId,
                    ColumnType = request.newColumn.columnType,
                    DataType = request.newColumn.dataType,
                    RelatedDataId = request.newColumn.relatedDataId,
                    CalcRule = request.newColumn.calcRule,
                    ColMaxValue = request.newColumn.colMaxValue,
                    ColMinValue = request.newColumn.colMinValue,
                    Rows = new List<StringDataRow>()
                };
                newRequest.NewColumn = newCol;
            }

            return newRequest;
        }
    }
}
