using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Failable{TValue, TFail}"/> on illegal value access because of failure.</summary>
   [Serializable]
   public class FailableFailureException : FailableException
   {
      /// <summary>Allows <see cref="Failable{TValue, TFail}"/> to construct and throw this exception with its default message.</summary>
      internal FailableFailureException( Type typeValue, Type typeFailure ) : base( $"Failable<{typeValue.Name}, {typeFailure.Name}> is a failure!" ) { }

      /// <summary>Allows <see cref="Failable{TValue, TFail}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of Failable<TValue, TFail>." )]
      public FailableFailureException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFailureException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected FailableFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}