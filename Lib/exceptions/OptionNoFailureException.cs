using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="Option{TValue, TFail}"/> and <see cref="EOption{TValue}"/> on illegal failure access.</summary>
   [Serializable]
   public class OptionNoFailureException : AnyOptionException
   {
      /// <summary>Allows classes to construct and throw this exception.</summary>
      internal OptionNoFailureException( Type typeValue, Type typeFailure ) : base( $"Option<{typeValue.Name}, {typeFailure.Name}> is not a failure!" ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionNoFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete( "DO NOT USE EXPLICITLY, exists only for serialization and similar usecases" )]
      public OptionNoFailureException( string message ) : base( message ) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionNoFailureException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected OptionNoFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}