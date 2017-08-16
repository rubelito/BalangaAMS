using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Data;
using BalangaAMS.Core.Domain.Enum;

namespace BalangaAMS.ApplicationLayer.HelperClass
{
    public class ReportTableConverter
    {
        private DataTable _reporTable;
        private readonly byte[] _checkImg;
        private readonly byte[] _lateImg;
        private readonly byte[] _crossImg;
        private readonly byte[] _naImg;
        private readonly byte[] _blankImg;

        public ReportTableConverter()
        {
            string imagelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                   ConfigurationManager.AppSettings["systemimagedirectory"];
            _checkImg = File.ReadAllBytes(imagelocation + ConfigurationManager.AppSettings["checkImg"]);
            _lateImg = File.ReadAllBytes(imagelocation + ConfigurationManager.AppSettings["lateImg"]);
            _crossImg = File.ReadAllBytes(imagelocation + ConfigurationManager.AppSettings["crossImg"]);
            _naImg = File.ReadAllBytes(imagelocation + ConfigurationManager.AppSettings["naImg"]);
            _blankImg = File.ReadAllBytes(imagelocation + ConfigurationManager.AppSettings["blankImg"]);
        }

        public DataTable ConvertEnumTableReportToImageTable(DataTable reportTable)
        {
            if (IsTableIsNullOrEmpty(reportTable))
                throw new ArgumentNullException("Report table should not be null or empty");

            _reporTable = reportTable;
            DataTable newTable = StartConvertion();
            return newTable;
        }

            private DataTable StartConvertion()
            {
                DataTable clonedTable = _reporTable.Clone();   
                ConvertEnumColumnToByteArray(clonedTable);
                DataTable newTable = CopyAndConvertValue(clonedTable, _reporTable);
                return newTable;
            }

            private DataTable CopyAndConvertValue(DataTable clonedTable, DataTable originalTable)
            {
                foreach (DataRow row in originalTable.Rows)
                {
                    var newRow = clonedTable.NewRow();
                    foreach (DataColumn column in originalTable.Columns)
                    {
                        if (column.DataType == typeof(Enum))
                            newRow[column.ColumnName] = ConvertEnumValueToByteArrayImage(row, column);
                        else
                            newRow[column.ColumnName] = row[column];
                    }
                    clonedTable.Rows.Add(newRow);
                }
                return clonedTable;
            }

            private byte[] ConvertEnumValueToByteArrayImage(DataRow row, DataColumn column)
            {
                byte[] imgStatus = _blankImg;
                var dayStatus = (DayAttendanceStatus)row[column];
                switch (dayStatus)
                {
                    case DayAttendanceStatus.Present:
                    case DayAttendanceStatus.Active:
                        imgStatus = _checkImg;
                        break;
                    case DayAttendanceStatus.Late:
                        imgStatus = _lateImg;
                        break;
                    case DayAttendanceStatus.Absent:
                    case DayAttendanceStatus.Inactive:
                        imgStatus = _crossImg;
                        break;
                    case DayAttendanceStatus.NA:
                        imgStatus = _naImg;
                        break;
                    case DayAttendanceStatus.None:
                        imgStatus = _blankImg;
                        break;
                }
                return imgStatus;
            }

            private void ConvertEnumColumnToByteArray(DataTable clonedTable)
            {
                foreach (DataColumn column in clonedTable.Columns)
                {
                    if (column.DataType == typeof(Enum))
                        column.DataType = typeof(byte[]);
                }
            }

            private bool IsTableIsNullOrEmpty(DataTable reportTable)
            {
                return reportTable == null || reportTable.Rows.Count == 0;
            }
    }
}
