using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Validation{TFail}"/> and <see cref="EValidation"/> on illegal "failure" access.</summary>
   [Serializable]
   public class ValidationNoFailureException : AnyValidationException
   {
      /// <summary>Allows <see cref="Validation{TFail}"/> to construct and throw this exception.</summary>
      internal ValidationNoFailureException( Type typeFailure ) : base( $"Validation<{typeFailure.Name}> is not a failure!" ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public ValidationNoFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public ValidationNoFailureException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public ValidationNoFailureException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected ValidationNoFailureException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}