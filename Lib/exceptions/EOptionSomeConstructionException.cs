using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EOption{TValue}"/> on illegal "some value" instance construction.</summary>
   [Serializable]
   public class EOptionSomeConstructionException : Exception
   {
      /// <summary>Allows <see cref="EFailable{TValue}"/> to construct and throw this exception.</summary>
      internal EOptionSomeConstructionException( Type typeValue )
         : base( $"Illegal 'some' given for EOption<{typeValue.Name}>, the value must not be a 'null'-reference!" ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionSomeConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public EOptionSomeConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionSomeConstructionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected EOptionSomeConstructionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}