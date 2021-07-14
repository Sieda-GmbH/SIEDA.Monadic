using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Failable{TValue, TFail}"/> and <see cref="EFailable{TValue}"/> on illegal "failure" instance construction.</summary>
   [Serializable]
   public class FailableFailureConstructionException : AnyFailableException
   {
      /// <summary>Allows classes to construct and throw this exception.</summary>
      internal FailableFailureConstructionException( Type typeValue, Type typeFailure )
         : base( $"Illegal 'failure' given for Failable<{typeValue.Name}, {typeFailure.Name}>, the failure-value must not be a 'null'-reference!" ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFailureConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFailureConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableFailureConstructionException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected FailableFailureConstructionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}