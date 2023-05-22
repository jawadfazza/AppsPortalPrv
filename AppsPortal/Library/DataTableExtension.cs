using AppsPortal.Library;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace AppsPortal.Extensions
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class DataTableSearchOperationNodeTypeAttribute : Attribute
    {
        public DataTableSearchOperationNodeTypeAttribute(ExpressionType nodeType, bool isBinary)
        {
            NodeType = nodeType;
            IsBinary = isBinary;
            StringComparisonMethod = StringManipulationMethod = String.Empty;
        }

        public DataTableSearchOperationNodeTypeAttribute(System.Linq.Expressions.ExpressionType nodeType,
                                                    bool isBinary,
                                                    String stringComparisonMethod = null,
                                                    String stringManipulationMethod = null)
        {
            NodeType = nodeType;
            IsBinary = isBinary;
            StringComparisonMethod = stringComparisonMethod;
            StringManipulationMethod = stringManipulationMethod;
        }

        public ExpressionType NodeType { get; set; }
        public Boolean IsBinary { get; set; }
        public String StringComparisonMethod { get; set; }
        public String StringManipulationMethod { get; set; }
    }
    [DataContract]
    [Flags]
    public enum DataTableSearchOperation
    {
        /// <summary>
        /// Equals
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.Equal, true, "", "ToUpper")]
        EQ = 1,
        /// <summary>
        /// Does not equal
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.NotEqual, true, "", "ToUpper")]
        NE = 2,
        /// <summary>
        /// Greater than
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.GreaterThan, true)]
        GT = 4,
        /// <summary>
        /// Greater than or equal to
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.GreaterThanOrEqual, true)]
        GE = 8,
        /// <summary>
        /// Less than
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.LessThan, true)]
        LT = 16,
        /// <summary>
        /// Less than or equal to
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.LessThanOrEqual, true)]
        LE = 32,
        /// <summary>
        /// Begins with
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsTrue, false, "StartsWith", "ToUpper")]
        BW = 64,
        /// <summary>
        /// Does not begin with
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsFalse, false, "StartsWith", "ToUpper")]
        BN = 128,
        /// <summary>
        /// Ends with
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsTrue, false, "EndsWith", "ToUpper")]
        EW = 256,
        /// <summary>
        /// Does not ends with
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsFalse, false, "EndsWith", "ToUpper")]
        EN = 512,
        /// <summary>
        /// Contains
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsTrue, false, "Contains", "ToUpper")]
        CN = 1024,
        /// <summary>
        /// Does not contain
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsFalse, false, "Contains", "ToUpper")]
        NC = 2048,
        /// <summary>
        /// IN
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsTrue, false, "Contains", "ToUpper")]
        IN = 4096,
        /// <summary>
        /// Not IN
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.IsFalse, false, "Contains", "ToUpper")]
        NI = 8196,
        /// <summary>
        /// Empty
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.Equal, true, "", "ToUpper")]
        EM = 16384,
        /// <summary>
        /// Not Empty
        /// </summary>
        [DataTableSearchOperationNodeType(ExpressionType.NotEqual, true, "", "ToUpper")]
        NM = 32768,

    }

    public class ExpressionOpFactory
    {
        private IOpConfig _config;
        public ExpressionOpFactory(IOpConfig config)
        {
            _config = config;
        }

        public ExpressionBehavior GetExpressionType(String value)
        {
            return _config[value];
        }
    }

    public class ExpressionBehavior
    {
        public ExpressionType ExpressionType { get; set; }
        public Boolean IsBinary { get; set; }
        public Boolean UseMethod { get; set; }
        public String Method { get; set; }
        public Boolean MethodResultCompareValue { get; set; }
    }

    public interface IOpConfig
    {
        IOpConfig Add(String value, ExpressionBehavior expressionType);
        ExpressionBehavior this[String exprName] { get; }
    }

    public class OpConfig : IOpConfig
    {
        private Dictionary<String, ExpressionBehavior> _expressionTypeMap;
        public OpConfig()
        {
            _expressionTypeMap = new Dictionary<string, ExpressionBehavior>();
        }
        public IOpConfig Add(string value, ExpressionBehavior expressionType)
        {
            _expressionTypeMap.Add(value, expressionType);
            return this;
        }
        public ExpressionBehavior this[String exprName]
        {
            get
            {
                if (_expressionTypeMap.ContainsKey(exprName))
                {
                    return _expressionTypeMap[exprName];
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class DataTableSearchOperationGroups
    {
        /// <summary>
        /// All operations
        /// </summary>
        public const DataTableSearchOperation Everything = DataTableSearchOperation.BN | DataTableSearchOperation.BW | DataTableSearchOperation.CN | DataTableSearchOperation.EN | DataTableSearchOperation.EQ
                                                        | DataTableSearchOperation.EW | DataTableSearchOperation.GE | DataTableSearchOperation.GT | DataTableSearchOperation.LE | DataTableSearchOperation.LT
                                                        | DataTableSearchOperation.NC | DataTableSearchOperation.NE;
        public const DataTableSearchOperation AllStrings = DataTableSearchOperation.BN | DataTableSearchOperation.BW | DataTableSearchOperation.CN | DataTableSearchOperation.EN | DataTableSearchOperation.EQ
                                                        | DataTableSearchOperation.EW | DataTableSearchOperation.NC | DataTableSearchOperation.NE;
        public const DataTableSearchOperation AllNumbers = DataTableSearchOperation.EQ | DataTableSearchOperation.GE | DataTableSearchOperation.GT
                                                        | DataTableSearchOperation.LE | DataTableSearchOperation.LT | DataTableSearchOperation.NE;
        public const DataTableSearchOperation AllDates = AllNumbers;

        public static string[] GetToLoweredStringArray(DataTableSearchOperation op)
        {
            return op.ToString()
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLower())
                        .ToArray();
        }

        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
        sealed class DataTableSearchOperationNodeTypeAttribute : Attribute
        {
            public DataTableSearchOperationNodeTypeAttribute(System.Linq.Expressions.ExpressionType nodeType, bool isBinary)
            {
                NodeType = nodeType;
                IsBinary = isBinary;
                StringComparisonMethod = StringManipulationMethod = String.Empty;
            }

            public DataTableSearchOperationNodeTypeAttribute(System.Linq.Expressions.ExpressionType nodeType,
                                                        bool isBinary,
                                                        String stringComparisonMethod = null,
                                                        String stringManipulationMethod = null)
            {
                NodeType = nodeType;
                IsBinary = isBinary;
                StringComparisonMethod = stringComparisonMethod;
                StringManipulationMethod = stringManipulationMethod;
            }

            public ExpressionType NodeType { get; set; }
            public Boolean IsBinary { get; set; }
            public String StringComparisonMethod { get; set; }
            public String StringManipulationMethod { get; set; }
        }
    }

    public static class SearchHelper
    {
        public static Expression<Func<T, bool>> CreateSearchPredicate<T>(DataTableFilters filters)
        {
            ExpressionOpFactory factory = Factory.Init();
            bool isGroupOperationAND = filters.GroupOperation == DataTableGroupSearchOperation.AND;
            //var predicate = isGroupOperationAND ? PredicateBuilder.True<T>() : PredicateBuilder.False<T>();
            //No need to check, we are always performing AND operand.
            var predicate = PredicateBuilder.New<T>(true);//.True<T>();
            if (filters.FilterRules != null)
            {
                foreach (var rule in filters.FilterRules)
                {
                    var operationAttribute = factory.GetExpressionType(rule.Operation);
                    //SKip if the properity not in the passed T 
                    PropertyInfo pi = typeof(T).GetProperty(rule.Field);
                    if (pi != null)
                    {
                        ParameterExpression lhsParam = Expression.Parameter(typeof(T));
                        List<string> MyStringList = rule.FieldData.Split(',').ToList();
                        //Process only for string type and clean any spaces
                        if (pi.PropertyType == typeof(string))
                        {
                            for (int i = 0; i < MyStringList.Count; i++)
                            {
                                MyStringList[i] = MyStringList[i].Trim();
                            }
                        }
                        //If properity is datetime then switch the value to UTC
                        if (pi.PropertyType == typeof(System.DateTime?))
                        {
                            rule.FieldData = new Portal().UTCTime(Convert.ToDateTime(rule.FieldData)).ToString();
                        }
                        Expression lhs = Expression.Property(lhsParam, pi);

                        Type u = Nullable.GetUnderlyingType(pi.PropertyType);
                        Expression rhs = null;
                        object cast;
                        if (string.IsNullOrEmpty(rule.FieldData))
                        {
                            rule.FieldData = null;
                        }
                        if (u != null)
                        {
                            cast = (rule.Operation.ToUpper() == "IN" || rule.Operation.ToUpper() == "NI") ? Convert.ChangeType(MyStringList, u) : Convert.ChangeType(rule.FieldData, u);
                        }
                        else
                        {
                            cast = (rule.Operation.ToUpper() == "IN" || rule.Operation.ToUpper() == "NI") ? Convert.ChangeType(MyStringList, typeof(List<string>)) : Convert.ChangeType(rule.FieldData, pi.PropertyType);
                        }
                        rhs = (rule.Operation.ToUpper() == "IN" || rule.Operation.ToUpper() == "NI") ? Expression.Constant(cast, typeof(List<string>)) : Expression.Constant(cast, pi.PropertyType);
                        Expression theOperation = null;
                        if (operationAttribute.UseMethod)
                        {
                            if (rule.Operation.ToUpper() == "IN" || rule.Operation.ToUpper() == "NI")
                            {
                                lhs = Expression.Call(rhs, operationAttribute.Method, null, lhs);
                            }
                            else
                            {
                                lhs = Expression.Call(lhs, operationAttribute.Method, null, rhs);
                            }
                        }
                        if (operationAttribute.IsBinary)
                        {
                            //Check if the value is string
                            try
                            {
                                var converted = (rhs.Type != lhs.Type) ? Expression.Convert(rhs, lhs.Type) : rhs;
                                if (rhs.Type == typeof(string))
                                {
                                    if (operationAttribute.ExpressionType == ExpressionType.GreaterThanOrEqual ||
                                        operationAttribute.ExpressionType == ExpressionType.GreaterThan ||
                                        operationAttribute.ExpressionType == ExpressionType.LessThanOrEqual ||
                                        operationAttribute.ExpressionType == ExpressionType.LessThan)
                                    {
                                        var method = rhs.Type.GetMethod("CompareTo", new[] { typeof(string) });
                                        var zero = Expression.Constant(0);

                                        lhs = Expression.Call(lhs, method, converted);
                                        theOperation = Expression.MakeBinary(operationAttribute.ExpressionType, lhs, zero);
                                    }
                                    else
                                    {
                                        theOperation = Expression.MakeBinary(operationAttribute.ExpressionType, lhs, rhs);
                                    }
                                }
                                if (theOperation == null)
                                    theOperation = Expression.MakeBinary(operationAttribute.ExpressionType, lhs, converted);
                            }
                            catch (Exception)
                            {
                                throw new InvalidOperationException(
                                    String.Format("Cannot convert value \"{0}\" of type \"{1}\" to field \"{2}\" of type \"{3}\"", rhs,
                                        lhs.Type, pi.Name, rhs.Type));
                            }
                        }
                        else  //TODO: need to fix this
                        {
                            theOperation = Expression.MakeBinary(operationAttribute.ExpressionType, lhs, Expression.Constant(operationAttribute.MethodResultCompareValue));
                        }
                        var theLambda = Expression.Lambda<Func<T, bool>>(theOperation, lhsParam);
                        if (isGroupOperationAND)
                        {
                            predicate = predicate.And(theLambda);
                        }
                        else
                        {
                            predicate = predicate.Or(theLambda);
                        }
                    }//End of if properity is not null
                }
            }
            return predicate;
        }

    
        private static DataTableSearchOperationNodeTypeAttribute ExtractOperationComparisonType(DataTableSearchOperation op)
        {
            //get the attribute and associated NodeType
            var memberInfo = typeof(DataTableSearchOperation).GetMember(op.ToString());
            object[] attrInfo = memberInfo[0].GetCustomAttributes(typeof(DataTableSearchOperationNodeTypeAttribute), false);
            var theAttribute = attrInfo[0] as DataTableSearchOperationNodeTypeAttribute;
            return theAttribute;
        }

        public static Type GetPropertyTypeOfPropertyName<T>(string propertyName)
        {
            PropertyInfo pi = typeof(T).GetProperty(propertyName);
            return pi.PropertyType;
        }

        public static Expression<Func<TObjectType, TTargetType>> GetOrderByClause<TObjectType, TTargetType>(string propertyName)
        {
            PropertyInfo pi = typeof(TObjectType).GetProperty(propertyName);
            ParameterExpression lhsParam = Expression.Parameter(typeof(TObjectType), "o");
            Expression orderBy = Expression.Property(lhsParam, pi);
            Expression conv = Expression.Convert(orderBy, typeof(object));

            var orderByLambda = Expression.Lambda<Func<TObjectType, TTargetType>>(conv, lhsParam);
            return orderByLambda;
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortColumn, string order)
        {
            // Dynamically creates a call like this: query.OrderBy(p =&gt; p.SortColumn)
            var parameter = Expression.Parameter(typeof(T), "p");

            string command = "OrderBy";

            if (order == "desc")
            {
                command = "OrderByDescending";
            }

            Expression resultExpression = null;
            if (sortColumn.StartsWith("Local"))
            {
                sortColumn = sortColumn.Replace("Local", "");
            }
            var property = typeof(T).GetProperty(sortColumn);
            // this is the part p.SortColumn
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            // this is the part p =&gt; p.SortColumn
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType },
               query.Expression, Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(resultExpression);
        }
    }

    public class Factory
    {
        public static ExpressionOpFactory Init()
        {
            OpConfig _config = new OpConfig();

            _config.Add("eq", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.Equal });
            _config.Add("ne", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.NotEqual });
            _config.Add("em", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.Equal });
            _config.Add("nm", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.NotEqual });
            _config.Add("gt", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.GreaterThan });
            _config.Add("ge", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.GreaterThanOrEqual });
            _config.Add("lt", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.LessThan });
            _config.Add("le", new ExpressionBehavior { IsBinary = true, ExpressionType = ExpressionType.LessThanOrEqual });
            _config.Add("bw", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = true, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "StartsWith" });
            _config.Add("bn", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = false, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "StartsWith" });
            _config.Add("ew", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = true, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "EndsWith" });
            _config.Add("en", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = false, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "EndsWith" });
            _config.Add("cn", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = true, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "Contains" });
            _config.Add("nc", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = false, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "Contains" });
            _config.Add("in", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = true, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "Contains" });
            _config.Add("ni", new ExpressionBehavior { IsBinary = false, MethodResultCompareValue = false, ExpressionType = ExpressionType.Equal, UseMethod = true, Method = "Contains" });
            ExpressionOpFactory _factory = new ExpressionOpFactory(_config);

            return _factory;
        }

    }

    public class DataTableRender
    {
        public string name { get; set; }
        public string data { get; set; }
        public Boolean? orderable { get; set; }
        public Boolean? searchable { get; set; }
        public int targets { get; set; }
        public int? responsivePriority { get; set; }
        public string width { get; set; }
        public string render { get; set; }
        public string defaultContent { get; set; }
        public string className { get; set; }
    }

    public class ConvertOptions
    {
        internal static DataTableOptions Fill(DataTableRecievedOptions rOpts)
        {
            DataTableOptions dto = new DataTableOptions
            {
                Draw = rOpts.draw,
                Start = rOpts.start,
                Length = rOpts.length
            };

            //Process Filter
            DataTableFilters dtf = new DataTableFilters();
            if (!String.IsNullOrWhiteSpace(rOpts.filters))
            {
                dtf = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTableFilters>(rOpts.filters);
            }
            dto.Filters = dtf;

            //Process Columns Class
            List<DataTableColumns> lc = new List<DataTableColumns>();

            for (int i = 0; i < rOpts.columns.Count; i++)
            {
                DataTableColumns dc = new DataTableColumns
                {
                    ColumnData = rOpts.columns[i]["data"],
                    ColumnName = rOpts.columns[i]["name"],
                    Orderable = Convert.ToBoolean(rOpts.columns[i]["orderable"]),
                    Searchable = Convert.ToBoolean(rOpts.columns[i]["searchable"]),
                };
                //dc.search = col["search"];
                lc.Add(dc);
            }


            dto.Columns = lc;

            //Process Global Search
            DataTableSearchingColumn dtsc = new DataTableSearchingColumn
            {
                Value = rOpts.search["value"],
                Regex = rOpts.search["regex"]
            };

            //Process Order
            //Important Note: 
            //DataTable support multi-column ordering, in this portal the user will be able to order by one column, 
            //becuase on the Tree Expression (Dynamic Linq) function it takes only one parameter
            DataTableOrderingColumn dtoc = new DataTableOrderingColumn
            {
                OrderBy = dto.Columns[Convert.ToInt32(rOpts.order[0]["column"])].ColumnData, //Convert the index directly to the column name
                OrderDirection = rOpts.order[0]["dir"]
            };
            dto.Order = dtoc;


            return dto;
        }
    }

    [DataContract]
    public class DataTableRecievedOptions
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string filters { get; set; }
        public List<Dictionary<string, string>> columns { get; set; }
        public Dictionary<string, string> search { get; set; }
        public List<Dictionary<string, string>> order { get; set; }
    }

    [DataContract]
    public class DataTableOptions
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableFilters Filters { get; set; }
        public List<DataTableColumns> Columns { get; set; }
        public DataTableSearchingColumn Search { get; set; }
        public DataTableOrderingColumn Order { get; set; }
    }

    [DataContract]
    public class DataTableOrderingColumn
    {
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; }
    }

    [DataContract]
    public class DataTableSearchingColumn
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    [DataContract]
    public class DataTableColumns
    {
        public string ColumnData { get; set; }
        public string ColumnName { get; set; }
        public Boolean Searchable { get; set; }
        public Boolean Orderable { get; set; }

        public DataTableSearchingColumn ColumnSearch { get; set; }
    }

    [DataContract]
    public class DataTableFilters
    {
        [DataMember(Name = "groupOp")]
        public DataTableGroupSearchOperation GroupOperation { get; set; }
        [DataMember(Name = "rules")]
        public List<DataTableFilterOptions> FilterRules { get; set; }
    }

    [DataContract]
    public class DataTableFilterOptions
    {
        [DataMember(Name = "field")]
        public string Field { get; set; }
        [DataMember(Name = "op")]
        public String Operation { get; set; }
        [DataMember(Name = "data")]
        public string FieldData { get; set; }
    }

    [DataContract]
    public enum DataTableGroupSearchOperation
    {
        AND = 1,
        OR = 2
    }


}