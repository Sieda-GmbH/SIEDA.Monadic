using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Failable{TValue, TFail}"/> on illegal "success" instance construction.</summary>
   [Serializable]
   public class FailableSuccessConstructionException : FailableException
   {
      /// <summary>Allows <see cref="Failable{TValue, TFail}"/> to construct and throw this exception.</summary>
      internal FailableSuccessConstructionException( Type typeValue, Type typeFailure )
         : base( $"Illegal 'success' given for Failable<{typeValue.Name}, {typeFailure.Name}>, the success-value must not be a 'null'-reference!" ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableSuccessConstructionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableSuccessConstructionException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public FailableSuccessConstructionException( string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected FailableSuccessConstructionException( SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}