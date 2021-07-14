using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Option{TValue, TFail}"/> and <see cref="EOption{TValue}"/> on illegal value access because of failure.</summary>
   [Serializable]
   public class OptionFailureException : AnyOptionException
   {
      /// <summary>Allows classes to construct and throw this exception with its default message.</summary>
      internal OptionFailureException( Type typeValue, Type typeFailure ) : base( $"Option<{typeValue.Name}, {typeFailure.Name}> is a failure!" ) { }

      /// <summary>Allows <see cref="Option{TValue, TFail}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of Option<TValue, TFail>." )]
      public OptionFailureException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionFailureException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected OptionFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}