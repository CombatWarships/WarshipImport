namespace WarshipConflictsAPI
{
	public class Conflict
	{
		public Conflict(ConflictSource source, string fieldName, string value)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or empty.", nameof(fieldName));
			}

			Source = source;
			FieldName = fieldName;
			Value = value;
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