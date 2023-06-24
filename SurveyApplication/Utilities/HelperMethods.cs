using Microsoft.Data.SqlClient;

namespace SurveyApplication.Utilities
{
	public static class HelperMethods
	{

		public static int SafeGetInt(this SqlDataReader dataReader, int fieldIndex)
		{
			//int fieldIndex = dataReader.GetOrdinal(fieldName);
			return dataReader.IsDBNull(fieldIndex) ? 0 : dataReader.GetInt32(fieldIndex);
		}

		public static int? SafeGetNullableInt(this SqlDataReader dataReader, int fieldIndex)
		{
			//int fieldIndex = dataReader.GetOrdinal(fieldName);
			return dataReader.GetValue(fieldIndex) as int?;
		}

		public static string SafeGetString(this SqlDataReader dataReader, int fieldIndex)
		{
			//int fieldIndex = dataReader.GetOrdinal(fieldName);
			return dataReader.IsDBNull(fieldIndex) ? string.Empty : dataReader.GetString(fieldIndex);
		}

		public static DateTime? SafeGetNullableDateTime(this SqlDataReader dataReader, int fieldIndex)
		{
			//int fieldIndex = dataReader.GetOrdinal(fieldName);
			return dataReader.GetValue(fieldIndex) as DateTime?;
		}

		public static bool SafeGetBoolean(this SqlDataReader dataReader, string fieldName, bool v)
		{
			return SafeGetBoolean(dataReader, fieldName, false);
		}

		public static bool SafeGetBoolean(this SqlDataReader dataReader, int fieldIndex, bool defaultValue)
		{
			//int fieldIndex = dataReader.GetOrdinal(fieldName);
			return dataReader.IsDBNull(fieldIndex) ? defaultValue : dataReader.GetBoolean(fieldIndex);
		}
	}
}
