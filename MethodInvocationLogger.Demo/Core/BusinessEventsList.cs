using System.Collections.Generic;

namespace MethodInvocationLogger.Demo.Controllers
{
	public class BusinessEventsList
	{
		private readonly List<BusinessEvent> _events = new List<BusinessEvent>();

		public IEnumerable<BusinessEvent> GetEvents()
		{
			return _events;
		}

		public void AddEvent(BusinessEvent @event)
		{
			_events.Add(@event);
		}
	}
}