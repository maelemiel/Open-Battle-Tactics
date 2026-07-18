namespace MobageEditor
{
	public class Score
	{
		public string uid;

		public double scoreValue;

		public string displayValue;

		public int rank;

		public int userId { get; set; }

		public string value
		{
			get
			{
				return scoreValue.ToString();
			}
			set
			{
				scoreValue = double.Parse(value);
			}
		}
	}
}
