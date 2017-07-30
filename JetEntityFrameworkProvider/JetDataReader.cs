﻿using System;
using System.Data.Common;
using System.Text;

namespace JetEntityFrameworkProvider
{
    class JetDataReader : DbDataReader
    {


        public JetDataReader(DbDataReader dataReader)
        {
            _wrappedDataReader = dataReader;
        }

        public JetDataReader(DbDataReader dataReader, int topCount, int skipCount)
        {
            _wrappedDataReader = dataReader;
            _topCount = topCount;
            for (int i = 0; i < skipCount; i++)
            {
                _wrappedDataReader.Read();
            }
        }


        private DbDataReader _wrappedDataReader;
        private readonly int _topCount = 0;
        private int _readCount = 0;

        public override void Close()
        {
            _wrappedDataReader.Close();
        }

        public override int Depth
        {
            get { return _wrappedDataReader.Depth; }
        }

        public override int FieldCount
        {
            get { return _wrappedDataReader.FieldCount; }
        }

        public override bool GetBoolean(int ordinal)
        {
            object booleanObject = _wrappedDataReader.GetValue(ordinal);
            if (booleanObject == null)
                throw new InvalidOperationException("Cannot cast null to boolean");
            if (booleanObject is bool)
                return _wrappedDataReader.GetBoolean(ordinal);
            else if (booleanObject is short)
                return ((short)booleanObject) != 0;
            else
                throw new InvalidOperationException(string.Format("Cannot convert {0} to boolean", booleanObject.GetType()));
        }

        public override byte GetByte(int ordinal)
        {
            return _wrappedDataReader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _wrappedDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return _wrappedDataReader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return _wrappedDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _wrappedDataReader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return _wrappedDataReader.GetDateTime(ordinal);
        }

        public virtual TimeSpan GetTimeSpan(int ordinal)
        {
            TimeSpan timeSpan = GetDateTime(ordinal) - JetConnection.TimeSpanOffset;

            return timeSpan;
        }

        public override decimal GetDecimal(int ordinal)
        {
            return _wrappedDataReader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return _wrappedDataReader.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return _wrappedDataReader.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return _wrappedDataReader.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return _wrappedDataReader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            // Fix for discussion https://jetentityframeworkprovider.codeplex.com/discussions/647028
            object value = _wrappedDataReader.GetValue(ordinal);
            if (value is byte[])
                return new Guid((byte[])value);
            else
                return _wrappedDataReader.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return _wrappedDataReader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            // Fix for discussion https://jetentityframeworkprovider.codeplex.com/discussions/647028
            object value = _wrappedDataReader.GetValue(ordinal);
            if (value is string)
            {
                byte[] buffer = Encoding.Unicode.GetBytes((string)value);
                int intValue = BitConverter.ToInt32(buffer, 0);
                return intValue;
            }
            else
                return _wrappedDataReader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return _wrappedDataReader.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return _wrappedDataReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return _wrappedDataReader.GetOrdinal(name);
        }

        public override System.Data.DataTable GetSchemaTable()
        {
            return _wrappedDataReader.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            return _wrappedDataReader.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            object getValue = _wrappedDataReader.GetValue(ordinal);

            // GetValue is called by EF on a DateTime only if the field is a TimeSpan
            // otherwise EF calls GetDateTime.
            // We can suppose that if the value is a DateTime then the EF type is a TimeSpan
            if (getValue is DateTime)
                return (DateTime) getValue - JetConnection.TimeSpanOffset;
            return getValue;
        }

        public override int GetValues(object[] values)
        {
            return _wrappedDataReader.GetValues(values);
        }

        public override bool HasRows
        {
            get { return _wrappedDataReader.HasRows; }
        }

        public override bool IsClosed
        {
            get { return _wrappedDataReader.IsClosed; }
        }

        public override bool IsDBNull(int ordinal)
        {
            if (_wrappedDataReader.IsDBNull(ordinal))
                return true;
            if (JetConnection.IntegerNullValue != null && ((int)JetConnection.IntegerNullValue).Equals(_wrappedDataReader.GetValue(ordinal)))
                return true;
            return false;
        }

        public override bool NextResult()
        {
            return _wrappedDataReader.NextResult();
        }

        public override bool Read()
        {
            _readCount++;
            if (_topCount != 0 && _readCount > _topCount)
                return false;

            return _wrappedDataReader.Read();
        }

        public override int RecordsAffected
        {
            get { return _wrappedDataReader.RecordsAffected; }
        }

        public override object this[string name]
        {
            get { return _wrappedDataReader[name]; }
        }

        public override object this[int ordinal]
        {
            get { return _wrappedDataReader[ordinal]; }
        }
    }
}
