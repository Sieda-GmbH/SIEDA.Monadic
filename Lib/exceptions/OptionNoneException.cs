using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Option{TValue, TFail}"/> on illegal value access because of absent value.</summary>
   [Serializable]
   public class OptionNoneException : OptionException
   {
      /// <summary>Allows <see cref="Option{TValue, TFail}"/> to construct and throw this exception with its default message.</summary>
      internal OptionNoneException( Type typeValue, Type typeFailure ) : base( $"Option<{typeValue.Name}, {typeFailure.Name}> has no value!" ) { }

      /// <summary>Allows <see cref="Option{TValue, TFail}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of Option<TValue, TFail>." )]
      public OptionNoneException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionNoneException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionNoneException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected OptionNoneException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}