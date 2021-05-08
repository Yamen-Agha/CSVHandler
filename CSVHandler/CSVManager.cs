using CSVHandler.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace CSVHandler
{
    public class CSVManager
    {

        /// <summary>
        /// Mode of filling the data
        /// 
        /// IGNORE_ALL = Ignore the increase and the decrease of data fields
        /// IGNORE_INCREASIG_ONLY = All fields that out of header columns will be ignored
        /// IGNORE_DECREASE_ONLY = If the data fields count is less than the header column count the last fields will be empty
        /// FORCE_MATCH_ALL = Exception will be thrown if fields count not matching the header column count
        /// </summary>
        public enum DataFillingMode
        {
            IGNORE_ALL,
            IGNORE_INCREASIG_ONLY,
            IGNORE_DECREASE_ONLY,
            FORCE_MATCH_ALL
        }

        public char DataSaperator { get; private set; }
        public DataTable CSVData { get; private set; }
        public string FullFilePath { get; private set; }
        public DataFillingMode DataMode { get; private set; }

        public CSVManager(string FullFilePath, char DataSaperator, DataFillingMode DataMode)
        {
            this.DataSaperator = DataSaperator;
            this.FullFilePath = FullFilePath;
            this.DataMode = DataMode;
            this.CSVData = Utils.FillDtFromFile(this.FullFilePath, this.DataSaperator, this.DataMode);
        }

        public CSVManager(string FullFilePath, char DataSaperator, DataFillingMode DataMode, DataTable Dt)
        {
            this.DataSaperator = DataSaperator;
            this.FullFilePath = FullFilePath;
            this.DataMode = DataMode;
            this.CSVData = Dt;
        }

        public CSVManager(string FilePath, string FileName, char DataSaperator, DataFillingMode DataMode) : this(Path.Combine(FilePath, FileName), DataSaperator, DataMode) { }

        public CSVManager(string FilePath, string FileName, char DataSaperator, DataFillingMode DataMode, DataTable Dt) : this(Path.Combine(FilePath, FileName), DataSaperator, DataMode, Dt) { }

        public void RefreshCSVData()
        {
            DataTable TempDt = Utils.FillDtFromFile(this.FullFilePath, this.DataSaperator, this.DataMode);
            this.CSVData.Rows.Clear();

            foreach (DataRow dr in TempDt.Rows)
                this.CSVData.Rows.Add(dr.ItemArray);

            this.CSVData.AcceptChanges();
        }

        public bool SaveChanges()
        {
            if (Utils.SaveDataTableChanges(this.CSVData, this.FullFilePath, this.DataSaperator))
            {
                this.CSVData.AcceptChanges();
                return true;
            }
            else
                return false;
        }
    }
}
