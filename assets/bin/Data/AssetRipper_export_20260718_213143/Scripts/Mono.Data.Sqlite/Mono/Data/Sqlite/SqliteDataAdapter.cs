using System;
using System.ComponentModel;
using System.Data.Common;

namespace Mono.Data.Sqlite
{
	[Designer("Microsoft.VSDesigner.Data.VS.SqlDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ToolboxItem("SQLite.Designer.SqliteDataAdapterToolboxItem, SQLite.Designer, Version=1.0.36.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139")]
	[DefaultEvent("RowUpdated")]
	public sealed class SqliteDataAdapter : DbDataAdapter
	{
		private static object _updatingEventPH = new object();

		private static object _updatedEventPH = new object();

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqliteCommand SelectCommand
		{
			get
			{
				return (SqliteCommand)base.SelectCommand;
			}
			set
			{
				base.SelectCommand = value;
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqliteCommand InsertCommand
		{
			get
			{
				return (SqliteCommand)base.InsertCommand;
			}
			set
			{
				base.InsertCommand = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue(null)]
		public new SqliteCommand UpdateCommand
		{
			get
			{
				return (SqliteCommand)base.UpdateCommand;
			}
			set
			{
				base.UpdateCommand = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue(null)]
		public new SqliteCommand DeleteCommand
		{
			get
			{
				return (SqliteCommand)base.DeleteCommand;
			}
			set
			{
				base.DeleteCommand = value;
			}
		}

		public event EventHandler<RowUpdatingEventArgs> RowUpdating
		{
			add
			{
				EventHandler<RowUpdatingEventArgs> eventHandler = (EventHandler<RowUpdatingEventArgs>)base.Events[_updatingEventPH];
				if (eventHandler != null && value.Target is DbCommandBuilder)
				{
					EventHandler<RowUpdatingEventArgs> eventHandler2 = (EventHandler<RowUpdatingEventArgs>)FindBuilder(eventHandler);
					if (eventHandler2 != null)
					{
						base.Events.RemoveHandler(_updatingEventPH, eventHandler2);
					}
				}
				base.Events.AddHandler(_updatingEventPH, value);
			}
			remove
			{
				base.Events.RemoveHandler(_updatingEventPH, value);
			}
		}

		public event EventHandler<RowUpdatedEventArgs> RowUpdated
		{
			add
			{
				base.Events.AddHandler(_updatedEventPH, value);
			}
			remove
			{
				base.Events.RemoveHandler(_updatedEventPH, value);
			}
		}

		public SqliteDataAdapter()
		{
		}

		public SqliteDataAdapter(SqliteCommand cmd)
		{
			SelectCommand = cmd;
		}

		public SqliteDataAdapter(string commandText, SqliteConnection connection)
		{
			SelectCommand = new SqliteCommand(commandText, connection);
		}

		public SqliteDataAdapter(string commandText, string connectionString)
		{
			SqliteConnection connection = new SqliteConnection(connectionString);
			SelectCommand = new SqliteCommand(commandText, connection);
		}

		internal static Delegate FindBuilder(MulticastDelegate mcd)
		{
			if ((object)mcd != null)
			{
				Delegate[] invocationList = mcd.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					if (invocationList[i].Target is DbCommandBuilder)
					{
						return invocationList[i];
					}
				}
			}
			return null;
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			EventHandler<RowUpdatingEventArgs> eventHandler = base.Events[_updatingEventPH] as EventHandler<RowUpdatingEventArgs>;
			if (eventHandler != null)
			{
				eventHandler(this, value);
			}
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			EventHandler<RowUpdatedEventArgs> eventHandler = base.Events[_updatedEventPH] as EventHandler<RowUpdatedEventArgs>;
			if (eventHandler != null)
			{
				eventHandler(this, value);
			}
		}
	}
}
