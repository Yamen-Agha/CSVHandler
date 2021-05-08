using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CSVHandler.Exceptions;

namespace CSVHandler.Classes
{
    internal class Utils
    {
        internal static DataTable FillDtFromFile(string FileFullPath, char DataSaperator, CSVManager.DataFillingMode DataMode)
        {
            DataTable CSVDataDt = new DataTable();

            string[] AllLinesRaw = File.ReadAllLines(FileFullPath);

            //Remove the empty lines
            List<string> LinesList = new List<string>();
            foreach (string Line in AllLinesRaw)
                if (!string.IsNullOrWhiteSpace(Line))
                    LinesList.Add(Line);

            if (LinesList.Count == 0)
                throw new FileEmptyException(FileFullPath);

            string[] HeaderColumns = LinesList[0].Split(DataSaperator);
            int ColumnsCount = HeaderColumns.Length;

            if (ColumnsCount == 0)
                throw new Exceptions.CSVHeaderNotValidException(FileFullPath);

            foreach (string HeaderColumn in HeaderColumns)
                CSVDataDt.Columns.Add(HeaderColumn, typeof(object));

            for (int i = 1; i < LinesList.Count; i++)
            {
                string[] RowValues = LinesList[i].Split(DataSaperator);

                bool ValidData = false;

                if (RowValues.Length == 0)
                    ValidData = false;
                else
                {
                    switch (DataMode)
                    {
                        case CSVManager.DataFillingMode.IGNORE_ALL:
                            ValidData = true;
                            break;

                        case CSVManager.DataFillingMode.IGNORE_DECREASE_ONLY:

                            if (RowValues.Length > ColumnsCount)
                                ValidData = false;
                            else
                                ValidData = true;
                            break;

                        case CSVManager.DataFillingMode.IGNORE_INCREASIG_ONLY:

                            if (RowValues.Length < ColumnsCount)
                                ValidData = false;
                            else
                                ValidData = true;

                            break;

                        case CSVManager.DataFillingMode.FORCE_MATCH_ALL:

                            if (RowValues.Length != ColumnsCount)
                                ValidData = false;
                            else
                                ValidData = true;

                            break;

                        default:
                            ValidData = false;
                            break;
                    }
                }

                if (!ValidData)
                    throw new Exceptions.CSVDataNotValidException(FileFullPath);

                DataRow NewDataRow = CSVDataDt.NewRow();
                for (int j = 0; j < RowValues.Length; j++)
                    NewDataRow[j] = RowValues[j];

                CSVDataDt.Rows.Add(NewDataRow);
            }

            CSVDataDt.AcceptChanges();

            return CSVDataDt;
        }

        internal static bool SaveDataTableChanges(DataTable CSVDataTable, string FileFullPath, char DataSaperator)
        {
            try
            {
                List<string> AddedLinesList = new List<string>();
                List<DataRow> result;


                if (!File.Exists(FileFullPath) || string.IsNullOrWhiteSpace(File.ReadAllText(FileFullPath)))
                {
                    string Header = "";

                    int Index = 0;
                    foreach (DataColumn Col in CSVDataTable.Columns)
                    {
                        if (Index == 0)
                            Header += Col.Caption;
                        else
                            Header += DataSaperator + Col.Caption;

                        Index++;
                    }

                    AddedLinesList.Add(Header);
                    result = CSVDataTable.AsEnumerable().ToList();
                }
                else
                    result = CSVDataTable.AsEnumerable().Where(dr => dr.RowState == DataRowState.Added).ToList();


                foreach (var AddedRow in result)
                {
                    string Line = "";

                    int Index = 0;
                    foreach (object Filed in AddedRow.ItemArray)
                    {
                        if (Index == 0)
                            Line += Filed.ToString();
                        else
                            Line += DataSaperator + Filed.ToString();

                        Index++;
                    }
                    AddedLinesList.Add(Line);
                }

                File.AppendAllLines(FileFullPath, AddedLinesList);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
