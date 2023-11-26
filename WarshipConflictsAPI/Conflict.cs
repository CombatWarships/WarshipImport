namespace WarshipConflictsAPI
{
	public class Conflict
	{
		public Conflict(string fieldName, ConflictSource source)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or empty.", nameof(fieldName));
			}

			FieldName = fieldName;
			Source = source;
		}

		public string FieldName { get; set; }

		public string Value { get; set; }

		public ConflictSource Source { get; set; }

		public override string ToString()
		{
			return FieldName;
		}
	}
}