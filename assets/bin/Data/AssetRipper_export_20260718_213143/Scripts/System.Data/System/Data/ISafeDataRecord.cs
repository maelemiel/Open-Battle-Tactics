namespace System.Data
{
	internal interface ISafeDataRecord : IDataRecord
	{
		bool GetBooleanSafe(int i);

		byte GetByteSafe(int i);

		char GetCharSafe(int i);

		DateTime GetDateTimeSafe(int i);

		decimal GetDecimalSafe(int i);

		double GetDoubleSafe(int i);

		float GetFloatSafe(int i);

		short GetInt16Safe(int i);

		int GetInt32Safe(int i);

		long GetInt64Safe(int i);

		string GetStringSafe(int i);
	}
}
