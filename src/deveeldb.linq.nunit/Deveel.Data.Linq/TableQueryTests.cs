﻿using System;
using System.Linq;

using Deveel.Data.Mapping;
using Deveel.Data.Sql;
using Deveel.Data.Sql.Expressions;
using Deveel.Data.Sql.Objects;
using Deveel.Data.Sql.Statements;
using Deveel.Data.Sql.Tables;
using Deveel.Data.Sql.Types;

using NUnit.Framework;

namespace Deveel.Data.Linq {
	[TestFixture]
	public class TableQueryTests : ContextBasedTest {
		private QueryContext Context { get; set; }
		protected override void OnFixtureSetUp() {
			using (var session = Database.CreateUserSession(AdminUserName, AdminPassword)) {
				using (var query = session.CreateQuery()) {
					CreateTestTable(query);
					AddTestData(query);

					query.Session.Commit();
				}
			}
		}

		private void CreateTestTable(IQuery context) {
			var tableInfo = new TableInfo(ObjectName.Parse("APP.people"));
			var idColumn = tableInfo.AddColumn("id", PrimitiveTypes.Integer());
			idColumn.DefaultExpression = SqlExpression.FunctionCall("UNIQUEKEY",
				new SqlExpression[] { SqlExpression.Constant(tableInfo.TableName.FullName) });
			tableInfo.AddColumn("first_name", PrimitiveTypes.String());
			tableInfo.AddColumn("last_name", PrimitiveTypes.String());
			tableInfo.AddColumn("birth_date", PrimitiveTypes.DateTime());
			tableInfo.AddColumn("active", PrimitiveTypes.Boolean());

			context.Session.Access.CreateTable(tableInfo);
			context.Session.Access.AddPrimaryKey(tableInfo.TableName, "id", "PK_PEOPLE_TABLE");
		}

		private void AddTestData(IQuery context) {
			var table = context.Access.GetMutableTable(ObjectName.Parse("APP.people"));
			var row = table.NewRow();

			// row.SetValue("id", Field.Integer(0));
			row.SetDefault(0, context);
			row.SetValue("first_name", Field.String("John"));
			row.SetValue("last_name", Field.String("Doe"));
			row.SetValue("birth_date", Field.Date(new SqlDateTime(1977, 01, 01)));
			row.SetValue("active", Field.Boolean(false));
			table.AddRow(row);

			row = table.NewRow();

			// row.SetValue("id", Field.Integer(1));
			row.SetDefault(0, context);
			row.SetValue("first_name", Field.String("Jane"));
			row.SetValue("last_name", Field.String("Doe"));
			row.SetValue("birth_date", Field.Date(new SqlDateTime(1978, 11, 01)));
			row.SetValue("active", Field.Boolean(true));
			table.AddRow(row);

			row = table.NewRow();

			// row.SetValue("id", Field.Integer(2));
			row.SetDefault(0, context);
			row.SetValue("first_name", Field.String("Roger"));
			row.SetValue("last_name", Field.String("Rabbit"));
			row.SetValue("birth_date", Field.Date(new SqlDateTime(1985, 05, 05)));
			row.SetValue("active", Field.Boolean(true));
			table.AddRow(row);

			context.Session.Commit();
		}

		protected override void OnSetUp(string testName) {
			Context = new TestQueryContext(Query);
		}

		[Test]
		public void FindById() {
			Person entity = null;
			Assert.DoesNotThrow(() => entity = Context.Table<Person>().FindById(1));
			Assert.IsNotNull(entity);
			Assert.AreEqual(1, entity.Id);
			Assert.AreEqual("John", entity.FirstName);
		}

		[Test]
		public void QueryById() {
			Person entity = null;
			Assert.DoesNotThrow(() => entity = Context.Table<Person>().FirstOrDefault(x => x.Id == 1));
			Assert.IsNotNull(entity);
			Assert.AreEqual(1, entity.Id);
			Assert.AreEqual("John", entity.FirstName);
		}

		class TestQueryContext : QueryContext {
			public TestQueryContext(IQuery context) 
				: base(context) {
			}

			protected override void OnBuildMap(MappingContext mappingContext) {
				mappingContext.Map<Person>()
					.ToTable("people");
				mappingContext.Map<Person>()
					.Member(person => person.Id)
					.HasName("id")
					.IsPrimaryKey();
				mappingContext.Map<Person>()
					.Member(person => person.FirstName)
					.HasName("first_name")
					.IsNotNull();
			}
		}

		#region Person

		class Person {
			public int Id { get; set; }

			public string FirstName { get; set; }
		}

		#endregion
	}
}
