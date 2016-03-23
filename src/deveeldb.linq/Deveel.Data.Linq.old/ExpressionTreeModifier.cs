﻿// 
//  Copyright 2010-2015 Deveel
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Linq;
using System.Linq.Expressions;

using Deveel.Data.Sql;

namespace Deveel.Data.Linq {
	class ExpressionTreeModifier : ExpressionVisitor {
		private IQueryable queryableContents;
		private readonly Type queryType;

		internal ExpressionTreeModifier(IQueryable contents, Type elementType) {
			queryableContents = contents;
			queryType = typeof (QueryableTable<>).MakeGenericType(elementType);
		}

		protected override Expression VisitConstant(ConstantExpression c) {
			if (c.Type == queryType)
				return Expression.Constant(queryableContents);

			return c;
		}
	}
}
