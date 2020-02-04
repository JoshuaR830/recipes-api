using System.Collections.Generic;

namespace recipe_api.Helpers
{
	public class ListConversionManager
	{
		public static string ConvertToString(List<string> list)
		{
			return string.Join("::", list);
		}

		public static List<string> ConvertToList(string @string)
		{
			return new List<string>(@string.Split("::"));
		}
	}
}
