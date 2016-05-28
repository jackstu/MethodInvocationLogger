using System;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace MethodInvocationLogger.SampleWriters
{
	public class SqlDataWriter : ILogOutput<EventLogData>
	{
		private readonly string _connectionString;

		public SqlDataWriter(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void WriteLog(EventLogData eventLogData)
		{
			Guid eventGuid = SqlGuidUtil.NewSequentialId();

			using (TransactionScope transaction = new TransactionScope())
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					connection.Open();
					using (SqlCommand command = connection.CreateCommand())
					{
						command.CommandText = CreateInsertBatch(eventLogData);
						command.Parameters.Add(new SqlParameter("P_EventGuid", eventGuid));
						command.Parameters.Add(new SqlParameter("P_CreateDate", eventLogData.CreateDateTime));
						command.Parameters.Add(new SqlParameter("P_EventType", eventLogData.EventType));
						command.Parameters.Add(new SqlParameter("P_TenantId", eventLogData.TenantId));

						command.ExecuteNonQuery();
						transaction.Complete();
					}
				}
			}
		}

		private string CreateInsertBatch(EventLogData eventLogData)
		{
			StringBuilder sb = new StringBuilder();

			if (eventLogData.Count > 0)
			{
				foreach (var customValuePair in eventLogData)
				{
					sb.Append(CreateSingleInsert(customValuePair.Key, customValuePair.Value.ToString()));
				}
			}
			else
			{
				sb.Append(CreateSingleInsert(null, null));
			}

			return sb.ToString();
		}

		private string CreateSingleInsert(string dataKey, string dataValue)
		{
			return $@"INSERT INTO [dboDWH].[tEvents]([EventGuid],[CreateDate],[EventType],[TenantId],[DataKey],[DataValue]) VALUES(@P_EventGuid, @P_CreateDate, @P_EventType, @P_TenantId, {
					(string.IsNullOrWhiteSpace(dataKey) ? "NULL" : "N'" + dataKey + "'")}, {
					(string.IsNullOrWhiteSpace(dataValue) ? "NULL" : "N'" + dataValue + "'")})";
		}
	}
}
