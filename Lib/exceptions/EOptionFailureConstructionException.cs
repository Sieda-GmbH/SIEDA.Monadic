using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EOption{TValue}"/> on illegal "failure" instance construction.</summary>
   [Serializable]
   public class EOptionFailureConstructionException : Exception
   {
      /// <summary>Allows <see cref="EOption{TValue}"/> to construct and throw this exception.</summary>
      internal EOptionFailureConstructionException( Type typeValue )
         : base( $"Illegal 'failure' given for EOption<{typeValue.Name}>, the exception must not be a 'null'-reference!" ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionFailureConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public EOptionFailureConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionFailureConstructionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected EOptionFailureConstructionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}