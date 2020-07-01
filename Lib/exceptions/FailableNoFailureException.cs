using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Failable{TValue, TFail}"/> on illegal failure access.</summary>
   [Serializable]
   public class FailableNoFailureException : FailableException
   {
      /// <summary>Allows <see cref="Failable{TValue, TFail}"/> to construct and throw this exception.</summary>
      internal FailableNoFailureException( Type typeValue, Type typeFailure ) : base( $"Failable<{typeValue.Name}, {typeFailure.Name}> is not a failure!" ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableNoFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public FailableNoFailureException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableNoFailureException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected FailableNoFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}