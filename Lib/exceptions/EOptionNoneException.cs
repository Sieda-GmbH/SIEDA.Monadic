using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>Exception thrown exclusively by <see cref="EOption{TValue}"/> on illegal value access because of absent value.</summary>
   [Serializable]
   public class EOptionNoneException : EOptionException
   {
      /// <summary>Allows <see cref="EOption{TValue}"/> to construct and throw this exception with its default message.</summary>
      internal EOptionNoneException( Type typeValue ) : base( $"EOption<{typeValue.Name}> has no value!" ) { }

      /// <summary>Allows <see cref="EOption{TValue}"/> to construct and throw this exception with a custom message.</summary>
      [Obsolete( "Do not use outside of EOption<TValue>." )]
      public EOptionNoneException( string customMessage ) : base( customMessage ) { }

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionNoneException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public EOptionNoneException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected EOptionNoneException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}