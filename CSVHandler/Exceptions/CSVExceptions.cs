using System;
using ELang = CSVHandler.Exceptions.ExceptionsLocale;

namespace CSVHandler.Exceptions
{
    internal class CSVHeaderNotValidException : Exception
    {
        internal CSVHeaderNotValidException(string FileName) : base(string.Format(ELang.CSVHeaderNotValidExceptionMessage, FileName)) { }
    }

    internal class CSVDataNotValidException : Exception
    {
        internal CSVDataNotValidException(string FileName) : base(string.Format(ELang.CSVDataNotValidExceptionMessage, FileName)) { }
    }

    public class FileEmptyException : Exception
    {
        public FileEmptyException(string FileName) : base(string.Format(ELang.FileEmptyExceptionMessage, FileName)) { }
    }
}
