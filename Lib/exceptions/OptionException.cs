using System;
using System.Runtime.Serialization;

namespace SIEDA.Monadic
{
   /// <summary>
   /// Common superclass for exceptions thrown exclusively by <see cref="Option{TValue, TFail}"/>.
   /// Is abstract due to is generic nature, classes actually implementating this proper model conrete problems.
   /// </summary>
   /// <remarks> Useful for 'try-catch' clauses. </remarks>
   [Serializable]
   public abstract class OptionException : AnyOptionException
   {
      /// <summary> Abstract CTOR </summary>
      protected OptionException( string message ) : base( message ) {}

      #region constructors for serialization
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionException() : base() { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      public OptionException(string message, Exception innerException) : base(message, innerException) { }
      ///<summary>DO NOT CALL THIS</summary>
      [Obsolete("DO NOT USE EXPLICITLY, exists only for serialization and similar usecases")]
      protected OptionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion constructors for serialization
   }
}