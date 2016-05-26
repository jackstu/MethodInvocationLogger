using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MethodInvocationLogger
{
	public class ConfigurationValidationResult
	{
		private readonly List<string> _errors=new List<string>();
		public IEnumerable<string> Errors => _errors;
		public bool IsValid => !Errors.Any();

		public void AddError(string error)
		{
			_errors.Add(error);
		}

		public void CopyErrorsFrom(ConfigurationValidationResult validationResult)
		{
			validationResult._errors.ForEach(AddError);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			_errors.ForEach(e=>sb.AppendLine(e));
			return sb.ToString();
		}
	}
}