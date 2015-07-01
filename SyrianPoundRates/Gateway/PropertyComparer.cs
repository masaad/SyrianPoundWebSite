using System;
using System.Collections.Generic;
using System.Reflection;

namespace SyrianPoundRates.Gateway
{
    public class PropertyComparer<T> : IComparer<T>
    {
        private enum SortingOrder
        {
            ASC = 0,
            DESC   
        };

        private string _sortColumn = string.Empty;
        private SortingOrder _sortOrder = SortingOrder.ASC;
        private readonly string[] _sortTerms;

        public PropertyComparer(string sortExpression)
        {
            _sortTerms = sortExpression.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public int Compare(T x, T y)
        {
            int result = 0;
            foreach (string sortTerm in _sortTerms)
            {
                string[] parts = sortTerm.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                _sortColumn = parts[0];
                _sortOrder = (SortingOrder)Enum.Parse(typeof(SortingOrder), parts[1], true);

                PropertyInfo propertyInfo = typeof(T).GetProperty(_sortColumn);
                var object1 = (IComparable)propertyInfo.GetValue(x, null);
                var object2 = (IComparable)propertyInfo.GetValue(y, null);
                if (_sortOrder == SortingOrder.ASC)
                    result = object1.CompareTo(object2);
                else
                    result = object2.CompareTo(object1);

                if (result != 0) break;
            }
            return result;
        }
    }   
}
