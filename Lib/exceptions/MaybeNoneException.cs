using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Maybe{TValue}"/> on illegal value access because of absent value.</summary>
   [Serializable]
   public class MaybeNoneException : AnyMaybeException
   {
      /// <summary>Allows <see cref="Maybe{TValue}"/> to construct and throw this exception with its default message.</summary>
      internal MaybeNoneException( Type typeValue ) : base( $"Maybe<{typeValue.Name}> has no value!" ) { }

      /// <summary>Allows <see cref="Maybe{TValue}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of Maybe<TValue>." )]
      public MaybeNoneException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public MaybeNoneException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public MaybeNoneException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected MaybeNoneException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}