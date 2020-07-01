using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EFailable{TValue}"/> on illegal "failure" instance construction.</summary>
   [Serializable]
   public class EFailableFailureConstructionException : EFailableException
   {
      /// <summary>Allows <see cref="EFailable{TValue}"/> to construct and throw this exception.</summary>
      internal EFailableFailureConstructionException( Type typeValue )
         : base( $"Illegal 'failure' given for EFailable<{typeValue.Name}>, the exception must not be a 'null'-reference!" ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EFailableFailureConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public EFailableFailureConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EFailableFailureConstructionException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected EFailableFailureConstructionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}