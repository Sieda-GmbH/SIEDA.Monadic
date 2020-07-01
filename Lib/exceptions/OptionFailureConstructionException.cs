using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Option{TValue, TFail}"/> on illegal "failure" instance construction.</summary>
   [Serializable]
   public class OptionFailureConstructionException : OptionException
   {
      /// <summary>Allows <see cref="Option{TValue, TFail}"/> to construct and throw this exception.</summary>
      internal OptionFailureConstructionException( Type typeValue, Type typeFailure )
         : base( $"Illegal 'failure' given for Option<{typeValue.Name}, {typeFailure.Name}>, the failure-value must not be a 'null'-reference!" ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionFailureConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionFailureConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionFailureConstructionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected OptionFailureConstructionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}