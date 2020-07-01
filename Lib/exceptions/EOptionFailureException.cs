using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EOption{TValue}"/> on illegal value access because of failure.</summary>
   [Serializable]
   public class EOptionFailureException : EOptionException
   {
      /// <summary>Allows <see cref="EOption{TValue}"/> to construct and throw this exception with its default message.</summary>
      internal EOptionFailureException( Type typeValue ) : base( $"EOption<{typeValue.Name}> is a failure!" ) { }

      /// <summary>Allows <see cref="EOption{TValue}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of EOption<TValue>." )]
      public EOptionFailureException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionFailureException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionFailureException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected EOptionFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}