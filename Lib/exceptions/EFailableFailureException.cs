using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EFailable{TValue}"/> on illegal value access because of failure.</summary>
   [Serializable]
   public class EFailableFailureException : EFailableException
   {
      /// <summary>Allows <see cref="EFailable{TValue}"/> to construct and throw this exception with its default message.</summary>
      internal EFailableFailureException( Type typeValue ) : base( $"EFailable<{typeValue.Name}> is a failure!" ) { }

      /// <summary>Allows <see cref="EFailable{TValue}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of EFailable<TValue>." )]
      public EFailableFailureException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EFailableFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EFailableFailureException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected EFailableFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}