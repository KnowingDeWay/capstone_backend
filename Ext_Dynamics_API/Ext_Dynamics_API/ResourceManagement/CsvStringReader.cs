using Ext_Dynamics_API.Canvas.RequestModels;
using Ext_Dynamics_API.Models.CustomTabModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResourceManagement
{
    public class CsvStringReader
    {
        public CsvStringReader()
        {

        }

        /// <summary>
        /// Creates a list of Bulk update Canvas entries from
        /// </summary>
        /// <param name="content">The string content from the csv file</param>
        /// <returns>CustomColumnsUpdateRequest: A "bulk update" request object to send to Canvas</returns>
        public CustomColumnsUpdateRequest CreateCanvasColumnRequestFromCsvString(string content, 
            List<DataTableStudent> students, int columnId)
        {
            var custColReq = new CustomColumnsUpdateRequest();
            custColReq.ColumnData = new List<CustomColumnDataEntry>();
            var csvLines = content.Split("\n");
            var headers = content.Split(",");
            for(int i = 1; i < csvLines.Length; i++)
            {
                var currLine = csvLines[i].Split(",");
                var student = students.Where(x => x.InstitutionId.Equals(currLine[0])).FirstOrDefault();
                if(student != null)
                {
                    var newEntry = new CustomColumnDataEntry
                    {
                        ColumnId = columnId,
                        Content = currLine[1],
                        UserId = student.Id
                    };
                    custColReq.ColumnData.Add(newEntry);
                }
            }
            return custColReq;
        }
    }
}
