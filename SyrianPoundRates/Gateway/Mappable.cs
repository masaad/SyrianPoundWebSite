using System;

namespace SyrianPoundRates.Gateway
{
    /// <summary>
    /// This attribute is created to help in mapping object properties values to stored procedure parameters. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Mappable : Attribute
    {
        private readonly bool _isSpGetParameter;
        private readonly bool _isSpInsertUpdateParameter;
        private readonly string _spParameter = string.Empty;

        /// <summary>
        /// Maps a property value to a Stored procedure parameter using the same name as the property for the prameters 
        /// that the stored proecedures expects. ex: PropertyName passed as @PropertyName.
        /// This Will map the property to AddUpdate AND GET stored procedures. 
        /// </summary>
        public Mappable()
        {
            _isSpInsertUpdateParameter = true;
            _isSpGetParameter = true;             
        }

        /// <summary>
        /// Maps a property value to AddUpdate AND GET stored procedure parameter using the name provided by the user.        
        /// </summary>
        public Mappable(string spParameterName) : this()
        {
            _spParameter = spParameterName; 
        }

        /// <summary>
        /// Maps a property value to a Stored procedure parameter using the same name as the property for the prameters 
        /// that the stored proecedures expects. ex: PropertyName passed as @PropertyName.
        /// </summary>
        /// <param name="mapsTo">Determine if a property maps to an AddUpdate or Get stored procedure parameter</param>              
        public Mappable(PropertyMapsTo mapsTo)
        {
            if (mapsTo == PropertyMapsTo.Get)
            {
                _isSpGetParameter = true;
                _isSpInsertUpdateParameter = false; 
            }
            else if (mapsTo == PropertyMapsTo.AddUpdate)
            {
                _isSpInsertUpdateParameter = true;
                _isSpGetParameter = false; 
            }        
        }

        /// <summary>
        /// Maps a property value to the AddUpdate Stored procedure's parameter provided by the user. 
        /// ex: @spParameterName.
        /// </summary>
        /// <param name="mapsTo"></param>
        /// <param name="spParameterName"></param>
        public Mappable(PropertyMapsTo mapsTo, string spParameterName) : this(mapsTo)
        {          
            _spParameter = spParameterName; 
        }
       
        public bool IsSpInsertUpdateParameter
        {
            get { return _isSpInsertUpdateParameter; }
        }

        public bool IsSpGetParameter
        {
            get { return _isSpGetParameter; }
        }

        public string SpParameter
        {
            get { return _spParameter; }
        }
    }
}